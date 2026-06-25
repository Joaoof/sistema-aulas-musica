import { AreaChart } from '@tremor/react'
import type { BpmPoint } from '@/types'

export function BpmAreaChart({ data }: { data: BpmPoint[] }) {
  return (
    <AreaChart
      className="h-44 mt-2"
      data={data}
      index="date"
      categories={['bpm']}
      colors={['amber']}
      showLegend={false}
      showGridLines={false}
      yAxisWidth={36}
      curveType="monotone"
      valueFormatter={(v) => `${v} BPM`}
      autoMinValue
    />
  )
}
