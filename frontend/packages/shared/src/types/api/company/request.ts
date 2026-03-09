import type { PaginationRequest } from '../common'

export interface CreateCompanyRequest {
  companyName: string
}

export interface CompanyListRequest extends PaginationRequest {
  searchText?: string
}
