# Documentaci�n de Paginaci�n

## Resumen

Se ha implementado paginaci�n en todos los servicios de la API para mejorar el rendimiento y la experiencia del usuario al trabajar con grandes conjuntos de datos.

## Estructura

### DTOs de Paginaci�n

#### `PaginationRequestDto`
Par�metros de entrada para solicitar datos paginados:
- `PageNumber`: N�mero de p�gina (comienza en 1, valor por defecto: 1)
- `PageSize`: Elementos por p�gina (m�ximo 100, valor por defecto: 10)

#### `PaginatedResponseDto<T>`
Respuesta paginada que incluye:
- `Data`: Lista de elementos de la p�gina actual
- `PageNumber`: N�mero de p�gina actual
- `PageSize`: Tama�o de p�gina
- `TotalRecords`: Total de registros en toda la colecci�n
- `TotalPages`: Total de p�ginas
- `HasPreviousPage`: Indica si existe una p�gina anterior
- `HasNextPage`: Indica si existe una p�gina siguiente

## Endpoints con Paginaci�n

### Properties (Propiedades)

#### 1. Buscar propiedades con filtros y paginaci�n
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

#### 2. Obtener propiedades por propietario con paginaci�n
```
GET /api/properties/by-owner/{ownerId}/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

### Owners (Propietarios)

#### 1. Obtener todos los propietarios con paginaci�n
```
GET /api/owners/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

#### 2. Buscar propietarios por nombre con paginaci�n
```
GET /api/owners/search/by-name/paginated?name={name}&pageNumber={pageNumber}&pageSize={pageSize}
```

#### 3. Buscar propietarios por direcci�n con paginaci�n
```
GET /api/owners/search/by-address/paginated?address={address}&pageNumber={pageNumber}&pageSize={pageSize}
```

#### 4. Buscar propietarios por rango de edad con paginaci�n
```
GET /api/owners/search/by-age/paginated?minAge={minAge}&maxAge={maxAge}&pageNumber={pageNumber}&pageSize={pageSize}
```

### Property Traces (Trazas de Propiedades)

#### 1. Obtener trazas de una propiedad con paginaci�n
```
GET /api/properties/{propertyId}/traces/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

#### 2. Buscar trazas por rango de fechas con paginaci�n
```
GET /api/traces/by-date/paginated?startDate={startDate}&endDate={endDate}&pageNumber={pageNumber}&pageSize={pageSize}
```

**Ejemplo:**
```bash
GET /api/traces/by-date/paginated?startDate=2024-01-01&endDate=2024-12-31&pageNumber=1&pageSize=20
```

#### 3. Buscar trazas por rango de valor con paginaci�n
```
GET /api/traces/by-value/paginated?minValue={minValue}&maxValue={maxValue}&pageNumber={pageNumber}&pageSize={pageSize}
```

### Property Images (Im�genes de Propiedades)

#### 1. Obtener im�genes de una propiedad con paginaci�n
```
GET /api/properties/{propertyId}/images/paginated?pageNumber={pageNumber}&pageSize={pageSize}
```

## Validaciones

- **PageNumber**: Si es menor a 1, se establece en 1 autom�ticamente
- **PageSize**: 
  - Si es menor a 1, se establece en 10 (valor por defecto)
  - Si es mayor a 100, se establece en 100 (valor m�ximo)

## Implementaci�n T�cnica

### Capa de Repositorio
Se agreg� el m�todo `GetPaginatedAsync` a la interfaz `IRepository<T>` y su implementaci�n en `Repository<T>`:

```csharp
Task<(IEnumerable<T> Items, long TotalCount)> GetPaginatedAsync(int pageNumber, int pageSize);
```

Adem�s, se agregaron m�todos espec�ficos de paginaci�n en cada repositorio:
- `PropertyRepository`: `SearchPropertiesPaginatedAsync`, `GetByOwnerIdPaginatedAsync`
- `OwnerRepository`: `FindByNamePaginatedAsync`, `FindByAddressPaginatedAsync`, `FindByAgeRangePaginatedAsync`
- `PropertyTraceRepository`: `GetByPropertyIdPaginatedAsync`, `GetByDateRangePaginatedAsync`, `GetByValueRangePaginatedAsync`
- `PropertyImageRepository`: `GetByPropertyIdPaginatedAsync`

### Capa de Servicio
Cada servicio expone m�todos paginados que:
1. Reciben `PaginationRequestDto`
2. Llaman al repositorio correspondiente
3. Retornan `PaginatedResponseDto<T>`

### Capa de Controlador
Los controladores exponen endpoints con el sufijo `/paginated` que aceptan par�metros de paginaci�n como query strings.

## Mejores Pr�cticas

1. **Tama�o de p�gina por defecto**: Se recomienda usar el valor por defecto de 10 elementos para la mayor�a de casos
2. **M�ximo de elementos**: El l�mite de 100 elementos por p�gina previene cargas excesivas
3. **Compatibilidad**: Los endpoints sin paginaci�n contin�an funcionando para mantener retrocompatibilidad
4. **Metadata �til**: Use `HasNextPage` y `HasPreviousPage` para implementar navegaci�n en el frontend

## Ejemplo de Uso en Cliente

```javascript
// Funci�n para obtener propiedades paginadas
async function getProperties(pageNumber = 1, pageSize = 10) {
  const response = await fetch(
    `/api/properties/search/paginated?pageNumber=${pageNumber}&pageSize=${pageSize}`
  );
  const data = await response.json();
  
  console.log(`Mostrando p�gina ${data.pageNumber} de ${data.totalPages}`);
  console.log(`Total de registros: ${data.totalRecords}`);
  
  return data;
}

// Navegaci�n
const page1 = await getProperties(1, 10);
if (page1.hasNextPage) {
  const page2 = await getProperties(2, 10);
}
```

## Ventajas

? **Rendimiento**: Reduce la carga del servidor y el tiempo de respuesta  
? **Escalabilidad**: Maneja grandes vol�menes de datos eficientemente  
? **UX Mejorada**: Carga m�s r�pida de datos en el frontend  
? **Flexibilidad**: El cliente puede controlar el tama�o de p�gina seg�n sus necesidades  
? **Metadata �til**: Informaci�n completa para implementar navegaci�n y UI
