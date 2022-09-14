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
        private bool clientlessToClient = false;
        private bool m_ClientWaitingForData = false;
        private bool m_ClientWatingForFinish = false;
        private bool currentSwitchActive = true;
        private readonly object login = new object();
        private bool agentIsConnected = false;
        private Utility.SilkroadLocale locale = Utility.SilkroadLocale.VSRO;

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
                this.clientlessToClient = Program.main.checkBox_clientlessToClient.Checked;
                this.locale = (Utility.SilkroadLocale)Program.main.localeSave;
                Log.LogMsg("Agent Socket Success IP:" + this.ip);
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

                            //Log.LogMsg("AG [S->C] " + packet.Opcode.ToString("X4") + " " + bytes.Length + " " + packet.Encrypted.ToString() + Environment.NewLine + Utility.HexDump(bytes) + Environment.NewLine);

                            #region FakeClient
                            if (this.clientlessToClient)
                            {
                                if (packet.Opcode == 0x30D2)
                                {
                                    while (this.m_ClientWaitingForData == false && this.m_ClientWatingForFinish == false)
                                    {
                                        System.Threading.Thread.Sleep(1);
                                    }

                                    //Client is ready
                                    if (this.m_ClientWaitingForData)
                                    {
                                        this.currentSwitchActive = true;
                                        //Accept Teleport
                                        Packet respone = new Packet(0x34B6);
                                        this.ag_remote_security.Send(respone);

                                        this.m_ClientWatingForFinish = true;
                                        this.m_ClientWaitingForData = false;

                                        //!!! CrossThreading!!!
                                        Program.main.MainLog("Waiting to finish teleport");
                                    }
                                }

                                //Incoming CharacterData
                                if (packet.Opcode == 0x34A5)
                                {
                                    if (this.m_ClientWatingForFinish)
                                    {
                                        this.clientlessToClient = false;
                                        this.m_ClientWatingForFinish = false;

                                        //!!! CrossThreading!!!
                                        Program.main.MainLog("Successfully switched");
                                    }
                                }
                            }
                            #endregion

                            if (packet.Opcode != 0x5000 && packet.Opcode != 0x9000)
                            {
                                if(packet.Opcode == 0xA103)
                                {
                                    lock (this.login)
                                    {
                                        if (!this._a103Sent)
                                            this._a103Sent = true;
                                        else
                                            continue;
                                    }
                                }

                                if(!this.clientlessToClient)
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
                                byte[] bytes = packet.GetBytes();

                                if(!this.clientlessToClient || this.currentSwitchActive)
                                {
                                    TransferBuffer key = current2.Key;
                                    while (key.Offset != key.Size)
                                    {
                                        this.ag_remote_client.Blocking = true;
                                        int num6 = this.ag_remote_client.Send(key.Buffer, key.Offset, key.Size - key.Offset, SocketFlags.None);
                                        this.ag_remote_client.Blocking = false;
                                        key.Offset += num6;
                                        Thread.Sleep(1);
                                    }
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

                            //Log.LogMsg("AG [C->S] " + packet.Opcode.ToString("X4") + " " + bytes.Length + " " + packet.Encrypted.ToString() + Environment.NewLine + Utility.HexDump(bytes) + Environment.NewLine);

                            int pLength = bytes.Length;

                            if (this.clientlessToClient && packet.Opcode == 0x7007)
                            {
                                byte type = packet.ReadUInt8();
                                if (type == 0x02)
                                {
                                    Packet responseEndCS = new Packet(0xB001);
                                    responseEndCS.WriteUInt8(0x01);
                                    this.ag_local_security.Send(responseEndCS);

                                    this.m_ClientWaitingForData = true;
                                    this.currentSwitchActive = false;

                                    Program.main.MainLog("Use ReturnScroll to finish Clientless->Client");
                                }
                            }

                            if (packet.Opcode == 0x6103 && this.clientlessToClient && Program.main.localeSave == Utility.SilkroadLocale.TRSRO)
                            {
                                Packet p = new Packet(0xA103, true);
                                p.WriteUInt8(1);
                                p.WriteUInt8(3);
                                p.WriteUInt8(0);
                                this.ag_local_security.Send(p);
                                continue;
                            }

                            if(packet.Opcode == 0x7005)
                            {
                                //Prevent logout
                                continue;
                            }

                            if (packet.Opcode == 0x5000 || packet.Opcode == 0x9000 || packet.Opcode == 0x2001)
                            {
                                if (!this.ag_remote_client.Connected)
                                {
                                    lock (this.login)
                                    {
                                        if (!this.agentIsConnected)
                                        {
                                            string ip = "127.0.0.1";
                                            int port = Program.main.portSave;

                                            if (this.clientlessToClient || Program.main.mBot)
                                                port = Program.main.getFreeBotPort(true);

                                            if (Program.main.checkBox_manualPortOverride.Checked)
                                                port = Convert.ToInt32(Program.main.textBox_manualPortOverride.Text);

                                            if (port != 0)
                                            {
                                                this.agentIsConnected = true;
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
                                }

                                if (packet.Opcode == 0x2001 && this.clientlessToClient)
                                {
                                    Packet response = new Packet(0x2001);
                                    response.WriteAscii("AgentServer");
                                    response.WriteUInt8(0);
                                    response.Lock();
                                    this.ag_local_security.Send(response);
                                    if (Program.main.localeSave == Utility.SilkroadLocale.VSRO)
                                    {
                                        response = new Packet(0x2005, false, true);
                                        response.WriteUInt8(0x01);
                                        response.WriteUInt8(0x00);
                                        response.WriteUInt8(0x01);
                                        response.WriteUInt8(0xBA);
                                        response.WriteUInt8(0x02);
                                        response.WriteUInt8(0x05);
                                        response.WriteUInt8(0x00);
                                        response.WriteUInt8(0x00);
                                        response.WriteUInt8(0x00);
                                        response.WriteUInt8(0x02);
                                        response.Lock();
                                        this.ag_local_security.Send(response);

                                        response = new Packet(0x6005, false, true);
                                        response.WriteUInt8(0x03);
                                        response.WriteUInt8(0x00);
                                        response.WriteUInt8(0x02);
                                        response.WriteUInt8(0x00);
                                        response.WriteUInt8(0x02);
                                        response.Lock();
                                        this.ag_local_security.Send(response);
                                    }
                                }
                            }
                            else if(!this.clientlessToClient || this.currentSwitchActive)
                            {
                                if (packet.Opcode == 0x6103)
                                {
                                    lock (this.login)
                                    {
                                        if (!this._a103Sent)
                                        {
                                            this._a103Sent = true;
                                            if (Program.main.localeSave == Utility.SilkroadLocale.VSRO)
                                            {
                                                Packet p = new Packet(0xA103, true);
                                                p.WriteUInt8(1);
                                                this.ag_local_security.Send(p);
                                            }
                                            else if (Program.main.localeSave == Utility.SilkroadLocale.TRSRO || Program.main.localeSave == Utility.SilkroadLocale.ISRO)
                                            {
                                                Packet p = new Packet(0xA103, true);
                                                p.WriteUInt8(1);
                                                p.WriteUInt8(3);
                                                p.WriteUInt8(0);
                                                this.ag_local_security.Send(p);
                                            }
                                        }
                                    }
                                }
                                if(!this.clientlessToClient)
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

                            if (!this.clientlessToClient || (this.currentSwitchActive || packet.Opcode == 0xB001))
                            {
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
