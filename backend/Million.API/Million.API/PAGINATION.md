# Documentación de Paginación

## Resumen

Se ha implementado paginación en todos los servicios de la API para mejorar el rendimiento y la experiencia del usuario al trabajar con grandes conjuntos de datos.

## Estructura

### DTOs de Paginación

#### `PaginationRequestDto`
Parámetros de entrada para solicitar datos paginados:
- `PageNumber`: Número de página (comienza en 1, valor por defecto: 1)
- `PageSize`: Elementos por página (máximo 100, valor por defecto: 10)

#### `PaginatedResponseDto<T>`
Respuesta paginada que incluye:
- `Data`: Lista de elementos de la página actual
- `PageNumber`: Número de página actual
- `PageSize`: Tamaño de página
- `TotalRecords`: Total de registros en toda la colección
- `TotalPages`: Total de páginas
- `HasPreviousPage`: Indica si existe una página anterior
- `HasNextPage`: Indica si existe una página siguiente

## Endpoints con Paginación

### Properties (Propiedades)

#### 1. Buscar propiedades con filtros y paginación
```
GET /api/properties/search/paginated?name={name}&address={address}&minPrice={minPrice}&maxPrice={maxPrice}&pageNumber={pageNumber}&pageSize={pageSize}
```

**Ejemplo:**
```bash
GET /api/properties/search/paginated?name=house&pageNumber=1&pageSize=10
```

**Respuesta:**
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 45,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "data": [
    {
      "idProperty": "...",
      "idOwner": "...",
      "name": "Beautiful House",
      "address": "123 Main St",
      "price": 250000,
      "image": "..."
    }
  ]
}
```

#### 2. Obtener propiedades por propietario con paginación
```
GET /api/properties/by-owner/{ownerId}/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

### Owners (Propietarios)

#### 1. Obtener todos los propietarios con paginación
```
GET /api/owners/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

#### 2. Buscar propietarios por nombre con paginación
```
GET /api/owners/search/by-name/paginated?name={name}&pageNumber={pageNumber}&pageSize={pageSize}
```

#### 3. Buscar propietarios por dirección con paginación
```
GET /api/owners/search/by-address/paginated?address={address}&pageNumber={pageNumber}&pageSize={pageSize}
```

#### 4. Buscar propietarios por rango de edad con paginación
```
GET /api/owners/search/by-age/paginated?minAge={minAge}&maxAge={maxAge}&pageNumber={pageNumber}&pageSize={pageSize}
```

### Property Traces (Trazas de Propiedades)

#### 1. Obtener trazas de una propiedad con paginación
```
GET /api/properties/{propertyId}/traces/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

#### 2. Buscar trazas por rango de fechas con paginación
```
GET /api/traces/by-date/paginated?startDate={startDate}&endDate={endDate}&pageNumber={pageNumber}&pageSize={pageSize}
```

**Ejemplo:**
```bash
GET /api/traces/by-date/paginated?startDate=2024-01-01&endDate=2024-12-31&pageNumber=1&pageSize=20
```

#### 3. Buscar trazas por rango de valor con paginación
```
GET /api/traces/by-value/paginated?minValue={minValue}&maxValue={maxValue}&pageNumber={pageNumber}&pageSize={pageSize}
```

### Property Images (Imágenes de Propiedades)

#### 1. Obtener imágenes de una propiedad con paginación
```
GET /api/properties/{propertyId}/images/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

## Validaciones

- **PageNumber**: Si es menor a 1, se establece en 1 automáticamente
- **PageSize**: 
  - Si es menor a 1, se establece en 10 (valor por defecto)
  - Si es mayor a 100, se establece en 100 (valor máximo)

## Implementación Técnica

### Capa de Repositorio
Se agregó el método `GetPaginatedAsync` a la interfaz `IRepository<T>` y su implementación en `Repository<T>`:

```csharp
Task<(IEnumerable<T> Items, long TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize);
```

Además, se agregaron métodos específicos de paginación en cada repositorio:
- `PropertyRepository`: `SearchPropertiesPaginatedAsync`, `GetByOwnerIdPaginatedAsync`
- `OwnerRepository`: `FindByNamePaginatedAsync`, `FindByAddressPaginatedAsync`, `FindByAgeRangePaginatedAsync`
- `PropertyTraceRepository`: `GetByPropertyIdPaginatedAsync`, `GetByDateRangePaginatedAsync`, `GetByValueRangePaginatedAsync`
- `PropertyImageRepository`: `GetByPropertyIdPaginatedAsync`

### Capa de Servicio
Cada servicio expone métodos paginados que:
1. Reciben `PaginationRequestDto`
2. Llaman al repositorio correspondiente
3. Retornan `PaginatedResponseDto<T>`

### Capa de Controlador
Los controladores exponen endpoints con el sufijo `/paginated` que aceptan parámetros de paginación como query strings.

## Mejores Prácticas

1. **Tamaño de página por defecto**: Se recomienda usar el valor por defecto de 10 elementos para la mayoría de casos
2. **Máximo de elementos**: El límite de 100 elementos por página previene cargas excesivas
3. **Compatibilidad**: Los endpoints sin paginación continúan funcionando para mantener retrocompatibilidad
4. **Metadata útil**: Use `HasNextPage` y `HasPreviousPage` para implementar navegación en el frontend

## Ejemplo de Uso en Cliente

```javascript
// Función para obtener propiedades paginadas
async function getProperties(pageNumber = 1, pageSize = 10) {
  const response = await fetch(
    `/api/properties/search/paginated?pageNumber=${pageNumber}&pageSize=${pageSize}`
  );
  const data = await response.json();
  
  console.log(`Mostrando página ${data.pageNumber} de ${data.totalPages}`);
  console.log(`Total de registros: ${data.totalRecords}`);
  
  return data;
}

// Navegación
const page1 = await getProperties(1, 10);
if (page1.hasNextPage) {
  const page2 = await getProperties(2, 10);
}
```

## Ventajas

? **Rendimiento**: Reduce la carga del servidor y el tiempo de respuesta  
? **Escalabilidad**: Maneja grandes volúmenes de datos eficientemente  
? **UX Mejorada**: Carga más rápida de datos en el frontend  
? **Flexibilidad**: El cliente puede controlar el tamaño de página según sus necesidades  
? **Metadata útil**: Información completa para implementar navegación y UI
