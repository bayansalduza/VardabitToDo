using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Vardabit.Domain.Models;

[Table("Basket")]
public partial class Basket
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Amount { get; set; }

    public int UserId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Baskets")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Baskets")]
    public virtual User User { get; set; } = null!;
}
