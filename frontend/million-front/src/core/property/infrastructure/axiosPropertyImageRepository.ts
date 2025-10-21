import { apiClient, transformError } from '@/api/apiClient';
import { API_ENDPOINTS } from '@/shared/constants/api';
import type { PropertyImageRepository } from '../domain/propertyRepository';
import { transformPropertyImageFromDto } from './adapter/propertyAdapter';
import type { PropertyImageDto } from './propertyDto';

export const axiosPropertyImageRepository: PropertyImageRepository = {
  async getByPropertyId(propertyId: string) {
    try {
      const { data } = await apiClient.get<PropertyImageDto[]>(
        `${API_ENDPOINTS.PROPERTY_IMAGES}/property/${propertyId}`
      );

      const transformedData =
        data
          ?.map(transformPropertyImageFromDto)
          .filter((item): item is NonNullable<typeof item> => item !== null) || [];

      return { data: transformedData, error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async create(propertyId: string, file: File) {
    try {
      const formData = new FormData();
      formData.append('file', file);
      formData.append('IdProperty', propertyId);

      const { data } = await apiClient.post<PropertyImageDto>(
        API_ENDPOINTS.PROPERTY_IMAGES,
        formData,
        {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        }
      );

      return { data: transformPropertyImageFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },

  async delete(id: string) {
    try {
      const { data } = await apiClient.delete<PropertyImageDto>(
        `${API_ENDPOINTS.PROPERTY_IMAGES}/${id}`
      );
      return { data: transformPropertyImageFromDto(data), error: null };
    } catch (error) {
      return { data: null, error: transformError(error) };
    }
  },
};
