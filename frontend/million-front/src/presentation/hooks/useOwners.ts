/**
 * Custom hooks for Owner operations using React Query
 */

import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { OwnerService } from '@/core/owner';
import { axiosOwnerRepository } from '@/core/owner';
import type { CreateOwnerRequest, UpdateOwnerRequest } from '@/core/owner';

// Create service instance
const ownerService = OwnerService(axiosOwnerRepository);

// Query keys
export const ownerKeys = {
  all: ['owners'] as const,
  lists: () => [...ownerKeys.all, 'list'] as const,
  list: () => [...ownerKeys.lists()] as const,
  details: () => [...ownerKeys.all, 'detail'] as const,
  detail: (id: string) => [...ownerKeys.details(), id] as const,
};

/**
 * Hook to fetch all owners
 */
export function useOwners() {
  return useQuery({
    queryKey: ownerKeys.list(),
    queryFn: async () => {
      const result = await ownerService.getAll();
      if (result.error) throw result.error;
      return result.data;
    },
  });
}

/**
 * Hook to fetch a single owner by ID
 */
export function useOwner(id: string) {
  return useQuery({
    queryKey: ownerKeys.detail(id),
    queryFn: async () => {
      const result = await ownerService.getById(id);
      if (result.error) throw result.error;
      return result.data;
    },
    enabled: !!id,
  });
}

/**
 * Hook to create a new owner
 */
export function useCreateOwner() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: CreateOwnerRequest) => {
      const result = await ownerService.create(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: () => {
      // Invalidate owner list
      queryClient.invalidateQueries({ queryKey: ownerKeys.lists() });
    },
  });
}

/**
 * Hook to update an existing owner
 */
export function useUpdateOwner() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (input: UpdateOwnerRequest) => {
      const result = await ownerService.update(input);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (data) => {
      // Invalidate owner list and the specific owner detail
      queryClient.invalidateQueries({ queryKey: ownerKeys.lists() });
      if (data?.id) {
        queryClient.invalidateQueries({ queryKey: ownerKeys.detail(data.id) });
      }
    },
  });
}

/**
 * Hook to delete an owner
 */
export function useDeleteOwner() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const result = await ownerService.delete(id);
      if (result.error) throw result.error;
      return result.data;
    },
    onSuccess: (_, id) => {
      // Invalidate owner list and remove the specific owner from cache
      queryClient.invalidateQueries({ queryKey: ownerKeys.lists() });
      queryClient.removeQueries({ queryKey: ownerKeys.detail(id) });
    },
  });
}
