using MediatR;
using Microsoft.AspNetCore.Mvc;
using RestaurantOrderTracking.Application.Feature.VoiceCommands.Commands.UpsertVoiceSidecarResult;
using RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos;

namespace RestaurantOrderTracking.WebApi.Controllers
{
    /// <summary>
    /// Receives callback results from Voice AI sidecar and forwards processing to Application layer.
    /// </summary>
    [Route("api/voice-sidecar")]
    [ApiController]
    public class VoiceSidecarController : ControllerBase
    {
        private const string ApiKeyHeaderName = "X-API-Key";

        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoiceSidecarController"/> class.
        /// </summary>
        /// <param name="mediator">Mediator used to forward requests to Application handlers.</param>
        public VoiceSidecarController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Sidecar AI callback endpoint: receive STT + parsed intent result and persist to VoiceCommands.
        /// </summary>
        [HttpPost("result")]
        public async Task<IActionResult> UpsertVoiceResult([FromBody] VoiceSidecarResultRequest request, CancellationToken cancellationToken)
        {
            var providedApiKey = Request.Headers[ApiKeyHeaderName].ToString();
            var command = new UpsertVoiceSidecarResultCommand(request, providedApiKey);
            var response = await _mediator.Send(command, cancellationToken);

            return StatusCode(response.StatusCode, response.Result);
        }
    }
}