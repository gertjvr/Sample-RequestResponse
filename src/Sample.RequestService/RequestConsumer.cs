namespace Sample.RequestService
{
    using System.Threading.Tasks;
    using Serilog;
    using MassTransit;
    using MessageTypes;

    public class RequestConsumer : IConsumer<ISimpleRequest>
    {
        public async Task Consume(ConsumeContext<ISimpleRequest> context)
        {
            Log.Information("Returning name for {CustomerId}", context.Message.CustomerId);

            await context.RespondAsync(new SimpleResponse($"Customer Number {context.Message.CustomerId}"));
        }
    }
}