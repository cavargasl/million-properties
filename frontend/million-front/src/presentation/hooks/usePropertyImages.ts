/**
 * Custom hooks for Property Image operations using React Query
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { axiosPropertyImageRepository } from '@/core/property';

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
      const result = await axiosPropertyImageRepository.getByPropertyId(propertyId);
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
    mutationFn: async ({ propertyId, fileUrl, enabled = true }: { propertyId: string; fileUrl: string; enabled?: boolean }) => {
      const result = await axiosPropertyImageRepository.create(propertyId, fileUrl, enabled);
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
      images: Array<{ fileUrl: string; enabled?: boolean }> 
    }) => {
      const result = await axiosPropertyImageRepository.createBulk(propertyId, images);
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
      const result = await axiosPropertyImageRepository.delete(propertyId, imageId);
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
 * Hook to toggle enabled/disabled state of a property image
 */
export function useTogglePropertyImageEnabled() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      propertyId,
      imageId,
      enabled,
    }: {
      propertyId: string;
      imageId: string;
      enabled: boolean;
    }) => {
      const result = await axiosPropertyImageRepository.toggleEnabled(propertyId, imageId, enabled);
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
