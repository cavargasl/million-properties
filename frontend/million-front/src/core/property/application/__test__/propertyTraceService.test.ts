import { propertyTraceRepositoryMock } from '@core/property/__mock__/propertyTraceMock.repository';
import {
  mockPropertyTraces,
  mockCreatePropertyTraceRequest,
  mockCreatedPropertyTrace,
  mockUpdatePropertyTraceRequest,
  mockUpdatedPropertyTrace,
} from '@core/property/__mock__/propertyTraceData';
import { type PropertyTraceRepository } from '@core/property/domain/propertyTraceRepository';
import { PropertyTraceService } from '../propertyTraceService';

describe('PropertyTrace Service', () => {
  let service: PropertyTraceRepository;

  beforeEach(() => {
    service = PropertyTraceService(propertyTraceRepositoryMock);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('getByPropertyId should', () => {
    it('return all traces for a property', async () => {
      const propertyId = '1';
      const response = await service.getByPropertyId(propertyId);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.length).toBeGreaterThan(0);
      expect(response.data?.every(trace => trace.propertyId === propertyId)).toBe(true);
    });

    it('return error when propertyId is not provided', async () => {
      const response = await service.getByPropertyId('');

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property ID is required');
    });

    it('not call repository when propertyId is not provided', async () => {
      await service.getByPropertyId('');
      expect(propertyTraceRepositoryMock.getByPropertyId).not.toHaveBeenCalled();
    });

    it('call repository getByPropertyId with correct id', async () => {
      await service.getByPropertyId('1');
      expect(propertyTraceRepositoryMock.getByPropertyId).toHaveBeenCalledWith('1');
      expect(propertyTraceRepositoryMock.getByPropertyId).toHaveBeenCalledTimes(1);
    });
  });

  describe('create should', () => {
    it('create a property trace with valid data', async () => {
      const response = await service.create(mockCreatePropertyTraceRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockCreatedPropertyTrace.id);
      expect(response.data?.propertyId).toEqual(mockCreatePropertyTraceRequest.propertyId);
      expect(response.data?.name).toEqual(mockCreatePropertyTraceRequest.name);
      expect(response.data?.value).toEqual(mockCreatePropertyTraceRequest.value);
      expect(response.data?.tax).toEqual(mockCreatePropertyTraceRequest.tax);
    });

    it('return error when propertyId is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, propertyId: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property ID is required');
    });

    it('return error when name is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, name: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('TRACE_NAME_REQUIRED');
      expect(response.error?.message).toEqual('Trace name is required');
    });

    it('return error when name is only whitespace', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, name: '   ' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('TRACE_NAME_REQUIRED');
    });

    it('return error when dateSale is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, dateSale: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('SALE_DATE_REQUIRED');
      expect(response.error?.message).toEqual('Sale date is required');
    });

    it('return error when dateSale is only whitespace', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, dateSale: '   ' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('SALE_DATE_REQUIRED');
    });

    it('return error when value is 0', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, value: 0 };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_TRACE_VALUE');
      expect(response.error?.message).toEqual('Trace value must be greater than 0');
    });

    it('return error when value is negative', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, value: -1000 };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_TRACE_VALUE');
    });

    it('return error when tax is negative', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, tax: -100 };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_TAX_VALUE');
      expect(response.error?.message).toEqual('Tax cannot be negative');
    });

    it('allow tax to be 0', async () => {
      const validRequest = { ...mockCreatePropertyTraceRequest, tax: 0 };
      await service.create(validRequest);

      expect(propertyTraceRepositoryMock.create).toHaveBeenCalledWith(validRequest);
    });

    it('not call repository when validation fails', async () => {
      const invalidRequest = { ...mockCreatePropertyTraceRequest, name: '' };
      await service.create(invalidRequest);
      expect(propertyTraceRepositoryMock.create).not.toHaveBeenCalled();
    });

    it('call repository create with correct data', async () => {
      await service.create(mockCreatePropertyTraceRequest);
      expect(propertyTraceRepositoryMock.create).toHaveBeenCalledWith(mockCreatePropertyTraceRequest);
      expect(propertyTraceRepositoryMock.create).toHaveBeenCalledTimes(1);
    });
  });

  describe('update should', () => {
    it('update a property trace with valid data', async () => {
      const response = await service.update(mockUpdatePropertyTraceRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockUpdatePropertyTraceRequest.id);
      expect(response.data?.name).toEqual(mockUpdatePropertyTraceRequest.name);
      expect(response.data?.value).toEqual(mockUpdatePropertyTraceRequest.value);
    });

    it('return error when id is not provided', async () => {
      const invalidRequest = { ...mockUpdatePropertyTraceRequest, id: '' };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_TRACE_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property Trace ID is required');
    });

    it('return error when value is 0', async () => {
      const invalidRequest = { ...mockUpdatePropertyTraceRequest, value: 0 };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_TRACE_VALUE');
    });

    it('return error when value is negative', async () => {
      const invalidRequest = { ...mockUpdatePropertyTraceRequest, value: -500 };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_TRACE_VALUE');
    });

    it('return error when tax is negative', async () => {
      const invalidRequest = { ...mockUpdatePropertyTraceRequest, tax: -50 };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_TAX_VALUE');
    });

    it('allow update without value validation if value is undefined', async () => {
      const validRequest = { id: '1', name: 'Updated Name' };
      await service.update(validRequest);

      expect(propertyTraceRepositoryMock.update).toHaveBeenCalledWith(validRequest);
    });

    it('allow update without tax validation if tax is undefined', async () => {
      const validRequest = { id: '1', value: 3000000 };
      await service.update(validRequest);

      expect(propertyTraceRepositoryMock.update).toHaveBeenCalledWith(validRequest);
    });

    it('allow tax to be 0 when updating', async () => {
      const validRequest = { ...mockUpdatePropertyTraceRequest, tax: 0 };
      await service.update(validRequest);

      expect(propertyTraceRepositoryMock.update).toHaveBeenCalledWith(validRequest);
    });

    it('not call repository when id is not provided', async () => {
      const invalidRequest = { ...mockUpdatePropertyTraceRequest, id: '' };
      await service.update(invalidRequest);
      expect(propertyTraceRepositoryMock.update).not.toHaveBeenCalled();
    });

    it('not call repository when value validation fails', async () => {
      const invalidRequest = { ...mockUpdatePropertyTraceRequest, value: -100 };
      await service.update(invalidRequest);
      expect(propertyTraceRepositoryMock.update).not.toHaveBeenCalled();
    });

    it('call repository update with correct data', async () => {
      await service.update(mockUpdatePropertyTraceRequest);
      expect(propertyTraceRepositoryMock.update).toHaveBeenCalledWith(mockUpdatePropertyTraceRequest);
      expect(propertyTraceRepositoryMock.update).toHaveBeenCalledTimes(1);
    });
  });

  describe('delete should', () => {
    it('delete a property trace with valid ids', async () => {
      const response = await service.delete('1', '1');

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
    });

    it('return error when propertyId is not provided', async () => {
      const response = await service.delete('', '1');

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
    });

    it('return error when id is not provided', async () => {
      const response = await service.delete('1', '');

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_TRACE_ID_REQUIRED');
    });

    it('not call repository when propertyId is not provided', async () => {
      await service.delete('', '1');
      expect(propertyTraceRepositoryMock.delete).not.toHaveBeenCalled();
    });

    it('not call repository when id is not provided', async () => {
      await service.delete('1', '');
      expect(propertyTraceRepositoryMock.delete).not.toHaveBeenCalled();
    });

    it('call repository delete with correct ids', async () => {
      await service.delete('1', '1');
      expect(propertyTraceRepositoryMock.delete).toHaveBeenCalledWith('1', '1');
      expect(propertyTraceRepositoryMock.delete).toHaveBeenCalledTimes(1);
    });
  });
});
