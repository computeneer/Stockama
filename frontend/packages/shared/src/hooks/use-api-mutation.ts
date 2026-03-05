import { useState } from 'react'
import type { ApiError } from '../types'

export interface ApiMutationState<TRequest, TResponse> {
  isLoading: boolean
  error: ApiError | null
  mutate: (payload: TRequest) => Promise<TResponse>
}

export function useApiMutation<TRequest, TResponse>(
  mutationFn: (payload: TRequest) => Promise<TResponse>,
): ApiMutationState<TRequest, TResponse> {
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState<ApiError | null>(null)

  async function mutate(payload: TRequest): Promise<TResponse> {
    setIsLoading(true)
    setError(null)

    try {
      return await mutationFn(payload)
    } catch (err) {
      const apiError = (err as ApiError) ?? { message: 'Unknown error' }
      setError(apiError)
      throw apiError
    } finally {
      setIsLoading(false)
    }
  }

  return {
    isLoading,
    error,
    mutate,
  }
}
