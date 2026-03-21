using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos
{
    public sealed class VoiceSidecarUpsertResult
    {
        public int StatusCode { get; }
        public Result<Guid> Result { get; }

        public VoiceSidecarUpsertResult(int statusCode, Result<Guid> result)
        {
            StatusCode = statusCode;
            Result = result;
        }
    }
}