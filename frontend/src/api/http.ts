import { session } from '@/auth'

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:8080'

export class ApiError extends Error {
  constructor(
    message: string,
    public status: number,
  ) {
    super(message)
  }
}

export async function http<T>(path: string, init?: RequestInit): Promise<T> {
  const token = session.token()
  const res = await fetch(`${BASE_URL}${path}`, {
    ...init,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init?.headers,
    },
  })

  if (res.status === 401) {
    session.clear()
    const loginPath = location.pathname.startsWith('/admin') ? '/admin/login' : '/login'
    if (location.pathname !== loginPath) location.assign(loginPath)
    throw new ApiError('Sessão expirada.', 401)
  }

  if (!res.ok) {
    let message = `Erro ${res.status}`
    try {
      const body = await res.json()
      if (body?.message) message = body.message
    } catch {
      /* sem corpo JSON */
    }
    throw new ApiError(message, res.status)
  }

  if (res.status === 204) return undefined as T
  return res.json() as Promise<T>
}
