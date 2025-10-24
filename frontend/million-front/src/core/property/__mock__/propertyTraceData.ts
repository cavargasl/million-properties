import type {
  PropertyTrace,
  PropertyTraceResponse,
  PropertyTracesResponse,
  CreatePropertyTraceRequest,
  UpdatePropertyTraceRequest,
} from '../domain/propertyTrace';

// Mock data
export const mockPropertyTraces: PropertyTrace[] = [
  {
    id: '1',
    propertyId: '1',
    dateSale: '2024-01-15',
    name: 'Initial Sale',
    value: 2500000,
    tax: 50000,
  },
  {
    id: '2',
    propertyId: '1',
    dateSale: '2024-03-20',
    name: 'Price Adjustment',
    value: 2750000,
    tax: 55000,
  },
  {
    id: '3',
    propertyId: '2',
    dateSale: '2024-02-10',
    name: 'Market Value Update',
    value: 850000,
    tax: 17000,
  },
];

export const mockCreatePropertyTraceRequest: CreatePropertyTraceRequest = {
  propertyId: '1',
  dateSale: '2024-05-01',
  name: 'New Valuation',
  value: 3000000,
  tax: 60000,
};

export const mockCreatedPropertyTrace: PropertyTrace = {
  id: '4',
  ...mockCreatePropertyTraceRequest,
};

export const mockUpdatePropertyTraceRequest: UpdatePropertyTraceRequest = {
  id: '1',
  name: 'Updated Initial Sale',
  value: 2600000,
  tax: 52000,
};

export const mockUpdatedPropertyTrace: PropertyTrace = {
  ...mockPropertyTraces[0],
  name: 'Updated Initial Sale',
  value: 2600000,
  tax: 52000,
};

// Mock responses
export const mockPropertyTracesResponse: PropertyTracesResponse = {
  data: mockPropertyTraces.filter(trace => trace.propertyId === '1'),
  error: null,
};

export const mockPropertyTraceResponse: PropertyTraceResponse = {
  data: mockPropertyTraces[0],
  error: null,
};

export const mockPropertyTraceCreatedResponse: PropertyTraceResponse = {
  data: mockCreatedPropertyTrace,
  error: null,
};

export const mockPropertyTraceUpdatedResponse: PropertyTraceResponse = {
  data: mockUpdatedPropertyTrace,
  error: null,
};

export const mockPropertyTraceDeletedResponse: PropertyTraceResponse = {
  data: mockPropertyTraces[0],
  error: null,
};
