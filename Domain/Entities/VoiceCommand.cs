using RestaurantOrderTracking.Domain.Common;
using RestaurantOrderTracking.Domain.Enums;
using System;

namespace RestaurantOrderTracking.Domain.Entities
{
    public class VoiceCommand : BaseEntities
    {
        public Guid AccountId { get; private set; }
        public virtual Account Account { get; private set; } = null!;

        public Guid? OrderItemId { get; private set; }
        public virtual OrderItem? OrderItem { get; private set; }

        public string AudioUrl { get; private set; } = null!;
        public string? TranscribedText { get; private set; }
        public Guid? ParsedTableId { get; private set; }
        public string? ParsedAction { get; private set; }
        public float? ConfidenceScore { get; private set; }
        public VoiceCommandStatus Status { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        protected VoiceCommand() { }

        public VoiceCommand(Guid accountId, string audioUrl, Guid? orderItemId = null)
        {
            AccountId = accountId;
            AudioUrl = audioUrl;
            OrderItemId = orderItemId;
            Status = VoiceCommandStatus.pending;
        }

        public void MarkAsProcessing()
        {
            Status = VoiceCommandStatus.processing;
        }

        public void SetTranscription(string transcribedText, float confidenceScore)
        {
            TranscribedText = transcribedText;
            ConfidenceScore = confidenceScore;
        }

        public void SetParsedResult(Guid? tableId, string? action)
        {
            ParsedTableId = tableId;
            ParsedAction = action;
        }

        public void MarkAsCompleted()
        {
            Status = VoiceCommandStatus.completed;
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkAsFailed(string errorMessage)
        {
            Status = VoiceCommandStatus.failed;
            ErrorMessage = errorMessage;
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
