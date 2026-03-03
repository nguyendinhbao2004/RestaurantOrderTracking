namespace RestaurantOrderTracking.Domain.Interface
{
    /// <summary>
    /// Contract for generating QR code images.
    /// Defined in Domain so Application handlers can depend on the abstraction
    /// without coupling to any infrastructure library.
    /// </summary>
    public interface IQRCodeService
    {
        /// <summary>
        /// Generates a QR code PNG image from <paramref name="content"/>
        /// and returns it as a Base64-encoded string.
        /// </summary>
        /// <param name="content">The text / URL to encode in the QR code.</param>
        /// <returns>Base64 PNG string — use as: data:image/png;base64,{result}</returns>
        string GenerateBase64(string content);
    }
}
