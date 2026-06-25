import { useEffect, useState } from 'react'
import { Check } from '@phosphor-icons/react'
import { adminApi } from '@/api/admin'
import { Card } from '@/components/ui/card'
import { Badge } from '@/components/ui/badge'
import type { PlanDto } from '@/types'

const brl = (v: number) => v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })

export default function PlansPage() {
  const [plans, setPlans] = useState<PlanDto[]>([])

  useEffect(() => {
    void adminApi.plans().then(setPlans)
  }, [])

  return (
    <div className="space-y-5">
      <header>
        <h1 className="text-xl font-semibold tracking-tight text-zinc-100">Pacotes mensais</h1>
        <p className="text-sm text-zinc-500">Catálogo · aulas na casa do aluno</p>
      </header>

      <div className="grid gap-4 sm:grid-cols-2">
        {plans.map((p) => (
          <Card key={p.id} className="flex flex-col p-5">
            <div className="mb-2 flex items-start justify-between">
              <h2 className="font-semibold text-zinc-100">{p.name}</h2>
              <Badge variant="amber">{brl(p.price)}/mês</Badge>
            </div>
            <p className="text-sm text-zinc-400">{p.summary}</p>
            <p className="mt-2 text-xs uppercase tracking-wide text-zinc-500">
              {p.sessionsPerMonth}x no mês · {p.durationMinutes} min
            </p>
            <ul className="mt-3 space-y-1.5">
              {p.features.map((f) => (
                <li key={f} className="flex gap-2 text-sm text-zinc-300">
                  <Check size={16} weight="bold" className="mt-0.5 shrink-0 text-amber-500" />
                  {f}
                </li>
              ))}
            </ul>
          </Card>
        ))}
      </div>
    </div>
  )
}
