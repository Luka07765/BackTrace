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
           
           
            services.AddScoped<IAccessTokenService, AccessTokenService>();
            services.AddScoped<ITokenResponseService, TokenResponseService>();
            services.AddScoped<ITokenInvalidationService, TokenInvalidationService>();
            services.AddScoped<ITokenRefreshService, TokenRefreshService>();
            services.AddScoped<IRefreshInvalidationService, RefreshInvalidationService>();  
            services.AddScoped<ITokenRotationService, TokenRotationService>();
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
                    NameClaimType = CustomClaimTypes.UserId


                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    },

                    OnTokenValidated = async context =>
                    {
                        var tokenRevoke = context.HttpContext.RequestServices.GetRequiredService<ITokenInvalidationService>();
                        var accessToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            var revoked = await tokenRevoke.IsAccessTokenRevoked(accessToken);
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
                NameClaimType = CustomClaimTypes.UserId
            });

            return services;
        }
    }
}
