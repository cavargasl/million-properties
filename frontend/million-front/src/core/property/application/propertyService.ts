import type { PropertyRepository } from '../domain/propertyRepository';

export const PropertyService = (repository: PropertyRepository): PropertyRepository => ({
  async getAll(filters) {
    return await repository.getAll(filters);
  },

  async getAllPaginated(filters, pagination) {
    return await repository.getAllPaginated(filters, pagination);
  },

  async getById(id) {
    if (!id) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }
    return await repository.getById(id);
  },

  async create(input) {
    // Validaciones de negocio
    if (!input.name || input.name.trim() === '') {
      return {
        data: null,
        error: {
          message: 'Property name is required',
          code: 'PROPERTY_NAME_REQUIRED',
        },
      };
    }

    if (!input.address || input.address.trim() === '') {
      return {
        data: null,
        error: {
          message: 'Property address is required',
          code: 'PROPERTY_ADDRESS_REQUIRED',
        },
      };
    }

    if (input.price <= 0) {
      return {
        data: null,
        error: {
          message: 'Property price must be greater than 0',
          code: 'INVALID_PROPERTY_PRICE',
        },
      };
    }

    if (!input.ownerId) {
      return {
        data: null,
        error: {
          message: 'Owner ID is required',
          code: 'OWNER_ID_REQUIRED',
        },
      };
    }

    return await repository.create(input);
  },

  async update(input) {
    if (!input.id) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }

    if (input.price !== undefined && input.price <= 0) {
      return {
        data: null,
        error: {
          message: 'Property price must be greater than 0',
          code: 'INVALID_PROPERTY_PRICE',
        },
      };
    }

    return await repository.update(input);
  },

  async delete(id) {
    if (!id) {
      return {
        data: null,
        error: {
          message: 'Property ID is required',
          code: 'PROPERTY_ID_REQUIRED',
        },
      };
    }
    return await repository.delete(id);
  },
});
