using MediatR;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.CreatePaymentLink
{
    public record CreatePaymentLinkCommand(
        Guid BillId,
        string CancelUrl,
        string ReturnUrl
    ) : IRequest<Result<PaymentLinkResponse>>;
}
