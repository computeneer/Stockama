export interface LoginResponse {
  accessToken: string
  validTo: string
  requirePasswordChange: boolean
}

export interface RefreshTokenResponse {
  accessToken: string
  validTo: string
  requirePasswordChange: boolean
}

export interface ValidateTokenResponse {
  isValid: boolean
}

export interface RevokeTokenResponse {
  revoked: boolean
}
