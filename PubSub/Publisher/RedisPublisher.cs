using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using ISubscriber = StackExchange.Redis.ISubscriber;
using System.Text.Json;

namespace PubSub.Publisher
{
    public class RedisPublisher : IPublisher
    {    
        private  ISubscriber _subscriber;
        public RedisPublisher(string connectionString) // Connection string to Redis
        {
            using ConnectionMultiplexer redis =
                ConnectionMultiplexer.Connect(connectionString);                // Connect to Redis
            _subscriber = redis.GetSubscriber();                                // Get the subscriber
        }
        public Task Publish(IMessage message) // Publish the message
        {
            string jsonString = JsonSerializer.Serialize(message);              // Serialize the message
            return _subscriber.PublishAsync(message.MessageType, jsonString);   // Publish the message
        }        
    }
}