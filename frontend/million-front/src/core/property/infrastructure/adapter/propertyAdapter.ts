import type {
  Property,
  CreatePropertyRequest,
  UpdatePropertyRequest,
  PropertyImage,
  PropertyTrace,
} from '../../domain/property';
import type {
  PropertyDto,
  CreatePropertyDto,
  UpdatePropertyDto,
  PropertyImageDto,
  PropertyTraceDto,
} from '../propertyDto';
import { transformDtoObject } from '@/core/shared/utils/transformDtoObject';

/**
 * Transforms PropertyDto from API to Property domain entity
 */
export const transformPropertyFromDto = (dto: PropertyDto | null): Property | null => {
  if (!dto) return null;

  const mapping: Record<keyof PropertyDto, keyof Property> = {
    id: 'id',
    name: 'name',
    address: 'address',
    price: 'price',
    codeInternal: 'codeInternal',
    year: 'year',
    ownerId: 'ownerId',
    ownerName: 'ownerName',
  };

  return transformDtoObject<PropertyDto, Property>(dto, mapping)
    .nullOrEmptyToUndefined()
    .result();
};

/**
 * Transforms CreatePropertyRequest to CreatePropertyDto for API
 */
export const transformCreatePropertyToDto = (
  property: CreatePropertyRequest
): CreatePropertyDto => {
  const mapping: Record<keyof CreatePropertyRequest, keyof CreatePropertyDto> = {
    name: 'name',
    address: 'address',
    price: 'price',
    codeInternal: 'codeInternal',
    year: 'year',
    ownerId: 'ownerId',
  };

  return transformDtoObject<CreatePropertyRequest, CreatePropertyDto>(property, mapping)
    .removeUndefined()
    .result();
};

/**
 * Transforms UpdatePropertyRequest to UpdatePropertyDto for API
 */
export const transformUpdatePropertyToDto = (
  property: UpdatePropertyRequest
): UpdatePropertyDto => {
  const { id, ...propertyWithoutId } = property;
  
  const mapping: Record<
    keyof Omit<UpdatePropertyRequest, 'id'>,
    keyof Omit<UpdatePropertyDto, 'id'>
  > = {
    name: 'name',
    address: 'address',
    price: 'price',
    codeInternal: 'codeInternal',
    year: 'year',
    ownerId: 'ownerId',
  };

  const dto = transformDtoObject<
    Omit<UpdatePropertyRequest, 'id'>,
    Omit<UpdatePropertyDto, 'id'>
  >(propertyWithoutId, mapping)
    .removeUndefined()
    .result();

  return { id, ...dto };
};

/**
 * Transforms PropertyImageDto from API to PropertyImage domain entity
 */
export const transformPropertyImageFromDto = (
  dto: PropertyImageDto | null
): PropertyImage | null => {
  if (!dto) return null;

  const mapping: Record<keyof PropertyImageDto, keyof PropertyImage> = {
    id: 'id',
    propertyId: 'propertyId',
    file: 'file',
    enabled: 'enabled',
  };

  return transformDtoObject<PropertyImageDto, PropertyImage>(dto, mapping)
    .nullOrEmptyToUndefined()
    .result();
};

/**
 * Transforms PropertyTraceDto from API to PropertyTrace domain entity
 */
export const transformPropertyTraceFromDto = (
  dto: PropertyTraceDto | null
): PropertyTrace | null => {
  if (!dto) return null;

  const mapping: Record<keyof PropertyTraceDto, keyof PropertyTrace> = {
    id: 'id',
    dateSale: 'dateSale',
    name: 'name',
    value: 'value',
    tax: 'tax',
    propertyId: 'propertyId',
  };

  return transformDtoObject<PropertyTraceDto, PropertyTrace>(dto, mapping)
    .nullOrEmptyToUndefined()
    .result();
};
