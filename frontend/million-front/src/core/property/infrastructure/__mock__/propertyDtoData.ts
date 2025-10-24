import type { PropertyDto, CreatePropertyDto, UpdatePropertyDto, PaginatedResponseDto } from '../propertyDto';

// Mock DTOs que vienen del backend
export const mockPropertyDtos: PropertyDto[] = [
  {
    idProperty: '1',
    idOwner: '1',
    name: 'Luxury Villa',
    address: '123 Beverly Hills, Los Angeles, CA 90210',
    price: 2500000,
    codeInternal: 'PROP-001',
    year: 2020,
    image: 'https://example.com/images/villa.jpg',
    ownerName: 'John Doe',
  },
  {
    idProperty: '2',
    idOwner: '2',
    name: 'Downtown Apartment',
    address: '456 Main St, New York, NY 10001',
    price: 850000,
    codeInternal: 'PROP-002',
    year: 2018,
    image: 'https://example.com/images/apartment.jpg',
    ownerName: 'Jane Smith',
  },
  {
    idProperty: '3',
    idOwner: '1',
    name: 'Suburban House',
    address: '789 Oak Ave, Chicago, IL 60601',
    price: 450000,
    codeInternal: 'PROP-003',
    year: 2015,
    ownerName: 'John Doe',
  },
];

export const mockCreatePropertyDto: CreatePropertyDto = {
  name: 'New Property',
  address: '555 Park Ave, Seattle, WA 98101',
  price: 750000,
  codeInternal: 'PROP-004',
  year: 2023,
  IdOwner: '2',
};

export const mockCreatedPropertyDto: PropertyDto = {
  idProperty: '4',
  idOwner: '2',
  name: 'New Property',
  address: '555 Park Ave, Seattle, WA 98101',
  price: 750000,
  codeInternal: 'PROP-004',
  year: 2023,
};

export const mockUpdatePropertyDto: UpdatePropertyDto = {
  id: '1',
  name: 'Luxury Villa Updated',
  price: 2750000,
};

export const mockUpdatedPropertyDto: PropertyDto = {
  ...mockPropertyDtos[0],
  name: 'Luxury Villa Updated',
  price: 2750000,
};

export const mockPaginatedPropertyDtos: PaginatedResponseDto<PropertyDto> = {
  pageNumber: 1,
  pageSize: 2,
  totalRecords: 3,
  totalPages: 2,
  hasNextPage: true,
  hasPreviousPage: false,
  data: mockPropertyDtos.slice(0, 2),
};

export const mockPropertyDetailDto = {
  idProperty: '1',
  name: 'Luxury Villa',
  address: '123 Beverly Hills, Los Angeles, CA 90210',
  price: 2500000,
  codeInternal: 'PROP-001',
  year: 2020,
  IdOwner: '1',
  owner: {
    name: 'John Doe',
  },
  images: [
    {
      idPropertyImage: '1',
      idProperty: '1',
      file: 'https://example.com/images/villa.jpg',
      enabled: true,
    },
  ],
};
