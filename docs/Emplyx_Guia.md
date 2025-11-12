# Emplyx — Guía general de análisis y diseño (v0.2)

> Documento base para análisis funcional y diseño técnico.  
> Fecha: 30/10/2025

## Tabla de contenidos
- [1) Visión, alcance y principios](#1-visión-alcance-y-principios)
- [2) Modelo organizativo y seguridad (autorizaciones)](#2-modelo-organizativo-y-seguridad-autorizaciones)
- [3) Arquitectura de referencia](#3-arquitectura-de-referencia)
- [4) Agente de IA (en todas las vistas)](#4-agente-de-ia-en-todas-las-vistas)
- [5) Catálogo completo de funcionalidades (85 ítems)](#5-catálogo-completo-de-funcionalidades-85-ítems)
- [6) Plantilla de análisis por funcionalidad](#6-plantilla-de-análisis-por-funcionalidad)
- [7) Tecnología y estándares](#7-tecnología-y-estándares)
- [8) Integraciones y conectores](#8-integraciones-y-conectores)
- [9) Licenciamiento y ediciones (visión)](#9-licenciamiento-y-ediciones-visión)
- [10) Requisitos no funcionales (NFR)](#10-requisitos-no-funcionales-nfr)
- [11) Roadmap y entornos](#11-roadmap-y-entornos)
- [12) Plantillas auxiliares](#12-plantillas-auxiliares)
- [13) Checklist de paridad con VT](#13-checklist-de-paridad-con-vt)
- [14) Preguntas abiertas iniciales](#14-preguntas-abiertas-iniciales)
- [15) Decisiones confirmadas (30/10/2025)](#15-decisiones-confirmadas-30102025)
- [16) UI multi-plataforma: contexto activo y delegación](#16-ui-multi-plataforma-contexto-activo-y-delegación)

---

## 1) Visión, alcance y principios
**Visión:** Emplyx es una plataforma multi-tenant de Gestión de Tiempos y Portal del Empleado que iguala y supera Visualtime (VT), con IA asistiva en todas las vistas.

**Ámbitos**
- **Gestión de tiempos (T&A):** fichajes, actividades, reglas, horarios, planificación, solicitudes, resultados/bolsas, festivos, convenios, terminales y zonas.
- **Portal del empleado:** autoservicio, documentos, comunicados, encuestas, onboarding, notificaciones, informes.
- **Transversales:** seguridad/auditoría, licencias, analítica, import/export, configuración, API/integraciones.

**Principios de diseño**
- Multi-tenant secure-by-default, **RBAC** con herencia/denegación y **ABAC**.
- **AI-in-every-view** con acciones seguras y explicabilidad.
- Arquitectura **API-First**, eventos asíncronos y **observabilidad** end-to-end.
- Cumplimiento (GDPR/LOPDGDD), accesibilidad e i18n.
- Localizacion usando resx

---

## 2) Modelo organizativo y seguridad (autorizaciones)
**Jerarquía:** Administrador WEB → Tenant → Corporación → Empresa → Unidad/Grupo → Usuario (opcional partners).

**Permisos:**
- **RBAC + ABAC**. Scopes: `read | create | update | delete | execute | export | approve | configure`.
- Deny > Allow; overrides; permisos por funcionalidad y por **campo** (datos sensibles). Grupos/supervisores con categorías de aprobación.

**Auditoría y cumplimiento:** Login, configuración, CRUD, aprobaciones y accesos a datos sensibles. Retención por tipo y export firmada.

---

## 3) Arquitectura de referencia
**Lógica:** microservicios por dominio; API Gateway (REST/GraphQL); eventos (Service Bus/Event Grid); Workers/Jobs para motor de tiempos; identidad OIDC/OAuth2.

**Datos:** **BD única multi-tenant (shared) con RLS**; `tenant_id` en claves; particionamiento por *tenant + fecha*; NoSQL para auditoría/metadatos; blobs con SAS.

**Frontend y apps:** **Blazor** + **.NET MAUI** (Desktop + iOS/Android) compartiendo **DTO, Servicios e Interfaces** en proyectos comunes; tiempo real (SignalR); soporte offline parcial.

**Observabilidad:** métricas SLI/SLO, trazas, logs y alertas proactivas.

---

## 4) Agente de IA (en todas las vistas)
**Entrada:** botón “Pregúntale a Emplyx”, paleta de comandos y voz.

**Capacidades**
- Copilot de acción (crear/editar solicitudes, explicar cálculos, informes, navegación y configuración bajo permisos).
- RAG seguro por tenant/empresa (mínimo privilegio) citando origen.
- Explicabilidad de cálculo (motivos y trazas del motor).
- Guardrails: PII/DLP, límites por rol, confirmaciones para acciones críticas.
- **Planificador IA:** planificación asistida (horarios/turnos/cobertura) usando **IA del cliente** (endpoint/modelo propio) o **IA gestionada por Emplyx** (según licencia).

---

## 5) Catálogo completo de funcionalidades (85 ítems)
### A) Plataforma, multi-tenant y seguridad
1. Multi-tenant jerárquico — Admin WEB → Tenant → Corporación → Empresa → Grupos; parámetros por nivel.  
2. RBAC con herencia y denegación explícita — Permisos por funcionalidad/campo y excepciones por dato sensible.  
3. ABAC (atributos) — Reglas por país, centro de coste, zona, puesto, turno, etc.  
4. Gestión de roles y perfiles — Catálogo de roles (empleado, supervisor, RRHH, auditor, partner).  
5. Seguridad avanzada — SSO OIDC/OAuth2, MFA, control de sesión/dispositivos, políticas y expiraciones.  
6. Trazabilidad/Auditoría — Accesos, cambios, aprobaciones y exportaciones con sello de tiempo.  

### B) Organización y datos maestros
7. Estructura organizativa — Organigrama, grupos, jerarquías y supervisores.  
8. Empleados — Ficha, documentos, contratos, estados y vigencias.  
9. Centros de trabajo y unidades — Parámetros legales, calendarios, ubicaciones.  
10. Convenios — Tiempos, jornadas, reglas asociadas y versiones.  
11. Festivos y calendarios — Por región/empresa; importación y plantillas.  

### C) Control horario (core T&A)
12. Fichajes IN/OUT — Web/móvil/terminal/NFC; validaciones y husos correctos.  
13. Geolocalización y precisión — Lat/long y exactitud para móviles (opcional por política).  
14. Tipos de fichaje/actividad — Trabajo, pausas, desplazamientos, proyectos, centros de coste.  
15. Retención legal — Conservación de fichajes (≥ 4 años) y políticas por tipo de dato.  
16. Validaciones de entrada — Duplicados, solapes, límites, ventanas.  

### D) Planificación y horarios
17. Definición de horarios — Fijos, rotativos, flexibles; descansos; extras.  
18. Planificación operativa — Asignación por persona/puesto; publicar/bloquear.  
19. Reglas de cumplimiento — Descansos mínimos, máximos de horas/días, fines de semana alternos.  
20. Simulación y borradores — What‑if y resolución de conflictos.  

### E) Solicitudes y ausencias
21. Vacaciones y ausencias — Solicitud, aprobación, cómputo y saldos.  
22. Permisos horarios — Puntuales o recurrentes; con reglas.  
23. Correcciones — Fichaje olvidado, cambios de actividad/horario.  
24. Flujos de aprobación configurables — Categorías, cortes/saltos, visibilidad y tracking.  
25. Vista unificada de solicitudes — Para empleados y supervisores (impersonación controlada).  

### F) Portal del empleado
26. Inicio con indicadores — Pendientes, último fichaje, alertas y accesos rápidos.  
27. Calendario personal/equipo — Turnos, ausencias, hitos; filtros.  
28. Autoservicio de datos — Datos personales, documentos, consentimientos.  
29. Preferencias y accesibilidad — Idioma, notificaciones, formatos, accesibilidad.  

### G) Comunicación y engagement
30. Comunicados (Tablón) — Lectura obligatoria, adjuntos y métricas de lectura.  
31. Encuestas — Diseño/envío; resultados y segmentación.  
32. Onboarding — Pasos por empleado, checklist y seguimiento.  

### H) Documentos y firma
33. Gestión documental — Tipos, categorías, obligatoriedad y plantillas.  
34. Envío de nóminas/adjuntos — Con trazabilidad de descarga/lectura.  
35. Firma digital — Contratos, políticas y anexos (integración proveedores).  

### I) Notificaciones y alertas
36. Eventos → notificaciones — Email/portal, persistentes, estado de lectura.  
37. Alertas operativas — Aforo, fallos de terminal, anomalías de cálculo.  
38. Preferencias por usuario/rol — Canal, horario y frecuencia.  

### J) Informes y analítica
39. Catálogo de informes — Operativos/legales con filtros por zona/periodo.  
40. Editor de informes — Diseños guardados y compartibles.  
41. Exportaciones — CSV/Excel/PDF con permisos por categoría.  
42. KPIs y dashboards — Productividad, absentismo, cumplimiento, SLAs.  

### K) Emergencia y seguridad laboral
43. Informe de emergencia — Están / pueden estar / no están; vistas rápidas y acceso protegido.  

### L) Dispositivos y captura
44. **Terminales de presencia** — Alta, estado, IP/firmware, lectores, validaciones, colas de sincronización.  
45. Biometría y tarjetas — Huella, facial, palma, proximidad.  
46. NFC tags y puntos de fichaje — Configuración y fichaje por tag en portal móvil/kiosko.  
47. Kiosko/Punto de fichaje — Web/app para centros sin terminal físico.  

### M) Zonas y geofencing
48. Definición de zonas (mapa/coords) — Prioridad, centro de trabajo, aforo, huso horario.  
49. Asociación a terminales — Vista de terminales por zona/ubicación.  
50. Aforo y alertas — Disparos al superar capacidad/umbral.  

### N) Integraciones y APIs
51. API‑First (REST/GraphQL) — Autenticación de servicio, scopes y límites.  
52. Webhooks/eventos — Altas/bajas, fichajes, aprobaciones, publicaciones.  
53. Conectores — Nómina/ERP/SSO/BI; plantillas y versionado.  
**Terminales soportados:** integración con **Suprema** y **ZKTeco** (y otras marcas) mediante conectores que **corren contra la API de Emplyx**.

### O) Importación/Exportación
54. Importadores masivos — Usuarios, calendarios, convenios, fichajes, saldos.  
55. Paquetes de export — Legales/BI con versionado y firma.  

### P) Auditoría y cumplimiento
56. Logs inmutables — Acceso, cambios y descargas con sello de tiempo.  
57. Retención por tipo de dato — Políticas configurables y anonimización.  
58. Consultas de auditoría — Por ámbito, usuario, entidad y periodo.  

### Q) Licenciamiento y operativa
59. Tipos de licencia — Empleado/Supervisor/Admin y límites.  
60. Módulos/ediciones — Activación por tenant/empresa; *feature flags*.  
61. Altas de clientes/partners — Flujo de creación y delegación.  

### R) Administración del sistema
62. Parámetros globales — Branding, idioma, formatos, TZ, moneda.  
63. Catálogos — Actividades, tipos de documento, motivos estandarizados.  
64. Mantenimiento preventivo — Estados de sincronización, colas y DLQs.  

### S) Experiencia, accesibilidad e i18n
65. UX portal — Cabecera, menú general/contextual, responsive.  
66. Accesibilidad — WCAG 2.2 AA, teclado, alto contraste.  
67. i18n y husos — Moneda/idioma por usuario; TZ por zona.  

### T) IA (Asistente + Planificador)
68. Asistente en todas las vistas — Respuestas con contexto y acciones (según permisos).  
69. Explicabilidad de cálculo — Motivos y trazas del motor de tiempos.  
70. Guardrails de IA — PII/DLP, confirmaciones y límites por rol.  

### U) Movilidad y apps
71. App/PWA — Fichar, solicitar, ver calendario y comunicados.  
72. Soporte offline parcial — Cola de fichajes, reintentos y reconciliación.  
73. Biometría móvil — Face/TouchID para acceso/acciones sensibles.  

### V) Operaciones y soporte
74. Observabilidad — Métricas, trazas e indicadores por cliente/tenant.  
75. Centro de ayuda y feedback — Guías, FAQ y reporte de mejoras.  
76. Ciclo ALM/entornos — Dev/Stage/Prod con despliegues controlados.  

### W) Módulos Plus (competencia RRHH)
77. ATS — Publicación de ofertas y pipeline de candidatos.  
78. Evaluación del desempeño — Objetivos, revisiones, 360.  
79. Objetivos/OKR — Alineación por equipo y ciclo.  
80. Formación/LMS básico — Cursos, asistencia, cumplimiento.  
81. Gastos y viajes — Captura/validación; integración con nómina.  
82. Chat/Comunidad — Muro interno y reacciones (opcional).  
83. Pre‑nómina / gateway nóminas — Reglas y export a proveedores.  
84. Proyectos y coste/hora — Partes de trabajo por proyecto/tarea.  
85. Helpdesk interno — Tickets vinculados a usuarios/turnos.  

---

## 6) Plantilla de análisis por funcionalidad
1. Objetivo de negocio · 2. Alcance · 3. Actores/Permisos · 4. Escenarios · 5. Datos · 6. Integraciones · 7. Cálculos/Reglas · 8. UI/UX · 9. IA · 10. Notificaciones · 11. Auditoría · 12. Rendimiento · 13. Observabilidad · 14. Seguridad · 15. Pruebas · 16. Rollout · 17. Riesgos · 18. DoD.

---

## 7) Tecnología y estándares
- **Backend:** .NET 9 (ASP.NET minimal APIs) y Workers; patrón Outbox; OpenAPI/JSON:API; FluentValidation; caching distribuido.
- **Frontend y Apps:** **Blazor** (Server/Hybrid) + **.NET MAUI** (Desktop + iOS/Android) con **código compartido**. **DTO, Servicios e Interfaces** en proyectos comunes reutilizados por Blazor y MAUI.
- **Datos:** Azure SQL (RLS, Always Encrypted), Cosmos DB (auditoría/metadatos), Azure Storage (blobs), búsqueda (Azure AI Search o Postgres trigram).
- **Mensajería:** Azure Service Bus/Event Grid; reintentos y DLQs.
- **Identidad:** OIDC/OAuth2 (Entra ID/ADFS/otros). **SSO empresarial** disponible en **planes superiores**; MFA; SCIM opcional.
- **Observabilidad y CI/CD:** OpenTelemetry; dashboards por tenant. CI/CD con GitHub Actions/Azure DevOps; IaC (Bicep/Terraform); SAST/DAST; blue‑green.
- **Terminales de presencia:** Integración con **Suprema** y **ZKTeco** mediante conectores contra la **API de Emplyx** (colas, idempotencia, control de firmware).

---

## 8) Integraciones y conectores
- Tipos: batch (SFTP/API), near‑real‑time (webhooks/eventos), pull (APIs) y push (bus/callbacks).
- **Migración desde Visualtime (VT):** importación por **API de VT** configurando **API base + Token** del cliente. Mapeo de entidades (usuarios, organización, calendarios, fichajes, solicitudes, documentos), idempotencia por `external_id`, pruebas de contrato y reconciliación post‑carga.
- Contratos: versionado semántico, schema registry, pruebas de contrato. Robustez: idempotencia, replay, poison queues, circuit breakers. Seguridad: OAuth client credentials, IP allow‑list, firma de mensajes.

---

## 9) Licenciamiento y ediciones (visión)
- Modelo por usuario (Empleado/Supervisor) + ediciones; feature flags por módulo.
- **IA:** edición/extra que habilita **Planificador IA** y **Asistente**. Opción de **modelo propio del cliente** o **IA gestionada por Emplyx**.
- **SSO empresarial (plan superior):** AD / ADFS / Entra ID (SAML/OIDC/OAuth2), MFA y aprovisionamiento SCIM opcional.

---

## 10) Requisitos no funcionales (NFR)
- Seguridad: cifrado en tránsito/reposo, secretos gestionados, CSP, anti‑fraude de sesiones.  
- Rendimiento: p95 < 300 ms (lecturas comunes); cálculo diario N usuarios < X min.  
- Disponibilidad: 99.9% (MVP) → 99.95% (GA). Escalado horizontal; back‑pressure.  
- Compatibilidad: navegadores evergreen; iOS/Android recientes. Accesibilidad: WCAG 2.2 AA.

---

## 11) Roadmap y entornos
- Entornos: Dev/Integration, Early Adopters (Stage), Prod.  
- Hitos: MVP (T&A core + Portal básico + IA limitada) → v1 (Planificación, bolsas, encuestas, comunicaciones) → v2 (NFC, emergencia avanzado, what‑if IA).

---

## 12) Plantillas auxiliares
- ADRs · Especificación de API · Catálogo de eventos · Catálogo de datos sensibles · Mapa de KPIs.

---

## 13) Checklist de paridad con VT
- Cobertura completa de módulos, flujos de aprobación, motor de cálculo, retención legal, portal autoservicio, notificaciones, seguridad e IA.

---

## 14) Preguntas abiertas iniciales
1) Motor de reglas declarativo (DSL) vs scripts controlados.  
2) Identidad: IdP propio vs federación adicional (más allá del plan superior).  
3) Plan exacto de MVP y definición de éxito por módulo.

