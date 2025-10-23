import type {
  CreatePropertyTraceRequest,
  UpdatePropertyTraceRequest,
  PropertyTraceResponse,
  PropertyTracesResponse,
} from './propertyTrace';

export interface PropertyTraceRepository {
  getByPropertyId(propertyId: string): Promise<PropertyTracesResponse>;
  create(input: CreatePropertyTraceRequest): Promise<PropertyTraceResponse>;
  update(input: UpdatePropertyTraceRequest): Promise<PropertyTraceResponse>;
  delete(propertyId: string, id: string): Promise<PropertyTraceResponse>;
}
