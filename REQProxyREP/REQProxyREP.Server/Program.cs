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

namespace REQProxyREP.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var proxy1 = new Proxy(new RouterSocket("@tcp://*:5001"), new DealerSocket("@tcp://*:5002"));
            Task.Factory.StartNew(proxy1.Start);

            NetMQPoller poller1 = new NetMQPoller();

            for (byte i = 0; i < 2; i++)
            {
                Task.Factory.StartNew((o) =>
                {
                    var response = new ResponseSocket(">tcp://127.0.0.1:5002");
                    response.Options.Identity = new byte[] { (byte)o };
                    try
                    {
                        
                        while (true)
                        {
                            var msg = response.ReceiveFrameString();

                            if (response.Options.Identity[0] == 0)
                            {
                                Thread.Sleep(5000);
                                throw new Exception();
                            }

                            response.SendFrame($"{response.Options.Identity[0]}-{msg}");
                        }
                    }
                    catch
                    {
                        response.Close();
                        Console.WriteLine($"close {i}");
                    }
                },i);
            }
            poller1.RunAsync();

            Console.WriteLine("Press any key to stop!");
            Console.Read();
        }
    }
}
