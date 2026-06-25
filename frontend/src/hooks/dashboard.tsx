import * as React from 'react'
import { api } from '@/api/client'
import { session } from '@/auth'
import type { StudentDashboardDto } from '@/types'

interface DashboardState {
  data: StudentDashboardDto | null
  loading: boolean
  error: string | null
  refresh: () => Promise<void>
}

const DashboardContext = React.createContext<DashboardState>({
  data: null,
  loading: true,
  error: null,
  refresh: async () => {},
})

/**
 * Carrega o dashboard e compartilha entre as abas (Dashboard / Repertório / Materiais).
 * Os dados chegam cacheados do Redis pela API; `refresh()` força nova leitura
 * (ex.: depois de registrar um BPM, quando o cache é invalidado no backend).
 */
export function DashboardProvider({ children }: { children: React.ReactNode }) {
  const [data, setData] = React.useState<StudentDashboardDto | null>(null)
  const [loading, setLoading] = React.useState(true)
  const [error, setError] = React.useState<string | null>(null)

  const refresh = React.useCallback(async () => {
    const student = session.get()
    if (!student) return
    try {
      const fresh = await api.dashboard(student.id)
      setData(fresh)
      setError(null)
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Erro ao carregar.')
    } finally {
      setLoading(false)
    }
  }, [])

  React.useEffect(() => {
    void refresh()
  }, [refresh])

  return (
    <DashboardContext.Provider value={{ data, loading, error, refresh }}>
      {children}
    </DashboardContext.Provider>
  )
}

export const useDashboard = () => React.useContext(DashboardContext)
