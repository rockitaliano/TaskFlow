using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ─── Serviços ───────────────────────────────────────────

// Registra o EF Core com SQLite
// O banco será criado como arquivo "taskflow.db" na pasta do projeto
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=taskflow.db"));

// Registra o repositório:
// "Quando alguém pedir ITaskRepository, entregue TaskRepository"
// AddScoped = uma instância por requisição HTTP
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Registra o MediatR — ele vai descobrir todos os Handlers automaticamente
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly));



// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Serializa enums como string ("Pending") em vez de número (0)
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Aplica as migrations automaticamente ao iniciar
// (cria o banco e as tabelas se não existirem)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();           // Gera o JSON da especificação em /openapi/v1.json
    app.MapScalarApiReference(); // Interface visual em /scalar/v1
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
