using System.Threading.Tasks;
using Vardabit.Domain.Models;
using Vardabit.Infrastructure.GenericRepository;
using Vardabit.Infrastructure.Persistence;

namespace Vardabit.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VardabitDbContext _context;

        public IGenericRepository<Product> Products { get; }
        public IGenericRepository<Category> Categories { get; }
        public IGenericRepository<Basket> Baskets { get; }
        public IGenericRepository<User> Users { get; }

        public UnitOfWork(VardabitDbContext context)
        {
            _context = context;

            Products = new GenericRepository<Product>(context);
            Categories = new GenericRepository<Category>(context);
            Baskets = new GenericRepository<Basket>(context);
            Users = new GenericRepository<User>(context);
        }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
