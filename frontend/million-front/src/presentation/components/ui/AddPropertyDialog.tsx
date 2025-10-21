'use client';

import { useState } from 'react';
import { Plus, Building2, ArrowLeft } from 'lucide-react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useCreateProperty } from '@/presentation/hooks';
import { PropertyImageUpload } from '@/presentation/components/property/PropertyImageUpload';
import { toast } from 'sonner';

// Schema de validación con Zod
const propertyFormSchema = z.object({
  name: z.string().min(1, 'El nombre es requerido').max(200, 'El nombre es muy largo'),
  address: z.string().min(1, 'La dirección es requerida').max(500, 'La dirección es muy larga'),
  price: z.number().min(0, 'El precio debe ser mayor o igual a 0'),
  codeInternal: z.string().min(1, 'El código interno es requerido').max(100, 'El código es muy largo'),
  year: z.number().min(1800, 'El año debe ser mayor a 1800').max(2100, 'El año debe ser menor a 2100'),
  ownerId: z.string().min(1, 'Debe seleccionar un propietario'),
});

type PropertyFormValues = z.infer<typeof propertyFormSchema>;

export function AddPropertyDialog() {
  const [open, setOpen] = useState(false);

  const createProperty = useCreateProperty();

  const form = useForm<PropertyFormValues>({
    resolver: zodResolver(propertyFormSchema),
    defaultValues: {
      name: '',
      address: '',
      price: 0,
      codeInternal: '',
      year: new Date().getFullYear(),
      ownerId: '',
    },
  });

  const onSubmit = async (values: PropertyFormValues) => {
    try {
      const result = await createProperty.mutateAsync(values);

      toast.success('Propiedad creada exitosamente');
    } catch (error: any) {
      toast.error(error?.message || 'Error al crear la propiedad');
    }
  };

  const handleOpenChange = (newOpen: boolean) => {
    setOpen(newOpen);
    
    // Reset when closing
    if (!newOpen) {
      setTimeout(() => {
        form.reset();
      }, 300);
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button>
          <Plus className="mr-2 h-4 w-4" />
          Agregar Propiedad
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Building2 className="h-5 w-5 text-primary" />
            Agregar Nueva Propiedad
          </DialogTitle>
          <DialogDescription>
            Complete los datos de la nueva propiedad. Haga clic en guardar cuando termine.
          </DialogDescription>
        </DialogHeader>


          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
              <div className="grid gap-4 py-4">
                <FormField
                  control={form.control}
                  name="name"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Nombre</FormLabel>
                      <FormControl>
                        <Input placeholder="Casa en el centro" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="address"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Dirección</FormLabel>
                      <FormControl>
                        <Input placeholder="Calle 123 #45-67" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <div className="grid grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="price"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Precio</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            placeholder="250000000"
                            {...field}
                            onChange={(e) => field.onChange(e.target.valueAsNumber)}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="year"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Año</FormLabel>
                        <FormControl>
                          <Input
                            type="number"
                            placeholder="2024"
                            {...field}
                            onChange={(e) => field.onChange(e.target.valueAsNumber)}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <FormField
                  control={form.control}
                  name="codeInternal"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Código Interno</FormLabel>
                      <FormControl>
                        <Input placeholder="PROP-001" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <DialogFooter>
                <Button type="button" variant="outline" onClick={() => handleOpenChange(false)}>
                  Cancelar
                </Button>
                <Button type="submit" disabled={createProperty.isPending}>
                  {createProperty.isPending ? 'Guardando...' : 'Guardar'}
                </Button>
              </DialogFooter>
            </form>
          </Form>

      </DialogContent>
    </Dialog>
  );
}