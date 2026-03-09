import type { ThemeMode } from '../hooks'
import { AppButton } from './button'

interface ThemeToggleButtonProps {
  theme: ThemeMode
  onToggle: () => void
  className?: string
}

export function ThemeToggleButton({ theme, onToggle, className = '' }: ThemeToggleButtonProps) {
  return (
    <AppButton variant="secondary" className={className} onClick={onToggle}>
      {theme === 'dark' ? 'Light Mode' : 'Dark Mode'}
    </AppButton>
  )
}
