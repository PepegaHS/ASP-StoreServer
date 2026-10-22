namespace StoreServer.models;

public class ProductReservation
{
    public int IdProductReservation { get; set; }
    public int IdProduct { get; set; }
    public int IdUser { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public virtual Product Product { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
