import { ownerRepositoryMock } from '@core/owner/__mock__/ownerMock.repository';
import {
  mockOwners,
  mockCreateOwnerRequest,
  mockCreatedOwner,
  mockUpdateOwnerRequest,
  mockUpdatedOwner,
} from '@core/owner/__mock__/data';
import { type OwnerRepository } from '@core/owner/domain/ownerRepository';
import { OwnerService } from '../ownerService';

describe('Owner Service', () => {
  let service: OwnerRepository;

  beforeEach(() => {
    service = OwnerService(ownerRepositoryMock);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('getAll should', () => {
    it('return all owners', async () => {
      const response = await service.getAll();
      
      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.length).toEqual(mockOwners.length);
      
      response.data?.forEach((owner: any, index: number) => {
        expect(owner).toEqual(
          expect.objectContaining({
            id: mockOwners[index].id,
            name: mockOwners[index].name,
            address: mockOwners[index].address,
            birthday: mockOwners[index].birthday,
          })
        );
      });
    });

    it('call repository getAll method', async () => {
      await service.getAll();
      expect(ownerRepositoryMock.getAll).toHaveBeenCalledTimes(1);
    });
  });

  describe('getById should', () => {
    it('return an owner when valid id is provided', async () => {
      const response = await service.getById('1');
      
      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual('1');
      expect(response.data?.name).toEqual(mockOwners[0].name);
    });

    it('return error when id is not provided', async () => {
      const response = await service.getById('');
      
      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_ID_REQUIRED');
      expect(response.error?.message).toEqual('Owner ID is required');
    });

    it('not call repository when id is not provided', async () => {
      await service.getById('');
      expect(ownerRepositoryMock.getById).not.toHaveBeenCalled();
    });

    it('call repository getById with correct id', async () => {
      await service.getById('1');
      expect(ownerRepositoryMock.getById).toHaveBeenCalledWith('1');
      expect(ownerRepositoryMock.getById).toHaveBeenCalledTimes(1);
    });
  });

  describe('create should', () => {
    it('create an owner with valid data', async () => {
      const response = await service.create(mockCreateOwnerRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockCreatedOwner.id);
      expect(response.data?.name).toEqual(mockCreateOwnerRequest.name);
      expect(response.data?.address).toEqual(mockCreateOwnerRequest.address);
    });

    it('return error when name is not provided', async () => {
      const invalidRequest = { ...mockCreateOwnerRequest, name: '' };
      const response = await service.create(invalidRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_NAME_REQUIRED');
      expect(response.error?.message).toEqual('Owner name is required');
    });

    it('return error when name is only whitespace', async () => {
      const invalidRequest = { ...mockCreateOwnerRequest, name: '   ' };
      const response = await service.create(invalidRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_NAME_REQUIRED');
    });

    it('return error when address is not provided', async () => {
      const invalidRequest = { ...mockCreateOwnerRequest, address: '' };
      const response = await service.create(invalidRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_ADDRESS_REQUIRED');
      expect(response.error?.message).toEqual('Owner address is required');
    });

    it('return error when address is only whitespace', async () => {
      const invalidRequest = { ...mockCreateOwnerRequest, address: '   ' };
      const response = await service.create(invalidRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_ADDRESS_REQUIRED');
    });

    it('return error when birthday is not provided', async () => {
      const invalidRequest = { ...mockCreateOwnerRequest, birthday: '' };
      const response = await service.create(invalidRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_BIRTHDAY_REQUIRED');
      expect(response.error?.message).toEqual('Owner birthday is required');
    });

    it('not call repository when validation fails', async () => {
      const invalidRequest = { ...mockCreateOwnerRequest, name: '' };
      await service.create(invalidRequest);
      expect(ownerRepositoryMock.create).not.toHaveBeenCalled();
    });

    it('call repository create with correct data', async () => {
      await service.create(mockCreateOwnerRequest);
      expect(ownerRepositoryMock.create).toHaveBeenCalledWith(mockCreateOwnerRequest);
      expect(ownerRepositoryMock.create).toHaveBeenCalledTimes(1);
    });
  });

  describe('update should', () => {
    it('update an owner with valid data', async () => {
      const response = await service.update(mockUpdateOwnerRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockUpdateOwnerRequest.id);
      expect(response.data?.name).toEqual(mockUpdateOwnerRequest.name);
    });

    it('return error when id is not provided', async () => {
      const invalidRequest = { ...mockUpdateOwnerRequest, id: '' };
      const response = await service.update(invalidRequest);
      
      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('OWNER_ID_REQUIRED');
      expect(response.error?.message).toEqual('Owner ID is required');
    });

    it('not call repository when id is not provided', async () => {
      const invalidRequest = { ...mockUpdateOwnerRequest, id: '' };
      await service.update(invalidRequest);
      expect(ownerRepositoryMock.update).not.toHaveBeenCalled();
    });

    it('call repository update with correct data', async () => {
      await service.update(mockUpdateOwnerRequest);
      expect(ownerRepositoryMock.update).toHaveBeenCalledWith(mockUpdateOwnerRequest);
      expect(ownerRepositoryMock.update).toHaveBeenCalledTimes(1);
    });
  });

  describe('delete should', () => {
    it('delete an owner with valid id', async () => {
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
      expect(response.error?.code).toEqual('OWNER_ID_REQUIRED');
      expect(response.error?.message).toEqual('Owner ID is required');
    });

    it('not call repository when id is not provided', async () => {
      await service.delete('');
      expect(ownerRepositoryMock.delete).not.toHaveBeenCalled();
    });

    it('call repository delete with correct id', async () => {
      await service.delete('1');
      expect(ownerRepositoryMock.delete).toHaveBeenCalledWith('1');
      expect(ownerRepositoryMock.delete).toHaveBeenCalledTimes(1);
    });
  });
});
