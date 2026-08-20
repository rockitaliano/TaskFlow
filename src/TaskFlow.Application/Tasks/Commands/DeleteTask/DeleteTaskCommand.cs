using MediatR;

namespace TaskFlow.Application.Tasks.Commands.DeleteTask;

public record DeleteTaskCommand(Guid Id) : IRequest<bool>;
