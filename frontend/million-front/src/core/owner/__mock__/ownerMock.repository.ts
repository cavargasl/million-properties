import type { OwnerRepository } from '../domain/ownerRepository';
import {
  mockOwnersResponse,
  mockOwnerResponse,
  mockOwnerNotFoundResponse,
  mockOwnerCreatedResponse,
  mockOwnerUpdatedResponse,
  mockOwnerDeletedResponse,
} from './data';

export const ownerRepositoryMock: OwnerRepository = {
  getAll: jest.fn().mockResolvedValue(mockOwnersResponse),
  getById: jest.fn().mockImplementation((id: string) => {
    if (id === '1') {
      return Promise.resolve(mockOwnerResponse);
    }
    return Promise.resolve(mockOwnerNotFoundResponse);
  }),
  create: jest.fn().mockResolvedValue(mockOwnerCreatedResponse),
  update: jest.fn().mockResolvedValue(mockOwnerUpdatedResponse),
  delete: jest.fn().mockResolvedValue(mockOwnerDeletedResponse),
};
