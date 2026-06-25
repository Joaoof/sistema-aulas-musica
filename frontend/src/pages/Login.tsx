import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { WaveSine } from '@phosphor-icons/react'
import { api } from '@/api/client'
import { session } from '@/auth'
import { Button } from '@/components/ui/button'

export default function Login() {
  const navigate = useNavigate()
  const [identifier, setIdentifier] = useState('ana@portal.dev')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      const student = await api.login(identifier.trim())
      session.save(student)
      navigate('/', { replace: true })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Falha no acesso.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-dvh items-center justify-center px-6">
      <div className="w-full max-w-sm">
        <div className="mb-10 flex flex-col items-center text-center">
          <div className="mb-4 flex h-16 w-16 items-center justify-center rounded-2xl border border-amber-500/30 bg-amber-500/10 shadow-glow">
            <WaveSine size={32} weight="duotone" className="text-amber-500" />
          </div>
          <h1 className="text-xl font-semibold tracking-tight text-zinc-100">Portal do Aluno</h1>
          <p className="mt-1 text-sm text-zinc-500">Console de prática · acesso por ID</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="mb-1.5 block text-[11px] font-medium uppercase tracking-wide text-zinc-500">
              ID ou e-mail do aluno
            </label>
            <input
              type="text"
              value={identifier}
              onChange={(e) => setIdentifier(e.target.value)}
              placeholder="ana@portal.dev"
              autoFocus
              className="h-11 w-full rounded-lg border border-zinc-800 bg-zinc-900 px-4 font-mono text-sm text-zinc-100 placeholder:text-zinc-600 outline-none transition focus:border-amber-500/60 focus:ring-2 focus:ring-amber-500/20"
            />
          </div>

          {error && (
            <p className="rounded-md border border-destructive/30 bg-destructive/10 px-3 py-2 text-xs text-red-300">
              {error}
            </p>
          )}

          <Button type="submit" variant="amber" size="lg" disabled={loading} className="w-full">
            {loading ? 'Conectando…' : 'Acessar console'}
          </Button>
        </form>

        <p className="mt-6 text-center text-xs text-zinc-600">
          Dados de demonstração já carregados no banco.
        </p>
      </div>
    </div>
  )
}
