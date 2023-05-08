using LeanCode.DomainModels.DataAccess;
using MassTransit;

namespace CoreLib.Example;

public class RetryPaymentEH : IConsumer<PaymentFailed>
{
    private readonly IRepository<Payment, PaymentId> payments;

    public RetryPaymentEH(IRepository<Payment, PaymentId> payments)
    {
        this.payments = payments;
    }

    public async Task Consume(ConsumeContext<PaymentFailed> context)
    {
        var payment = Payment.Retry(context.Message);
        payments.Add(payment);
    }
}
