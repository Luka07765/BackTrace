

using Trace.Extensions;


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
builder.Services.AddGraphQLServerConfig();
builder.Services.AddSwaggerDocs();
builder.Services.AddCorsPolicy();
builder.Services.AddRepositories();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();




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


