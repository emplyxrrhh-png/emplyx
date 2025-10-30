# Guía de Configuración de Emuladores Locales - VisualTime

## Estado Actual del Sistema

El sistema VisualTime utiliza los siguientes servicios de Azure en producción:
- **CosmosDB (MongoDB API)**: Caché distribuida y configuración compartida
- **Redis Cache**: Caché de alto rendimiento 
- **Application Insights**: Telemetría y logs

Para desarrollo local, podemos usar **emuladores gratuitos**.

---

## 1. CosmosDB Emulator (MongoDB API)

### ¿Qué datos almacena?
- Configuración de compañías (roCompanyConfiguration)
- Caché de permisos y usuarios
- Metadatos de aplicación
- Configuración de servicios Azure
- Parámetros del sistema compartidos

### Instalación del Emulador

#### Opción A: Azure Cosmos DB Emulator (Windows)
```powershell
# Descargar e instalar desde:
# https://aka.ms/cosmosdb-emulator

# O con Chocolatey:
choco install azure-cosmosdb-emulator

# Iniciar el emulador
Start-CosmosDbEmulator
```

**Configuración por defecto:**
- Endpoint: `https://localhost:8081`
- Primary Key: `C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==`
- MongoDB Connection: `mongodb://localhost:C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==@localhost:10255/?ssl=true`

#### Opción B: CosmosDB en Docker (Multiplataforma)
```bash
# Descargar imagen
docker pull mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator

# Ejecutar contenedor
docker run -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254 \
  -m 3g --cpus=2.0 --name=cosmosdb-emulator \
  -e AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10 \
  -e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true \
  mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
```

### Configurar en Web.config

```xml
<appSettings>
  <!-- CosmosDB Emulator Local -->
  <add key="CosmosDB.ConnectionString" value="mongodb://localhost:C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==@localhost:10255/?ssl=true&amp;retrywrites=false" />
  <add key="CosmosDB.DBName" value="VisualTimeCache" />
  
  <!-- Alternativa: Deshabilitar CosmosDB en desarrollo -->
  <!-- <add key="CosmosDB.ConnectionString" value="" /> -->
  <!-- <add key="CosmosDB.DBName" value="" /> -->
</appSettings>
```

### Datos Iniciales a Crear

Cuando arranques el emulador, necesitarás crear:

**Base de datos**: `VisualTimeCache`

**Colecciones principales**:
1. **CompanyConfigurations** - Configuración de compañías
   ```json
   {
     "_id": "70922-9000",
     "CompanyId": "70922-9000",
     "CompanyName": "Development Company",
     "ApiConnectionString": "Data Source=.\\SQLEXPRESS;Initial Catalog=VisualTime;...",
     "IsActive": true,
     "Features": {...}
   }
   ```

2. **ServiceConfigurations** - Configuración de servicios Azure
   ```json
   {
     "_id": "analytics",
     "QueueType": "analytics",
     "ServiceBusConnectionString": "",
     "ServiceBusQueueName": "",
     "StorageConnectionString": "",
     "StorageBlobContainer": ""
   }
   ```

3. **CacheData** - Datos de caché general
4. **SessionData** - Datos de sesión distribuida

---

## 2. Redis Cache Emulator

### ¿Qué datos almacena?
- Caché de objetos de sesión (ASP.NET Session State)
- Caché de configuración temporal
- Datos frecuentemente accedidos

### Opción A: Redis en Docker (Recomendado)

```powershell
# Descargar imagen de Redis
docker pull redis:latest

# Ejecutar Redis
docker run --name redis-visualtime -p 6379:6379 -d redis

# Verificar que está corriendo
docker ps

# Conectar para probar
docker exec -it redis-visualtime redis-cli
# > ping
# PONG
```

### Opción B: Memurai (Redis para Windows)

```powershell
# Descargar desde: https://www.memurai.com/get-memurai
# O con Chocolatey:
choco install memurai-developer

# Iniciar servicio
net start Memurai
```

### Opción C: Redis Stack con RedisInsight (GUI incluida)

```powershell
docker run -d --name redis-stack -p 6379:6379 -p 8001:8001 redis/redis-stack:latest
```

Acceder a la GUI: `http://localhost:8001`

### Configurar en Web.config

