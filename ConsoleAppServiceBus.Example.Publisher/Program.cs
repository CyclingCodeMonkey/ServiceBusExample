using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using ConsoleAppServiceBus.Example.Common;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;

namespace ConsoleAppServiceBus.Example.Publisher
{
    public class Program
    {
        static void Main(string[] args)
        {
            Setup();
            SendMessages();
            Cleanup();
        }

        private static void SendMessages()
        {
            var helper = new BusHelper();
            var queueClient = helper.CreateQueueClient();
            var topicClient = helper.CreateTopicClient();
            var messages = CreateQueueMessages(50, CreateMessagePayload());

            Console.WriteLine("\nSending messages to Queue...");
            var rand = new Random(1000);
            foreach (var message in messages)
            {
                try
                {
                    var topicMessage = message.Clone();
                    //for simplicity clone the message and send the event
                    topicClient.Send(topicMessage);
                    queueClient.Send(message);
                }
                catch (MessagingException e)
                {
                    if (!e.IsTransient)
                    {
                        Console.WriteLine(e.Message);
                        throw;
                    }
                    HandleTransientErrors(e);
                }
                Console.WriteLine($"Message received: Id = {message.MessageId} ApplicationType = {message.Properties[Constants.CustomProperties.ApplicationType.ToString()]}");
                // sleep for a short period of time
                Thread.Sleep(rand.Next(500, 2000));
            }
        }

        private static Applicant CreateMessagePayload()
        {
            return new Applicant
            {
                ApplicantionId = Guid.NewGuid(),
                Salutation = Salutation.Dr,
                FirstName = "Fred",
                LastName = "Smith",
                Addresses = CreateApplicantAddress(),
                CreatedBy = "System",
                ModifiedBy = "System"
            };

        }

        private static IList<Address> CreateApplicantAddress()
        {
            return new List<Address>
            {
                new Address
                {
                    Id = 1,
                    LevelNumber = "9",
                    StreetNumber = "45",
                    StreetName = "Clarence Street",
                    PostCode = "2000",
                    State = "NSW",
                    Country = "Australia"
                },
                new Address
                {
                    Id = 2,
                    LevelNumber = "24",
                    StreetNumber = "45",
                    StreetName = "Clarence Street",
                    PostCode = "2000",
                    State = "NSW",
                    Country = "Australia"
                }
            };

        }

        private static IEnumerable<BrokeredMessage> CreateQueueMessages(int count, object messageBody)
        {
            var messageList = new List<BrokeredMessage>();
            if (messageBody == null) return messageList;
            for (var i = 100; i < 100+count; i++)
            {
                ((IMessage) messageBody).Id = i;
                ((IMessage) messageBody).Created = DateTime.Now;
                ((IMessage) messageBody).Modified = DateTime.Now;
                messageList.Add((i % 2) == 0
                    ? CreateMessage(i.ToString(), messageBody, Constants.Topics.Company.ToString())
                    : CreateMessage(i.ToString(), messageBody, Constants.Topics.Individual.ToString()));
            }
            return messageList;
        }

        private static BrokeredMessage CreateMessage(string messageId, object messageBody, string applicationType = "Individual")
        {
            var message = new BrokeredMessage(messageBody)
            {
                MessageId = messageId,
                CorrelationId = Guid.NewGuid().ToString(),
                Label = $"Application {messageId}",
                TimeToLive = TimeSpan.FromDays(7)
            };
            message.Properties[Constants.CustomProperties.Custom1.ToString()] = Guid.NewGuid().ToString();
            message.Properties[Constants.CustomProperties.ApplicationType.ToString()] = applicationType;
            return message;
        }

        private static void Setup()
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            var namespaceManager = NamespaceManager.Create();
            CreateQueue(namespaceManager);
            CreateTopic(namespaceManager);
            Console.WriteLine("Press anykey to start sending messages ...");
            Console.ReadKey();
        }

        private static void Cleanup()
        {
            //var namespaceManager = NamespaceManager.Create();
            //namespaceManager.DeleteQueue(Constants.QueueName);
            Console.WriteLine("\nEnd of scenario, press anykey to exit.");
            Console.ReadKey();
        }

        private static void CreateQueue(NamespaceManager namespaceManager)
        {
            if (namespaceManager == null) namespaceManager = NamespaceManager.Create();
            if (namespaceManager.QueueExists(Constants.QueueName)) return;

            Console.WriteLine("\nCreating Queue '{0}'...", Constants.QueueName);
            namespaceManager.CreateQueue(Constants.QueueName);
        }

        private static void CreateTopic(NamespaceManager namespaceManager)
        {
            if (namespaceManager == null) namespaceManager = NamespaceManager.Create();
            if (namespaceManager.TopicExists(Constants.TopicName)) return;

            Console.WriteLine("\nCreating Topic '{0}'...", Constants.TopicName);
            namespaceManager.CreateTopic(Constants.TopicName);
        }

        private static void HandleTransientErrors(MessagingException e)
        {
            //If transient error/exception, let's back-off for 2 seconds and retry 
            Console.WriteLine(e.Message);
            Console.WriteLine("Will retry sending the message in 2 seconds");
            Thread.Sleep(2000);
        }
    }
}
