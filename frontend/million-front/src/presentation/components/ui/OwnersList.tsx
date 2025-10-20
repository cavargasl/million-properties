/**
 * OwnersList component with CRUD operations
 */

'use client';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { Skeleton } from '@/components/ui/skeleton';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { useOwners, useDeleteOwner } from '@/presentation/hooks/useOwners';
import type { Owner } from '@/core/owner';
import { Pencil, Trash2, Plus, Search, AlertCircle, User } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { OwnerDialog } from './OwnerDialog';
import { DeleteOwnerDialog } from './DeleteOwnerDialog';
import { toast } from 'sonner';

export function OwnersList() {
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedOwner, setSelectedOwner] = useState<Owner | null>(null);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [ownerToDelete, setOwnerToDelete] = useState<Owner | null>(null);

  const { data: owners, isLoading, isError, error } = useOwners();
  const deleteMutation = useDeleteOwner();

  // Filter owners based on search query
  const filteredOwners = owners?.filter(
    (owner) =>
      owner.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      owner.address.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const handleCreateOwner = () => {
    setSelectedOwner(null);
    setIsDialogOpen(true);
  };

  const handleEditOwner = (owner: Owner) => {
    setSelectedOwner(owner);
    setIsDialogOpen(true);
  };

  const handleDeleteClick = (owner: Owner) => {
    setOwnerToDelete(owner);
    setIsDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!ownerToDelete) return;

    try {
      await deleteMutation.mutateAsync(ownerToDelete.id);
      toast.success('Propietario eliminado exitosamente');
      setIsDeleteDialogOpen(false);
      setOwnerToDelete(null);
    } catch (error: any) {
      console.error('Error deleting owner:', error);
      // El transformError de axios ya procesó el error y extrajo el mensaje apropiado
      toast.error(error?.message || 'Error al eliminar el propietario. Por favor, intenta de nuevo.');
    }
  };

  const formatDate = (dateString: string) => {
    try {
      return new Date(dateString).toLocaleDateString('es-ES', {
        year: 'numeric',
        month: 'long',
        day: 'numeric',
      });
    } catch {
      return dateString;
    }
  };

  const calculateAge = (birthday: string) => {
    const birthDate = new Date(birthday);
    const today = new Date();
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    
    return age;
  };

  if (isError) {
    return (
      <Alert variant="destructive" className="border-red-200 bg-red-50">
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Error</AlertTitle>
        <AlertDescription>
          {error?.message || 'Error al cargar los propietarios'}
        </AlertDescription>
      </Alert>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header with Search and Add Button */}
      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div>
              <CardTitle>Lista de Propietarios</CardTitle>
              <CardDescription>
                Gestiona la información de los propietarios
              </CardDescription>
            </div>
            <Button onClick={handleCreateOwner}>
              <Plus className="mr-2 h-4 w-4" />
              Nuevo Propietario
            </Button>
          </div>
        </CardHeader>
        <CardContent>
          <div className="flex gap-4">
            <div className="relative flex-1">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
              <Input
                placeholder="Buscar por nombre o dirección..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="pl-10"
              />
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Owners Table */}
      <Card>
        <CardContent className="p-0">
          {isLoading ? (
            <div className="p-6 space-y-4">
              {Array.from({ length: 5 }).map((_, index) => (
                <div key={index} className="flex items-center space-x-4">
                  <Skeleton className="h-12 w-12 rounded-full" />
                  <div className="space-y-2 flex-1">
                    <Skeleton className="h-4 w-[250px]" />
                    <Skeleton className="h-4 w-[200px]" />
                  </div>
                </div>
              ))}
            </div>
          ) : filteredOwners && filteredOwners.length > 0 ? (
            <div className="overflow-x-auto">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Foto</TableHead>
                    <TableHead>Nombre</TableHead>
                    <TableHead>Dirección</TableHead>
                    <TableHead>Fecha de Nacimiento</TableHead>
                    <TableHead>Edad</TableHead>
                    <TableHead className="text-right">Acciones</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredOwners.map((owner) => (
                    <TableRow key={owner.id}>
                      <TableCell>
                        {owner.photo ? (
                          <img
                            src={owner.photo}
                            alt={owner.name}
                            className="h-10 w-10 rounded-full object-cover"
                          />
                        ) : (
                          <div className="h-10 w-10 rounded-full bg-gray-200 flex items-center justify-center">
                            <User className="h-5 w-5 text-gray-500" />
                          </div>
                        )}
                      </TableCell>
                      <TableCell className="font-medium">{owner.name}</TableCell>
                      <TableCell className="text-gray-600">{owner.address}</TableCell>
                      <TableCell>{formatDate(owner.birthday)}</TableCell>
                      <TableCell>
                        <Badge variant="outline">{calculateAge(owner.birthday)} años</Badge>
                      </TableCell>
                      <TableCell className="text-right">
                        <div className="flex justify-end gap-2">
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleEditOwner(owner)}
                          >
                            <Pencil className="h-4 w-4" />
                          </Button>
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleDeleteClick(owner)}
                            className="text-red-600 hover:text-red-700 hover:bg-red-50"
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          ) : (
            <div className="p-12 text-center">
              <User className="mx-auto h-12 w-12 text-gray-400" />
              <h3 className="mt-4 text-lg font-semibold text-gray-900">
                No se encontraron propietarios
              </h3>
              <p className="mt-2 text-sm text-gray-600">
                {searchQuery
                  ? 'Intenta con otros términos de búsqueda'
                  : 'Comienza agregando un nuevo propietario'}
              </p>
              {!searchQuery && (
                <Button onClick={handleCreateOwner} className="mt-4">
                  <Plus className="mr-2 h-4 w-4" />
                  Crear Propietario
                </Button>
              )}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Summary Card */}
      {filteredOwners && filteredOwners.length > 0 && (
        <Card>
          <CardContent className="pt-6">
            <div className="flex items-center justify-between">
              <div className="text-sm text-gray-600">
                Total de propietarios:{' '}
                <span className="font-semibold text-gray-900">
                  {filteredOwners.length}
                </span>
                {searchQuery && owners && filteredOwners.length !== owners.length && (
                  <span className="text-gray-500"> (de {owners.length})</span>
                )}
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Dialogs */}
      <OwnerDialog
        open={isDialogOpen}
        onOpenChange={setIsDialogOpen}
        owner={selectedOwner}
      />

      <DeleteOwnerDialog
        open={isDeleteDialogOpen}
        onOpenChange={setIsDeleteDialogOpen}
        owner={ownerToDelete}
        onConfirm={handleDeleteConfirm}
        isDeleting={deleteMutation.isPending}
      />
    </div>
  );
}
