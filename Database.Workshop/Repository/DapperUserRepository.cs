using System.Diagnostics;
using Dapper;
using Database.Workshop.Data;
using Npgsql;

namespace Database.Workshop.Repository;

public class DapperUserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var sql = "SELECT \"Id\", \"Name\" FROM \"Users\"";
        var users = await conn.QueryAsync<User>(sql);
        return users;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var sql = "SELECT \"Id\", \"Name\" FROM \"Users\" WHERE \"Id\" = @Id";
        return await conn.QueryFirstOrDefaultAsync<User>(sql, new { Id = id });
    }

    public async Task AddAsync(User user)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var sql = "INSERT INTO \"Users\" (\"Name\") VALUES (@Name)";
        await conn.ExecuteAsync(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var sql = "UPDATE \"Users\" SET \"Name\" = @Name WHERE \"Id\" = @Id";
        await conn.ExecuteAsync(sql, user);
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        var sql = "DELETE FROM \"Users\" WHERE \"Id\" = @Id";
        await conn.ExecuteAsync(sql, new { Id = id });
    }
    public async Task<IEnumerable<TimeSpan>> BenchmarkIndividualAsync()
    {
        var results = new List<TimeSpan>();
        var users = Enumerable.Range(1, 10_00).Select(i => new User { Id = i, Name = $"User {i}" }).ToList();
        var stopwatch = new Stopwatch();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        stopwatch.Start();
        foreach (var user in users)
            await conn.ExecuteAsync("INSERT INTO \"Users\" (\"Id\", \"Name\") VALUES (@Id, @Name)", user);
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        foreach (var user in users)
        {
            user.Name += " Updated";
            await conn.ExecuteAsync("UPDATE \"Users\" SET \"Name\" = @Name WHERE \"Id\" = @Id", user);
        }
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        foreach (var user in users)
            await conn.ExecuteAsync("DELETE FROM \"Users\" WHERE \"Id\" = @Id", user);
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        return results;
    }


    public async Task<IEnumerable<TimeSpan>> BenchmarkBatchAsync()
    {
        var results = new List<TimeSpan>();
        var users = Enumerable.Range(1, 10_00).Select(i => new User { Id = i, Name = $"User {i}" }).ToList();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        var stopwatch = Stopwatch.StartNew();
        await conn.ExecuteAsync("INSERT INTO \"Users\" (\"Id\", \"Name\") VALUES (@Id, @Name)", users);
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        users.ForEach(u => u.Name += " Updated");
        stopwatch.Restart();
        foreach (var user in users)
            await conn.ExecuteAsync("UPDATE \"Users\" SET \"Name\" = @Name WHERE \"Id\" = @Id", user);
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        await conn.ExecuteAsync("DELETE FROM \"Users\" WHERE \"Id\" = @Id", users);
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        return results;
    }

}