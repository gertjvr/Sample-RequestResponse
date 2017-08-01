namespace Sample.Client
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Serilog;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using MessageTypes;
    using Microsoft.Extensions.Configuration;

    static class Extensions
    {
        public static IRequestClient<TRequest, TResponse> CreateRequestClient<TRequest, TResponse>(this IBus bus, TimeSpan timeout, TimeSpan? ttl = null, Action<SendContext<TRequest>> callback = null)
            where TRequest : class
            where TResponse : class
        {
            var settings = bus.Address.GetHostSettings();
            var sendAddress = settings.Topology.GetDestinationAddress(typeof(TRequest));
            return bus.CreateRequestClient<TRequest, TResponse>(sendAddress, timeout, ttl, callback);
        }
    }

    class Program
    {
        static void Main()
        {
            Task.Run(MainAsync).Wait();
        }

        static async Task MainAsync()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            
            Log.Information("Creating bus...");
            
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri(configuration["RabbitMQHost"]), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
                
                cfg.UseSerilog();
            });

            Log.Information("Starting bus...");
            
            await busControl.StartAsync();

            try
            {
                var client = busControl.CreateRequestClient<SimpleRequest, SimpleResponse>(TimeSpan.FromSeconds(10));

                for(;;)
                {
                    Console.Write("Enter customer id (quit exits): ");
                    var customerId = Console.ReadLine();
                    if (customerId == "quit")
                        break;

                    var response = await client.Request(new SimpleRequest(customerId, DateTime.UtcNow));
                    Console.WriteLine("Customer Name: {0}", response.CustomerName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception!!! OMG!!! {0}", ex);
            }
            finally
            {
                Log.Information("Stopping bus...");
                
                await busControl.StopAsync();
            }
        }
    }
}
