import type {
  PropertyResponse,
  PropertiesResponse,
  PaginatedPropertiesResponse,
  CreatePropertyRequest,
  UpdatePropertyRequest,
  PropertyFilters,
  PaginationParams,
  PropertyImageResponse,
  PropertyImagesResponse,
  PropertyTraceResponse,
  PropertyTracesResponse,
} from './property';

export interface PropertyRepository {
  getAll(filters?: PropertyFilters): Promise<PropertiesResponse>;
  getAllPaginated(filters?: PropertyFilters, pagination?: PaginationParams): Promise<PaginatedPropertiesResponse>;
  getById(id: string): Promise<PropertyResponse>;
  create(input: CreatePropertyRequest): Promise<PropertyResponse>;
  update(input: UpdatePropertyRequest): Promise<PropertyResponse>;
  delete(id: string): Promise<PropertyResponse>;
}

export interface PropertyImageRepository {
  getByPropertyId(propertyId: string): Promise<PropertyImagesResponse>;
  create(propertyId: string, file: File): Promise<PropertyImageResponse>;
  delete(id: string): Promise<PropertyImageResponse>;
}

export interface PropertyTraceRepository {
  getByPropertyId(propertyId: string): Promise<PropertyTracesResponse>;
  create(trace: Omit<import('./property').PropertyTrace, 'id'>): Promise<PropertyTraceResponse>;
  update(trace: import('./property').PropertyTrace): Promise<PropertyTraceResponse>;
  delete(id: string): Promise<PropertyTraceResponse>;
}
