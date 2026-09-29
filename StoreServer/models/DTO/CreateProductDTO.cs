namespace StoreServer.models.DTO
{
    public class CreateProductDTO
    {
        
        public string ProductName { get; set; } = null!;

        public string? ProductDescription { get; set; }

        public decimal Price { get; set; }

        public int IdCategory { get; set; }

    }
}
