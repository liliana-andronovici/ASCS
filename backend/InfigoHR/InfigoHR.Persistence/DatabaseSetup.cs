using EvolveDb;
using Npgsql;

namespace InfigoHR.Persistence;

public static class DatabaseSetup
{
    public static void MigrateDatabase(string connectionString)
    {
        EnsureDatabaseExists(connectionString);

        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            var evolve = new Evolve(connection, msg => Console.WriteLine(msg))
            {
                Locations = new[] { Path.Combine(AppContext.BaseDirectory, "Migrations") },
                IsEraseDisabled = true
            };
            evolve.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Database migration failed: " + ex.Message);
            throw;
        }
    }

    private static void EnsureDatabaseExists(string connectionString)
    {
        var csBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = csBuilder.Database;
        csBuilder.Database = "postgres";

        using var connection = new NpgsqlConnection(csBuilder.ConnectionString);
        connection.Open();

        using var checkCommand = new NpgsqlCommand(
            "SELECT 1 FROM pg_database WHERE datname = @name", connection);
        checkCommand.Parameters.AddWithValue("name", databaseName!);
        var exists = checkCommand.ExecuteScalar() != null;

        if (!exists)
        {
            using var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
            createCommand.ExecuteNonQuery();
            Console.WriteLine($"Database '{databaseName}' did not exist, so it was created automatically.");
        }
    }
}