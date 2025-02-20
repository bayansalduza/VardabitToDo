using System.Collections.Generic;
using System.Threading.Tasks;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;

namespace Vardabit.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
