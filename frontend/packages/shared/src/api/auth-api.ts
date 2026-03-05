import type { ApiClient } from './http-client'
import type { LoginRequest, LoginResponse } from '../types'

export function createAuthApi(client: ApiClient) {
  return {
    login(request: LoginRequest) {
      return client.post<LoginRequest, LoginResponse>('/api/auth/login', request)
    },
  }
}
