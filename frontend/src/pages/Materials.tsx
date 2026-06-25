import { FilePdf, FilmSlate, Headphones, MusicNotes, ArrowUpRight, type Icon } from '@phosphor-icons/react'
import { useDashboard } from '@/hooks/dashboard'
import { Card } from '@/components/ui/card'
import type { MaterialDto } from '@/types'

const typeMeta: Record<MaterialDto['type'], { icon: Icon; label: string }> = {
  Pdf: { icon: FilePdf, label: 'PDF' },
  Video: { icon: FilmSlate, label: 'Vídeo' },
  Audio: { icon: Headphones, label: 'Áudio' },
  Sheet: { icon: MusicNotes, label: 'Partitura' },
}

export default function Materials() {
  const { data, loading } = useDashboard()

  return (
    <div className="space-y-4">
      <header>
        <h1 className="text-xl font-semibold tracking-tight text-zinc-100">Materiais</h1>
        <p className="text-sm text-zinc-500">Arquivos externos (Google Drive)</p>
      </header>

      {loading && <p className="text-sm text-zinc-500">Carregando…</p>}

      {data && (
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          {data.materials.map((m) => {
            const meta = typeMeta[m.type]
            const Icon = meta.icon
            return (
              <a key={m.id} href={m.externalUrl} target="_blank" rel="noreferrer">
                <Card className="group flex items-center gap-4 p-4 transition hover:border-amber-500/40 hover:bg-zinc-900">
                  <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-lg border border-zinc-800 bg-zinc-950 text-amber-500">
                    <Icon size={22} weight="duotone" />
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="truncate font-medium text-zinc-100">{m.title}</p>
                    <p className="text-[11px] uppercase tracking-wide text-zinc-500">{meta.label}</p>
                  </div>
                  <ArrowUpRight
                    size={18}
                    className="text-zinc-600 transition group-hover:text-amber-500"
                  />
                </Card>
              </a>
            )
          })}
          {data.materials.length === 0 && (
            <p className="text-sm text-zinc-500">Nenhum material disponível.</p>
          )}
        </div>
      )}
    </div>
  )
}
