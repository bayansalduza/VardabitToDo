using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.UnitOfWork;
using Vardabit.Service.Interfaces;

namespace Vardabit.Service.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IUnitOfWork unitOfWork, ILogger<CategoryService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            _logger.LogWarning("GetAllAsync started for fetching categories");

            try
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
                }).ToList();

                _logger.LogWarning("GetAllAsync completed. Fetched {Count} categories", categoryDTOs.Count);
                return categoryDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllAsync while fetching categories");
                throw;
            }
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            _logger.LogWarning("GetByIdAsync started for category Id {CategoryId}", id);

            try
            {
                var categories = await _unitOfWork.Categories.GetAllAsync(query =>
                    query.Include(c => c.Products)
                );

                var category = categories.FirstOrDefault(c => c.Id == id);
                if (category == null)
                {
                    _logger.LogWarning("GetByIdAsync: Category with Id {CategoryId} not found", id);
                    throw new Exception("Kategori bulunamadı!");
                }

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

                _logger.LogWarning("GetByIdAsync completed for category Id {CategoryId}", id);
                return categoryDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetByIdAsync for category Id {CategoryId}", id);
                throw;
            }
        }

        public async Task AddAsync(Category category)
        {
            _logger.LogWarning("AddAsync started. Attempting to add category with Name {CategoryName}", category.Name);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Categories.AddAsync(category);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("AddAsync completed. Category added successfully with Id {CategoryId}", category.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddAsync while adding category with Name {CategoryName}", category.Name);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Category category)
        {
            _logger.LogWarning("UpdateAsync started for category Id {CategoryId}", category.Id);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Categories.GetByIdAsync(category.Id);
                if (existing == null)
                {
                    _logger.LogWarning("UpdateAsync: Category with Id {CategoryId} not found", category.Id);
                    throw new Exception("Kategori bulunamadı!");
                }

                existing.Name = category.Name;
                _unitOfWork.Categories.Update(existing);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("UpdateAsync completed for category Id {CategoryId}", category.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UpdateAsync for category Id {CategoryId}", category.Id);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogWarning("DeleteAsync started for category Id {CategoryId}", id);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Categories.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("DeleteAsync: Category with Id {CategoryId} not found", id);
                    throw new Exception("Silinecek kategori bulunamadı!");
                }

                _unitOfWork.Categories.Remove(existing);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("DeleteAsync completed. Category with Id {CategoryId} removed successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DeleteAsync for category Id {CategoryId}", id);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
