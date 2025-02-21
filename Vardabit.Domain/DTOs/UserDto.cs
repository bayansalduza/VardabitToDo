using System.ComponentModel.DataAnnotations;

namespace Vardabit.Domain.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public List<BasketDto> Baskets { get; set; } = new();
    }
}
