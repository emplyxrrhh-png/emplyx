# ? Configuración Completa de Servicios Locales - VisualTime

## ?? Resumen de Servicios Configurados

### Servicios Necesarios para VisualTime

| Servicio | Propósito | Emulador Local | Estado |
|----------|-----------|----------------|--------|
| **SQL Server Express** | Base de datos principal | ? Ya instalado | ? Configurado |
| **CosmosDB (MongoDB API)** | Caché distribuida | ? Emulador disponible | ?? Por instalar |
| **Redis Cache** | Caché de sesiones | ? Docker / Memurai | ?? Por instalar |
| **Application Insights** | Telemetría | ? Servicio real | ? Configurado |

---

## ?? OPCIÓN RECOMENDADA: Emuladores Locales

### Paso 1: Instalar Docker Desktop (si no lo tienes)
```powershell
# Descargar desde:
# https://www.docker.com/products/docker-desktop

# O con winget:
winget install Docker.DockerDesktop
```

### Paso 2: Ejecutar Script de Inicio Automático
```powershell
# Navegar a la carpeta del proyecto
cd D:\VsProyects\repos\VisualTime\Src

# Ejecutar script de inicio
.\Start-LocalEmulators.ps1
```

**El script automáticamente**:
- ? Inicia Redis en Docker
- ? Inicia CosmosDB Emulator (si está instalado)
- ? Verifica SQL Server Express
- ? Muestra resumen de todos los servicios

### Paso 3: Instalar CosmosDB Emulator

**Opción A: Instalador directo**
```powershell
# Descargar e instalar desde:
# https://aka.ms/cosmosdb-emulator
```

**Opción B: Con Chocolatey**
```powershell
choco install azure-cosmosdb-emulator
```

**Opción C: Usando el emulador en Docker** (alternativa)
```powershell
docker run -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254 `
  -m 3g --cpus=2.0 --name=cosmosdb-emulator `
  -e AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10 `
  mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator
```

### Paso 4: Configurar Base de Datos en CosmosDB

**Abrir CosmosDB Explorer**: https://localhost:8081/_explorer/index.html

**Crear base de datos**:
- Nombre: `VisualTimeCache`
- API: MongoDB

**Crear colecciones**:
1. `CompanyConfigurations` - Configuración de compañías
2. `ServiceConfigurations` - Configuración de servicios Azure
3. `CacheData` - Caché general
4. `SessionData` - Sesiones distribuidas

### Paso 5: Ejecutar la Aplicación
```powershell
# Todo está listo, ejecutar desde Visual Studio o IIS
```

---

## ?? OPCIÓN ALTERNATIVA: Sin Emuladores

Si no quieres instalar emuladores, puedes **deshabilitar** estos servicios temporalmente.

### Modificar Web.config

```xml
<appSettings>
  <!-- Deshabilitar CosmosDB -->
  <add key="CosmosDB.ConnectionString" value="" />
  <add key="CosmosDB.DBName" value="" />
  
  <!-- Redis: Usar caché en memoria (sin emulador) -->
  <add key="cacheName" value="mycache" />
  
  <!-- Application Insights: Mantener o deshabilitar -->
  <add key="ApplicationInsights.ConnectionString" value="" />
  <add key="ApplicationInsights.Key" value="" />
</appSettings>

<system.web>
  <!-- Session State: Usar modo InProc (en memoria) -->
  <sessionState mode="InProc" timeout="20" />
</system.web>
```

**Consecuencias de deshabilitar servicios**:
| Servicio | Consecuencia | Gravedad |
|----------|--------------|----------|
| CosmosDB | No habrá caché distribuida entre servicios | ?? Media |
| Redis | Sesiones solo funcionan en un servidor | ?? Media |
| App Insights | No habrá telemetría en Azure | ?? Baja |

**Ventajas**:
- ? Menos servicios que mantener
- ? Más simple para desarrollo básico
- ? No requiere Docker ni instalaciones adicionales

**Recomendado para**:
- Desarrollo inicial
- Pruebas de funcionalidad básica
- Cuando solo trabajas en un solo servidor

---

## ?? Configuración Actual del Web.config

### ? Ya Configurado

```xml
<appSettings>
  <!-- Base de datos principal -->
  <add key="ApiService.ConnectionString" value="Data Source=.\SQLEXPRESS;Initial Catalog=VisualTime;User ID=sa;Password=Ab75007500;..." />
  
  <!-- CosmosDB Emulator Local -->
  <add key="CosmosDB.ConnectionString" value="mongodb://localhost:C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==@localhost:10255/?ssl=true&amp;retrywrites=false" />
  <add key="CosmosDB.DBName" value="VisualTimeCache" />
  
  <!-- Redis Local -->
  <add key="cacheName" value="localhost:6379" />
  <add key="Redis.ConnectionString" value="localhost:6379,ssl=False,abortConnect=False" />
  
  <!-- Application Insights -->
  <add key="ApplicationInsights.ConnectionString" value="InstrumentationKey=6825e043-9845-40f5-b5d0-74f42d3aeb32;..." />
  <add key="ApplicationInsights.Key" value="6825e043-9845-40f5-b5d0-74f42d3aeb32" />
