namespace StoreServer.models.DTO
{
    // DTO for patching a product
    public class PatchProductDTO
    {

        public string? ProductName { get; set; }

        public string? ProductDescription { get; set; }

        public decimal? Price { get; set; }

        public int? IdCategory { get; set; }

        [System.ComponentModel.DataAnnotations.Range(0, int.MaxValue)]
        public int? AvailableQuantity { get; set; }
    }
}
