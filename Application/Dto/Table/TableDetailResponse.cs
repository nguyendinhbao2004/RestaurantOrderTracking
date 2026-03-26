using System.Text.Json.Serialization;

namespace Application.Dto.Table
{
    public class TableDetailResponse
    {
        public Guid Id { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? QRCode { get; set; }
        public int Capacity { get; set; }

        [JsonPropertyName("Orders")]
        public List<ActiveOrderDto> Orders { get; set; } = new();

    }
}