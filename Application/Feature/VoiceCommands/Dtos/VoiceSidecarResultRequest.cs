namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos
{
    public sealed class VoiceSidecarResultRequest
    {
        public Guid? VoiceCommandId { get; set; }
        public string? TranscribedText { get; set; }
        public float? ConfidenceScore { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
