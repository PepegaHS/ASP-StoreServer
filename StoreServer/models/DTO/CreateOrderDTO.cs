namespace StoreServer.models.DTO
{
    public class CreateOrderDTO
    {
        public int IdUser { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = null!;
    }
}
