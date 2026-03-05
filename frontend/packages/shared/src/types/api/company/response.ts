import type { ApiListResponse, Guid } from '../common'

export interface CompanyDto {
  id: Guid
  companyName: string
  isActive: boolean
  createdOn?: string
}

export type CompanyListResponse = ApiListResponse<CompanyDto>
