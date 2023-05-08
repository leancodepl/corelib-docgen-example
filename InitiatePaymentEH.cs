using LeanCode.DomainModels.DataAccess;
using MassTransit;

namespace CoreLib.Example;

public class InitiatePaymentEH : IConsumer<OrderPlaced>
{
    private readonly IRepository<Payment, PaymentId> payments;

    public InitiatePaymentEH(IRepository<Payment, PaymentId> payments)
    {
        this.payments = payments;
    }

    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        var payment = Payment.Start(context.Message);
        payments.Add(payment);
    }
}
