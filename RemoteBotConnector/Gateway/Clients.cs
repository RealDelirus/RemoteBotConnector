using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RemoteBotConnector;
using SilkroadSecurityApi;

namespace Gateways
{
	internal class Clients
	{
		public Socket gw_local_client;
		public Socket gw_remote_client;
		private Security gw_local_security;
		private Security gw_remote_security;
		private TransferBuffer gw_remote_recv_buffer;
		private List<Packet> gw_remote_recv_packets;
		private List<KeyValuePair<TransferBuffer, Packet>> gw_remote_send_buffers;
		private TransferBuffer gw_local_recv_buffer;
		private List<Packet> gw_local_recv_packets;
		private List<KeyValuePair<TransferBuffer, Packet>> gw_local_send_buffers;
		private string ip = "";
		private bool local = true;
		private bool remote = true;
		public Clients(Socket tSocket)
		{
            try
            {
                this.gw_local_client = tSocket;
                this.gw_local_security = new Security();
                this.gw_local_security.GenerateSecurity(true, true, true);
                this.gw_remote_security = new Security();
                this.gw_remote_recv_buffer = new TransferBuffer(4096, 0, 0);
                this.gw_local_recv_buffer = new TransferBuffer(4096, 0, 0);
                this.gw_local_client.Blocking = false;
                this.gw_local_client.NoDelay = true;
                this.gw_remote_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.ip = IPAddress.Parse(((IPEndPoint)this.gw_local_client.RemoteEndPoint).Address.ToString()).ToString();
                Log.LogMsg("IP Connected : " + this.ip);
                Thread thread = new Thread(new ThreadStart(this.GatewayLocalThread));
                thread.Start();
            }
            catch (Exception e)
            {
                Log.LogMsg("Error creating socket!");
                Log.LogMsg(e.ToString());
            }
		}
        Dictionary<string, bool> SecurityForChar = new Dictionary<string, bool>();
		private void GatewayRemoteThread()
		{
			try
			{
				while (this.remote)
				{
					this.gw_remote_recv_buffer.Offset = 0;
					SocketError socketError;
					this.gw_remote_recv_buffer.Size = this.gw_remote_client.Receive(this.gw_remote_recv_buffer.Buffer, 0, this.gw_remote_recv_buffer.Buffer.Length, SocketFlags.None, out socketError);
					if (socketError != SocketError.Success)
					{
						if (socketError != SocketError.WouldBlock)
						{
							this.CleanClient();
							break;
						}
					}
					else
					{
						if (!this.gw_remote_client.Connected || this.gw_remote_recv_buffer.Size <= 0)
						{
							this.CleanClient();
							break;
						}
						this.gw_remote_security.Recv(this.gw_remote_recv_buffer);
					}
					this.gw_remote_recv_packets = this.gw_remote_security.TransferIncoming();
					if (this.gw_remote_recv_packets != null)
					{
						foreach (Packet packet in this.gw_remote_recv_packets)
						{
							byte[] bytes = packet.GetBytes();
                            
							if (packet.Opcode != 0x5000 && packet.Opcode != 0x9000)
							{
                                if (packet.Opcode == 0xA102)
                                {   
                                    byte b = packet.ReadUInt8();
                                    if (b == 1)
                                    {
                                        
                                        uint value = packet.ReadUInt32();
                                        string text = packet.ReadAscii();
                                        uint port = packet.ReadUInt16();

                                        Log.LogMsg("Redirect Client!");
                                        Packet packet2 = new Packet(0xA102, true);
                                        packet2.WriteUInt8(b);
                                        packet2.WriteUInt32(value);
                                        packet2.WriteAscii(Gateway.realIP);
                                        packet2.WriteUInt16(Gateway.fakeAgentPort);
                                        this.gw_local_security.Send(packet2);
                                        
                                        
                                    }
                                }
                                if(packet.Opcode == 0xA103)
                                {
                                    Log.LogMsg("Connected");
                                }
								this.gw_local_security.Send(packet);
                                
							}
						}
					}
					this.gw_remote_send_buffers = this.gw_remote_security.TransferOutgoing();
					if (this.gw_remote_send_buffers != null)
					{
						foreach (KeyValuePair<TransferBuffer, Packet> current in this.gw_remote_send_buffers)
						{
							Packet packet = current.Value;
							TransferBuffer key = current.Key;
							while (key.Offset != key.Size)
							{
								this.gw_remote_client.Blocking = true;
								int num2 = this.gw_remote_client.Send(key.Buffer, key.Offset, key.Size - key.Offset, SocketFlags.None);
								this.gw_remote_client.Blocking = false;
								key.Offset += num2;
								Thread.Sleep(1);
							}
							byte[] bytes = packet.GetBytes();
						}
					}
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
                //Log.LogMsg("RemoteThread error, disconnecting " + this.ip + Environment.NewLine + "Error Message: " + ex.Message + Environment.NewLine + "Error Source: " + ex.Source);
				this.CleanClient();
			}
		}
        bool receivedSecurity;
		private void GatewayLocalThread()
		{
			try
			{
				while (this.local)
				{
					this.gw_local_recv_buffer.Offset = 0;
					SocketError socketError;
					this.gw_local_recv_buffer.Size = this.gw_local_client.Receive(this.gw_local_recv_buffer.Buffer, 0, this.gw_local_recv_buffer.Buffer.Length, SocketFlags.None, out socketError);
					if (socketError != SocketError.Success)
					{
						if (socketError != SocketError.WouldBlock)
						{
							this.CleanClient();
							break;
						}
					}
					else
					{
						if (!this.gw_local_client.Connected || this.gw_local_recv_buffer.Size <= 0)
						{
							this.CleanClient();
							break;
						}
						this.gw_local_security.Recv(this.gw_local_recv_buffer);
					}
					this.gw_local_recv_packets = this.gw_local_security.TransferIncoming();
					if (this.gw_local_recv_packets != null)
					{
						foreach (Packet packet in this.gw_local_recv_packets)
						{
							byte[] bytes = packet.GetBytes();
							if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000 || packet.Opcode == 0x2001)
							{
                                if (!this.gw_remote_client.Connected)
                                {
                                    string ip = "127.0.0.1";
                                    int port = Program.main.getFreeBotPort();
                                    if (port != 0)
                                    {
                                        Program.main.portSave = port;
                                        this.gw_remote_client.Connect(ip, port);
                                        this.gw_remote_client.Blocking = false;
                                        this.gw_remote_client.NoDelay = true;
                                        Thread thread = new Thread(new ThreadStart(this.GatewayRemoteThread));
                                        thread.Start();
                                    }
                                    else
                                    {
                                        Program.main.MainLog("Cannot find free Bot");
                                        return;
                                    }
                                }
							}
							else
							{
                                this.gw_remote_security.Send(packet);
                            }
						}
					}
					this.gw_local_send_buffers = this.gw_local_security.TransferOutgoing();
					if (this.gw_local_send_buffers != null)
					{
						foreach (KeyValuePair<TransferBuffer, Packet> current in this.gw_local_send_buffers)
						{
							Packet packet = current.Value;
							TransferBuffer key = current.Key;
							while (key.Offset != key.Size)
							{
								this.gw_local_client.Blocking = true;
								int num = this.gw_local_client.Send(key.Buffer, key.Offset, key.Size - key.Offset, SocketFlags.None);
								this.gw_local_client.Blocking = false;
								key.Offset += num;
							}
							byte[] bytes = packet.GetBytes();
						}
					}
					Thread.Sleep(1);
				}
			}
			catch (Exception ex)
			{
                Log.LogMsg("LocalThread error, disconnecting " + this.ip);
                //Log.LogMsg(ex.ToString());
				this.CleanClient();
			}
		}

		private void CleanClient()
		{
			try
			{
                Log.LogMsg("IP Disconnected: " + this.ip);
                this.gw_remote_client.Close();
                this.gw_local_client.Close();
				this.local = false;
				this.remote = false;
                this.ip = "";
				GC.SuppressFinalize(this);
			}
			catch
			{
				GC.SuppressFinalize(this);
			}
		}
	}
}
