using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RemoteBotConnector
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        /// 
        public static Form1 main { get; set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
