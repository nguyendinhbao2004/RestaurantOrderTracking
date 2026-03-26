using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantOrderTracking.Application.Common.Interface;
using RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RestaurantOrderTracking.Infrastructure.Services
{
    public class VoiceCommandAiParser : IVoiceCommandAiParser
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;
        private readonly ILogger<VoiceCommandAiParser> _logger;
        private readonly string _apiKey;
        private readonly string _model;

        public VoiceCommandAiParser(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<VoiceCommandAiParser> logger)
        {
            _httpClient = httpClientFactory.CreateClient("OpenAI");
            _logger = logger;

            _apiKey = configuration["OPENAI_API_KEY"]
                ?? throw new InvalidOperationException("OPENAI_API_KEY is not configured.");
            _model = configuration["OPENAI_MODEL"] ?? "gpt-4o-mini";

            _httpClient.BaseAddress = new Uri("https://api.openai.com/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<ParsedVoiceCommandDto> ParseAsync(string inputText, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(inputText))
            {
                return new ParsedVoiceCommandDto();
            }

            var systemPrompt = "You are a restaurant voice command parser. Return ONLY valid JSON with this schema: {\"intent\":string,\"tableNumber\":string|null,\"items\":[{\"productName\":string,\"quantity\":number}],\"note\":string|null}. Supported intents: add_item, unknown. If unsure, set intent=unknown and keep items empty.";
            var userPrompt = $"Input: {inputText}";

            var payload = new
            {
                model = _model,
                response_format = new { type = "json_object" },
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.1
            };

            var requestContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("v1/chat/completions", requestContent, cancellationToken);
            var raw = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenAI parse failed with status {StatusCode}: {Body}", response.StatusCode, raw);
                return new ParsedVoiceCommandDto();
            }

            var chatResponse = JsonSerializer.Deserialize<ChatCompletionResponse>(raw, JsonOptions);
            var content = chatResponse?.Choices?.FirstOrDefault()?.Message?.Content;
            if (string.IsNullOrWhiteSpace(content))
            {
                return new ParsedVoiceCommandDto();
            }

            try
            {
                var parsed = JsonSerializer.Deserialize<ParsedVoiceCommandDto>(content, JsonOptions);
                return parsed ?? new ParsedVoiceCommandDto();
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "OpenAI returned invalid JSON: {Content}", content);
                return new ParsedVoiceCommandDto();
            }
        }

        private sealed class ChatCompletionResponse
        {
            public List<ChatChoice>? Choices { get; set; }
        }

        private sealed class ChatChoice
        {
            public ChatMessage? Message { get; set; }
        }

        private sealed class ChatMessage
        {
            public string? Content { get; set; }
        }
    }
}
