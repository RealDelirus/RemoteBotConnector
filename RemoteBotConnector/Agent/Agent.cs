using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Agents
{
    internal class Agent
    {
        private static Socket ag_local_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static int fakeagentport = 50002;
        public static string fakeip = "127.0.0.1";

        public static void StartAgent()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader("Agent.txt", Encoding.UTF8))
                {
                    streamReader.ReadLine();
                    Agent.fakeip = streamReader.ReadLine();
                    streamReader.ReadLine();
                    Agent.fakeagentport = Convert.ToInt32(streamReader.ReadLine());
                }
               
                Agent.ag_local_server.Bind(new IPEndPoint(IPAddress.Parse(Agent.fakeip), Agent.fakeagentport));
                Agent.ag_local_server.Listen(0);

                Log.LogMsg("Config loaded!");
                Log.LogMsg(Agent.fakeip +":"+ Agent.fakeagentport+" Listening for connections..");

                Thread thread = new Thread(new ThreadStart(Agent.handleConnections));
                thread.Start();
            }
            catch (Exception x)
            {
                Log.LogMsg(x.ToString());
                Log.LogMsg("Error starting Agent service");
                return;
            }
           
        }

        public static void handleConnections()
        {
            while (true)
            {
                try
                {
                    Socket tSocket = Agent.ag_local_server.Accept();
                    new AgentClients(tSocket);
                }
                catch (Exception e)
                {
                    Log.LogEx(e);
                }
                Thread.Sleep(5);
            }
        }
    }
}
