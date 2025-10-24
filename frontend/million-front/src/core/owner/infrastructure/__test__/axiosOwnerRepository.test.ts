import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { OwnerRepository } from '../../domain/ownerRepository';
import { axiosOwnerRepository } from '../axiosOwnerRepository';
import {
  mockOwnerDtos,
  mockCreatedOwnerDto,
  mockUpdatedOwnerDto,
} from '../__mock__/dtoData';

describe('axiosOwnerRepository integration test', () => {
  let repository: OwnerRepository;
  let mockAxios: MockAdapter;

  beforeEach(() => {
    repository = axiosOwnerRepository;
    mockAxios = new MockAdapter(apiClient);
  });

  afterEach(() => {
    mockAxios.reset();
    jest.clearAllMocks();
  });

  describe('getAll should', () => {
    it('return all owners successfully', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.OWNERS).reply(200, mockOwnerDtos);

      // Act
      const result = await repository.getAll();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.length).toEqual(mockOwnerDtos.length);

      result.data?.forEach((owner, index) => {
        expect(owner.id).toEqual(mockOwnerDtos[index].idOwner);
        expect(owner.name).toEqual(mockOwnerDtos[index].name);
        expect(owner.address).toEqual(mockOwnerDtos[index].address);
        expect(owner.birthday).toEqual(mockOwnerDtos[index].birthday);
      });
    });

    it('return error when API call fails', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.OWNERS).reply(500, {
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
      mockAxios.onGet(API_ENDPOINTS.OWNERS).reply(200, mockOwnerDtos);

      // Act
      const result = await repository.getAll();

      // Assert
      const firstOwner = result.data?.[0];
      expect(firstOwner).toBeDefined();
      
      // Verificar que idOwner se transformó a id
      expect(firstOwner?.id).toEqual(mockOwnerDtos[0].idOwner);
      expect(firstOwner).not.toHaveProperty('idOwner');
    });
  });

  describe('getById should', () => {
    it('return owner by id successfully', async () => {
      // Arrange
      const ownerId = '1';
      mockAxios.onGet(`${API_ENDPOINTS.OWNERS}/${ownerId}`).reply(200, mockOwnerDtos[0]);

      // Act
      const result = await repository.getById(ownerId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.id).toEqual(ownerId);
      expect(result.data?.name).toEqual(mockOwnerDtos[0].name);
    });

    it('return error when owner not found', async () => {
      // Arrange
      const ownerId = '999';
      mockAxios.onGet(`${API_ENDPOINTS.OWNERS}/${ownerId}`).reply(404, {
        error: 'Owner not found',
      });

      // Act
      const result = await repository.getById(ownerId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toBeDefined();
    });

    it('call correct endpoint with owner id', async () => {
      // Arrange
      const ownerId = '1';
      mockAxios.onGet(`${API_ENDPOINTS.OWNERS}/${ownerId}`).reply(200, mockOwnerDtos[0]);

      // Act
      await repository.getById(ownerId);

      // Assert
      expect(mockAxios.history.get.length).toBe(1);
      expect(mockAxios.history.get[0].url).toBe(`${API_ENDPOINTS.OWNERS}/${ownerId}`);
    });
  });

  describe('create should', () => {
    it('create owner successfully', async () => {
      // Arrange
      const newOwner = {
        name: 'Alice Williams',
        address: '321 Elm St, Miami, FL 33101',
        photo: 'https://example.com/photos/alice-williams.jpg',
        birthday: '1990-03-25',
      };

      mockAxios.onPost(API_ENDPOINTS.OWNERS).reply(201, mockCreatedOwnerDto);

      // Act
      const result = await repository.create(newOwner);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.name).toEqual(newOwner.name);
      expect(result.data?.address).toEqual(newOwner.address);
      expect(result.data?.id).toBeDefined();
    });

    it('return error when validation fails', async () => {
      // Arrange
      const invalidOwner = {
        name: '',
        address: '321 Elm St',
        birthday: '1990-03-25',
      };

      mockAxios.onPost(API_ENDPOINTS.OWNERS).reply(400, {
        errors: {
          Name: ['Name is required'],
        },
      });

      // Act
      const result = await repository.create(invalidOwner);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toContain('Name is required');
    });

    it('send correct DTO format to API', async () => {
      // Arrange
      const newOwner = {
        name: 'Test Owner',
        address: 'Test Address',
        birthday: '1990-01-01',
      };

      mockAxios.onPost(API_ENDPOINTS.OWNERS).reply((config) => {
        const requestData = JSON.parse(config.data || '{}');
        
        // Verificar que se envía en formato DTO (sin 'id')
        expect(requestData).toHaveProperty('name');
        expect(requestData).toHaveProperty('address');
        expect(requestData).toHaveProperty('birthday');
        expect(requestData).not.toHaveProperty('id');
        expect(requestData).not.toHaveProperty('idOwner');
        
        return [201, mockCreatedOwnerDto];
      });

      // Act
      await repository.create(newOwner);

      // Assert
      expect(mockAxios.history.post.length).toBe(1);
    });
  });

  describe('update should', () => {
    it('update owner successfully', async () => {
      // Arrange
      const updateData = {
        id: '1',
        name: 'John Doe Updated',
        address: '123 Main St, New York, NY 10001',
      };

      mockAxios.onPut(`${API_ENDPOINTS.OWNERS}/${updateData.id}`).reply(200, mockUpdatedOwnerDto);

      // Act
      const result = await repository.update(updateData);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.id).toEqual(updateData.id);
      expect(result.data?.name).toEqual(updateData.name);
    });

    it('return error when owner not found', async () => {
      // Arrange
      const updateData = {
        id: '999',
        name: 'Non Existent',
      };

      mockAxios.onPut(`${API_ENDPOINTS.OWNERS}/${updateData.id}`).reply(404, {
        error: 'Owner not found',
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
      };

      mockAxios.onPut(`${API_ENDPOINTS.OWNERS}/${updateData.id}`).reply((config) => {
        const requestData = JSON.parse(config.data || '{}');
        
        // Verificar que 'id' se transformó a 'idOwner'
        expect(requestData).toHaveProperty('idOwner', updateData.id);
        expect(requestData).not.toHaveProperty('id');
        
        return [200, mockUpdatedOwnerDto];
      });

      // Act
      await repository.update(updateData);

      // Assert
      expect(mockAxios.history.put.length).toBe(1);
      expect(mockAxios.history.put[0].url).toBe(`${API_ENDPOINTS.OWNERS}/${updateData.id}`);
    });
  });

  describe('delete should', () => {
    it('delete owner successfully', async () => {
      // Arrange
      const ownerId = '1';
      mockAxios.onDelete(`${API_ENDPOINTS.OWNERS}/${ownerId}`).reply(200, mockOwnerDtos[0]);

      // Act
      const result = await repository.delete(ownerId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.id).toEqual(ownerId);
    });

    it('return error when owner not found', async () => {
      // Arrange
      const ownerId = '999';
      mockAxios.onDelete(`${API_ENDPOINTS.OWNERS}/${ownerId}`).reply(404, {
        error: 'Owner not found',
      });

      // Act
      const result = await repository.delete(ownerId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('call correct endpoint with owner id', async () => {
      // Arrange
      const ownerId = '1';
      mockAxios.onDelete(`${API_ENDPOINTS.OWNERS}/${ownerId}`).reply(200, mockOwnerDtos[0]);

      // Act
      await repository.delete(ownerId);

      // Assert
      expect(mockAxios.history.delete.length).toBe(1);
      expect(mockAxios.history.delete[0].url).toBe(`${API_ENDPOINTS.OWNERS}/${ownerId}`);
    });
  });

  describe('error handling should', () => {
    it('handle network errors', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.OWNERS).networkError();

      // Act
      const result = await repository.getAll();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toBeDefined();
    });

    it('handle timeout errors', async () => {
      // Arrange
      mockAxios.onGet(API_ENDPOINTS.OWNERS).timeout();

      // Act
      const result = await repository.getAll();

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('transform validation errors from backend', async () => {
      // Arrange
      const newOwner = {
        name: '',
        address: '',
        birthday: 'invalid',
      };

      mockAxios.onPost(API_ENDPOINTS.OWNERS).reply(400, {
        errors: {
          Name: ['Name is required', 'Name must be at least 3 characters'],
          Address: ['Address is required'],
          Birthday: ['Invalid date format'],
        },
      });

      // Act
      const result = await repository.create(newOwner);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toContain('Name is required');
      expect(result.error?.message).toContain('Address is required');
    });
  });
});
