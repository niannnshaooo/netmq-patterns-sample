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
        static void Main(string[] args)
        {
            var proxy1 = new Proxy(new RouterSocket("@tcp://*:5001"), new DealerSocket("@tcp://*:5002"));
            Task.Factory.StartNew(proxy1.Start);

            NetMQPoller poller1 = new NetMQPoller();

            for (var i = 0; i < 2; i++)
            {
                var response = new ResponseSocket(">tcp://127.0.0.1:5002");
                response.ReceiveReady += (o, s) =>
                {
                    var msg = s.Socket.ReceiveFrameString();

                    s.Socket.SendFrame(msg);

                    Console.WriteLine(msg);
                };
                poller1.Add(response);
            }
            poller1.RunAsync();

            Console.WriteLine("Press any key to stop!");
            Console.Read();
        }
    }
}
