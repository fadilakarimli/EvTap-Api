namespace EvTap.Modules.Bookings.Infrastructure;

/// <summary>
/// Simulated payment processor — no external service, no real money ever moves. Mirrors the
/// well-known Stripe test-card convention so the "gateway" behaves predictably in a demo:
/// any 16-digit card starting with 4242 succeeds, everything else is declined.
/// </summary>
internal sealed class MockPaymentGateway : IPaymentGateway
{
    public Task<ChargeResult> ChargeAsync(ChargeRequest request, CancellationToken cancellationToken)
    {
        var digits = new string(request.CardNumber.Where(char.IsDigit).ToArray());

        if (digits.Length != 16)
        {
            return Task.FromResult(ChargeResult.Failure("Kart nömrəsi 16 rəqəmdən ibarət olmalıdır."));
        }

        if (request.Cvv.Length is < 3 or > 4 || !request.Cvv.All(char.IsDigit))
        {
            return Task.FromResult(ChargeResult.Failure("CVV kodu yanlışdır."));
        }

        var result = digits switch
        {
            "4242424242424242" => ChargeResult.Success($"mock_{Guid.NewGuid():N}"),
            "4000000000000002" => ChargeResult.Failure("Kart bank tərəfindən rədd edildi."),
            "4000000000009995" => ChargeResult.Failure("Kartda kifayət qədər vəsait yoxdur."),
            _ => ChargeResult.Failure("Kart etibarsızdır. Test üçün 4242 4242 4242 4242 istifadə edin."),
        };

        return Task.FromResult(result);
    }
}
