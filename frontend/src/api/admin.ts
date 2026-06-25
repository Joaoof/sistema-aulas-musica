import { http } from '@/api/http'
import type {
  AdminLessonDto,
  AdminLoginResponse,
  AssignedPlan,
  PlanDto,
  StudentDetail,
  StudentSummary,
} from '@/types'

export const adminApi = {
  login: (email: string, password: string) =>
    http<AdminLoginResponse>('/api/auth/admin/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }),

  // Alunos
  students: () => http<StudentSummary[]>('/api/admin/students'),
  student: (id: string) => http<StudentDetail>(`/api/admin/students/${id}`),
  createStudent: (name: string, email: string, instrument: string) =>
    http<StudentSummary>('/api/admin/students', {
      method: 'POST',
      body: JSON.stringify({ name, email, instrument }),
    }),
  deleteStudent: (id: string) =>
    http<void>(`/api/admin/students/${id}`, { method: 'DELETE' }),
  assignPlan: (id: string, planId: string, monthlyPrice: number, monthlySessions: number) =>
    http<AssignedPlan>(`/api/admin/students/${id}/plan`, {
      method: 'PUT',
      body: JSON.stringify({ planId, monthlyPrice, monthlySessions }),
    }),
  addRepertoire: (
    id: string,
    body: { title: string; composer: string; videoUrl?: string; status?: string },
  ) =>
    http<void>(`/api/admin/students/${id}/repertoire`, {
      method: 'POST',
      body: JSON.stringify(body),
    }),
  addMaterial: (id: string, body: { title: string; type: string; externalUrl: string }) =>
    http<void>(`/api/admin/students/${id}/materials`, {
      method: 'POST',
      body: JSON.stringify(body),
    }),

  // Planos
  plans: () => http<PlanDto[]>('/api/admin/plans'),

  // Aulas
  scheduleLesson: (studentId: string, scheduledAt: string, durationMinutes: number) =>
    http<AdminLessonDto>('/api/admin/lessons', {
      method: 'POST',
      body: JSON.stringify({ studentId, scheduledAt, durationMinutes }),
    }),
  today: () => http<AdminLessonDto[]>('/api/admin/lessons/today'),
  completeLesson: (id: string) =>
    http<void>(`/api/admin/lessons/${id}/complete`, { method: 'POST' }),
  justifyLesson: (id: string, reason: string) =>
    http<void>(`/api/admin/lessons/${id}/justify`, {
      method: 'POST',
      body: JSON.stringify({ reason }),
    }),
  monthlyStats: (year?: number, month?: number) => {
    const q = new URLSearchParams()
    if (year) q.set('year', String(year))
    if (month) q.set('month', String(month))
    const qs = q.toString()
    return http<StudentSummary[]>(`/api/admin/lessons/stats${qs ? `?${qs}` : ''}`)
  },
}
