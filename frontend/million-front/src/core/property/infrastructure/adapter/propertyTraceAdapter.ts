import type {
  PropertyTrace,
  CreatePropertyTraceRequest,
  UpdatePropertyTraceRequest,
} from '../../domain/propertyTrace';
import type {
  PropertyTraceDto,
  CreatePropertyTraceDto,
  UpdatePropertyTraceDto,
} from '../propertyTraceDto';
import { transformDtoObject } from '@/core/shared/utils/transformDtoObject';

/**
 * Transforms PropertyTraceDto from API to PropertyTrace domain entity
 */
export const transformPropertyTraceFromDto = (
  dto: PropertyTraceDto | null
): PropertyTrace | null => {
  if (!dto) return null;

  return {
    id: dto.idPropertyTrace,
    dateSale: dto.dateSale,
    name: dto.name,
    value: dto.value,
    tax: dto.tax,
    propertyId: dto.idProperty,
  };
};

/**
 * Transforms CreatePropertyTraceRequest to CreatePropertyTraceDto for API
 */
export const transformCreatePropertyTraceToDto = (
  propertyTrace: CreatePropertyTraceRequest
): CreatePropertyTraceDto => {
  const mapping: Record<keyof CreatePropertyTraceRequest, keyof CreatePropertyTraceDto> = {
    dateSale: 'dateSale',
    name: 'name',
    value: 'value',
    tax: 'tax',
    propertyId: 'IdProperty',
  };

  return transformDtoObject<CreatePropertyTraceRequest, CreatePropertyTraceDto>(
    propertyTrace,
    mapping
  )
    .removeUndefined()
    .result();
};

/**
 * Transforms UpdatePropertyTraceRequest to UpdatePropertyTraceDto for API
 */
export const transformUpdatePropertyTraceToDto = (
  propertyTrace: UpdatePropertyTraceRequest
): UpdatePropertyTraceDto => {
  const { id, ...propertyTraceWithoutId } = propertyTrace;

  const mapping: Record<
    keyof Omit<UpdatePropertyTraceRequest, 'id'>,
    keyof Omit<UpdatePropertyTraceDto, 'id'>
  > = {
    dateSale: 'dateSale',
    name: 'name',
    value: 'value',
    tax: 'tax',
    propertyId: 'IdProperty',
  };

  const dto = transformDtoObject<
    Omit<UpdatePropertyTraceRequest, 'id'>,
    Omit<UpdatePropertyTraceDto, 'id'>
  >(propertyTraceWithoutId, mapping)
    .removeUndefined()
    .result();

  return { id, ...dto };
};

/**
 * Transforms array of PropertyTraceDto to PropertyTrace array
 */
export const transformPropertyTracesFromDto = (
  dtos: PropertyTraceDto[]
): PropertyTrace[] => {
  return dtos
    .map(transformPropertyTraceFromDto)
    .filter((item): item is PropertyTrace => item !== null);
};
