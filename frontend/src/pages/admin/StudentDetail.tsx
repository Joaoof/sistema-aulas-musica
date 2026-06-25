import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { CaretLeft, Plus, CheckCircle, XCircle, Clock, Trash } from '@phosphor-icons/react'
import { adminApi } from '@/api/admin'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import type { PlanDto, StudentDetail as Detail } from '@/types'

const inputCls =
  'h-10 w-full rounded-lg border border-zinc-800 bg-zinc-950 px-3 text-sm text-zinc-100 placeholder:text-zinc-600 outline-none focus:border-amber-500/60'
const brl = (v: number | null) =>
  v == null ? '—' : v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

export default function StudentDetail() {
  const { id = '' } = useParams()
  const navigate = useNavigate()
  const [data, setData] = useState<Detail | null>(null)
  const [plans, setPlans] = useState<PlanDto[]>([])
  const [loading, setLoading] = useState(true)
  const [confirmDelete, setConfirmDelete] = useState(false)
  const [deleting, setDeleting] = useState(false)

  async function remove() {
    setDeleting(true)
    try {
      await adminApi.deleteStudent(id)
      navigate('/admin/alunos', { replace: true })
    } catch {
      setDeleting(false)
      setConfirmDelete(false)
    }
  }

  async function load() {
    const [d, p] = await Promise.all([adminApi.student(id), adminApi.plans()])
    setData(d)
    setPlans(p)
    setLoading(false)
  }
  useEffect(() => {
    void load()
  }, [id])

  if (loading || !data) return <p className="text-sm text-zinc-500">Carregando…</p>

  return (
    <div className="space-y-5">
      <Link to="/admin/alunos" className="inline-flex items-center gap-1 text-sm text-zinc-400 hover:text-zinc-100">
        <CaretLeft size={16} /> Alunos
      </Link>

      <header className="flex items-start justify-between gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight text-zinc-100">{data.name}</h1>
          <p className="text-sm text-zinc-500">
            {data.instrument} · {data.email}
          </p>
        </div>
        <div className="flex items-center gap-4">
          <div className="text-right">
            <p className="text-2xl font-bold text-amber-500">{data.doneThisMonth}</p>
            <p className="text-[11px] uppercase tracking-wide text-zinc-500">aulas feitas/mês</p>
          </div>
          {confirmDelete ? (
            <div className="flex flex-col gap-1">
              <Button size="sm" variant="destructive" onClick={remove} disabled={deleting}>
                {deleting ? 'Excluindo…' : 'Confirmar exclusão'}
              </Button>
              <button
                onClick={() => setConfirmDelete(false)}
                className="text-[11px] text-zinc-500 hover:text-zinc-300"
              >
                cancelar
              </button>
            </div>
          ) : (
            <Button
              size="icon"
              variant="outline"
              onClick={() => setConfirmDelete(true)}
              aria-label="Excluir aluno"
              className="text-red-400 hover:border-red-500/40 hover:text-red-300"
            >
              <Trash size={18} />
            </Button>
          )}
        </div>
      </header>

      <Tabs defaultValue="plano">
        <TabsList className="w-full">
          <TabsTrigger value="plano" className="flex-1">Plano</TabsTrigger>
          <TabsTrigger value="repertorio" className="flex-1">Repertório</TabsTrigger>
          <TabsTrigger value="materiais" className="flex-1">Materiais</TabsTrigger>
          <TabsTrigger value="aulas" className="flex-1">Aulas</TabsTrigger>
        </TabsList>

        <TabsContent value="plano">
          <PlanTab data={data} plans={plans} onSaved={load} />
        </TabsContent>
        <TabsContent value="repertorio">
          <RepertoireTab data={data} onSaved={load} />
        </TabsContent>
        <TabsContent value="materiais">
          <MaterialsTab data={data} onSaved={load} />
        </TabsContent>
        <TabsContent value="aulas">
          <LessonsTab data={data} onSaved={load} />
        </TabsContent>
      </Tabs>
    </div>
  )
}

