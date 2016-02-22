using System;
using System.Net;
using Microsoft.ServiceBus.Messaging;
using ConsoleAppServiceBus.Example.Common;
using Microsoft.WindowsAzure;

namespace ConsoleAppServiceBus.Example.Consumer
{
    public class Program
    {
        static void Main(string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
            ReceiveMessages();
            Console.WriteLine("\nWaiting...");
            Console.ReadKey();
        }

        private static void ReceiveMessages()
        {
            Console.WriteLine("\nReceiving message from Queue...");
            Console.WriteLine($"Connectionstring = {CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString")}");
            var helper = new BusHelper();
            var queueClient = helper.CreateQueueClient();

            queueClient.OnMessage(message =>
            {
                try
                {
                    var body = message.GetBody<Applicant>();

                    Console.WriteLine(
                        $"Message received: Id = {message.MessageId}; Custom Property={message.Properties[Constants.CustomProperties.Custom1.ToString()]}; Body = {body}");
                    message.Complete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    message.Abandon();
                }
            }, new OnMessageOptions
            {
                AutoComplete = false
            });
        }
    }
}
