using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace DEALERProxyDEALER.Client
{
    class Program
    {
        private static bool isRunning = true;


        private static long sendcount, presendcount = 0;
        private static long receivecount, prereceivecount = 0;
        private static long processcount, preprocesscount = 0;

        private static int testcount = 1000;
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start!");
            Console.Read();

            NetMQPoller Poller2 = new NetMQPoller();
            NetMQQueue<int> Queue = new NetMQQueue<int>();
            var index = 0;
            var client = new DealerSocket(">tcp://127.0.0.1:5001");

            prereceivecount = testcount;

            new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine($"send:{(sendcount)/10}/s,receive:{(receivecount)/10}/s");

                    sendcount = 0;
                    receivecount = 0;

                    Thread.Sleep(1000 * 10);
                }
            }).Start();


            new Thread(()=> 
            {
                while (true)
                {
                    if (prereceivecount >= testcount)
                    {
                        prereceivecount = 0;

                        for (var i = 0; i < testcount; i++)
                        {
                            Queue.Enqueue(++index);
                        }
                    }

                    Thread.Sleep(0);
                }
            }).Start();

            client.Options.Identity = Encoding.UTF8.GetBytes($"dealerclient");
            client.ReceiveReady += (s, e) =>
            {
                e.Socket.ReceiveFrameBytes();
                var msg = e.Socket.ReceiveFrameString();
                //Console.WriteLine($"RECEIVE:{msg}");
                receivecount++;
                prereceivecount++;
            };
            Queue.ReceiveReady += (s, e) =>
            {
                var msg = e.Queue.Dequeue();
                client.SendMoreFrameEmpty().SendFrame($"{Encoding.UTF8.GetString(client.Options.Identity)}-{msg}");
                //Console.WriteLine($"SEND:{Encoding.UTF8.GetString(client.Options.Identity)}-{msg}");
                sendcount++;
            };

            Poller2.Add(Queue);

            Poller2.Add(client);
            Poller2.RunAsync();


            Console.WriteLine("Press any key to stop!");
            Console.Read();
        }
    }
}
