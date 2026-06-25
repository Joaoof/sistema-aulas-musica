import { Navigate, Route, Routes } from 'react-router-dom'
import { session } from '@/auth'
import { AppShell } from '@/components/layout/AppShell'
import { AdminShell } from '@/components/admin/AdminShell'
import Login from '@/pages/Login'
import Dashboard from '@/pages/Dashboard'
import Repertoire from '@/pages/Repertoire'
import Materials from '@/pages/Materials'
import AdminLogin from '@/pages/admin/AdminLogin'
import TodayChecklist from '@/pages/admin/TodayChecklist'
import StudentsList from '@/pages/admin/StudentsList'
import StudentDetail from '@/pages/admin/StudentDetail'
import PlansPage from '@/pages/admin/PlansPage'

function RequireAuth({ children }: { children: JSX.Element }) {
  return session.get() ? children : <Navigate to="/login" replace />
}

function RequireAdmin({ children }: { children: JSX.Element }) {
  return session.isAdmin() ? children : <Navigate to="/admin/login" replace />
}

export default function App() {
  return (
    <Routes>
      {/* Aluno */}
      <Route path="/login" element={<Login />} />
      <Route
        element={
          <RequireAuth>
            <AppShell />
          </RequireAuth>
        }
      >
        <Route path="/" element={<Dashboard />} />
        <Route path="/repertorio" element={<Repertoire />} />
        <Route path="/materiais" element={<Materials />} />
      </Route>

      {/* Admin (super usuário) */}
      <Route path="/admin/login" element={<AdminLogin />} />
      <Route
        element={
          <RequireAdmin>
            <AdminShell />
          </RequireAdmin>
        }
      >
        <Route path="/admin" element={<TodayChecklist />} />
        <Route path="/admin/alunos" element={<StudentsList />} />
        <Route path="/admin/alunos/:id" element={<StudentDetail />} />
        <Route path="/admin/planos" element={<PlansPage />} />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}
