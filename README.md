# 🏢 Million Properties

Sistema completo de gestión de propiedades inmobiliarias desarrollado con arquitectura moderna, escalable y mantenible.

> **Prueba Técnica**: Plataforma full-stack para administración de propiedades, propietarios, imágenes y trazabilidad de operaciones.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Stack Tecnológico](#-stack-tecnológico)
- [Arquitectura](#-arquitectura)
- [Requisitos Previos](#-requisitos-previos)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Ejecución](#-ejecución)
- [Testing](#-testing)
- [API Documentation](#-api-documentation)
- [Estructura del Proyecto](#-estructura-del-proyecto)
- [Patrones y Principios](#-patrones-y-principios)

---

## ✨ Características

### Backend (API REST)
- ✅ **CRUD completo** para Propiedades, Propietarios, Imágenes y Trazas
- ✅ **Filtros avanzados**: búsqueda por nombre, dirección y rango de precios
- ✅ **Paginación** configurable en todas las consultas
- ✅ **Validación de datos** con FluentValidation
- ✅ **Manejo robusto de errores** con mensajes descriptivos
- ✅ **CORS** configurado para desarrollo
- ✅ **Swagger/OpenAPI** para documentación interactiva

### Frontend (Next.js)
- ✅ **Interfaz moderna y responsive** con Tailwind CSS
- ✅ **Gestión de estado del servidor** con React Query
- ✅ **Formularios validados** con React Hook Form + Zod
- ✅ **Componentes reutilizables** con Radix UI
- ✅ **Testing completo** con Jest y Testing Library
- ✅ **TypeScript** para seguridad de tipos
- ✅ **Dark Mode** integrado

---

## 🛠 Stack Tecnológico

### Backend
| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| **.NET** | 9.0 | Framework principal |
| **MongoDB** | 3.5.0 | Base de datos NoSQL |
| **FluentValidation** | 12.0.0 | Validación de modelos |
| **NUnit** | 4.2.2 | Testing unitario |
| **Moq** | 4.20.72 | Mocking para tests |

### Frontend
| Tecnología | Versión | Propósito |
|-----------|---------|-----------|
| **Next.js** | 15.5.6 | Framework React |
| **React** | 19.1.0 | Librería UI |
| **TypeScript** | 5.x | Tipado estático |
| **Tailwind CSS** | 4.x | Estilos utility-first |
| **React Query** | 5.90.5 | Gestión de estado servidor |
| **Axios** | 1.12.2 | Cliente HTTP |
| **Zod** | 4.1.12 | Validación de schemas |
| **Jest** | 30.2.0 | Testing framework |

---

## 🏗 Arquitectura

### Backend: Clean Architecture + SOLID

```
┌──────────────────────────────────────────────┐
│          Controllers (API Layer)             │
│  ┌──────────────────────────────────────┐  │
│  │ OwnersController                      │  │
│  │ PropertiesController                  │  │
│  │ PropertyImagesController              │  │
│  │ PropertyTracesController              │  │
│  └──────────────────────────────────────┘  │
└──────────────────┬───────────────────────────┘
                   │ DTOs
┌──────────────────▼───────────────────────────┐
│      Services (Business Logic Layer)         │
│  ┌──────────────────────────────────────┐  │
│  │ OwnerService         (IOwnerService) │  │
│  │ PropertyService   (IPropertyService) │  │
│  │ PropertyImageService                  │  │
│  │ PropertyTraceService                  │  │
│  └──────────────────────────────────────┘  │
└──────────────────┬───────────────────────────┘
                   │ Entities
┌──────────────────▼───────────────────────────┐
│      Repositories (Data Access Layer)        │
│  ┌──────────────────────────────────────┐  │
│  │ Repository<T> (Generic Base)         │  │
│  │ OwnerRepository                       │  │
│  │ PropertyRepository                    │  │
│  │ PropertyImageRepository               │  │
│  │ PropertyTraceRepository               │  │
│  └──────────────────────────────────────┘  │
└──────────────────┬───────────────────────────┘
                   │
┌──────────────────▼───────────────────────────┐
│            MongoDB Database                  │
│  Collections: owners, properties,            │
│  propertyImages, propertyTraces              │
└──────────────────────────────────────────────┘
```

**Principios SOLID aplicados:**
- **SRP**: Cada clase tiene una única responsabilidad
- **OCP**: Abierto para extensión, cerrado para modificación
- **LSP**: Repositorios intercambiables mediante interfaces
- **ISP**: Interfaces segregadas por funcionalidad
- **DIP**: Dependencias hacia abstracciones (interfaces)

### Frontend: Arquitectura HYFORCE (Hexagonal + Clean)

```
src/
├── core/                          # 🎯 Lógica de Negocio (Domain Core)
│   ├── property/                  # Módulo de propiedades
│   │   ├── domain/               # Entidades y contratos
│   │   │   ├── property.ts       # Entity + Types
│   │   │   ├── propertyRepository.ts
│   │   │   ├── propertyImage.ts
│   │   │   └── propertyTrace.ts
│   │   ├── application/          # Casos de uso (Services)
│   │   │   ├── propertyService.ts
│   │   │   ├── propertyImageService.ts
│   │   │   └── propertyTraceService.ts
│   │   ├── infrastructure/       # Implementaciones
│   │   │   ├── axiosPropertyRepository.ts
│   │   │   ├── propertyDto.ts   # Mappers
│   │   │   └── propertyMapper.ts
│   │   └── __mock__/            # Test utilities
│   ├── owner/                    # Módulo de propietarios
│   └── shared/                   # Código compartido
│       ├── domain/types.ts      # Result, Pagination
│       └── utils/
├── infrastructure/               # 🔌 Servicios Externos
│   └── api/                     # Cliente Axios configurado
├── presentation/                 # 🎨 UI Components
│   ├── components/
│   └── hooks/
└── shared/                      # 🌐 Constantes globales
```

**Beneficios:**
- ✅ **Testabilidad**: Lógica de negocio independiente de frameworks
- ✅ **Mantenibilidad**: Cambios aislados por capa
- ✅ **Escalabilidad**: Fácil agregar nuevas features
- ✅ **Reusabilidad**: Core compartible entre plataformas

---

## 📦 Requisitos Previos

### Software Necesario

- **Node.js**: >= 20.x (recomendado: 20.18.0 LTS)
- **pnpm**: >= 9.x (gestor de paquetes)
- **.NET SDK**: 9.0 o superior
- **MongoDB**: >= 7.0 (local o Docker)
- **Git**: Para clonar el repositorio

### Instalación de Herramientas

```bash
# Instalar pnpm (si no lo tienes)
npm install -g pnpm

# Verificar instalaciones
node --version
pnpm --version
dotnet --version
mongod --version
```

---

## 🚀 Instalación

### 1. Clonar el Repositorio

```bash
git clone https://github.com/cavargasl/million-properties.git
cd million-properties
```

### 2. Configurar Backend

```bash
cd backend/Million.API/Million.API

# Restaurar dependencias
dotnet restore

# Compilar proyecto
dotnet build
```

### 3. Configurar Frontend

```bash
cd ../../../frontend/million-front

# Instalar dependencias
pnpm install
```

---

## ⚙ Configuración

### Backend - MongoDB

**Opción 1: MongoDB Local**

1. Instalar MongoDB desde [mongodb.com](https://www.mongodb.com/try/download/community)
2. Iniciar servicio:
```bash
# Windows
net start MongoDB

# Linux/Mac
sudo systemctl start mongod
```


**Configuración de Conexión** (`appsettings.json`):

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017/",
    "DatabaseName": "million_db"
  }
}
```

### Frontend - Variables de Entorno

Crear archivo `.env.local`:

```bash
# API Backend URL
NEXT_PUBLIC_API_URL=http://localhost:5269/api

# Optional: Development Mode
NODE_ENV=development
```

---

## 🎮 Ejecución

### Iniciar Backend

```bash
cd backend/Million.API/Million.API

# Modo desarrollo (con hot-reload)
dotnet watch run

# Modo producción
dotnet run
```

**API disponible en:**
- HTTP: `http://localhost:5269`
- Swagger UI: `http://localhost:5269/swagger`

### Poblar Base de Datos (Opcional)

El proyecto incluye datos de prueba listos para usar. **No necesitas tener MongoDB CLI (mongosh/mongo) instalado.**

#### Opción 1: Script PowerShell (Recomendado)

```bash
# 1. Inicia la API en una terminal
cd backend/Million.API/Million.API
dotnet run

# 2. En otra terminal PowerShell, carga los datos
.\seed-database.ps1

# O si quieres BORRAR TODO y cargar datos frescos (⚠️ Cuidado)
.\reset-and-seed.ps1
```

#### Opción 2: Endpoint API Directo

```bash
# Solo cargar datos (sin borrar existentes)
curl -X POST http://localhost:5269/api/database/seed

# Resetear TODO y cargar datos frescos (⚠️ Elimina todos los datos)
curl -X POST http://localhost:5269/api/database/reset
```

**Datos incluidos:**
- 8 Propietarios (diferentes ciudades de Colombia)
- 15 Propiedades (apartamentos, casas, penthouses)
- 2-4 Imágenes por propiedad (URLs de Unsplash)
- 1-3 Trazas por propiedad (historial de cambios)

📖 **Guía de inicio rápido**: [backend/Million.API/Million.API/INICIO_RAPIDO.md](backend/Million.API/Million.API/INICIO_RAPIDO.md)

### Iniciar Frontend

```bash
cd frontend/million-front

# Modo desarrollo
pnpm dev

# Modo producción
pnpm build
pnpm start
```

**Frontend disponible en:**
- Desarrollo: `http://localhost:3000`

---

## 🧪 Testing

### Backend Tests

```bash
cd backend/Million.API/Million.API

# Ejecutar todos los tests
dotnet test

# Tests con cobertura
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

### Frontend Tests

```bash
cd frontend/million-front

# Ejecutar tests
pnpm test

# Tests en modo watch
pnpm test:watch

# Generar reporte de cobertura
pnpm test:coverage
```

**Cobertura de Tests:**
- **Backend**: Servicios y Repositorios > 80%
- **Frontend**: Core logic > 85%

---

## 📚 API Documentation

### Endpoints Principales

#### Properties (Propiedades)

```http
GET    /api/properties              # Listar todas (paginado)
GET    /api/properties/search        # Buscar con filtros
GET    /api/properties/{id}          # Obtener por ID
POST   /api/properties               # Crear propiedad
PUT    /api/properties/{id}          # Actualizar completa
PATCH  /api/properties/{id}          # Actualizar parcial
DELETE /api/properties/{id}          # Eliminar
```

**Ejemplo: Buscar propiedades con filtros**

```bash
GET /api/properties/search?name=apartamento&minPrice=100000&maxPrice=500000
```

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "idProperty": "6756d1f2e4b0a1c2d3e4f5g6",
      "name": "Apartamento Moderno",
      "address": "Calle 123 #45-67",
      "price": 250000,
      "codeInternal": "PROP-001",
      "year": 2023,
      "idOwner": "6756d1f2e4b0a1c2d3e4f5g7",
      "ownerName": "Juan Pérez",
      "image": "https://example.com/image1.jpg"
    }
  ],
  "message": "Properties retrieved successfully"
}
```

#### Owners (Propietarios)

```http
GET    /api/owners                   # Listar todos (paginado)
GET    /api/owners/{id}              # Obtener por ID
POST   /api/owners                   # Crear propietario
PUT    /api/owners/{id}              # Actualizar
DELETE /api/owners/{id}              # Eliminar
```

#### Property Images (Imágenes)

```http
GET    /api/propertyimages/property/{propertyId}  # Por propiedad
POST   /api/propertyimages                         # Agregar imagen
PUT    /api/propertyimages/{id}                    # Actualizar
DELETE /api/propertyimages/{id}                    # Eliminar
PATCH  /api/propertyimages/{id}/toggle             # Habilitar/Deshabilitar
```

#### Property Traces (Trazas)

```http
GET    /api/propertytraces/property/{propertyId}  # Por propiedad
POST   /api/propertytraces                         # Registrar traza
PUT    /api/propertytraces/{id}                    # Actualizar
DELETE /api/propertytraces/{id}                    # Eliminar
```

### Paginación

Todos los endpoints de listado soportan paginación:

```bash
GET /api/properties?pageNumber=1&pageSize=10
```

**Response con metadata:**
```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalItems": 125,
    "totalPages": 13,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

---

## 📁 Estructura del Proyecto

### Backend (Million.API)

```
Million.API/
├── Controllers/           # API Endpoints
│   ├── OwnersController.cs
│   ├── PropertiesController.cs
│   ├── PropertyImagesController.cs
│   └── PropertyTracesController.cs
├── Domain/               # Entidades de negocio
│   ├── Owner.cs
│   ├── Property.cs
│   ├── PropertyImage.cs
│   └── PropertyTrace.cs
├── DTOs/                 # Data Transfer Objects
│   ├── OwnerDtos.cs
│   ├── PropertyDtos.cs
│   ├── PropertyFilterDto.cs
│   ├── PropertyImageDtos.cs
│   ├── PropertyTraceDtos.cs
│   └── PaginationDtos.cs
├── Interfaces/           # Contratos
│   ├── IRepository.cs
│   ├── IPropertyRepository.cs
│   ├── IPropertyService.cs
│   └── ... (otros)
├── Repository/           # Acceso a datos
│   ├── Repository.cs     # Genérico base
│   ├── PropertyRepository.cs
│   └── ... (otros)
├── Services/             # Lógica de negocio
│   ├── MongoDbService.cs
│   ├── PropertyService.cs
│   └── ... (otros)
├── Settings/             # Configuración
│   └── MongoDbSettings.cs
└── Program.cs            # Punto de entrada
```

### Frontend (million-front)

```
frontend/million-front/
├── app/                  # Next.js App Router
│   ├── layout.tsx       # Layout principal
│   ├── page.tsx         # Home page
│   ├── properties/      # Módulo propiedades
│   │   ├── page.tsx
│   │   └── manage/
│   └── owners/          # Módulo propietarios
├── src/
│   ├── core/            # Lógica de negocio
│   │   ├── property/
│   │   │   ├── domain/
│   │   │   ├── application/
│   │   │   ├── infrastructure/
│   │   │   └── __mock__/
│   │   ├── owner/
│   │   └── shared/
│   ├── infrastructure/  # API Client
│   │   └── api/
│   ├── presentation/    # UI Components
│   │   ├── components/
│   │   └── hooks/
│   └── shared/          # Constantes
├── public/              # Assets estáticos
├── coverage/            # Reportes de tests
└── package.json
```

---

## 🎨 Patrones y Principios

### Backend

#### 1. **Repository Pattern**
```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(string id);
    Task<T> CreateAsync(T entity);
    // ... más métodos
}
```

#### 2. **Dependency Injection**
```csharp
// Program.cs
builder.Services.AddSingleton<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
```

#### 3. **DTO Pattern**
```csharp
// Separación de entidades de dominio y respuestas API
public class PropertyDto
{
    public string IdProperty { get; set; }
    public string Name { get; set; }
    // Solo campos necesarios para el cliente
}
```

### Frontend

#### 1. **Hexagonal Architecture (Ports & Adapters)**

```typescript
// Port (Interface)
export interface PropertyRepository {
  getAll: (params?: PaginationParams) => Promise<PropertiesResponse>;
  getById: (id: string) => Promise<PropertyResponse>;
  // ...
}

// Adapter (Implementation)
export class AxiosPropertyRepository implements PropertyRepository {
  constructor(private apiClient: AxiosInstance) {}
  
  async getAll(params?: PaginationParams): Promise<PropertiesResponse> {
    // Implementación con axios
  }
}
```

#### 2. **Service Layer (Use Cases)**

```typescript
export class PropertyService {
  constructor(private repository: PropertyRepository) {}
  
  async getAllProperties(params?: PaginationParams) {
    return await this.repository.getAll(params);
  }
}
```

#### 3. **Custom Hooks para React Query**

```typescript
export function useProperties(params?: PaginationParams) {
  return useQuery({
    queryKey: ['properties', params],
    queryFn: () => propertyService.getAllProperties(params),
  });
}
```

---

## 🔒 Seguridad

- ✅ Validación de entrada en todos los endpoints
- ✅ CORS configurado apropiadamente
- ✅ Sanitización de datos en MongoDB queries
- ✅ HTTPS en producción (recomendado)
- 🔄 Autenticación JWT (próximamente)
- 🔄 Rate limiting (próximamente)

---

## 📝 Próximas Mejoras

- [ ] Autenticación y autorización con JWT
- [ ] Upload real de imágenes (Azure Blob / AWS S3)
- [ ] Notificaciones en tiempo real (SignalR)
- [ ] Exportar reportes a PDF/Excel
- [ ] Gráficos y estadísticas avanzadas
- [ ] PWA para uso offline
- [ ] Internacionalización (i18n)
- [ ] Rate limiting y caching (Redis)

---

## 📄 Licencia

Este proyecto es parte de una prueba técnica y está disponible con fines educativos.

---

## 📧 Contacto

**Camilo Vargas** - [@cavargasl](https://github.com/cavargasl)

**Link del Proyecto**: [https://github.com/cavargasl/million-properties](https://github.com/cavargasl/million-properties)

---

<div align="center">
  <strong>⭐ Si este proyecto te fue útil, considera darle una estrella ⭐</strong>
</div>
