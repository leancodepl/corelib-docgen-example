using LeanCode.DomainModels.DataAccess;
using MassTransit;

namespace CoreLib.Example;

public class FinalizeOrderEH : IConsumer<PaymentSucceeded>
{
    private readonly IRepository<Order, OrderId> orders;

    public FinalizeOrderEH(IRepository<Order, OrderId> orders)
    {
        this.orders = orders;
    }

    public async Task Consume(ConsumeContext<PaymentSucceeded> context)
    {
        var order = await orders.FindAndEnsureExistsAsync(context.Message.OrderId);
        order.Pay();
        orders.Update(order);
    }
}
