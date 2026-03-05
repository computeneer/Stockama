import type { ApiClient } from './http-client'
import type { CompanyListRequest, CompanyListResponse, CreateCompanyRequest, CompanyDto } from '../types'

function toQueryString(params: Record<string, string | number | undefined>) {
  const query = new URLSearchParams()

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && `${value}`.length > 0) {
      query.set(key, String(value))
    }
  })

  const built = query.toString()
  return built ? `?${built}` : ''
}

export function createCompanyApi(client: ApiClient) {
  return {
    list(request: CompanyListRequest) {
      const query = toQueryString({
        page: request.page,
        pageSize: request.pageSize,
        searchText: request.searchText,
      })

      return client.get<CompanyListResponse>(`/api/company/list${query}`)
    },
    create(request: CreateCompanyRequest) {
      return client.post<CreateCompanyRequest, CompanyDto>('/api/company/create', request)
    },
  }
}
