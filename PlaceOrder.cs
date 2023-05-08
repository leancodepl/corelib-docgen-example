using LeanCode.Contracts;
using LeanCode.CQRS.Execution;
using LeanCode.DomainModels.DataAccess;

namespace CoreLib.Example;

public class PlaceOrder : ICommand
{
    public string OrderId { get; set; }
}

public class PlaceOrderCH : ICommandHandler<ExampleContext, PlaceOrder>
{
    private readonly IRepository<Order, OrderId> orders;

    public PlaceOrderCH(IRepository<Order, OrderId> orders)
    {
        this.orders = orders;
    }

    public async Task ExecuteAsync(ExampleContext context, PlaceOrder command)
    {
        var order = await orders.FindAndEnsureExistsAsync(OrderId.Parse(command.OrderId));
        order.Place();
        orders.Update(order);
    }
}
