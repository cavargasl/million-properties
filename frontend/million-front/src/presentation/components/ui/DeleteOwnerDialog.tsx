/**
 * DeleteOwnerDialog component for confirming owner deletion
 */

"use client";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import type { Owner } from "@/core/owner";
import { AlertTriangle, Loader2 } from "lucide-react";

interface DeleteOwnerDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  owner: Owner | null;
  onConfirm: () => void;
  isDeleting: boolean;
}

export function DeleteOwnerDialog({
  open,
  onOpenChange,
  owner,
  onConfirm,
  isDeleting,
}: DeleteOwnerDialogProps) {
  if (!owner) return null;

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <AlertTriangle className="h-5 w-5 text-red-500" />
            Confirmar Eliminación
          </DialogTitle>
          <DialogDescription>
            Esta acción no se puede deshacer.
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-4">
          <Alert variant="destructive" className="border-red-200 bg-red-50">
            <AlertTitle>¿Estás seguro de que deseas eliminar?</AlertTitle>
            <AlertDescription>{owner.name}</AlertDescription>
          </Alert>

          <div className="rounded-lg bg-gray-50 p-4 space-y-2">
            <div className="text-sm">
              <span className="font-medium text-gray-700">Nombre:</span>{" "}
              <span className="text-gray-900">{owner.name}</span>
            </div>
            <div className="text-sm">
              <span className="font-medium text-gray-700">Dirección:</span>{" "}
              <span className="text-gray-900">{owner.address}</span>
            </div>
          </div>

          <p className="text-sm text-gray-600">
            Se eliminará toda la información asociada a este propietario.
          </p>
        </div>

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={isDeleting}
          >
            Cancelar
          </Button>
          <Button
            type="button"
            variant="destructive"
            onClick={onConfirm}
            disabled={isDeleting}
          >
            {isDeleting && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
            Eliminar
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
