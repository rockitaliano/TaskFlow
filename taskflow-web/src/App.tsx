import { Routes, Route, Link } from 'react-router-dom'
import { TaskList } from './features/tasks/TaskList'
import { CreateTask } from './features/tasks/CreateTask'

export default function App() {
  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-white border-b border-gray-200 px-6 py-4">
        <Link to="/" className="text-xl font-bold text-blue-600">
          TaskFlow
        </Link>
      </nav>
      <main className="max-w-2xl mx-auto px-4 py-8">
        <Routes>
          <Route path="/" element={<TaskList />} />
          <Route path="/tasks/new" element={<CreateTask />} />
          <Route path="*" element={<p className="text-gray-500">Página não encontrada.</p>} />
        </Routes>
      </main>
    </div>
  )
}