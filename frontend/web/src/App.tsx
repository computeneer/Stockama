import { useState } from 'react'
import {
  AppButton,
  KeyValueList,
  Panel,
  ThemeToggleButton,
  createApiClient,
  createAuthApi,
  useApiMutation,
  useTheme,
  type LoginRequest,
  type LoginResponse,
} from '@stockama/shared'

const apiClient = createApiClient({
  baseUrl: import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5001',
})

const authApi = createAuthApi(apiClient)

function App() {
  const { theme, toggleTheme } = useTheme()
  const [email, setEmail] = useState('demo@stockama.local')
  const [password, setPassword] = useState('123456')
  const [lastToken, setLastToken] = useState<string | null>(null)

  const loginMutation = useApiMutation<LoginRequest, LoginResponse>((payload) => authApi.login(payload))

  async function handleLogin() {
    try {
      const response = await loginMutation.mutate({ email, password })
      setLastToken(response.token)
    } catch {
      setLastToken(null)
    }
  }

  return (
    <main className="mx-auto grid min-h-screen w-full max-w-3xl gap-4 px-4 py-8">
      <header className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="m-0 text-2xl font-bold tracking-tight">Stockama Web</h1>
          <p className="m-0 mt-1 text-sm text-slate-600 dark:text-slate-400">
            Tailwind + dark/light mode + shared API types.
          </p>
        </div>
        <ThemeToggleButton theme={theme} onToggle={toggleTheme} />
      </header>

      <Panel title="Kullanici Girisi" subtitle="request/response tipleri @stockama/shared/src/types/api/auth altindan gelir.">
        <label className="grid gap-1 text-sm">
          <span className="font-medium text-slate-700 dark:text-slate-300">E-posta</span>
          <input
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            className="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none ring-0 placeholder:text-slate-400 focus:border-slate-500 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
          />
        </label>

        <label className="grid gap-1 text-sm">
          <span className="font-medium text-slate-700 dark:text-slate-300">Sifre</span>
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            className="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm outline-none ring-0 placeholder:text-slate-400 focus:border-slate-500 dark:border-slate-700 dark:bg-slate-950 dark:text-slate-100"
          />
        </label>

        <AppButton onClick={() => void handleLogin()} disabled={loginMutation.isLoading}>
          Giris Yap
        </AppButton>

        <KeyValueList
          items={[
            { key: 'Theme', value: theme },
            { key: 'Durum', value: loginMutation.isLoading ? 'Gonderiliyor' : 'Hazir' },
            { key: 'Hata', value: loginMutation.error?.message ?? '-' },
            { key: 'Token', value: lastToken ? `${lastToken.slice(0, 24)}...` : '-' },
          ]}
        />
      </Panel>
    </main>
  )
}

export default App
