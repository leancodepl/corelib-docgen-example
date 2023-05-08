using LeanCode.DomainModels.Ids;
using LeanCode.DomainModels.Model;
using LeanCode.Time;

namespace CoreLib.Example;

[TypedId(TypedIdFormat.PrefixedGuid)]
public readonly partial record struct PaymentId;

public class Payment : IAggregateRoot<PaymentId>
{
    public PaymentId Id { get; private init; }

    public PaymentId? PreviousPayment { get; private init; }
    public OrderId OrderId { get; private init; }
    public PaymentState State { get; private set; }

    DateTime IOptimisticConcurrency.DateModified { get; set; }

    private Payment() { }

    public static Payment Start(OrderPlaced evt)
    {
        var payment = new Payment
        {
            Id = PaymentId.New(),
            OrderId = evt.OrderId,
            State = PaymentState.InProgress,
        };
        DomainEvents.Raise(new PaymentInitiated(payment));
        return payment;
    }

    public static Payment Retry(PaymentFailed evt)
    {
        var payment = new Payment
        {
            Id = PaymentId.New(),
            PreviousPayment = evt.PaymentId,
            OrderId = evt.OrderId,
            State = PaymentState.InProgress,
        };
        DomainEvents.Raise(new PaymentInitiated(payment));
        return payment;
    }

    public void Pay()
    {
        EnsureState(PaymentState.InProgress);
        State = PaymentState.Succeeded;
        DomainEvents.Raise(new PaymentSucceeded(this));
    }

    public void Fail()
    {
        EnsureState(PaymentState.InProgress);
        State = PaymentState.Failed;
        DomainEvents.Raise(new PaymentFailed(this));
    }

    private void EnsureState(PaymentState state)
    {
        if (State != state)
        {
            throw new InvalidOperationException(
                $"The payment {Id} is in invalid state (expected: {state}, actual: {State})."
            );
        }
    }
}

public enum PaymentState
{
    InProgress = 0,
    Failed = 1,
    Succeeded = 2,
}

public record PaymentInitiated : IDomainEvent
{
    public Guid Id { get; }
    public DateTime DateOccurred { get; }

    public PaymentId PaymentId { get; }

    public PaymentInitiated(Payment payment)
    {
        Id = Guid.NewGuid();
        DateOccurred = TimeProvider.Now;

        PaymentId = payment.Id;
    }
}

public record PaymentFailed : IDomainEvent
{
    public Guid Id { get; }
    public DateTime DateOccurred { get; }

    public PaymentId PaymentId { get; }
    public OrderId OrderId { get; }

    public PaymentFailed(Payment payment)
    {
        Id = Guid.NewGuid();
        DateOccurred = TimeProvider.Now;

        PaymentId = payment.Id;
        OrderId = payment.OrderId;
    }
}

public record PaymentSucceeded : IDomainEvent
{
    public Guid Id { get; }
    public DateTime DateOccurred { get; }

    public PaymentId PaymentId { get; }
    public OrderId OrderId { get; }

    public PaymentSucceeded(Payment payment)
    {
        Id = Guid.NewGuid();
        DateOccurred = TimeProvider.Now;

        PaymentId = payment.Id;
        OrderId = payment.OrderId;
    }
}
