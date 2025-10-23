import { apiClient, transformError } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PropertyImageRepository } from '../domain/propertyImageRepository';
import type { CreatePropertyImageRequest } from '../domain/propertyImage';
import {
  transformPropertyImageFromDto,
  transformPropertyImagesFromDto,
  transformCreatePropertyImageToDto,
  transformUpdatePropertyImageToDto,
} from './adapter/propertyImageAdapter';
import type { PropertyImageDto } from './propertyImageDto';

export const axiosPropertyImageRepository: PropertyImageRepository = {
  async getByPropertyId(propertyId: string) {
    try {
      const { data } = await apiClient.get<PropertyImageDto[]>(
        `${API_ENDPOINTS.PROPERTIES}/${propertyId}/images`
      );

      const transformedData = transformPropertyImagesFromDto(data || []);

      return { data: transformedData, error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async create(input: CreatePropertyImageRequest) {
    try {
      const dto = transformCreatePropertyImageToDto(input);

      const { data } = await apiClient.post<PropertyImageDto>(
        `${API_ENDPOINTS.PROPERTIES}/${input.propertyId}/images`,
        dto
      );

      return { data: transformPropertyImageFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async createBulk(propertyId: string, images: CreatePropertyImageRequest[]) {
    try {
      const payload = images.map(transformCreatePropertyImageToDto);

      const { data } = await apiClient.post<PropertyImageDto[]>(
        `${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/bulk`,
        payload
      );

      const transformedData = transformPropertyImagesFromDto(data || []);

      return { data: transformedData, error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async update(input) {
    try {
      const dto = transformUpdatePropertyImageToDto(input);

      const { data } = await apiClient.put<PropertyImageDto>(
        `${API_ENDPOINTS.PROPERTIES}/${input.propertyId}/images/${input.id}`,
        dto
      );

      return { data: transformPropertyImageFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async delete(propertyId: string, id: string) {
    try {
      const { data } = await apiClient.delete<PropertyImageDto>(
        `${API_ENDPOINTS.PROPERTIES}/${propertyId}/images/${id}`
      );
      return { data: transformPropertyImageFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },
};
