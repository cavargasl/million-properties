# Million Properties API

API RESTful para gestión de propiedades inmobiliarias construida con .NET 9, MongoDB y Clean Architecture.

## 🏗️ Arquitectura

### Clean Architecture + SOLID

```
┌─────────────────────────────────────┐
│  Controllers (API Layer)            │
│  - OwnersController                 │
│  - PropertiesController             │
│  - PropertyImagesController         │
│  - PropertyTracesController         │
└─────────────────────────────────────┘
                ↓↓ DTOs
┌─────────────────────────────────────┐
│  Services (Business Logic)          │
│  - MongoDbService                   │
│  - OwnerService                     │
│  - PropertyService                  │
│  - PropertyImageService             │
│  - PropertyTraceService             │
└─────────────────────────────────────┘
                ↓↓ Entities
┌─────────────────────────────────────┐
│  Repositories (Data Access)         │
│  - Repository<T> (Base)             │
│  - OwnerRepository                  │
│  - PropertyRepository               │
│  - PropertyImageRepository          │
│  - PropertyTraceRepository          │
└─────────────────────────────────────┘
                ↓↓
┌─────────────────────────────────────┐
│  MongoDB Database                   │
└─────────────────────────────────────┘
```

## 📦 Entidades de Dominio

### Owner
- IdOwner (ObjectId)
- Name
- Address
- Photo
- Birthday

### Property
- IdProperty (ObjectId)
- Name
- Address
- Price
- CodeInternal
- Year
- IdOwner (FK)

### PropertyImage
- IdPropertyImage (ObjectId)
- IdProperty (FK)
- File
- Enabled

### PropertyTrace
- IdPropertyTrace (ObjectId)
- IdProperty (FK)
- DateSale
- Name
- Value
- Tax

## ✅ Características Implementadas

### ✨ Requerimientos Cumplidos

1. **API en .NET 9** ✓
2. **MongoDB como base de datos** ✓
3. **Filtros para propiedades** ✓
   - Por nombre (parcial, case-insensitive)
   - Por dirección (parcial, case-insensitive)
   - Por rango de precio (min/max)
4. **DTOs con campos requeridos** ✓
   - IdOwner
   - Name
   - Address
   - Price
   - **Solo una imagen** (primera habilitada)

### 🚀 Características Adicionales

- **Clean Architecture**: Separación clara de capas
- **SOLID Principles**: 
  - SRP: Cada clase tiene una única responsabilidad
  - OCP: Extensible sin modificar código existente
  - LSP: Repositorios intercambiables
  - DIP: Dependencias de abstracciones
- **Repository Pattern Genérico**: Reutilización de código CRUD
- **Type-Safe Queries**: Expresiones lambda en lugar de strings
- **Error Handling**: Manejo robusto de excepciones
- **Logging**: Integrado en todos los controladores
- **CORS**: Configurado para desarrollo
- **Swagger/OpenAPI**: Documentación automática
- **FluentValidation**: Validaciones de datos
- **MongoDbService**: Servicio centralizado para acceso a MongoDB

## ⚙️ Configuración

### appsettings.json

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "MillionPropertiesDB"
  }
}
```

## 🌐 Endpoints de la API

### Owners

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/owners` | Obtener todos los propietarios |
| GET | `/api/owners/{id}` | Obtener propietario por ID |
| POST | `/api/owners` | Crear propietario |
| PUT | `/api/owners/{id}` | Actualizar propietario |
| DELETE | `/api/owners/{id}` | Eliminar propietario |
| GET | `/api/owners/search/by-name?name={name}` | Buscar por nombre |
| GET | `/api/owners/search/by-address?address={address}` | Buscar por dirección |
| GET | `/api/owners/search/by-age?minAge={min}&maxAge={max}` | Buscar por rango de edad |

