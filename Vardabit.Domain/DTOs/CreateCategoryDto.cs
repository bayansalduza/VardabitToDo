using System.ComponentModel.DataAnnotations;

namespace Vardabit.Domain.DTOs
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Kategori adı gereklidir.")]
        public string Name { get; set; }
    }
}
