using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace ConsoleAppServiceBus.Example.Common
{
    public sealed class BusHelper
    {
        public QueueClient CreateQueueClient()
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            try
            {
                return QueueClient.CreateFromConnectionString(connectionString, Constants.QueueName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }
        public TopicClient CreateTopicClient()
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            try
            {
                return TopicClient.CreateFromConnectionString(connectionString, Constants.TopicName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }
        public static void CreateSubscription(NamespaceManager namespaceManager, string topicPath, string topicName, Filter filter)
        {
            if (!namespaceManager.SubscriptionExists(topicPath, topicName))
            {
                namespaceManager.CreateSubscription(topicPath, topicName, filter);
            }
        }
        public static void CreateSubscription(NamespaceManager namespaceManager, string topicPath, string topicName)
        {
            if (!namespaceManager.SubscriptionExists(topicPath, topicName))
            {
                namespaceManager.CreateSubscription(topicPath, topicName);
            }
        }

    }
}
