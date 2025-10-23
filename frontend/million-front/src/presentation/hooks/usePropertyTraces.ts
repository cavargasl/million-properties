/**
 * Custom hooks for Property Trace operations using React Query
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { PropertyTraceService } from '@/core/property/application/propertyTraceService';
import { axiosPropertyTraceRepository } from '@/core/property/infrastructure/axiosPropertyTraceRepository';
import type { CreatePropertyTraceRequest, UpdatePropertyTraceRequest } from '@/core/property/domain/propertyTrace';

// Create service instance
const propertyTraceService = PropertyTraceService(axiosPropertyTraceRepository);

// Query keys
export const propertyTraceKeys = {
  all: ['property-traces'] as const,
  byProperty: (propertyId: string) => [...propertyTraceKeys.all, 'property', propertyId] as const,
};

/**
 * Hook to fetch traces for a specific property
 */
export function usePropertyTraces(propertyId: string) {
  return useQuery({
    queryKey: propertyTraceKeys.byProperty(propertyId),
    queryFn: async () => {
      const result = await propertyTraceService.getByPropertyId(propertyId);
      if (result.error) throw result.error;
      return result.data;
    },
    enabled: !!propertyId,
  });
}

/**
 * Hook to create a new property trace
 */
export function useCreatePropertyTrace() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: CreatePropertyTraceRequest) => {
      const result = await propertyTraceService.create(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property traces for the specific property
      queryClient.invalidateQueries({
        queryKey: propertyTraceKeys.byProperty(variables.propertyId),
      });
    },
  });
}

/**
 * Hook to update an existing property trace
 */
export function useUpdatePropertyTrace() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: UpdatePropertyTraceRequest) => {
      const result = await propertyTraceService.update(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property traces for the specific property
      if (variables.propertyId) {
        queryClient.invalidateQueries({
          queryKey: propertyTraceKeys.byProperty(variables.propertyId),
        });
      }
    },
  });
}

/**
 * Hook to delete a property trace
 */
export function useDeletePropertyTrace() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({propertyId, traceId}: {propertyId: string; traceId: string}) => {
      const result = await propertyTraceService.delete(propertyId, traceId);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, variables) => {
      // Invalidate property traces for the specific property
      queryClient.invalidateQueries({
        queryKey: propertyTraceKeys.byProperty(variables.propertyId),
      });
    },
  });
}
