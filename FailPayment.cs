using LeanCode.Contracts;
using LeanCode.CQRS.Execution;
using LeanCode.DomainModels.DataAccess;

namespace CoreLib.Example;

public class FailPayment : ICommand
{
    public string PaymentId { get; set; }
}

public class FailPaymentCH : ICommandHandler<ExampleContext, FailPayment>
{
    private readonly IRepository<Payment, PaymentId> payments;

    public FailPaymentCH(IRepository<Payment, PaymentId> payments)
    {
        this.payments = payments;
    }

    public async Task ExecuteAsync(ExampleContext context, FailPayment command)
    {
        var payment = await payments.FindAndEnsureExistsAsync(PaymentId.Parse(command.PaymentId));

        payment.Fail();

        payments.Update(payment);
    }
}
