import type { OwnerDto, CreateOwnerDto, UpdateOwnerDto } from '../ownerDto';

// Mock DTOs que vienen del backend
export const mockOwnerDtos: OwnerDto[] = [
  {
    idOwner: '1',
    name: 'John Doe',
    address: '123 Main St, New York, NY 10001',
    photo: 'https://example.com/photos/john-doe.jpg',
    birthday: '1980-05-15',
  },
  {
    idOwner: '2',
    name: 'Jane Smith',
    address: '456 Oak Ave, Los Angeles, CA 90001',
    photo: 'https://example.com/photos/jane-smith.jpg',
    birthday: '1985-08-22',
  },
  {
    idOwner: '3',
    name: 'Robert Johnson',
    address: '789 Pine Rd, Chicago, IL 60601',
    birthday: '1975-12-10',
  },
];

export const mockCreateOwnerDto: CreateOwnerDto = {
  name: 'Alice Williams',
  address: '321 Elm St, Miami, FL 33101',
  photo: 'https://example.com/photos/alice-williams.jpg',
  birthday: '1990-03-25',
};

export const mockCreatedOwnerDto: OwnerDto = {
  idOwner: '4',
  ...mockCreateOwnerDto,
};

export const mockUpdateOwnerDto: UpdateOwnerDto = {
  idOwner: '1',
  name: 'John Doe Updated',
  address: '123 Main St, New York, NY 10001',
};

export const mockUpdatedOwnerDto: OwnerDto = {
  idOwner: '1',
  name: 'John Doe Updated',
  address: '123 Main St, New York, NY 10001',
  photo: 'https://example.com/photos/john-doe.jpg',
  birthday: '1980-05-15',
};
