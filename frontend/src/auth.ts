import type { Role } from './types'

const KEY = 'portal.session'

export interface AuthUser {
  token: string
  id: string
  name: string
  email: string
  role: Role
  instrument?: string
}

export const session = {
  save(user: AuthUser) {
    localStorage.setItem(KEY, JSON.stringify(user))
  },
  get(): AuthUser | null {
    const raw = localStorage.getItem(KEY)
    return raw ? (JSON.parse(raw) as AuthUser) : null
  },
  token(): string | null {
    return this.get()?.token ?? null
  },
  isAdmin(): boolean {
    return this.get()?.role === 'Admin'
  },
  clear() {
    localStorage.removeItem(KEY)
  },
}
