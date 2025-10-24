import type { PropertyRepository } from '../domain/propertyRepository';
import {
  mockPropertiesResponse,
  mockPaginatedPropertiesResponse,
  mockPropertyResponse,
  mockPropertyNotFoundResponse,
  mockPropertyCreatedResponse,
  mockPropertyUpdatedResponse,
  mockPropertyDeletedResponse,
  mockFilteredByOwnerResponse,
  mockFilteredByPriceRangeResponse,
} from './data';

export const propertyRepositoryMock: PropertyRepository = {
  getAll: jest.fn().mockImplementation((filters) => {
    // Simular filtrado por ownerId
    if (filters?.ownerId) {
      return Promise.resolve(mockFilteredByOwnerResponse);
    }
    // Simular filtrado por rango de precio
    if (filters?.minPrice || filters?.maxPrice) {
      return Promise.resolve(mockFilteredByPriceRangeResponse);
    }
    return Promise.resolve(mockPropertiesResponse);
  }),

  getAllPaginated: jest.fn().mockResolvedValue(mockPaginatedPropertiesResponse),

  getById: jest.fn().mockImplementation((id: string) => {
    if (id === '1') {
      return Promise.resolve(mockPropertyResponse);
    }
    return Promise.resolve(mockPropertyNotFoundResponse);
  }),

  create: jest.fn().mockResolvedValue(mockPropertyCreatedResponse),

  update: jest.fn().mockResolvedValue(mockPropertyUpdatedResponse),

  delete: jest.fn().mockResolvedValue(mockPropertyDeletedResponse),
};
