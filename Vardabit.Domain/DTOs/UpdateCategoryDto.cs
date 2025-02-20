using System.ComponentModel.DataAnnotations;

namespace Vardabit.Domain.DTOs
{
    public class UpdateCategoryDto
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kategori adı gereklidir.")]
        public string Name { get; set; }
    }
}
