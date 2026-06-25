import { Suspense, lazy } from 'react'
import { useNavigate } from 'react-router-dom'
import { CalendarDots, Barbell, SignOut, Gauge } from '@phosphor-icons/react'
import { useDashboard } from '@/hooks/dashboard'
import { session } from '@/auth'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { GlowCard } from '@/components/aceternity/GlowCard'
import { LogBpmCard } from '@/components/practice/LogBpmCard'

// Tremor/recharts é pesado -> carregado sob demanda só no Dashboard
const BpmAreaChart = lazy(() =>
  import('@/components/charts/BpmAreaChart').then((m) => ({ default: m.BpmAreaChart })),
)
const RepertoireDonut = lazy(() =>
  import('@/components/charts/RepertoireDonut').then((m) => ({ default: m.RepertoireDonut })),
)

const ChartFallback = () => <div className="h-40 animate-pulse rounded-lg bg-zinc-800/50" />

function formatLesson(iso: string | null): string {
  if (!iso) return 'Sem aula agendada'
  return new Date(iso).toLocaleString('pt-BR', {
    weekday: 'short',
    day: '2-digit',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export default function Dashboard() {
  const navigate = useNavigate()
  const student = session.get()
  const { data, loading, error } = useDashboard()

  function logout() {
    session.clear()
    navigate('/login', { replace: true })
  }

  const latestBpm = data?.bpmHistory.at(-1)?.bpm ?? 0

  return (
    <div className="space-y-6">
      {/* Header */}
      <header className="flex items-start justify-between">
        <div>
          <p className="text-xs uppercase tracking-wide text-zinc-500">Console de prática</p>
          <h1 className="text-2xl font-semibold tracking-tight text-zinc-100">{student?.name}</h1>
          <p className="text-sm text-amber-500">{student?.instrument}</p>
        </div>
        <Button variant="outline" size="icon" onClick={logout} aria-label="Sair">
          <SignOut size={18} />
        </Button>
      </header>

      {loading && <p className="text-sm text-zinc-500">Carregando telemetria…</p>}
      {error && (
        <p className="rounded-md border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-red-300">
          {error}
        </p>
      )}

      {data && (
        <>
          {/* Sprint Atual — destaque Aceternity (glow) */}
          <GlowCard>
            <div className="flex items-center justify-between">
              <Badge variant="amber" className="gap-1.5">
                <Barbell size={12} weight="fill" /> Sprint Atual
              </Badge>
              <span className="flex items-center gap-1.5 text-xs text-zinc-500">
                <Gauge size={14} weight="duotone" className="text-amber-500" />
                {latestBpm} BPM
              </span>
            </div>
            <h2 className="mt-3 text-lg font-semibold text-zinc-100">{data.currentSprint}</h2>
            <div className="mt-4 flex items-center gap-2 text-sm text-zinc-400">
              <CalendarDots size={18} weight="duotone" className="text-amber-500" />
              <span className="capitalize">Próxima aula · {formatLesson(data.nextLessonAt)}</span>
            </div>
          </GlowCard>

          {/* Painel de dados — Tremor */}
          <Card>
            <CardHeader className="pb-2">
              <CardTitle>Evolução de BPM</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <Suspense fallback={<ChartFallback />}>
                <BpmAreaChart data={data.bpmHistory} />
              </Suspense>
              <LogBpmCard />
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="pb-2">
              <CardTitle>Repertório · domínio</CardTitle>
            </CardHeader>
            <CardContent>
              <Suspense fallback={<ChartFallback />}>
                <RepertoireDonut stats={data.repertoireStats} />
              </Suspense>
            </CardContent>
          </Card>
        </>
      )}
    </div>
  )
}
