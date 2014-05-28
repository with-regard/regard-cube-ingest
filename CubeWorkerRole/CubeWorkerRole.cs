using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WorkerRoleWithSBQueue1
{
    public class CubeWorkerRole : RoleEntryPoint
    {
        private readonly ManualResetEvent m_CompletedEvent = new ManualResetEvent(false);

        private CubeScubscriptionClient m_ScubscriptionClient;

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            m_ScubscriptionClient.Start();

            m_CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Regard.ServiceBus.ConnectionString");
            string topic = CloudConfigurationManager.GetSetting("Regard.ServiceBus.EventTopic");
            string subscriptionName = CloudConfigurationManager.GetSetting("Regard.ServiceBus.SubscriptionName");
            string cubeEndpoint = CloudConfigurationManager.GetSetting("Regard.Cube.Endpoint");

            m_ScubscriptionClient = new CubeScubscriptionClient(connectionString, topic, subscriptionName, new CubePoster(cubeEndpoint));

            return base.OnStart();
        }

        public override void OnStop()
        {
            m_ScubscriptionClient.Stop();
            m_CompletedEvent.Set();
            base.OnStop();
        }
    }
}
