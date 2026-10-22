namespace StoreServer.models.DTO
{
    public class ProductReservationDTO
    {
        public int IdProductReservation { get; set; }
        public int IdProduct { get; set; }
        public int IdUser { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public ProductDTO Product { get; set; } = null!;
        public UserSummaryDTO User { get; set; } = null!;
    }
}
