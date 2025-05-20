using Database.Workshop.Data;
using Database.Workshop.Models;

namespace Database.Workshop.Repository;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    Task<IEnumerable<TimeSpan>> BenchmarkIndividualAsync();
    Task<IEnumerable<TimeSpan>> BenchmarkBatchAsync();
}