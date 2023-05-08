using LeanCode.Contracts;
using LeanCode.CQRS.Execution;
using LeanCode.DomainModels.DataAccess;

namespace CoreLib.Example;

public class Pay : ICommand
{
    public string PaymentId { get; set; }
}

public class PayCH : ICommandHandler<ExampleContext, Pay>
{
    private readonly IRepository<Payment, PaymentId> payments;

    public PayCH(IRepository<Payment, PaymentId> payments)
    {
        this.payments = payments;
    }

    public async Task ExecuteAsync(ExampleContext context, Pay command)
    {
        var payment = await payments.FindAndEnsureExistsAsync(PaymentId.Parse(command.PaymentId));

        payment.Pay();

        payments.Update(payment);
    }
}