</appSettings>
```

---

## ?? Inicio Rápido - 3 Comandos

### Si tienes Docker instalado:
```powershell
# 1. Iniciar Redis
docker run --name redis-visualtime -p 6379:6379 -d redis

# 2. Ejecutar script de verificación
.\Start-LocalEmulators.ps1

# 3. Abrir Visual Studio y ejecutar VTLive40
```

### Si NO tienes Docker:
```powershell
# 1. Modificar Web.config para deshabilitar Redis/CosmosDB
# (ver sección "Sin Emuladores" arriba)

# 2. Verificar SQL Server
Get-Service MSSQL$SQLEXPRESS

# 3. Ejecutar desde Visual Studio
```

---

## ?? Verificación de Servicios

### Verificar que todo está corriendo

```powershell
# Redis
docker ps | findstr redis-visualtime

# CosmosDB
Get-Process -Name "CosmosDB.Emulator"

# SQL Server
Get-Service MSSQL$SQLEXPRESS

# Probar conexión Redis
docker exec -it redis-visualtime redis-cli ping
# Debe responder: PONG
```

### Abrir exploradores web

```powershell
# CosmosDB Explorer
Start-Process "https://localhost:8081/_explorer/index.html"

# Application Insights (Azure Portal)
Start-Process "https://portal.azure.com"
```

---

## ?? Archivos Creados

1. ? `Start-LocalEmulators.ps1` - Script de inicio automático
2. ? `Local-Emulators-Guide.md` - Guía detallada completa
3. ? `Quick-Start.md` - Esta guía de inicio rápido
4. ? `Web.config` - Ya configurado con todos los endpoints

---

## ?? Troubleshooting

### Error: "No se puede conectar a Redis"
```powershell
# Verificar que Docker está corriendo
docker ps

# Reiniciar contenedor Redis
docker restart redis-visualtime
```

### Error: "CosmosDB SSL Certificate"
- Al abrir https://localhost:8081, acepta el certificado auto-firmado
- Es normal en el emulador local

### Error: "MongoDB connection refused"
- Espera 10-15 segundos después de iniciar CosmosDB Emulator
- El emulador tarda en arrancar completamente

### Error: "SQL Server no responde"
```powershell
# Iniciar servicio
Start-Service MSSQL$SQLEXPRESS

# Verificar estado
Get-Service MSSQL$SQLEXPRESS
```

---

## ?? Documentación Adicional

- **Guía Completa**: `Local-Emulators-Guide.md`
- **CosmosDB Emulator Docs**: https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator
- **Redis Docs**: https://redis.io/docs/getting-started/
- **Application Insights**: `ApplicationInsights-Configuration.md`

---

## ? Checklist de Configuración

- [ ] Docker Desktop instalado
- [ ] Redis corriendo en Docker
- [ ] CosmosDB Emulator instalado y corriendo
- [ ] SQL Server Express corriendo
- [ ] Web.config configurado
- [ ] Script `Start-LocalEmulators.ps1` ejecutado
- [ ] Base de datos `VisualTimeCache` creada en CosmosDB
- [ ] Colecciones creadas en CosmosDB
- [ ] Aplicación ejecutándose sin errores

---

## ?? Siguiente Paso

Una vez que todos los emuladores estén corriendo:

```powershell
# Ejecutar la aplicación
cd D:\VsProyects\repos\VisualTime\Src

# Abrir Visual Studio
start VTLive40\VTLive40.vbproj

# O ejecutar desde IIS
```

¡Todo listo para desarrollo local! ??
