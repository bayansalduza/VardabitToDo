using System.Threading.Tasks;
using Vardabit.Domain.Models;

namespace Vardabit.Service.Interfaces
{
    public interface IUserService
    {
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task<User> GetByIdAsync(int id);
        Task<string> LoginAsync(string username, string password);
    }
}
