import type { PropsWithChildren } from 'react'

interface PanelProps extends PropsWithChildren {
  title: string
  subtitle?: string
  className?: string
}

export function Panel({ title, subtitle, className = '', children }: PanelProps) {
  return (
    <section
      className={`rounded-2xl border border-slate-200 bg-white p-4 shadow-sm dark:border-slate-800 dark:bg-slate-900 ${className}`.trim()}
    >
      <header className="mb-3">
        <h2 className="m-0 text-lg font-semibold text-slate-900 dark:text-slate-100">{title}</h2>
        {subtitle ? <p className="mt-1 text-sm text-slate-600 dark:text-slate-400">{subtitle}</p> : null}
      </header>
      <div className="grid gap-3">{children}</div>
    </section>
  )
}
