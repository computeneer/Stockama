import type { ApiError } from '../types'

export interface ApiClientOptions {
  baseUrl: string
  getToken?: () => string | null | undefined
  onTokenRotated?: (token: string) => void
}

export class ApiClient {
  private readonly baseUrl: string
  private readonly getToken?: () => string | null | undefined
  private readonly onTokenRotated?: (token: string) => void

  constructor(options: ApiClientOptions) {
    this.baseUrl = options.baseUrl.replace(/\/$/, '')
    this.getToken = options.getToken
    this.onTokenRotated = options.onTokenRotated
  }

  async get<TResponse>(path: string): Promise<TResponse> {
    return this.request<TResponse>(path, { method: 'GET' })
  }

  async post<TRequest, TResponse>(path: string, body: TRequest): Promise<TResponse> {
    return this.request<TResponse>(path, {
      method: 'POST',
      body: JSON.stringify(body),
    })
  }

  async postNoBody<TResponse>(path: string): Promise<TResponse> {
    return this.request<TResponse>(path, { method: 'POST' })
  }

  async put<TRequest, TResponse>(path: string, body: TRequest): Promise<TResponse> {
    return this.request<TResponse>(path, {
      method: 'PUT',
      body: JSON.stringify(body),
    })
  }

  async delete<TResponse>(path: string): Promise<TResponse> {
    return this.request<TResponse>(path, { method: 'DELETE' })
  }

  private async request<TResponse>(path: string, init: RequestInit): Promise<TResponse> {
    const headers = new Headers(init.headers)
    if (init.body !== undefined) {
      headers.set('Content-Type', 'application/json')
    }

    const token = this.getToken?.()
    if (token) {
      headers.set('Authorization', `Bearer ${token}`)
    }

    const response = await fetch(`${this.baseUrl}${path}`, {
      ...init,
      headers,
    })

    const rotatedToken = response.headers.get('X-Access-Token')
    if (rotatedToken) {
      this.onTokenRotated?.(rotatedToken)
    }

    if (!response.ok) {
      let details: unknown = undefined
      try {
        details = await response.json()
      } catch {
        details = await response.text()
      }

      const error: ApiError = {
        message: `API request failed: ${response.status}`,
        status: response.status,
        details,
      }
      throw error
    }

    if (response.status === 204) {
      return undefined as TResponse
    }

    return (await response.json()) as TResponse
  }
}

export function createApiClient(options: ApiClientOptions): ApiClient {
  return new ApiClient(options)
}
