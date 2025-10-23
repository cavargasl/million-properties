import type { PropertyImageDto } from './propertyImageDto';

// DTOs que coinciden con la API del backend .NET para Property
export interface PropertyDto {
  idProperty: string;
  idOwner: string;
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  image?: string; // URL de la primera imagen habilitada
  ownerName?: string; // Nombre del propietario
}

export interface CreatePropertyDto {
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  IdOwner: string;
}

export interface UpdatePropertyDto {
  id: string;
  name?: string;
  address?: string;
  price?: number;
  codeInternal?: string;
  year?: number;
  IdOwner?: string;
}

export interface PropertyDetailDto {
  idProperty: string;
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  IdOwner: string;
  images: PropertyImageDto[];
}

// Pagination DTOs that match the backend
export interface PaginatedResponseDto<T> {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  data: T[];
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
