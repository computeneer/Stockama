import { useToastStore } from '../stores'

export function useToast() {
  const success = useToastStore((state) => state.success)
  const error = useToastStore((state) => state.error)
  const info = useToastStore((state) => state.info)

  return {
    success,
    error,
    info,
  }
}
