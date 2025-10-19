export interface Property {
  id: string;
  name: string;
  address: string;
  price: number;
  codeInternal: string;
  year: number;
  ownerId: string;
  ownerName?: string;
  images?: PropertyImage[];
  traces?: PropertyTrace[];
}

export interface PropertyImage {
  id: string;
  propertyId: string;
  file: string;
  enabled: boolean;
}

export interface PropertyTrace {
  id: string;
  dateSale: string;
  name: string;
  value: number;
  tax: number;
  propertyId: string;
}
