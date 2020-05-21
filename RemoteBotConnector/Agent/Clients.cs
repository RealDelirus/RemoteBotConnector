using SilkroadSecurityApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RemoteBotConnector;

namespace Agents
{
    public class AgentClients
    {
        public Socket ag_local_client;
        public Socket ag_remote_client;
        public string Charname = "";
        private Security ag_local_security;
        private Security ag_remote_security;
        private TransferBuffer ag_remote_recv_buffer;
        private List<Packet> ag_remote_recv_packets;
        private List<KeyValuePair<TransferBuffer, Packet>> ag_remote_send_buffers;
        private TransferBuffer ag_local_recv_buffer;
        private List<Packet> ag_local_recv_packets;
        private List<KeyValuePair<TransferBuffer, Packet>> ag_local_send_buffers;
        public string ip = "";
        private bool local = true;
        private bool remote = true;
        private bool _a103Sent = false;

        private Thread localThread, remoteThread;

        public AgentClients(Socket tSocket)  
        {
            try
            {
                this.ag_local_client = tSocket;
                this.ag_local_security = new Security();
                this.ag_local_security.GenerateSecurity(true, true, true);
                this.ag_remote_security = new Security();
                this.ag_remote_recv_buffer = new TransferBuffer(4069, 0, 0);
                this.ag_local_recv_buffer = new TransferBuffer(4069, 0, 0);
                this.ag_local_client.Blocking = false;
                this.ag_local_client.NoDelay = true;
                this.ag_remote_client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.ip = IPAddress.Parse(((IPEndPoint)this.ag_local_client.RemoteEndPoint).Address.ToString()).ToString();
                Log.LogMsg("Socket Success IP:" + this.ip);
                localThread = new Thread(new ThreadStart(AgentLocalThread));
                localThread.Start();
            }
            catch (Exception e)
            {
                Log.LogMsg("Error creating socket!");
                Log.LogEx(e);
                this.Disconnect(true);
            }
        }

        public void Disconnect(bool showmsg)
        {
            try
            {
                if (showmsg)
                {
                    Log.LogMsg("Disconnected " + this.ip);
                }
                this.ag_remote_client.Close();
                this.ag_local_client.Close();
                this.local = false;
                this.remote = false;
                try
                {
                    localThread.Abort();
                    remoteThread.Abort();
                }
                catch(Exception x)
                {

                }
            }
            catch(Exception ex)
            {

            }
        }


        private void AgentRemoteThread()
        {
            while (this.remote)
            {
                try
                {
                    this.ag_remote_recv_buffer.Offset = 0;
                    SocketError socketError;
                    try
                    {
                        this.ag_remote_recv_buffer.Size = this.ag_remote_client.Receive(this.ag_remote_recv_buffer.Buffer, 0, this.ag_remote_recv_buffer.Buffer.Length, SocketFlags.None, out socketError);
                        if (socketError != SocketError.Success)
                        {
                            if (socketError != SocketError.WouldBlock)
                            {

                                this.Disconnect(false);
                                break;
                            }
                        }
                        else
                        {
                            if (!this.ag_remote_client.Connected || this.ag_remote_recv_buffer.Size <= 0)
                            {
                                this.Disconnect(true); 
                                break;
                            }
                            this.ag_remote_security.Recv(this.ag_remote_recv_buffer);
                        }
                    }
                    catch
                    {

                        this.Disconnect(false);
                    }

                    try
                    {
                        this.ag_remote_recv_packets = this.ag_remote_security.TransferIncoming();
                    }
                    catch
                    {

                        this.Disconnect(false);
                    }
                   
                    if (this.ag_remote_recv_packets != null)
                    {
                        foreach (Packet packet in this.ag_remote_recv_packets)
                        {
                            byte[] bytes = packet.GetBytes();
                            if (packet.Opcode != 0x5000 && packet.Opcode != 0x9000)
                            {
                                this.ag_local_security.Send(packet);
                            }
                            
                        }
                    }
                    #region Outgoing Packets
                    try
                    {
                            
                        this.ag_remote_send_buffers = this.ag_remote_security.TransferOutgoing();
                        if (this.ag_remote_send_buffers != null)
                        {
                            foreach (KeyValuePair<TransferBuffer, Packet> current2 in this.ag_remote_send_buffers)
                            {
                                Packet packet = current2.Value;
                                TransferBuffer key = current2.Key;
                                while (key.Offset != key.Size)
                                {
                                    this.ag_remote_client.Blocking = true;
                                    int num6 = this.ag_remote_client.Send(key.Buffer, key.Offset, key.Size - key.Offset, SocketFlags.None);
                                    this.ag_remote_client.Blocking = false;
                                    key.Offset += num6;
                                    Thread.Sleep(1);
                                }

                                byte[] bytes = packet.GetBytes();

                                //C->S

                                if(!this._a103Sent && packet.Opcode == 0x6103)
                                {
                                    this._a103Sent = true;
                                    Packet p = new Packet(0xA103, true);
                                    p.WriteUInt8(1);
                                    this.ag_local_security.Send(p);
                                }
                            }
                        }
                        Thread.Sleep(1);
                    }
                    catch(Exception e)
                    {
                        Log.LogEx(e);
                        this.Disconnect(true);
                    }
                    #endregion
                }
                catch(Exception e)
                { 
                    Log.LogEx(e);
                    this.Disconnect(false);
                }
                Thread.Sleep(5);
            }
        }
        
