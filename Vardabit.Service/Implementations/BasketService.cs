using Microsoft.EntityFrameworkCore;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.UnitOfWork;
using Vardabit.Service.Interfaces;

namespace Vardabit.Service.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BasketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BasketDto>> GetUserBasketAsync(int userId)
        {
            var baskets = await _unitOfWork.Baskets.GetAllAsync(query =>
                query.Include(b => b.Product)
                     .Include(b => b.User)
            );

            return baskets
                .Where(b => b.UserId == userId)
                .Select(b => new BasketDto
                {
                    Id = b.Id,
                    ProductId = b.ProductId,
                    Amount = b.Amount,
                    UserId = b.UserId,
                    ProductName = b.Product.Name,
                    ProductCode = b.Product.Code,
                    UserName = b.User.UserName
                });
        }

        public async Task AddToBasketAsync(Basket basket)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Baskets.AddAsync(basket);
                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveFromBasketAsync(int basketId)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Baskets.GetByIdAsync(basketId);

                if (existing == null)
                    throw new Exception("Silinecek sepet öğesi bulunamadı!");

                _unitOfWork.Baskets.Remove(existing);
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
