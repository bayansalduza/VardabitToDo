using Vardabit.Domain.Models;
using Vardabit.Infrastructure.GenericRepository;

namespace Vardabit.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Basket> Baskets { get; }
        IGenericRepository<User> Users { get; }
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
