import { NavLink } from 'react-router-dom'
import { Metronome, ListBullets, FolderSimple, type Icon } from '@phosphor-icons/react'
import { cn } from '@/lib/utils'

interface NavItem {
  to: string
  label: string
  icon: Icon
}

const items: NavItem[] = [
  { to: '/', label: 'Dashboard', icon: Metronome },
  { to: '/repertorio', label: 'Repertório', icon: ListBullets },
  { to: '/materiais', label: 'Materiais', icon: FolderSimple },
]

export function BottomNav() {
  return (
    <nav className="fixed inset-x-0 bottom-0 z-50 border-t border-zinc-800 bg-zinc-950/90 backdrop-blur-md">
      <ul className="mx-auto flex max-w-lg items-stretch justify-around px-2 pb-[env(safe-area-inset-bottom)]">
        {items.map(({ to, label, icon: Icon }) => (
          <li key={to} className="flex-1">
            <NavLink
              to={to}
              end={to === '/'}
              className="flex flex-col items-center gap-1 py-2.5 outline-none"
            >
              {({ isActive }) => (
                <>
                  {/* LED do equipamento: aceso (fill + âmbar) vs apagado (regular + zinc) */}
                  <Icon
                    size={24}
                    weight={isActive ? 'fill' : 'regular'}
                    className={cn(
                      'transition-colors',
                      isActive
                        ? 'text-amber-500 drop-shadow-[0_0_6px_rgba(245,158,11,0.55)]'
                        : 'text-zinc-500',
                    )}
                  />
                  <span
                    className={cn(
                      'text-[11px] font-medium tracking-wide transition-colors',
                      isActive ? 'text-amber-500' : 'text-zinc-500',
                    )}
                  >
                    {label}
                  </span>
                </>
              )}
            </NavLink>
          </li>
        ))}
      </ul>
    </nav>
  )
}
