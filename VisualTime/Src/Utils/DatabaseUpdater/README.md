# Guía de Actualización de Base de Datos - VisualTime

## Estado Actual

### Información de la Base de Datos
- **Nombre**: VisualTime
- **Servidor**: localhost\SQLEXPRESS  
- **Versión de BD**: 398 (según última verificación)
- **Usuario**: sa
- **ID Único**: 70922-9000

### Proyectos Compilados
- ? **92 proyectos principales** compilados correctamente
- ? **7 proyectos de pruebas** con errores (requieren Microsoft Fakes - no necesarios para producción)

## Problema Identificado

El comando `sqlcmd` se cuelga al intentar ejecutarlo desde PowerShell. Esto puede deberse a:
1. Timeout de la consulta
2. Problemas de permisos
3. Conflictos con la sesión de PowerShell

## Soluciones Propuestas

### Opción 1: Verificación Manual con SQL Server Management Studio

1. Abrir **SQL Server Management Studio (SSMS)**
2. Conectarse a `localhost\SQLEXPRESS`
3. Ejecutar el script: `Utils\DatabaseUpdater\VerifyDatabaseVersion.sql`

Este script te mostrará:
- Versión actual de la base de datos
- Información general del sistema
- Última ejecución de triggers
- Opciones de configuración

### Opción 2: Programa DatabaseUpdater (Simplificado)

Se ha creado un programa de consola que **NO requiere Azure Functions Core Tools** ni **Application Insights**:

**Ubicación**: `Utils\DatabaseUpdater\`

**Para compilar**:
```powershell
cd D:\VsProyects\repos\VisualTime\Src
dotnet build Utils\DatabaseUpdater\DatabaseUpdater.csproj
```

**Para ejecutar**:
```powershell
cd Utils\DatabaseUpdater\bin\Debug\net48
.\DatabaseUpdater.exe
```

**Nota**: Este programa solo **verifica** la versión actual. No ejecuta actualizaciones automáticas porque requiere configuración de Azure.

### Opción 3: Ejecutar Actualizaciones Manualmente

Los scripts de actualización están en:
```
Common\DBScript\SQLUpgradeVersion\
```

Estructura:
- `Informes DX\` - Scripts de informes (01 al 45)
- `Partials\` - Scripts parciales
- `Custom\` - Scripts personalizados  
- `Database Texts\` - Textos multiidioma

Para aplicar actualizaciones manualmente:
1. Identificar scripts necesarios según la versión actual (398)
2. Ejecutar scripts en orden en SSMS
3. Actualizar el valor de `DBVERSION` en la tabla `sysroParameters`

## Sistema de Actualización Automática

El sistema VisualTime utiliza **Azure Functions** para actualizar automáticamente:

### Configuración Requerida (No Disponible Actualmente)

Para que las actualizaciones automáticas funcionen, se necesita:

1. **Application Insights** - Para logging
2. **Azure Service Bus** - Para colas de tareas
3. **Azure Blob Storage** - Para almacenar scripts
4. **Azure Functions Core Tools** - Para ejecutar localmente

### Azure Functions de Actualización

- `roScheduleFunctions\RunDatabaseUpdater.cs` - Actualiza base de datos
- Ejecuta cada hora en el minuto 20 (Cron: `0 20 * * * *`)
- Descarga scripts desde Azure Blob Storage
- Ejecuta scripts pendientes según versión

## Recomendaciones

### Para Desarrollo Local

1. **Usar SQL Server Management Studio** para:
   - Verificar versión de base de datos
   - Ejecutar scripts manualmente
   - Monitorear cambios

2. **Ejecutar la aplicación web** (VTLive40):
   - La aplicación principal está compilada
   - No requiere proyectos de pruebas
   - Funcionará con la BD en versión 398

### Para Configurar Actualización Automática

Si necesitas actualización automática:

1. **Configurar Azure Services**:
   - Crear cuenta de Azure
   - Configurar Application Insights
   - Configurar Service Bus
   - Configurar Blob Storage

2. **Actualizar configuración**:
   - `local.settings.json` en Functions
   - `Web.config` en VTLive40
   - Connection strings y keys de Azure

3. **Instalar Azure Functions Core Tools**:
```powershell
winget install Microsoft.Azure.FunctionsCoreTools
# o
npm install -g azure-functions-core-tools@4 --unsafe-perm true
```

## Consultas SQL Útiles

### Verificar versión actual
```sql
SELECT ID, Data FROM sysroParameters WHERE ID = 'DBVERSION'
```

### Ver todas las tablas del sistema
```sql
SELECT name FROM sys.tables WHERE name LIKE 'sysro%' ORDER BY name
```

### Ver parámetros del sistema
```sql
SELECT ID, 
    CASE 
        WHEN LEN(Data) > 100 THEN LEFT(Data, 100) + '...'
        ELSE Data
    END as Data
FROM sysroParameters
```

## Logs y Diagnóstico

### Ubicación de Logs

- **Application Insights**: No configurado
- **Logs locales**: Verificar configuración en `Web.config`
- **Event Viewer**: Windows ? Application logs

### Verificar si la aplicación está ejecutándose

```powershell
# Verificar IIS
Get-Process -Name w3wp

# Verificar puerto de la aplicación
netstat -ano | findstr :80
```

## Archivos Creados

1. `Utils\DatabaseUpdater\DatabaseUpdater.csproj` - Proyecto de consola
2. `Utils\DatabaseUpdater\Program.cs` - Programa principal
3. `Utils\DatabaseUpdater\App.config` - Configuración
4. `Utils\DatabaseUpdater\VerifyDatabaseVersion.sql` - Script de verificación
5. `Functions\roScheduleFunctions\local.settings.json` - Configuración de Azure Functions

## Próximos Pasos Sugeridos

1. ? Ejecutar `VerifyDatabaseVersion.sql` en SSMS
2. ? Verificar que la aplicación web funciona (IIS)
3. ?? Decidir si necesitas actualizaciones automáticas
4. ?? Si es necesario, configurar servicios de Azure
5. ?? O continuar con actualizaciones manuales vía SSMS

## Soporte

Para más información sobre scripts de actualización:
- Ver carpeta: `Common\DBScript\SQLUpgradeVersion\`
- Documentación: `Common\DBScript\SQLUpgradeVersion\Doc\CodingStandard.txt`
