# Million Properties - Frontend

Sistema de gestión de propiedades inmobiliarias construido con **Next.js 15** y **arquitectura HYFORCE** (Hexagonal + Clean Architecture).

## 🏗️ Arquitectura

El proyecto sigue la arquitectura **HYFORCE** con separación clara de responsabilidades:

- **Core**: Lógica de negocio pura (domain, application, infrastructure)
- **Presentation**: Componentes React y UI
- **Infrastructure**: Clientes API y servicios externos

📖 **Documentación completa**: [HYFORCE-ARCHITECTURE.md](./HYFORCE-ARCHITECTURE.md)

## 🚀 Tecnologías

- **Next.js 15** - Framework React con App Router
- **TypeScript** - Tipado estático
- **Tailwind CSS 4** - Estilos utility-first
- **React Query (TanStack Query)** - Gestión de estado del servidor
- **Axios** - Cliente HTTP para API REST
- **Lucide React** - Iconos
- **Arquitectura HYFORCE** - Hexagonal + Clean Architecture

## 📦 Instalación

```bash
# Instalar dependencias
pnpm install

# Configurar variables de entorno
cp .env.local.example .env.local

# Iniciar servidor de desarrollo
pnpm dev
```

## 🎯 Características

- ✅ Arquitectura hexagonal para mejor mantenibilidad
- ✅ Separación clara de capas (Domain, Application, Infrastructure)
- ✅ Tipado fuerte con TypeScript
- ✅ Gestión de estado con React Query
- ✅ Cliente Axios configurado con interceptors
- ✅ Diseño responsive con Tailwind CSS
- ✅ Componentes reutilizables
- ✅ Integración con API REST del backend .NET

## 🔧 Scripts

```bash
pnpm dev      # Iniciar en modo desarrollo (puerto 3000)
pnpm build    # Construir para producción
pnpm start    # Iniciar servidor de producción
pnpm lint     # Ejecutar linter
```

## 📁 Estructura del Proyecto

```
src/
├── core/                      # Lógica de negocio
│   ├── property/             # Módulo de propiedades
│   ├── owner/                # Módulo de propietarios
│   └── shared/               # Código compartido
├── infrastructure/            # Servicios externos
│   └── api/                  # Cliente axios
├── presentation/              # UI y componentes
│   ├── components/
│   └── hooks/
└── shared/                    # Constantes globales
```

## 🌐 Integración con Backend

El frontend se conecta al backend .NET en `http://localhost:5000/api`

### Endpoints disponibles:

- **GET** `/api/properties` - Listar propiedades
- **POST** `/api/properties` - Crear propiedad
- **PUT** `/api/properties/{id}` - Actualizar propiedad
- **DELETE** `/api/properties/{id}` - Eliminar propiedad
- **GET** `/api/owners` - Listar propietarios
- **POST** `/api/owners` - Crear propietario

## 🎨 Componentes Principales

### Layout
- **Header**: Navegación principal con menú responsive
- **Footer**: Footer con links y redes sociales
- **Providers**: Configuración de React Query

### Pages
- **Home**: Hero section con características destacadas
- **Properties**: Listado y gestión de propiedades (próximamente)
- **Owners**: Listado y gestión de propietarios (próximamente)

## 🧪 Ejemplo de Uso

```typescript
import { PropertyService } from '@/core/property';
import { axiosPropertyRepository } from '@/core/property';

// Crear instancia del servicio
const propertyService = PropertyService(axiosPropertyRepository);

// Usar en React Query
const { data, error } = useQuery({
  queryKey: ['properties'],
  queryFn: async () => {
    const result = await propertyService.getAll();
    if (result.error) throw result.error;
    return result.data;
  },
});
```

## 📝 Variables de Entorno

```env
NEXT_PUBLIC_API_URL=http://localhost:5000/api
```

## 🔄 Flujo de Datos

```
Component → Hook → Service → Repository → Axios → API Backend
    ↑                                         ↓
    └────────────── Adapter ← DTO ← Response ←
```

## 📚 Recursos

- [Next.js Documentation](https://nextjs.org/docs)
- [TanStack Query](https://tanstack.com/query/latest)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [Axios](https://axios-http.com/docs/intro)

## 📄 Licencia

MIT
