using System.ComponentModel.DataAnnotations;

namespace StoreServer.models.DTO
{
    public class PatchProductReservationDTO
    {
        [Range(1, int.MaxValue)]
        public int? Quantity { get; set; }
    }
}
