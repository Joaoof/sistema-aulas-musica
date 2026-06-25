import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Plus, CaretRight } from '@phosphor-icons/react'
import { adminApi } from '@/api/admin'
import { Card } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import type { StudentSummary } from '@/types'

export default function StudentsList() {
  const [students, setStudents] = useState<StudentSummary[]>([])
  const [loading, setLoading] = useState(true)
  const [open, setOpen] = useState(false)
  const [form, setForm] = useState({ name: '', email: '', instrument: '' })
  const [error, setError] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)

  async function load() {
    setLoading(true)
    try {
      setStudents(await adminApi.students())
    } finally {
      setLoading(false)
    }
  }
  useEffect(() => {
    void load()
  }, [])

  async function create(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setSaving(true)
    try {
      await adminApi.createStudent(form.name.trim(), form.email.trim(), form.instrument.trim())
      setForm({ name: '', email: '', instrument: '' })
      setOpen(false)
      await load()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao criar.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-5">
      <header className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-semibold tracking-tight text-zinc-100">Alunos</h1>
          <p className="text-sm text-zinc-500">{students.length} cadastrado(s)</p>
        </div>
        <Button variant="amber" size="sm" onClick={() => setOpen((v) => !v)}>
          <Plus size={16} weight="bold" /> Novo aluno
        </Button>
      </header>

      {open && (
        <Card className="p-4">
          <form onSubmit={create} className="grid gap-3 sm:grid-cols-3">
            <input
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              placeholder="Nome"
              required
              className="h-10 rounded-lg border border-zinc-800 bg-zinc-950 px-3 text-sm text-zinc-100 placeholder:text-zinc-600 outline-none focus:border-amber-500/60"
            />
            <input
              type="email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              placeholder="E-mail"
              required
              className="h-10 rounded-lg border border-zinc-800 bg-zinc-950 px-3 text-sm text-zinc-100 placeholder:text-zinc-600 outline-none focus:border-amber-500/60"
            />
            <input
              value={form.instrument}
              onChange={(e) => setForm({ ...form, instrument: e.target.value })}
              placeholder="Instrumento"
              required
              className="h-10 rounded-lg border border-zinc-800 bg-zinc-950 px-3 text-sm text-zinc-100 placeholder:text-zinc-600 outline-none focus:border-amber-500/60"
            />
            {error && <p className="text-xs text-red-400 sm:col-span-3">{error}</p>}
            <div className="sm:col-span-3">
              <Button type="submit" variant="amber" size="sm" disabled={saving}>
                {saving ? 'Salvando…' : 'Criar aluno'}
              </Button>
            </div>
          </form>
        </Card>
      )}

      {loading && <p className="text-sm text-zinc-500">Carregando…</p>}

      <div className="space-y-2">
        {students.map((s) => (
          <Link key={s.id} to={`/admin/alunos/${s.id}`}>
            <Card className="flex items-center justify-between gap-3 p-4 transition hover:border-amber-500/40 hover:bg-zinc-900">
              <div className="min-w-0">
                <p className="truncate font-medium text-zinc-100">{s.name}</p>
                <p className="truncate text-xs text-zinc-500">
                  {s.instrument} · {s.email}
                </p>
              </div>
              <div className="flex shrink-0 items-center gap-3">
                <div className="text-right">
                  <Badge variant={s.planName ? 'amber' : 'outline'}>
                    {s.planName ?? 'Sem plano'}
                  </Badge>
                  <p className="mt-1 text-[11px] text-zinc-500">{s.doneThisMonth} aula(s)/mês</p>
                </div>
                <CaretRight size={16} className="text-zinc-600" />
              </div>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  )
}
