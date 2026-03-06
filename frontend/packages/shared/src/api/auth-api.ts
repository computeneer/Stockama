import type { ApiClient } from './http-client'
import type {
  LoginRequest,
  LoginResponse,
  RefreshTokenResponse,
  ValidateTokenResponse,
} from '../types'

interface ApiEnvelope<TData> {
  data: TData
}

export function createAuthApi(client: ApiClient) {
  return {
    async login(request: LoginRequest) {
      const response = await client.post<LoginRequest, ApiEnvelope<LoginResponse>>('/api/auth/login', request)
      return response.data
    },
    async refresh() {
      const response = await client.postNoBody<ApiEnvelope<RefreshTokenResponse>>('/api/auth/refresh')
      return response.data
    },
    async validate() {
      const response = await client.postNoBody<ApiEnvelope<ValidateTokenResponse>>('/api/auth/validate')
      return response.data
    },
    async logout() {
      const response = await client.postNoBody<ApiEnvelope<boolean>>('/api/auth/logout')
      return response.data
    },
    async revoke() {
      const response = await client.postNoBody<ApiEnvelope<boolean>>('/api/auth/revoke')
      return response.data
    },
  }
}
