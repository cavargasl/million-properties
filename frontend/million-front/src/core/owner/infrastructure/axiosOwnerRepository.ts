import { apiClient, transformError } from '@/infrastructure/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { OwnerRepository } from '../domain/ownerRepository';
import {
  transformOwnerFromDto,
  transformCreateOwnerToDto,
  transformUpdateOwnerToDto,
} from './adapter/ownerAdapter';
import type { OwnerDto } from './ownerDto';

export const axiosOwnerRepository: OwnerRepository = {
  async getAll() {
    try {
      const { data } = await apiClient.get<OwnerDto[]>(API_ENDPOINTS.OWNERS);

      const transformedData =
        data
          ?.map(transformOwnerFromDto)
          .filter((item): item is NonNullable<typeof item> => item !== null) || [];

      return { data: transformedData, error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async getById(id) {
    try {
      const { data } = await apiClient.get<OwnerDto>(`${API_ENDPOINTS.OWNERS}/${id}`);
      return { data: transformOwnerFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async create(input) {
    try {
      const dto = transformCreateOwnerToDto(input);
      const { data } = await apiClient.post<OwnerDto>(API_ENDPOINTS.OWNERS, dto);
      return { data: transformOwnerFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async update(input) {
    try {
      const dto = transformUpdateOwnerToDto(input);
      const { data } = await apiClient.put<OwnerDto>(`${API_ENDPOINTS.OWNERS}/${input.id}`, dto);
      return { data: transformOwnerFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async delete(id) {
    try {
      const { data } = await apiClient.delete<OwnerDto>(`${API_ENDPOINTS.OWNERS}/${id}`);
      return { data: transformOwnerFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },
};
