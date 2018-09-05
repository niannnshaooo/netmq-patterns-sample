using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace PubSub.Client
{
    class Program
    {
        private static PublisherSocket socket = new PublisherSocket(">tcp://localhost:1011");
        private static NetMQQueue<string> queue = new NetMQQueue<string>();
        static void Main(string[] args)
        {

            queue.ReceiveReady += (s, e) =>
            {
                var msg = e.Queue.Dequeue();
                socket.SendMoreFrame("aaa").SendFrame(msg);
            };

            NetMQPoller poller = new NetMQPoller();
            poller.Add(queue);
            poller.RunAsync();

            for (var i = 0; i < 50; i++)
            {
                Task.Factory.StartNew(Run, i);
            }

            Console.WriteLine("running...");
            Console.Read();
        }

        private static void Run(object obj)
        {
            while (true)
            {
                //using (var s = new PublisherSocket(">tcp://localhost:1022"))
                //{
                //    for (var i = 0; i < 1000; i++)
                //    {
                //        s.SendMoreFrame("aaa").SendFrame(Convert.ToString(DateTime.Now.Ticks));
                //    }
                //}

                queue.Enqueue(Convert.ToString(DateTime.Now.Ticks));

                Thread.Sleep(1);
            }
        }
    }
}
