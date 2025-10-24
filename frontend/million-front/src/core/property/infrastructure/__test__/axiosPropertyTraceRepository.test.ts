import MockAdapter from 'axios-mock-adapter';
import { apiClient } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PropertyTraceRepository } from '../../domain/propertyTraceRepository';
import { axiosPropertyTraceRepository } from '../axiosPropertyTraceRepository';
import {
  mockPropertyTraceDtos,
  mockCreatedPropertyTraceDto,
  mockUpdatedPropertyTraceDto,
} from '../__mock__/propertyTraceDtoData';

describe('axiosPropertyTraceRepository integration test', () => {
  let repository: PropertyTraceRepository;
  let mockAxios: MockAdapter;

  beforeEach(() => {
    repository = axiosPropertyTraceRepository;
    mockAxios = new MockAdapter(apiClient);
  });

  afterEach(() => {
    mockAxios.reset();
    jest.clearAllMocks();
  });

  describe('getByPropertyId should', () => {
    it('return all traces for a property successfully', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios
        .onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces`)
        .reply(200, mockPropertyTraceDtos);

      // Act
      const result = await repository.getByPropertyId(propertyId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.length).toEqual(mockPropertyTraceDtos.length);

      result.data?.forEach((trace, index) => {
        expect(trace.id).toEqual(mockPropertyTraceDtos[index].idPropertyTrace);
        expect(trace.propertyId).toEqual(mockPropertyTraceDtos[index].idProperty);
        expect(trace.name).toEqual(mockPropertyTraceDtos[index].name);
        expect(trace.value).toEqual(mockPropertyTraceDtos[index].value);
      });
    });

    it('return error when API call fails', async () => {
      // Arrange
      const propertyId = '1';
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces`).reply(500);

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
        .onGet(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces`)
        .reply(200, mockPropertyTraceDtos);

      // Act
      const result = await repository.getByPropertyId(propertyId);

      // Assert
      const firstTrace = result.data?.[0];
      expect(firstTrace?.id).toEqual(mockPropertyTraceDtos[0].idPropertyTrace);
      expect(firstTrace?.propertyId).toEqual(mockPropertyTraceDtos[0].idProperty);
      expect(firstTrace).not.toHaveProperty('idPropertyTrace');
      expect(firstTrace).not.toHaveProperty('idProperty');
    });
  });

  describe('create should', () => {
    it('create property trace successfully', async () => {
      // Arrange
      const newTrace = {
        propertyId: '1',
        dateSale: '2024-05-01',
        name: 'New Valuation',
        value: 3000000,
        tax: 60000,
      };

      mockAxios
        .onPost(`${API_ENDPOINTS.PROPERTIES}/${newTrace.propertyId}/traces`)
        .reply(201, mockCreatedPropertyTraceDto);

      // Act
      const result = await repository.create(newTrace);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.name).toEqual(newTrace.name);
      expect(result.data?.value).toEqual(newTrace.value);
    });

    it('return error when validation fails', async () => {
      // Arrange
      const invalidTrace = {
        propertyId: '1',
        dateSale: '',
        name: '',
        value: 0,
        tax: 0,
      };

      mockAxios.onPost(`${API_ENDPOINTS.PROPERTIES}/${invalidTrace.propertyId}/traces`).reply(400, {
        errors: {
          Name: ['Name is required'],
          DateSale: ['DateSale is required'],
        },
      });

      // Act
      const result = await repository.create(invalidTrace);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('send correct DTO format to API', async () => {
      // Arrange
      const newTrace = {
        propertyId: '1',
        dateSale: '2024-05-01',
        name: 'Test Trace',
        value: 1000000,
        tax: 20000,
      };

      mockAxios.onPost(`${API_ENDPOINTS.PROPERTIES}/${newTrace.propertyId}/traces`).reply((config) => {
        const requestData = JSON.parse(config.data || '{}');

        // Verificar transformación a formato backend
        expect(requestData).toHaveProperty('IdProperty');
        expect(requestData).toHaveProperty('dateSale');
        expect(requestData).toHaveProperty('name');
        expect(requestData).toHaveProperty('value');
        expect(requestData).toHaveProperty('tax');
        expect(requestData).not.toHaveProperty('propertyId');
        expect(requestData).not.toHaveProperty('id');

        return [201, mockCreatedPropertyTraceDto];
      });

      // Act
      await repository.create(newTrace);

      // Assert
      expect(mockAxios.history.post.length).toBe(1);
    });
  });

  describe('update should', () => {
    it('update property trace successfully', async () => {
      // Arrange
      const updateData = {
        id: '1',
        propertyId: '1',
        name: 'Updated Trace',
        value: 2800000,
      };

      mockAxios
        .onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.propertyId}/traces/${updateData.id}`)
        .reply(200, mockUpdatedPropertyTraceDto);

      // Act
      const result = await repository.update(updateData);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
      expect(result.data?.name).toEqual(mockUpdatedPropertyTraceDto.name);
    });

    it('return error when trace not found', async () => {
      // Arrange
      const updateData = {
        id: '999',
        propertyId: '1',
        name: 'Non Existent',
      };

      mockAxios
        .onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.propertyId}/traces/${updateData.id}`)
        .reply(404);

      // Act
      const result = await repository.update(updateData);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('send correct DTO format to API', async () => {
      // Arrange
      const updateData = {
        id: '1',
        propertyId: '1',
        name: 'Updated Name',
        value: 2700000,
      };

      mockAxios
        .onPut(`${API_ENDPOINTS.PROPERTIES}/${updateData.propertyId}/traces/${updateData.id}`)
        .reply((config) => {
          const requestData = JSON.parse(config.data || '{}');

          expect(requestData).toHaveProperty('id');
          if (updateData.propertyId) {
            expect(requestData).toHaveProperty('IdProperty');
            expect(requestData).not.toHaveProperty('propertyId');
          }

          return [200, mockUpdatedPropertyTraceDto];
        });

      // Act
      await repository.update(updateData);

      // Assert
      expect(mockAxios.history.put.length).toBe(1);
    });
  });

  describe('delete should', () => {
    it('delete property trace successfully', async () => {
      // Arrange
      const propertyId = '1';
      const traceId = '1';

      mockAxios
        .onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces/${traceId}`)
        .reply(200, mockPropertyTraceDtos[0]);

      // Act
      const result = await repository.delete(propertyId, traceId);

      // Assert
      expect(result).toBeDefined();
      expect(result.data).toBeDefined();
      expect(result.error).toBeNull();
    });

    it('return error when trace not found', async () => {
      // Arrange
      const propertyId = '1';
      const traceId = '999';

      mockAxios
        .onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces/${traceId}`)
        .reply(404);

      // Act
      const result = await repository.delete(propertyId, traceId);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('call correct endpoint', async () => {
      // Arrange
      const propertyId = '1';
      const traceId = '1';

      mockAxios
        .onDelete(`${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces/${traceId}`)
        .reply(200, mockPropertyTraceDtos[0]);

      // Act
      await repository.delete(propertyId, traceId);

      // Assert
      expect(mockAxios.history.delete.length).toBe(1);
      expect(mockAxios.history.delete[0].url).toBe(
        `${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces/${traceId}`
      );
    });
  });

  describe('error handling should', () => {
    it('handle network errors', async () => {
      // Arrange
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/1/traces`).networkError();

      // Act
      const result = await repository.getByPropertyId('1');

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('handle timeout errors', async () => {
      // Arrange
      mockAxios.onGet(`${API_ENDPOINTS.PROPERTIES}/1/traces`).timeout();

      // Act
      const result = await repository.getByPropertyId('1');

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
    });

    it('transform validation errors from backend', async () => {
      // Arrange
      const invalidTrace = {
        propertyId: '1',
        dateSale: '',
        name: '',
        value: -100,
        tax: -50,
      };

      mockAxios.onPost(`${API_ENDPOINTS.PROPERTIES}/${invalidTrace.propertyId}/traces`).reply(400, {
        errors: {
          Name: ['Name is required'],
          Value: ['Value must be greater than 0'],
          Tax: ['Tax cannot be negative'],
        },
      });

      // Act
      const result = await repository.create(invalidTrace);

      // Assert
      expect(result.data).toBeNull();
      expect(result.error).toBeDefined();
      expect(result.error?.message).toContain('Name is required');
    });
  });
});
