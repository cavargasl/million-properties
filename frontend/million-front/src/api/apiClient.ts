import axios, { type AxiosError, type AxiosInstance } from 'axios';
import { API_BASE_URL } from '@/shared/constants/api';
import type { BaseError } from '@/core/shared/domain/types';

export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor
apiClient.interceptors.request.use(
  (config) => {
    // Aquí puedes agregar tokens de autenticación si es necesario
    // const token = localStorage.getItem('token');
    // if (token) {
    //   config.headers.Authorization = `Bearer ${token}`;
    // }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Response interceptor
apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    return Promise.reject(error);
  }
);

/**
 * Transforms axios errors to BaseError format
 * Extracts the most relevant error message from different error formats
 */
export const transformError = (error: unknown): BaseError => {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data;
    let errorMessage = 'An error occurred';
    
    // Caso 1: Error simple con formato { "error": "mensaje" }
    if (responseData?.error) {
      errorMessage = responseData.error;
    }
    // Caso 2: Errores de validación ASP.NET con formato { "errors": { "Field": ["message"] } }
    else if (responseData?.errors) {
      const validationErrors = responseData.errors;
      const errorMessages: string[] = [];
      
      Object.keys(validationErrors).forEach((field) => {
        const fieldErrors = validationErrors[field];
        if (Array.isArray(fieldErrors)) {
          errorMessages.push(...fieldErrors);
        }
      });
      
      // Unir todos los mensajes de error
      errorMessage = errorMessages.length > 0 
        ? errorMessages.join(', ') 
        : 'Validation error occurred';
    }
    // Caso 3: Error con message directo
    else if (responseData?.message) {
      errorMessage = responseData.message;
    }
    // Caso 4: Usar mensaje de axios
    else if (error.message) {
      // Evitar mensajes genéricos de HTTP
      if (!error.message.includes('Request failed with status code')) {
        errorMessage = error.message;
      }
    }
    
    return {
      message: errorMessage,
      code: error.response?.data?.code || error.code || 'UNKNOWN_ERROR',
      details: responseData,
    };
  }
  
  if (error instanceof Error) {
    return {
      message: error.message,
      code: 'UNKNOWN_ERROR',
    };
  }
  
  return {
    message: 'An unknown error occurred',
    code: 'UNKNOWN_ERROR',
    details: error,
  };
};

