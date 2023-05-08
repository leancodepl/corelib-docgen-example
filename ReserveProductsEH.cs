using MassTransit;

namespace CoreLib.Example;

public class ReserveProductsEH : IConsumer<OrderLineAdded>
{
    public async Task Consume(ConsumeContext<OrderLineAdded> context)
    {
        var msg = context.Message;
        Console.WriteLine(
            $"The product {msg.Line.ProductName} was added to order {msg.OrderId} (qty: {msg.Line.Quantity})."
        );
    }
}