---

## 15) Decisiones confirmadas (30/10/2025)
- **Migración desde VT:** por API de VT (API base + Token); importación integral.  
- **Modelo de datos:** **BD única multi‑tenant (shared)** con **RLS** y particionamiento por *tenant + fecha*.  
- **SSO empresarial:** conexión con **AD/ADFS/Entra ID** incluida en **planes superiores** (SAML/OIDC/OAuth2, MFA, SCIM opcional).

---

## 16) UI multi-plataforma: contexto activo y delegación
**Objetivo:** disponer de componentes coherentes en Blazor y .NET MAUI que permitan alternar contextos (tenant/corporación/empresa/proyecto) y gestionar delegaciones con MFA, actualizando tokens, vistas y auditoría en tiempo real sin interrumpir la sesión.

### 16.1 Selector de contexto activo
**Requisitos comunes**
- Fuente de datos: servicio `IContextDirectoryService` (cacheado por usuario) que expone los contextos habilitados, metadatos visuales (logo, color, slogan) y flags de permisos.  
- Contexto seleccionado se almacena en `ContextState` (Scoped en Blazor, Singleton Observable en MAUI) con eventos (`ContextChanging`, `ContextChanged`) para refrescar vistas.  
- Al cambiar, se invoca `ContextSessionService.SwitchAsync(contextId)` → API `POST /api/context/switch` que devuelve token JWT renovado/claims vía SignalR (fallback polling).  
- Cambio exitoso dispara: refresco de menús, filtros, dashboards, caches por contexto y log `AuditContextChanged`.

