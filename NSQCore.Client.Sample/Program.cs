using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NSQCore.Client.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var topic = new Topic("NSQCore.Client.Sample");
            var consumer = NsqConsumer.Create(new ConsumerOptions
            {
                Topic = topic,
                Channel = "NSQCore.Client.Sample.Channel",
                NsqEndPoint = new DnsEndPoint("127.0.0.1", 4150),
            }) as NsqTcpConnection;

            //consumer.InternalMessages = (s, a) => Console.WriteLine(a.Message);

            consumer.ConnectAndWaitAsync(async msg =>
            {
                Console.WriteLine("Echo: " + msg.Body);
                await msg.FinishAsync();
            }).Wait();

            consumer.SetMaxInFlightAsync(1).Wait();

            var producer = new NsqProducer("127.0.0.1", 4151);

            Console.WriteLine("Type a message then hit [enter] to send...");

            while (true)
            {
                string input = Console.ReadLine();
                producer.PublishAsync(topic, input).Wait();

                if (input.StartsWith("die")) break;
            }

            Console.WriteLine("All done! shutting down...");

            consumer.Dispose();
        }
    }
}
