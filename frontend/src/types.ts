export type Role = 'Student' | 'Admin'

export interface LoginResponse {
  token: string
  id: string
  name: string
  email: string
  instrument: string
  role: Role
}

export interface AdminLoginResponse {
  token: string
  id: string
  name: string
  email: string
  role: Role
}

// ── Admin DTOs ──────────────────────────────
export interface PlanDto {
  id: string
  code: string
  name: string
  sessionsPerMonth: number
  durationMinutes: number
  price: number
  summary: string
  features: string[]
}

export interface StudentSummary {
  id: string
  name: string
  email: string
  instrument: string
  planName: string | null
  monthlyPrice: number | null
  monthlySessions: number | null
  doneThisMonth: number
}

export interface AssignedPlan {
  planId: string | null
  planName: string | null
  monthlyPrice: number | null
  monthlySessions: number | null
}

export interface AdminLessonDto {
  id: string
  studentId: string
  studentName: string
  scheduledAt: string
  durationMinutes: number
  status: 'Scheduled' | 'Done' | 'Justified'
  justification: string | null
}

export interface StudentDetail {
  id: string
  name: string
  email: string
  instrument: string
  plan: AssignedPlan
  doneThisMonth: number
  repertoire: RepertoireDto[]
  materials: MaterialDto[]
  lessons: AdminLessonDto[]
}

export type RepertoireStatus = 'ToStudy' | 'InProgress' | 'Mastered'

export interface RepertoireDto {
  id: string
  title: string
  composer: string
  status: RepertoireStatus
  videoUrl: string | null
}

export interface MaterialDto {
  id: string
  title: string
  type: 'Pdf' | 'Video' | 'Audio' | 'Sheet'
  externalUrl: string
}

export interface BpmPoint {
  date: string
  bpm: number
}

export interface RepertoireStats {
  mastered: number
  learning: number
  total: number
}

export interface StudentDashboardDto {
  studentId: string
  name: string
  instrument: string
  nextLessonAt: string | null
  currentSprint: string
  repertoire: RepertoireDto[]
  materials: MaterialDto[]
  bpmHistory: BpmPoint[]
  repertoireStats: RepertoireStats
}
