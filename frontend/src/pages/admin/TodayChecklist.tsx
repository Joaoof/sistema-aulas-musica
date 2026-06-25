import { useEffect, useState } from 'react'
import { CheckCircle, XCircle, Clock } from '@phosphor-icons/react'
import { adminApi } from '@/api/admin'
import { Card } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import { Button } from '@/components/ui/button'
import type { AdminLessonDto } from '@/types'

function time(iso: string) {
  return new Date(iso).toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
}

export default function TodayChecklist() {
  const [lessons, setLessons] = useState<AdminLessonDto[]>([])
  const [loading, setLoading] = useState(true)
  const [justifyingId, setJustifyingId] = useState<string | null>(null)
  const [reason, setReason] = useState('')

  async function load() {
    setLoading(true)
    try {
      setLessons(await adminApi.today())
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void load()
  }, [])

  async function markDone(id: string) {
    await adminApi.completeLesson(id)
    await load()
  }

  async function submitJustify(id: string) {
    if (!reason.trim()) return
    await adminApi.justifyLesson(id, reason.trim())
    setJustifyingId(null)
    setReason('')
    await load()
  }

  const today = new Date().toLocaleDateString('pt-BR', {
    weekday: 'long',
    day: '2-digit',
    month: 'long',
  })

  return (
    <div className="space-y-5">
      <header>
        <h1 className="text-xl font-semibold tracking-tight text-zinc-100">Checklist do dia</h1>
        <p className="text-sm capitalize text-zinc-500">{today}</p>
      </header>

      {loading && <p className="text-sm text-zinc-500">Carregando…</p>}
      {!loading && lessons.length === 0 && (
        <Card className="p-6 text-center text-sm text-zinc-500">
          Nenhuma aula agendada para hoje.
        </Card>
      )}

      <div className="space-y-3">
        {lessons.map((l) => (
          <Card key={l.id} className="p-4">
            <div className="flex items-center justify-between gap-3">
              <div className="min-w-0">
                <p className="truncate font-medium text-zinc-100">{l.studentName}</p>
                <p className="flex items-center gap-1 text-xs text-zinc-500">
                  <Clock size={13} /> {time(l.scheduledAt)} · {l.durationMinutes} min
                </p>
              </div>

              {l.status === 'Done' && (
                <Badge variant="emerald" className="gap-1">
                  <CheckCircle size={13} weight="fill" /> Feita
                </Badge>
              )}
              {l.status === 'Justified' && (
                <Badge variant="amber" className="gap-1">
                  <XCircle size={13} weight="fill" /> Justificada
                </Badge>
              )}
              {l.status === 'Scheduled' && (
                <div className="flex shrink-0 gap-2">
                  <Button size="sm" variant="amber" onClick={() => markDone(l.id)}>
                    <CheckCircle size={15} weight="fill" /> Feita
                  </Button>
                  <Button
                    size="sm"
                    variant="outline"
                    onClick={() => {
                      setJustifyingId(justifyingId === l.id ? null : l.id)
                      setReason('')
                    }}
                  >
                    Justificar
                  </Button>
                </div>
              )}
            </div>

            {l.status === 'Justified' && l.justification && (
              <p className="mt-2 rounded-md bg-zinc-950/60 px-3 py-2 text-xs text-zinc-400">
                “{l.justification}”
              </p>
            )}

            {justifyingId === l.id && (
              <div className="mt-3 flex gap-2">
                <input
                  value={reason}
                  onChange={(e) => setReason(e.target.value)}
                  placeholder="Motivo da não realização…"
                  autoFocus
                  className="h-9 flex-1 rounded-lg border border-zinc-800 bg-zinc-950 px-3 text-sm text-zinc-100 placeholder:text-zinc-600 outline-none focus:border-amber-500/60"
                />
                <Button size="sm" variant="amber" onClick={() => submitJustify(l.id)}>
                  Salvar
                </Button>
              </div>
            )}
          </Card>
        ))}
      </div>
    </div>
  )
}
