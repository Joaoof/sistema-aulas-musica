import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { CheckSquare, Users, Stack, SignOut, type Icon } from '@phosphor-icons/react'
import { session } from '@/auth'
import { cn } from '@/lib/utils'

const items: { to: string; label: string; icon: Icon; end?: boolean }[] = [
  { to: '/admin', label: 'Hoje', icon: CheckSquare, end: true },
  { to: '/admin/alunos', label: 'Alunos', icon: Users },
  { to: '/admin/planos', label: 'Planos', icon: Stack },
]

export function AdminShell() {
  const navigate = useNavigate()
  const admin = session.get()

  function logout() {
    session.clear()
    navigate('/admin/login', { replace: true })
  }

  return (
    <div className="min-h-dvh">
      <header className="sticky top-0 z-40 border-b border-zinc-800 bg-zinc-950/90 backdrop-blur-md">
        <div className="mx-auto flex max-w-5xl items-center justify-between px-4 py-3">
          <div className="flex items-center gap-2">
            <span className="text-sm font-semibold text-amber-500">● Painel</span>
            <span className="hidden text-xs text-zinc-500 sm:inline">{admin?.name}</span>
          </div>
          <nav className="flex items-center gap-1">
            {items.map(({ to, label, icon: Icon, end }) => (
              <NavLink
                key={to}
                to={to}
                end={end}
                className={({ isActive }) =>
                  cn(
                    'flex items-center gap-1.5 rounded-lg px-3 py-1.5 text-sm font-medium transition-colors',
                    isActive
                      ? 'bg-zinc-800 text-amber-500'
                      : 'text-zinc-400 hover:bg-zinc-900 hover:text-zinc-100',
                  )
                }
              >
                {({ isActive }) => (
                  <>
                    <Icon size={18} weight={isActive ? 'fill' : 'regular'} />
                    <span className="hidden sm:inline">{label}</span>
                  </>
                )}
              </NavLink>
            ))}
            <button
              onClick={logout}
              className="ml-1 rounded-lg p-1.5 text-zinc-400 transition hover:bg-zinc-900 hover:text-zinc-100"
              aria-label="Sair"
            >
              <SignOut size={18} />
            </button>
          </nav>
        </div>
      </header>

      <main className="mx-auto max-w-5xl px-4 py-6">
        <Outlet />
      </main>
    </div>
  )
}
