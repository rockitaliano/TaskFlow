using MediatR;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Tasks.Queries.GetAllTasks;

public record GetAllTasksQuery() : IRequest<IReadOnlyList<TaskItem>>;
