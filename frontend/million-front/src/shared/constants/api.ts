export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5269/api';

export const API_ENDPOINTS = {
  PROPERTIES: '/properties',
  OWNERS: '/owners',
} as const;
