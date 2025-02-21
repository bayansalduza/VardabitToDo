using Microsoft.EntityFrameworkCore;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.UnitOfWork;
using Vardabit.Service.Interfaces;

namespace Vardabit.Service.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync(query =>
                query.Include(p => p.Category)
            );

            return products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code,
                Amount = p.Amount,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name
            });
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var products = await _unitOfWork.Products.GetAllAsync(query =>
                query.Include(p => p.Category)
            );

            var product = products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                throw new Exception("Ürün bulunamadı!");

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Amount = product.Amount,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty
            };
        }

        public async Task AddAsync(Product product)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(Product product)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Products.GetByIdAsync(product.Id);
                if (existing == null) throw new Exception("Ürün bulunamadı!");

                existing.Name = product.Name;
                existing.Code = product.Code;
                existing.Amount = product.Amount;
                existing.CategoryId = product.CategoryId;
                _unitOfWork.Products.Update(existing);
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
                var existing = await _unitOfWork.Products.GetByIdAsync(id);
                if (existing == null) throw new Exception("Silinecek ürün bulunamadı!");

                _unitOfWork.Products.Remove(existing);
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
