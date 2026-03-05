import type { ApiError } from '../types'

export interface ApiClientOptions {
  baseUrl: string
  getToken?: () => string | null | undefined
}

export class ApiClient {
  private readonly baseUrl: string
  private readonly getToken?: () => string | null | undefined

  constructor(options: ApiClientOptions) {
    this.baseUrl = options.baseUrl.replace(/\/$/, '')
    this.getToken = options.getToken
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
    headers.set('Content-Type', 'application/json')

    const token = this.getToken?.()
    if (token) {
      headers.set('Authorization', `Bearer ${token}`)
    }

    const response = await fetch(`${this.baseUrl}${path}`, {
      ...init,
      headers,
    })

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
