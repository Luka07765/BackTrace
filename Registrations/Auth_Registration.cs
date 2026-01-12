using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Trace.Models.Account;
using Trace.Service.Auth;
using Trace.Service.Auth.GeneralAuth;
using Trace.Service.Auth.Token.Phase1_AccessToken;
using Trace.Service.Auth.Token.Phase2_RefreshToken.Refresh;
using Trace.Service.Auth.Token.Phase2_RefreshToken.Response;
using Trace.Service.Auth.Token.Phase3_Logout.InvalidateRefresh;
using Trace.Service.Auth.Token.Phase3_Logout.InvalidateToken;
using Trace.Service.Auth.Token.Phase4_Rotation;

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

            if (string.IsNullOrWhiteSpace(issuer))
                throw new Exception("JWT_Issuer/Jwt:Issuer is missing.");

            if (string.IsNullOrWhiteSpace(audience))
                throw new Exception("JWT_Audience/Jwt:Audience is missing.");

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

                // Optional; unrelated to JWT bearer validation:
                options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Email;
            });

            // === Auth-related services ===
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<ITokenResponseService, TokenResponseService>();
            services.AddScoped<IAccessInvalidationService, AccessInvalidationService>();
            services.AddScoped<ITokenRefreshService, TokenRefreshService>();
            services.AddScoped<IRefreshInvalidationService, RefreshInvalidationService>();
            services.AddScoped<ITokenRotationService, TokenRotationService>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            // === JWT Authentication ===
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

                    // IMPORTANT: your custom user id claim
                    NameClaimType = CustomClaimTypes.UserId
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenInvalidation = context.HttpContext.RequestServices.GetRequiredService<IAccessInvalidationService>();
                        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();

                        // Extract raw token (needed because your invalidation service parses JWT again)
                        var authHeader = context.Request.Headers.Authorization.ToString();
                        var accessToken = authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                            ? authHeader["Bearer ".Length..].Trim()
                            : authHeader.Trim();



                        if (string.IsNullOrWhiteSpace(accessToken))
                        {
                            context.Fail("Missing token.");
                            return;
                        }

                        // 1) JTI blacklist check (uses CustomClaimTypes.Jti in your fixed service)
                        if (await tokenInvalidation.IsAccessTokenRevoked(accessToken))
                        {
                            context.Fail("Token has been revoked.");
                            return;
                        }

                        // 2) SessionVersion enforcement (makes logout SessionVersion++ actually work)
                        var userId = context.Principal?.FindFirstValue(CustomClaimTypes.UserId);
                        if (string.IsNullOrWhiteSpace(userId))
                        {
                            context.Fail("Invalid token (missing user id).");
                            return;
                        }

                        var user = await userManager.FindByIdAsync(userId);
                        if (user == null)
                        {
                            context.Fail("User not found.");
                            return;
                        }

                        if (!tokenInvalidation.ValidateSessionVersion(context.Principal, user))
                        {
                            context.Fail("Session expired.");
                            return;
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
                NameClaimType = CustomClaimTypes.UserId
            });

            return services;
        }
    }
}
