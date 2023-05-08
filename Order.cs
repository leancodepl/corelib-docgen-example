using LeanCode.DomainModels.Ids;
using LeanCode.DomainModels.Model;
using LeanCode.Time;

namespace CoreLib.Example;

[TypedId(TypedIdFormat.PrefixedGuid)]
public readonly partial record struct OrderId;

public class Order : IAggregateRoot<OrderId>
{
    private readonly List<OrderLine> lines = new();

    public OrderId Id { get; private init; }
    public OrderState State { get; private set; }

    public string OwnerFullName { get; private init; }
    public IReadOnlyList<OrderLine> Lines => lines;

    public decimal TotalValue => Lines.Sum(e => e.Value);

    DateTime IOptimisticConcurrency.DateModified { get; set; }

    private Order()
    {
        OwnerFullName = "";
    }

    public static Order Create(string fullName)
    {
        var order = new Order
        {
            Id = OrderId.New(),
            State = OrderState.Created,
            OwnerFullName = fullName,
        };
        DomainEvents.Raise(new OrderCreated(order));
        return order;
    }

    public void AddProduct(OrderLine line)
    {
        EnsureState(OrderState.Created);
        lines.Add(line);
        DomainEvents.Raise(new OrderLineAdded(this, line));
    }

    public void Place()
    {
        EnsureState(OrderState.Created);
        State = OrderState.Placed;
        DomainEvents.Raise(new OrderPlaced(this));
    }

    public void Pay()
    {
        EnsureState(OrderState.Placed);
        State = OrderState.Paid;
        DomainEvents.Raise(new OrderPaid(this));
    }

    private void EnsureState(OrderState state)
    {
        if (State != state)
        {
            throw new InvalidOperationException(
                $"The order {Id} is in invalid state (expected: {state}, actual: {State})."
            );
        }
    }
}

public enum OrderState
{
    Created = 0,
    Placed = 1,
    Paid = 2
}

public record OrderLine(string ProductName, int Quantity, decimal Value) : ValueObject;

public record OrderCreated : IDomainEvent
{
    public Guid Id { get; }
    public DateTime DateOccurred { get; }

    public OrderId OrderId { get; }

    public OrderCreated(Order order)
    {
        Id = Guid.NewGuid();
        DateOccurred = TimeProvider.Now;

        OrderId = order.Id;
    }
}

public record OrderLineAdded : IDomainEvent
{
    public Guid Id { get; }
    public DateTime DateOccurred { get; }

    public OrderId OrderId { get; }
    public OrderLine Line { get; }

    public OrderLineAdded(Order order, OrderLine line)
    {
        Id = Guid.NewGuid();
        DateOccurred = TimeProvider.Now;

        OrderId = order.Id;
        Line = line;
    }
}

public record OrderPlaced : IDomainEvent
{
    public Guid Id { get; }
    public DateTime DateOccurred { get; }

    public OrderId OrderId { get; }
    public decimal TotalValue { get; }

    public OrderPlaced(Order order)
    {
        Id = Guid.NewGuid();
        DateOccurred = TimeProvider.Now;

        OrderId = order.Id;
        TotalValue = order.TotalValue;
    }
}

public record OrderPaid : IDomainEvent
{
    public Guid Id { get; }
    public DateTime DateOccurred { get; }

    public OrderId OrderId { get; }

    public OrderPaid(Order order)
    {
        Id = Guid.NewGuid();
        DateOccurred = TimeProvider.Now;

        OrderId = order.Id;
    }
}
