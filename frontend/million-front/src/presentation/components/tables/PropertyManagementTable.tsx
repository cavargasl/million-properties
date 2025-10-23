/**
 * PropertyManagementTable component
 * Displays properties in a table with edit and delete actions and pagination
 */

'use client';

import { useState } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import { Pencil, Trash2, Eye, ChevronLeft, ChevronRight, ImagePlus } from 'lucide-react';
import type { Property } from '@/core/property';
import { useDeleteProperty } from '@/presentation/hooks/useProperties';
import { PropertyImageUpload } from '@/presentation/components/property/PropertyImageUpload';
import { toast } from 'sonner';
import { PaginationMetadata } from '@/core/shared';

interface PropertyManagementTableProps {
  properties: Property[];
  pagination?: PaginationMetadata;
  onEdit: (property: Property) => void;
  onView?: (property: Property) => void;
  onPageChange?: (page: number) => void;
}

export function PropertyManagementTable({
  properties,
  pagination,
  onEdit,
  onView,
  onPageChange,
}: PropertyManagementTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [propertyToDelete, setPropertyToDelete] = useState<Property | null>(null);
  const [imagesDialogOpen, setImagesDialogOpen] = useState(false);
  const [propertyForImages, setPropertyForImages] = useState<Property | null>(null);

  const deleteMutation = useDeleteProperty();

  const handleDeleteClick = (property: Property) => {
    setPropertyToDelete(property);
    setDeleteDialogOpen(true);
  };

  const handleImagesClick = (property: Property) => {
    setPropertyForImages(property);
    setImagesDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!propertyToDelete) return;

    try {
      await deleteMutation.mutateAsync(propertyToDelete.id);
      toast.success('Propiedad eliminada exitosamente');
      setDeleteDialogOpen(false);
      setPropertyToDelete(null);
    } catch (error: any) {
      toast.error(error?.message || 'Error al eliminar la propiedad');
    }
  };

  const handlePreviousPage = () => {
    if (pagination?.hasPreviousPage && onPageChange) {
      onPageChange(pagination.pageNumber - 1);
    }
  };

  const handleNextPage = () => {
    if (pagination?.hasNextPage && onPageChange) {
      onPageChange(pagination.pageNumber + 1);
    }
  };

  const handlePageClick = (page: number) => {
    if (onPageChange) {
      onPageChange(page);
    }
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat('es-CO', {
      style: 'currency',
      currency: 'COP',
      minimumFractionDigits: 0,
    }).format(price);
  };

  if (properties.length === 0) {
    return (
      <div className="text-center py-12 border rounded-lg bg-gray-50">
        <div className="text-6xl opacity-20 mb-4">🏠</div>
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          No hay propiedades registradas
        </h3>
        <p className="text-sm text-gray-500">
          Comienza agregando una nueva propiedad usando el botón "Crear Propiedad"
        </p>
      </div>
    );
  }

  return (
    <>
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Nombre</TableHead>
              <TableHead>Dirección</TableHead>
              <TableHead>Precio</TableHead>
              <TableHead>Código</TableHead>
              <TableHead>Año</TableHead>
              <TableHead>Propietario</TableHead>
              <TableHead className="text-right">Acciones</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {properties.map((property) => (
              <TableRow key={property.id}>
                <TableCell className="font-medium">{property.name}</TableCell>
                <TableCell className="max-w-[200px] truncate" title={property.address}>
                  {property.address}
                </TableCell>
                <TableCell className="font-semibold text-green-600">
                  {formatPrice(property.price)}
                </TableCell>
                <TableCell>
                  <Badge variant="outline">{property.codeInternal}</Badge>
                </TableCell>
                <TableCell>
                  {property.year > 0 ? (
                    <Badge variant="secondary">{property.year}</Badge>
                  ) : (
                    <span className="text-gray-400">-</span>
                  )}
                </TableCell>
                <TableCell className="max-w-[150px] truncate" title={property.ownerName || property.ownerId}>
                  {property.ownerName || property.ownerId}
                </TableCell>
                <TableCell className="text-right">
                  <div className="flex justify-end gap-2">
                    {onView && (
                      <Button
                        variant="ghost"
                        size="icon"
                        onClick={() => onView(property)}
                        title="Ver detalles"
                      >
                        <Eye className="h-4 w-4" />
                      </Button>
                    )}
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => handleImagesClick(property)}
                      title="Gestionar imágenes"
                      className="text-blue-600 hover:text-blue-700 hover:bg-blue-50"
                    >
                      <ImagePlus className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => onEdit(property)}
                      title="Editar propiedad"
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                    <Button
                      variant="ghost"
                      size="icon"
                      onClick={() => handleDeleteClick(property)}
                      title="Eliminar propiedad"
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

      {/* Pagination Controls */}
      {pagination && pagination.totalPages > 1 && (
        <div className="flex flex-col sm:flex-row items-center justify-between gap-4 pt-4 px-2">
          <div className="text-sm text-gray-600">
            Mostrando{' '}
            <span className="font-semibold">
              {(pagination.pageNumber - 1) * pagination.pageSize + 1} -{' '}
              {Math.min(pagination.pageNumber * pagination.pageSize, pagination.totalRecords)}
            </span>{' '}
            de <span className="font-semibold">{pagination.totalRecords}</span> propiedades
          </div>

          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handlePreviousPage}
              disabled={!pagination.hasPreviousPage}
            >
              <ChevronLeft className="h-4 w-4" />
              Anterior
            </Button>

            {/* Page numbers */}
            <div className="flex gap-1">
              {Array.from({ length: pagination.totalPages }, (_, i) => i + 1).map((page) => {
                // Show first page, last page, current page, and pages around current
                const showPage =
                  page === 1 ||
                  page === pagination.totalPages ||
                  (page >= pagination.pageNumber - 1 && page <= pagination.pageNumber + 1);

                // Show ellipsis
                const showEllipsis =
                  (page === 2 && pagination.pageNumber > 3) ||
                  (page === pagination.totalPages - 1 &&
                    pagination.pageNumber < pagination.totalPages - 2);

                if (showEllipsis) {
                  return (
                    <span key={page} className="px-3 py-1 text-gray-400">
                      ...
                    </span>
                  );
                }

                if (!showPage) return null;

                return (
                  <Button
                    key={page}
                    variant={page === pagination.pageNumber ? 'default' : 'outline'}
                    size="sm"
                    onClick={() => handlePageClick(page)}
                  >
                    {page}
                  </Button>
                );
              })}
            </div>

            <Button
              variant="outline"
              size="sm"
              onClick={handleNextPage}
              disabled={!pagination.hasNextPage}
            >
              Siguiente
              <ChevronRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      )}

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>¿Está seguro?</DialogTitle>
            <DialogDescription>
              Esta acción no se puede deshacer. Se eliminará permanentemente la propiedad{' '}
              <strong>{propertyToDelete?.name}</strong> del sistema.
            </DialogDescription>
          </DialogHeader>
          <DialogFooter className="gap-2">
            <Button
              variant="outline"
              onClick={() => {
                setPropertyToDelete(null);
                setDeleteDialogOpen(false);
              }}
              disabled={deleteMutation.isPending}
            >
              Cancelar
            </Button>
            <Button
              onClick={handleDeleteConfirm}
              className="bg-red-600 hover:bg-red-700"
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? 'Eliminando...' : 'Eliminar'}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Images Management Dialog */}
      <Dialog open={imagesDialogOpen} onOpenChange={setImagesDialogOpen} modal>
        <DialogContent className='overflow-y-auto max-h-[80vh] w-auto sm:max-w-[90vw]'>
          <DialogHeader>
            <DialogTitle>Gestionar Imágenes</DialogTitle>
            <DialogDescription>
              Suba y administre las imágenes de <strong>{propertyForImages?.name}</strong>
            </DialogDescription>
          </DialogHeader>
          {propertyForImages && (
            <PropertyImageUpload
              propertyId={propertyForImages.id}
              onClose={() => setImagesDialogOpen(false)}
            />
          )}
          <DialogFooter>
            <Button onClick={() => setImagesDialogOpen(false)}>Cerrar</Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  );
}
