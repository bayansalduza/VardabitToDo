using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.UnitOfWork;
using Vardabit.Service.Interfaces;

namespace Vardabit.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }



        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync(query => query.Include(c => c.Products));
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }

        public async Task AddAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(category.Id);
            if (existing == null) throw new Exception("Kategori bulunamadı!");
            existing.Name = category.Name;
            _unitOfWork.Categories.Update(existing);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existing == null) throw new Exception("Silinecek kategori bulunamadı!");
            _unitOfWork.Categories.Remove(existing);
            await _unitOfWork.CommitAsync();
        }
    }
}
