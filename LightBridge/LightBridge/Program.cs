using System.ServiceProcess;

namespace LightBridge
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] servicesToRun = new ServiceBase[] { new LightBridge() };
            ServiceBase.Run(servicesToRun);
        }
    }
}
