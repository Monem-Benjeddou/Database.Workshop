using Npgsql;

namespace Database.Workshop.MigrationAdo;

public class AdoMigrationService
{
    private readonly string _connectionString;

    public AdoMigrationService(IConfiguration configuration, string connectionString)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task EnsureDatabaseCreatedAsync()
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id SERIAL PRIMARY KEY,
                Name TEXT NOT NULL
            );";

        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
}
