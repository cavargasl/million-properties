// DTOs que coinciden con la API del backend .NET para PropertyTrace
export interface PropertyTraceDto {
  idPropertyTrace: string;
  dateSale: string;
  name: string;
  value: number;
  tax: number;
  idProperty: string;
}

export interface CreatePropertyTraceDto {
  dateSale: string;
  name: string;
  value: number;
  tax: number;
  IdProperty: string;
}

export interface UpdatePropertyTraceDto {
  id: string;
  dateSale?: string;
  name?: string;
  value?: number;
  tax?: number;
  IdProperty?: string;
}
