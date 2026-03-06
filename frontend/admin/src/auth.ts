import { createApiClient, createAuthApi, createAuthStore } from '@stockama/shared'

const ADMIN_API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? 'http://localhost:5000'

let authStoreRef: ReturnType<typeof createAuthStore> | null = null

const apiClient = createApiClient({
  baseUrl: ADMIN_API_BASE_URL,
  getToken: () => authStoreRef?.getState().accessToken ?? null,
  onTokenRotated: (token) => authStoreRef?.getState().setAccessToken(token),
})

const authApi = createAuthApi(apiClient)

export const useAdminAuthStore = createAuthStore({
  clientType: 'admin',
  cookiePrefix: 'admin',
  authApi,
})

authStoreRef = useAdminAuthStore

export { ADMIN_API_BASE_URL }
