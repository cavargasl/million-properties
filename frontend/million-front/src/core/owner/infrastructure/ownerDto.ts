// DTOs que coinciden con la API del backend .NET
export interface OwnerDto {
  id: string;
  name: string;
  address: string;
  photo?: string;
  birthday: string;
}

export interface CreateOwnerDto {
  name: string;
  address: string;
  photo?: string;
  birthday: string;
}

export interface UpdateOwnerDto {
  id: string;
  name?: string;
  address?: string;
  photo?: string;
  birthday?: string;
}
