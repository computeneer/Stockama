export type Guid = string

export interface ApiError {
  message: string
  status?: number
  details?: unknown
}

export interface ApiListResponse<T> {
  data: T[]
  totalCount?: number
}

export interface ApiDataResponse<T> {
  data: T
}
