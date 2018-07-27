using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace REQProxyREP.Client
{
    class Program
    {
        private static bool isRunning = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start!");
            Console.Read();

            NetMQPoller Poller2 = new NetMQPoller();
            NetMQQueue<int> Queue = new NetMQQueue<int>();

            for (var i = 0; i < 10; i++)
            {
                Task.Factory.StartNew((o)=> 
                {
                    using (var req = new RequestSocket(">tcp://127.0.0.1:5001"))
                    {
                        if (req.TrySendFrame(TimeSpan.FromSeconds(1), $"task:{o}"))
                        {
                            var msg = req.ReceiveFrameString();

                            Console.WriteLine(msg);
                        }
                        else
                        {
                            Console.WriteLine($"task:{i} timeout!");
                        }
                    }
                },i);
            }

            Console.WriteLine("Press any key to stop!");
            Console.Read();
            Console.Read();
        }
    }
}
