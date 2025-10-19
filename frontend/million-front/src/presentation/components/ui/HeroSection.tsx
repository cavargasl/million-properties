'use client';

import Link from 'next/link';
import { ArrowRight, Building2, Users, FileText, TrendingUp } from 'lucide-react';

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
        <div className="inline-flex items-center gap-2 bg-primary/10 text-primary px-4 py-2 rounded-full text-sm font-medium">
          <Building2 className="h-4 w-4" />
          <span>Sistema de Gestión Inmobiliaria</span>
        </div>

        <h1 className="text-4xl sm:text-5xl md:text-6xl lg:text-7xl font-bold tracking-tight max-w-4xl">
          Gestiona tus propiedades de forma{' '}
          <span className="text-primary">inteligente</span>
        </h1>

        <p className="text-lg sm:text-xl text-muted-foreground max-w-2xl">
          Plataforma completa para administrar propiedades, propietarios y 
          realizar seguimiento detallado de cada transacción inmobiliaria.
        </p>

        <div className="flex flex-col sm:flex-row gap-4 mt-4">
          <Link
            href="/properties"
            className="inline-flex items-center justify-center gap-2 bg-primary text-primary-foreground hover:bg-primary/90 px-6 py-3 rounded-lg font-medium transition-colors"
          >
            Ver Propiedades
            <ArrowRight className="h-4 w-4" />
          </Link>
          <Link
            href="/owners"
            className="inline-flex items-center justify-center gap-2 border border-input hover:bg-accent hover:text-accent-foreground px-6 py-3 rounded-lg font-medium transition-colors"
          >
            Gestionar Propietarios
          </Link>
        </div>
      </section>

      {/* Features Grid */}
      <section className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 px-4 max-w-7xl mx-auto w-full">
        {features.map((feature) => {
          const Icon = feature.icon;
          return (
            <div
              key={feature.title}
              className="flex flex-col items-start gap-3 p-6 rounded-lg border border-border bg-card hover:shadow-md transition-shadow"
            >
              <div className="p-2 bg-primary/10 rounded-lg">
                <Icon className="h-6 w-6 text-primary" />
              </div>
              <h3 className="font-semibold text-lg">{feature.title}</h3>
              <p className="text-sm text-muted-foreground">{feature.description}</p>
            </div>
          );
        })}
      </section>

      {/* Stats Section */}
      <section className="flex flex-col sm:flex-row justify-center gap-8 sm:gap-12 px-4 py-8 border-t border-border">
        <div className="flex flex-col items-center gap-1">
          <p className="text-3xl sm:text-4xl font-bold">1000+</p>
          <p className="text-sm text-muted-foreground">Propiedades Gestionadas</p>
        </div>
        <div className="flex flex-col items-center gap-1">
          <p className="text-3xl sm:text-4xl font-bold">500+</p>
          <p className="text-sm text-muted-foreground">Propietarios Activos</p>
        </div>
        <div className="flex flex-col items-center gap-1">
          <p className="text-3xl sm:text-4xl font-bold">99.9%</p>
          <p className="text-sm text-muted-foreground">Disponibilidad</p>
        </div>
      </section>
    </div>
  );
}
