using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace DEALERProxyREP.Server
{
    class Program
    {
        private static Random random = new Random();
        static void Main(string[] args)
        {
            var proxy1 = new Proxy(new RouterSocket("@tcp://*:5001"), new DealerSocket("@tcp://*:5002"));
            Task.Factory.StartNew(proxy1.Start);

            //NetMQPoller poller1 = new NetMQPoller();

            for (var i = 0; i < 10; i++)
            {
                Task.Factory.StartNew((o)=> 
                {
                    var response = new ResponseSocket(">tcp://127.0.0.1:5002");
                    while (true)
                    {
                        var msg = response.ReceiveFrameString();

                        /* your task
                        var r = random.NextDouble();

                        if (r > 0 && r < 0.78)
                        {
                            Thread.Sleep(random.Next(1, 51));
                        }
                        else if (r >= 0.78 & r < 0.78 + 0.21)
                        {
                            Thread.Sleep(random.Next(51, 101));
                        }
                        else
                        {
                            Thread.Sleep(random.Next(101, 300));
                        }
                        */

                        //Console.WriteLine(response.Options.Identity[0]);

                        response.SendFrame(msg);

                        //Console.WriteLine(msg);
                    }
                },i);
            }
            //poller1.RunAsync();

            Console.WriteLine("Press any key to stop!");
            Console.Read();
        }
    }
}
