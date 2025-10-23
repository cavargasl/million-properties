import type { Result, ResultArray } from '@/core/shared/domain/types';

export interface PropertyTrace {
  id: string;
  dateSale: string;
  name: string;
  value: number;
  tax: number;
  propertyId: string;
}

export type CreatePropertyTraceRequest = Omit<PropertyTrace, 'id'>;
export type UpdatePropertyTraceRequest = Partial<CreatePropertyTraceRequest> & { id: string };

export type PropertyTraceResponse = Result<PropertyTrace>;
export type PropertyTracesResponse = ResultArray<PropertyTrace>;
