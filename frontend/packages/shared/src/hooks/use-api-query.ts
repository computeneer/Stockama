import { useCallback, useEffect, useState } from 'react'
import type { ApiError } from '../types'

export interface ApiQueryState<TData> {
  data: TData | null
  error: ApiError | null
  isLoading: boolean
  refetch: () => Promise<void>
}

export interface UseApiQueryOptions {
  enabled?: boolean
}

export function useApiQuery<TData>(
  queryFn: () => Promise<TData>,
  deps: unknown[] = [],
  options: UseApiQueryOptions = {},
): ApiQueryState<TData> {
  const { enabled = true } = options
  const [data, setData] = useState<TData | null>(null)
  const [error, setError] = useState<ApiError | null>(null)
  const [isLoading, setIsLoading] = useState(false)

  const refetch = useCallback(async () => {
    if (!enabled) {
      return
    }

    setIsLoading(true)
    setError(null)

    try {
      const response = await queryFn()
      setData(response)
    } catch (err) {
      setError((err as ApiError) ?? { message: 'Unknown error' })
    } finally {
      setIsLoading(false)
    }
  }, [enabled, ...deps])

  useEffect(() => {
    if (!enabled) {
      setIsLoading(false)
      return
    }

    void refetch()
  }, [enabled, refetch])

  return {
    data,
    error,
    isLoading,
    refetch,
  }
}
