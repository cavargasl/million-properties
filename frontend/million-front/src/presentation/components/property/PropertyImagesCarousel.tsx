'use client';

import { useEffect, useState } from 'react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
  type CarouselApi,
} from '@/components/ui/carousel';
import { Skeleton } from '@/components/ui/skeleton';
import { Badge } from '@/components/ui/badge';
import { usePropertyImages } from '@/presentation/hooks';
import type { Property } from '@/core/property';
import { AlertCircle, ImageIcon } from 'lucide-react';

interface PropertyImagesCarouselProps {
  property: Property | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function PropertyImagesCarousel({
  property,
  open,
  onOpenChange,
}: PropertyImagesCarouselProps) {
  const [api, setApi] = useState<CarouselApi>();
  const [current, setCurrent] = useState(0);
  const [count, setCount] = useState(0);

  const { data: images, isLoading } = usePropertyImages(property?.id || '');

  // Filtrar solo las imágenes habilitadas
  const enabledImages = images?.filter((img) => img.enabled) || [];

  useEffect(() => {
    if (!api) return;

    setCount(api.scrollSnapList().length);
    setCurrent(api.selectedScrollSnap() + 1);

    api.on('select', () => {
      setCurrent(api.selectedScrollSnap() + 1);
    });
  }, [api]);

  // Reset cuando se abre el diálogo
  useEffect(() => {
    if (open && api) {
      api.scrollTo(0);
    }
  }, [open, api]);

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="w-[95vw] sm:max-w-auto lg:max-w-[900px] h-auto">
        <DialogHeader>
          <DialogTitle>{property?.name || 'Propiedad'}</DialogTitle>
          <DialogDescription>
            {property?.address || 'Galería de imágenes'}
          </DialogDescription>
        </DialogHeader>

        {isLoading ? (
          <div className="space-y-4">
            <Skeleton className="w-full h-full rounded-lg" />
            <div className="flex justify-center gap-2">
              <Skeleton className="h-4 w-20" />
            </div>
          </div>
        ) : enabledImages.length > 0 ? (
          <div className="relative">
            <Carousel setApi={setApi}>
              <CarouselContent>
                {enabledImages.map((image, index) => (
                  <CarouselItem key={image.id}>
                    <div className="relative aspect-auto w-full overflow-hidden rounded-lg">
                      <img
                        src={image.file}
                        alt={`${property?.name} - Imagen ${index + 1}`}
                        className="w-full object-contain"
                        onError={(e) => {
                          const target = e.target as HTMLImageElement;
                          target.src =
                            'https://via.placeholder.com/800x600?text=Error+cargando+imagen';
                        }}
                      />
                      <div className="absolute bottom-2 right-2">
                        <Badge variant="secondary" className="bg-black/50 text-white">
                          {index + 1} / {enabledImages.length}
                        </Badge>
                      </div>
                    </div>
                  </CarouselItem>
                ))}
              </CarouselContent>
              {enabledImages.length > 1 && (
                <>
                  <CarouselPrevious className="left-0" />
                  <CarouselNext className="right-0" />
                </>
              )}
            </Carousel>

            {enabledImages.length > 1 && (
              <div className="py-2 text-center text-sm text-muted-foreground">
                Imagen {current} de {count}
              </div>
            )}
          </div>
        ) : (
          <div className="flex flex-col items-center justify-center py-12 text-center">
            <AlertCircle className="h-12 w-12 text-muted-foreground mb-4" />
            <p className="text-lg font-medium text-muted-foreground">
              No hay imágenes disponibles
            </p>
            <p className="text-sm text-muted-foreground mt-2">
              Esta propiedad no tiene imágenes habilitadas para mostrar.
            </p>
          </div>
        )}
      </DialogContent>
    </Dialog>
  );
}
