namespace EvTap.Modules.Bookings.Infrastructure;

public sealed record ChargeRequest(
    decimal AmountAzn,
    string CardNumber,
    string CardholderName,
    string ExpiryMonth,
    string ExpiryYear,
    string Cvv);

public sealed record ChargeResult(bool Succeeded, string? TransactionId, string? FailureReason)
{
    public static ChargeResult Success(string transactionId) => new(true, transactionId, null);

    public static ChargeResult Failure(string reason) => new(false, null, reason);
}

public interface IPaymentGateway
{
    Task<ChargeResult> ChargeAsync(ChargeRequest request, CancellationToken cancellationToken);
}
