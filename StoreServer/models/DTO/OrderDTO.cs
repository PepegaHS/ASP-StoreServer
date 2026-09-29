namespace StoreServer.models.DTO
{
    public class OrderDTO
    {
        public int IdOrder { get; set; }

        public int IdUser { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = null!;
    }
}
