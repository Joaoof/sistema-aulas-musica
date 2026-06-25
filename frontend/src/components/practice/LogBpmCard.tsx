import { useState } from 'react'
import { Metronome } from '@phosphor-icons/react'
import { api } from '@/api/client'
import { session } from '@/auth'
import { useDashboard } from '@/hooks/dashboard'
import { Button } from '@/components/ui/button'

export function LogBpmCard() {
  const { refresh } = useDashboard()
  const [bpm, setBpm] = useState('')
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    const value = Number(bpm)
    if (!Number.isFinite(value) || value <= 0) {
      setError('Informe um BPM válido.')
      return
    }
    const student = session.get()
    if (!student) return

    setError(null)
    setSaving(true)
    try {
      await api.logPractice(student.id, value)
      setBpm('')
      await refresh() // backend invalidou o cache -> puxa série atualizada
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha ao registrar.')
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-1.5">
      <form
        onSubmit={submit}
        className="flex items-center gap-2 rounded-xl border border-zinc-800 bg-zinc-900/60 p-2"
      >
        <Metronome size={20} weight="duotone" className="ml-1 shrink-0 text-amber-500" />
        <input
          type="number"
          inputMode="numeric"
          min={1}
          max={400}
          value={bpm}
          onChange={(e) => setBpm(e.target.value)}
          placeholder="Registrar BPM de hoje"
          className="h-9 flex-1 bg-transparent px-1 font-mono text-sm text-zinc-100 placeholder:text-zinc-600 outline-none"
        />
        <Button type="submit" variant="amber" size="sm" disabled={saving}>
          {saving ? '…' : 'Registrar'}
        </Button>
      </form>
      {error && <p className="px-1 text-xs text-red-400">{error}</p>}
    </div>
  )
}
