// import.meta.env = variáveis de ambiente do Vite
// VITE_API_URL vem do arquivo .env.local (se existir)
// ?? = "se for null ou undefined, usa o valor da direita"
const API_URL = ''
//                                                         ↑ porta do seu backend

// Função auxiliar genérica para todas as requisições HTTP
// <T> = tipo do retorno (quem chama decide)
// path = "/api/tasks", "/api/tasks/123", etc.
// options = método, body, etc. — é opcional (o ? indica isso)
async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
    },
    ...options,  // "spread" — mescla as opções extras (method, body, etc.)
  })

  // fetch não lança erro em respostas 4xx/5xx — precisamos verificar manualmente
  if (!response.ok) {
    // Tenta ler a mensagem de erro do corpo, senão usa o statusText
    const error = await response.json().catch(() => ({ message: response.statusText }))
    throw new Error(error.message ?? `HTTP ${response.status}`)
  }

  // 204 No Content = resposta sem corpo (complete e delete retornam isso)
  // Se tentarmos fazer .json() em resposta vazia, vai dar erro
  if (response.status === 204) return undefined as T

  return response.json() as Promise<T>
}

// Interface TypeScript: define a "forma" do objeto Task
// Espelha exatamente o que a sua API .NET retorna
export interface Task {
  id: string
  title: string
  description?: string   // ? = opcional, pode não vir na resposta
  status: 'Pending' | 'InProgress' | 'Completed' | 'Cancelled'  // só esses 4 valores
  createdAt: string      // datas vêm como string no JSON
  completedAt?: string
}

// Objeto que agrupa todos os endpoints da API
// Exportamos para usar nos componentes React
export const taskApi = {
  // GET /api/tasks → retorna lista de tasks
  getAll: () => request<Task[]>('/api/tasks'),

  // GET /api/tasks/{id} → retorna uma task
  getById: (id: string) => request<Task>(`/api/tasks/${id}`),

  // POST /api/tasks com body JSON → retorna o id criado
  create: (data: { title: string; description?: string }) =>
    request<{ id: string }>('/api/tasks', {
      method: 'POST',
      body: JSON.stringify(data),  // converte objeto JS para string JSON
    }),

  // POST /api/tasks/{id}/complete → retorna 204, sem body
  complete: (id: string) =>
    request<void>(`/api/tasks/${id}/complete`, { method: 'POST' }),

  // DELETE /api/tasks/{id} → retorna 204, sem body
  delete: (id: string) =>
    request<void>(`/api/tasks/${id}`, { method: 'DELETE' }),
}