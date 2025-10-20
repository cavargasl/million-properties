import type {
  Property,
  CreatePropertyRequest,
  UpdatePropertyRequest,
  PropertyImage,
  PropertyTrace,
  PaginationMetadata,
} from '../../domain/property';
import type {
  PropertyDto,
  CreatePropertyDto,
  UpdatePropertyDto,
  PropertyImageDto,
  PropertyTraceDto,
  PaginatedResponseDto,
} from '../propertyDto';
import { transformDtoObject } from '@/core/shared/utils/transformDtoObject';

/**
 * Transforms PropertyDto from API to Property domain entity
 */
export const transformPropertyFromDto = (dto: PropertyDto | null): Property | null => {
  if (!dto) return null;

  return {
    id: dto.idProperty,
    name: dto.name,
    address: dto.address,
    price: dto.price,
    ownerId: dto.idOwner,
    ownerName: dto.ownerName,
    image: dto.image,
    // Campos que vienen en otras llamadas API (detail view)
    codeInternal: '',
    year: 0,
  };
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

/**
 * Transforms PaginatedResponseDto from API to domain pagination format
 */
export const transformPaginatedPropertiesFromDto = (
  dto: PaginatedResponseDto<PropertyDto>
): { items: Property[]; pagination: PaginationMetadata } => {
  const items = dto.data
    .map(transformPropertyFromDto)
    .filter((item): item is Property => item !== null);

  const pagination: PaginationMetadata = {
    pageNumber: dto.pageNumber,
    pageSize: dto.pageSize,
    totalRecords: dto.totalRecords,
    totalPages: dto.totalPages,
    hasNextPage: dto.hasNextPage,
    hasPreviousPage: dto.hasPreviousPage,
  };

  return { items, pagination };
};
