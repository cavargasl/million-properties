import type { PropertyImageRepository } from '../domain/propertyImageRepository';

export const PropertyImageService = (repository: PropertyImageRepository): PropertyImageRepository => ({
  async getByPropertyId(propertyId) {
    if (!propertyId) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }
    return await repository.getByPropertyId(propertyId);
  },

  async create(input) {
    // Validaciones de negocio
    if (!input.propertyId) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }

    if (!input.file || input.file.trim() === '') {
      return {
        data: null,
        error: {
          message: 'Image file URL is required',
          code: 'IMAGE_FILE_REQUIRED',
        },
      };
    }

    return await repository.create(input);
  },

  async createBulk(propertyId, images) {
    if (!propertyId) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }

    if (!images || images.length === 0) {
      return {
        data: null,
        error: {
          message: 'At least one image is required',
          code: 'IMAGES_REQUIRED',
        },
      };
    }

    // Validar que todas las imágenes tengan file
    const invalidImages = images.filter(img => !img.file || img.file.trim() === '');
    if (invalidImages.length > 0) {
      return {
        data: null,
        error: {
          message: 'All images must have a valid file URL',
          code: 'INVALID_IMAGE_FILE',
        },
      };
    }

    return await repository.createBulk(propertyId, images);
  },

  async update(input) {
    if (!input.id) {
      return {
        data: null,
        error: {
          message: 'Property Image ID is required',
          code: 'PROPERTY_IMAGE_ID_REQUIRED',
        },
      };
    }

    if (!input.propertyId) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }

    return await repository.update(input);
  },

  async delete(propertyId, id) {
    if (!propertyId) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }

    if (!id) {
      return {
        data: null,
        error: {
          message: 'Property Image ID is required',
          code: 'PROPERTY_IMAGE_ID_REQUIRED',
        },
      };
    }

    return await repository.delete(propertyId, id);
  },
});
