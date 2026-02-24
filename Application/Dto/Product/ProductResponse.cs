namespace Application.Dto.Product
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
    }
}