```xml
<appSettings>
  <!-- Redis Local -->
  <add key="cacheName" value="localhost:6379" />
  <add key="Redis.ConnectionString" value="localhost:6379,password=,ssl=False,abortConnect=False" />
  
  <!-- Session State con Redis -->
</appSettings>

<system.web>
  <sessionState mode="Custom" customProvider="RedisSessionProvider">
    <providers>
      <add name="RedisSessionProvider" 
           type="Microsoft.Web.Redis.RedisSessionStateProvider" 
           host="localhost" 
           port="6379" 
           accessKey="" 
           ssl="false" />
    </providers>
  </sessionState>
</system.web>
```

---

## 3. Application Insights

### Opciones para Desarrollo Local

#### Opción A: Usar el servicio real (Ya configurado ?)
Connection string ya está configurado con la key que proporcionaste.

#### Opción B: Deshabilitar Application Insights

```xml
<appSettings>
  <!-- Deshabilitar Application Insights en local -->
  <add key="ApplicationInsights.ConnectionString" value="" />
  <add key="ApplicationInsights.Key" value="" />
</appSettings>
```

También comentar en `ApplicationInsights.config`:
```xml
<ApplicationInsights xmlns="http://schemas.microsoft.com/ApplicationInsights/2013/Settings">
  <!-- <ConnectionString>...</ConnectionString> -->
  <!-- <InstrumentationKey>...</InstrumentationKey> -->
  <TelemetryChannel Type="Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel.ServerTelemetryChannel, Microsoft.AI.ServerTelemetryChannel">
    <DeveloperMode>true</DeveloperMode>
  </TelemetryChannel>
</ApplicationInsights>
```

#### Opción C: Application Insights para desarrollo local
Si quieres ver logs localmente sin enviar a Azure:

```csharp
// En Global.asax.cs o Startup
TelemetryConfiguration.Active.DisableTelemetry = true;
```

---

## ?? Configuración Completa para Desarrollo Local

### Script de inicio rápido

Crear archivo: `Start-LocalEmulators.ps1`

