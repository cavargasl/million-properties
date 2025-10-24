# Script simplificado para poblar la base de datos
# Solo requiere que la API esté corriendo - NO requiere MongoDB CLI

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Million Properties - Database Seed   " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "Este script poblará la base de datos con datos de prueba." -ForegroundColor Yellow
Write-Host "Requiere que la API esté corriendo en http://localhost:5269" -ForegroundColor Yellow
Write-Host ""

# Verificar si la API está corriendo
Write-Host "Verificando conexión a la API..." -ForegroundColor Yellow

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
Write-Host "Poblando base de datos con datos de prueba..." -ForegroundColor Yellow

# Llamar al endpoint de seed
try {
    $seedResponse = Invoke-RestMethod -Uri "http://localhost:5269/api/database/seed" -Method POST -ContentType "application/json"
    
    if ($seedResponse.success) {
        Write-Host "✅ Base de datos poblada exitosamente!" -ForegroundColor Green
        Write-Host ""
        Write-Host "Datos creados:" -ForegroundColor Cyan
        Write-Host "  • Propietarios: $($seedResponse.data.owners)" -ForegroundColor White
        Write-Host "  • Propiedades: $($seedResponse.data.properties)" -ForegroundColor White
        Write-Host "  • Imágenes: $($seedResponse.data.images)" -ForegroundColor White
        Write-Host "  • Trazas: $($seedResponse.data.traces)" -ForegroundColor White
    }
    else {
        Write-Host "⚠️  $($seedResponse.message)" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Nota: Si ya existen datos, el seed no cargará duplicados." -ForegroundColor Gray
    }
}
catch {
    Write-Host "❌ Error al poblar la base de datos" -ForegroundColor Red
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
Write-Host "  • Swagger: http://localhost:5269/swagger" -ForegroundColor White
Write-Host "  • Endpoint: GET http://localhost:5269/api/properties" -ForegroundColor White
Write-Host ""
Write-Host "Para eliminar y recargar datos, usa el endpoint:" -ForegroundColor Yellow
Write-Host "  • DELETE http://localhost:5269/api/database/reset (si lo implementas)" -ForegroundColor Gray
Write-Host ""
Read-Host "Presiona Enter para salir"
