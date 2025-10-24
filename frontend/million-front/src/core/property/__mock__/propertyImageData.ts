import type {
  PropertyImage,
  PropertyImageResponse,
  PropertyImagesResponse,
  CreatePropertyImageRequest,
  UpdatePropertyImageRequest,
} from '../domain/propertyImage';

// Mock data
export const mockPropertyImages: PropertyImage[] = [
  {
    id: '1',
    propertyId: '1',
    file: 'https://example.com/images/property1-main.jpg',
    enabled: true,
  },
  {
    id: '2',
    propertyId: '1',
    file: 'https://example.com/images/property1-bedroom.jpg',
    enabled: true,
  },
  {
    id: '3',
    propertyId: '1',
    file: 'https://example.com/images/property1-kitchen.jpg',
    enabled: false,
  },
  {
    id: '4',
    propertyId: '2',
    file: 'https://example.com/images/property2-main.jpg',
    enabled: true,
  },
];

export const mockCreatePropertyImageRequest: CreatePropertyImageRequest = {
  propertyId: '1',
  file: 'https://example.com/images/new-image.jpg',
  enabled: true,
};

export const mockCreatedPropertyImage: PropertyImage = {
  id: '5',
  ...mockCreatePropertyImageRequest,
};

export const mockBulkCreateImages: CreatePropertyImageRequest[] = [
  {
    propertyId: '3',
    file: 'https://example.com/images/bulk1.jpg',
    enabled: true,
  },
  {
    propertyId: '3',
    file: 'https://example.com/images/bulk2.jpg',
    enabled: true,
  },
];

export const mockBulkCreatedImages: PropertyImage[] = [
  {
    id: '6',
    ...mockBulkCreateImages[0],
  },
  {
    id: '7',
    ...mockBulkCreateImages[1],
  },
];

export const mockUpdatePropertyImageRequest: UpdatePropertyImageRequest = {
  id: '1',
  propertyId: '1',
  enabled: false,
};

export const mockUpdatedPropertyImage: PropertyImage = {
  ...mockPropertyImages[0],
  enabled: false,
};

// Mock responses
export const mockPropertyImagesResponse: PropertyImagesResponse = {
  data: mockPropertyImages.filter(img => img.propertyId === '1'),
  error: null,
};

export const mockPropertyImageResponse: PropertyImageResponse = {
  data: mockPropertyImages[0],
  error: null,
};

export const mockPropertyImageCreatedResponse: PropertyImageResponse = {
  data: mockCreatedPropertyImage,
  error: null,
};

export const mockBulkPropertyImagesCreatedResponse: PropertyImagesResponse = {
  data: mockBulkCreatedImages,
  error: null,
};

export const mockPropertyImageUpdatedResponse: PropertyImageResponse = {
  data: mockUpdatedPropertyImage,
  error: null,
};

export const mockPropertyImageDeletedResponse: PropertyImageResponse = {
  data: mockPropertyImages[0],
  error: null,
};

export const mockPropertyImageToggledResponse: PropertyImageResponse = {
  data: mockUpdatedPropertyImage,
  error: null,
};
