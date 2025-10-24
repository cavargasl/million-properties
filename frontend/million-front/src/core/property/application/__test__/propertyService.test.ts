import { propertyRepositoryMock } from '@core/property/__mock__/propertyMock.repository';
import {
  mockProperties,
  mockCreatePropertyRequest,
  mockCreatedProperty,
  mockUpdatePropertyRequest,
  mockUpdatedProperty,
} from '@core/property/__mock__/data';
import { type PropertyRepository } from '@core/property/domain/propertyRepository';
import { PropertyService } from '../propertyService';

describe('Property Service', () => {
  let service: PropertyRepository;

  beforeEach(() => {
    service = PropertyService(propertyRepositoryMock);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('getAll should', () => {
    it('return all properties', async () => {
      const response = await service.getAll();

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.length).toEqual(mockProperties.length);

      response.data?.forEach((property: any, index: number) => {
        expect(property).toEqual(
          expect.objectContaining({
            id: mockProperties[index].id,
            name: mockProperties[index].name,
            address: mockProperties[index].address,
            price: mockProperties[index].price,
          })
        );
      });
    });

    it('call repository getAll method', async () => {
      await service.getAll();
      expect(propertyRepositoryMock.getAll).toHaveBeenCalledTimes(1);
    });

    it('pass filters to repository', async () => {
      const filters = { ownerId: '1', minPrice: 100000 };
      await service.getAll(filters);
      expect(propertyRepositoryMock.getAll).toHaveBeenCalledWith(filters);
    });
  });

  describe('getAllPaginated should', () => {
    it('return paginated properties', async () => {
      const response = await service.getAllPaginated();

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.items).toBeDefined();
      expect(response.data?.pagination).toBeDefined();
      expect(response.data?.pagination.pageNumber).toEqual(1);
    });

    it('call repository getAllPaginated method', async () => {
      await service.getAllPaginated();
      expect(propertyRepositoryMock.getAllPaginated).toHaveBeenCalledTimes(1);
    });

    it('pass filters and pagination to repository', async () => {
      const filters = { name: 'Villa' };
      const pagination = { pageNumber: 2, pageSize: 10 };
      await service.getAllPaginated(filters, pagination);
      expect(propertyRepositoryMock.getAllPaginated).toHaveBeenCalledWith(filters, pagination);
    });
  });

  describe('getById should', () => {
    it('return a property when valid id is provided', async () => {
      const response = await service.getById('1');

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual('1');
      expect(response.data?.name).toEqual(mockProperties[0].name);
    });

    it('return error when id is not provided', async () => {
      const response = await service.getById('');

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property ID is required');
    });

    it('not call repository when id is not provided', async () => {
      await service.getById('');
      expect(propertyRepositoryMock.getById).not.toHaveBeenCalled();
    });

    it('call repository getById with correct id', async () => {
      await service.getById('1');
      expect(propertyRepositoryMock.getById).toHaveBeenCalledWith('1');
      expect(propertyRepositoryMock.getById).toHaveBeenCalledTimes(1);
    });
  });

  describe('create should', () => {
    it('create a property with valid data', async () => {
      const response = await service.create(mockCreatePropertyRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockCreatedProperty.id);
      expect(response.data?.name).toEqual(mockCreatePropertyRequest.name);
      expect(response.data?.address).toEqual(mockCreatePropertyRequest.address);
      expect(response.data?.price).toEqual(mockCreatePropertyRequest.price);
    });

    it('return error when name is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, name: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_NAME_REQUIRED');
      expect(response.error?.message).toEqual('Property name is required');
    });

    it('return error when name is only whitespace', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, name: '   ' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_NAME_REQUIRED');
    });

    it('return error when address is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, address: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ADDRESS_REQUIRED');
      expect(response.error?.message).toEqual('Property address is required');
    });

    it('return error when address is only whitespace', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, address: '   ' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ADDRESS_REQUIRED');
    });

    it('return error when price is 0', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, price: 0 };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_PROPERTY_PRICE');
      expect(response.error?.message).toEqual('Property price must be greater than 0');
    });

    it('return error when price is negative', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, price: -1000 };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_PROPERTY_PRICE');
    });

    it('return error when ownerId is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, ownerId: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_ID_REQUIRED');
      expect(response.error?.message).toEqual('Owner ID is required');
    });

    it('not call repository when validation fails', async () => {
      const invalidRequest = { ...mockCreatePropertyRequest, name: '' };
      await service.create(invalidRequest);
      expect(propertyRepositoryMock.create).not.toHaveBeenCalled();
    });

    it('call repository create with correct data', async () => {
      await service.create(mockCreatePropertyRequest);
      expect(propertyRepositoryMock.create).toHaveBeenCalledWith(mockCreatePropertyRequest);
      expect(propertyRepositoryMock.create).toHaveBeenCalledTimes(1);
    });
  });

  describe('update should', () => {
    it('update a property with valid data', async () => {
      const response = await service.update(mockUpdatePropertyRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockUpdatePropertyRequest.id);
      expect(response.data?.name).toEqual(mockUpdatePropertyRequest.name);
      expect(response.data?.price).toEqual(mockUpdatePropertyRequest.price);
    });

    it('return error when id is not provided', async () => {
      const invalidRequest = { ...mockUpdatePropertyRequest, id: '' };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property ID is required');
    });

    it('return error when price is 0', async () => {
      const invalidRequest = { ...mockUpdatePropertyRequest, price: 0 };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_PROPERTY_PRICE');
    });

    it('return error when price is negative', async () => {
      const invalidRequest = { ...mockUpdatePropertyRequest, price: -500 };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_PROPERTY_PRICE');
    });

    it('allow update without price validation if price is undefined', async () => {
      const validRequest = { id: '1', name: 'Updated Name' };
      await service.update(validRequest);

      expect(propertyRepositoryMock.update).toHaveBeenCalledWith(validRequest);
    });

    it('not call repository when id is not provided', async () => {
      const invalidRequest = { ...mockUpdatePropertyRequest, id: '' };
      await service.update(invalidRequest);
      expect(propertyRepositoryMock.update).not.toHaveBeenCalled();
    });

    it('not call repository when price validation fails', async () => {
      const invalidRequest = { ...mockUpdatePropertyRequest, price: -100 };
      await service.update(invalidRequest);
      expect(propertyRepositoryMock.update).not.toHaveBeenCalled();
    });

    it('call repository update with correct data', async () => {
      await service.update(mockUpdatePropertyRequest);
      expect(propertyRepositoryMock.update).toHaveBeenCalledWith(mockUpdatePropertyRequest);
      expect(propertyRepositoryMock.update).toHaveBeenCalledTimes(1);
    });
  });

  describe('delete should', () => {
    it('delete a property with valid id', async () => {
      const response = await service.delete('1');

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
    });

    it('return error when id is not provided', async () => {
      const response = await service.delete('');

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property ID is required');
    });

    it('not call repository when id is not provided', async () => {
      await service.delete('');
      expect(propertyRepositoryMock.delete).not.toHaveBeenCalled();
    });

    it('call repository delete with correct id', async () => {
      await service.delete('1');
      expect(propertyRepositoryMock.delete).toHaveBeenCalledWith('1');
      expect(propertyRepositoryMock.delete).toHaveBeenCalledTimes(1);
    });
  });
});
