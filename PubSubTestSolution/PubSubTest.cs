using System;
using System.Threading.Tasks;
using Common.Interfaces;
using PubSub.Publisher;
using PubSub.Subscriber;

namespace EventBusExample
{
    public class Message : IMessage
    {
        public Message(string messageType)
        {
            MessageType = messageType;
        }

        public string MessageType { get; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // Redis connection string, replace it with your actual connection string
            string redisConnectionString = "your-redis-connection-string";

            // Create instances of publisher and subscriber
            var publisher1 = new RedisPublisher(redisConnectionString);
            var publisher2 = new RedisPublisher(redisConnectionString);

            var subscriber1 = new RedisSubscriber(redisConnectionString);
            var subscriber2 = new RedisSubscriber(redisConnectionString);

            // Subscribe actions
            await subscriber1.Subscribe("MessageType1", (obj) => Console.WriteLine($"Subscriber 1 received: {obj}"));
            await subscriber2.Subscribe("MessageType1", (obj) => Console.WriteLine($"Subscriber 2 received: {obj}"));

            // Publish messages
            var message1 = new Message("MessageType1");
            var message2 = new Message("MessageType2");

            await publisher1.Publish(message1);
            await publisher2.Publish(message2);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}