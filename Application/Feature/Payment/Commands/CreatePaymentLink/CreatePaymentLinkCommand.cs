using MediatR;
using RestaurantOrderTracking.Domain.Common;
using System;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.CreatePaymentLink
{
    public record CreatePaymentLinkCommand(
        Guid BillId,
        string CancelUrl,
        string ReturnUrl
    ) : IRequest<Result<string>>;
}
