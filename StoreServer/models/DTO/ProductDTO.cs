namespace StoreServer.models.DTO
{
    public class ProductDTO
    {
        public int IdProduct { get; set; }

        public string ProductName { get; set; } = null!;

        public string? ProductDescription { get; set; }

        public decimal Price { get; set; }

        public int IdCategory { get; set; }

        
    }
}
