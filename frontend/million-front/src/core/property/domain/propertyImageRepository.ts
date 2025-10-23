import type {
  CreatePropertyImageRequest,
  UpdatePropertyImageRequest,
  PropertyImageResponse,
  PropertyImagesResponse,
} from './propertyImage';

export interface PropertyImageRepository {
  getByPropertyId(propertyId: string): Promise<PropertyImagesResponse>;
  create(input: CreatePropertyImageRequest): Promise<PropertyImageResponse>;
  createBulk(propertyId: string, images: CreatePropertyImageRequest[]): Promise<PropertyImagesResponse>;
  update(input: UpdatePropertyImageRequest): Promise<PropertyImageResponse>;
  delete(propertyId: string, id: string): Promise<PropertyImageResponse>;
}
