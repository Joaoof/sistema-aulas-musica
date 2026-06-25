import { Play } from '@phosphor-icons/react'
import { useDashboard } from '@/hooks/dashboard'
import { Badge } from '@/components/ui/badge'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import type { RepertoireStatus } from '@/types'

const statusMap: Record<RepertoireStatus, { label: string; variant: 'default' | 'amber' | 'emerald' }> = {
  ToStudy: { label: 'A estudar', variant: 'default' },
  InProgress: { label: 'Em treino', variant: 'amber' },
  Mastered: { label: 'Dominada', variant: 'emerald' },
}

export default function Repertoire() {
  const { data, loading } = useDashboard()

  return (
    <div className="space-y-4">
      <header>
        <h1 className="text-xl font-semibold tracking-tight text-zinc-100">Repertório</h1>
        <p className="text-sm text-zinc-500">Status técnico de cada peça</p>
      </header>

      {loading && <p className="text-sm text-zinc-500">Carregando…</p>}

      {data && (
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Peça</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Vídeo</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {data.repertoire.map((r) => {
              const s = statusMap[r.status]
              return (
                <TableRow key={r.id}>
                  <TableCell>
                    <p className="font-medium text-zinc-100">{r.title}</p>
                    <p className="text-xs text-zinc-500">{r.composer}</p>
                  </TableCell>
                  <TableCell>
                    <Badge variant={s.variant}>{s.label}</Badge>
                  </TableCell>
                  <TableCell className="text-right">
                    {r.videoUrl ? (
                      <a
                        href={r.videoUrl}
                        target="_blank"
                        rel="noreferrer"
                        className="inline-flex items-center gap-1 text-amber-500 hover:text-amber-400"
                      >
                        <Play size={16} weight="fill" />
                      </a>
                    ) : (
                      <span className="text-zinc-600">—</span>
                    )}
                  </TableCell>
                </TableRow>
              )
            })}
          </TableBody>
        </Table>
      )}
    </div>
  )
}
