using MediatR;
using RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos;

namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Commands.UpsertVoiceSidecarResult
{
    public record UpsertVoiceSidecarResultCommand(
        VoiceSidecarResultRequest? Request,
        string ProvidedApiKey) : IRequest<VoiceSidecarUpsertResult>;
}