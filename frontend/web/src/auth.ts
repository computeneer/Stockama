import { createApiClient, createAuthApi, createAuthStore } from '@stockama/shared'

const WEB_API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? 'http://localhost:5001'

let authStoreRef: ReturnType<typeof createAuthStore> | null = null

const apiClient = createApiClient({
  baseUrl: WEB_API_BASE_URL,
  getToken: () => authStoreRef?.getState().accessToken ?? null,
  onTokenRotated: (token) => authStoreRef?.getState().setAccessToken(token),
})

const authApi = createAuthApi(apiClient)

export const useWebAuthStore = createAuthStore({
  clientType: 'web',
  cookiePrefix: 'web',
  authApi,
})

authStoreRef = useWebAuthStore

export { WEB_API_BASE_URL }
