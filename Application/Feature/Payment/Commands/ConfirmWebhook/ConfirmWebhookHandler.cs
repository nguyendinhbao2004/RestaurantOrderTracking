using MediatR;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Application.Feature.Payment.Commands.ConfirmWebhook
{

    public class ConfirmWebhookHandler : IRequestHandler<ConfirmWebhookCommand, Result<string>>
    {
        private readonly IPayOSService _payOSService;

        public ConfirmWebhookHandler(IPayOSService payOSService)
        {
            _payOSService = payOSService;
        }

        public async Task<Result<string>> Handle(ConfirmWebhookCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.WebhookUrl))
                return Result<string>.Failure("WebhookUrl không được để trống.");

            bool confirmed = await _payOSService.ConfirmWebhookUrlAsync(request.WebhookUrl);

            return confirmed
                ? Result<string>.Success("Đăng ký Webhook URL thành công.", request.WebhookUrl)
                : Result<string>.Failure("PayOS không xác nhận được Webhook URL. Kiểm tra URL có public và trả về HTTP 200 không.");
        }
    }
}
