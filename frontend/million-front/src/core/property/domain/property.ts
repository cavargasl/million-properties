import type { BaseError } from '@/core/shared/domain/types';

export interface Property {
  id: string;
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  ownerId: string;
  ownerName?: string;
  image?: string; // URL de la primera imagen de la propiedad
  createdAt?: string;
  updatedAt?: string;
}

export interface PropertyImage {
  id: string;
  propertyId: string;
  file: string;
  enabled: boolean;
}

export interface PropertyTrace {
  id: string;
  dateSale: string;
  name: string;
  value: number;
  tax: number;
  propertyId: string;
}

export type CreatePropertyRequest = Omit<Property, 'id' | 'createdAt' | 'updatedAt' | 'ownerName' | 'image'>;
export type UpdatePropertyRequest = Partial<CreatePropertyRequest> & { id: string };

export interface PropertyFilters {
  name?: string;
  address?: string;
  minPrice?: number;
  maxPrice?: number;
  year?: number;
  ownerId?: string;
}

export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
}

export interface PaginationMetadata {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PropertyResponse {
  data: Property | null;
  error: BaseError | null;
}

export interface PropertiesResponse {
  data: Property[] | null;
  error: BaseError | null;
}

export interface PaginatedPropertiesResponse {
  data: {
    items: Property[];
    pagination: PaginationMetadata;
  } | null;
  error: BaseError | null;
}

export interface PropertyImageResponse {
  data: PropertyImage | null;
  error: BaseError | null;
}

export interface PropertyImagesResponse {
  data: PropertyImage[] | null;
  error: BaseError | null;
}

export interface PropertyTraceResponse {
  data: PropertyTrace | null;
  error: BaseError | null;
}

export interface PropertyTracesResponse {
  data: PropertyTrace[] | null;
  error: BaseError | null;
}