#### Blazor (Emplyx.WebApp)
- Componente `ContextSelector.razor` ubicado en la barra superior; muestra etiqueta/empresa actual, logo miniatura y botón desplegable accesible (`aria-haspopup="listbox"`).  
- Dropdown con búsqueda incremental, favoritos y agrupaciones (Corporación → Empresa → Proyecto). Opción “Administrar contextos” linkea a pantalla de configuración.  
- Uso de `HeadlessUI`/componentes propios para soportar teclado (`Alt+K`), lector de pantalla y tema oscuro.  
- Estado de carga: shimmer + mensaje si no hay contextos habilitados. Errores muestran banner no modal con opción retry.

#### .NET MAUI (Emplyx.App)
- `ContextPickerView` reutiliza ViewModel compartido (`ContextSelectorViewModel`). Disponible como picker en el `Shell` superior y en Ajustes.  
- En Windows/macOS: menú contextual en barra superior + `Ctrl+Alt+K`. En iOS/Android: sheet modal deslizable con buscador y badges.  
- Permite modo offline restringido: si no hay red, se limita a los contextos cacheados y se marca “(sincronizar al reconectar)”.  
- Animaciones livianas (Entry/Exit) y compatibilidad con lector de pantalla (`SemanticProperties.Description`).

#### Flujo completo
1. Usuario abre selector → se carga lista (cache + refresh background).  
2. Selecciona nuevo contexto → UI lanza diálogo de confirmación si el cambio implica pérdida de formularios no guardados.  
3. `ContextSessionService` envía solicitud, recibe token actualizado y notifica a `AuthStateProvider` (Blazor) o `SecureStorage` (MAUI).  
4. SignalR `ContextHub` emite evento `ContextChanged` para tabs abiertas; componentes escuchan y rehidratan datos.  
5. Se registra auditoría con `tenant_from`, `tenant_to`, `device_id`, `mfa_result` (si aplica) y timestamp.  
6. Registro se envía a Azure Functions (`fn-audit-context`) para persistir y emitir métricas (`ContextSwitchLatency`).

