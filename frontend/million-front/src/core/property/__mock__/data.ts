import type {
  Property,
  PropertyResponse,
  PropertiesResponse,
  PaginatedPropertiesResponse,
  CreatePropertyRequest,
  UpdatePropertyRequest,
} from '../domain/property';

// Mock data
export const mockProperties: Property[] = [
  {
    id: '1',
    name: 'Luxury Villa',
    address: '123 Beverly Hills, Los Angeles, CA 90210',
    price: 2500000,
    codeInternal: 'PROP-001',
    year: 2020,
    ownerId: '1',
    ownerName: 'John Doe',
    image: 'https://example.com/images/villa.jpg',
    createdAt: '2024-01-15T10:30:00Z',
    updatedAt: '2024-01-15T10:30:00Z',
  },
  {
    id: '2',
    name: 'Downtown Apartment',
    address: '456 Main St, New York, NY 10001',
    price: 850000,
    codeInternal: 'PROP-002',
    year: 2018,
    ownerId: '2',
    ownerName: 'Jane Smith',
    image: 'https://example.com/images/apartment.jpg',
    createdAt: '2024-01-16T11:20:00Z',
    updatedAt: '2024-01-16T11:20:00Z',
  },
  {
    id: '3',
    name: 'Suburban House',
    address: '789 Oak Ave, Chicago, IL 60601',
    price: 450000,
    codeInternal: 'PROP-003',
    year: 2015,
    ownerId: '1',
    ownerName: 'John Doe',
    createdAt: '2024-01-17T09:15:00Z',
    updatedAt: '2024-01-17T09:15:00Z',
  },
  {
    id: '4',
    name: 'Beach Condo',
    address: '321 Ocean Dr, Miami, FL 33139',
    price: 1200000,
    codeInternal: 'PROP-004',
    year: 2022,
    ownerId: '3',
    ownerName: 'Robert Johnson',
    image: 'https://example.com/images/condo.jpg',
    createdAt: '2024-01-18T14:30:00Z',
    updatedAt: '2024-01-18T14:30:00Z',
  },
];

export const mockCreatePropertyRequest: CreatePropertyRequest = {
  name: 'New Property',
  address: '555 Park Ave, Seattle, WA 98101',
  price: 750000,
  codeInternal: 'PROP-005',
  year: 2023,
  ownerId: '2',
};

export const mockCreatedProperty: Property = {
  id: '5',
  ...mockCreatePropertyRequest,
  createdAt: '2024-01-20T16:45:00Z',
  updatedAt: '2024-01-20T16:45:00Z',
};

export const mockUpdatePropertyRequest: UpdatePropertyRequest = {
  id: '1',
  name: 'Luxury Villa Updated',
  price: 2750000,
};

export const mockUpdatedProperty: Property = {
  ...mockProperties[0],
  name: 'Luxury Villa Updated',
  price: 2750000,
  updatedAt: '2024-01-25T10:00:00Z',
};

// Mock responses
export const mockPropertiesResponse: PropertiesResponse = {
  data: mockProperties,
  error: null,
};

export const mockPaginatedPropertiesResponse: PaginatedPropertiesResponse = {
  data: {
    items: mockProperties.slice(0, 2),
    pagination: {
      pageNumber: 1,
      pageSize: 2,
      totalRecords: 4,
      totalPages: 2,
      hasNextPage: true,
      hasPreviousPage: false,
    },
  },
  error: null,
};

export const mockPropertyResponse: PropertyResponse = {
  data: mockProperties[0],
  error: null,
};

export const mockPropertyNotFoundResponse: PropertyResponse = {
  data: null,
  error: {
    message: 'Property not found',
    code: 'PROPERTY_NOT_FOUND',
  },
};

export const mockPropertyCreatedResponse: PropertyResponse = {
  data: mockCreatedProperty,
  error: null,
};

export const mockPropertyUpdatedResponse: PropertyResponse = {
  data: mockUpdatedProperty,
  error: null,
};

export const mockPropertyDeletedResponse: PropertyResponse = {
  data: mockProperties[0],
  error: null,
};

// Filtered mock responses
export const mockFilteredByOwnerResponse: PropertiesResponse = {
  data: mockProperties.filter(p => p.ownerId === '1'),
  error: null,
};

export const mockFilteredByPriceRangeResponse: PropertiesResponse = {
  data: mockProperties.filter(p => p.price >= 500000 && p.price <= 1500000),
  error: null,
};
