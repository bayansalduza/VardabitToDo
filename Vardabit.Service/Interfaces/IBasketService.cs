using System.Collections.Generic;
using System.Threading.Tasks;
using Vardabit.Domain.Models;

namespace Vardabit.Service.Interfaces
{
    public interface IBasketService
    {
        Task<IEnumerable<Basket>> GetUserBasketAsync(int userId);
        Task AddToBasketAsync(Basket basket);
        Task RemoveFromBasketAsync(int basketId);
    }
}
