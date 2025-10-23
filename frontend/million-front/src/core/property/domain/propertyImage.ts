import type { Result, ResultArray } from '@/core/shared/domain/types';

export interface PropertyImage {
  id: string;
  propertyId: string;
  file: string;
  enabled: boolean;
}

export type CreatePropertyImageRequest = Omit<PropertyImage, 'id'>;
export type UpdatePropertyImageRequest = Partial<Omit<PropertyImage, 'id' | 'propertyId'>> & { id: string; propertyId: string };

export type PropertyImageResponse = Result<PropertyImage>;
export type PropertyImagesResponse = ResultArray<PropertyImage>;
