import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PropertyRepository } from '../../domain/propertyRepository';
import { axiosPropertyRepository } from '../axiosPropertyRepository';
import {
  mockPropertyDtos,
  mockCreatedPropertyDto,
  mockUpdatedPropertyDto,
  mockPaginatedPropertyDtos,
  mockPropertyDetailDto,
} from '../__mock__/propertyDtoData';

describe('axiosPropertyRepository integration test', () => {
  let repository: PropertyRepository;
  let mockAxios: MockAdapter;

  beforeEach(() => {
    repository = axiosPropertyRepository;
    mockAxios = new MockAdapter(apiClient);
  });

  afterEach(() => {
    mockAxios.reset();
    jest.clearAllMocks();
  });

  describe('getAll should', () => {
    it('return all properties successfully', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.PROPERTIES).reply(200, mockPropertyDtos);

      // Act
      const result = await repository.getAll();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.length).toEqual(mockPropertyDtos.length);

      result.data?.forEach((property, index) => {
        expect(property.id).toEqual(mockPropertyDtos[index].idProperty);
        expect(property.name).toEqual(mockPropertyDtos[index].name);
        expect(property.price).toEqual(mockPropertyDtos[index].price);
      });
    });

    it('return error when API call fails', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.PROPERTIES).reply(500, {
        error: 'Internal server error',
      });

      // Act
      const result = await repository.getAll();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toBeDefined();
    });

    it('transform DTO fields correctly to domain entity', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.PROPERTIES).reply(200, mockPropertyDtos);

      // Act
      const result = await repository.getAll();

      // Assert
      const firstProperty = result.data?.[0];
      expect(firstProperty).toBeDefined();

      // Verificar que idProperty se transformó a id
      expect(firstProperty?.id).toEqual(mockPropertyDtos[0].idProperty);
      expect(firstProperty).not.toHaveProperty('idProperty');

      // Verificar que idOwner se transformó a ownerId
      expect(firstProperty?.ownerId).toEqual(mockPropertyDtos[0].idOwner);
      expect(firstProperty).not.toHaveProperty('idOwner');
    });

    it('pass filters as query parameters', async () => {
      // Arrange
      const filters = { ownerId: '1', minPrice: 100000 };
      mockAxios.onGet(API_ENDPOINTS.PROPERTIES).reply(200, mockPropertyDtos);

      // Act
      await repository.getAll(filters);

      // Assert
      expect(mockAxios.history.get.length).toBe(1);
      expect(mockAxios.history.get[0].params).toEqual(filters);
    });
  });

  describe('getAllPaginated should', () => {
    it('return paginated properties successfully', async () => {
      // Arrange
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/search/paginated`)
        .reply(200, mockPaginatedPropertyDtos);

      // Act
      const result = await repository.getAllPaginated();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.items).toBeDefined();
      expect(result.data?.pagination).toBeDefined();
      expect(result.data?.pagination.pageNumber).toEqual(1);
      expect(result.data?.items.length).toEqual(2);
    });

    it('pass pagination params to API', async () => {
      // Arrange
      const pagination = { pageNumber: 2, pageSize: 5 };
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/search/paginated`)
        .reply(200, mockPaginatedPropertyDtos);

      // Act
      await repository.getAllPaginated(undefined, pagination);

      // Assert
      expect(mockAxios.history.get.length).toBe(1);
      expect(mockAxios.history.get[0].params).toMatchObject({
        pageNumber: 2,
        pageSize: 5,
      });
    });

    it('use default pagination when not provided', async () => {
      // Arrange
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/search/paginated`)
        .reply(200, mockPaginatedPropertyDtos);

      // Act
      await repository.getAllPaginated();

      // Assert
      expect(mockAxios.history.get[0].params).toMatchObject({
        pageNumber: 1,
        pageSize: 10,
      });
    });

    it('return error when API call fails', async () => {
      // Arrange
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/search/paginated`).reply(500);

      // Act
      const result = await repository.getAllPaginated();

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });
  });

  describe('getById should', () => {
    it('return property by id successfully', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`)
        .reply(200, mockPropertyDetailDto);

      // Act
      const result = await repository.getById(propertyId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.id).toEqual(propertyId);
      expect(result.data?.name).toEqual(mockPropertyDetailDto.name);
      expect(result.data?.ownerName).toEqual(mockPropertyDetailDto.owner.name);
      expect(result.data?.image).toEqual(mockPropertyDetailDto.images[0].file);
    });

    it('return error when property not found', async () => {
      // Arrange
      const propertyId = '999';
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`).reply(404, {
        error: 'Property not found',
      });

      // Act
      const result = await repository.getById(propertyId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('call correct endpoint with property id', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`)
        .reply(200, mockPropertyDetailDto);

      // Act
      await repository.getById(propertyId);

      // Assert
      expect(mockAxios.history.get.length).toBe(1);
      expect(mockAxios.history.get[0].url).toBe(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`);
    });
  });

  describe('create should', () => {
    it('create property successfully', async () => {
      // Arrange
      const newProperty = {
        name: 'New Property',
        address: '555 Park Ave',
        price: 750000,
        codeInternal: 'PROP-004',
        year: 2023,
        ownerId: '2',
      };

      mockAxios.onPost(API_ENDPOINTS.PROPERTIES).reply(201, mockCreatedPropertyDto);

      // Act
      const result = await repository.create(newProperty);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.name).toEqual(newProperty.name);
      expect(result.data?.id).toBeDefined();
    });

    it('return error when validation fails', async () => {
      // Arrange
      const invalidProperty = {
        name: '',
        address: '555 Park Ave',
        price: 750000,
        codeInternal: 'PROP-004',
        year: 2023,
        ownerId: '2',
      };

      mockAxios.onPost(API_ENDPOINTS.PROPERTIES).reply(400, {
        errors: {
          Name: ['Name is required'],
        },
      });

      // Act
      const result = await repository.create(invalidProperty);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toContain('Name is required');
    });

    it('send correct DTO format to API', async () => {
      // Arrange
      const newProperty = {
        name: 'Test Property',
        address: 'Test Address',
        price: 500000,
        codeInternal: 'TEST-001',
        year: 2023,
        ownerId: '1',
      };

      mockAxios.onPost(API_ENDPOINTS.PROPERTIES).reply((config) => {
        const requestData = JSON.parse(config.data || '{}');

        // Verificar que ownerId se transformó a IdOwner
        expect(requestData).toHaveProperty('IdOwner');
        expect(requestData).not.toHaveProperty('ownerId');
        expect(requestData).not.toHaveProperty('id');

        return [201, mockCreatedPropertyDto];
      });

      // Act
      await repository.create(newProperty);

      // Assert
      expect(mockAxios.history.post.length).toBe(1);
    });
  });

  describe('update should', () => {
    it('update property successfully', async () => {
      // Arrange
      const updateData = {
        id: '1',
        name: 'Luxury Villa Updated',
        price: 2750000,
      };

      mockAxios
        .onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.id}`)
        .reply(200, mockUpdatedPropertyDto);

      // Act
      const result = await repository.update(updateData);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.id).toEqual(updateData.id);
      expect(result.data?.name).toEqual(updateData.name);
    });

    it('return error when property not found', async () => {
      // Arrange
      const updateData = {
        id: '999',
        name: 'Non Existent',
      };

      mockAxios.onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.id}`).reply(404, {
        error: 'Property not found',
      });

      // Act
      const result = await repository.update(updateData);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('send correct DTO format to API', async () => {
      // Arrange
      const updateData = {
        id: '1',
        name: 'Updated Name',
        ownerId: '2',
      };

      mockAxios.onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.id}`).reply((config) => {
        const requestData = JSON.parse(config.data || '{}');

        // Verificar que id está en el DTO
        expect(requestData).toHaveProperty('id', updateData.id);
        // Verificar que ownerId se transformó a IdOwner
        if (updateData.ownerId) {
          expect(requestData).toHaveProperty('IdOwner');
          expect(requestData).not.toHaveProperty('ownerId');
        }

        return [200, mockUpdatedPropertyDto];
      });

      // Act
      await repository.update(updateData);

      // Assert
      expect(mockAxios.history.put.length).toBe(1);
    });
  });

  describe('delete should', () => {
    it('delete property successfully', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios.onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`).reply(200, mockPropertyDtos[0]);

      // Act
      const result = await repository.delete(propertyId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
    });

    it('return error when property not found', async () => {
      // Arrange
      const propertyId = '999';
      mockAxios.onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`).reply(404, {
        error: 'Property not found',
      });

      // Act
      const result = await repository.delete(propertyId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('call correct endpoint with property id', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios.onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`).reply(200, mockPropertyDtos[0]);

      // Act
      await repository.delete(propertyId);

      // Assert
      expect(mockAxios.history.delete.length).toBe(1);
      expect(mockAxios.history.delete[0].url).toBe(`${API_ENDPOINTS.PROPERTIES}/${propertyId}`);
    });
  });

  describe('error handling should', () => {
    it('handle network errors', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.PROPERTIES).networkError();

      // Act
      const result = await repository.getAll();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('handle timeout errors', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.PROPERTIES).timeout();

      // Act
      const result = await repository.getAll();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('transform validation errors from backend', async () => {
      // Arrange
      const invalidProperty = {
        name: '',
        address: '',
        price: -100,
        codeInternal: '',
        year: 0,
        ownerId: '',
      };

      mockAxios.onPost(API_ENDPOINTS.PROPERTIES).reply(400, {
        errors: {
          Name: ['Name is required'],
          Address: ['Address is required'],
          Price: ['Price must be greater than 0'],
        },
      });

      // Act
      const result = await repository.create(invalidProperty);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toContain('Name is required');
    });
  });
});
