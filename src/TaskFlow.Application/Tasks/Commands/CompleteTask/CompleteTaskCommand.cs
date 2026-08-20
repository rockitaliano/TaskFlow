using MediatR;

namespace TaskFlow.Application.Tasks.Commands.CompleteTask;

public record CompleteTaskCommand(Guid Id) : IRequest<bool>;
