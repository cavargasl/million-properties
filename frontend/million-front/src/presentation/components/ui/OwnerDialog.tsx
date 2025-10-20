/**
 * OwnerDialog component for creating and editing owners
 */

'use client';

import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useCreateOwner, useUpdateOwner } from '@/presentation/hooks/useOwners';
import type { Owner, CreateOwnerRequest } from '@/core/owner';
import { Loader2 } from 'lucide-react';
import { toast } from 'sonner';

interface OwnerDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  owner?: Owner | null;
}

type OwnerFormData = {
  name: string;
  address: string;
  photo?: string;
  birthday: string;
};

export function OwnerDialog({ open, onOpenChange, owner }: OwnerDialogProps) {
  const isEditing = !!owner;
  const createMutation = useCreateOwner();
  const updateMutation = useUpdateOwner();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<OwnerFormData>({
    defaultValues: {
      name: '',
      address: '',
      photo: '',
      birthday: '',
    },
  });

  // Reset form when dialog opens with owner data
  useEffect(() => {
    if (open) {
      if (owner) {
        // Format date for input type="date"
        const formattedBirthday = owner.birthday.split('T')[0];
        reset({
          name: owner.name,
          address: owner.address,
          photo: owner.photo || '',
          birthday: formattedBirthday,
        });
      } else {
        reset({
          name: '',
          address: '',
          photo: '',
          birthday: '',
        });
      }
    }
  }, [open, owner, reset]);

  const onSubmit = async (data: OwnerFormData) => {
    try {
      if (isEditing) {
        await updateMutation.mutateAsync({
          id: owner.id,
          name: data.name,
          address: data.address,
          photo: data.photo || undefined,
          birthday: data.birthday,
        });
        toast.success('Propietario actualizado exitosamente');
      } else {
        const ownerData: CreateOwnerRequest = {
          name: data.name,
          address: data.address,
          photo: data.photo || undefined,
          birthday: data.birthday,
        };
        await createMutation.mutateAsync(ownerData);
        toast.success('Propietario creado exitosamente');
      }
      onOpenChange(false);
      reset();
    } catch (error: any) {
      console.error('Error saving owner:', error);
      // El transformError de axios ya procesó el error y extrajo el mensaje apropiado
      toast.error(error?.message || 'Error al guardar el propietario. Por favor, intenta de nuevo.');
    }
  };

  const isLoading = createMutation.isPending || updateMutation.isPending;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle>
            {isEditing ? 'Editar Propietario' : 'Nuevo Propietario'}
          </DialogTitle>
          <DialogDescription>
            {isEditing
              ? 'Actualiza la información del propietario'
              : 'Completa los datos del nuevo propietario'}
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Name */}
          <div className="space-y-2">
            <Label htmlFor="name">
              Nombre <span className="text-red-500">*</span>
            </Label>
            <Input
              id="name"
              {...register('name', {
                required: 'El nombre es obligatorio',
                minLength: {
                  value: 2,
                  message: 'El nombre debe tener al menos 2 caracteres',
                },
                maxLength: {
                  value: 100,
                  message: 'El nombre no puede superar 100 caracteres',
                },
              })}
              placeholder="Juan Pérez"
              disabled={isLoading}
            />
            {errors.name && (
              <p className="text-sm text-red-500">{errors.name.message}</p>
            )}
          </div>

          {/* Address */}
          <div className="space-y-2">
            <Label htmlFor="address">
              Dirección <span className="text-red-500">*</span>
            </Label>
            <Input
              id="address"
              {...register('address', {
                required: 'La dirección es obligatoria',
                minLength: {
                  value: 5,
                  message: 'La dirección debe tener al menos 5 caracteres',
                },
                maxLength: {
                  value: 200,
                  message: 'La dirección no puede superar 200 caracteres',
                },
              })}
              placeholder="Calle Principal 123"
              disabled={isLoading}
            />
            {errors.address && (
              <p className="text-sm text-red-500">{errors.address.message}</p>
            )}
          </div>

          {/* Birthday */}
          <div className="space-y-2">
            <Label htmlFor="birthday">
              Fecha de Nacimiento <span className="text-red-500">*</span>
            </Label>
            <Input
              id="birthday"
              type="date"
              {...register('birthday', {
                required: 'La fecha de nacimiento es obligatoria',
                validate: {
                  notFuture: (value) => {
                    const selectedDate = new Date(value);
                    const today = new Date();
                    return (
                      selectedDate < today ||
                      'La fecha de nacimiento no puede ser futura'
                    );
                  },
                  validAge: (value) => {
                    const birthDate = new Date(value);
                    const today = new Date();
                    let age = today.getFullYear() - birthDate.getFullYear();
                    const monthDiff = today.getMonth() - birthDate.getMonth();
                    
                    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
                      age--;
                    }
                    
                    return age >= 18 || 'El propietario debe ser mayor de edad';
                  },
                },
              })}
              max={new Date().toISOString().split('T')[0]}
              disabled={isLoading}
            />
            {errors.birthday && (
              <p className="text-sm text-red-500">{errors.birthday.message}</p>
            )}
          </div>

          {/* Photo URL */}
          <div className="space-y-2">
            <Label htmlFor="photo">URL de la Foto (opcional)</Label>
            <Input
              id="photo"
              {...register('photo', {
                pattern: {
                  value: /^https?:\/\/.+/,
                  message: 'Debe ser una URL válida (http:// o https://)',
                },
              })}
              placeholder="https://ejemplo.com/foto.jpg"
              disabled={isLoading}
            />
            {errors.photo && (
              <p className="text-sm text-red-500">{errors.photo.message}</p>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => onOpenChange(false)}
              disabled={isLoading}
            >
              Cancelar
            </Button>
            <Button type="submit" disabled={isLoading}>
              {isLoading && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              {isEditing ? 'Actualizar' : 'Crear'}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
