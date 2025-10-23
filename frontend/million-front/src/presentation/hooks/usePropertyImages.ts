/**
 * Custom hooks for Property Image operations using React Query
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PropertyImageService } from '@/core/property/application/propertyImageService';
import { axiosPropertyImageRepository } from '@/core/property/infrastructure/axiosPropertyImageRepository';
import type { CreatePropertyImageRequest, UpdatePropertyImageRequest } from '@/core/property/domain/propertyImage';

// Create service instance
const propertyImageService = PropertyImageService(axiosPropertyImageRepository);

// Query keys
export const propertyImageKeys = {
  all: ['property-images'] as const,
  byProperty: (propertyId: string) => [...propertyImageKeys.all, 'property', propertyId] as const,
};

/**
 * Hook to fetch images for a specific property
 */
export function usePropertyImages(propertyId: string) {
  return useQuery({
    queryKey: propertyImageKeys.byProperty(propertyId),
    queryFn: async () => {
      const result = await propertyImageService.getByPropertyId(propertyId);
      if (result.error) throw result.error;
      return result.data;
    },
    enabled: !!propertyId,
  });
}

/**
 * Hook to create/upload a new property image
 */
export function useCreatePropertyImage() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: CreatePropertyImageRequest) => {
      const result = await propertyImageService.create(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property images for the specific property
      queryClient.invalidateQueries({
        queryKey: propertyImageKeys.byProperty(variables.propertyId),
      });
    },
  });
}

/**
 * Hook to create/upload multiple property images in bulk
 */
export function useCreatePropertyImagesBulk() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ 
      propertyId, 
      images 
    }: { 
      propertyId: string; 
      images: CreatePropertyImageRequest[]
    }) => {
      const result = await propertyImageService.createBulk(propertyId, images);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property images for the specific property
      queryClient.invalidateQueries({
        queryKey: propertyImageKeys.byProperty(variables.propertyId),
      });
    },
  });
}

/**
 * Hook to update a property image
 */
export function useUpdatePropertyImage() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: UpdatePropertyImageRequest) => {
      const result = await propertyImageService.update(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property images for the specific property
      queryClient.invalidateQueries({
        queryKey: propertyImageKeys.byProperty(variables.propertyId),
      });
    },
  });
}

/**
 * Hook to delete a property image
 */
export function useDeletePropertyImage() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({propertyId, imageId}: {propertyId: string; imageId: string}) => {
      const result = await propertyImageService.delete(propertyId, imageId);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property images for the specific property
      queryClient.invalidateQueries({
        queryKey: propertyImageKeys.byProperty(variables.propertyId),
      });
    },
  });
}
/**
 * Hook to change the enabled status of a property image
 */

export function useTogglePropertyImageEnabled() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({propertyId, imageId, enabled}: {propertyId: string; imageId: string; enabled: boolean}) => {
      const result = await propertyImageService.toggleEnabled(propertyId, imageId, enabled);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property images for the specific property
      queryClient.invalidateQueries({
        queryKey: propertyImageKeys.byProperty(variables.propertyId),
      });
    },
  });
}
