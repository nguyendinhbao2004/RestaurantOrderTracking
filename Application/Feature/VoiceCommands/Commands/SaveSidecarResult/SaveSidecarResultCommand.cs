using MediatR;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Commands.SaveSidecarResult
{
    public record SaveSidecarResultCommand(
        Guid VoiceCommandId,
        string? TranscribedText,
        float? ConfidenceScore,
        string? ErrorMessage) : IRequest<Result<Guid>>;
}
