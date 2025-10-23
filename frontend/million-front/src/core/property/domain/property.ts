import type { Result, ResultArray, PaginatedResult } from '@/core/shared/domain/types';

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

export type PropertyResponse = Result<Property>;
export type PropertiesResponse = ResultArray<Property>;
export type PaginatedPropertiesResponse = PaginatedResult<Property>;
