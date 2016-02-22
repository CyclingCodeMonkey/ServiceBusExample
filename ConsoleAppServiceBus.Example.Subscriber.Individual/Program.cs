using System;
using ConsoleAppServiceBus.Example.Common;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace ConsoleAppServiceBus.Example.Subscriber.Individual
{
    public class Program
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            Console.WriteLine("Setting up subscriptions");
            CreateSubscriptions(namespaceManager);
            Console.WriteLine("Create handler for individual applications");
            HandleIndividualTopic(connectionString);

            Console.WriteLine("Waiting for events ...");
            Console.ReadKey();
        }
        
        private static void HandleIndividualTopic(string connectionString)
        {
            var clientIndividual = SubscriptionClient.CreateFromConnectionString(connectionString, Constants.TopicName, Constants.Topics.Individual.ToString());
            var options = new OnMessageOptions {AutoComplete = false};
            

            clientIndividual.OnMessage((message) =>
            {
                try
                {
                    // Process message from subscription.
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("\n**Individual Application**");
                    Console.WriteLine("Body: " + message.GetBody<Applicant>());
                    Console.WriteLine("MessageID: " + message.MessageId);
                    Console.WriteLine("Application Type: " + message.Properties[Constants.CustomProperties.ApplicationType.ToString()]);

                    // Remove message from subscription.
                    message.Complete();
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    Console.WriteLine(ex.Message);
                    message.Abandon();
                }
            }, options);
        }


        private static void CreateSubscriptions(NamespaceManager namespaceManager)
        {
            var filter = new SqlFilter($"{Constants.CustomProperties.ApplicationType} = '{Constants.Topics.Individual}'");
            BusHelper.CreateSubscription(namespaceManager, Constants.TopicName, Constants.Topics.Individual.ToString(), filter);
        }

    }
}
