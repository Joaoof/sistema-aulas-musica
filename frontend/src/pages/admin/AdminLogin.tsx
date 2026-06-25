import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { ShieldCheck } from '@phosphor-icons/react'
import { adminApi } from '@/api/admin'
import { session } from '@/auth'
import { Button } from '@/components/ui/button'

export default function AdminLogin() {
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  async function submit(e: React.FormEvent) {
    e.preventDefault()
    setError(null)
    setLoading(true)
    try {
      const admin = await adminApi.login(email.trim(), password)
      session.save(admin)
      navigate('/admin', { replace: true })
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
            <ShieldCheck size={32} weight="duotone" className="text-amber-500" />
          </div>
          <h1 className="text-xl font-semibold tracking-tight text-zinc-100">Painel do Professor</h1>
          <p className="mt-1 text-sm text-zinc-500">Acesso restrito · super usuário</p>
        </div>

        <form onSubmit={submit} className="space-y-3">
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="E-mail"
            autoFocus
            className="h-11 w-full rounded-lg border border-zinc-800 bg-zinc-900 px-4 text-sm text-zinc-100 placeholder:text-zinc-600 outline-none transition focus:border-amber-500/60 focus:ring-2 focus:ring-amber-500/20"
          />
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Senha"
            className="h-11 w-full rounded-lg border border-zinc-800 bg-zinc-900 px-4 text-sm text-zinc-100 placeholder:text-zinc-600 outline-none transition focus:border-amber-500/60 focus:ring-2 focus:ring-amber-500/20"
          />
          {error && (
            <p className="rounded-md border border-destructive/30 bg-destructive/10 px-3 py-2 text-xs text-red-300">
              {error}
            </p>
          )}
          <Button type="submit" variant="amber" size="lg" disabled={loading} className="w-full">
            {loading ? 'Entrando…' : 'Entrar no painel'}
          </Button>
        </form>
      </div>
    </div>
  )
}
