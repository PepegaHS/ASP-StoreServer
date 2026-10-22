using System.ComponentModel.DataAnnotations;

namespace StoreServer.models.DTO
{
    public class CreateProductReservationDTO
    {
        [Range(1, int.MaxValue)]
        public int IdProduct { get; set; }
        [Range(1, int.MaxValue)]
        public int IdUser { get; set; }
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
