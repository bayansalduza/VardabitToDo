using System.Collections.Generic;
using Vardabit.Domain.DTOs;

namespace Vardabit.Domain.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public IEnumerable<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
