// DTOs que coinciden con la API del backend .NET
export interface OwnerDto {
  idOwner: string;
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
  idOwner: string;
  name?: string;
  address?: string;
  photo?: string;
  birthday?: string;
}
