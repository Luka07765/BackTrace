using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Extensions
{
    public static class DbContextExtensions
    {
        public static IServiceCollection AddAppDb(this IServiceCollection services, string connectionString)
        {
            // For Identity and runtime context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null
                    );
                }));

            // For repositories (factory)
            services.AddDbContextFactory<ApplicationDbContext>(
                options => options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null
                    );
                }),
                ServiceLifetime.Scoped
            );

            return services;
        }
    }
}
