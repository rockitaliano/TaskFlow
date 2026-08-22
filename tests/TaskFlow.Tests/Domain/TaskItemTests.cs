using FluentAssertions;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Tests.Domain;

public class TaskItemTests
{
    [Fact]
    public void Create_ComTituloValido_DeveRetornarTaskItemComStatusPending()
    {
        //Arrange - prepara os dados de entrega
        var titulo = "Implementar autenticação";
        var descrição = "Usar JWT com refresh token";

        //Act - executa o metdo que queremos testar
        var task = TaskItem.Create(titulo, descrição);

        // Assert - verifica cada propriedade
        // ".Should()" vem do FluentAssertions - lê-se como inglês natural

        task.Id.Should().NotBeEmpty(); //Guid não pode ser all-zeros
        task.Title.Should().Be(titulo);
        task.Description.Should().Be(descrição);
        task.Status.Should().Be(WorkItemStatus.Pending);
        task.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        task.CompletedAt.Should().BeNull(); //Ainda não foi concluida


    }

    [Fact]
    public void Create_ComTituloVazio_DeveLancarArgumentException()
    {
        // Arrange
        var tituloVazio = "";

        // Act — "Action" é um delegate que encapsula a chamada que pode lançar exceção
        // Não chamamos TaskItem.Create() diretamente pois a exceção quebraria o teste
        var act = () => TaskItem.Create(tituloVazio, null);

        // Assert — verifica que a exceção foi lançada com a mensagem certa
        act.Should().Throw<ArgumentException>();
        //.WithMessage("Titulo não pode ser vazio.", nameof(tituloVazio));  // "*" = wildcard, não precisa ser exata
    }

    [Fact]
    public void Create_ComTituloSomenteEspacos_DeveLancarArgumentException()
    {
        // Valida o IsNullOrWhiteSpace — "   " não é um título válido
        var act = () => TaskItem.Create("   ", null);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Complete_DeveAlterarStatusParaCompletedESetarCompletedAt()
    {
        // Arrange — cria uma task válida primeiro
        var task = TaskItem.Create("Minha tarefa", null);

        // Act — chama o método que queremos testar
        task.Complete();

        // Assert
        task.Status.Should().Be(WorkItemStatus.Completed);
        task.CompletedAt.Should().NotBeNull();
        task.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Complete_EmTaskJaConcluida_DeveLancarInvalidOperationException()
    {
        // Arrange — cria e já completa a task
        var task = TaskItem.Create("Minha tarefa", null);
        task.Complete(); // primeira vez — OK

        // Act — segunda vez deve lançar exceção
        var act = () => task.Complete();

        // Assert
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Tarefa já está concuída.");
    }
}
