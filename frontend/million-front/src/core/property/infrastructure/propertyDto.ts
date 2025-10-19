// DTOs que coinciden con la API del backend .NET
export interface PropertyDto {
  id: string;
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  ownerId: string;
  ownerName?: string;
}

export interface CreatePropertyDto {
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  ownerId: string;
}

export interface UpdatePropertyDto {
  id: string;
  name?: string;
  address?: string;
  price?: number;
  codeInternal?: string;
  year?: number;
  ownerId?: string;
}

export interface PropertyImageDto {
  id: string;
  propertyId: string;
  file: string;
  enabled: boolean;
}

export interface PropertyTraceDto {
  id: string;
  dateSale: string;
  name: string;
  value: number;
  tax: number;
  propertyId: string;
}
