using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.UnitOfWork;
using Vardabit.Service.Interfaces;

namespace Vardabit.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            _logger.LogWarning("GetAllAsync started for fetching products");

            try
            {
                var products = await _unitOfWork.Products.GetAllAsync(query =>
                    query.Include(p => p.Category)
                );

                var productDtos = products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                    Amount = p.Amount,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name
                });

                _logger.LogWarning("GetAllAsync completed. Fetched {Count} products", productDtos.Count());
                return productDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetAllAsync while fetching products");
                throw;
            }
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            _logger.LogWarning("GetByIdAsync started for product Id {ProductId}", id);

            try
            {
                var products = await _unitOfWork.Products.GetAllAsync(query =>
                    query.Include(p => p.Category)
                );

                var product = products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    _logger.LogWarning("GetByIdAsync: Product with Id {ProductId} not found", id);
                    throw new Exception("Ürün bulunamadı!");
                }

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Code = product.Code,
                    Amount = product.Amount,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name ?? string.Empty
                };

                _logger.LogWarning("GetByIdAsync completed for product Id {ProductId}", id);
                return productDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetByIdAsync for product Id {ProductId}", id);
                throw;
            }
        }

        public async Task AddAsync(Product product)
        {
            _logger.LogWarning("AddAsync started. Attempting to add product with Name {ProductName}", product.Name);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("AddAsync completed. Product added successfully with Id {ProductId}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddAsync while adding product with Name {ProductName}", product.Name);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            _logger.LogWarning("UpdateAsync started for product Id {ProductId}", product.Id);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Products.GetByIdAsync(product.Id);
                if (existing == null)
                {
                    _logger.LogWarning("UpdateAsync: Product with Id {ProductId} not found", product.Id);
                    throw new Exception("Ürün bulunamadı!");
                }

                existing.Name = product.Name;
                existing.Code = product.Code;
                existing.Amount = product.Amount;
                existing.CategoryId = product.CategoryId;

                _unitOfWork.Products.Update(existing);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("UpdateAsync completed for product Id {ProductId}", product.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in UpdateAsync for product Id {ProductId}", product.Id);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogWarning("DeleteAsync started for product Id {ProductId}", id);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Products.GetByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("DeleteAsync: Product with Id {ProductId} not found", id);
                    throw new Exception("Silinecek ürün bulunamadı!");
                }

                _unitOfWork.Products.Remove(existing);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("DeleteAsync completed. Product with Id {ProductId} removed successfully", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in DeleteAsync for product Id {ProductId}", id);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
