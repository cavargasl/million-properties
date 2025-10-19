export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:7231/api';

export const API_ENDPOINTS = {
  PROPERTIES: '/properties',
  OWNERS: '/owners',
  PROPERTY_IMAGES: '/propertyimages',
  PROPERTY_TRACES: '/propertytraces',
} as const;
