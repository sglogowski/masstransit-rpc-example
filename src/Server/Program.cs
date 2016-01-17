using System;
using System.Threading.Tasks;
using MassTransit;
using MessageContract;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost/"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "ServerQueue", endpoint =>
                {
                    endpoint.Handler<MessageRequest>(async context =>
                    {
                        await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");

                        var status = 200;
                        var message = $"respons for message id : {context.Message.Id}";
                        var hash = context.Message.Hash;
                        var myMessageResponse = new MessageResponse(status, message, hash);

                        await context.RespondAsync(myMessageResponse);
                    });
                });
            });

            Task.Run(() => bus.Start());

            Console.ReadKey();
        }
    }
}
