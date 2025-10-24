import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PropertyImageRepository } from '../../domain/propertyImageRepository';
import { axiosPropertyImageRepository } from '../axiosPropertyImageRepository';
import {
  mockPropertyImageDtos,
  mockCreatedPropertyImageDto,
  mockBulkCreatedPropertyImageDtos,
  mockUpdatedPropertyImageDto,
} from '../__mock__/propertyImageDtoData';

describe('axiosPropertyImageRepository integration test', () => {
  let repository: PropertyImageRepository;
  let mockAxios: MockAdapter;

  beforeEach(() => {
    repository = axiosPropertyImageRepository;
    mockAxios = new MockAdapter(apiClient);
  });

  afterEach(() => {
    mockAxios.reset();
    jest.clearAllMocks();
  });

  describe('getByPropertyId should', () => {
    it('return all images for a property successfully', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images`)
        .reply(200, mockPropertyImageDtos);

      // Act
      const result = await repository.getByPropertyId(propertyId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.length).toEqual(mockPropertyImageDtos.length);

      result.data?.forEach((image, index) => {
        expect(image.id).toEqual(mockPropertyImageDtos[index].idPropertyImage);
        expect(image.propertyId).toEqual(mockPropertyImageDtos[index].idProperty);
        expect(image.file).toEqual(mockPropertyImageDtos[index].file);
      });
    });

    it('return error when API call fails', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images`).reply(500);

      // Act
      const result = await repository.getByPropertyId(propertyId);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('transform DTO fields correctly', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images`)
        .reply(200, mockPropertyImageDtos);

      // Act
      const result = await repository.getByPropertyId(propertyId);

      // Assert
      const firstImage = result.data?.[0];
      expect(firstImage?.id).toEqual(mockPropertyImageDtos[0].idPropertyImage);
      expect(firstImage?.propertyId).toEqual(mockPropertyImageDtos[0].idProperty);
      expect(firstImage).not.toHaveProperty('idPropertyImage');
      expect(firstImage).not.toHaveProperty('idProperty');
    });
  });

  describe('create should', () => {
    it('create property image successfully', async () => {
      // Arrange
      const newImage = {
        propertyId: '1',
        file: 'https://example.com/images/new-image.jpg',
        enabled: true,
      };

      mockAxios
        .onPost(`${API_ENDPOINTS.PROPERTIES}/${newImage.propertyId}/images`)
        .reply(201, mockCreatedPropertyImageDto);

      // Act
      const result = await repository.create(newImage);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.file).toEqual(newImage.file);
    });

    it('return error when validation fails', async () => {
      // Arrange
      const invalidImage = {
        propertyId: '1',
        file: '',
        enabled: true,
      };

      mockAxios.onPost(`${API_ENDPOINTS.PROPERTIES}/${invalidImage.propertyId}/images`).reply(400, {
        errors: {
          File: ['File is required'],
        },
      });

      // Act
      const result = await repository.create(invalidImage);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('send correct DTO format to API', async () => {
      // Arrange
      const newImage = {
        propertyId: '1',
        file: 'https://example.com/test.jpg',
        enabled: true,
      };

      mockAxios.onPost(`${API_ENDPOINTS.PROPERTIES}/${newImage.propertyId}/images`).reply((config) => {
        const requestData = JSON.parse(config.data || '{}');

        // Verificar transformación a formato backend
        expect(requestData).toHaveProperty('IdProperty');
        expect(requestData).toHaveProperty('File');
        expect(requestData).toHaveProperty('Enabled');
        expect(requestData).not.toHaveProperty('propertyId');
        expect(requestData).not.toHaveProperty('file');
        expect(requestData).not.toHaveProperty('enabled');

        return [201, mockCreatedPropertyImageDto];
      });

      // Act
      await repository.create(newImage);

      // Assert
      expect(mockAxios.history.post.length).toBe(1);
    });
  });

  describe('createBulk should', () => {
    it('create multiple images successfully', async () => {
      // Arrange
      const propertyId = '2';
      const newImages = [
        { propertyId, file: 'https://example.com/bulk1.jpg', enabled: true },
        { propertyId, file: 'https://example.com/bulk2.jpg', enabled: true },
      ];

      mockAxios
        .onPost(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/bulk`)
        .reply(201, mockBulkCreatedPropertyImageDtos);

      // Act
      const result = await repository.createBulk(propertyId, newImages);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.length).toEqual(2);
    });

    it('return error when API call fails', async () => {
      // Arrange
      const propertyId = '2';
      const newImages = [
        { propertyId, file: 'https://example.com/bulk1.jpg', enabled: true },
      ];

      mockAxios.onPost(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/bulk`).reply(400);

      // Act
      const result = await repository.createBulk(propertyId, newImages);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('send array of DTOs to API', async () => {
      // Arrange
      const propertyId = '2';
      const newImages = [
        { propertyId, file: 'test1.jpg', enabled: true },
        { propertyId, file: 'test2.jpg', enabled: false },
      ];

      mockAxios.onPost(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/bulk`).reply((config) => {
        const requestData = JSON.parse(config.data || '[]');

        expect(Array.isArray(requestData)).toBe(true);
        expect(requestData.length).toBe(2);
        requestData.forEach((dto: any) => {
          expect(dto).toHaveProperty('IdProperty');
          expect(dto).toHaveProperty('File');
          expect(dto).toHaveProperty('Enabled');
        });

        return [201, mockBulkCreatedPropertyImageDtos];
      });

      // Act
      await repository.createBulk(propertyId, newImages);

      // Assert
      expect(mockAxios.history.post.length).toBe(1);
    });
  });

  describe('update should', () => {
    it('update property image successfully', async () => {
      // Arrange
      const updateData = {
        id: '1',
        propertyId: '1',
        enabled: false,
      };

      mockAxios
        .onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.propertyId}/images/${updateData.id}`)
        .reply(200, mockUpdatedPropertyImageDto);

      // Act
      const result = await repository.update(updateData);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.enabled).toEqual(false);
    });

    it('return error when image not found', async () => {
      // Arrange
      const updateData = {
        id: '999',
        propertyId: '1',
        enabled: false,
      };

      mockAxios
        .onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.propertyId}/images/${updateData.id}`)
        .reply(404);

      // Act
      const result = await repository.update(updateData);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });
  });

  describe('delete should', () => {
    it('delete property image successfully', async () => {
      // Arrange
      const propertyId = '1';
      const imageId = '1';

      mockAxios
        .onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${imageId}`)
        .reply(200, mockPropertyImageDtos[0]);

      // Act
      const result = await repository.delete(propertyId, imageId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
    });

    it('return error when image not found', async () => {
      // Arrange
      const propertyId = '1';
      const imageId = '999';

      mockAxios
        .onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${imageId}`)
        .reply(404);

      // Act
      const result = await repository.delete(propertyId, imageId);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('call correct endpoint', async () => {
      // Arrange
      const propertyId = '1';
      const imageId = '1';

      mockAxios
        .onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${imageId}`)
        .reply(200, mockPropertyImageDtos[0]);

      // Act
      await repository.delete(propertyId, imageId);

      // Assert
      expect(mockAxios.history.delete.length).toBe(1);
      expect(mockAxios.history.delete[0].url).toBe(
        `${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${imageId}`
      );
    });
  });

  describe('toggleEnabled should', () => {
    it('toggle enabled status successfully', async () => {
      // Arrange
      const propertyId = '1';
      const imageId = '1';
      const enabled = false;

      mockAxios
        .onPatch(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${imageId}/toggle`)
        .reply(200, mockUpdatedPropertyImageDto);

      // Act
      const result = await repository.toggleEnabled(propertyId, imageId, enabled);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
    });

    it('send enabled value in request body', async () => {
      // Arrange
      const propertyId = '1';
      const imageId = '1';
      const enabled = false;

      mockAxios
        .onPatch(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${imageId}/toggle`)
        .reply((config) => {
          const requestData = JSON.parse(config.data || '{}');
          expect(requestData).toHaveProperty('enabled', enabled);
          return [200, mockUpdatedPropertyImageDto];
        });

      // Act
      await repository.toggleEnabled(propertyId, imageId, enabled);

      // Assert
      expect(mockAxios.history.patch.length).toBe(1);
    });

    it('return error when image not found', async () => {
      // Arrange
      const propertyId = '1';
      const imageId = '999';

      mockAxios
        .onPatch(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${imageId}/toggle`)
        .reply(404);

      // Act
      const result = await repository.toggleEnabled(propertyId, imageId, true);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });
  });

  describe('error handling should', () => {
    it('handle network errors', async () => {
      // Arrange
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/1/images`).networkError();

      // Act
      const result = await repository.getByPropertyId('1');

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('handle timeout errors', async () => {
      // Arrange
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/1/images`).timeout();

      // Act
      const result = await repository.getByPropertyId('1');

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });
  });
});
