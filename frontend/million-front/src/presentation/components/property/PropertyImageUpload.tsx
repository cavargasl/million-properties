'use client';

import { useState, useCallback } from 'react';
import { Upload, X, ImageIcon, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { useCreatePropertyImagesBulk } from '@/presentation/hooks';
import { toast } from 'sonner';

interface PropertyImageUploadProps {
  propertyId: string;
  onClose?: () => void;
}

interface RandomImage {
  id: string;
  url: string;
}

export function PropertyImageUpload({ propertyId, onClose }: PropertyImageUploadProps) {
  const [images, setImages] = useState<RandomImage[]>([]);
  const [isLoadingImages, setIsLoadingImages] = useState(false);
  const createImagesBulk = useCreatePropertyImagesBulk();

  // Función para obtener 5 imágenes aleatorias de Picsum (API pública y gratuita)
  const fetchRandomImages = useCallback(async () => {
    setIsLoadingImages(true);
    try {
      const imageCount = 5;
      const width = 800;
      const height = 600;
      
      const newImages: RandomImage[] = Array.from({ length: imageCount }, (_, index) => ({
        id: `${Date.now()}-${index}`,
        // Agregamos un timestamp para evitar caché y obtener diferentes imágenes
        url: `https://picsum.photos/${width}/${height}?random=${Date.now()}-${index}`,
      }));

      setImages(newImages);
      toast.success(`${imageCount} imágenes aleatorias obtenidas`);
    } catch (error: any) {
      toast.error('Error al obtener imágenes aleatorias');
    } finally {
      setIsLoadingImages(false);
    }
  }, []);

  const handleRemoveImage = useCallback((id: string) => {
    setImages((prev) => prev.filter((img) => img.id !== id));
  }, []);

  const handleUploadAll = async () => {
    if (images.length === 0) return;

    try {
      const imagesToUpload = images.map((img) => ({
        fileUrl: img.url,
        enabled: true,
      }));

      await createImagesBulk.mutateAsync({
        propertyId,
        images: imagesToUpload,
      });

      toast.success(`${images.length} imágenes subidas exitosamente`);
      onClose?.();
      setImages([]);
    } catch (error: any) {
      toast.error(error?.message || 'Error al subir las imágenes');
    }
  };

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-2">
        <Button
          type="button"
          variant="outline"
          onClick={fetchRandomImages}
          disabled={isLoadingImages || createImagesBulk.isPending}
        >
          {isLoadingImages ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Obteniendo imágenes...
            </>
          ) : (
            <>
              <ImageIcon className="mr-2 h-4 w-4" />
              Obtener 5 imágenes aleatorias
            </>
          )}
        </Button>

        {images.length > 0 && (
          <Button
            type="button"
            onClick={handleUploadAll}
            disabled={createImagesBulk.isPending}
            size="sm"
          >
            {createImagesBulk.isPending ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Subiendo...
              </>
            ) : (
              <>
                <Upload className="mr-2 h-4 w-4" />
                Subir todas ({images.length})
              </>
            )}
          </Button>
        )}
      </div>

      {images.length > 0 && (
        <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
          {images.map((image) => (
            <div
              key={image.id}
              className="relative aspect-video rounded-lg border bg-muted overflow-hidden group"
            >
              <img
                src={image.url}
                alt="Random property"
                className="w-full h-full object-cover"
              />

              {createImagesBulk.isPending && (
                <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
                  <Loader2 className="h-8 w-8 text-white animate-spin" />
                </div>
              )}

              <div className="absolute inset-0 bg-black/0 group-hover:bg-black/40 transition-colors flex items-end p-2">
                <div className="w-full flex items-center justify-between opacity-0 group-hover:opacity-100 transition-opacity">
                  <span className="text-xs text-white truncate flex-1">
                    Imagen aleatoria
                  </span>
                  {!createImagesBulk.isPending && (
                    <Button
                      type="button"
                      variant="destructive"
                      size="icon"
                      className="h-6 w-6 ml-2 flex-shrink-0"
                      onClick={() => handleRemoveImage(image.id)}
                    >
                      <X className="h-3 w-3" />
                    </Button>
                  )}
                </div>
              </div>
            </div>
          ))}
        </div>
      )}

      {images.length === 0 && (
        <div className="border-2 border-dashed rounded-lg p-8 text-center text-muted-foreground">
          <ImageIcon className="h-12 w-12 mx-auto mb-2 opacity-50" />
          <p className="text-sm">No hay imágenes seleccionadas</p>
          <p className="text-xs mt-1">Haga clic en el botón para obtener imágenes aleatorias</p>
          <p className="text-xs mt-1 text-muted-foreground/80">
            Las imágenes se obtienen de Picsum Photos (API pública)
          </p>
        </div>
      )}
    </div>
  );
}
