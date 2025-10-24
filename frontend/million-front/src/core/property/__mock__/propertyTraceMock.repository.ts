import type { PropertyTraceRepository } from '../domain/propertyTraceRepository';
import {
  mockPropertyTracesResponse,
  mockPropertyTraceCreatedResponse,
  mockPropertyTraceUpdatedResponse,
  mockPropertyTraceDeletedResponse,
} from './propertyTraceData';

export const propertyTraceRepositoryMock: PropertyTraceRepository = {
  getByPropertyId: jest.fn().mockResolvedValue(mockPropertyTracesResponse),

  create: jest.fn().mockResolvedValue(mockPropertyTraceCreatedResponse),

  update: jest.fn().mockResolvedValue(mockPropertyTraceUpdatedResponse),

  delete: jest.fn().mockResolvedValue(mockPropertyTraceDeletedResponse),
};
