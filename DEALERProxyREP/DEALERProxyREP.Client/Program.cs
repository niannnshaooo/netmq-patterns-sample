using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace DEALERProxyREP.Client
{
    //http://zguide.zeromq.org/page:all#The-DEALER-to-REP-Combination
    class Program
    {
        private static bool isRunning = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start!");
            Console.Read();

            NetMQPoller Poller2 = new NetMQPoller();
            NetMQQueue<int> Queue = new NetMQQueue<int>();
            var index = 0;
            var client = new DealerSocket(">tcp://127.0.0.1:5001");
            
            client.Options.Identity = Encoding.UTF8.GetBytes($"clientid-{Process.GetCurrentProcess().Id}");
            client.ReceiveReady += (s, e) =>
            {
                e.Socket.ReceiveFrameBytes();
                var msg = e.Socket.ReceiveFrameString();
                Console.WriteLine($"RECEIVE:{msg}");

            };
            Queue.ReceiveReady += (s, e) =>
            {
                var msg = e.Queue.Dequeue();
                client.SendMoreFrameEmpty().SendFrame($"{Encoding.UTF8.GetString(client.Options.Identity)}-{msg}");
                Console.WriteLine($"SEND:{msg}");
            };
            Poller2.Add(Queue);
            Poller2.Add(client);
            Poller2.RunAsync();
            for (var i = 0; i < 5; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    while (isRunning)
                    {
                        Queue.Enqueue(++index);
                        Thread.Sleep(10);
                    }
                });
            }

            Console.WriteLine("Press any key to stop!");
            Console.Read();
        }
    }
}
