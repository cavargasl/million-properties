import type { BaseError } from '@/core/shared/domain/types';

export interface Owner {
  id: string;
  name: string;
  address: string;
  photo?: string;
  birthday: string;
  createdAt?: string;
  updatedAt?: string;
}

export type CreateOwnerRequest = Omit<Owner, 'id' | 'createdAt' | 'updatedAt'>;
export type UpdateOwnerRequest = Partial<CreateOwnerRequest> & { id: string };

export interface OwnerResponse {
  data: Owner | null;
  error: BaseError | null;
}

export interface OwnersResponse {
  data: Owner[] | null;
  error: BaseError | null;
}
