using System.ComponentModel.DataAnnotations;

namespace Vardabit.Domain.DTOs
{
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Ürün adı gereklidir.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ürün kodu gereklidir.")]
        [MaxLength(50, ErrorMessage = "Ürün kodu en fazla 50 karakter olabilir.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Amount gereklidir.")]
        public int Amount { get; set; }

        [Required(ErrorMessage = "CategoryId gereklidir.")]
        public int CategoryId { get; set; }

        public int? BasketId { get; set; }
    }
}