### 16.2 Activación de delegación (MFA + auditoría)
**Escenarios:** configuración por delegante, activación por delegado y finalización manual/automática, siempre con MFA contextual (Azure AD B2C, OTP vía Azure Function o proveedor externo).

#### Configuración por delegante
- Accesible desde perfil/ajustes (`/account/delegations`). Tabla con delegaciones activas, programadas y revocadas.  
- Flujo UI: seleccionar destinatario (picker filtrado por organización), definir rango fechas/horas, seleccionar permisos (roles completos por defecto, granular via toggles) y opcional nota.  
- Al confirmar, se abre modal MFA (código 2FA, push o WebAuthn). UI muestra contador y reenvío tras 30s.  
- Exitoso → banner “Delegación activada”, notificación al destinatario (Toast + email/Push). También se ofrece acción rápida para revocar.  
- Revocación anticipada requiere MFA corta (solo confirmación) y deja registro `AuditDelegationRevoked`.

#### Activación por delegado
- Aviso persistente (“Tienes una delegación disponible de Juan Pérez”) en el header y en bandeja de notificaciones.  
- Botón “Actuar como …” abre modal/resumen con alcance, caducidad y auditoría esperada. Se puede rechazar.  
- MFA opcional configurable por política (recomendado cuando la delegación otorga permisos > propios).  
- Tras confirmación se solicita token delegado (`POST /api/delegations/activate`), se añade claim `delegator_id` y se cambia el branding a color distintivo + banda superior “Actuando como …”.  
- Acción “Finalizar delegación” disponible en el banner; al salir se restaura el token original y se refresca el contexto.

