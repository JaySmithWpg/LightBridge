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
        private string m_test;

        public LightGui()
        {
            InitializeComponent();
            m_reader.OnStatusRecieved += OnStatusRecieve;
        }

        private void OnStatusRecieve(Status status)
        {
            if (!String.IsNullOrEmpty(status.text))
            {
                var port = new SerialPort(m_test);
                var upperCaseStatus = status.text.ToUpper();

                using (port)
                {
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
                }
            }
        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (m_reader.Running)
            {
                m_reader.Abort();
            }
            else
            {
                m_test = cboComPorts.Text;
                m_reader.ProcessStream();
            }
        }

        private void LightGui_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_reader.Abort();
        }
    }
}
