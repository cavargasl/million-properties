import type { PaginationParams } from '@/core/shared/domain/types';
import type {
  PropertyResponse,
  PropertiesResponse,
  PaginatedPropertiesResponse,
  CreatePropertyRequest,
  UpdatePropertyRequest,
  PropertyFilters,
} from './property';

export interface PropertyRepository {
  getAll(filters?: PropertyFilters): Promise<PropertiesResponse>;
  getAllPaginated(filters?: PropertyFilters, pagination?: PaginationParams): Promise<PaginatedPropertiesResponse>;
  getById(id: string): Promise<PropertyResponse>;
  create(input: CreatePropertyRequest): Promise<PropertyResponse>;
  update(input: UpdatePropertyRequest): Promise<PropertyResponse>;
  delete(id: string): Promise<PropertyResponse>;
}
