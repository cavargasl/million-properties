export interface BaseError {
  message: string;
  code: string;
  details?: unknown;
}

export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
}

export interface PaginationMetadata {
  pageNumber: number;
  pageSize: number;
  totalRecords: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
}

export type Result<T> = {
  data: T | null;
  error: BaseError | null;
};

export type ResultArray<T> = {
  data: T[] | null;
  error: BaseError | null;
};

export type PaginatedResult<T> = {
  data: {
    items: T[];
    pagination: PaginationMetadata;
  } | null;
  error: BaseError | null;
};
