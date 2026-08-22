// useQuery = hook para BUSCAR dados (GET)
import { useQuery } from '@tanstack/react-query'
// Link = componente de navegação do React Router (como <a> mas sem recarregar a página)
import { Link } from 'react-router-dom'
import { taskApi, type Task } from '../../api/taskApi'

export function TaskList() {
  // useQuery faz o fetch automaticamente quando o componente aparece na tela
  // queryKey: identificador do cache — se outro componente usar ['tasks'], compartilham o mesmo cache
  // queryFn: a função que busca os dados
  const {
    data: tasks,   // renomeia "data" para "tasks" — mais semântico
    isLoading,
    isError,
    error,
  } = useQuery<Task[]>({
    queryKey: ['tasks'],
    queryFn: taskApi.getAll,
  })

  // Renderização condicional — React renderiza o que você retornar
  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        {/* Spinner animado com Tailwind */}
        <div className="animate-spin h-8 w-8 border-4 border-blue-500 border-t-transparent rounded-full" />
      </div>
    )
  }

  if (isError) {
    return (
      <div className="bg-red-50 border border-red-200 rounded-lg p-4">
        <p className="text-red-600">
          Erro ao carregar tarefas: {error instanceof Error ? error.message : 'Erro desconhecido'}
        </p>
      </div>
    )
  }

  return (
    <div>
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl font-semibold text-gray-800">
          Tarefas ({tasks?.length ?? 0})
        </h2>
        {/* Link navega para a rota /tasks/new sem recarregar a página */}
        <Link
          to="/tasks/new"
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
        >
          + Nova Tarefa
        </Link>
      </div>

      {/* Lista — renderização condicional: sem tarefas mostra mensagem, com tarefas mostra lista */}
      {tasks?.length === 0 ? (
        <p className="text-gray-500 text-center py-8">Nenhuma tarefa ainda. Crie a primeira!</p>
      ) : (
        <ul className="space-y-3">
          {/* .map() transforma cada task em um elemento React */}
          {/* "key" é obrigatório em listas — React usa para atualizar o DOM eficientemente */}
          {tasks?.map((task) => (
            <li key={task.id}>
              <TaskCard task={task} />
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

// Componente filho — recebe "task" como prop (dado que o pai passa para o filho)
// { task }: { task: Task } = desestrutura a prop e define o tipo
function TaskCard({ task }: { task: Task }) {
  // Objeto como "switch" — mapeia status para classe CSS de cor
  const statusColors: Record<Task['status'], string> = {
    Pending: 'bg-yellow-100 text-yellow-800',
    InProgress: 'bg-blue-100 text-blue-800',
    Completed: 'bg-green-100 text-green-800',
    Cancelled: 'bg-gray-100 text-gray-600',
  }

  return (
    <Link to={`/tasks/${task.id}`} className="block">
      <div className="bg-white rounded-lg border border-gray-200 p-4 hover:shadow-md transition-shadow">
        <div className="flex justify-between items-start">
          <div className="flex-1">
            <h3 className="font-medium text-gray-900">{task.title}</h3>
            {/* && = "só renderiza se description existir" */}
            {task.description && (
              <p className="text-sm text-gray-500 mt-1">{task.description}</p>
            )}
          </div>
          {/* Classe dinâmica: pega a cor do objeto statusColors baseado no status da task */}
          <span className={`ml-3 px-2 py-1 text-xs rounded-full font-medium ${statusColors[task.status]}`}>
            {task.status}
          </span>
        </div>
        <p className="text-xs text-gray-400 mt-2">
          {/* Converte a string ISO para data legível em pt-BR */}
          Criada em: {new Date(task.createdAt).toLocaleDateString('pt-BR')}
        </p>
      </div>
    </Link>
  )
}