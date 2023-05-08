using LeanCode.Contracts;
using LeanCode.CQRS.Execution;
using LeanCode.DomainModels.DataAccess;

namespace CoreLib.Example;

public class AddProduct : ICommand
{
    public string OrderId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
}

public class AddProductCH : ICommandHandler<ExampleContext, AddProduct>
{
    private readonly IRepository<Order, OrderId> orders;

    public AddProductCH(IRepository<Order, OrderId> orders)
    {
        this.orders = orders;
    }

    public async Task ExecuteAsync(ExampleContext context, AddProduct command)
    {
        var order = await orders.FindAndEnsureExistsAsync(OrderId.Parse(command.OrderId));
        var line = new OrderLine(command.ProductName, command.Quantity, command.Quantity * 100m); // TODO: get the price from somewhere :)
        order.AddProduct(line);
        orders.Update(order);
    }
}
