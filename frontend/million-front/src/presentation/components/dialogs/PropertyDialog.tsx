/**
 * PropertyDialog component for creating and editing properties
 * Uses Dialog component from shadcn/ui
 */

'use client';

import { useEffect } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { PropertyForm, type PropertyFormValues } from '../forms/PropertyForm';
import { useCreateProperty, useUpdateProperty } from '@/presentation/hooks/useProperties';
import { toast } from 'sonner';
import type { Property } from '@/core/property';

interface PropertyDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  property?: Property | null;
  mode: 'create' | 'edit';
}

export function PropertyDialog({ open, onOpenChange, property, mode }: PropertyDialogProps) {
  const createMutation = useCreateProperty();
  const updateMutation = useUpdateProperty();

  const isSubmitting = createMutation.isPending || updateMutation.isPending;

  // Reset mutations when dialog closes
  useEffect(() => {
    if (!open) {
      createMutation.reset();
      updateMutation.reset();
    }
  }, [open, createMutation, updateMutation]);

  const handleSubmit = async (values: PropertyFormValues) => {
    try {
      if (mode === 'create') {
        await createMutation.mutateAsync({
          name: values.name,
          address: values.address,
          price: values.price,
          codeInternal: values.codeInternal,
          year: values.year,
          ownerId: values.ownerId,
        });
        toast.success('Propiedad creada exitosamente');
        onOpenChange(false);
      } else if (mode === 'edit' && property?.id) {
        await updateMutation.mutateAsync({
          id: property.id,
          name: values.name,
          address: values.address,
          price: values.price,
          codeInternal: values.codeInternal,
          year: values.year,
          ownerId: values.ownerId,
        });
        toast.success('Propiedad actualizada exitosamente');
        onOpenChange(false);
      }
    } catch (error: any) {
      toast.error(error?.message || 'Ocurrió un error al guardar la propiedad');
    }
  };

  const handleCancel = () => {
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>
            {mode === 'create' ? 'Crear Nueva Propiedad' : 'Editar Propiedad'}
          </DialogTitle>
          <DialogDescription>
            {mode === 'create'
              ? 'Complete el formulario para agregar una nueva propiedad al sistema.'
              : 'Modifique los campos necesarios para actualizar la información de la propiedad.'}
          </DialogDescription>
        </DialogHeader>

        <PropertyForm
          initialValues={
            property
              ? {
                  name: property.name,
                  address: property.address,
                  price: property.price,
                  codeInternal: property.codeInternal,
                  year: property.year,
                  ownerId: property.ownerId,
                }
              : undefined
          }
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          isSubmitting={isSubmitting}
          submitLabel={mode === 'create' ? 'Crear Propiedad' : 'Actualizar Propiedad'}
        />
      </DialogContent>
    </Dialog>
  );
}
