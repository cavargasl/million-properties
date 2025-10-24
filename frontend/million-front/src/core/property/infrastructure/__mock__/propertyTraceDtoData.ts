import type { PropertyTraceDto, CreatePropertyTraceDto, UpdatePropertyTraceDto } from '../propertyTraceDto';

// Mock DTOs que vienen del backend
export const mockPropertyTraceDtos: PropertyTraceDto[] = [
  {
    idPropertyTrace: '1',
    idProperty: '1',
    dateSale: '2024-01-15',
    name: 'Initial Sale',
    value: 2500000,
    tax: 50000,
  },
  {
    idPropertyTrace: '2',
    idProperty: '1',
    dateSale: '2024-03-20',
    name: 'Price Adjustment',
    value: 2750000,
    tax: 55000,
  },
  {
    idPropertyTrace: '3',
    idProperty: '2',
    dateSale: '2024-02-10',
    name: 'Market Value Update',
    value: 850000,
    tax: 17000,
  },
];

export const mockCreatePropertyTraceDto: CreatePropertyTraceDto = {
  dateSale: '2024-05-01',
  name: 'New Valuation',
  value: 3000000,
  tax: 60000,
  IdProperty: '1',
};

export const mockCreatedPropertyTraceDto: PropertyTraceDto = {
  idPropertyTrace: '4',
  idProperty: '1',
  dateSale: '2024-05-01',
  name: 'New Valuation',
  value: 3000000,
  tax: 60000,
};

export const mockUpdatePropertyTraceDto: UpdatePropertyTraceDto = {
  id: '1',
  name: 'Updated Initial Sale',
  value: 2600000,
  tax: 52000,
};

export const mockUpdatedPropertyTraceDto: PropertyTraceDto = {
  ...mockPropertyTraceDtos[0],
  name: 'Updated Initial Sale',
  value: 2600000,
  tax: 52000,
};
