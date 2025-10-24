import type { Owner, OwnerResponse, OwnersResponse, CreateOwnerRequest, UpdateOwnerRequest } from '../domain/owner';

// Mock data
export const mockOwners: Owner[] = [
  {
    id: '1',
    name: 'John Doe',
    address: '123 Main St, New York, NY 10001',
    photo: 'https://example.com/photos/john-doe.jpg',
    birthday: '1980-05-15',
    createdAt: '2024-01-15T10:30:00Z',
    updatedAt: '2024-01-15T10:30:00Z',
  },
  {
    id: '2',
    name: 'Jane Smith',
    address: '456 Oak Ave, Los Angeles, CA 90001',
    photo: 'https://example.com/photos/jane-smith.jpg',
    birthday: '1985-08-22',
    createdAt: '2024-01-16T11:20:00Z',
    updatedAt: '2024-01-16T11:20:00Z',
  },
  {
    id: '3',
    name: 'Robert Johnson',
    address: '789 Pine Rd, Chicago, IL 60601',
    birthday: '1975-12-10',
    createdAt: '2024-01-17T09:15:00Z',
    updatedAt: '2024-01-17T09:15:00Z',
  },
];

export const mockCreateOwnerRequest: CreateOwnerRequest = {
  name: 'Alice Williams',
  address: '321 Elm St, Miami, FL 33101',
  photo: 'https://example.com/photos/alice-williams.jpg',
  birthday: '1990-03-25',
};

export const mockCreatedOwner: Owner = {
  id: '4',
  ...mockCreateOwnerRequest,
  createdAt: '2024-01-18T14:30:00Z',
  updatedAt: '2024-01-18T14:30:00Z',
};

export const mockUpdateOwnerRequest: UpdateOwnerRequest = {
  id: '1',
  name: 'John Doe Updated',
  address: '123 Main St, New York, NY 10001',
};

export const mockUpdatedOwner: Owner = {
  ...mockOwners[0],
  name: 'John Doe Updated',
  updatedAt: '2024-01-20T16:45:00Z',
};

// Mock responses
export const mockOwnersResponse: OwnersResponse = {
  data: mockOwners,
  error: null,
};

export const mockOwnerResponse: OwnerResponse = {
  data: mockOwners[0],
  error: null,
};

export const mockOwnerNotFoundResponse: OwnerResponse = {
  data: null,
  error: {
    message: 'Owner not found',
    code: 'OWNER_NOT_FOUND',
  },
};

export const mockOwnerCreatedResponse: OwnerResponse = {
  data: mockCreatedOwner,
  error: null,
};

export const mockOwnerUpdatedResponse: OwnerResponse = {
  data: mockUpdatedOwner,
  error: null,
};

export const mockOwnerDeletedResponse: OwnerResponse = {
  data: mockOwners[0],
  error: null,
};
