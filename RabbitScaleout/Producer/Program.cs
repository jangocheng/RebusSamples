﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Messages;
using Rebus.Activation;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Logging;
using Rebus.RabbitMq;

namespace Producer
{
    class Program
    {
        static void Main()
        {
            using (var adapter = new BuiltinHandlerActivator())
            {
                Configure.With(adapter)
                    .Logging(l => l.ColoredConsole(LogLevel.Warn))
                    .Transport(t => t.UseRabbitMqAsOneWayClient("amqp://localhost"))
                    .Start();

                var keepRunning = true;
                
                while (keepRunning)
                {
                    Console.WriteLine(@"a) Publish 10 jobs
b) Publish 100 jobs
c) Publish 1000 jobs

q) Quit");
                    var key = char.ToLower(Console.ReadKey(true).KeyChar);

                    switch (key)
                    {
                        case 'a':
                            Publish(10, adapter.Bus);
                            break;
                        case 'b':
                            Publish(100, adapter.Bus);
                            break;
                        case 'c':
                            Publish(1000, adapter.Bus);
                            break;
                        case 'q':
                            Console.WriteLine("Quitting");
                            keepRunning = false;
                            break;
                    }
                }
            }
        }

        static void Publish(int numberOfJobs, IBus bus)
        {
            Console.WriteLine("Publishing {0} jobs", numberOfJobs);

            var jobs = Enumerable.Range(0, numberOfJobs)
                .Select(i => new Job { JobNumber = i })
                .Select(job => bus.Publish(job))
                .ToArray();

            Task.WaitAll(jobs);
        }
    }
}
