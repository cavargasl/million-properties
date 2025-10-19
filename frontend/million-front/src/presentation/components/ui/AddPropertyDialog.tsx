'use client';

import { useState } from 'react';
import { Plus, Building2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';

export function AddPropertyDialog() {
  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    address: '',
    price: '',
    codeInternal: '',
    year: '',
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Aquí iría la lógica para crear la propiedad
    console.log('Crear propiedad:', formData);
    setOpen(false);
    // Reset form
    setFormData({
      name: '',
      address: '',
      price: '',
      codeInternal: '',
      year: '',
    });
  };

  const handleChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button>
          <Plus className="mr-2 h-4 w-4" />
          Agregar Propiedad
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[525px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Building2 className="h-5 w-5 text-primary" />
            Agregar Nueva Propiedad
          </DialogTitle>
          <DialogDescription>
            Complete los datos de la nueva propiedad. Haga clic en guardar cuando termine.
          </DialogDescription>
        </DialogHeader>
        <form onSubmit={handleSubmit}>
          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label htmlFor="name">Nombre</Label>
              <Input
                id="name"
                placeholder="Casa en el centro"
                value={formData.name}
                onChange={(e) => handleChange('name', e.target.value)}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="address">Dirección</Label>
              <Input
                id="address"
                placeholder="Calle 123 #45-67"
                value={formData.address}
                onChange={(e) => handleChange('address', e.target.value)}
                required
              />
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div className="grid gap-2">
                <Label htmlFor="price">Precio</Label>
                <Input
                  id="price"
                  type="number"
                  placeholder="250000000"
                  value={formData.price}
                  onChange={(e) => handleChange('price', e.target.value)}
                  required
                />
              </div>
              <div className="grid gap-2">
                <Label htmlFor="year">Año</Label>
                <Input
                  id="year"
                  type="number"
                  placeholder="2020"
                  value={formData.year}
                  onChange={(e) => handleChange('year', e.target.value)}
                  required
                />
              </div>
            </div>
            <div className="grid gap-2">
              <Label htmlFor="codeInternal">Código Interno</Label>
              <Input
                id="codeInternal"
                placeholder="PROP-001"
                value={formData.codeInternal}
                onChange={(e) => handleChange('codeInternal', e.target.value)}
                required
              />
            </div>
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={() => setOpen(false)}>
              Cancelar
            </Button>
            <Button type="submit">Guardar</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

// Componente de ejemplo que muestra el diálogo en acción
export function PropertyManagementExample() {
  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Gestión de Propiedades</CardTitle>
            <CardDescription>
              Administra las propiedades del sistema
            </CardDescription>
          </div>
          <AddPropertyDialog />
        </div>
      </CardHeader>
      <CardContent>
        <p className="text-sm text-muted-foreground">
          Usa el botón "Agregar Propiedad" para crear nuevas propiedades en el sistema.
          Este es un ejemplo de cómo usar los componentes Dialog, Form, Input y Label de shadcn/ui.
        </p>
      </CardContent>
    </Card>
  );
}