#### Estados y mensajes comunes
- **Indicadores visibles:** icono de identidades superpuestas en el header, tooltip con delegado, contador regresivo de expiración.  
- **Conflictos:** si existen formularios sin guardar al cambiar de delegación/contexto, se ofrece guardar borrador o cancelar.  
- **Expiraciones automáticas:** notificación 5 min antes y al expirar, con revalidación forzada de token.  
- **Errores MFA:** mostrar número de intentos restantes y ruta a soporte sin revelar información sensible.

### 16.3 Integraciones técnicas
- Servicios compartidos (`Emplyx.Shared`) expondrán `IContextSessionService`, `IDelegationService`, `IMfaChallengeService` y `IAuditClient`.  
- SignalR Hubs: `ContextHub` (broadcast de cambios de contexto) y `DelegationHub` (activaciones/revocaciones). Ambos autenticados y con reintentos exponenciales.  
- Compatibilidad con JS interop en Blazor para integración con portales externos (ej. abrir selector desde shell corporativo).  
- MAUI usa `MessagingCenter`/`WeakReferenceMessenger` para notificar a páginas abiertas y `SecureStorage` para tokens (con `Platform-protected storage` en iOS/Android).  
- Telemetría: eventos `ContextSwitchRequested`, `ContextSwitchCompleted`, `DelegationActivated`, `DelegationRejected`, `DelegationEnded` con atributos (tenant, roles, MFA type, latency).

