using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;

namespace PubSub.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            NetMQ.Proxy proxy = new NetMQ.Proxy(new XSubscriberSocket("@tcp://*:1011"),new XPublisherSocket("@tcp://*:1012"));
            Console.WriteLine("running...");
            proxy.Start();
        }
    }
}
