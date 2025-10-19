/**
 * PropertyCard component to display individual property information
 */

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import type { Property } from '@/core/property';
import Image from 'next/image';

interface PropertyCardProps {
  property: Property;
  onSelect?: (property: Property) => void;
}

export function PropertyCard({ property, onSelect }: PropertyCardProps) {
  const handleClick = () => {
    onSelect?.(property);
  };

  const formattedPrice = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
  }).format(property.price);

  return (
    <Card
      className="cursor-pointer hover:shadow-lg transition-shadow duration-300"
      onClick={handleClick}
    >
      <div className="relative h-48 w-full overflow-hidden rounded-t-lg bg-gray-200">
        {/* Placeholder for property image */}
        <div className="flex items-center justify-center h-full bg-gradient-to-br from-blue-100 to-blue-200">
          <span className="text-4xl text-gray-400">🏠</span>
        </div>
        <Badge className="absolute top-2 right-2" variant="secondary">
          {property.year}
        </Badge>
      </div>
      <CardHeader>
        <CardTitle className="text-xl line-clamp-1">{property.name}</CardTitle>
        <CardDescription className="line-clamp-1">{property.address}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-2">
          <div className="flex justify-between items-center">
            <span className="text-sm text-gray-500">Precio</span>
            <span className="text-lg font-bold text-green-600">{formattedPrice}</span>
          </div>
          <div className="flex justify-between items-center">
            <span className="text-sm text-gray-500">Código</span>
            <Badge variant="outline">{property.codeInternal}</Badge>
          </div>
          {property.ownerName && (
            <div className="flex justify-between items-center">
              <span className="text-sm text-gray-500">Propietario</span>
              <span className="text-sm font-medium">{property.ownerName}</span>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
