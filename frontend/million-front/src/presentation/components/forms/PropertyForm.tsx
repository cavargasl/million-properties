"use client";

import { Alert, AlertDescription } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { useOwners } from "@/presentation/hooks/useOwners";
import { zodResolver } from "@hookform/resolvers/zod";
import { AlertCircle } from "lucide-react";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import * as z from "zod";

// Form validation schema
const propertyFormSchema = z.object({
  name: z
    .string()
    .min(2, "El nombre debe tener al menos 2 caracteres")
    .max(100, "El nombre no puede exceder 100 caracteres"),
  address: z
    .string()
    .min(5, "La dirección debe tener al menos 5 caracteres")
    .max(250, "La dirección no puede exceder 250 caracteres"),
  price: z
    .number()
    .positive("El precio debe ser mayor a 0")
    .min(0.01, "El precio debe ser mayor a 0"),
  codeInternal: z
    .string()
    .min(3, "El código debe tener al menos 3 caracteres")
    .max(50, "El código no puede exceder 50 caracteres")
    .regex(
      /^[A-Za-z0-9-_]+$/,
      "El código solo puede contener letras, números, guiones y guiones bajos"
    ),
  year: z
    .number()
    .int("El año debe ser un número entero")
    .min(1800, "El año no puede ser anterior a 1800")
    .max(
      new Date().getFullYear() + 5,
      `El año no puede ser más de 5 años en el futuro`
    ),
  ownerId: z.string().min(1, "Debe seleccionar un propietario"),
});

export type PropertyFormValues = z.infer<typeof propertyFormSchema>;

interface PropertyFormProps {
  initialValues?: PropertyFormValues;
  onSubmit: (values: PropertyFormValues) => void;
  onCancel?: () => void;
  isSubmitting?: boolean;
  submitLabel?: string;
}

export function PropertyForm({
  initialValues,
  onSubmit,
  onCancel,
  isSubmitting = false,
  submitLabel = "Guardar",
}: PropertyFormProps) {
  const {
    data: owners,
    isLoading: isLoadingOwners,
    isError: isOwnersError,
  } = useOwners();

  const form = useForm<PropertyFormValues>({
    resolver: zodResolver(propertyFormSchema),
    defaultValues: {
      name: initialValues?.name || "",
      address: initialValues?.address || "",
      price: initialValues?.price || 0,
      codeInternal: initialValues?.codeInternal || "",
      year: initialValues?.year || new Date().getFullYear(),
      ownerId: initialValues?.ownerId || "",
    },
  });

  // Update form when initialValues change (for edit mode)
  useEffect(() => {
    if (initialValues) {
      form.reset({
        name: initialValues.name || "",
        address: initialValues.address || "",
        price: initialValues.price || 0,
        codeInternal: initialValues.codeInternal || "",
        year: initialValues.year || new Date().getFullYear(),
        ownerId: initialValues.ownerId || "",
      });
    }
  }, [initialValues, form]);

  const handleSubmit = (values: PropertyFormValues) => {
    onSubmit(values);
  };

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-6">
        {/* Property Name */}
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Nombre de la Propiedad *</FormLabel>
              <FormControl>
                <Input placeholder="Ej: Casa en el centro" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Address */}
        <FormField
          control={form.control}
          name="address"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Dirección *</FormLabel>
              <FormControl>
                <Input placeholder="Ej: Calle 123 #45-67" {...field} />
              </FormControl>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Price and Year in a grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {/* Price */}
          <FormField
            control={form.control}
            name="price"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Precio (COP) *</FormLabel>
                <FormControl>
                  <Input
                    type="number"
                    placeholder="150000000"
                    onFocus={(e) =>
                      Number(e.target.value) === 0
                        ? field.onChange("")
                        : field.onChange(e.target.value)
                    }
                    {...field}
                    onChange={(e) => field.onChange(parseFloat(e.target.value))}
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />

          {/* Year */}
          <FormField
            control={form.control}
            name="year"
            render={({ field }) => (
              <FormItem>
                <FormLabel>Año de Construcción *</FormLabel>
                <FormControl>
                  <Input
                    type="number"
                    placeholder="2024"
                    {...field}
                    onChange={(e) =>
                      field.onChange(parseInt(e.target.value) || 0)
                    }
                  />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
        </div>

        {/* Internal Code */}
        <FormField
          control={form.control}
          name="codeInternal"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Código Interno *</FormLabel>
              <FormControl>
                <Input placeholder="Ej: PROP-001" {...field} />
              </FormControl>
              <FormDescription>
                Solo letras, números, guiones y guiones bajos
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Owner Selection */}
        <FormField
          control={form.control}
          name="ownerId"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Propietario *</FormLabel>
              {isLoadingOwners ? (
                <Skeleton className="h-10 w-full" />
              ) : isOwnersError ? (
                <Alert variant="destructive">
                  <AlertCircle className="h-4 w-4" />
                  <AlertDescription>
                    Error al cargar los propietarios. Por favor intente de
                    nuevo.
                  </AlertDescription>
                </Alert>
              ) : (
                <Select onValueChange={field.onChange} value={field.value}>
                  <FormControl>
                    <SelectTrigger>
                      <SelectValue placeholder="Seleccione un propietario" />
                    </SelectTrigger>
                  </FormControl>
                  <SelectContent>
                    {owners && owners.length > 0 ? (
                      owners.map((owner) => (
                        <SelectItem key={owner.id} value={owner.id}>
                          {owner.name}
                        </SelectItem>
                      ))
                    ) : (
                      <SelectItem value="no-owners" disabled>
                        No hay propietarios disponibles
                      </SelectItem>
                    )}
                  </SelectContent>
                </Select>
              )}
              <FormDescription>
                Seleccione el propietario de la propiedad
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Form Actions */}
        <div className="flex justify-end gap-4 pt-4">
          {onCancel && (
            <Button
              type="button"
              variant="outline"
              onClick={onCancel}
              disabled={isSubmitting}
            >
              Cancelar
            </Button>
          )}
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Guardando..." : submitLabel}
          </Button>
        </div>
      </form>
    </Form>
  );
}
