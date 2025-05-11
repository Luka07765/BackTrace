using Trace.Data;
using Microsoft.EntityFrameworkCore;
using Trace.GraphQL.Mutations;
using Trace.GraphQL.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Trace.Service.Logic.File;
using Trace.Service.Logic.Folder;
using Trace.Service.Auth.Token;
using Trace.Service.Auth.GeneralAuth;
using Trace.Repository.File;
using Trace.Repository.Folder;
using Trace.Models.Auth;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

string connectionString = Environment.GetEnvironmentVariable("DATABASE_MAIN_URL") ?? "User Id=postgres.ugmsgixmsqekhvxvuxet;Password=nuGqsKetCUWjCuPX;Server=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres";
string jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? builder.Configuration["Jwt:Key"]
    ?? throw new Exception("JWT_SECRET is not set in environment or appsettings.json");

string validAudience = Environment.GetEnvironmentVariable("JWT_Audience") ?? builder.Configuration["Jwt:Audience"];
string validIssuer = Environment.GetEnvironmentVariable("JWT_Issuer") ?? builder.Configuration["Jwt:Issuer"];

// Register Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddDistributedMemoryCache();


// Register DbContext for Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register DbContextFactory for Repositories with singleton lifetimes
builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => options.UseNpgsql(connectionString),
    ServiceLifetime.Scoped
);


// Configure Identity with custom password requirements
builder.Services.AddIdentityCore<ApplicationUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 2;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    options.ClaimsIdentity.UserNameClaimType = ClaimTypes.Email;

});

// KOnfiguracija jwt tokena
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = validAudience,
        ValidIssuer = validIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret) ?? throw new Exception("JWT_SECRET is not set")),
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime = true, // Ensure token expiration is validated
        ValidateIssuerSigningKey = true,
        NameClaimType = ClaimTypes.NameIdentifier,

    };

    // Add the event handler to check for revoked tokens
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
            var accessToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(accessToken))
            {
                var isRevoked = await tokenService.IsAccessTokenRevoked(accessToken);
                if (isRevoked)
                {
                    context.Fail("Token has been revoked.");
                }
            }
        }
    };
});

// Register Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\r\n\r\nExample: \"Bearer abc123\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register Repositories and Services
builder.Services.AddScoped<IFolderRepository, FolderRepository>();
builder.Services.AddScoped<IFileRepository, FileRepository>();
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IFileService, FileService>();

// Register GraphQL
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

// ===== Add CORS services =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins("https://front-trace.vercel.app")
            .WithOrigins("https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => "Radi !");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();