### 16.4 Auditoría y cumplimiento
- Todas las operaciones generan entrada estructurada (`category=context|delegation`, `actor`, `acted_as`, `mfa`, `device`, `ip`, `correlation_id`).  
- Azure Function `fn-audit-context` y `fn-audit-delegation` persisten en Cosmos DB/Log Analytics y publican eventos para BI.  
- Registro de delegación incluye vínculo al MFA (ID de desafío) para trazabilidad y evidencia ante auditorías.  
- Políticas de retención alineadas con sección 2 (cumplimiento) y exportables por tenant.  
- Dashboards de administración muestran historial + filtros por usuario, contexto, resultado MFA y dispositivo.

---

## 17) Gestión de usuarios, roles y permisos en caliente
**Objetivo:** proveer una consola de administración segura (solo rol `Administrador` o equivalentes con `SEGURIDAD.USUARIOS.GESTIONAR`) que permita crear/editar usuarios, asignar roles/permisos y forzar la actualización inmediata de sesiones en Blazor y .NET MAUI sin pedir un re-login manual.

### 17.1 Alcance, actores y servicios base
- **Servicios/contratos:** `IUsuarioService`, `IRolService`, `IPermisoService`, `ILicenciaService` y `IAuditoriaService` exponen CRUD + búsqueda (`SearchUsuariosRequest`, `UpsertRolRequest`, etc.).  
- **Datasets cacheables:** catálogos de roles, permisos y contextos se sirven vía `SecurityDirectoryCache` (per-user, 5 min) para minimizar round-trips en filtros.  
- **Autorización UI:** sección visible solo si el usuario tiene claim `security:admin=true`; de lo contrario, la ruta devuelve 404. Operaciones sensibles (reset password, borrar sesión) solicitan MFA corto.  
- **Ámbitos:** cambios aplican al tenant/corporación actual respetando herencias; un admin puede ver usuarios de empresas hijas siempre que tenga `SEGURIDAD.USUARIOS.VER`.  
- **Auditoría obligatoria:** cada acción UI invoca `IAuditoriaService.LogAsync` con `category=security.user|security.role`, detalle, `before/after` y `correlation_id` (GUID por interacción).

