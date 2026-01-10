using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Trace.Models.Account;
using Trace.Service.Auth.GeneralAuth;
using Trace.Service.Auth.Token;

namespace Trace.Registrations
{
    public static class Auth_Registration
    {
        public static IServiceCollection Register_Auth(this IServiceCollection services, IConfiguration config)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? config["Jwt:Key"]
                ?? throw new Exception("JWT_SECRET is not set in environment or appsettings.json");

            var issuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? config["Jwt:Issuer"];
            var audience = Environment.GetEnvironmentVariable("JWT_Audience") ?? config["Jwt:Audience"];

            // === Identity Configuration ===
            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<Data.ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 2;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Email;
            });

            // === Auth-related services ===
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            // === JWT Authentication (same as Program.cs) ===
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
                        var accessToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            var revoked = await tokenService.IsAccessTokenRevoked(accessToken);
                            if (revoked)
                                context.Fail("Token has been revoked.");
                        }
                    }
                };
            });

            // === Make TokenValidationParameters available for WebSocket interceptor ===
            services.AddSingleton(new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = ClaimTypes.NameIdentifier
            });

            return services;
        }
    }
}
