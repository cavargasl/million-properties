// DTOs que coinciden con la API del backend .NET
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

export interface PropertyImageDto {
  idPropertyImage: string;
  idProperty: string;
  file: string;
  enabled: boolean;
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

export interface PropertyTraceDto {
  id: string;
  dateSale: string;
  name: string;
  value: number;
  tax: number;
  propertyId: string;
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
