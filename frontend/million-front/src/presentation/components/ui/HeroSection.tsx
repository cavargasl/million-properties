'use client';

import Link from 'next/link';
import { ArrowRight, Building2, Users, FileText, TrendingUp } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';

const features = [
  {
    icon: Building2,
    title: 'Gestión de Propiedades',
    description: 'Administra tu portafolio inmobiliario de manera eficiente',
  },
  {
    icon: Users,
    title: 'Control de Propietarios',
    description: 'Mantén actualizada la información de todos los propietarios',
  },
  {
    icon: FileText,
    title: 'Trazabilidad Completa',
    description: 'Registra todas las transacciones e historial de cada propiedad',
  },
  {
    icon: TrendingUp,
    title: 'Análisis de Mercado',
    description: 'Obtén insights sobre el valor y tendencias del mercado',
  },
];

export function HeroSection() {
  return (
    <div className="flex flex-col gap-16 py-12">
      {/* Hero Principal */}
      <section className="flex flex-col items-center text-center gap-6 px-4">
        <Badge variant="secondary" className="gap-2">
          <Building2 className="h-4 w-4" />
          <span>Sistema de Gestión Inmobiliaria</span>
        </Badge>

        <h1 className="text-4xl sm:text-5xl md:text-6xl lg:text-7xl font-bold tracking-tight max-w-4xl">
          Gestiona tus propiedades de forma{' '}
          <span className="text-primary">inteligente</span>
        </h1>

        <p className="text-lg sm:text-xl text-muted-foreground max-w-2xl">
          Plataforma completa para administrar propiedades, propietarios y 
          realizar seguimiento detallado de cada transacción inmobiliaria.
        </p>

        <div className="flex flex-col sm:flex-row gap-4 mt-4">
          <Button asChild size="lg">
            <Link href="/properties">
              Ver Propiedades
              <ArrowRight className="ml-2 h-4 w-4" />
            </Link>
          </Button>
          <Button asChild variant="outline" size="lg">
            <Link href="/owners">
              Gestionar Propietarios
            </Link>
          </Button>
        </div>
      </section>

      {/* Features Grid */}
      <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 px-4 max-w-7xl mx-auto w-full">
        {features.map((feature) => {
          const Icon = feature.icon;
          return (
            <Card key={feature.title} className="hover:shadow-md transition-shadow">
              <CardHeader>
                <div className="p-2 bg-primary/10 rounded-lg w-fit">
                  <Icon className="h-6 w-6 text-primary" />
                </div>
                <CardTitle className="text-lg">{feature.title}</CardTitle>
              </CardHeader>
              <CardContent>
                <CardDescription>{feature.description}</CardDescription>
              </CardContent>
            </Card>
          );
        })}
      </section>

      {/* Stats Section */}
      <section className="flex flex-col sm:flex-row justify-center gap-8 sm:gap-12 px-4 py-8 border-t border-border">
        <Card className="text-center w-sm">
          <CardHeader className="pb-2">
            <CardTitle className="text-3xl sm:text-4xl">1000+</CardTitle>
          </CardHeader>
          <CardContent>
            <CardDescription>Propiedades Gestionadas</CardDescription>
          </CardContent>
        </Card>
        <Card className="text-center w-sm">
          <CardHeader className="pb-2">
            <CardTitle className="text-3xl sm:text-4xl">500+</CardTitle>
          </CardHeader>
          <CardContent>
            <CardDescription>Propietarios Activos</CardDescription>
          </CardContent>
        </Card>
        <Card className="text-center w-sm">
          <CardHeader className="pb-2">
            <CardTitle className="text-3xl sm:text-4xl">99.9%</CardTitle>
          </CardHeader>
          <CardContent>
            <CardDescription>Disponibilidad</CardDescription>
          </CardContent>
        </Card>
      </section>
    </div>
  );
}
