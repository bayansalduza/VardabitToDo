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

        public async Task<IEnumerable<Basket>> GetUserBasketAsync(int userId)
        {
            var all = await _unitOfWork.Baskets.GetAllAsync();
            return all.Where(b => b.UserId == userId);
        }

        public async Task AddToBasketAsync(Basket basket)
        {
            await _unitOfWork.Baskets.AddAsync(basket);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveFromBasketAsync(int basketId)
        {
            var existing = await _unitOfWork.Baskets.GetByIdAsync(basketId);

            if (existing == null) 
                throw new Exception("Silinecek sepet öğesi bulunamadı!");

            _unitOfWork.Baskets.Remove(existing);

            await _unitOfWork.CommitAsync();
        }
    }
}
