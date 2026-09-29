namespace StoreServer.models.DTO
{
    public class CreateOrderItemDTO
    {
        public int IdOrder { get; set; }

        public int IdProduct { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtMoment { get; set; }
    }
}
