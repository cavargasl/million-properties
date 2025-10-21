'use client';

import { useState, useCallback } from 'react';
import { Upload, X, ImageIcon, Loader2, Trash2, AlertCircle, Eye, EyeOff } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  usePropertyImages, 
  useCreatePropertyImagesBulk, 
  useDeletePropertyImage,
  useTogglePropertyImageEnabled
} from '@/presentation/hooks';
import { toast } from 'sonner';
import type { PropertyImage } from '@/core/property';

interface PropertyImageUploadProps {
  propertyId: string;
  onClose?: () => void;
}

interface RandomImage {
  id: string;
  url: string;
}

export function PropertyImageUpload({ propertyId, onClose }: PropertyImageUploadProps) {
  const [newImages, setNewImages] = useState<RandomImage[]>([]);
  const [isLoadingRandomImages, setIsLoadingRandomImages] = useState(false);
  const [imageToDelete, setImageToDelete] = useState<string | null>(null);
  const [imageToToggle, setImageToToggle] = useState<string | null>(null);

  // Hooks para cargar, crear y eliminar imágenes
  const { data: existingImages, isLoading } = usePropertyImages(propertyId);
  const createImagesBulk = useCreatePropertyImagesBulk();
  const deleteImageMutation = useDeletePropertyImage();
  const toggleEnabledMutation = useTogglePropertyImageEnabled();

  // Función para obtener 5 imágenes aleatorias de Picsum
  const fetchRandomImages = useCallback(async () => {
    setIsLoadingRandomImages(true);
    try {
      const imageCount = 5;
      const width = 800;
      const height = 600;
      
      const randomImages: RandomImage[] = Array.from({ length: imageCount }, (_, index) => ({
        id: `temp-${Date.now()}-${index}`,
        url: `https://picsum.photos/${width}/${height}?random=${Date.now()}-${index}`,
      }));

      setNewImages(randomImages);
      toast.success(`${imageCount} imágenes aleatorias obtenidas`);
    } catch (error: any) {
      toast.error('Error al obtener imágenes aleatorias');
    } finally {
      setIsLoadingRandomImages(false);
    }
  }, []);

  const handleRemoveNewImage = useCallback((id: string) => {
    setNewImages((prev) => prev.filter((img) => img.id !== id));
  }, []);

  const handleDeleteExistingImage = async (imageId: string) => {
    setImageToDelete(imageId);
    try {
      await deleteImageMutation.mutateAsync({propertyId, imageId});
      toast.success('Imagen eliminada exitosamente');
    } catch (error: any) {
      toast.error(error?.message || 'Error al eliminar la imagen');
    } finally {
      setImageToDelete(null);
    }
  };

  const handleToggleEnabled = async (imageId: string, currentEnabled: boolean) => {
    setImageToToggle(imageId);
    const newEnabled = !currentEnabled;
    try {
      await toggleEnabledMutation.mutateAsync({
        propertyId,
        imageId,
        enabled: newEnabled,
      });
      toast.success(`Imagen ${newEnabled ? 'habilitada' : 'deshabilitada'} exitosamente`);
    } catch (error: any) {
      toast.error(error?.message || 'Error al cambiar el estado de la imagen');
    } finally {
      setImageToToggle(null);
    }
  };

  const handleUploadNewImages = async () => {
    if (newImages.length === 0) return;

    try {
      const imagesToUpload = newImages.map((img) => ({
        fileUrl: img.url,
        enabled: true,
      }));

      await createImagesBulk.mutateAsync({
        propertyId,
        images: imagesToUpload,
      });

      toast.success(`${newImages.length} imágenes subidas exitosamente`);
      setNewImages([]);
    } catch (error: any) {
      toast.error(error?.message || 'Error al subir las imágenes');
    }
  };

  return (
    <div className="space-y-6">
      {/* Sección de imágenes existentes */}
      <div>
        <div className="flex items-center justify-between mb-3">
          <h3 className="text-sm font-semibold">Imágenes actuales</h3>
          {existingImages && existingImages.length > 0 && (
            <Badge variant="secondary">{existingImages.length} imagen(es)</Badge>
          )}
        </div>

        {isLoading ? (
          <div className="flex items-center justify-center py-8 border rounded-lg">
            <Loader2 className="h-8 w-8 animate-spin text-gray-400" />
          </div>
        ) : existingImages && existingImages.length > 0 ? (
          <div className="grid grid-cols-[repeat(auto-fill,minmax(250px,1fr))] gap-3">
            {existingImages.map((image: PropertyImage) => (
              <div
                key={image.id}
                className="relative aspect-video rounded-lg border bg-muted overflow-hidden group"
              >
                <img
                  src={image.file}
                  alt="Property"
                  className="w-full h-full object-cover"
                  onError={(e) => {
                    const target = e.target as HTMLImageElement;
                    target.src = 'https://via.placeholder.com/800x600?text=Error+cargando+imagen';
                  }}
                />

                {!image.enabled && (
                  <div className="absolute top-2 right-2">
                    <Badge variant="destructive" className="text-xs">
                      Deshabilitada
                    </Badge>
                  </div>
                )}

                {(imageToDelete === image.id || imageToToggle === image.id) && (
                  <div className="absolute inset-0 bg-black/50 flex items-center justify-center">
                    <Loader2 className="h-8 w-8 text-white animate-spin" />
                  </div>
                )}

                <div className="absolute inset-0 bg-black/0 group-hover:bg-black/50 transition-colors flex items-center justify-center">
                  <div className="flex gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                    <Button
                      type="button"
                      variant={image.enabled ? "secondary" : "default"}
                      size="sm"
                      onClick={() => handleToggleEnabled(image.id, image.enabled)}
                      disabled={imageToDelete === image.id || imageToToggle === image.id}
                      title={image.enabled ? 'Deshabilitar imagen' : 'Habilitar imagen'}
                    >
                      {image.enabled ? (
                        <>
                          <EyeOff className="h-4 w-4 mr-2" />
                          Deshabilitar
                        </>
                      ) : (
                        <>
                          <Eye className="h-4 w-4 mr-2" />
                          Habilitar
                        </>
                      )}
                    </Button>
                    <Button
                      type="button"
                      variant="destructive"
                      size="sm"
                      onClick={() => handleDeleteExistingImage(image.id)}
                      disabled={imageToDelete === image.id || imageToToggle === image.id}
                      title="Eliminar imagen"
                    >
                      <Trash2 className="h-4 w-4 mr-2" />
                      Eliminar
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <Alert>
            <AlertCircle className="h-4 w-4" />
            <AlertDescription>
              Esta propiedad no tiene imágenes. Agregue algunas usando el botón de abajo.
            </AlertDescription>
          </Alert>
        )}
      </div>

      {/* Separador */}
      <div className="relative">
        <div className="absolute inset-0 flex items-center">
          <span className="w-full border-t" />
        </div>
        <div className="relative flex justify-center text-xs uppercase">
          <span className="bg-background px-2 text-muted-foreground">
            Agregar nuevas imágenes
          </span>
        </div>
      </div>

      {/* Sección para agregar nuevas imágenes */}
      <div>
        <div className="flex items-center gap-2 mb-3">
          <Button
            type="button"
            variant="outline"
            onClick={fetchRandomImages}
            disabled={isLoadingRandomImages || createImagesBulk.isPending}
          >
            {isLoadingRandomImages ? (
              <>
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Obteniendo...
              </>
            ) : (
              <>
                <ImageIcon className="mr-2 h-4 w-4" />
                Obtener 5 imágenes aleatorias
              </>
            )}
          </Button>

          {newImages.length > 0 && (
            <Button
              type="button"
              onClick={handleUploadNewImages}
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
                  Subir nuevas ({newImages.length})
                </>
              )}
            </Button>
          )}
        </div>

        {newImages.length > 0 ? (
          <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
            {newImages.map((image) => (
              <div
                key={image.id}
                className="relative aspect-video rounded-lg border border-dashed border-blue-300 bg-blue-50 overflow-hidden group"
              >
                <img
                  src={image.url}
                  alt="Nueva imagen"
                  className="w-full h-full object-cover"
                />

                <div className="absolute top-2 left-2">
                  <Badge className="text-xs bg-blue-600">Nueva</Badge>
                </div>

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
                        onClick={() => handleRemoveNewImage(image.id)}
                      >
                        <X className="h-3 w-3" />
                      </Button>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div className="border-2 border-dashed rounded-lg p-6 text-center text-muted-foreground">
            <ImageIcon className="h-10 w-10 mx-auto mb-2 opacity-50" />
            <p className="text-xs">Haga clic en el botón para obtener imágenes aleatorias</p>
            <p className="text-xs mt-1 text-muted-foreground/80">
              Las imágenes se obtienen de Picsum Photos (API pública)
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
