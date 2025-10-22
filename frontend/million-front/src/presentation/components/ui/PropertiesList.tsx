"use client";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Skeleton } from "@/components/ui/skeleton";
import type { PropertyFilters } from "@/core/property";
import { usePropertiesPaginated } from "@/presentation/hooks/useProperties";
import { ChevronLeft, ChevronRight, Search } from "lucide-react";
import { useState } from "react";
import { PropertyCard } from "./PropertyCard";

const PAGE_SIZE = 9;

export function PropertiesList() {
  const [currentPage, setCurrentPage] = useState(1);
  const [filters, setFilters] = useState<PropertyFilters>({});
  const [searchName, setSearchName] = useState("");
  const [searchAddress, setSearchAddress] = useState("");

  const { data, isLoading, isError, error } = usePropertiesPaginated(filters, {
    pageNumber: currentPage,
    pageSize: PAGE_SIZE,
  });

  const handleSearch = () => {
    setFilters({
      ...filters,
      name: searchName || undefined,
      address: searchAddress || undefined,
    });
    setCurrentPage(1); // Reset to first page on new search
  };

  const handleClearFilters = () => {
    setSearchName("");
    setSearchAddress("");
    setFilters({});
    setCurrentPage(1);
  };

  const handleNextPage = () => {
    if (data?.pagination.hasNextPage) {
      setCurrentPage((prev) => prev + 1);
    }
  };

  const handlePreviousPage = () => {
    if (data?.pagination.hasPreviousPage) {
      setCurrentPage((prev) => prev - 1);
    }
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
  };

  if (isError) {
    return (
      <Card className="border-red-200 bg-red-50">
        <CardHeader>
          <CardTitle className="text-red-800">Error</CardTitle>
          <CardDescription className="text-red-600">
            {error?.message || "Error al cargar las propiedades"}
          </CardDescription>
        </CardHeader>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      {/* Filters Section */}
      <Card>
        <CardHeader>
          <CardTitle>Buscar Propiedades</CardTitle>
          <CardDescription>
            Filtra las propiedades por nombre o dirección
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            <Input
              placeholder="Buscar por nombre..."
              value={searchName}
              onChange={(e) => setSearchName(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
            />
            <Input
              placeholder="Buscar por dirección..."
              value={searchAddress}
              onChange={(e) => setSearchAddress(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
            />
            <div className="flex gap-2">
              <Button onClick={handleSearch} className="flex-1">
                <Search className="mr-2 h-4 w-4" />
                Buscar
              </Button>
              <Button onClick={handleClearFilters} variant="outline">
                Limpiar
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Properties Grid */}
      {isLoading ? (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {Array.from({ length: PAGE_SIZE }).map((_, index) => (
            <Card key={index}>
              <Skeleton className="h-48 w-full rounded-t-lg" />
              <CardHeader>
                <Skeleton className="h-6 w-3/4" />
                <Skeleton className="h-4 w-full" />
              </CardHeader>
              <CardContent>
                <div className="space-y-2">
                  <Skeleton className="h-4 w-full" />
                  <Skeleton className="h-4 w-full" />
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      ) : data?.items && data.items.length > 0 ? (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {data.items.map((property) => (
              <PropertyCard key={property.id} property={property} />
            ))}
          </div>

          {/* Pagination Controls */}
          <div className="flex flex-col sm:flex-row items-center justify-between gap-4 pt-4">
            <div className="text-sm text-gray-600">
              Mostrando{" "}
              <span className="font-semibold">
                {(currentPage - 1) * PAGE_SIZE + 1} -{" "}
                {Math.min(
                  currentPage * PAGE_SIZE,
                  data.pagination.totalRecords
                )}
              </span>{" "}
              de{" "}
              <span className="font-semibold">
                {data.pagination.totalRecords}
              </span>{" "}
              propiedades
            </div>

            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={handlePreviousPage}
                disabled={!data.pagination.hasPreviousPage}
              >
                <ChevronLeft className="h-4 w-4" />
                Anterior
              </Button>

              {/* Page numbers */}
              <div className="flex gap-1">
                {Array.from(
                  { length: data.pagination.totalPages },
                  (_, i) => i + 1
                ).map((page) => {
                  // Show first page, last page, current page, and pages around current
                  const showPage =
                    page === 1 ||
                    page === data.pagination.totalPages ||
                    (page >= currentPage - 1 && page <= currentPage + 1);

                  // Show ellipsis
                  const showEllipsis =
                    (page === 2 && currentPage > 3) ||
                    (page === data.pagination.totalPages - 1 &&
                      currentPage < data.pagination.totalPages - 2);

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
                      variant={page === currentPage ? "default" : "outline"}
                      size="sm"
                      onClick={() => handlePageChange(page)}
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
                disabled={!data.pagination.hasNextPage}
              >
                Siguiente
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </>
      ) : (
        <Card>
          <CardContent className="py-12 text-center">
            <div className="flex flex-col items-center justify-center space-y-4">
              <div className="text-6xl opacity-20">🏠</div>
              <div>
                <h3 className="text-lg font-semibold">
                  No se encontraron propiedades
                </h3>
                <p className="text-sm text-gray-500">
                  Intenta ajustar los filtros de búsqueda
                </p>
              </div>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  );
}
