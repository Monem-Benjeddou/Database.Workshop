using System.Diagnostics;
using Database.Workshop.Data;
using Database.Workshop.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.Workshop.Repository;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();

    public async Task<User> GetByIdAsync(int id) => await _context.Users.FindAsync(id);

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<IEnumerable<TimeSpan>> BenchmarkIndividualAsync()
    {
        var results = new List<TimeSpan>();
        var stopwatch = new Stopwatch();

        var users = Enumerable.Range(1, 10_00).Select(i => new User { Name = $"User {i}" }).ToList();

        stopwatch.Start();
        foreach (var user in users)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        users.ForEach(u => u.Name += " Updated");
        stopwatch.Restart();
        foreach (var user in users)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        foreach (var user in users)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        return results;
    }

    public async Task<IEnumerable<TimeSpan>> BenchmarkBatchAsync()
    {
        var results = new List<TimeSpan>();
        var users = Enumerable.Range(1, 10_00).Select(i => new User { Name = $"User {i}" }).ToList();

        var stopwatch = Stopwatch.StartNew();
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        foreach (var user in users) user.Name += " Updated";
        stopwatch.Restart();
        _context.Users.UpdateRange(users);
        await _context.SaveChangesAsync();
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        _context.Users.RemoveRange(users);
        await _context.SaveChangesAsync();
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        return results;
    }

}
