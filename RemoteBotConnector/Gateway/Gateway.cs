using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Gateways
{
	internal class Gateway
    {
		public static int fakeGatewayPort = 30457;
        public static int fakeAgentPort = 30458;
		public static string realIP = "127.0.0.1";
		public static string fakeIP = "127.0.0.1";

		private static Socket gw_local_server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		public static void StartGateway()
        {
            try
            {
                using (StreamReader streamReader = new StreamReader("Gateway.txt", Encoding.UTF8))
                {
                    streamReader.ReadLine();
                    Gateway.fakeIP = streamReader.ReadLine();
                    streamReader.ReadLine();
                    Gateway.realIP = streamReader.ReadLine();
                    streamReader.ReadLine();
                    Gateway.fakeGatewayPort = Convert.ToInt32(streamReader.ReadLine());
                    streamReader.ReadLine();
                    Gateway.fakeAgentPort = Convert.ToInt32(streamReader.ReadLine());
                }

                Log.LogMsg("Config loaded!");
                Gateway.gw_local_server.Bind(new IPEndPoint(IPAddress.Parse(Gateway.fakeIP), Gateway.fakeGatewayPort));
                Gateway.gw_local_server.Listen(0);

                Log.LogMsg(Gateway.fakeIP + ":" + Gateway.fakeGatewayPort + " Listening for connections..");

                Thread thread = new Thread(new ThreadStart(Gateway.handleConnections));
                thread.Start();

            }
            catch (Exception ex)
            {
                Log.LogMsg(ex.ToString());
                Log.LogMsg("Error starting Gateway service" + Environment.NewLine);
            }


        }
		public static void handleConnections()
		{
			while (true)
			{
                try
                {
                    Socket tSocket = Gateway.gw_local_server.Accept();
                    Clients clients = new Clients(tSocket);
                    Thread.Sleep(5);
                }
                catch
                {
                }
			}
		}

	}
}
