using MassTransit;
using PaymentMicroservice.API.Repositories;
using Shared.Events;

namespace PaymentMicroservice.API.Consumers;

public class StockReservedEventConsumer(AppDbContext dbContext, IPublishEndpoint publishEndpoint)
    : IConsumer<StockReservedEvent>
{
    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var payment = new Payment
        {
            OrderId = context.Message.OrderId,
            Status = PaymentStatus.Completed
        };

        dbContext.Payments.Add(payment);


        await publishEndpoint.Publish(new PaymentCompletedEvent(payment.OrderId, payment.Id));


        await dbContext.SaveChangesAsync();

        Console.WriteLine($"Payment completed, orderId:{context.Message.OrderId}");
    }
}