# Script completo para resetear y poblar la base de datos
# Solo requiere que la API esté corriendo - NO requiere MongoDB CLI

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Million Properties - Database Reset   " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "⚠️  ADVERTENCIA: Este script eliminará TODOS los datos de la base de datos" -ForegroundColor Red
Write-Host ""

# Preguntar confirmación
$confirmation = Read-Host "¿Desea continuar? Escriba 'si' para confirmar"
if ($confirmation -ne "si") {
    Write-Host "❌ Operación cancelada" -ForegroundColor Yellow
    Read-Host "Presiona Enter para salir"
    exit 0
}

Write-Host ""
Write-Host "Verificando conexión a la API..." -ForegroundColor Yellow

# Verificar si la API está corriendo
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5269/api/database/health" -Method GET -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ API está corriendo" -ForegroundColor Green
}
catch {
    Write-Host "❌ La API no está corriendo en http://localhost:5269" -ForegroundColor Red
    Write-Host "" 
    Write-Host "Por favor, inicia la API primero:" -ForegroundColor Yellow
    Write-Host "  1. Abre otra terminal PowerShell" -ForegroundColor White
    Write-Host "  2. Navega a: backend\Million.API\Million.API" -ForegroundColor White
    Write-Host "  3. Ejecuta: dotnet run" -ForegroundColor White
    Write-Host "  4. Espera a que diga 'Now listening on: http://localhost:5269'" -ForegroundColor White
    Write-Host "  5. Luego ejecuta este script nuevamente" -ForegroundColor White
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host ""
Write-Host "Reseteando y poblando base de datos..." -ForegroundColor Yellow
Write-Host "(Esto puede tomar unos segundos)" -ForegroundColor Gray

# Llamar al endpoint de reset
try {
    $resetResponse = Invoke-RestMethod -Uri "http://localhost:5269/api/database/reset" -Method POST -ContentType "application/json" -TimeoutSec 30
    
    if ($resetResponse.success) {
        Write-Host "✅ Base de datos reseteada y poblada exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Datos creados:" -ForegroundColor Cyan
        Write-Host "  • Propietarios: $($resetResponse.data.owners)" -ForegroundColor White
        Write-Host "  • Propiedades: $($resetResponse.data.properties)" -ForegroundColor White
        Write-Host "  • Imágenes: $($resetResponse.data.images)" -ForegroundColor White
        Write-Host "  • Trazas: $($resetResponse.data.traces)" -ForegroundColor White
    }
    else {
        Write-Host "❌ Error: $($resetResponse.message)" -ForegroundColor Red
    }
}
catch {
    Write-Host "❌ Error al resetear la base de datos" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Read-Host "Presiona Enter para salir"
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "         ✅ Proceso Completado          " -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Puedes verificar los datos en:" -ForegroundColor Yellow
Write-Host "  • Propiedades: GET http://localhost:5269/api/properties" -ForegroundColor White
Write-Host "  • Propietarios: GET http://localhost:5269/api/owners" -ForegroundColor White
Write-Host ""
Write-Host "Ejemplos de búsqueda:" -ForegroundColor Cyan
Write-Host "  curl http://localhost:5269/api/properties" -ForegroundColor Gray
Write-Host "  curl `"http://localhost:5269/api/properties/search?minPrice=300000000&maxPrice=600000000`"" -ForegroundColor Gray
Write-Host ""
Read-Host "Presiona Enter para salir"
