namespace StoreServer.models.DTO
{
    public class OrderItemDTO
    {
        public int IdOrderItem { get; set; }

        public int IdOrder { get; set; }

        public int IdProduct { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtMoment { get; set; }
        public OrderSummaryDTO Order { get; set; } = null!;
        public ProductDTO Product { get; set; } = null!;
    }
}
