'use client';

import { useOwners } from '@/presentation/hooks';
import { Users, Mail, Phone, Loader2, AlertCircle, UserCircle } from 'lucide-react';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';

export function OwnersTable() {
  const { data: owners, isLoading, error } = useOwners();

  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <Skeleton className="h-6 w-1/3" />
          <Skeleton className="h-4 w-1/2" />
        </CardHeader>
        <CardContent>
          <div className="space-y-2">
            {[...Array(5)].map((_, i) => (
              <Skeleton key={i} className="h-16 w-full" />
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  if (error) {
    return (
      <Alert variant="destructive">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Error al cargar propietarios</AlertTitle>
        <AlertDescription>{error.message}</AlertDescription>
      </Alert>
    );
  }

  if (!owners || owners.length === 0) {
    return (
      <Card>
        <CardContent className="text-center p-12">
          <Users className="h-12 w-12 mx-auto mb-4 opacity-50 text-muted-foreground" />
          <CardTitle className="mb-2">No hay propietarios registrados</CardTitle>
          <CardDescription>
            Aún no se han registrado propietarios en el sistema
          </CardDescription>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center gap-2">
          <Users className="h-5 w-5 text-primary" />
          <CardTitle>Propietarios</CardTitle>
        </div>
        <CardDescription>
          Lista de todos los propietarios registrados en el sistema
        </CardDescription>
      </CardHeader>
      <CardContent>
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nombre</TableHead>
              <TableHead>Dirección</TableHead>
              <TableHead>Contacto</TableHead>
              <TableHead>Fecha de Nacimiento</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {owners.map((owner) => (
              <TableRow key={owner.id}>
                <TableCell>
                  <div className="flex items-center gap-2">
                    <UserCircle className="h-4 w-4 text-muted-foreground" />
                    <span className="font-medium">{owner.name}</span>
                  </div>
                </TableCell>
                <TableCell>
                  <span className="text-sm text-muted-foreground">{owner.address}</span>
                </TableCell>
                <TableCell>
                  <div className="flex flex-col gap-1">
                    {owner.photo && (
                      <Badge variant="outline" className="w-fit gap-1">
                        <Mail className="h-3 w-3" />
                        <span className="text-xs">{owner.photo}</span>
                      </Badge>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <span className="text-sm">
                    {new Date(owner.birthday).toLocaleDateString('es-CO', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                    })}
                  </span>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </CardContent>
    </Card>
  );
}
