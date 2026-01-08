using Supabase;
using Trace.Registrations;


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





builder.Services.Register_ApplicationDataBase(connectionString);
builder.Services.Register_Auth(builder.Configuration);
builder.Services.Register_GraphQLServer();
builder.Services.Register_SwaggerDocs();
builder.Services.Register_CorsPolicy();
builder.Services.Register_QueryAndModify();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton(provider =>
{
    return new Supabase.Client(
        builder.Configuration["Supabase:Url"],
        builder.Configuration["Supabase:ServiceKey"],
        new SupabaseOptions
        {
            AutoConnectRealtime = false
        }
    );
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


