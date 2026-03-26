using RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos;

namespace RestaurantOrderTracking.Application.Common.Interface
{
    public interface IVoiceCommandAiParser
    {
        Task<ParsedVoiceCommandDto> ParseAsync(string inputText, CancellationToken cancellationToken = default);
    }
}
