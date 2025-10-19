/**
 * Custom hooks for Property operations using React Query
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PropertyService } from '@/core/property';
import { axiosPropertyRepository } from '@/core/property';
import type {
  PropertyFilters,
  CreatePropertyRequest,
  UpdatePropertyRequest,
} from '@/core/property';

// Create service instance
const propertyService = PropertyService(axiosPropertyRepository);

// Query keys
export const propertyKeys = {
  all: ['properties'] as const,
  lists: () => [...propertyKeys.all, 'list'] as const,
  list: (filters?: PropertyFilters) => [...propertyKeys.lists(), filters] as const,
  details: () => [...propertyKeys.all, 'detail'] as const,
  detail: (id: string) => [...propertyKeys.details(), id] as const,
};

/**
 * Hook to fetch all properties with optional filters
 */
export function useProperties(filters?: PropertyFilters) {
  return useQuery({
    queryKey: propertyKeys.list(filters),
    queryFn: async () => {
      const result = await propertyService.getAll(filters);
      if (result.error) throw result.error;
      return result.data;
    },
  });
}

/**
 * Hook to fetch a single property by ID
 */
export function useProperty(id: string) {
  return useQuery({
    queryKey: propertyKeys.detail(id),
    queryFn: async () => {
      const result = await propertyService.getById(id);
      if (result.error) throw result.error;
      return result.data;
    },
    enabled: !!id,
  });
}

/**
 * Hook to create a new property
 */
export function useCreateProperty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: CreatePropertyRequest) => {
      const result = await propertyService.create(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: () => {
      // Invalidate all property lists
      queryClient.invalidateQueries({ queryKey: propertyKeys.lists() });
    },
  });
}

/**
 * Hook to update an existing property
 */
export function useUpdateProperty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: UpdatePropertyRequest) => {
      const result = await propertyService.update(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (data) => {
      // Invalidate property lists and the specific property detail
      queryClient.invalidateQueries({ queryKey: propertyKeys.lists() });
      if (data?.id) {
        queryClient.invalidateQueries({ queryKey: propertyKeys.detail(data.id) });
      }
    },
  });
}

/**
 * Hook to delete a property
 */
export function useDeleteProperty() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const result = await propertyService.delete(id);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, id) => {
      // Invalidate property lists and remove the specific property from cache
      queryClient.invalidateQueries({ queryKey: propertyKeys.lists() });
      queryClient.removeQueries({ queryKey: propertyKeys.detail(id) });
    },
  });
}
