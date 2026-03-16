using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.Payment.Dtos;
using RestaurantOrderTracking.Application.Feature.Payment.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestaurantOrderTracking.Infrastructure.Services
{
    public class PayOSService : IPayOSService
    {
        // Dependency fields
        private readonly HttpClient _httpClient;
        private readonly ILogger<PayOSService> _logger;

        private readonly string _clientId;
        private readonly string _apiKey;
        private readonly string _checksumKey;

        // Base URL của PayOS API
        private const string PayOSBaseUrl = "https://api-merchant.payos.vn";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true
        };

        public PayOSService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<PayOSService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PayOS");
            _logger = logger;

            _clientId = configuration["PAYOS_CLIENT_ID"] ?? throw new InvalidOperationException("PAYOS_CLIENT_ID chưa được cấu hình.");
            _apiKey = configuration["PAYOS_API_KEY"] ?? throw new InvalidOperationException("PAYOS_API_KEY chưa được cấu hình.");
            _checksumKey = configuration["PAYOS_CHECKSUM_KEY"] ?? throw new InvalidOperationException("PAYOS_CHECKSUM_KEY chưa được cấu hình.");

            _httpClient.BaseAddress = new Uri(PayOSBaseUrl);
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _clientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        // ══════════════════════════════════════════════════════════════════════
        // 1. TẠO LINK THANH TOÁN
        // POST /v2/payment-requests
        // ══════════════════════════════════════════════════════════════════════

        public async Task<PaymentLinkResponse> CreatePaymentLinkAsync(
            long orderCode, int amount, string description, string cancelUrl, string returnUrl)
        {
            // Tạo chuỗi data để ký 
            var signatureData = new PaymentSignatureData
            {
                Amount = amount,
                CancelUrl = cancelUrl,
                Description = description,
                OrderCode = orderCode,
                ReturnUrl = returnUrl
            };
            string dataToSign = signatureData.ToSignatureString();

            // Tính chữ ký HMAC_SHA256
            string signature = ComputeHmacSha256(dataToSign);
            _logger.LogDebug("PayOS CreateLink - DataToSign: {Data} | Signature: {Sig}", dataToSign, signature);

            // Xây dựng request body
            var requestBody = new
            {
                orderCode,
                amount,
                description,
                cancelUrl,
                returnUrl,
                signature
            };

            // Gọi API PayOS
            var response = await _httpClient.PostAsJsonAsync("/v2/payment-requests", requestBody, JsonOptions);
            var rawJson = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("PayOS CreateLink Response [{Status}]: {Body}", response.StatusCode, rawJson);

            var result = JsonSerializer.Deserialize<PayOSApiResponse<PaymentLinkResponse>>(rawJson, JsonOptions)
                         ?? throw new InvalidOperationException("PayOS trả về response rỗng.");

            if (result.Code != "00")
                throw new InvalidOperationException($"PayOS tạo link thất bại: [{result.Code}] {result.Desc}");

            return result.Data!;
        }

        // ══════════════════════════════════════════════════════════════════════
        // 2. LẤY THÔNG TIN LINK THANH TOÁN
        // GET /v2/payment-requests/{paymentLinkId}
        // ══════════════════════════════════════════════════════════════════════

        public async Task<PaymentLinkInfoResponse> GetPaymentLinkInfoAsync(string paymentLinkId)
        {
            if (string.IsNullOrWhiteSpace(paymentLinkId))
                throw new ArgumentException("paymentLinkId không được để trống.", nameof(paymentLinkId));

            var response = await _httpClient.GetAsync($"/v2/payment-requests/{paymentLinkId}");
            var rawJson = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("PayOS GetInfo Response [{Status}]: {Body}", response.StatusCode, rawJson);

            var result = JsonSerializer.Deserialize<PayOSApiResponse<PaymentLinkInfoResponse>>(rawJson, JsonOptions)
                         ?? throw new InvalidOperationException("PayOS trả về response rỗng.");

            if (result.Code != "00")
                throw new InvalidOperationException($"PayOS lấy thông tin thất bại: [{result.Code}] {result.Desc}");

            return result.Data!;
        }

        // ══════════════════════════════════════════════════════════════════════
        // 3. HỦY LINK THANH TOÁN
        // POST /v2/payment-requests/{paymentLinkId}/cancel
        // ══════════════════════════════════════════════════════════════════════

        public async Task<CancelledPaymentLinkResponse> CancelPaymentLinkAsync(string paymentLinkId, string? cancellationReason = null)
        {
            if (string.IsNullOrWhiteSpace(paymentLinkId))
                throw new ArgumentException("paymentLinkId không được để trống.", nameof(paymentLinkId));

            var requestBody = new { cancellationReason };

            var response = await _httpClient.PostAsJsonAsync($"/v2/payment-requests/{paymentLinkId}/cancel", requestBody, JsonOptions);
            var rawJson = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("PayOS CancelLink Response [{Status}]: {Body}", response.StatusCode, rawJson);

            var result = JsonSerializer.Deserialize<PayOSApiResponse<CancelledPaymentLinkResponse>>(rawJson, JsonOptions)
                         ?? throw new InvalidOperationException("PayOS trả về response rỗng.");

            if (result.Code != "00")
                throw new InvalidOperationException($"PayOS hủy link thất bại: [{result.Code}] {result.Desc}");

            return result.Data!;
        }

        // ══════════════════════════════════════════════════════════════════════
        // 4. XỬ LÝ WEBHOOK - XÁC THỰC CHỮ KÝ
        // ══════════════════════════════════════════════════════════════════════

        public PayOSWebhookData? VerifyAndExtractWebhookData(PayOSWebhookPayload payload)
        {
            if (!payload.Success)
            {
                _logger.LogWarning("PayOS Webhook nhận được nhưng giao dịch không thành công. Code: {Code}", payload.Code);
                return null;
            }

            var data = payload.Data;

            var webhookSignatureFields = new SortedDictionary<string, string>(StringComparer.Ordinal);
            var properties = data.GetType().GetProperties();

            foreach (var prop in properties)
            {
                // ignore extension data and ignore data
                if (prop.GetCustomAttributes(typeof(JsonExtensionDataAttribute), false).Any() ||
                    prop.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Any())
                {
                    continue;
                }

                // convert property name to camel case
                string key = char.ToLowerInvariant(prop.Name[0]) + prop.Name.Substring(1);

                var value = prop.GetValue(data);
                string stringValue = value?.ToString() ?? string.Empty;

                webhookSignatureFields[key] = stringValue;
            }

            // handle additional data
            if (data.AdditionalData != null)
            {
                foreach (var kvp in data.AdditionalData)
                {
                    if (kvp.Value.ValueKind == JsonValueKind.Null ||
                        kvp.Value.ValueKind == JsonValueKind.Object ||
                        kvp.Value.ValueKind == JsonValueKind.Array)
                    {
                        webhookSignatureFields[kvp.Key] = string.Empty;
                    }
                    else
                    {
                        webhookSignatureFields[kvp.Key] = kvp.Value.ToString();
                    }
                }
            }

            // join key=value&key=value
            string dataToVerify = string.Join("&", webhookSignatureFields.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            // compute expected signature
            string expectedSignature = ComputeHmacSha256(dataToVerify);

            _logger.LogDebug("PayOS Webhook - DataToVerify: {Data} || Expected: {Expected} || Received: {Received}", dataToVerify, expectedSignature, payload.Signature);

            // compare signature
            bool isValid = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(payload.Signature ?? string.Empty)
            );

            if (!isValid)
            {
                _logger.LogWarning("PayOS Webhook: Chữ ký không hợp lệ! Có thể bị giả mạo.");
                return null;
            }

            return data;
        }

        // ══════════════════════════════════════════════════════════════════════
        // 5. ĐĂNG KÝ / XÁC NHẬN WEBHOOK URL
        // POST /confirm-webhook
        // ══════════════════════════════════════════════════════════════════════

        public async Task<bool> ConfirmWebhookUrlAsync(string webhookUrl)
        {
            if (string.IsNullOrWhiteSpace(webhookUrl))
                throw new ArgumentException("webhookUrl không được để trống.", nameof(webhookUrl));

            var requestBody = new { webhookUrl };

            var response = await _httpClient.PostAsJsonAsync("/confirm-webhook", requestBody, JsonOptions);
            var rawJson = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("PayOS ConfirmWebhook Response [{Status}]: {Body}", response.StatusCode, rawJson);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("PayOS ConfirmWebhook HTTP lỗi: {Status}", response.StatusCode);
                return false;
            }

            var result = JsonSerializer.Deserialize<PayOSApiResponse<object>>(rawJson, JsonOptions);
            return result?.Code == "00";
        }

        // ══════════════════════════════════════════════════════════════════════
        // 6. TIỆN ÍCH: TÍNH HMAC_SHA256
        // ══════════════════════════════════════════════════════════════════════

        public string ComputeHmacSha256(string data)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));

            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }


        private class PayOSApiResponse<T>
        {
            public string Code { get; set; } = string.Empty;
            public string Desc { get; set; } = string.Empty;
            public T? Data { get; set; }
            public string? Signature { get; set; }
        }
    }
}
