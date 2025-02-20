using System.ComponentModel.DataAnnotations;

namespace Vardabit.API.DTOs
{
    public class CreateBasketDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public int UserId { get; set; }
    }
}
