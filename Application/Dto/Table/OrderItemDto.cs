namespace Application.Dto.Table
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
