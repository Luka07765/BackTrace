
using Microsoft.OpenApi.Models;

using Trace.Extensions;
using Trace.GraphQL.Mutations;
using Trace.GraphQL.Mutations.Files;
using Trace.GraphQL.Mutations.Folders;
using Trace.GraphQL.Queries;
using Trace.GraphQL.Queries.Files;
using Trace.GraphQL.Queries.Folders;
using Trace.GraphQL.Subscriptions;
using Trace.Repository.Files.Fetch;
using Trace.Repository.Files.Modify;
using Trace.Repository.Folder.Fetch.Progressive;
using Trace.Repository.Folder.Fetch.Query;
using Trace.Repository.Folder.Modify;
using Trace.Repository.TagSystem.Tag;
using Trace.Service.Files.Fetch;
using Trace.Service.Files.Modify;
using Trace.Service.Folder.Fetch.Progressive;
using Trace.Service.Folder.Fetch.Query;
using Trace.Service.Folder.Modify;
using Trace.Service.Tag;
var builder = WebApplication.CreateBuilder(args);

string connectionString =
    "User Id=postgres.ugmsgixmsqekhvxvuxet;" +
    "Password=nuGqsKetCUWjCuPX;" +
    "Host=aws-0-eu-central-1.pooler.supabase.com;" +
    "Port=5432;" +
    "Database=postgres;" +
    "Pooling=true;" +
    "Keepalive=30;" +
    "Connection Idle Lifetime=60;" +
    "Command Timeout=180;";



builder.Services.AddDistributedMemoryCache();
builder.Services.AddAppDb(connectionString);
builder.Services.AddAuth(builder.Configuration);


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
builder.Services.AddScoped<IFileQueryRepository, FileQueryRepository>();
builder.Services.AddScoped<IFileQueryService, FileQueryService>();
builder.Services.AddScoped<IFileModifyService, FileModifyService>();
builder.Services.AddScoped<IFileModifyRepository, FileModifyRepository>();
// Register Repositories and Services
builder.Services.AddScoped<IFolderProgressiveRepository, FolderProgressiveRepository>();
builder.Services.AddScoped<IFolderQueryService, FolderQueryService>();
builder.Services.AddScoped<IFolderQueryRepository, FolderQueryRepository>();
builder.Services.AddScoped<IFolderProgressiveService, FolderProgressiveService>();
builder.Services.AddScoped<IFolderModifyRepository, FolderModifyRepository>();
builder.Services.AddScoped<IFolderModifyService, FolderModifyService>();


builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();
// Register GraphQL
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddTypeExtension<QueryFolders>()             

    .AddTypeExtension<QueryFiles>()
    .AddMutationType<Mutation>()
     .AddTypeExtension<FoldersMutation>()
    .AddTypeExtension<FilesMutation>()
    .AddSubscriptionType<FolderSubscription>()

    .AddInMemorySubscriptions()
    .AddSocketSessionInterceptor<JwtWebSocketAuthInterceptor>()
    .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = true);

// ===== Add CORS services =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .WithOrigins("https://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();
app.UseWebSockets();
app.UseHttpsRedirection();
app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/", () => "Radi spajdermen!");



app.MapGraphQL();
app.MapControllers();

app.Run();


