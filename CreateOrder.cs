using LeanCode.Contracts;
using LeanCode.CQRS.Execution;
using LeanCode.DomainModels.DataAccess;

namespace CoreLib.Example;

public class CreateOrder : ICommand
{
    public string Owner { get; set; }
}

public class CreateOrderCH : ICommandHandler<ExampleContext, CreateOrder>
{
    private readonly IRepository<Order, OrderId> orders;

    public CreateOrderCH(IRepository<Order, OrderId> orders)
    {
        this.orders = orders;
    }

    public async Task ExecuteAsync(ExampleContext context, CreateOrder command)
    {
        var order = Order.Create(command.Owner);
        orders.Add(order);
    }
}