        private void AgentLocalThread()
        {
             try
             {
                while (this.local)
                {
                    this.ag_local_recv_buffer.Offset = 0;
                    SocketError socketError;
                    this.ag_local_recv_buffer.Size = this.ag_local_client.Receive(this.ag_local_recv_buffer.Buffer, 0, this.ag_local_recv_buffer.Buffer.Length, SocketFlags.None, out socketError);
                    if (socketError != SocketError.Success)
                    {
                        if (socketError != SocketError.ConnectionReset)
                        {
                            if (socketError != SocketError.WouldBlock)
                            {
                                this.Disconnect(true);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (!this.ag_local_client.Connected)
                        {
                            this.Disconnect(false);
                            break;
                        }
                        else if (this.ag_local_recv_buffer.Size < 0)
                        {
                            this.Disconnect(false);
                            break;
                        }
                        this.ag_local_security.Recv(this.ag_local_recv_buffer);
                    }

                    this.ag_local_recv_packets = this.ag_local_security.TransferIncoming();

                    if (this.ag_local_recv_packets != null)
                    {
                        foreach (Packet packet in this.ag_local_recv_packets)
                        {
                            #region Packets
                            byte[] bytes = packet.GetBytes();
                            int pLength = bytes.Length;

                            if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000 || packet.Opcode == 0x2001)
                            {
                                if (!this.ag_remote_client.Connected)
                                {
                                    string ip = "127.0.0.1";
                                    int port = Program.main.portSave;
                                    if (port != 0)
                                    {
                                        this.ag_remote_client.Connect(ip, port);
                                        this.ag_remote_client.Blocking = false;
                                        this.ag_remote_client.NoDelay = true;
                                        remoteThread = new Thread(new ThreadStart(AgentRemoteThread));
                                        remoteThread.Start();
                                    }
                                    else
                                    {
                                        Program.main.MainLog("Not free BotPort found - Agent");
                                    }
                                }
                            }
                            else
                            {
                                this.ag_remote_security.Send(packet);
                            }
                            #endregion
                        }
                    }
                    #region Outgoing Packetx
                    this.ag_local_send_buffers = this.ag_local_security.TransferOutgoing();
                    if (this.ag_local_send_buffers != null)
                    {
                        foreach (KeyValuePair<TransferBuffer, Packet> current in this.ag_local_send_buffers)
                        {
                            Packet packet = current.Value;
                            TransferBuffer key = current.Key;
                            while (key.Offset != key.Size)
                            {
                                this.ag_local_client.Blocking = true;
                                int num4 = this.ag_local_client.Send(key.Buffer, key.Offset, key.Size - key.Offset, SocketFlags.None);
                                this.ag_local_client.Blocking = false;
                                key.Offset += num4;
                            }
                        }
                    }
                    #endregion
                    Thread.Sleep(1);
                }
            }
            catch
            {
                this.Disconnect(false);
            }
        }
    }
}
