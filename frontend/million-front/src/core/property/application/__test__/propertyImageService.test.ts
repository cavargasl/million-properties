import { propertyImageRepositoryMock } from '@core/property/__mock__/propertyImageMock.repository';
import {
  mockPropertyImages,
  mockCreatePropertyImageRequest,
  mockCreatedPropertyImage,
  mockBulkCreateImages,
  mockBulkCreatedImages,
  mockUpdatePropertyImageRequest,
  mockUpdatedPropertyImage,
} from '@core/property/__mock__/propertyImageData';
import { type PropertyImageRepository } from '@core/property/domain/propertyImageRepository';
import { PropertyImageService } from '../propertyImageService';

describe('PropertyImage Service', () => {
  let service: PropertyImageRepository;

  beforeEach(() => {
    service = PropertyImageService(propertyImageRepositoryMock);
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  describe('getByPropertyId should', () => {
    it('return all images for a property', async () => {
      const propertyId = '1';
      const response = await service.getByPropertyId(propertyId);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.length).toBeGreaterThan(0);
      expect(response.data?.every(img => img.propertyId === propertyId)).toBe(true);
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
      expect(propertyImageRepositoryMock.getByPropertyId).not.toHaveBeenCalled();
    });

    it('call repository getByPropertyId with correct id', async () => {
      await service.getByPropertyId('1');
      expect(propertyImageRepositoryMock.getByPropertyId).toHaveBeenCalledWith('1');
      expect(propertyImageRepositoryMock.getByPropertyId).toHaveBeenCalledTimes(1);
    });
  });

  describe('create should', () => {
    it('create a property image with valid data', async () => {
      const response = await service.create(mockCreatePropertyImageRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockCreatedPropertyImage.id);
      expect(response.data?.propertyId).toEqual(mockCreatePropertyImageRequest.propertyId);
      expect(response.data?.file).toEqual(mockCreatePropertyImageRequest.file);
    });

    it('return error when propertyId is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyImageRequest, propertyId: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property ID is required');
    });

    it('return error when file is not provided', async () => {
      const invalidRequest = { ...mockCreatePropertyImageRequest, file: '' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('IMAGE_FILE_REQUIRED');
      expect(response.error?.message).toEqual('Image file URL is required');
    });

    it('return error when file is only whitespace', async () => {
      const invalidRequest = { ...mockCreatePropertyImageRequest, file: '   ' };
      const response = await service.create(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('IMAGE_FILE_REQUIRED');
    });

    it('not call repository when validation fails', async () => {
      const invalidRequest = { ...mockCreatePropertyImageRequest, file: '' };
      await service.create(invalidRequest);
      expect(propertyImageRepositoryMock.create).not.toHaveBeenCalled();
    });

    it('call repository create with correct data', async () => {
      await service.create(mockCreatePropertyImageRequest);
      expect(propertyImageRepositoryMock.create).toHaveBeenCalledWith(mockCreatePropertyImageRequest);
      expect(propertyImageRepositoryMock.create).toHaveBeenCalledTimes(1);
    });
  });

  describe('createBulk should', () => {
    it('create multiple images with valid data', async () => {
      const propertyId = '3';
      const response = await service.createBulk(propertyId, mockBulkCreateImages);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.length).toEqual(mockBulkCreatedImages.length);
    });

    it('return error when propertyId is not provided', async () => {
      const response = await service.createBulk('', mockBulkCreateImages);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
    });

    it('return error when images array is empty', async () => {
      const response = await service.createBulk('1', []);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('IMAGES_REQUIRED');
      expect(response.error?.message).toEqual('At least one image is required');
    });

    it('return error when images array is not provided', async () => {
      const response = await service.createBulk('1', null as any);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('IMAGES_REQUIRED');
    });

    it('return error when any image has invalid file', async () => {
      const invalidImages = [
        { propertyId: '1', file: 'valid.jpg', enabled: true },
        { propertyId: '1', file: '', enabled: true },
      ];
      const response = await service.createBulk('1', invalidImages);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('INVALID_IMAGE_FILE');
      expect(response.error?.message).toEqual('All images must have a valid file URL');
    });

    it('not call repository when validation fails', async () => {
      await service.createBulk('1', []);
      expect(propertyImageRepositoryMock.createBulk).not.toHaveBeenCalled();
    });

    it('call repository createBulk with correct data', async () => {
      const propertyId = '3';
      await service.createBulk(propertyId, mockBulkCreateImages);
      expect(propertyImageRepositoryMock.createBulk).toHaveBeenCalledWith(propertyId, mockBulkCreateImages);
      expect(propertyImageRepositoryMock.createBulk).toHaveBeenCalledTimes(1);
    });
  });

  describe('update should', () => {
    it('update a property image with valid data', async () => {
      const response = await service.update(mockUpdatePropertyImageRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
      expect(response.data?.id).toEqual(mockUpdatePropertyImageRequest.id);
      expect(response.data?.enabled).toEqual(mockUpdatePropertyImageRequest.enabled);
    });

    it('return error when id is not provided', async () => {
      const invalidRequest = { ...mockUpdatePropertyImageRequest, id: '' };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_IMAGE_ID_REQUIRED');
      expect(response.error?.message).toEqual('Property Image ID is required');
    });

    it('return error when propertyId is not provided', async () => {
      const invalidRequest = { ...mockUpdatePropertyImageRequest, propertyId: '' };
      const response = await service.update(invalidRequest);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
    });

    it('not call repository when id is not provided', async () => {
      const invalidRequest = { ...mockUpdatePropertyImageRequest, id: '' };
      await service.update(invalidRequest);
      expect(propertyImageRepositoryMock.update).not.toHaveBeenCalled();
    });

    it('call repository update with correct data', async () => {
      await service.update(mockUpdatePropertyImageRequest);
      expect(propertyImageRepositoryMock.update).toHaveBeenCalledWith(mockUpdatePropertyImageRequest);
      expect(propertyImageRepositoryMock.update).toHaveBeenCalledTimes(1);
    });
  });

  describe('delete should', () => {
    it('delete a property image with valid ids', async () => {
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
      expect(response.error?.code).toEqual('PROPERTY_IMAGE_ID_REQUIRED');
    });

    it('not call repository when propertyId is not provided', async () => {
      await service.delete('', '1');
      expect(propertyImageRepositoryMock.delete).not.toHaveBeenCalled();
    });

    it('not call repository when id is not provided', async () => {
      await service.delete('1', '');
      expect(propertyImageRepositoryMock.delete).not.toHaveBeenCalled();
    });

    it('call repository delete with correct ids', async () => {
      await service.delete('1', '1');
      expect(propertyImageRepositoryMock.delete).toHaveBeenCalledWith('1', '1');
      expect(propertyImageRepositoryMock.delete).toHaveBeenCalledTimes(1);
    });
  });

  describe('toggleEnabled should', () => {
    it('toggle enabled status with valid ids', async () => {
      const response = await service.toggleEnabled('1', '1', false);

      expect(response).toBeDefined();
      expect(response.data).toBeDefined();
      expect(response.error).toBeNull();
    });

    it('return error when propertyId is not provided', async () => {
      const response = await service.toggleEnabled('', '1', true);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_ID_REQUIRED');
    });

    it('return error when id is not provided', async () => {
      const response = await service.toggleEnabled('1', '', true);

      expect(response).toBeDefined();
      expect(response.data).toBeNull();
      expect(response.error).toBeDefined();
      expect(response.error?.code).toEqual('PROPERTY_IMAGE_ID_REQUIRED');
    });

    it('not call repository when propertyId is not provided', async () => {
      await service.toggleEnabled('', '1', true);
      expect(propertyImageRepositoryMock.toggleEnabled).not.toHaveBeenCalled();
    });

    it('not call repository when id is not provided', async () => {
      await service.toggleEnabled('1', '', true);
      expect(propertyImageRepositoryMock.toggleEnabled).not.toHaveBeenCalled();
    });

    it('call repository toggleEnabled with correct parameters', async () => {
      await service.toggleEnabled('1', '1', false);
      expect(propertyImageRepositoryMock.toggleEnabled).toHaveBeenCalledWith('1', '1', false);
      expect(propertyImageRepositoryMock.toggleEnabled).toHaveBeenCalledTimes(1);
    });
  });
});
