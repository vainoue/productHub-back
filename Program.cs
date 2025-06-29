using Microsoft.EntityFrameworkCore;
using ProductAPI.Data;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

//colocar a conexão com o Supabase
var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"Username={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
                       $"SSL Mode={Environment.GetEnvironmentVariable("DB_SSLMODE")};" +
                       $"Trust Server Certificate={Environment.GetEnvironmentVariable("DB_TRUST_CERT")}";

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173", "https://product-front-beta.vercel.app").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseRouting();

app.UseCors();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}");

app.Run();