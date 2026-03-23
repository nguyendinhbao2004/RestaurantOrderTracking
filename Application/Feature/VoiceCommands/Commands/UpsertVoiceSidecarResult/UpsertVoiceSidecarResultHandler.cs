using MediatR;
using Microsoft.Extensions.Configuration;
using RestaurantOrderTracking.Application.Feature.VoiceCommands.Commands.SaveSidecarResult;
using RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos;
using RestaurantOrderTracking.Domain.Common;

namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Commands.UpsertVoiceSidecarResult
{
    public class UpsertVoiceSidecarResultHandler : IRequestHandler<UpsertVoiceSidecarResultCommand, VoiceSidecarUpsertResult>
    {
        private const int Status200Ok = 200;
        private const int Status400BadRequest = 400;
        private const int Status401Unauthorized = 401;
        private const int Status404NotFound = 404;
        private const int Status500InternalServerError = 500;

        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public UpsertVoiceSidecarResultHandler(
            IMediator mediator,
            IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        public async Task<VoiceSidecarUpsertResult> Handle(UpsertVoiceSidecarResultCommand request, CancellationToken cancellationToken)
        {
            var expectedApiKey = _configuration["DOTNET_API_KEY"];
            if (string.IsNullOrWhiteSpace(expectedApiKey))
            {
                return new VoiceSidecarUpsertResult(
                    Status500InternalServerError,
                    Result<Guid>.Failure("DOTNET_API_KEY is not configured on .NET API."));
            }

            if (!string.Equals(request.ProvidedApiKey, expectedApiKey, StringComparison.Ordinal))
            {
                return new VoiceSidecarUpsertResult(
                    Status401Unauthorized,
                    Result<Guid>.Failure("Invalid X-API-Key."));
            }

            if (request.Request is null)
            {
                return new VoiceSidecarUpsertResult(
                    Status400BadRequest,
                    Result<Guid>.Failure("Request body is required."));
            }

            if (!request.Request.VoiceCommandId.HasValue)
            {
                return new VoiceSidecarUpsertResult(
                    Status400BadRequest,
                    Result<Guid>.Failure("VoiceCommandId is required."));
            }

            var saveCommand = new SaveSidecarResultCommand(
                request.Request.VoiceCommandId.Value,
                request.Request.TranscribedText,
                request.Request.ConfidenceScore,
                request.Request.ErrorMessage);

            var saveResult = await _mediator.Send(saveCommand, cancellationToken);
            if (saveResult.Succeeded)
            {
                return new VoiceSidecarUpsertResult(Status200Ok, saveResult);
            }

            if (saveResult.Errors?.Any(e => e == "VoiceCommand not found.") == true)
            {
                return new VoiceSidecarUpsertResult(Status404NotFound, saveResult);
            }

            return new VoiceSidecarUpsertResult(Status400BadRequest, saveResult);
        }
    }
}