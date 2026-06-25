import { http } from '@/api/http'
import type { LoginResponse, StudentDashboardDto } from '@/types'

export const api = {
  login: (identifier: string) =>
    http<LoginResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ identifier }),
    }),

  dashboard: (studentId: string) =>
    http<StudentDashboardDto>(`/api/students/${studentId}/dashboard`),

  logPractice: (studentId: string, bpm: number) =>
    http<{ date: string; bpm: number }>(`/api/students/${studentId}/practice`, {
      method: 'POST',
      body: JSON.stringify({ bpm }),
    }),
}
