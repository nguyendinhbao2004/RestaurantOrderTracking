using MediatR;
using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Entities;
using RestaurantOrderTracking.Domain.Interface;
using RestaurantOrderTracking.Domain.Interface.Repository;

namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Commands.SaveSidecarResult
{
    public class SaveSidecarResultHandler : IRequestHandler<SaveSidecarResultCommand, Result<Guid>>
    {
        private readonly IGenericRepository<VoiceCommand> _voiceCommandRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SaveSidecarResultHandler(
            IGenericRepository<VoiceCommand> voiceCommandRepository,
            IUnitOfWork unitOfWork)
        {
            _voiceCommandRepository = voiceCommandRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(SaveSidecarResultCommand request, CancellationToken cancellationToken)
        {
            var voiceCommand = await _voiceCommandRepository.GetByIdAsync(request.VoiceCommandId, cancellationToken);
            if (voiceCommand is null)
            {
                return Result<Guid>.Failure("VoiceCommand not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.ErrorMessage))
            {
                voiceCommand.MarkAsFailed(request.ErrorMessage);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.TranscribedText))
                {
                    return Result<Guid>.Failure("TranscribedText is required when ErrorMessage is empty.");
                }

                voiceCommand.SetTranscription(request.TranscribedText, request.ConfidenceScore ?? 0f);
                voiceCommand.MarkAsCompleted();
            }

            _voiceCommandRepository.Update(voiceCommand, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success("Voice result saved.", voiceCommand.Id);
        }
    }
}
