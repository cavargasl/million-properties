import type { OwnerResponse, OwnersResponse, CreateOwnerRequest, UpdateOwnerRequest } from './owner';

export interface OwnerRepository {
  getAll(): Promise<OwnersResponse>;
  getById(id: string): Promise<OwnerResponse>;
  create(input: CreateOwnerRequest): Promise<OwnerResponse>;
  update(input: UpdateOwnerRequest): Promise<OwnerResponse>;
  delete(id: string): Promise<OwnerResponse>;
}