```powershell
# Script para iniciar todos los emuladores locales

Write-Host "Iniciando emuladores locales para VisualTime..." -ForegroundColor Green

# 1. Verificar Docker
if (Get-Command docker -ErrorAction SilentlyContinue) {
    Write-Host "? Docker encontrado" -ForegroundColor Green
    
    # Iniciar Redis
    Write-Host "Iniciando Redis..." -ForegroundColor Yellow
    docker start redis-visualtime 2>$null
    if ($LASTEXITCODE -ne 0) {
        docker run --name redis-visualtime -p 6379:6379 -d redis
    }
    Write-Host "? Redis corriendo en localhost:6379" -ForegroundColor Green
    
} else {
    Write-Host "? Docker no encontrado. Instalar desde: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
}

# 2. Iniciar CosmosDB Emulator
Write-Host "Iniciando CosmosDB Emulator..." -ForegroundColor Yellow
if (Test-Path "C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe") {
    Start-Process "C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe"
    Write-Host "? CosmosDB Emulator iniciando en https://localhost:8081" -ForegroundColor Green
    Write-Host "  Espera unos segundos para que inicie completamente..." -ForegroundColor Yellow
} else {
    Write-Host "? CosmosDB Emulator no instalado" -ForegroundColor Yellow
    Write-Host "  Descargar desde: https://aka.ms/cosmosdb-emulator" -ForegroundColor Cyan
}

# 3. Verificar SQL Server
Write-Host "Verificando SQL Server..." -ForegroundColor Yellow
$sqlService = Get-Service -Name "MSSQL`$SQLEXPRESS" -ErrorAction SilentlyContinue
if ($sqlService -and $sqlService.Status -eq 'Running') {
    Write-Host "? SQL Server Express corriendo" -ForegroundColor Green
} else {
    Write-Host "? SQL Server Express no está corriendo" -ForegroundColor Yellow
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Resumen de servicios:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Redis:     localhost:6379" -ForegroundColor White
Write-Host "CosmosDB:  https://localhost:8081" -ForegroundColor White
Write-Host "SQL:       localhost\SQLEXPRESS" -ForegroundColor White
Write-Host "========================================" -ForegroundColor Cyan
```

### Ejecutar el script

```powershell
.\Start-LocalEmulators.ps1
```

---

## ?? Configuración Final de Web.config

```xml
<configuration>
  <connectionStrings>
    <add name="VisualTimeConnectionString" 
         connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=VisualTime;User ID=sa;Password=Ab75007500;MultipleActiveResultSets=True;Connect Timeout=30;" 
         providerName="System.Data.SqlClient" />
  </connectionStrings>
  
  <appSettings>
    <!-- Base de datos principal -->
    <add key="ApiService.ConnectionString" value="Data Source=.\SQLEXPRESS;Initial Catalog=VisualTime;User ID=sa;Password=Ab75007500;MultipleActiveResultSets=True;Connect Timeout=30;" />
    
    <!-- CosmosDB Emulator Local -->
    <add key="CosmosDB.ConnectionString" value="mongodb://localhost:C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==@localhost:10255/?ssl=true&amp;retrywrites=false" />
    <add key="CosmosDB.DBName" value="VisualTimeCache" />
    
    <!-- Redis Local -->
    <add key="cacheName" value="localhost:6379" />
    <add key="Redis.ConnectionString" value="localhost:6379,ssl=False,abortConnect=False" />
    
    <!-- Application Insights (Real) -->
    <add key="ApplicationInsights.ConnectionString" value="InstrumentationKey=6825e043-9845-40f5-b5d0-74f42d3aeb32;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=e5454841-74a2-4a66-b30d-8cc50893b21f" />
    <add key="ApplicationInsights.Key" value="6825e043-9845-40f5-b5d0-74f42d3aeb32" />
    
    <!-- Configuración general -->
    <add key="IISDistributedEnabled" value="true" />
    <add key="DebugMode" value="true" />
    <add key="URL.VTLive" value="http://localhost/VTLive40" />
    <add key="URL.VTPortal" value="http://localhost/VTPortalWeb" />
  </appSettings>
  
  <!-- Session State con Redis -->
  <system.web>
    <sessionState mode="Custom" customProvider="RedisSessionProvider">
      <providers>
        <add name="RedisSessionProvider" 
             type="Microsoft.Web.Redis.RedisSessionStateProvider" 
             host="localhost" 
             port="6379" 
             accessKey="" 
             ssl="false" />
      </providers>
    </sessionState>
  </system.web>
</configuration>
```

---

## ?? Verificación de Servicios

### Verificar Redis
```powershell
# Conectar a Redis
docker exec -it redis-visualtime redis-cli

# Probar comandos
> ping
PONG
> set test "hello"
OK
> get test
"hello"
```

### Verificar CosmosDB
1. Abrir navegador: `https://localhost:8081/_explorer/index.html`
2. Aceptar el certificado SSL (auto-firmado)
3. Crear base de datos `VisualTimeCache`
4. Crear colecciones necesarias

### Verificar SQL Server
```powershell
sqlcmd -S localhost\SQLEXPRESS -d VisualTime -E -Q "SELECT @@VERSION"
```

---

## ?? Paquetes NuGet Necesarios

Si no están instalados, agregar a los proyectos:

```powershell
# Para Redis
Install-Package Microsoft.Web.Redis.RedisSessionStateProvider
Install-Package StackExchange.Redis

# Para CosmosDB (MongoDB API)
Install-Package MongoDB.Driver

# Para Application Insights (ya instalado)
Install-Package Microsoft.ApplicationInsights
Install-Package Microsoft.ApplicationInsights.Web
```

---

## ? Inicio Rápido

1. **Instalar Docker Desktop** (si no lo tienes)
2. **Ejecutar Redis en Docker**:
   ```powershell
   docker run --name redis-visualtime -p 6379:6379 -d redis
   ```

3. **Instalar CosmosDB Emulator**:
   - Descargar: https://aka.ms/cosmosdb-emulator
   - Ejecutar instalador
   - Iniciar emulador

4. **Configurar Web.config** (usar la configuración de arriba)

5. **Iniciar aplicación**

---

## ?? Alternativa Simplificada (Sin emuladores)

Si no quieres instalar emuladores, puedes **deshabilitar** estos servicios:

```xml
<appSettings>
  <!-- Deshabilitar CosmosDB -->
  <add key="CosmosDB.ConnectionString" value="" />
  <add key="CosmosDB.DBName" value="" />
  
  <!-- Deshabilitar Application Insights -->
  <add key="ApplicationInsights.ConnectionString" value="" />
  <add key="ApplicationInsights.Key" value="" />
  
  <!-- Usar Session State en memoria (InProc) -->
</appSettings>

<system.web>
  <sessionState mode="InProc" timeout="20" />
</system.web>
```

**Consecuencias**:
- ? La aplicación funcionará
- ?? No habrá caché distribuida
- ?? Sesiones solo funcionan en un servidor
- ?? No habrá telemetría

---

## ?? Referencias

- [CosmosDB Emulator](https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator)
- [Redis Docker](https://hub.docker.com/_/redis)
- [Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net)
- [Redis Session State Provider](https://github.com/Azure/aspnet-redis-providers)
