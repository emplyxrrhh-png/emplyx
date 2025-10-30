# Configuración de Application Insights - VisualTime

## Estado de la Configuración

? Application Insights ha sido configurado correctamente en todos los archivos necesarios.

### Connection String Configurado

```
InstrumentationKey=6825e043-9845-40f5-b5d0-74f42d3aeb32
IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/
LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/
ApplicationId=e5454841-74a2-4a66-b30d-8cc50893b21f
```

## Archivos Configurados

### 1. VTLive40\Web.config ?
**Ubicación**: `<appSettings>` section

```xml
<add key="ApplicationInsights.ConnectionString" value="InstrumentationKey=6825e043-9845-40f5-b5d0-74f42d3aeb32;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=e5454841-74a2-4a66-b30d-8cc50893b21f" />
<add key="ApplicationInsights.InstrumentationKey" value="6825e043-9845-40f5-b5d0-74f42d3aeb32" />
<add key="ApplicationInsights.Key" value="6825e043-9845-40f5-b5d0-74f42d3aeb32" />
```

### 2. VTLive40\ApplicationInsights.config ?
**Archivo creado con configuración completa**

Incluye:
- ConnectionString
- InstrumentationKey
- TelemetryInitializers
- TelemetryModules (DependencyTracking, PerformanceCounters, etc.)
- TelemetryProcessors (Sampling, Metrics)

### 3. Functions\roScheduleFunctions\local.settings.json ?
**Ubicación**: `Values` section

```json
"APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey=6825e043-9845-40f5-b5d0-74f42d3aeb32;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=e5454841-74a2-4a66-b30d-8cc50893b21f",
"ApplicationInsights.Key": "6825e043-9845-40f5-b5d0-74f42d3aeb32"
```

### 4. Utils\DatabaseUpdater\App.config ?
**Ubicación**: `<appSettings>` section

```xml
<add key="ApplicationInsights.ConnectionString" value="InstrumentationKey=6825e043-9845-40f5-b5d0-74f42d3aeb32;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=e5454841-74a2-4a66-b30d-8cc50893b21f" />
<add key="ApplicationInsights.InstrumentationKey" value="6825e043-9845-40f5-b5d0-74f42d3aeb32" />
<add key="ApplicationInsights.Key" value="6825e043-9845-40f5-b5d0-74f42d3aeb32" />
```

## Componentes de Application Insights

### Telemetry Initializers
Agregan información de contexto a cada telemetría:
- Dependencias HTTP
- Información del entorno Azure
- Versión de compilación
- Pruebas web
- Agentes sintéticos
- IP del cliente
- Nombres de operación
- Correlación de operaciones
- Usuario
- ID de usuario autenticado
- ID de cuenta
- Sesión

### Telemetry Modules
Recopilan telemetría automáticamente:
- **DependencyTrackingTelemetryModule**: Rastrea llamadas a dependencias externas
- **PerformanceCollectorModule**: Recopila contadores de rendimiento
  - % Tiempo de procesador
  - Memoria disponible
  - Bytes privados del proceso
  - Tasa de E/S del proceso
  - Excepciones CLR
  - Tasa de solicitudes ASP.NET
  - Tasa de errores
  - Solicitudes en cola
  - Solicitudes rechazadas
- **DiagnosticsTelemetryModule**: Diagnósticos internos
- **DeveloperModeWithDebuggerAttachedTelemetryModule**: Modo desarrollador

### Telemetry Processors
Procesan y filtran telemetría:
- **QuickPulseTelemetryProcessor**: Métricas en tiempo real
- **AutocollectedMetricsExtractor**: Extrae métricas automáticas
- **AdaptiveSamplingTelemetryProcessor**: Muestreo adaptativo (máx 5 items/segundo)

## Verificación de Configuración

### Opción 1: Verificar en el código
La aplicación debería inicializar Application Insights automáticamente al leer el `Web.config` o `ApplicationInsights.config`.

### Opción 2: Verificar en Azure Portal
1. Ir a [https://portal.azure.com](https://portal.azure.com)
2. Buscar el Application Insights con ID: `e5454841-74a2-4a66-b30d-8cc50893b21f`
3. Verificar que llegan datos de telemetría

### Opción 3: Verificar logs locales
Los logs también pueden escribirse localmente si Application Insights no está disponible.

## Próximos Pasos

### Para ejecutar VTLive40:
1. Compilar la solución (ya compilada ?)
2. Configurar IIS o ejecutar desde Visual Studio
3. La aplicación debe conectarse automáticamente a Application Insights

### Para ejecutar Azure Functions localmente:
```powershell
cd Functions\roScheduleFunctions
func start
```

### Para ejecutar DatabaseUpdater:
```powershell
cd Utils\DatabaseUpdater\bin\Debug\net48
.\DatabaseUpdater.exe
```

## Características Configuradas

### Sampling (Muestreo)
- Máximo 5 telemetry items por segundo
- Eventos excluidos del muestreo general
- Muestreo separado para eventos

### Diagnósticos
- Rastreo de dependencias habilitado
- Contadores de rendimiento habilitados
- Modo desarrollador cuando debugger está conectado

### Seguridad
- Dominios excluidos de correlación:
  - core.windows.net
  - core.chinacloudapi.cn
  - core.cloudapi.de
  - core.usgovcloudapi.net
  - localhost
  - 127.0.0.1

## Troubleshooting

### Si la aplicación se cuelga al arrancar:
1. ? Verificar que `ApplicationInsights.ConnectionString` está configurado
2. ? Verificar que `ApplicationInsights.config` existe
3. ?? Verificar conectividad con Azure (firewall, proxy)
4. ?? Revisar logs de Application Insights

### Si no llegan datos a Azure:
1. Verificar que el InstrumentationKey es correcto
2. Verificar conectividad a Internet
3. Revisar firewall/proxy corporativo
4. Verificar que el recurso existe en Azure

### Logs de diagnóstico
Application Insights escribirá logs de diagnóstico en:
- Ventana de Output de Visual Studio
- Windows Event Viewer
- Archivos de log si está configurado

## Endpoints de Azure

- **Ingestion**: `https://westeurope-5.in.applicationinsights.azure.com/`
- **Live Metrics**: `https://westeurope.livediagnostics.monitor.azure.com/`
- **Region**: West Europe

## Notas Importantes

1. ? Todos los archivos de configuración han sido actualizados
2. ? Application Insights está configurado para desarrollo local
3. ?? Asegúrate de tener acceso al recurso de Azure
4. ?? Los datos se enviarán a Azure en tiempo real
5. ?? Considera los costos de telemetría en producción

## Referencias

- [Application Insights Documentation](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Connection Strings](https://docs.microsoft.com/en-us/azure/azure-monitor/app/sdk-connection-string)
- [Sampling in Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling)
