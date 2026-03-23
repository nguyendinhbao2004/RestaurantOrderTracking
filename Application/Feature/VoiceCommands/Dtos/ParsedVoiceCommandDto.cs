namespace RestaurantOrderTracking.Application.Feature.VoiceCommands.Dtos
{
    public sealed class ParsedVoiceCommandDto
    {
        public string? Intent { get; set; }
        public string? TableNumber { get; set; }
        public List<ParsedVoiceItemDto> Items { get; set; } = new();
        public string? Note { get; set; }
    }

    public sealed class ParsedVoiceItemDto
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
