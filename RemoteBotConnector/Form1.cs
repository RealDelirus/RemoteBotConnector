using Gateways;
using Agents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Collections;
using System.Management;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace RemoteBotConnector
{
    public partial class Form1 : Form
    {

        public int portSave = 0;
        public  Form1()
        {
            InitializeComponent();
            Program.main = this;
        }

        public void MainLog(string msg)
        {
            rtb_log.AppendText(msg + Environment.NewLine);
            rtb_log.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MainLog("RemoteBotConnector started");
            Gateway.StartGateway();
            Agent.StartAgent(); 
        }

        public int getFreeBotPort()
        {
            if (checkBox_manualPortOverride.Checked && textBox_manualPortOverride.Text != "")
            {
                int tmpport = 0;
                if (!int.TryParse(textBox_manualPortOverride.Text, out tmpport))
                    MainLog("Manual Port set failed, please only use a valid port from 1-65535");
                return tmpport;
            }

            int manualPID = 0;
            if(radioButton_manual.Checked)
            {
                if (listBox_botClients.SelectedItem != null)
                {
                    BotList bl = listBox_botClients.SelectedItem as BotList;
                    manualPID = bl.PID;
                }
                else
                {
                    manualPID = -1;
                }
            }

            int x = 0;
            int port = 0;

            foreach (Process procesInfo in Process.GetProcesses())
            {
                if (procesInfo.ProcessName == "phBot" && procesInfo.MainWindowTitle != "")
                {
                    int PID = procesInfo.Id;
                    if (radioButton_autoSelect.Checked || manualPID == 0 || manualPID == PID)
                    {
                        foreach (TcpRow tcpRow in ManagedIpHelper.GetExtendedTcpTable(true))
                        {
                            if (tcpRow.ProcessId != 0)
                            {
                                Process process = Process.GetProcessById(tcpRow.ProcessId);
                                if (process.ProcessName != "System")
                                {
                                    try
                                    {
                                        if (process.ProcessName.Contains("phBot") && process.Id == PID)
                                        {
                                            if ((tcpRow.LocalEndPoint.ToString().Contains("127.0.0.1")) && (!process.MainWindowTitle.Contains("Connected") || process.MainWindowTitle.Contains("N/A") || (checkBox_ignoreState.Checked && !radioButton_autoSelect.Checked)))
                                            {
                                                x++;
                                                port = 0;
                                                int.TryParse(tcpRow.LocalEndPoint.ToString().Replace("127.0.0.1:", ""), out port);
                                            }
                                        }
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                }
            }
            return port;
        }



        #region Managed IP Helper API

        public class TcpTable : IEnumerable<TcpRow>
        {
            #region Private Fields

            private IEnumerable<TcpRow> tcpRows;

            #endregion

            #region Constructors

            public TcpTable(IEnumerable<TcpRow> tcpRows)
            {
                this.tcpRows = tcpRows;
            }

            #endregion

            #region Public Properties

            public IEnumerable<TcpRow> Rows
            {
                get { return this.tcpRows; }
            }

            #endregion

            #region IEnumerable<TcpRow> Members

            public IEnumerator<TcpRow> GetEnumerator()
            {
                return this.tcpRows.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.tcpRows.GetEnumerator();
            }

            #endregion
        }

        public class TcpRow
        {
            #region Private Fields

            private IPEndPoint localEndPoint;
            private IPEndPoint remoteEndPoint;
            private TcpState state;
            private int processId;

            #endregion

            #region Constructors

            public TcpRow(IpHelper.TcpRow tcpRow)
            {
                this.state = tcpRow.state;
                this.processId = tcpRow.owningPid;

                int localPort = (tcpRow.localPort1 << 8) + (tcpRow.localPort2) + (tcpRow.localPort3 << 24) + (tcpRow.localPort4 << 16);
                long localAddress = tcpRow.localAddr;
                this.localEndPoint = new IPEndPoint(localAddress, localPort);

                int remotePort = (tcpRow.remotePort1 << 8) + (tcpRow.remotePort2) + (tcpRow.remotePort3 << 24) + (tcpRow.remotePort4 << 16);
                long remoteAddress = tcpRow.remoteAddr;
                this.remoteEndPoint = new IPEndPoint(remoteAddress, remotePort);
            }

            #endregion

            #region Public Properties

            public IPEndPoint LocalEndPoint
            {
                get { return this.localEndPoint; }
            }

            public IPEndPoint RemoteEndPoint
            {
                get { return this.remoteEndPoint; }
            }

            public TcpState State
            {
                get { return this.state; }
            }

            public int ProcessId
            {
                get { return this.processId; }
            }

            #endregion
        }

        public static class ManagedIpHelper
        {
            #region Public Methods

            public static TcpTable GetExtendedTcpTable(bool sorted)
            {
                List<TcpRow> tcpRows = new List<TcpRow>();

                IntPtr tcpTable = IntPtr.Zero;
                int tcpTableLength = 0;

                if (IpHelper.GetExtendedTcpTable(tcpTable, ref tcpTableLength, sorted, IpHelper.AfInet, IpHelper.TcpTableType.OwnerPidAll, 0) != 0)
                {
                    try
                    {
                        tcpTable = Marshal.AllocHGlobal(tcpTableLength);
                        if (IpHelper.GetExtendedTcpTable(tcpTable, ref tcpTableLength, true, IpHelper.AfInet, IpHelper.TcpTableType.OwnerPidAll, 0) == 0)
                        {
                            IpHelper.TcpTable table = (IpHelper.TcpTable)Marshal.PtrToStructure(tcpTable, typeof(IpHelper.TcpTable));

                            IntPtr rowPtr = (IntPtr)((long)tcpTable + Marshal.SizeOf(table.length));
                            for (int i = 0; i < table.length; ++i)
                            {
                                tcpRows.Add(new TcpRow((IpHelper.TcpRow)Marshal.PtrToStructure(rowPtr, typeof(IpHelper.TcpRow))));
                                rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(typeof(IpHelper.TcpRow)));
                            }
                        }
                    }
                    finally
                    {
                        if (tcpTable != IntPtr.Zero)
                        {
                            Marshal.FreeHGlobal(tcpTable);
                        }
                    }
                }

                return new TcpTable(tcpRows);
            }

            #endregion
        }

        #endregion

        #region P/Invoke IP Helper API

        /// <summary>
        /// <see cref="http://msdn2.microsoft.com/en-us/library/aa366073.aspx"/>
        /// </summary>
        public static class IpHelper
        {
            #region Public Fields

            public const string DllName = "iphlpapi.dll";
            public const int AfInet = 2;

            #endregion

            #region Public Methods

            /// <summary>
            /// <see cref="http://msdn2.microsoft.com/en-us/library/aa365928.aspx"/>
            /// </summary>
            [DllImport(IpHelper.DllName, SetLastError = true)]
            public static extern uint GetExtendedTcpTable(IntPtr tcpTable, ref int tcpTableLength, bool sort, int ipVersion, TcpTableType tcpTableType, int reserved);

            #endregion

            #region Public Enums

            /// <summary>
            /// <see cref="http://msdn2.microsoft.com/en-us/library/aa366386.aspx"/>
            /// </summary>
            public enum TcpTableType
            {
                BasicListener,
                BasicConnections,
                BasicAll,
                OwnerPidListener,
                OwnerPidConnections,
                OwnerPidAll,
                OwnerModuleListener,
                OwnerModuleConnections,
                OwnerModuleAll,
            }

            #endregion

            #region Public Structs

            /// <summary>
            /// <see cref="http://msdn2.microsoft.com/en-us/library/aa366921.aspx"/>
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct TcpTable
            {
                public uint length;
                public TcpRow row;
            }

            /// <summary>
            /// <see cref="http://msdn2.microsoft.com/en-us/library/aa366913.aspx"/>
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct TcpRow
            {
                public TcpState state;
                public uint localAddr;
                public byte localPort1;
                public byte localPort2;
                public byte localPort3;
                public byte localPort4;
                public uint remoteAddr;
                public byte remotePort1;
                public byte remotePort2;
                public byte remotePort3;
                public byte remotePort4;
                public int owningPid;
            }

            #endregion
        }

        #endregion

        #region WINApi
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        static string GetChildPrecesses(int parentId)
        {
            var query = "Select * From Win32_Process Where ParentProcessId = "
                    + parentId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            string pid = "";
            foreach (ManagementObject mo in processList)
            {
                if (mo["name"].ToString().Contains("sro_client"))
                {
                    pid = mo["ProcessId"].ToString();
                    break;
                }
            }

            return pid;
        }

        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(int dwDesiredAccess,
               bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(int hProcess, int lpBaseAddress,
          byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        private static IntPtr[] GetWindowHandlesForThread(int threadHandle)
        {
            _results.Clear();
            EnumWindows(WindowEnum, threadHandle);
            return _results.ToArray();
        }

        // enum windows
        private const uint WM_COMMAND = 0x0111;
        private const int BN_CLICKED = 245;
        private const int IDOK = 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);

        private delegate int EnumWindowsProc(IntPtr hwnd, int lParam);

        [DllImport("user32.Dll")]
        private static extern int EnumWindows(EnumWindowsProc x, int y);
        [DllImport("user32")]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowsProc callback, int lParam);
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        private static List<IntPtr> _results = new List<IntPtr>();

        private static int WindowEnum(IntPtr hWnd, int lParam)
        {
            int processID = 0;
            int threadID = GetWindowThreadProcessId(hWnd, out processID);
            if (threadID == lParam)
            {
                _results.Add(hWnd);
                EnumChildWindows(hWnd, WindowEnum, threadID);
            }
            return 1;
        }

        // get window text

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]

        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);
        private static string GetText(IntPtr hWnd)
        {
            int length = GetWindowTextLength(hWnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        // get richedit text 

        private const int GWL_ID = -12;
        private const int WM_GETTEXT = 0x000D;
        private const int WM_SETTEXT = 0x000C;

        [DllImport("User32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int index);
        [DllImport("User32.dll")]
        private static extern IntPtr SendDlgItemMessage(IntPtr hWnd, int IDDlgItem, int uMsg, int nMaxCount, StringBuilder lpString);
        [DllImport("User32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        private static StringBuilder GetEditText(IntPtr hWnd)
        {
            Int32 dwID = GetWindowLong(hWnd, GWL_ID);
            IntPtr hWndParent = GetParent(hWnd);
            StringBuilder title = new StringBuilder(128);
            SendDlgItemMessage(hWndParent, dwID, WM_GETTEXT, 128, title);
            return title;
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure? All open connections will get terminated", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
                e.Cancel = true;
            if (result == DialogResult.Yes)
            {
                Process.GetCurrentProcess().Close();
                System.Environment.Exit(0);
            }
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            listBox_botClients.Items.Clear();
            listBox_botClients.Refresh();


            foreach (Process procesInfo in Process.GetProcesses())
            {
                if (procesInfo.ProcessName == "phBot" && procesInfo.MainWindowTitle != "" && 
                    ((radioButton_autoSelect.Checked && (!procesInfo.MainWindowTitle.Contains("Connected") || procesInfo.MainWindowTitle.Contains("N/A"))) ||
                    (radioButton_manual.Checked && (!procesInfo.MainWindowTitle.Contains("Connected") || procesInfo.MainWindowTitle.Contains("N/A"))) ||
                    (checkBox_ignoreState.Checked)))
                {
                    listBox_botClients.Items.Add(new BotList(procesInfo.MainWindowTitle, procesInfo.Id, procesInfo, getFolder(procesInfo.Id)));      
                }
            }
        }

        private string getFolder(int PID)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\handle.exe"))
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = Environment.CurrentDirectory + "\\handle.exe";
                    process.StartInfo.Arguments = "-p " + PID.ToString(); // Note the /c command (*)
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.Start();
                    //* Read the output (or the error)
                    string allhandles = process.StandardOutput.ReadToEnd();

                    string[] lines = allhandles.Split(new[] { '\r', '\n' });
                    foreach (string s in lines)
                    {
                        if (s.Contains("Media.pk2"))
                            return (s.Substring(0, s.IndexOf("Media.pk2") - 1)).Substring(s.IndexOf("\\") - 2);
                    }
                }
                catch (Exception ex)
                {
                    MainLog("Handle receive error. Does handle.exe exists? - Error " + ex.ToString());
                }
            }

            return ""; 
        }

        private void radioButton_autoSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_autoSelect.Checked)
            {
                listBox_botClients.Enabled = false;
                checkBox_ignoreState.Checked = false;
                checkBox_ignoreState.Enabled = false;
            }
        }

        private void radioButton_manual_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_manual.Checked)
            {
                listBox_botClients.Enabled = true;
                checkBox_ignoreState.Enabled = true;
            }
        }

        private void checkBox_manualPortOverride_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
                textBox_manualPortOverride.Enabled = true;
            else
                textBox_manualPortOverride.Enabled = false;
        }
    }

    class BotList
    {
        public int PID = 0;
        public Process Process = null;
        public string Name;
        public string Folder;

        public BotList(string name,int pid, Process process, string folder)
        {
            this.Name = name;
            this.PID = pid;
            this.Process = process;
            this.Folder = folder;
        }

        public override string ToString()
        {
            return Name.ToString() + " - [" + Folder.ToString() + "]";
        }
    }
}
