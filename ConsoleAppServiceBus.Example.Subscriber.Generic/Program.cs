using System;
using ConsoleAppServiceBus.Example.Common;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace ConsoleAppServiceBus.Example.Subscriber.Generic
{
    public class Program
    {
        static void Main(string[] args)
        {
            var connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            Console.WriteLine("Setting up subscriptions");
            CreateSubscriptions(namespaceManager);
            Console.WriteLine("Create handler for company applications");
            HandleCompanyTopic(connectionString);
            
            Console.WriteLine("Waiting for events ...");
            Console.ReadKey();
        }

        private static void HandleCompanyTopic(string connectionString)
        {
            var clientCompany = SubscriptionClient.CreateFromConnectionString(connectionString, Constants.TopicName, "Generic");
            var options = new OnMessageOptions { AutoComplete = false };


            clientCompany.OnMessage((message) =>
            {
                try
                {
                    var type = message.Properties[Constants.CustomProperties.ApplicationType.ToString()].ToString();
                    var body = message.GetBody<Applicant>();
                    // Process message from subscription.
                    Console.ForegroundColor = type == Constants.Topics.Company.ToString() ? ConsoleColor.Green : ConsoleColor.White;
                    Console.WriteLine($"\n**Individual {type}**");
                    Console.WriteLine("Body: " + body);
                    Console.WriteLine("MessageID: " + message.MessageId);
                    Console.WriteLine("Application Type: " + type);

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
            BusHelper.CreateSubscription(namespaceManager, Constants.TopicName, "Generic");
        }
    }
}
