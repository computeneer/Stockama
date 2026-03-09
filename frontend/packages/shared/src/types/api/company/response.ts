import type { ApiResponseEnvelope, Guid } from '../common'

export interface CompanyDto {
  id: Guid
  companyName: string
  isActive: boolean
  createdOn?: string
}

export type CompanyListResponse = ApiResponseEnvelope<CompanyDto[]>
export type CreateCompanyResponse = ApiResponseEnvelope<boolean>
