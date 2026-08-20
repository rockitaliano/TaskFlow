using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Infrastructure.Repositories;

public class TaskRepository(AppDbContext context) : ITaskRepository
{
    public async Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {

        await context.Tasks.AddAsync(task, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await context.Tasks.FindAsync(new object[] { id }, cancellationToken);
        if (task is null)
            throw new InvalidOperationException($"Tarefa{id} não encontrada.");

        context.Tasks.Remove(task);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Tasks
            .AsNoTracking()
            .OrderByDescending(t => t.CreatedAt) //mais recente primeiro
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Tasks.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        context.Tasks.Update(task);
        await context.SaveChangesAsync(cancellationToken);
    }
}
