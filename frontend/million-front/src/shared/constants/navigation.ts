export const NAVIGATION_LINKS = [
  {
    name: 'Inicio',
    href: '/',
  },
  {
    name: 'Propiedades',
    href: '/properties',
  },
  {
    name: 'Gestión de Propiedades',
    href: '/properties/manage',
  },
  {
    name: 'Propietarios',
    href: '/owners',
  },
] as const;

export const SITE_CONFIG = {
  name: 'Million Properties',
  description: 'Sistema de gestión de propiedades inmobiliarias',
} as const;
