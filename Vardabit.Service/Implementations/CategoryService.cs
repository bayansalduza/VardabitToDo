using Microsoft.EntityFrameworkCore;
using Vardabit.Domain.DTOs;
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

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories
                .GetAllAsync(query => query.Include(c => c.Products));

            var categoryDTOs = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    Amount = p.Amount,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty
                }).ToList()
            });

            return categoryDTOs;
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(query =>
                query.Include(c => c.Products)
            );

            var category = categories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                throw new Exception("Kategori bulunamadı!");

            var categoryDto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    Amount = p.Amount,
                    CategoryId = p.CategoryId,
                    CategoryName = category.Name
                }).ToList()
            };

            return categoryDto;
        }

        public async Task AddAsync(Category category)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Category category)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Categories.GetByIdAsync(category.Id);
                if (existing == null)
                    throw new Exception("Kategori bulunamadı!");

                existing.Name = category.Name;
                _unitOfWork.Categories.Update(existing);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Categories.GetByIdAsync(id);
                if (existing == null)
                    throw new Exception("Silinecek kategori bulunamadı!");

                _unitOfWork.Categories.Remove(existing);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
