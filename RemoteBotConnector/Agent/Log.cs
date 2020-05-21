using System;
using System.Threading;
using RemoteBotConnector;

namespace Agents
{
	public class Log
	{
        public static void LogEx(Exception x)
        {
            if (!(x is ObjectDisposedException) && !(x is ThreadAbortException))
            {
                Console.WriteLine("[{0}:{1}:{2}] {3}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, x.Message);
                Console.WriteLine("\t Source: {0}\t Target: {1}\n\tStackTrace:\n\t{2}\n\tData:\n\t{3}", x.Source, x.TargetSite, x.StackTrace, x.Data);
            }
        }
        public static void LogMsg(string msg)
        {
            Program.main.MainLog(msg);
        }
	}
}
