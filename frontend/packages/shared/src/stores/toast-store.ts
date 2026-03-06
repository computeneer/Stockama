import { create } from 'zustand'

export type ToastVariant = 'success' | 'error' | 'info'

export interface ToastItem {
  id: string
  variant: ToastVariant
  title: string
  message?: string
  durationMs: number
}

interface ToastInput {
  variant: ToastVariant
  title: string
  message?: string
  durationMs?: number
}

interface ToastStore {
  toasts: ToastItem[]
  push: (input: ToastInput) => string
  dismiss: (id: string) => void
  success: (title: string, message?: string, durationMs?: number) => string
  error: (title: string, message?: string, durationMs?: number) => string
  info: (title: string, message?: string, durationMs?: number) => string
}

const DEFAULT_DURATION_MS = 2800

function createToastId() {
  return `${Date.now()}-${Math.random().toString(36).slice(2, 10)}`
}

export const useToastStore = create<ToastStore>((set, get) => ({
  toasts: [],
  push: (input) => {
    const id = createToastId()
    const toast: ToastItem = {
      id,
      variant: input.variant,
      title: input.title,
      message: input.message,
      durationMs: input.durationMs ?? DEFAULT_DURATION_MS,
    }

    set((state) => ({ toasts: [...state.toasts, toast] }))

    setTimeout(() => {
      get().dismiss(id)
    }, toast.durationMs)

    return id
  },
  dismiss: (id) => {
    set((state) => ({ toasts: state.toasts.filter((toast) => toast.id !== id) }))
  },
  success: (title, message, durationMs) => get().push({ variant: 'success', title, message, durationMs }),
  error: (title, message, durationMs) => get().push({ variant: 'error', title, message, durationMs }),
  info: (title, message, durationMs) => get().push({ variant: 'info', title, message, durationMs }),
}))
