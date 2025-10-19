import { apiClient, transformError } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PropertyRepository } from '../domain/propertyRepository';
import {
  transformPropertyFromDto,
  transformCreatePropertyToDto,
  transformUpdatePropertyToDto,
} from './adapter/propertyAdapter';
import type { PropertyDto } from './propertyDto';

export const axiosPropertyRepository: PropertyRepository = {
  async getAll(filters) {
    try {
      const { data } = await apiClient.get<PropertyDto[]>(API_ENDPOINTS.PROPERTIES, {
        params: filters,
      });

      const transformedData =
        data
          ?.map(transformPropertyFromDto)
          .filter((item): item is NonNullable<typeof item> => item !== null) || [];

      return { data: transformedData, error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async getById(id) {
    try {
      const { data } = await apiClient.get<PropertyDto>(`${API_ENDPOINTS.PROPERTIES}/${id}`);
      return { data: transformPropertyFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async create(input) {
    try {
      const dto = transformCreatePropertyToDto(input);
      const { data } = await apiClient.post<PropertyDto>(API_ENDPOINTS.PROPERTIES, dto);
      return { data: transformPropertyFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async update(input) {
    try {
      const dto = transformUpdatePropertyToDto(input);
      const { data } = await apiClient.put<PropertyDto>(
        `${API_ENDPOINTS.PROPERTIES}/${input.id}`,
        dto
      );
      return { data: transformPropertyFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async delete(id) {
    try {
      const { data } = await apiClient.delete<PropertyDto>(`${API_ENDPOINTS.PROPERTIES}/${id}`);
      return { data: transformPropertyFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },
};
