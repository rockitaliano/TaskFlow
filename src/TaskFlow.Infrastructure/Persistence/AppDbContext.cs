using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Representa a tabela "Tasks" no banco.
    // É por aqui que você faz queries: _context.Tasks.Where(...).ToListAsync()
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
