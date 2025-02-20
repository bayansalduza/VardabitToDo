using System.ComponentModel.DataAnnotations;

namespace Vardabit.Domain.DTOs
{
    public class BasketDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public int UserId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string UserName { get; set; } = null!;
    }
}
