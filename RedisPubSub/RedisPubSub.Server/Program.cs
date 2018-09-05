using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RedisPubSub.Server
{
    class Program
    {
        private static ConnectionMultiplexer ConnectionMultiplexer = ConnectionMultiplexer.Connect("127.0.0.1:5379,127.0.0.1:5380,password=123456");


        private static List<TimeSpan> d = new List<TimeSpan>();
        
        static void Main(string[] args)
        {
            var date = DateTime.Now;

            ConnectionMultiplexer.GetSubscriber().Subscribe("aaa", (c, m) =>
            { 
                var t = DateTime.Now.Subtract(new DateTime(Convert.ToInt64(m)));

                d.Add(t);
            });


            Task.Factory.StartNew(() => 
            {
                while (true)
                {
                    Thread.Sleep(10 * 1000);

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
    }
}
