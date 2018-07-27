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

namespace DEALERProxyDEALER.Server
{
    class Program
    {
        private static Random random = new Random();
        static void Main(string[] args)
        {
            var proxy1 = new Proxy(new RouterSocket("@tcp://*:5001"), new DealerSocket("@tcp://*:5002"));
            Task.Factory.StartNew(proxy1.Start);

            var dealer = new DealerSocket(">tcp://127.0.0.1:5002");
            var queue = new NetMQQueue<NetMQMessage>();
            var poller = new NetMQPoller { dealer, queue };

            var show = false;

            dealer.ReceiveReady += (s,e) => 
            {
                var clientmessage = e.Socket.ReceiveMultipartMessage();

                Task.Factory.StartNew((o) =>
                {
                    var cmsg = (NetMQMessage)o;

                    var address = cmsg.First().ConvertToString();
                    var msg = cmsg.Last().ConvertToString() + "-OK";

                    var servermessage = new NetMQMessage(3);
                    servermessage.Append(address);//fortest
                    servermessage.AppendEmptyFrame();
                    servermessage.Append(msg);


                    //var r = random.NextDouble();
                    //if (r > 0 && r < 0.78)
                    //{
                    //    Thread.Sleep(random.Next(1, 51));
                    //}
                    //else if (r >= 0.78 & r < 0.78 + 0.21)
                    //{
                    //    Thread.Sleep(random.Next(51, 101));
                    //}
                    //else
                    //{
                    //    Thread.Sleep(random.Next(101, 300));
                    //}

                    queue.Enqueue(servermessage);

                    //if (show)
                    //    Console.WriteLine(msg);

                }, clientmessage);

                if(show)
                    Console.WriteLine(1);
            };

            queue.ReceiveReady += (s, e) =>
            {
                var msg = e.Queue.Dequeue();

                dealer.SendMultipartMessage(msg);
            };

            poller.RunAsync();

            //for (var i = 0; i < 5; i++)
            //{
            //    Task.Factory.StartNew(() =>
            //    {
            //        while (true)
            //        {
            //            var servermessage = new NetMQMessage(3);
            //            servermessage.Append(Encoding.UTF8.GetBytes("dealerclient")); //for test
            //            servermessage.AppendEmptyFrame();
            //            servermessage.Append($"================={DateTime.Now.ToString()}======================");

            //            queue.Enqueue(servermessage);
            //            Thread.Sleep(1000);
            //        }
            //    });
            //}
            Console.WriteLine("Press any key to stop!");
            Console.Read();
            show = true;

            Console.Read();
            Console.Read();
            Console.Read();

        }
    }
}
