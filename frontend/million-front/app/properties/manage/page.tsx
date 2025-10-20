/**
 * Property Management Page
 * CRUD interface for managing properties with pagination
 */

'use client';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { Plus } from 'lucide-react';
import { PropertyDialog } from '@/presentation/components/dialogs/PropertyDialog';
import { PropertyManagementTable } from '@/presentation/components/tables/PropertyManagementTable';
import { usePropertiesPaginated } from '@/presentation/hooks/useProperties';
import type { Property } from '@/core/property';

const PAGE_SIZE = 10; // Properties per page

export default function PropertyManagementPage() {
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogMode, setDialogMode] = useState<'create' | 'edit'>('create');
  const [selectedProperty, setSelectedProperty] = useState<Property | null>(null);
  const [currentPage, setCurrentPage] = useState(1);

  const { data, isLoading, isError, error } = usePropertiesPaginated(undefined, {
    pageNumber: currentPage,
    pageSize: PAGE_SIZE,
  });

  const properties = data?.items || [];
  const pagination = data?.pagination;

  const handleCreateClick = () => {
    setSelectedProperty(null);
    setDialogMode('create');
    setDialogOpen(true);
  };

  const handleEditClick = (property: Property) => {
    setSelectedProperty(property);
    setDialogMode('edit');
    setDialogOpen(true);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setSelectedProperty(null);
    // Reset to first page after creating a new property
    if (dialogMode === 'create') {
      setCurrentPage(1);
    }
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  // Calculate statistics from current page data
  const totalValue = properties.reduce((sum, p) => sum + p.price, 0);
  const averagePrice = properties.length > 0 ? totalValue / properties.length : 0;

  return (
    <div className="flex flex-col min-h-[calc(100vh-4rem)]">
      <main className="flex-1">
        <div className="container max-w-screen-2xl mx-auto py-8">
          {/* Page Header */}
          <div className="flex justify-between items-start mb-8">
            <div>
              <h1 className="text-4xl font-bold text-gray-900 mb-2">
                Gestión de Propiedades
              </h1>
              <p className="text-lg text-gray-600">
                Administra el inventario completo de propiedades del sistema
              </p>
            </div>
            <Button onClick={handleCreateClick} size="lg">
              <Plus className="mr-2 h-5 w-5" />
              Crear Propiedad
            </Button>
          </div>

          {/* Statistics Cards */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Total de Propiedades</CardDescription>
                <CardTitle className="text-3xl">
                  {isLoading ? (
                    <Skeleton className="h-9 w-16" />
                  ) : (
                    pagination?.totalRecords || 0
                  )}
                </CardTitle>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Propiedades en Página Actual</CardDescription>
                <CardTitle className="text-3xl">
                  {isLoading ? (
                    <Skeleton className="h-9 w-16" />
                  ) : (
                    properties.length
                  )}
                </CardTitle>
              </CardHeader>
            </Card>

            <Card>
              <CardHeader className="pb-3">
                <CardDescription>Total de Páginas</CardDescription>
                <CardTitle className="text-3xl">
                  {isLoading ? (
                    <Skeleton className="h-9 w-16" />
                  ) : (
                    pagination?.totalPages || 0
                  )}
                </CardTitle>
              </CardHeader>
            </Card>
          </div>

          {/* Properties Table */}
          <Card>
            <CardHeader>
              <CardTitle>Lista de Propiedades</CardTitle>
              <CardDescription>
                Visualiza, edita y elimina propiedades existentes
              </CardDescription>
            </CardHeader>
            <CardContent>
              {isLoading ? (
                <div className="space-y-3">
                  {[...Array(5)].map((_, i) => (
                    <Skeleton key={i} className="h-16 w-full" />
                  ))}
                </div>
              ) : isError ? (
                <Card className="border-red-200 bg-red-50">
                  <CardContent className="py-6">
                    <div className="text-center text-red-800">
                      <p className="font-semibold mb-2">Error al cargar las propiedades</p>
                      <p className="text-sm text-red-600">
                        {error?.message || 'Ocurrió un error inesperado'}
                      </p>
                    </div>
                  </CardContent>
                </Card>
              ) : (
                <PropertyManagementTable
                  properties={properties}
                  pagination={pagination}
                  onEdit={handleEditClick}
                  onPageChange={handlePageChange}
                />
              )}
            </CardContent>
          </Card>
        </div>
      </main>

      {/* Property Dialog */}
      <PropertyDialog
        open={dialogOpen}
        onOpenChange={handleDialogClose}
        property={selectedProperty}
        mode={dialogMode}
      />
    </div>
  );
}
