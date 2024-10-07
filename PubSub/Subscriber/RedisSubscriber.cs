using Common.Interfaces;
using Common.Messages;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISubscriber = StackExchange.Redis.ISubscriber;


namespace PubSub.Subscriber
{
    public class RedisSubscriber : ISubscribe
    {
        private ISubscriber _subscriber;
        private readonly ConcurrentDictionary<string, ConcurrentBag<Action<object>>> _subscribeDictionary;
        private static readonly object _lock = new object();

        public RedisSubscriber(string connectionString)
        {
            using ConnectionMultiplexer redis =
                ConnectionMultiplexer.Connect(connectionString);                                            // Connect to Redis
            _subscriber = redis.GetSubscriber();                                                            // Get the subscriber
            _subscribeDictionary = new ConcurrentDictionary<string, ConcurrentBag<Action<object>>>();       // Create a dictionary to store the actions
        }

        public async Task Subscribe(string messageType, Action<object> action) // Subscribe to a message type
        {

            var actions = _subscribeDictionary[messageType]; // Get the actions for the message type
            lock (_lock) // Lock the dictionary
            {
                if (actions == null)
                {
                    actions = new ConcurrentBag<Action<object>>(); // Create a new bag of actions
                }
            }

            actions.Add(action); // Add the action to the bag
            await _subscriber.SubscribeAsync(messageType, (messageType, message) =>  // Subscribe to the message type
            {
                foreach (Action<object> act in _subscribeDictionary[messageType])
                {
                    act(message); // Execute the action
                }
            });
            
        }

        public void Subscribe(string v, Func<OrderCreatedMessage, Task> handleOrderCreated)
        {
            throw new NotImplementedException();
        }
    }
}
