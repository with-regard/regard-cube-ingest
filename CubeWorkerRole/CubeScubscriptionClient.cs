using System.IO;
using System.Text;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace WorkerRoleWithSBQueue1
{
    public class CubeScubscriptionClient
    {
        private readonly CubePoster m_Poster;
        private readonly SubscriptionClient m_Client;

        public CubeScubscriptionClient(string connectionString, string topic, string subscription, CubePoster poster)
        {
            m_Poster = poster;
            var regardNamespace = NamespaceManager.CreateFromConnectionString(connectionString);

            if (!regardNamespace.TopicExists(topic))
                regardNamespace.CreateTopic(topic);

            // Create the subscription
            if (!regardNamespace.SubscriptionExists(topic, subscription))
                regardNamespace.CreateSubscription(topic, subscription);

            m_Client = SubscriptionClient.CreateFromConnectionString(connectionString, topic, subscription);
        }

        public void Start()
        {
            m_Client.OnMessage(OnMessage);
        }

        public void Stop()
        {
            if (m_Client != null && !m_Client.IsClosed)
                m_Client.Close();
        }

        private async void OnMessage(BrokeredMessage receivedMessage)
        {
            Stream rawMessage = receivedMessage.GetBody<Stream>();

            using (var memoryStream = new MemoryStream())
            {
                rawMessage.CopyTo(memoryStream);
                var body = Encoding.UTF8.GetString(memoryStream.ToArray());

                await m_Poster.Post(body);
            }
        }
    }
}