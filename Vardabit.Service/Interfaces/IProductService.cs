using System.Collections.Generic;
using System.Threading.Tasks;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;

namespace Vardabit.Service.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllAsync();
        Task<ProductDto> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
