using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.IO;

namespace RemoteBotConnector
{
    public partial class NetworkSettings : Form
    {
        public NetworkSettings()
        {
            InitializeComponent();
        }

        private void comboBox_network_address_DropDown(object sender, EventArgs e)
        {
            comboBox_network_address.Items.Clear();
            comboBox_network_address.Items.Add("127.0.0.1");
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            comboBox_network_address.Items.Add(ip.Address.ToString());
                        }
                    }
                }
            }
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            using (StreamWriter streamWriter = new StreamWriter("Gateway.txt"))
            {
                streamWriter.WriteLine("//FAKEIP");
                streamWriter.WriteLine(comboBox_network_address.Text);
                streamWriter.WriteLine("//REALIP");
                streamWriter.WriteLine(comboBox_network_address.Text);
                streamWriter.WriteLine("//FAKE_GW_PORT");
                streamWriter.WriteLine(textBox_network_gwport.Text);
                streamWriter.WriteLine("//FAKE_AGENT_PORT");
                streamWriter.WriteLine(textBox_network_agport.Text);
            }

            using (StreamWriter streamWriter = new StreamWriter("Agent.txt"))
            {
                streamWriter.WriteLine("//FAKEIP");
                streamWriter.WriteLine(comboBox_network_address.Text);
                streamWriter.WriteLine("//FAKEAGENTPORT");
                streamWriter.WriteLine(textBox_network_agport.Text);
            }

            this.Close();
        }
    }
}
