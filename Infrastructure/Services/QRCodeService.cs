using QRCoder;
using RestaurantOrderTracking.Domain.Interface;

namespace RestaurantOrderTracking.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of <see cref="IQRCodeService"/>.
    /// Uses QRCoder (PngByteQRCode) — no GDI+ dependency, compatible with .NET 10 on Linux/Windows.
    /// </summary>
    public sealed class QRCodeService : IQRCodeService
    {
        private const int PixelsPerModule = 10;

        /// <inheritdoc/>
        public string GenerateBase64(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("QR content cannot be empty.", nameof(content));

            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);
            byte[] pngBytes = qrCode.GetGraphic(PixelsPerModule);
            return Convert.ToBase64String(pngBytes);
        }
    }
}
