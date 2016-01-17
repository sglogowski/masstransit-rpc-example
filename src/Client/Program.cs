using System;
using System.Threading;
using System.Threading.Tasks;
using Client.Properties;
using MassTransit;
using MessageContract;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            using (bus.Start())
            {
                var timeout = TimeSpan.FromSeconds(5);
                var timeSpan = TimeSpan.FromSeconds(1);
                var requestClient = bus.CreatePublishRequestClient<MessageRequest, MessageResponse>(timeout, timeSpan);

                var parallelOptions = new ParallelOptions {MaxDegreeOfParallelism = 5};
                Parallel.For(1, Int64.MaxValue, parallelOptions, (index) =>
                {
                    var value = $"Message id: {index}";
                    var hash = Guid.NewGuid().ToString();
                    var messageRequest = new MessageRequest(index, value, hash);
                    var responseMessage = requestClient.Request(messageRequest);

                    responseMessage.ContinueWith(t =>
                    {
                        var isResponseMessageMatchToRequest = string.Equals(t.Result.Hash, hash, StringComparison.OrdinalIgnoreCase);

                        Console.Out.WriteLineAsync($"result status: {t.Result.Status}, message: {t.Result.Message}, isResponseMessageMatchToRequest: {isResponseMessageMatchToRequest}");
                    }, TaskContinuationOptions.OnlyOnRanToCompletion);

                    responseMessage.ContinueWith(t =>
                    {
                        t.Exception?.Handle(exception =>
                        {
                            Console.Out.WriteLineAsync(exception.ToString());

                            return true;
                        });
                    }, TaskContinuationOptions.OnlyOnFaulted);


                    Thread.Sleep(Settings.Default.DelayBetweenRequest);
                });
            }
        }
    }
}
