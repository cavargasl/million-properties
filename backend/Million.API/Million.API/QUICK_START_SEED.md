# 🚀 Quick Start - Database Seeding

Guía rápida para poblar la base de datos con datos de prueba en menos de 1 minuto.

## ⚡ Inicio Rápido (1 minuto)

### Windows (PowerShell)

```powershell
# 1. Asegúrate de que MongoDB esté corriendo
net start MongoDB

# 2. Inicia la API (en una terminal)
cd backend/Million.API/Million.API
dotnet run

# 3. En otra terminal, ejecuta el seed
cd backend/Million.API/Million.API
.\seed-database.ps1

# ✅ ¡Listo! Datos cargados
```

### Opción Alternativa: Endpoint API Directo

```bash
# Solo cargar datos
curl -X POST http://localhost:5269/api/database/seed

# Resetear y cargar (⚠️ elimina todo)
curl -X POST http://localhost:5269/api/database/reset
```

## 🎯 Verificación Rápida

### Opción 1: Swagger UI (Visual)
1. Abre: `http://localhost:5269/swagger`
2. Prueba `GET /api/properties`
3. Deberías ver 15 propiedades

### Opción 2: curl (Terminal)
```bash
# Ver propiedades
curl http://localhost:5269/api/properties

# Ver propietarios
curl http://localhost:5269/api/owners

# Buscar por precio
curl "http://localhost:5269/api/properties/search?minPrice=300000000&maxPrice=600000000"
```

### Opción 3: MongoDB Compass (Visual)
1. Conecta a: `mongodb://localhost:27017`
2. Abre base de datos: `million_db`
3. Verifica colecciones:
   - `owners` → 8 documentos
   - `properties` → 15 documentos
   - `propertyImages` → 30-60 documentos
   - `propertyTraces` → 15-45 documentos

## 🔄 Solo Seed (Sin Resetear)

Si ya tienes la base de datos y solo quieres cargar datos:

```powershell
# Windows PowerShell
.\seed-database.ps1

# O endpoint directo
curl -X POST http://localhost:5269/api/database/seed
```

**Nota**: Si ya existen datos, el seed no hará nada (para evitar duplicados).

## 🧹 Limpiar y Volver a Cargar

```powershell
# Windows PowerShell (⚠️ Elimina TODO)
.\reset-and-seed.ps1

# O endpoint directo
curl -X POST http://localhost:5269/api/database/reset
```

Esto:
1. ✅ Elimina toda la base de datos
2. ✅ Carga datos frescos
3. ✅ Te muestra un resumen

## ❌ Troubleshooting Rápido

### "MongoDB no está corriendo"
```bash
# Windows
net start MongoDB

# Linux/Mac
sudo systemctl start mongod

# Docker
docker run -d -p 27017:27017 --name mongodb mongo:latest
```

### "API no está corriendo"
```bash
cd backend/Million.API/Million.API
dotnet run
# Espera a que diga: "Now listening on: http://localhost:5269"
```

### "Error al conectar con la API"
```powershell
# Verifica que la API esté corriendo
# Deberías ver: "Now listening on: http://localhost:5269"
cd backend/Million.API/Million.API
dotnet run
```

## 📊 ¿Qué Datos se Cargan?

```
✅ 8 Propietarios
   • Juan Pérez García (Bogotá)
   • María Rodríguez López (Medellín)
   • Carlos Martínez Sánchez (Cali)
   • ... y 5 más

✅ 15 Propiedades
   • Apartamento Moderno en Chapinero ($450M)
   • Casa Campestre en La Calera ($850M)
   • Penthouse El Poblado Premium ($1,200M)
   • ... y 12 más

✅ 2-4 Imágenes por propiedad
   • URLs de Unsplash (fotos reales)
   • Primera imagen habilitada por defecto

✅ 1-3 Trazas por propiedad
   • Historial de ventas
   • Actualizaciones de precio
   • Renovaciones
```

## 🎮 Siguiente Paso

Una vez cargados los datos, inicia el frontend:

```bash
cd frontend/million-front
pnpm install
pnpm dev

# Abre: http://localhost:3000
```

## 📖 Documentación Completa

- [INICIO_RAPIDO.md](./INICIO_RAPIDO.md) - Guía rápida en español
- [DATABASE_SEED.md](./DATABASE_SEED.md) - Guía detallada completa
- [README.md](./README.md) - Documentación del backend
- [mock-data-reference.json](./mock-data-reference.json) - Referencia de datos

## 💡 Tips

1. **Primera vez**: Usa `.\seed-database.ps1` para cargar datos iniciales
2. **Reset completo**: Usa `.\reset-and-seed.ps1` cuando necesites datos limpios
3. **Testing**: El endpoint `/api/database/reset` es útil en tests automatizados
4. **Producción**: ⚠️ NUNCA uses estos endpoints/scripts en producción
5. **Sin MongoDB CLI**: No necesitas `mongo` ni `mongosh` instalados

## 🆘 ¿Necesitas Ayuda?

1. Verifica los logs de la API: Busca errores en la consola donde ejecutaste `dotnet run`
2. Verifica MongoDB: `mongosh` → `show dbs` → debe aparecer `million_db`
3. Verifica el endpoint: Abre `http://localhost:5269/api/database/health`

---

<div align="center">
  <strong>Happy Coding! 🚀</strong>
</div>
