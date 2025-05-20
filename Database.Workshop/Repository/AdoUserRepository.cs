using System.Diagnostics;
using Database.Workshop.Data;
using Database.Workshop.Models;
using Npgsql;

namespace Database.Workshop.Repository;

public class AdoUserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        var users = new List<User>();
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("SELECT * FROM \"Users\"", conn);
        var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return users;
    }

    public async Task<User> GetByIdAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("SELECT * FROM \"Users\" WHERE \"Id\" = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
        }

        return null;
    }

    public async Task AddAsync(User user)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("INSERT INTO \"Users\" (\"Id\", \"Name\") VALUES (@id, @name)", conn);
        cmd.Parameters.AddWithValue("name", user.Name);
        cmd.Parameters.AddWithValue("id", user.Id);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(User user)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("UPDATE \"Users\" SET \"Name\" = @name WHERE \"Id\" = @id", conn);
        cmd.Parameters.AddWithValue("id", user.Id);
        cmd.Parameters.AddWithValue("name", user.Name);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = new NpgsqlCommand("DELETE FROM \"Users\" WHERE \"Id\" = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<IEnumerable<TimeSpan>> BenchmarkIndividualAsync()
    {
        var results = new List<TimeSpan>();
        var stopwatch = new Stopwatch();
        var users = Enumerable.Range(1, 10_00).Select(i => new User { Id = i, Name = $"User {i}" }).ToList();

        stopwatch.Start();
        foreach (var user in users)
            await AddAsync(user);
        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        foreach (var user in users)
        {
            user.Name += " Updated";
            await UpdateAsync(user);
        }

        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        foreach (var user in users)
            await DeleteAsync(user.Id);
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
        foreach (var user in users)
        {
            var cmd = new NpgsqlCommand("INSERT INTO \"Users\" (\"Id\", \"Name\") VALUES (@id, @name)", conn);
            cmd.Parameters.AddWithValue("id", user.Id);
            cmd.Parameters.AddWithValue("name", user.Name);
            await cmd.ExecuteNonQueryAsync();
        }

        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        foreach (var user in users)
        {
            user.Name += " Updated";
            var cmd = new NpgsqlCommand("UPDATE \"Users\" SET \"Name\" = @name WHERE \"Id\" = @id", conn);
            cmd.Parameters.AddWithValue("id", user.Id);
            cmd.Parameters.AddWithValue("name", user.Name);
            await cmd.ExecuteNonQueryAsync();
        }

        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        stopwatch.Restart();
        foreach (var user in users)
        {
            var cmd = new NpgsqlCommand("DELETE FROM \"Users\" WHERE \"Id\" = @id", conn);
            cmd.Parameters.AddWithValue("id", user.Id);
            await cmd.ExecuteNonQueryAsync();
        }

        stopwatch.Stop();
        results.Add(stopwatch.Elapsed);

        return results;
    }
}