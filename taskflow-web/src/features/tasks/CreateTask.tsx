import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { taskApi } from '../../api/taskApi'

// ===== SCHEMA ZOD =====
// Define as regras de validação
// z.infer vai extrair o tipo TypeScript automaticamente daqui
const createTaskSchema = z.object({
  title: z
    .string()
    .min(1, 'Título é obrigatório')
    .max(200, 'Máximo 200 caracteres')
    .trim(),
  description: z
    .string()
    .max(1000, 'Máximo 1000 caracteres')
    .trim()
    .optional(),
})

// Extrai o tipo TypeScript do schema — não precisa criar interface separada
// Resultado: { title: string, description?: string | undefined }
type CreateTaskFormData = z.infer<typeof createTaskSchema>

export function CreateTask() {
  // useNavigate = hook para navegar programaticamente (sem clicar em Link)
  const navigate = useNavigate()

  // useQueryClient = acessa o cache global do TanStack Query
  const queryClient = useQueryClient()

  // ===== REACT HOOK FORM =====
  const {
    register,       // conecta cada <input> ao formulário
    handleSubmit,   // wrapper: valida com Zod antes de chamar onSubmit
    formState: { errors, isSubmitting },
  } = useForm<CreateTaskFormData>({
    resolver: zodResolver(createTaskSchema),  // integra Zod com RHF
    defaultValues: { title: '', description: '' },
  })

  // ===== MUTATION =====
  const createMutation = useMutation({
    mutationFn: taskApi.create,

    onSuccess: () => {
      // Invalida o cache de 'tasks' — força a TaskList buscar dados frescos
      queryClient.invalidateQueries({ queryKey: ['tasks'] })
      navigate('/')  // volta para a lista
    },
  })

  // Chamado só após validação bem-sucedida do Zod
  const onSubmit = (data: CreateTaskFormData) => {
    createMutation.mutate(data)
  }

  return (
    <div className="max-w-lg">
      <h2 className="text-xl font-semibold text-gray-800 mb-6">Nova Tarefa</h2>

      {/* handleSubmit intercepta o submit, valida, e só chama onSubmit se tudo OK */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">

        {/* Campo Título */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Título *
          </label>
          {/* ...register('title') = espalha ref, name, onChange, onBlur no input */}
          <input
            {...register('title')}
            type="text"
            placeholder="Ex: Implementar autenticação JWT"
            className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {/* Só aparece se houver erro de validação neste campo */}
          {errors.title && (
            <p className="text-red-500 text-sm mt-1">{errors.title.message}</p>
          )}
        </div>

        {/* Campo Descrição */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Descrição
          </label>
          <textarea
            {...register('description')}
            rows={4}
            placeholder="Descreva os detalhes..."
            className="w-full border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
          {errors.description && (
            <p className="text-red-500 text-sm mt-1">{errors.description.message}</p>
          )}
        </div>

        {/* Erro da API (ex: backend fora do ar) */}
        {createMutation.isError && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-3">
            <p className="text-red-600 text-sm">Erro ao criar tarefa. Tente novamente.</p>
          </div>
        )}

        {/* Botões */}
        <div className="flex gap-3 pt-2">
          <button
            type="submit"
            disabled={isSubmitting || createMutation.isPending}
            className="flex-1 bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
          >
            {createMutation.isPending ? 'Criando...' : 'Criar Tarefa'}
          </button>

          <button
            type="button"
            onClick={() => navigate('/')}
            className="px-4 py-2 border border-gray-300 rounded-lg text-gray-700 hover:bg-gray-50 transition-colors"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}