using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(Product product)
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

        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(id);
            if (existing == null) throw new Exception("Silinecek ürün bulunamadı!");
            _unitOfWork.Products.Remove(existing);
            await _unitOfWork.CommitAsync();
        }
    }
}
