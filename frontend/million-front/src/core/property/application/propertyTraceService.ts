import type { PropertyTraceRepository } from '../domain/propertyTraceRepository';

export const PropertyTraceService = (repository: PropertyTraceRepository): PropertyTraceRepository => ({
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

    if (!input.name || input.name.trim() === '') {
      return {
        data: null,
        error: {
          message: 'Trace name is required',
          code: 'TRACE_NAME_REQUIRED',
        },
      };
    }

    if (!input.dateSale || input.dateSale.trim() === '') {
      return {
        data: null,
        error: {
          message: 'Sale date is required',
          code: 'SALE_DATE_REQUIRED',
        },
      };
    }

    if (input.value <= 0) {
      return {
        data: null,
        error: {
          message: 'Trace value must be greater than 0',
          code: 'INVALID_TRACE_VALUE',
        },
      };
    }

    if (input.tax < 0) {
      return {
        data: null,
        error: {
          message: 'Tax cannot be negative',
          code: 'INVALID_TAX_VALUE',
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
          message: 'Property Trace ID is required',
          code: 'PROPERTY_TRACE_ID_REQUIRED',
        },
      };
    }

    if (input.value !== undefined && input.value <= 0) {
      return {
        data: null,
        error: {
          message: 'Trace value must be greater than 0',
          code: 'INVALID_TRACE_VALUE',
        },
      };
    }

    if (input.tax !== undefined && input.tax < 0) {
      return {
        data: null,
        error: {
          message: 'Tax cannot be negative',
          code: 'INVALID_TAX_VALUE',
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
          message: 'Property Trace ID is required',
          code: 'PROPERTY_TRACE_ID_REQUIRED',
        },
      };
    }

    return await repository.delete(propertyId, id);
  },
});
