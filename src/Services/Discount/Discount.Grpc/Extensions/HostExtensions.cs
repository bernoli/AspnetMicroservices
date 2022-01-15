using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Threading;

namespace Discount.Grpc.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            var retryForAvailability = retry.Value;
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger<TContext>>();
            try
            {
                logger.LogInformation("Migrating postgresql database.");
                using var connection = new NpgsqlConnection(configuration["DatabaseSettings:ConnectionString"]);
                connection.Open();

                using var command = connection.CreateCommand();
                command.CommandText = "DROP TABLE IF EXISTS Coupon";
                command.ExecuteNonQuery();

                command.CommandText = @"CREATE TABLE Coupon(
		                                        ID SERIAL PRIMARY KEY         NOT NULL,
		                                        ProductName     VARCHAR(24) NOT NULL,
		                                        Description     TEXT,
		                                        Amount          INT
                                    	);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) " +
                                      "VALUES ('IPhone X', 'IPhone Discount', 151);";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Coupon (ProductName, Description, Amount) " +
                                      "VALUES ('Samsung 10', 'Samsung Discount', 100);";
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                logger.LogError(ex, "An error occurred while migrating the postgresql database.");
                if (retryForAvailability < 12) //todo: use Polly
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    MigrateDatabase<TContext>(host, retryForAvailability);
                }
            }

            return host;
        }
    }
}
