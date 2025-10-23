// DTOs que coinciden con la API del backend .NET para PropertyImage
export interface PropertyImageDto {
  idPropertyImage: string;
  idProperty: string;
  file: string;
  enabled: boolean;
}

export interface CreatePropertyImageDto {
  IdProperty: string;
  File: string;
  Enabled: boolean;
}

export interface UpdatePropertyImageDto {
  id: string;
  idProperty: string;
  file?: string;
  enabled?: boolean;
}
