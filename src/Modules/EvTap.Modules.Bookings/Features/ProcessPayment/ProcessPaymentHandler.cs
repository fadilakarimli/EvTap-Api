using EvTap.Modules.Bookings.Domain;
using EvTap.Modules.Bookings.Infrastructure;
using EvTap.Modules.Bookings.Persistence;
using EvTap.Shared.Contracts;
using EvTap.Shared.Results;
using MassTransit;
using MediatR;

namespace EvTap.Modules.Bookings.Features.ProcessPayment;

internal sealed class ProcessPaymentHandler(
    BookingsDbContext dbContext,
    IPaymentGateway paymentGateway,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<ProcessPaymentCommand, Result<ProcessPaymentResponse>>
{
    public async Task<Result<ProcessPaymentResponse>> Handle(
        ProcessPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var booking = await dbContext.Bookings.FindAsync([request.BookingId], cancellationToken);

        if (booking is null)
        {
            return Result.Failure<ProcessPaymentResponse>(
                Error.NotFound("Bookings.NotFound", "Booking not found."));
        }

        if (booking.RenterId != request.UserId)
        {
            return Result.Failure<ProcessPaymentResponse>(
                Error.Forbidden("Bookings.NotOwner", "This booking does not belong to you."));
        }

        // Idempotent: a booking already marked Paid is simply reported back as-is.
        if (booking.Status == BookingStatus.Paid)
        {
            return Result.Success(new ProcessPaymentResponse(booking.Id, booking.Status.ToString()));
        }

        if (booking.Status != BookingStatus.PendingPayment)
        {
            return Result.Failure<ProcessPaymentResponse>(
                Error.Conflict("Bookings.NotPayable", "This booking is not awaiting payment."));
        }

        var chargeResult = await paymentGateway.ChargeAsync(
            new ChargeRequest(
                booking.TotalPriceAzn,
                request.CardNumber,
                request.CardholderName,
                request.ExpiryMonth,
                request.ExpiryYear,
                request.Cvv),
            cancellationToken);

        if (!chargeResult.Succeeded)
        {
            // Booking stays PendingPayment so the renter can retry with a different card.
            return Result.Failure<ProcessPaymentResponse>(
                Error.Failure("Bookings.PaymentDeclined", chargeResult.FailureReason ?? "Ödəniş rədd edildi."));
        }

        var digits = new string(request.CardNumber.Where(char.IsDigit).ToArray());

        booking.Status = BookingStatus.Paid;
        booking.PaymentReference = chargeResult.TransactionId;
        booking.CardLast4 = digits[^4..];
        booking.PaidAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new PaymentCompletedEvent(
                booking.Id,
                booking.ListingId,
                booking.ListingTitle,
                booking.RenterId,
                booking.RenterEmail,
                booking.OwnerId,
                booking.RentalType.ToString(),
                booking.StartAt,
                booking.EndAt,
                booking.Units,
                booking.UnitPriceAzn,
                booking.TotalPriceAzn,
                booking.PaymentReference!,
                booking.CardLast4!,
                booking.PaidAt.Value),
            cancellationToken);

        return Result.Success(new ProcessPaymentResponse(booking.Id, booking.Status.ToString()));
    }
}
