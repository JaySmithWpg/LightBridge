using System.Runtime.Serialization.Json;
using System.ServiceProcess;
using System.Net;
using System.IO.Ports;
using LightBridge.TwitterService;
using LightBridge.TwitterService.DataContracts;

namespace LightBridge
{
    public partial class LightBridge : ServiceBase
    {
        private readonly TwitterStreamReader m_reader = new TwitterStreamReader(new DataContractJsonSerializer(typeof(Status)));

        public LightBridge()
        {
            InitializeComponent();
            ServicePointManager.Expect100Continue = false;
        }

        protected override void OnStart(string[] args)
        {
            m_reader.OnStatusRecieved += OnStatusRecieve;
            m_reader.ProcessStream();
        }

        private void OnStatusRecieve(Status status)
        {
            //TODO: Serial port interface
        }

        protected override void OnStop()
        {
            m_reader.Abort();
        }
    }
}
