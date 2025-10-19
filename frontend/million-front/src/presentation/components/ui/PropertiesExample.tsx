'use client';

import { useProperties } from '@/presentation/hooks';
import { Building2, Loader2, AlertCircle, DollarSign, Calendar, Hash } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';

export function PropertiesExample() {
  const { data: properties, isLoading, error } = useProperties();

  if (isLoading) {
    return (
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {[...Array(6)].map((_, i) => (
          <Card key={i}>
            <CardHeader>
              <Skeleton className="h-6 w-3/4" />
              <Skeleton className="h-4 w-1/2" />
            </CardHeader>
            <CardContent>
              <Skeleton className="h-20 w-full" />
            </CardContent>
          </Card>
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Error al cargar propiedades</AlertTitle>
        <AlertDescription>{error.message}</AlertDescription>
      </Alert>
    );
  }

  if (!properties || properties.length === 0) {
    return (
      <Card>
        <CardContent className="text-center p-12">
          <Building2 className="h-12 w-12 mx-auto mb-4 opacity-50 text-muted-foreground" />
          <CardTitle className="mb-2">No hay propiedades disponibles</CardTitle>
          <CardDescription>
            Aún no se han registrado propiedades en el sistema
          </CardDescription>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
      {properties.map((property) => (
        <Card key={property.id} className="hover:shadow-lg transition-shadow">
          <CardHeader>
            <div className="flex items-start justify-between">
              <div className="flex-1">
                <CardTitle className="text-xl">{property.name}</CardTitle>
                <CardDescription className="mt-1">{property.address}</CardDescription>
              </div>
              <Badge variant="secondary" className="ml-2">
                <Building2 className="h-3 w-3 mr-1" />
                {property.year}
              </Badge>
            </div>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              <div className="flex items-center justify-between p-3 bg-primary/5 rounded-lg">
                <div className="flex items-center gap-2">
                  <DollarSign className="h-4 w-4 text-primary" />
                  <span className="text-sm text-muted-foreground">Precio</span>
                </div>
                <span className="font-bold text-lg">
                  ${property.price.toLocaleString('es-CO')}
                </span>
              </div>
              
              <div className="flex items-center gap-2 text-sm">
                <Calendar className="h-4 w-4 text-muted-foreground" />
                <span className="text-muted-foreground">Año de construcción:</span>
                <span className="font-medium">{property.year}</span>
              </div>
              
              <div className="flex items-center gap-2 text-sm">
                <Hash className="h-4 w-4 text-muted-foreground" />
                <span className="text-muted-foreground">Código interno:</span>
                <Badge variant="outline" className="font-mono text-xs">
                  {property.codeInternal}
                </Badge>
              </div>
            </div>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
