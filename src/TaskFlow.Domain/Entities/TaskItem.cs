namespace TaskFlow.Domain.Entities
{
    public enum WorkItemStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }
    public class TaskItem
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public WorkItemStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        // Construtor privado — ninguém de fora consegue usar "new TaskItem()"
        private TaskItem()
        {

        }

        // Método estático público — único jeito de criar uma TaskItem
        public static TaskItem Create(string title, string? description)
        {
            // 1. Valida os dados
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Titulo não pode ser vazio.", nameof(title));


            // 2. Cria a instância (pode usar "new" aqui porque estamos DENTRO da classe)
            // 3. Preenche os dados
            // 4. Retorna
            return new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Status = WorkItemStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Complete()
        {
            if (Status == WorkItemStatus.Completed)
                throw new InvalidOperationException("Tarefa já está concuída.");

            Status = WorkItemStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

    }
}