function PlanTab({ data, plans, onSaved }: { data: Detail; plans: PlanDto[]; onSaved: () => Promise<void> }) {
  const [planId, setPlanId] = useState(data.plan.planId ?? '')
  const [price, setPrice] = useState(String(data.plan.monthlyPrice ?? ''))
  const [sessions, setSessions] = useState(String(data.plan.monthlySessions ?? ''))
  const [saving, setSaving] = useState(false)

  function selectPlan(value: string) {
    setPlanId(value)
    const p = plans.find((x) => x.id === value)
    if (p) {
      setPrice(String(p.price))
      setSessions(String(p.sessionsPerMonth))
    }
  }

  async function save() {
    if (!planId) return
    setSaving(true)
    try {
      await adminApi.assignPlan(data.id, planId, Number(price), Number(sessions))
      await onSaved()
    } finally {
      setSaving(false)
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle>Plano do aluno</CardTitle>
      </CardHeader>
      <CardContent className="space-y-3">
        <div className="flex items-center justify-between text-sm">
          <span className="text-zinc-400">Atual</span>
          <Badge variant={data.plan.planName ? 'amber' : 'outline'}>
            {data.plan.planName ?? 'Sem plano'} · {brl(data.plan.monthlyPrice)}
          </Badge>
        </div>
        <select value={planId} onChange={(e) => selectPlan(e.target.value)} className={inputCls}>
          <option value="">Selecione um pacote…</option>
          {plans.map((p) => (
            <option key={p.id} value={p.id}>
              {p.name} — {brl(p.price)}
            </option>
          ))}
        </select>
        <div className="grid grid-cols-2 gap-3">
          <label className="text-xs text-zinc-500">
            Valor mensal (R$)
            <input value={price} onChange={(e) => setPrice(e.target.value)} type="number" className={inputCls} />
          </label>
          <label className="text-xs text-zinc-500">
            Aulas no mês
            <input value={sessions} onChange={(e) => setSessions(e.target.value)} type="number" className={inputCls} />
          </label>
        </div>
        <Button variant="amber" size="sm" onClick={save} disabled={saving || !planId}>
          {saving ? 'Salvando…' : 'Salvar plano'}
        </Button>
      </CardContent>
    </Card>
  )
}

function RepertoireTab({ data, onSaved }: { data: Detail; onSaved: () => Promise<void> }) {
  const [f, setF] = useState({ title: '', composer: '', videoUrl: '', status: 'ToStudy' })
  const [saving, setSaving] = useState(false)

  async function add(e: React.FormEvent) {
    e.preventDefault()
    setSaving(true)
    try {
      await adminApi.addRepertoire(data.id, {
        title: f.title,
        composer: f.composer,
        videoUrl: f.videoUrl || undefined,
        status: f.status,
      })
      setF({ title: '', composer: '', videoUrl: '', status: 'ToStudy' })
      await onSaved()
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4">
      <Card>
        <CardContent className="pt-5">
          <form onSubmit={add} className="grid gap-3 sm:grid-cols-2">
            <input className={inputCls} placeholder="Título" required value={f.title} onChange={(e) => setF({ ...f, title: e.target.value })} />
            <input className={inputCls} placeholder="Compositor" required value={f.composer} onChange={(e) => setF({ ...f, composer: e.target.value })} />
            <input className={inputCls} placeholder="URL do vídeo (Drive)" value={f.videoUrl} onChange={(e) => setF({ ...f, videoUrl: e.target.value })} />
            <select className={inputCls} value={f.status} onChange={(e) => setF({ ...f, status: e.target.value })}>
              <option value="ToStudy">A estudar</option>
              <option value="InProgress">Em treino</option>
              <option value="Mastered">Dominada</option>
            </select>
            <div className="sm:col-span-2">
              <Button type="submit" variant="amber" size="sm" disabled={saving}>
                <Plus size={15} weight="bold" /> Adicionar peça
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
      <div className="space-y-2">
        {data.repertoire.map((r) => (
          <Card key={r.id} className="flex items-center justify-between p-3">
            <div>
              <p className="font-medium text-zinc-100">{r.title}</p>
              <p className="text-xs text-zinc-500">{r.composer}</p>
            </div>
            <Badge>{r.status}</Badge>
          </Card>
        ))}
      </div>
    </div>
  )
}

function MaterialsTab({ data, onSaved }: { data: Detail; onSaved: () => Promise<void> }) {
  const [f, setF] = useState({ title: '', type: 'Pdf', externalUrl: '' })
  const [saving, setSaving] = useState(false)

  async function add(e: React.FormEvent) {
    e.preventDefault()
    setSaving(true)
    try {
      await adminApi.addMaterial(data.id, f)
      setF({ title: '', type: 'Pdf', externalUrl: '' })
      await onSaved()
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4">
      <Card>
        <CardContent className="pt-5">
          <form onSubmit={add} className="grid gap-3 sm:grid-cols-2">
            <input className={inputCls} placeholder="Título" required value={f.title} onChange={(e) => setF({ ...f, title: e.target.value })} />
            <select className={inputCls} value={f.type} onChange={(e) => setF({ ...f, type: e.target.value })}>
              <option value="Pdf">PDF</option>
              <option value="Video">Vídeo</option>
              <option value="Audio">Áudio</option>
              <option value="Sheet">Partitura</option>
            </select>
            <input className={`${inputCls} sm:col-span-2`} placeholder="URL externa (Google Drive)" required value={f.externalUrl} onChange={(e) => setF({ ...f, externalUrl: e.target.value })} />
            <div className="sm:col-span-2">
              <Button type="submit" variant="amber" size="sm" disabled={saving}>
                <Plus size={15} weight="bold" /> Adicionar material
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
      <div className="space-y-2">
        {data.materials.map((m) => (
          <a key={m.id} href={m.externalUrl} target="_blank" rel="noreferrer">
            <Card className="flex items-center justify-between p-3 transition hover:border-amber-500/40">
              <p className="font-medium text-zinc-100">{m.title}</p>
              <Badge variant="outline">{m.type}</Badge>
            </Card>
          </a>
        ))}
      </div>
    </div>
  )
}

function LessonsTab({ data, onSaved }: { data: Detail; onSaved: () => Promise<void> }) {
  const [when, setWhen] = useState('')
  const [duration, setDuration] = useState('60')
  const [saving, setSaving] = useState(false)

  async function schedule(e: React.FormEvent) {
    e.preventDefault()
    if (!when) return
    setSaving(true)
    try {
      // datetime-local é horário local -> converte para ISO/UTC
      await adminApi.scheduleLesson(data.id, new Date(when).toISOString(), Number(duration))
      setWhen('')
      await onSaved()
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-4">
      <Card>
        <CardContent className="pt-5">
          <form onSubmit={schedule} className="grid gap-3 sm:grid-cols-2">
            <input className={inputCls} type="datetime-local" required value={when} onChange={(e) => setWhen(e.target.value)} />
            <input className={inputCls} type="number" min={10} placeholder="Duração (min)" value={duration} onChange={(e) => setDuration(e.target.value)} />
            <div className="sm:col-span-2">
              <Button type="submit" variant="amber" size="sm" disabled={saving}>
                <Plus size={15} weight="bold" /> Agendar aula
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
      <div className="space-y-2">
        {data.lessons.map((l) => (
          <Card key={l.id} className="flex items-center justify-between p-3">
            <p className="flex items-center gap-1.5 text-sm text-zinc-200">
              <Clock size={14} className="text-zinc-500" />
              {new Date(l.scheduledAt).toLocaleString('pt-BR', {
                day: '2-digit',
                month: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
              })}
            </p>
            {l.status === 'Done' && (
              <Badge variant="emerald" className="gap-1"><CheckCircle size={12} weight="fill" /> Feita</Badge>
            )}
            {l.status === 'Justified' && (
              <Badge variant="amber" className="gap-1"><XCircle size={12} weight="fill" /> Justificada</Badge>
            )}
            {l.status === 'Scheduled' && <Badge variant="outline">Agendada</Badge>}
          </Card>
        ))}
      </div>
    </div>
  )
}