### 17.2 Administración de usuarios (Blazor Web)
- **Disposición:** página `/admin/usuarios` contiene cabecera con KPIs (totales activos/inactivos, pendientes de invitación), filtro avanzado colapsable y layout Master/Detail (lista izquierda 35%, detalle derecha).  
- **Tabla/lista:** componente `UsuarioDataGrid` (hereda de `QuickDataGrid`) soporta búsqueda incremental (nombre, email, rol, contexto, estado), columnas configurables, badges de estado e iconos para MFA habilitado / delegando / bloqueado. Paginación server-side (lazy) y export a CSV (según permiso).  
- **Acciones en lista:** selección múltiple para activar/desactivar, asignar roles masivos o reenviar invitación; cada acción muestra confirmación y resumen del impacto.  
- **Detalle/Edición:** `UsuarioDetailPanel` se abre al seleccionar o al pulsar "Nuevo usuario". Tabs: Perfil, Seguridad, Roles, Contextos/Licencias, Sesiones activas. Campos soportan validaciones instantáneas con `FluentValidation`.  
- **Perfil básico:** nombres, apellidos, cargo, contacto, `PreferredContexto`, `Clearance`. Campos sensibles muestran tooltips de ayuda.  
- **Seguridad:** toggles "Usuario activo", "Forzar cambio de contraseña", "Requerir MFA". Botones `Reset password`, `Invitar usuario`, `Cerrar todas las sesiones`.  
- **Roles:** panel lateral tipo dual-list (`Roles disponibles` vs `Roles asignados`) con búsqueda, descripciones descriptivas ("Supervisor RRHH") y tooltips con permisos clave. Cambios se aplican en memoria y muestran diff (`+3 / -1`).  
- **Contextos/Licencias:** pickers multiselección con badges. Contexto principal obliga a elegir solo uno; se valida que exista al menos un contexto asignado.  
- **Sesiones activas:** lista de dispositivos/IP, última actividad y botón para revocar sesión individual (SignalR notificará al cliente).  
- **Persistencia:** botón Guardar invoca `PATCH /api/usuarios/{id}` (o `POST` si nuevo). Tras éxito, se muestra toast, se resalta el usuario actualizado y se dispara evento `UsuarioUpdated` al hub en tiempo real.

### 17.3 Gestión de roles y permisos
- **Ruta dedicada:** `/admin/roles` comparte layout. Lista con tarjetas/resumen (usuarios asignados, permisos críticos) y tabla detallada con sorting.  
- **Creación/Edición:** `RolDetailDrawer` permite cambiar nombre, descripción, clearance y permisos (agrupados por módulo/categoría). Inspirado en mejores prácticas de `permit.io`: vista jerárquica plegable, buscador por código y toggle "Marcar permiso como crítico" que exige MFA.  
- **Permisos por rol:** al marcar/desmarcar items, se muestra impacto ("14 usuarios perderán acceso a Reportes"). Confirmación requiere escribir el nombre del rol si implica quitar permisos críticos.  
- **Versionado:** al guardar, se incrementa `RoleVersion` en `Rol` y se registra `RolPermisosChangedEvent` para recalcular permisos efectivos de usuarios asociados.  
- **Dependencias:** si un rol requiere otro (roles compuestos), la UI muestra advertencia y ofrece asignarlo en cascada.  
- **Acciones adicionales:** duplicar rol, exportar definición (JSON) y adjuntar notas internas (almacenadas en `RolMetadata`). Eliminación bloqueada para roles `IsSystem=true`.

