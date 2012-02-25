using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using LightBridge.TwitterService;
using LightBridge.TwitterService.DataContracts;
using System.Runtime.Serialization.Json;

namespace LightBridge
{
    public partial class LightGui : Form
    {
        private readonly TwitterStreamReader m_reader = new TwitterStreamReader(new DataContractJsonSerializer(typeof(Status)));
        private string m_port;

        public LightGui()
        {
            InitializeComponent();
            m_reader.OnStatusRecieved += OnStatusRecieve;

            var portNames = SerialPort.GetPortNames();
            cboComPorts.Items.AddRange(portNames);
        }

        private void OnStatusRecieve(Status status)
        {
            if (!String.IsNullOrEmpty(status.text))
            {
                var port = new SerialPort(m_port);
                var upperCaseStatus = status.text.ToUpper();

                using (port)
                {
                    port.Open();
                    if (upperCaseStatus.Contains("RED") || upperCaseStatus.Contains("GREEN") || upperCaseStatus.Contains("YELLOW"))
                    {
                        port.Write("C");

                        if (upperCaseStatus.Contains("RED"))
                        {
                            port.Write("R");
                        }

                        if (upperCaseStatus.Contains("GREEN"))
                        {
                            port.Write("G");
                        }

                        if (upperCaseStatus.Contains("YELLOW"))
                        {
                            port.Write("Y");
                        }
                    }
                    port.Close();
                }
            }
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (m_reader.Running)
            {
                m_reader.Abort();
                cboComPorts.Enabled = true;
                cmdConnect.Text = "Connect";
            }
            else
            {
                m_port = cboComPorts.Text;
                m_reader.ProcessStream();
                cboComPorts.Enabled = false;
                cmdConnect.Text = "Disconnect";
            }
        }

        private void LightGui_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_reader.Abort();
        }
    }
}
