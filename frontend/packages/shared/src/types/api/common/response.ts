export type Guid = string

export interface ApiResponseEnvelope<TData> {
  isSuccess: boolean
  status: string
  message: string
  total?: number
  data: TData
}

export interface ApiError {
  message: string
  status?: number
  details?: unknown
}
