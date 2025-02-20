using System.ComponentModel.DataAnnotations;

namespace Vardabit.API.DTOs
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Kategori adı gereklidir.")]
        public string Name { get; set; }
    }
}
