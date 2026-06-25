import { DonutChart, Legend } from '@tremor/react'
import type { RepertoireStats } from '@/types'

export function RepertoireDonut({ stats }: { stats: RepertoireStats }) {
  const data = [
    { name: 'Dominadas', value: stats.mastered },
    { name: 'Em aprendizado', value: stats.learning },
  ]

  return (
    <div className="flex flex-col items-center gap-3">
      <DonutChart
        className="h-40"
        data={data}
        category="value"
        index="name"
        colors={['emerald', 'amber']}
        showLabel
        valueFormatter={(v) => `${v} música(s)`}
      />
      <Legend
        categories={['Dominadas', 'Em aprendizado']}
        colors={['emerald', 'amber']}
        className="justify-center"
      />
    </div>
  )
}
