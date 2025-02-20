using System.ComponentModel.DataAnnotations;

namespace Vardabit.Domain.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adı gereklidir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ürün kodu gereklidir.")]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        public int Amount { get; set; }

        [Required]
        public int CategoryId { get; set; }

    }
}
