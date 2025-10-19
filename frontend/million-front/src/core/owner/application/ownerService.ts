import type { OwnerRepository } from '../domain/ownerRepository';

export const OwnerService = (repository: OwnerRepository): OwnerRepository => ({
  async getAll() {
    return await repository.getAll();
  },

  async getById(id) {
    if (!id) {
      return {
        data: null,
        error: {
          message: 'Owner ID is required',
          code: 'OWNER_ID_REQUIRED',
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
          message: 'Owner name is required',
          code: 'OWNER_NAME_REQUIRED',
        },
      };
    }

    if (!input.address || input.address.trim() === '') {
      return {
        data: null,
        error: {
          message: 'Owner address is required',
          code: 'OWNER_ADDRESS_REQUIRED',
        },
      };
    }

    if (!input.birthday) {
      return {
        data: null,
        error: {
          message: 'Owner birthday is required',
          code: 'OWNER_BIRTHDAY_REQUIRED',
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
          message: 'Owner ID is required',
          code: 'OWNER_ID_REQUIRED',
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
          message: 'Owner ID is required',
          code: 'OWNER_ID_REQUIRED',
        },
      };
    }
    return await repository.delete(id);
  },
});
