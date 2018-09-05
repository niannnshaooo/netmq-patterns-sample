using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace PubSub.Server
{
    class Program
    {

        static void Main(string[] args)
        {
            SubscriberSocket socket = new SubscriberSocket(">tcp://localhost:1012");
            socket.Subscribe("aaa");
            socket.ReceiveReady += Socket_ReceiveReady;

            NetMQ.NetMQPoller poller = new NetMQ.NetMQPoller();
            poller.Add(socket);

            poller.RunAsync();


            Task.Factory.StartNew(() => 
            {
                while (true)
                {
                    Thread.Sleep(10*1000);

                    var tmp = d;
                    d = new List<TimeSpan>();

                    if (tmp.Any())
                        Console.WriteLine($"{DateTime.Now}\t{tmp.Count / 10}/s\t{tmp.Average(x => x.TotalMilliseconds)}");
                    else
                        Console.WriteLine("-");
                }
            });

            Console.Read();
        }

        private static List<TimeSpan> d = new List<TimeSpan>();
        
        private static void Socket_ReceiveReady(object sender, NetMQ.NetMQSocketEventArgs e)
        {
            var topic = e.Socket.ReceiveFrameString();
            var dt = e.Socket.ReceiveFrameString();
            //Console.WriteLine(dt);
            var t = DateTime.Now.Subtract(new DateTime(Convert.ToInt64(dt)));
            d.Add(t);
        }
    }
}
