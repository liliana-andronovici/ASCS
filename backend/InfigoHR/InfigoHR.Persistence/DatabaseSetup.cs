using EvolveDb;
using Npgsql;

namespace InfigoHR.Persistence;

public static class DatabaseSetup
{
    public static void MigrateDatabase(string connectionString)
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.InvalidCatalogName)
            {
                CreateDatabase(connectionString);
                connection.Open();
            }

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

    private static void CreateDatabase(string connectionString)
    {
        var csBuilder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = csBuilder.Database;
        csBuilder.Database = "postgres";

        using var connection = new NpgsqlConnection(csBuilder.ConnectionString);
        connection.Open();

        using var createCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection);
        createCommand.ExecuteNonQuery();
        Console.WriteLine($"Database '{databaseName}' did not exist, so it was created automatically.");
    }
}
