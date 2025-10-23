import type { PaginationMetadata } from '@/core/shared/domain/types';
import type {
  Property,
  CreatePropertyRequest,
  UpdatePropertyRequest,
} from '../../domain/property';
import type {
  PropertyDto,
  CreatePropertyDto,
  UpdatePropertyDto,
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
    codeInternal: dto.codeInternal,
    year: dto.year,
    ownerId: dto.idOwner,
    ownerName: dto.ownerName,
    image: dto.image,
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
    ownerId: 'IdOwner',
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
    ownerId: 'IdOwner',
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
