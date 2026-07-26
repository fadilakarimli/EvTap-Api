using EvTap.Shared.Results;
using MediatR;

namespace EvTap.Modules.Bookings.Features.ProcessPayment;

public sealed record ProcessPaymentCommand(
    Guid BookingId,
    Guid UserId,
    string CardNumber,
    string CardholderName,
    string ExpiryMonth,
    string ExpiryYear,
    string Cvv) : IRequest<Result<ProcessPaymentResponse>>;

public sealed record ProcessPaymentResponse(Guid BookingId, string Status);
