import type { AuthClientType } from '../common'

export interface LoginRequest {
  username: string
  password: string
  companyCode?: string
  clientType: AuthClientType
}

export interface RefreshTokenRequest {}
export interface ValidateTokenRequest {}
export interface RevokeTokenRequest {}