### Properties

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/properties` | Obtener todas las propiedades |
| GET | `/api/properties/search?name={}&address={}&minPrice={}&maxPrice={}` | **Buscar con filtros** ✨ |
| GET | `/api/properties/{id}` | Obtener propiedad por ID (con todas las imágenes) |
| POST | `/api/properties` | Crear propiedad |
| PUT | `/api/properties/{id}` | Actualizar propiedad |
| DELETE | `/api/properties/{id}` | Eliminar propiedad |
| GET | `/api/properties/by-owner/{ownerId}` | Obtener propiedades de un propietario |

### Property Images

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/properties/{propertyId}/images` | Obtener imágenes de una propiedad |
| GET | `/api/properties/{propertyId}/images/{id}` | Obtener imagen específica |
| POST | `/api/properties/{propertyId}/images` | Agregar imagen |
| PUT | `/api/properties/{propertyId}/images/{id}` | Actualizar imagen |
| DELETE | `/api/properties/{propertyId}/images/{id}` | Eliminar imagen |
| PATCH | `/api/properties/{propertyId}/images/{id}/toggle?enabled={bool}` | Habilitar/deshabilitar |

### Property Traces

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/properties/{propertyId}/traces` | Obtener todos los traces de una propiedad |
| GET | `/api/properties/{propertyId}/traces/{id}` | Obtener trace específico |
| POST | `/api/properties/{propertyId}/traces` | Crear nuevo trace |
| PUT | `/api/properties/{propertyId}/traces/{id}` | Actualizar trace |
| DELETE | `/api/properties/{propertyId}/traces/{id}` | Eliminar trace |

## 📝 Ejemplos de Uso

### Buscar Propiedades con Filtros

```http
GET /api/properties/search?name=Casa&address=Bogotá&minPrice=200000&maxPrice=500000
```

**Respuesta (PropertyDto):**
```json
[
  {
    "idProperty": "507f1f77bcf86cd799439011",
    "idOwner": "507f191e810c19729de860ea",
    "name": "Casa en Bogotá",
    "address": "Calle 123 #45-67, Bogotá",
    "price": 350000,
    "image": "https://example.com/images/house-front.jpg"
  }
]
```

### Crear Propietario

```http
POST /api/owners
Content-Type: application/json

{
  "name": "Juan Pérez",
  "address": "Calle Principal 123",
  "photo": "https://example.com/photo.jpg",
  "birthday": "1985-05-15T00:00:00"
}
```

### Crear Propiedad

```http
POST /api/properties
Content-Type: application/json

{
  "name": "Casa Moderna",
  "address": "Carrera 7 #45-23",
  "price": 450000,
  "codeInternal": "PROP-2024-001",
  "year": 2023,
  "idOwner": "507f191e810c19729de860ea"
}
```

### Crear Property Trace

```http
POST /api/properties/507f1f77bcf86cd799439011/traces
Content-Type: application/json