### 17.4 Hot reload de autorización y control de sesiones
- **Modelo de versiones:** tabla `Usuarios` añade campos `SecurityVersion` y `LastSecurityUpdateUtc`; `Roles` mantiene `RoleVersion`. Cada JWT incluye claim `token_version` (hash de `SecurityVersion` + `RoleVersion` agregada).  
- **Proceso cuando un admin edita usuarios/roles:**  
  1. API aplica cambios (transacción) y actualiza `SecurityVersion=SecurityVersion+1`.  
  2. Se persiste en `UsuarioEventos` (`UsuarioPermisosActualizadosDomainEvent`).  
  3. Worker `SecurityInvalidationProcessor` publica mensaje en `security-session-invalidation` (Service Bus) y avisa al `SecurityNotificationsHub` (SignalR) usando el `userId` como grupo.  
  4. Clientes Blazor/MAUI, suscritos al hub `NotificacionesSeguridad`, reciben `PermissionsUpdated` con payload (`reason`, `required_claims`, `issued_at`).  
  5. Cliente ejecuta `TokenRefreshService.RefreshAsync` (llama a `/api/auth/refresh-token`), reemplaza el JWT y vuelve a consultar permisos/menús; se muestra banner "Tus permisos han cambiado, refrescando..." o botón manual si falla.  
- **Proceso cuando el propio usuario cambia contexto/delegación/licencia activa:** la UI sabe que se requiere nuevo token y llama directamente a `RefreshAsync` sin esperar notificación externa; aun así registra `SecurityVersion` para mantener consistencia.  
- **Expiración corta:** tokens de acceso duran 5 minutos; el refresh token incluye `security_version_at_issue`. Si el servidor detecta versión diferente, responde 409 + payload `needs_refresh=true`.  
- **Fallback:** si SignalR no conecta, un `SecurityVersionPoller` (timer 60 s) consulta `/api/auth/version` para detectar diferencias.  
- **Sesiones forzadas:** al revocar sesiones desde la UI, se insertan registros en `UsuarioSesiones` con `IsActive=false`, se emite `SessionRevoked` y el cliente muestra modal "Tu sesión fue cerrada por seguridad".

### 17.5 Experiencia MAUI y accesibilidad
- **MAUI admin:** se replica funcionalidad esencial (lista de usuarios con CollectionView virtualizada, detalle en página independiente). Rol assignment usa `BindableLayout` con chips y bottom sheet para permisos. Gestos adaptados (swipe para acciones rápidas).  
- **Sincronización compartida:** ViewModels reutilizan servicios compartidos; `SecurityNotificationsHub` usa `SignalR.Client` con reconexión automática y notificaciones locales cuando el app está en background.  
- **Accesibilidad/UX:** se usan labels descriptivos, atajos (`Ctrl+K` para buscar, `/` para foco en buscador), tooltips/semántica y confirmaciones claras ("¿Seguro que deseas remover el rol Supervisor RRHH?"). Estados largos muestran skeletons y mensajes amistosos.  
- **Invitación de usuarios:** botón "Invitar" abre wizard (datos mínimos + rol/contexto). Tras guardar, se envía email con link de activación; UI monitorea estado "Pendiente" y ofrece reenviar.  
- **Auditoría y reporting:** panel lateral con bitácora contextual (últimos cambios sobre el usuario/rol seleccionado) y acceso a export JSON/CSV para cumplimiento. Métricas (latencia de guardado, ratio de éxito de refresh) alimentan dashboards de seguridad.

### 17.6 Observabilidad y pruebas
- **Telemetría:** eventos `UserAdmin.Search`, `UserAdmin.Save`, `RoleAdmin.Save`, `PermissionsHotReload.Success|Failure`, `SessionRevoked`. Incluyen tenant, duración, volumen afectado y `result`.  
- **Alertas:** si más del 2% de los refresh fallan en 5 min, se genera alerta (posible caída del hub o API).  
- **Pruebas recomendadas:** tests UI bUnit/Playwright para tabla/panel, pruebas de integración para `SecurityInvalidationProcessor`, pruebas de carga sobre SignalR (escenarios con miles de usuarios conectados) y validaciones de accesibilidad (axe).  
- **Hardening:** fuzzing de API (códigos maliciosos en emails/nombres), verificación de límites (paginación, bulk actions) y pruebas de regresión para garantizar que usuarios sin permisos no ven la sección.

---
