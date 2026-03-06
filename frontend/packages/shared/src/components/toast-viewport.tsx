import { AnimatePresence, motion } from 'framer-motion'
import { AlertCircle, CheckCircle2, Info } from 'lucide-react'
import type { ToastVariant } from '../stores'
import { useToastStore } from '../stores'

function getToastStyles(variant: ToastVariant) {
  if (variant === 'success') {
    return {
      icon: CheckCircle2,
      iconClass: 'text-emerald-500 dark:text-emerald-400',
      cardClass: 'border-emerald-200/80 bg-white dark:border-emerald-900/80 dark:bg-slate-900',
    }
  }

  if (variant === 'error') {
    return {
      icon: AlertCircle,
      iconClass: 'text-rose-500 dark:text-rose-400',
      cardClass: 'border-rose-200/80 bg-white dark:border-rose-900/80 dark:bg-slate-900',
    }
  }

  return {
    icon: Info,
    iconClass: 'text-sky-500 dark:text-sky-400',
    cardClass: 'border-slate-200 bg-white dark:border-slate-800 dark:bg-slate-900',
  }
}

export function ToastViewport() {
  const toasts = useToastStore((state) => state.toasts)
  const dismiss = useToastStore((state) => state.dismiss)

  return (
    <div className="pointer-events-none fixed right-0 top-3 z-[60] flex w-full max-w-sm flex-col gap-2 px-3 sm:right-4 sm:top-4 sm:px-0">
      <AnimatePresence initial={false}>
        {toasts.map((toast) => {
          const styles = getToastStyles(toast.variant)
          const Icon = styles.icon

          return (
            <motion.button
              key={toast.id}
              onClick={() => dismiss(toast.id)}
              initial={{ opacity: 0, y: 10, scale: 0.985 }}
              animate={{ opacity: 1, y: 0, scale: 1 }}
              exit={{ opacity: 0, y: -6, scale: 0.985 }}
              transition={{ duration: 0.18, ease: 'easeOut' }}
              className={`pointer-events-auto flex w-full items-start gap-2 rounded-xl border px-3 py-2 text-left shadow-soft ${styles.cardClass}`}
            >
              <Icon className={`mt-0.5 h-4 w-4 shrink-0 ${styles.iconClass}`} />
              <div className="min-w-0">
                <p className="m-0 truncate text-sm font-semibold text-slate-900 dark:text-slate-100">{toast.title}</p>
                {toast.message ? <p className="m-0 mt-0.5 text-xs text-slate-600 dark:text-slate-400">{toast.message}</p> : null}
              </div>
            </motion.button>
          )
        })}
      </AnimatePresence>
    </div>
  )
}
