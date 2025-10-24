import type { PropertyImageRepository } from '../domain/propertyImageRepository';
import {
  mockPropertyImagesResponse,
  mockPropertyImageCreatedResponse,
  mockBulkPropertyImagesCreatedResponse,
  mockPropertyImageUpdatedResponse,
  mockPropertyImageDeletedResponse,
  mockPropertyImageToggledResponse,
} from './propertyImageData';

export const propertyImageRepositoryMock: PropertyImageRepository = {
  getByPropertyId: jest.fn().mockResolvedValue(mockPropertyImagesResponse),

  create: jest.fn().mockResolvedValue(mockPropertyImageCreatedResponse),

  createBulk: jest.fn().mockResolvedValue(mockBulkPropertyImagesCreatedResponse),

  update: jest.fn().mockResolvedValue(mockPropertyImageUpdatedResponse),

  delete: jest.fn().mockResolvedValue(mockPropertyImageDeletedResponse),

  toggleEnabled: jest.fn().mockResolvedValue(mockPropertyImageToggledResponse),
};
