﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Vardabit.Domain.Models;

namespace Vardabit.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> GetByIdAsync(int id);
        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task DeleteAsync(int id);
    }
}
