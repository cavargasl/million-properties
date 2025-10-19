export interface BaseError {
  message: string;
  code: string;
  details?: unknown;
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
