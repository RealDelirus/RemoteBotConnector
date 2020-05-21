using RemoteBotConnector;

namespace Gateways
{
    public class Log
    {
        public static void LogMsg(string msg)
        {
            Program.main.MainLog(msg);
        }
    }
}
