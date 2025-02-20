using System.Collections.Generic;
using System.Threading.Tasks;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;

namespace Vardabit.Service.Interfaces
{
    public interface IBasketService
    {
        Task<IEnumerable<BasketDto>> GetUserBasketAsync(int userId);
        Task AddToBasketAsync(Basket basket);
        Task RemoveFromBasketAsync(int basketId);
    }
}
