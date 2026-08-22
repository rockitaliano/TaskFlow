using FluentAssertions;
using Moq;
using TaskFlow.Application.Tasks.Commands.CreateTask;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Tests.Application
{
    public class CreateTaskCommandHandlerTests
    {
        // "Mock<ITaskRepository>" cria uma versão falsa da interface
        // Você define o que ela retorna — sem banco, sem rede, instantâneo
        private readonly Mock<ITaskRepository> _repositoryMock;
        private readonly CreateTaskCommandHandler _handler;

        // Construtor do xUnit — roda antes de CADA teste (isolamento garantido)
        public CreateTaskCommandHandlerTests()
        {
            _repositoryMock = new Mock<ITaskRepository>();

            // Passa o mock.Object (a instância falsa) para o handler
            _handler = new CreateTaskCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveRetornarGuidNaoVazio()
        {
            // Arrange
            var command = new CreateTaskCommand("Nova tarefa", "Descrição opcional");

            // Configura o mock: quando AddAsync for chamado com QUALQUER TaskItem,
            // simplesmente retorna (não faz nada, não joga exceção)
            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var id = await _handler.Handle(command, CancellationToken.None);

            // Assert
            id.Should().NotBeEmpty(); // Guid gerado não pode ser all-zeros
        }

        [Fact]
        public async Task Handle_ComDadosValidos_DeveAdicionarTaskNoRepositorio()
        {
            // Arrange
            var command = new CreateTaskCommand("Nova tarefa", null);

            _repositoryMock
                .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert — VERIFICA SE O MÉTODO FOI CHAMADO (não só se retornou certo)
            // Times.Once = deve ter sido chamado exatamente 1 vez
            _repositoryMock.Verify(
                r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ComTituloVazio_DeveLancarArgumentException()
        {
            // Arrange — título vazio vai quebrar no TaskItem.Create() dentro do handler
            var command = new CreateTaskCommand("", null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }
    }
}
