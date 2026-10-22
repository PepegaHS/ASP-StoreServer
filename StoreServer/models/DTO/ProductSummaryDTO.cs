namespace StoreServer.models.DTO
{
    public class ProductSummaryDTO
    {
        public int IdProduct { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
        public int AvailableQuantity { get; set; }
    }
}
