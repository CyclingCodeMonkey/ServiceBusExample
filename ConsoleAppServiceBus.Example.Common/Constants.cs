namespace ConsoleAppServiceBus.Example.Common
{
    public class Constants
    {
        public const string QueueName = "SampleQueue";
        public const string TopicName = "TopicName";


        public enum Topics
        {
            Individual,
            Company,
            Trust,
            Smsf
        }

        public enum CustomProperties
        {
            Custom1,
            ApplicationType
        }
    }
}
