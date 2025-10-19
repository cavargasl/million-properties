export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

export const API_ENDPOINTS = {
  PROPERTIES: '/properties',
  OWNERS: '/owners',
  PROPERTY_IMAGES: '/propertyimages',
  PROPERTY_TRACES: '/propertytraces',
} as const;
