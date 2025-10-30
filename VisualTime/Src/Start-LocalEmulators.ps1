# ========================================
# Script para iniciar emuladores locales
# VisualTime Development Environment
# ========================================

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " VisualTime - Emuladores Locales" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "SilentlyContinue"

# ========================================
# 1. VERIFICAR Y INICIAR REDIS
# ========================================
Write-Host "1. Redis Cache" -ForegroundColor Yellow
Write-Host "   -------------------------------------"

if (Get-Command docker -ErrorAction SilentlyContinue) {
    Write-Host "   ? Docker encontrado" -ForegroundColor Green
    
    # Verificar si el contenedor existe
    $redisContainer = docker ps -a --filter "name=redis-visualtime" --format "{{.Names}}"
    
    if ($redisContainer -eq "redis-visualtime") {
        # Contenedor existe, verificar si está corriendo
        $redisStatus = docker ps --filter "name=redis-visualtime" --format "{{.Status}}"
        
        if ($redisStatus) {
            Write-Host "   ? Redis ya está corriendo" -ForegroundColor Green
        } else {
            Write-Host "   ? Iniciando contenedor Redis existente..." -ForegroundColor Yellow
            docker start redis-visualtime
            Start-Sleep -Seconds 2
            Write-Host "   ? Redis iniciado" -ForegroundColor Green
        }
    } else {
        # Crear y ejecutar nuevo contenedor
        Write-Host "   ? Creando contenedor Redis..." -ForegroundColor Yellow
        docker run --name redis-visualtime -p 6379:6379 -d redis
        Start-Sleep -Seconds 3
        Write-Host "   ? Redis creado e iniciado" -ForegroundColor Green
    }
    
    Write-Host "   ?? Endpoint: localhost:6379" -ForegroundColor White
    
    # Verificar conexión
    $redisTest = docker exec redis-visualtime redis-cli ping 2>$null
    if ($redisTest -eq "PONG") {
        Write-Host "   ? Redis responde correctamente" -ForegroundColor Green
    }
} else {
    Write-Host "   ? Docker no encontrado" -ForegroundColor Red
    Write-Host "   ? Instalar desde: https://www.docker.com/products/docker-desktop" -ForegroundColor Cyan
    Write-Host "   ? Alternativa Windows: Memurai (https://www.memurai.com/)" -ForegroundColor Cyan
}

Write-Host ""

# ========================================
# 2. VERIFICAR Y INICIAR COSMOSDB EMULATOR
# ========================================
Write-Host "2. CosmosDB Emulator (MongoDB API)" -ForegroundColor Yellow
Write-Host "   -------------------------------------"

$cosmosPath = "C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe"

if (Test-Path $cosmosPath) {
    # Verificar si ya está corriendo
    $cosmosProcess = Get-Process -Name "CosmosDB.Emulator" -ErrorAction SilentlyContinue
    
    if ($cosmosProcess) {
        Write-Host "   ? CosmosDB Emulator ya está corriendo" -ForegroundColor Green
    } else {
        Write-Host "   ? Iniciando CosmosDB Emulator..." -ForegroundColor Yellow
        Start-Process $cosmosPath -WindowStyle Minimized
        Write-Host "   ? Espera 10-15 segundos para que inicie completamente..." -ForegroundColor Yellow
        Start-Sleep -Seconds 3
        Write-Host "   ? CosmosDB Emulator iniciando" -ForegroundColor Green
    }
    
    Write-Host "   ?? Endpoint: https://localhost:8081" -ForegroundColor White
    Write-Host "   ?? MongoDB: mongodb://localhost:10255" -ForegroundColor White
    Write-Host "   ?? Explorer: https://localhost:8081/_explorer/index.html" -ForegroundColor White
    Write-Host "   ? Primary Key: C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" -ForegroundColor DarkGray
} else {
    Write-Host "   ? CosmosDB Emulator no instalado" -ForegroundColor Red
    Write-Host "   ? Descargar desde: https://aka.ms/cosmosdb-emulator" -ForegroundColor Cyan
    Write-Host "   ? O instalar con: choco install azure-cosmosdb-emulator" -ForegroundColor Cyan
}

Write-Host ""

# ========================================
# 3. VERIFICAR SQL SERVER EXPRESS
# ========================================
Write-Host "3. SQL Server Express" -ForegroundColor Yellow
Write-Host "   -------------------------------------"

$sqlService = Get-Service -Name "MSSQL`$SQLEXPRESS" -ErrorAction SilentlyContinue

if ($sqlService) {
    if ($sqlService.Status -eq 'Running') {
        Write-Host "   ? SQL Server Express está corriendo" -ForegroundColor Green
    } else {
        Write-Host "   ? Iniciando SQL Server Express..." -ForegroundColor Yellow
        Start-Service "MSSQL`$SQLEXPRESS"
        Start-Sleep -Seconds 2
        Write-Host "   ? SQL Server Express iniciado" -ForegroundColor Green
    }
    Write-Host "   ?? Endpoint: localhost\SQLEXPRESS" -ForegroundColor White
    Write-Host "   ?? Database: VisualTime" -ForegroundColor White
} else {
    Write-Host "   ? SQL Server Express no encontrado" -ForegroundColor Red
    Write-Host "   ? Verificar instalación de SQL Server Express" -ForegroundColor Cyan
}

Write-Host ""

# ========================================
# RESUMEN FINAL
# ========================================
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Resumen de Servicios" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Redis Cache:    " -NoNewline -ForegroundColor White
if (docker ps --filter "name=redis-visualtime" --format "{{.Status}}") {
    Write-Host "? CORRIENDO" -ForegroundColor Green
} else {
    Write-Host "? NO DISPONIBLE" -ForegroundColor Red
}

Write-Host "  CosmosDB:       " -NoNewline -ForegroundColor White
if (Get-Process -Name "CosmosDB.Emulator" -ErrorAction SilentlyContinue) {
    Write-Host "? CORRIENDO" -ForegroundColor Green
} else {
    Write-Host "? NO DISPONIBLE" -ForegroundColor Red
}

Write-Host "  SQL Server:     " -NoNewline -ForegroundColor White
if ($sqlService -and $sqlService.Status -eq 'Running') {
    Write-Host "? CORRIENDO" -ForegroundColor Green
} else {
    Write-Host "? NO DISPONIBLE" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host " Endpoints" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "  Redis:          localhost:6379" -ForegroundColor White
Write-Host "  CosmosDB HTTPS: https://localhost:8081" -ForegroundColor White
Write-Host "  CosmosDB Mongo: mongodb://localhost:10255" -ForegroundColor White
Write-Host "  SQL Server:     localhost\SQLEXPRESS" -ForegroundColor White
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# ========================================
# COMANDOS ÚTILES
# ========================================
Write-Host "Comandos útiles:" -ForegroundColor Yellow
Write-Host "  • Ver logs Redis:    docker logs redis-visualtime" -ForegroundColor Gray
Write-Host "  • Parar Redis:       docker stop redis-visualtime" -ForegroundColor Gray
Write-Host "  • Conectar Redis:    docker exec -it redis-visualtime redis-cli" -ForegroundColor Gray
Write-Host "  • CosmosDB Explorer: https://localhost:8081/_explorer/index.html" -ForegroundColor Gray
Write-Host ""

# Pausar para que el usuario vea el resultado
Write-Host "Presiona cualquier tecla para continuar..." -ForegroundColor Cyan
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
