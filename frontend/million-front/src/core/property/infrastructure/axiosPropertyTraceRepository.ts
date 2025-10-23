import { apiClient, transformError } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PropertyTraceRepository } from '../domain/propertyTraceRepository';
import type { CreatePropertyTraceRequest, UpdatePropertyTraceRequest } from '../domain/propertyTrace';
import {
  transformPropertyTraceFromDto,
  transformPropertyTracesFromDto,
  transformCreatePropertyTraceToDto,
  transformUpdatePropertyTraceToDto,
} from './adapter/propertyTraceAdapter';
import type { PropertyTraceDto } from './propertyTraceDto';

export const axiosPropertyTraceRepository: PropertyTraceRepository = {
  async getByPropertyId(propertyId: string) {
    try {
      const { data } = await apiClient.get<PropertyTraceDto[]>(
        `${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces`
      );

      const transformedData = transformPropertyTracesFromDto(data || []);

      return { data: transformedData, error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async create(input: CreatePropertyTraceRequest) {
    try {
      const dto = transformCreatePropertyTraceToDto(input);

      const { data } = await apiClient.post<PropertyTraceDto>(
        `${API_ENDPOINTS.PROPERTIES}/${input.propertyId}/traces`,
        dto
      );

      return { data: transformPropertyTraceFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async update(input: UpdatePropertyTraceRequest) {
    try {
      const dto = transformUpdatePropertyTraceToDto(input);

      const { data } = await apiClient.put<PropertyTraceDto>(
        `${API_ENDPOINTS.PROPERTIES}/${input.propertyId}/traces/${input.id}`,
        dto
      );

      return { data: transformPropertyTraceFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async delete(propertyId: string, id: string) {
    try {
      const { data } = await apiClient.delete<PropertyTraceDto>(
        `${API_ENDPOINTS.PROPERTIES}/${propertyId}/traces/${id}`
      );
      return { data: transformPropertyTraceFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },
};
