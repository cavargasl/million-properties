'use client';

import { useProperties } from '@/presentation/hooks';
import { Building2, Loader2 } from 'lucide-react';

export function PropertiesExample() {
  const { data: properties, isLoading, error } = useProperties();

  if (isLoading) {
    return (
      <div className="flex items-center justify-center p-8">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-destructive/10 text-destructive p-4 rounded-lg">
        <p className="font-semibold">Error al cargar propiedades</p>
        <p className="text-sm">{error.message}</p>
      </div>
    );
  }

  if (!properties || properties.length === 0) {
    return (
      <div className="text-center p-8 text-muted-foreground">
        <Building2 className="h-12 w-12 mx-auto mb-4 opacity-50" />
        <p>No hay propiedades disponibles</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {properties.map((property) => (
        <div
          key={property.id}
          className="border border-border rounded-lg p-6 hover:shadow-md transition-shadow"
        >
          <div className="flex items-start justify-between mb-4">
            <div>
              <h3 className="font-semibold text-lg">{property.name}</h3>
              <p className="text-sm text-muted-foreground">{property.address}</p>
            </div>
            <Building2 className="h-5 w-5 text-primary" />
          </div>

          <div className="space-y-2 text-sm">
            <div className="flex justify-between">
              <span className="text-muted-foreground">Precio:</span>
              <span className="font-semibold">
                ${property.price.toLocaleString('es-CO')}
              </span>
            </div>
            <div className="flex justify-between">
              <span className="text-muted-foreground">Año:</span>
              <span>{property.year}</span>
            </div>
            <div className="flex justify-between">
              <span className="text-muted-foreground">Código:</span>
              <span className="font-mono text-xs">{property.codeInternal}</span>
            </div>
          </div>
        </div>
      ))}
    </div>
  );
}
