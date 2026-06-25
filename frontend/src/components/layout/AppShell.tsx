import { Outlet } from 'react-router-dom'
import { BottomNav } from './BottomNav'
import { DashboardProvider } from '@/hooks/dashboard'

/**
 * Casca mobile-first: conteúdo rolável + bottom nav fixa.
 * O padding inferior reserva espaço para a barra de navegação.
 */
export function AppShell() {
  return (
    <DashboardProvider>
      <div className="mx-auto min-h-dvh w-full max-w-lg">
        <main className="px-4 pb-28 pt-6">
          <Outlet />
        </main>
        <BottomNav />
      </div>
    </DashboardProvider>
  )
}
