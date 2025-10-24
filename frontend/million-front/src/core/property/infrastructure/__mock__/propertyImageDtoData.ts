import type { PropertyImageDto, CreatePropertyImageDto, UpdatePropertyImageDto } from '../propertyImageDto';

// Mock DTOs que vienen del backend
export const mockPropertyImageDtos: PropertyImageDto[] = [
  {
    idPropertyImage: '1',
    idProperty: '1',
    file: 'https://example.com/images/property1-main.jpg',
    enabled: true,
  },
  {
    idPropertyImage: '2',
    idProperty: '1',
    file: 'https://example.com/images/property1-bedroom.jpg',
    enabled: true,
  },
  {
    idPropertyImage: '3',
    idProperty: '1',
    file: 'https://example.com/images/property1-kitchen.jpg',
    enabled: false,
  },
];

export const mockCreatePropertyImageDto: CreatePropertyImageDto = {
  IdProperty: '1',
  File: 'https://example.com/images/new-image.jpg',
  Enabled: true,
};

export const mockCreatedPropertyImageDto: PropertyImageDto = {
  idPropertyImage: '4',
  idProperty: '1',
  file: 'https://example.com/images/new-image.jpg',
  enabled: true,
};

export const mockBulkCreatePropertyImageDtos: CreatePropertyImageDto[] = [
  {
    IdProperty: '2',
    File: 'https://example.com/images/bulk1.jpg',
    Enabled: true,
  },
  {
    IdProperty: '2',
    File: 'https://example.com/images/bulk2.jpg',
    Enabled: true,
  },
];

export const mockBulkCreatedPropertyImageDtos: PropertyImageDto[] = [
  {
    idPropertyImage: '5',
    idProperty: '2',
    file: 'https://example.com/images/bulk1.jpg',
    enabled: true,
  },
  {
    idPropertyImage: '6',
    idProperty: '2',
    file: 'https://example.com/images/bulk2.jpg',
    enabled: true,
  },
];

export const mockUpdatePropertyImageDto: UpdatePropertyImageDto = {
  id: '1',
  idProperty: '1',
  enabled: false,
};

export const mockUpdatedPropertyImageDto: PropertyImageDto = {
  ...mockPropertyImageDtos[0],
  enabled: false,
};
