namespace Trace.Registrations
{
    public static class Cors_Registration
    {
        public static IServiceCollection Register_CorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                            .WithOrigins(
                            "https://localhost:3000",
                            "https://front-w89v.vercel.app"
                        )
                           .AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowCredentials();
                });
            });
            return services;
        }
    }
}
