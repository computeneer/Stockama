import type { ReactNode } from 'react'

export interface KeyValueItem {
  key: string
  value: ReactNode
}

interface KeyValueListProps {
  items: KeyValueItem[]
}

export function KeyValueList({ items }: KeyValueListProps) {
  return (
    <dl className="grid gap-2">
      {items.map((item) => (
        <div key={item.key} className="grid grid-cols-1 gap-1 rounded-lg bg-slate-100 p-2 sm:grid-cols-[180px_1fr] dark:bg-slate-800">
          <dt className="text-sm font-semibold text-slate-700 dark:text-slate-200">{item.key}</dt>
          <dd className="m-0 text-sm text-slate-900 dark:text-slate-100">{item.value}</dd>
        </div>
      ))}
    </dl>
  )
}
