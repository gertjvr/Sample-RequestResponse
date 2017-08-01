namespace Sample.RequestService
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Serilog;
    using Microsoft.Extensions.Configuration;
    using MassTransit;

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
                
                cfg.ReceiveEndpoint(host, configuration["ServiceQueueName"], e =>
                {
                    e.Consumer<RequestConsumer>();
                });
            });

            Log.Information("Starting bus...");
            
            await busControl.StartAsync();
            
            try
            {
                for(;;)
                {
                    Console.Write("Enter quit to exit: ");
                    var customerId = Console.ReadLine();
                    if (customerId == "quit")
                        break;
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
