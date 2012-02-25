using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using LightBridge.TwitterService.DataContracts;
using OAuth;

namespace LightBridge.TwitterService
{
    public delegate void OnStatusRecievedDelegate(Status status);

    internal class TwitterStreamReader
    {
        public OnStatusRecievedDelegate OnStatusRecieved { get; set; }

        private readonly XmlObjectSerializer m_jsonSerializer;

        //Config settings
        private readonly string m_consumerKey = ConfigurationManager.AppSettings["consumer-key"];
        private readonly string m_consumerSecret = ConfigurationManager.AppSettings["consumer-secret"];
        private readonly string m_accessToken = ConfigurationManager.AppSettings["access-token"];
        private readonly string m_accessTokenSecret = ConfigurationManager.AppSettings["access-token-secret"];
        private readonly string m_username = ConfigurationManager.AppSettings["user-name"];
        private readonly string m_password = ConfigurationManager.AppSettings["user-password"];
        private readonly string m_streamUrl = ConfigurationManager.AppSettings["stream-url"];

        private bool m_running;
        private Thread m_worker;

        public bool Running
        {
            get { return m_running;}
        }

        public TwitterStreamReader(XmlObjectSerializer jsonSerializer)
        {
            m_jsonSerializer = jsonSerializer;
        }

        public void ProcessStream()
        {
            if (!m_running)
            {
                m_running = true;
                m_worker = new Thread(ProcessStreamThread);
                m_worker.Start();
            }
            else
            {
                throw new Exception("Twitter stream is already being processed.");
            }
        }

        public void Abort()
        {
            if (m_running && m_worker != null)
            {
                m_running = false;
                m_worker.Abort();
            }
        }

        private string GetOAuthHeader()
        {
            //A third party OAuth library just to build a HTTP header with known access tokens? Yes, I am that lazy.
            OAuthRequest request = OAuthRequest.ForProtectedResource(m_streamUrl, m_consumerKey, m_consumerSecret, m_accessToken, m_accessTokenSecret);
            request.RequestUrl = m_streamUrl;
            return request.GetAuthorizationHeader();
        }

        private void ProcessResponseLine(string responseLine)
        {
            if (!String.IsNullOrEmpty(responseLine) && OnStatusRecieved != null)
            {
                var byteArray = Encoding.UTF8.GetBytes(responseLine);
                var stream = new MemoryStream(byteArray);
                OnStatusRecieved(m_jsonSerializer.ReadObject(stream) as Status);
            }
        }

        private void ProcessStreamThread()
        {
            var previousConnectionFailed = false;

            while (m_running)
            {
                HttpWebRequest webRequest = null;
                HttpWebResponse webResponse = null;
                StreamReader responseStream = null;

                var reconnectDelay = 3000;

                try
                {
                    webRequest = (HttpWebRequest)WebRequest.Create(m_streamUrl);
                    webRequest.Timeout = -1;
                    webRequest.Credentials = new NetworkCredential(m_username, m_password);
                    webRequest.Headers["Authorization"] = GetOAuthHeader();
                    webResponse = (HttpWebResponse)webRequest.GetResponse();

                    responseStream = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("utf-8"));
                    while (m_running)
                    {
                        ProcessResponseLine(responseStream.ReadLine());
                        previousConnectionFailed = false;
                    }
                }
                catch (WebException)
                {
                    //From the twitter API Pre-launch checklist:
                    //7.Backing off from failures: none for first disconnect,
                    //  seconds for repeated network (TCP/IP) level issues,
                    //  minutes for non-200 HTTP codes?

                    //I'm just going to keep it simple with a 3 second delay on the first error
                    //and a 3 minute delay after the second
                    if (previousConnectionFailed)
                    {
                        reconnectDelay = 180000;
                    }
                    else
                    {
                        reconnectDelay = 300;
                        previousConnectionFailed = true;
                    }
                }
                finally
                {
                    if (webRequest != null)
                    {
                        webRequest.Abort();
                    }
                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }
                    if (webResponse != null)
                    {
                        webResponse.Close();
                    }
                    Thread.Sleep(reconnectDelay);
                }
            }
        }
    }
}
