import type { Owner, CreateOwnerRequest, UpdateOwnerRequest } from '../../domain/owner';
import type { OwnerDto, CreateOwnerDto, UpdateOwnerDto } from '../ownerDto';
import { transformDtoObject } from '@/core/shared/utils/transformDtoObject';

/**
 * Transforms OwnerDto from API to Owner domain entity
 */
export const transformOwnerFromDto = (dto: OwnerDto | null): Owner | null => {
  if (!dto) return null;

  const mapping: Record<keyof OwnerDto, keyof Owner> = {
    idOwner: 'id',
    name: 'name',
    address: 'address',
    photo: 'photo',
    birthday: 'birthday',
  };

  return transformDtoObject<OwnerDto, Owner>(dto, mapping)
    .nullOrEmptyToUndefined()
    .result();
};

/**
 * Transforms CreateOwnerRequest to CreateOwnerDto for API
 */
export const transformCreateOwnerToDto = (owner: CreateOwnerRequest): CreateOwnerDto => {
  const mapping: Record<keyof CreateOwnerRequest, keyof CreateOwnerDto> = {
    name: 'name',
    address: 'address',
    photo: 'photo',
    birthday: 'birthday',
  };

  return transformDtoObject<CreateOwnerRequest, CreateOwnerDto>(owner, mapping)
    .removeUndefined()
    .result();
};

/**
 * Transforms UpdateOwnerRequest to UpdateOwnerDto for API
 */
export const transformUpdateOwnerToDto = (owner: UpdateOwnerRequest): UpdateOwnerDto => {
  const { id, ...ownerWithoutId } = owner;

  const mapping: Record<
    keyof Omit<UpdateOwnerRequest, 'id'>,
    keyof Omit<UpdateOwnerDto, 'idOwner'>
  > = {
    name: 'name',
    address: 'address',
    photo: 'photo',
    birthday: 'birthday',
  };

  const dto = transformDtoObject<Omit<UpdateOwnerRequest, 'id'>, Omit<UpdateOwnerDto, 'idOwner'>>(
    ownerWithoutId,
    mapping
  )
    .removeUndefined()
    .result();

  return { idOwner: id, ...dto };
};
