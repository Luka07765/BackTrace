using Microsoft.EntityFrameworkCore;
using Trace.Data;

namespace Trace.Registrations
{
    public static class DbContext_Registration
    {
        public static IServiceCollection Register_ApplicationDataBase(this IServiceCollection services, string connectionString)
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