{
  "idProperty": "507f1f77bcf86cd799439011",
  "dateSale": "2024-01-15T00:00:00",
  "name": "Venta Inicial",
  "value": 350000,
  "tax": 17500
}
```

## 📋 DTOs

### PropertyDto (Para Listados)
```csharp
public class PropertyDto
{
    public string IdProperty { get; set; }
    public string IdOwner { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; } // Solo UNA imagen
}
```

### PropertyFilterDto (Para Búsquedas)
```csharp
public class PropertyFilterDto
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
```

### PropertyDetailDto (Para Detalles)
```csharp
public class PropertyDetailDto
{
    public string IdProperty { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public decimal Price { get; set; }
    public string CodeInternal { get; set; }
    public int Year { get; set; }
    public string IdOwner { get; set; }
    public OwnerDto? Owner { get; set; }
    public List<PropertyImageDto> Images { get; set; } // TODAS las imágenes
}
```

### PropertyTraceDto
```csharp
public class PropertyTraceDto
{
    public string IdPropertyTrace { get; set; }
    public string IdProperty { get; set; }
    public DateTime DateSale { get; set; }
    public string Name { get; set; }
    public decimal Value { get; set; }
    public decimal Tax { get; set; }
}
```

## 🔧 Características Técnicas

### Repository Pattern con Type-Safety

```csharp
// ❌ Antiguo (hardcoded strings)
var filter = Builders<Owner>.Filter.Regex("name", ...);

// ✅ Nuevo (type-safe)
var filter = Builders<Owner>.Filter.Regex(x => x.Name, ...);
```

### Separación de Responsabilidades

- **Controllers**: Solo manejan HTTP (reciben/retornan DTOs)
- **Services**: Lógica de negocio y mapeo DTOs ↔ Entities
- **Repositories**: Acceso a datos (solo Entities)
- **MongoDbService**: Servicio centralizado para acceso a la base de datos

### Inyección de Dependencias

```csharp
// MongoDbService: Singleton
builder.Services.AddSingleton<MongoDbService>();

// Repositorios: Singleton (MongoDB thread-safe)
builder.Services.AddSingleton<OwnerRepository>();
builder.Services.AddSingleton<PropertyRepository>();
builder.Services.AddSingleton<PropertyImageRepository>();
builder.Services.AddSingleton<PropertyTraceRepository>();

// Servicios: Scoped (por petición HTTP)
builder.Services.AddScoped<OwnerService>();
builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<PropertyImageService>();
builder.Services.AddScoped<PropertyTraceService>();
```

## 🚀 Cómo Ejecutar

1. **Instalar MongoDB**
   ```bash
   # Asegúrate de tener MongoDB corriendo en localhost:27017
   ```

2. **Configurar appsettings.json**
   ```json
   {
     "MongoDbSettings": {
       "ConnectionString": "mongodb://localhost:27017",
       "DatabaseName": "million_db"
     }
   }
   ```

3. **Restaurar paquetes**
   ```bash
   dotnet restore
   ```

4. **Ejecutar la API**
   ```bash
   dotnet run --project Million.API
   ```

5. **Probar con Swagger**
   ```
   https://localhost:7001/openapi
   ```

6. **O usar los archivos .http**
   - Abrir `Million.API.http` en Visual Studio
   - Ejecutar las peticiones directamente

## 📁 Estructura del Proyecto

```
Million.API/
├── Controllers/
│   ├── OwnersController.cs
│   ├── PropertiesController.cs
│   ├── PropertyImagesController.cs
│   └── PropertyTracesController.cs
├── Services/
│   ├── MongoDbService.cs
│   ├── OwnerService.cs
│   ├── PropertyService.cs
│   ├── PropertyImageService.cs
│   └── PropertyTraceService.cs
├── Repository/
│   ├── IRepository.cs
│   ├── Repository.cs (Generic Base)
│   ├── OwnerRepository.cs
│   ├── PropertyRepository.cs
│   ├── PropertyImageRepository.cs
│   └── PropertyTraceRepository.cs
├── Domain/
│   ├── Owner.cs
│   ├── Property.cs
│   ├── PropertyImage.cs
│   └── PropertyTrace.cs
├── DTOs/
│   ├── OwnerDtos.cs
│   ├── PropertyDto.cs
│   ├── PropertyDtos.cs
│   ├── PropertyFilterDto.cs
│   ├── PropertyImageDtos.cs
│   └── PropertyTraceDtos.cs
├── Settings/
│   └── MongoDbSettings.cs
├── Program.cs
├── appsettings.json
└── Million.API.http
```

## ✔️ Validaciones Implementadas

- Verificación de existencia antes de actualizar/eliminar
- Validación de códigos internos únicos
- Validación de rangos de edad
- Validación de propiedades antes de agregar imágenes/traces
- Validación de datos con Data Annotations
- FluentValidation integrado para validaciones complejas
- Manejo de errores con mensajes descriptivos
- Logging de errores y warnings

## 📦 Dependencias

- **.NET 9.0** - Framework principal
- **MongoDB.Driver 3.5.0** - Driver oficial de MongoDB
- **FluentValidation 12.0.0** - Validaciones fluidas
- **Microsoft.AspNetCore.OpenApi 9.0.10** - Documentación OpenAPI/Swagger
- **Microsoft.AspNet.Mvc 5.3.0** - ASP.NET MVC

## 🎯 Próximos Pasos Sugeridos

1. ~~Agregar validaciones con FluentValidation~~ ✅ (Ya implementado)
2. Implementar AutoMapper para mapeos automáticos
3. Agregar paginación en listados
4. ~~Implementar PropertyTrace endpoints~~ ✅ (Ya implementado)
5. Agregar autenticación y autorización (JWT)
6. Implementar caché con Redis
7. Agregar tests unitarios e integración (xUnit, Moq)
8. Implementar logging avanzado con Serilog
9. Agregar soporte para subida de imágenes real (Azure Blob Storage)
10. Implementar rate limiting y throttling

---

**Desarrollado con .NET 9, MongoDB y Clean Architecture** 🚀💼
