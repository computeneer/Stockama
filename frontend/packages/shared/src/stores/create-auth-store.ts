import Cookies from 'js-cookie'
import { create, type UseBoundStore } from 'zustand'
import type { StoreApi } from 'zustand'
import type {
  AuthClientType,
  LoginRequest,
  LoginResponse,
  RefreshTokenResponse,
  ValidateTokenResponse,
} from '../types'

interface AuthApi {
  login: (request: LoginRequest) => Promise<LoginResponse>
  refresh: () => Promise<RefreshTokenResponse>
  validate: () => Promise<ValidateTokenResponse>
  logout: () => Promise<boolean>
  revoke: () => Promise<boolean>
}

export interface CreateAuthStoreOptions {
  clientType: AuthClientType
  cookiePrefix: string
  authApi: AuthApi
}

export interface AuthStoreState {
  accessToken: string | null
  isAuthenticated: boolean
  isInitializing: boolean
  setAccessToken: (accessToken: string | null) => void
  login: (username: string, password: string, companyCode?: string) => Promise<void>
  initializeSession: () => Promise<void>
  logout: () => Promise<void>
}

const COOKIE_EXPIRES_DAYS = 7

export function createAuthStore(options: CreateAuthStoreOptions): UseBoundStore<StoreApi<AuthStoreState>> {
  const accessCookieName = `${options.cookiePrefix}_access_token`

  function getCookieOptions(): Cookies.CookieAttributes {
    return {
      expires: COOKIE_EXPIRES_DAYS,
      sameSite: 'lax',
      secure: window.location.protocol === 'https:',
      path: '/',
    }
  }

  function saveTokens(accessToken: string) {
    Cookies.set(accessCookieName, accessToken, getCookieOptions())
  }

  function clearTokens() {
    Cookies.remove(accessCookieName, { path: '/' })
  }

  function readAccessToken() {
    return Cookies.get(accessCookieName) ?? null
  }

  async function tryRefreshToken(): Promise<RefreshTokenResponse | null> {
    try {
      return await options.authApi.refresh()
    } catch {
      return null
    }
  }

  async function validateAccessToken(): Promise<boolean> {
    try {
      const result = await options.authApi.validate()

      return result.isValid
    } catch {
      return false
    }
  }

  return create<AuthStoreState>((set) => ({
    accessToken: readAccessToken(),
    isAuthenticated: !!readAccessToken(),
    isInitializing: true,
    setAccessToken: (accessToken: string | null) => {
      if (!accessToken) {
        clearTokens()
        set({
          accessToken: null,
          isAuthenticated: false,
        })
        return
      }

      saveTokens(accessToken)
      set({
        accessToken,
        isAuthenticated: true,
      })
    },
    login: async (username: string, password: string, companyCode?: string) => {
      const response = await options.authApi.login({
        username,
        password,
        companyCode,
        clientType: options.clientType,
      })

      saveTokens(response.accessToken)

      set({
        accessToken: response.accessToken,
        isAuthenticated: true,
      })
    },
    initializeSession: async () => {
      set({ isInitializing: true })

      const accessToken = readAccessToken()
      if (!accessToken) {
        clearTokens()
        set({
          accessToken: null,
          isAuthenticated: false,
          isInitializing: false,
        })
        return
      }

      if (accessToken) {
        const isValid = await validateAccessToken()
        if (isValid) {
          set({
            accessToken,
            isAuthenticated: true,
            isInitializing: false,
          })
          return
        }

        const refreshed = await tryRefreshToken()
        if (refreshed) {
          saveTokens(refreshed.accessToken)
          set({
            accessToken: refreshed.accessToken,
            isAuthenticated: true,
            isInitializing: false,
          })
          return
        }
      }

      clearTokens()
      set({
        accessToken: null,
        isAuthenticated: false,
        isInitializing: false,
      })
    },
    logout: async () => {
      const accessToken = readAccessToken()
      if (accessToken) {
        try {
          await options.authApi.logout()
        } catch {
          // Local cleanup must still happen even if revoke fails.
        }
      }

      clearTokens()
      set({
        accessToken: null,
        isAuthenticated: false,
      })
    },
  }))
}
