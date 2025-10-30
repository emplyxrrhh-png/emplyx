param(
    [string]$CosmosMongoConnection = "mongodb://localhost:C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==@localhost:10255/?ssl=true&retrywrites=false",
    [string]$DatabaseName = "VisualTimeCache",
    [string]$SeedFolder = "$PSScriptRoot\"
)

Write-Host "Importando datos a CosmosDB (Mongo API)" -ForegroundColor Cyan
Write-Host "Conexion: $CosmosMongoConnection" -ForegroundColor White
Write-Host "Database: $DatabaseName" -ForegroundColor White

# Requiere que mongo tools (mongoimport) esten instalados.
# Comprobar si mongoimport existe
$mongoImport = Get-Command mongoimport -ErrorAction SilentlyContinue

if (-not $mongoImport) {
    Write-Host "mongoimport no encontrado. Instalando MongoDB Database Tools..." -ForegroundColor Yellow
    Write-Host "Descarga manual: https://www.mongodb.com/try/download/database-tools" -ForegroundColor Gray
    Write-Host "O instalar con choco: choco install mongodb-database-tools" -ForegroundColor Gray
    return
}

# Importar colecciones
$files = @(
    @{ File = "CompanyConfigurations.json"; Collection = "CompanyConfigurations" },
    @{ File = "ServiceConfigurations.json"; Collection = "ServiceConfigurations" },
    @{ File = "CacheData.json"; Collection = "CacheData" },
    @{ File = "SessionData.json"; Collection = "SessionData" }
)

foreach ($f in $files) {
    $path = Join-Path $SeedFolder $f.File
    if (-not (Test-Path $path)) {
        Write-Host "Archivo no encontrado: $path" -ForegroundColor Yellow
        continue
    }

    Write-Host "Importando $($f.File) -> $($f.Collection)" -ForegroundColor Green

    & mongoimport --uri "$CosmosMongoConnection" --db $DatabaseName --collection $($f.Collection) --drop --file $path --jsonArray

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error importando $($f.File)" -ForegroundColor Red
    } else {
        Write-Host "Importado $($f.File) correctamente" -ForegroundColor Green
    }
}

Write-Host "Importación completada" -ForegroundColor Cyan
