import type {
  PropertyImage,
  CreatePropertyImageRequest,
  UpdatePropertyImageRequest,
} from '../../domain/propertyImage';
import type {
  PropertyImageDto,
  CreatePropertyImageDto,
  UpdatePropertyImageDto,
} from '../propertyImageDto';
import { transformDtoObject } from '@/core/shared/utils/transformDtoObject';

/**
 * Transforms PropertyImageDto from API to PropertyImage domain entity
 */
export const transformPropertyImageFromDto = (
  dto: PropertyImageDto | null
): PropertyImage | null => {
  if (!dto) return null;

  return {
    id: dto.idPropertyImage,
    propertyId: dto.idProperty,
    file: dto.file,
    enabled: dto.enabled,
  };
};

/**
 * Transforms CreatePropertyImageRequest to CreatePropertyImageDto for API
 */
export const transformCreatePropertyImageToDto = (
  propertyImage: CreatePropertyImageRequest
): CreatePropertyImageDto => {
  const mapping: Record<keyof CreatePropertyImageRequest, keyof CreatePropertyImageDto> = {
    propertyId: 'IdProperty',
    file: 'File',
    enabled: 'Enabled',
  };

  return transformDtoObject<CreatePropertyImageRequest, CreatePropertyImageDto>(
    propertyImage,
    mapping
  )
    .removeUndefined()
    .result();
};

/**
 * Transforms UpdatePropertyImageRequest to UpdatePropertyImageDto for API
 */
export const transformUpdatePropertyImageToDto = (
  propertyImage: UpdatePropertyImageRequest
): UpdatePropertyImageDto => {
  const { id, propertyId, ...propertyImageWithoutIds } = propertyImage;

  const mapping: Record<
    keyof Omit<UpdatePropertyImageRequest, 'id' | 'propertyId'>,
    keyof Omit<UpdatePropertyImageDto, 'id' | 'idProperty'>
  > = {
    file: 'file',
    enabled: 'enabled',
  };

  const dto = transformDtoObject<
    Omit<UpdatePropertyImageRequest, 'id' | 'propertyId'>,
    Omit<UpdatePropertyImageDto, 'id' | 'idProperty'>
  >(propertyImageWithoutIds, mapping)
    .removeUndefined()
    .result();

  return { id, idProperty: propertyId, ...dto };
};

/**
 * Transforms array of PropertyImageDto to PropertyImage array
 */
export const transformPropertyImagesFromDto = (
  dtos: PropertyImageDto[]
): PropertyImage[] => {
  return dtos
    .map(transformPropertyImageFromDto)
    .filter((item): item is PropertyImage => item !== null);
};
