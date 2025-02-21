using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vardabit.Domain.DTOs;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.UnitOfWork;
using Vardabit.Service.Interfaces;

namespace Vardabit.Service.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BasketService> _logger;

        public BasketService(IUnitOfWork unitOfWork, ILogger<BasketService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<BasketDto>> GetUserBasketAsync(int userId)
        {
            _logger.LogWarning("GetUserBasketAsync started for user with Id {UserId}", userId);

            try
            {
                var baskets = await _unitOfWork.Baskets.GetAllAsync(query =>
                    query.Include(b => b.Product)
                         .Include(b => b.User)
                );

                var userBaskets = baskets
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
                    })
                    .ToList();

                _logger.LogWarning("GetUserBasketAsync completed for user with Id {UserId}. Found {Count} baskets", userId, userBaskets.Count);

                return userBaskets;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in GetUserBasketAsync for user with Id {UserId}", userId);
                throw;
            }
        }

        public async Task AddToBasketAsync(Basket basket)
        {
            _logger.LogWarning("AddToBasketAsync started. Attempting to add basket for user {UserId} with product {ProductId}. Basket details: {@Basket}",
                                 basket.UserId, basket.ProductId, basket);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Baskets.AddAsync(basket);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("AddToBasketAsync completed. Basket added successfully. Basket details: {@Basket}", basket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in AddToBasketAsync for user {UserId} with product {ProductId}. Basket details: {@Basket}",
                                 basket.UserId, basket.ProductId, basket);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task RemoveFromBasketAsync(int basketId)
        {
            _logger.LogWarning("RemoveFromBasketAsync started. Attempting to remove basket with Id {BasketId}", basketId);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existing = await _unitOfWork.Baskets.GetByIdAsync(basketId);
                if (existing == null)
                {
                    _logger.LogWarning("RemoveFromBasketAsync: Basket with Id {BasketId} not found", basketId);
                    throw new Exception("Silinecek sepet öğesi bulunamadı!");
                }

                _unitOfWork.Baskets.Remove(existing);
                await _unitOfWork.CommitAsync();

                _logger.LogWarning("RemoveFromBasketAsync completed. Basket with Id {BasketId} removed successfully", basketId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in RemoveFromBasketAsync for basket with Id {BasketId}", basketId);
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
