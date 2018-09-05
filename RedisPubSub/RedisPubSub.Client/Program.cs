using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace RedisPubSub.Client
{
    class Program
    {
        private static ConnectionMultiplexer ConnectionMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1:5379,127.0.0.1:5380,password=123456");

        static void Main(string[] args)
        {
            for (var i = 0; i < 50; i++)
            {
                Task.Factory.StartNew(Run,i);
            }

            Console.WriteLine("running...");
            Console.Read();
        }

        private static void Run(object obj)
        {
            while (true)
            {
                ConnectionMultiplexer.GetSubscriber().Publish("aaa", DateTime.Now.Ticks);
                Thread.Sleep(1);
            }
        }
    }
}
