# Migracion a Emplyx 4.0 - Plan de Adaptacion de UI (Blazor + MAUI con Bootstrap)
Version: 0.2 (borrador)
Fecha: {{actualizar}}
Autor: Equipo Emplyx

## 1) Objetivo
Definir un plan de migracion y adaptacion de la interfaz de usuario (UI), menus y pantallas del sistema existente en `VisualTime/Src/VTLive40` (ASP.NET WebForms, VB) hacia una nueva capa de presentacion basada en Blazor (web) y .NET MAUI (apps), utilizando Bootstrap como sistema de diseno, manteniendo como minimo la misma funcionalidad disponible para el cliente.

No se realizaran cambios de logica de negocio en esta fase. El alcance se limita al relevamiento, analisis, diseno, arquitectura de UI y planificacion de implementacion, con una lista To Do de seguimiento.

Importante: se tomara la logica del sistema actual solo como referencia funcional (reglas, flujos, validaciones). No se reutilizara ni portara codigo legacy; la UI se reescribira con componentes nuevos.

Restricciones claves:
- No se realizaran cambios en el codigo legacy bajo `VisualTime/Src/VTLive40`.
- No se usara el nombre "VTLive" en nuevos artefactos. El nuevo producto se denomina "Emplyx".
- Todo lo nuevo se creara/modificara bajo la solucion/repositorio Emplyx.
- No se reutilizara codigo de WebForms/VB, ni scripts/CSS legacy. La logica se usara unicamente como referencia de comportamiento.

## 2) Alcance y supuestos
- Alcance: UI/UX, menus, pantallas, formularios, validaciones, Navegacion, modales, tablas, filtros, y elementos visuales. No incluye reescritura de logica de negocio o backend.
 - Frameworks destino: .NET 9 (C#) Blazor (preferencia por Blazor Server para acelerar adopcion y simplificar despliegue) y .NET 9 MAUI para apps. Diseno en Bootstrap 5 (tema consistente en web y app).
- Funcionalidad minima: paridad funcional con el sistema actual, priorizando pantallas criticas y flujos mas usados.
- Autenticacion y seguridad: se mantiene el modelo actual. Si es necesario, se agregan adaptadores temporales desde UI para reutilizar endpoints existentes (`.ashx`, llamadas AJAX desde `.aspx`, etc.).
- Internacionalizacion (i18n) y accesibilidad: se contemplan desde el diseno (nivel AA como objetivo).
- Estrategia de entrega: migracion incremental modulo por modulo, con convivencia de UI nueva y legado durante la transicion.

Politica de nombres y ubicacion:
- Nuevos proyectos, namespaces, rutas y paquetes usaran el prefijo `Emplyx`.
- El arbol `VisualTime/Src/VTLive40` es solo referencia para relevamiento; no se modifica.

Reutilizacion de codigo:
- No se copiara ni portara codigo legacy (VB, .aspx, .js, .css).
- Se podran consumir endpoints existentes temporalmente como "cajas negras" mediante adaptadores (sin reutilizar su implementacion) para asegurar paridad funcional durante la transicion.
- Se definiran contratos de datos nuevos y estables para la UI. Los adaptadores son transitorios.

## 3) Diagnostico inicial (relevamiento rapido del repo)
Stack actual observado:
- ASP.NET WebForms con VB (`.aspx`, `.vb`, `.designer.vb`), `Web.config` y transformaciones.
- Modulos/patrones detectados por carpetas y archivos clave:
  - `VTLogin/` (login, portal), `Auth/` (autenticacion)
  - `Main.aspx` (shell/entrada)
  - `Alerts/` (listado, detalle, cola de tareas, descargas)
  - `Cameras/` (paginas, selector, mensajes, handler `Handlers/srvCameras.ashx`)
  - `AIScheduler/` (unidades productivas, budgets, selector de fechas, JS y CSS especificos)
  - `Wizards/` (asistentes como NewTerminal y EmergencyReports)
  - `Diagnostics/Status.aspx`
  - `Audit/` (estructura presente)
  - Scripts JS por modulo, CSS especificos y recursos estaticos

Observaciones relevantes:
- Acoplamiento UI/Backend tipico de WebForms (ViewState, Postbacks, PageMethods, handlers `.ashx`).
- Varias paginas con logica de UI implementada en JS y code-behind VB.
- Presencia de assets y estilos por modulo (p.ej., `Alerts/css/alerts.css`, `AIScheduler/Styles/Budgets.css`).
- La migracion 1:1 de WebForms a componentes Blazor requiere diseno de componentes y, en algunos casos, adaptadores a endpoints JSON para desacoplar la UI.

## 4) Estrategia de migracion (solo UI)
Enfoque incremental tipo "strangler":
1. Descubrimiento y alineacion: inventariar pantallas, reglas y validaciones; priorizar por uso/impacto; definir criterios de paridad funcional.
2. Diseno/Arquitectura UI: establecer layout, navegacion, set de componentes base, tokens de diseno y librerias, sin portar codigo.
3. Implementacion incremental: reescritura de UI por modulos con componentes nuevos; convivencia con legado y redireccion gradual de rutas.
4. Validacion/UAT: pruebas con usuarios clave y correcciones; preparacion de rollout por etapas.

Entregables clave por fase:
- Inventario de pantallas y flujos; mapa de Navegacion; criterios de aceptacion.
- Sistema de diseno Bootstrap (tokens, tema, componentes base); layout master; guidelines.
- Componentes y paginas Blazor/MAUI por modulo con adaptadores a endpoints existentes (temporalmente).
- Plan de transicion y cutover; checklist de paridad y pruebas.

## 5) Arquitectura UI propuesta
Estructura sugerida (monorepo, orientativia):
- `src/Emplyx.Blazor` - Aplicacion web Blazor Server con Bootstrap 5.
- `src/Emplyx.Maui` - Aplicacion .NET MAUI con estilos y tokens compartidos.
- `src/Emplyx.Shared` - Modelos, DTOs, contratos de servicios, validaciones y recursos i18n.
- `src/Emplyx.ApiAdapter` - Capa de adaptadores a endpoints legacy (p.ej., wrapping de `.ashx` o metodos `.aspx`) exponiendo JSON simplificado para UI.
  - Nota: estos adaptadores consumen endpoints existentes como cajas negras, sin reutilizar ni portar codigo legacy. Son temporales hasta contar con APIs definitivias.
  - Objetivo de frameworks (TFM): `net9.0` para Emplyx.Blazor y Emplyx.Shared. Emplyx.Maui segun plataformas objetivo (`net9.0-android`, `net9.0-ios`, `net9.0-maccatalyst`, `net9.0-windows`).

## 5.1) Alineacion con principios Emplyx (referencias)
Este plan adopta los principios definidos en `docs/Emplyx_Guia.md` y cada pantalla nueva debe alinearse a ellos:
- Roles, permisos, jerarquia y auditoria: ver `docs/Emplyx_Guia.md#2-modelo-organizativo-y-seguridad-autorizaciones`.
- Workflows y analisis por funcionalidad (estados, aprobaciones, SLA): ver `docs/Emplyx_Guia.md#6-plantilla-de-analisis-por-funcionalidad` y `docs/Emplyx_Guia.md#5-catalogo-completo-de-funcionalidades-85-items`.
- Licenciamiento y ediciones: ver `docs/Emplyx_Guia.md#9-licenciamiento-y-ediciones-vision`.
- Arquitectura y multi-tenant: ver `docs/Emplyx_Guia.md#3-arquitectura-de-referencia`.
- Requisitos no funcionales (accesibilidad, rendimiento, seguridad): ver `docs/Emplyx_Guia.md#10-requisitos-no-funcionales-nfr`.
- Agente de IA en vistas: ver `docs/Emplyx_Guia.md#4-agente-de-ia-en-todas-las-vistas`.

Implicancias para la UI:
- Guardas de UI por RBAC/ABAC: habilitar/ocultar acciones y datos por rol/atributos; negar prevalece sobre permitir.
- Gating por licencia: caracteristicas visibles/ocultas o modo degradado segun edicion/licencia activia.
- Workflows: estados y transiciones explicitos (por ejemplo, solicitudes → aprobado/rechazado), con permisos de aprobar/ejecutar.
- Auditoria: registrar acciones releviantes (ver, crear, editar, aprobar, exportar) y exponer motivos en flujos criticos.
- Multi-tenant: contexto de tenant/empresa presente en la UI (filtros, encabezados, breadcrumbs cuando aplica).

## 5.2) Resumen de principios Emplyx (breve)
- Seguridad y permisos: RBAC con herencia/denegacion y ABAC por atributos; negar prevalece sobre permitir; auditoria completa de acciones clave.
- Workflows: estados y transiciones explicitas con roles de aprobacion; SLA/tiempos cuando aplique; bitacora de cambios.
- Licenciamiento: gating de funcionalidades por edicion/licencia; modo degradado o oculto segun corresponda.
- Arquitectura: API-first, eventos asincronos, DTO/servicios compartidos; desacople via adaptadores temporales.
- Multi-tenant: jerarquia y aislamiento por tenant/empresa/unidad; contexto visible en la UI.
- Cumplimiento: priviacidad/PII, trazabilidad, exportaciones firmadas; i18n y accesibilidad como baseline.
- Observiabilidad: metricas, trazas y logs releviantes; estados de error consistentes en UI.
- IA en vistas: asistente seguro por permisos, con explicabilidad y confirmaciones para acciones criticas.

Diseno del shell (Blazor):
- `MainLayout` con Sidebar (menu), Topbar (usuario/acciones rapidas), Breadcrumbs, y contenedor de contenidos.
- Proteccion de rutas por rol/permiso (mapeo desde configuracion actual).
- Estados de carga viacia, error, viacio (empty states) y feedback consistente.

Librerias de UI (Bootstrap-first):
- Bootstrap 5 como base (utilidades responsive, grid, modales, offcanvas, toasts).
- DataGrid/Tabla: evaluar `BlazorBootstrap`, `BootstrapBlazor` o `Blazorise (tema Bootstrap)` para datagrid/virtual scroll/filtros.
- Iconografia: Bootstrap Icons.
- Formularios: `EditForm` + `DataAnnotations` + validaciones personalizadas.

MAUI:
- Reutilizacion de `Emplyx.Shared` para modelos/servicios.
- Estilos XAML con equivalentes a tokens de diseno (colores, tipografias, espaciados).
- Navegacion por Shell y paginas espejo de las rutas web, cuando aplique (no todas las pantallas deben existir en movil).

## 6) Mapeo de modulos y pantallas (propuesta inicial)
Rutas Blazor sugeridas (un espejo MAUI cuando corresponda):
- Autenticacion / Inicio
  - `Pages/Account/Login.razor` (VTLogin/Auth)
  - `Pages/Home.razor` (equivalente a `Main.aspx`)
- Alerts
  - `Pages/Alerts/Index.razor` (listado de alertas con filtros y paginacion)
  - `Pages/Alerts/Detail.razor` (detalle de alerta)
  - `Pages/Alerts/TasksQueue.razor` (cola de tareas)
- Cameras
  - `Pages/Cameras/Index.razor` (listado/visor)
  - `Pages/Cameras/Selector.razor` (selector de camaras)
  - `Shared/Dialogs/MessageBox.razor` (reemplazo de `srvMsgBoxCameras.aspx`)
- AI Scheduler
  - `Pages/Scheduler/Units.razor` (ProductiveUnit)
  - `Pages/Scheduler/Budgets.razor`
  - `Shared/Controls/DateSelector.razor`
- Wizards
  - `Pages/Wizards/NewTerminal.razor`
  - `Pages/Wizards/EmergencyReports.razor`
- Diagnostics
  - `Pages/Diagnostics/Status.razor`

Para cada pantalla:
- Definir componentes (tabla, formulario, modal, filtros, paginator) y fuentes de datos.
- Identificar dependencias con JS legacy y handlers `.ashx` / metodos en `.aspx`.
- Plan de datos: usar `ApiAdapter` para exponer/consumir JSON (temporal), y/o definir contratos definitivos.

## 7) Lineamientos de UX/UI
- Responsive 100% (grid Bootstrap, breakpoints, mobile-first).
- Accesibilidad AA: foco visible, contraste, labels/aria, orden tabulable, accesos por teclado.
- Interaccion consistente: estados de carga, errores, confirmaciones, toasts; evitar bloqueos innecesarios.
- Internacionalizacion: texto y formatos centralizados (reutilizar recursos si existen, o preparar `resx`).
- Componentes reusables: Tabla, Form, Modal, ConfirmDialog, DatePicker, Selector generico, Badges/Chips, Toolbar de filtros.
- Guia visual: tokens de color, tipografia, espaciados, radios, sombras; tema claro/oscuro (opcional).
- No reutilizar assets legacy: iconos, estilos y scripts seran nuevos o de librerias estandar.
 - Respetar principios Emplyx: controles y pantallas deben aplicar permisos/atributos y gating de licencia de forma consistente.

## 11.2) Componentes base y patrones (especificacion funcional)
Componentes (Blazor, tema Bootstrap):
- AppLayout
  - Partes: Sidebar (menu por dominios, responsive), Topbar (usuario/acciones rapidas), Breadcrumbs, Content.
  - Requisitos: accesible (tab order), colapsable, persistencia local de preferencia.
- AppMessageBox / ConfirmDialog
  - Props: title, message (soporta i18n), severity: info|success|warning|error, buttons: [{id,label,variant}], defaultButton.
  - Eventos: onClose(result), onConfirm(). Accesible (focus trap, ESC cierra si permitido).
- AppToast
  - Props: severity, message, timeout, position. Util para feedback no modal.
- AppDataGrid
  - Props: columns:[{field,header,type,format,width,sortable,filterable,template?}], dataSource (server-side), page,size,total, sort, filter.
  - Eventos: onPage, onSort, onFilter, onRowClick, onRowAction.
  - Requisitos: accesible (teclado), vacios/errores/cargando; seleccion simple/multiple; columnas ocultables.
- AppSelector (generico)
  - Props: entity, multi:boolean, page,size, filter, displayField, valueField; fetch:url o provider.
  - Funciones: buscable (debounce), paginacion, seleccion multiple, etiquetas de seleccion.
  - Uso: reemplazo de SelectorData.aspx (Employee, Concept, Zone, Status, etc.).
- AppForm / AppField
  - Basado en EditForm + DataAnnotations + validadores custom. Soporte de Summary y mensajes inline.
  - Campos: InputText, Number, Select (enum), DatePicker, DateRangePicker, Switch, Tags, File.
- AppDateRangePicker
  - Props: from, to, min?, max?, required?, validateRange?:boolean.
  - Regla: si validateRange, forzar from ≤ to y minimo de 2 fechas cuando aplique (Scheduler, Alerts).
- AppTasksQueue
  - Props: dataSource (server-side), columnas estandar (id, type, status, progress, startedAt, finishedAt).
  - Uso: Alerts TasksQueue, DataLink jobs, broadcasts de terminals.

Patron de descargas/exports
- API: GET /{dominio}/download/{id} devuelve stream con Content-Disposition y Content-Type correcto.
- UI:
  - Usa fetch + blob + enlace ancla oculto; maneja progreso si el backend lo expone; muestra toast de exito/error.
  - Registra auditoria (UI event: download.started/completed/failed) con correlationId.
- Errores: mapear 403/404/410/5xx a mensajes de usuario consistentes; ofrecer reintento si backend temporalmente no disponible.

Contexto multi-tenant
- TenantBar: selector de Tenant/Empresa/Unidad (segun permisos) con cascada de valores y cache local.
- ContextService: expone tenantId/companyId/unitId, culture, timeZone; inyectado via CascadingValue.
- Efectos: filtra dataSources, breadcrumbs y headers de auditoria/breadcrumb; integra RBAC/ABAC.

Observiabilidad UI
- Eventos: view.opened, action.clicked, data.load.started/completed/failed, download.started/completed/failed, job.poll.tick.
- Campos minimos: name, page, entity, id?, correlationId, durationMs, ok, errorCode?.
- Integracion: Application Insights o equivalente.

Accesibilidad
- Navegacion por teclado en todos los componentes; focus visible; roles aria correctos; contraste AA; labels asociadas.

Tokens de diseno (Bootstrap)
- Colores (brand-primary/secundario), spacing (xs–xl), radius, shadows, z-index; dark mode opcional.

Interfaces internas (adapter)
- IDataSource<T>: Task<PagedResult<T>> Load(DataQuery q);
- IDownloads: Task<Stream> DownloadAsync(id, token);
- IJobs: Task<JobStatus> GetStatus(id);

## 8) Pruebas y aceptacion
- Criterios por modulo (paridad funcional minima + UX basica):
  - Navegacion equivalente y permisos respetados.
  - Datos correctos, filtros y paginacion funcionales.
  - Formularios con validacion y mensajes de error claros.
  - Rendimiento aceptable (tiempo de primer render y operaciones basicas).
- UAT por usuarios clave y checklist de paridad antes de desactivar pantalla legacy.
 - Verificacion de principios Emplyx: pruebas de limites de permisos (RBAC/ABAC), gating por licencia, y trazabilidad de acciones clave.

## 9) Cronograma de alto nivel (estimativo)
- Semanas 1-2: relevamiento, inventario, mapa de Navegacion, criterios de aceptacion.
- Semanas 3-4: Sistema de diseno (Bootstrap), layout, componentes base.
- Semanas 5-8: Modulos Login/Home/Alerts.
- Semanas 9-12: Modulos Cameras/AIScheduler.
- Semanas 13-14: Wizards/Diagnostics y cierre de brechas.
- Semanas 15-16: UAT, performance, accesibilidad, hardening y plan de cutover.
Nota: Ajustar por carga real de pantallas/flujo y complejidad tecnica.

## 10) Riesgos y mitigaciones
- Dependencias a WebForms/handlers no JSON: crear adaptadores rapidos (`ApiAdapter`) que devuelvan JSON estable para UI.
- Gaps de librerias de UI (grids av avanzados): escoger una libreria bootstrap-based estable; evitar lock-in si no hay licencias.
- Diferencias de auth/session: validar compatibilidad y, de ser necesario, incorporar endpoints minimos para tokens/cookies.
- Carga de trabajo subestimada: priorizar por uso/impacto; entregar valor incremental.
- Discrepancias UX: validar wireframes/maquetas con stakeholders antes de construir.
- Divergencia de reglas al reescribir: documentar reglas por pantalla y casos limite; tests de aceptacion por modulo.

## 11) Lista To Do
_Leyenda de prioridad: P0 = inmediato (iteracion actual), P1 = siguiente iteracion, P2 = backlog. Formato: (Prioridad | Estado | Responsable + fecha).

Actualizado por Codex el 2025-11-12.

### 11.2 Checklist - Componentes base y patrones (Emplyx)
- [x] (P0 | Completado | Codex 2025-11-12) Confirmar estructura de soluciones: Emplyx.Blazor, Emplyx.Shared, Emplyx.ApiAdapter, Emplyx.Maui (sin directorio VisualTime).
- [x] (P0 | Completado | Codex 2025-11-12) Verificar TFM: net9.0 (Blazor/Shared) y net9.0-* (MAUI por plataforma).
- [x] (P0 | Completado | Codex 2025-11-12) Integrar Bootstrap 5 + Bootstrap Icons; definir tokens (colores, spacing, radius, sombras, dark mode opcional).
- [ ] (P0 | En curso | Codex 2025-11-12) Seleccionar DataGrid (Blazorise/BootstrapBlazor/BlazorBootstrap) y estandarizar API de columnas/eventos. (Evaluar libreria; sin paquete disponible en feed actual, se usa Bootstrap nativo hasta seleccionar DataGrid definitivo).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppLayout (Sidebar/Topbar/Breadcrumbs/Content) accesible y responsive.
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppMessageBox y ConfirmDialog (i18n, focus trap, variantes) y AppToast.
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppDataGrid (server-side) con onPage/onSort/onFilter/onRowAction y estados vacio/error/cargando (Filtros básicos + acciones por fila integradas en Emplyx.Blazor/Pages/Index.razor).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppSelector generico (debounce, multi, etiquetas) para reemplazar *SelectorData.aspx (AppSelector + EmployeesMockDataSource.SearchAsync).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppForm/AppField (EditForm + DataAnnotations + validadores custom) y AppDateRangePicker (from ≤ to; min 2 fechas cuando aplique). AppForm ahora admite Model o EditContext externo, validators adicionales y manejo de estados `IsBusy`; AppDateRangePicker agrega clamps min/max, `MinimumSpanDays`, `RangeChanged` y `ValidationMessage` por campo/gap.
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppTasksQueue (id,type,status,progress,startedAt,finishedAt) reutilizable (Alerts/DataLink/Broadcasts) con AppTasksQueue + SampleTasksQueueService.
- [ ] Definir Contexto multi-tenant: TenantBar + ContextService (tenant/company/unit, culture, timeZone) via DI/CascadingValue.
- [x] (P0 | Completado | Codex 2025-11-12) Definir Contexto multi-tenant: TenantBar + ContextService (tenant/company/unit, culture, timeZone) via DI/CascadingValue.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Gating por licencia: condiciones centralizadas (feature flags/ediciones) que controlan visibilidad/estado de componentes.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Patron de descargas: cliente de downloads (fetch->blob) con auditoria (download.started/completed/failed) y manejo de 403/404/410/5xx.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Observiabilidad UI: instrumentar eventos (view.opened, data.load.*, action.clicked, job.poll.tick) con correlationId (App Insights u otro).
- [x] (P0 | Completado | Codex 2025-11-12) API Adapter: estandarizar envelope {ok,data,meta,error}; mapear HTTP/errores a UI (toasts/dialogos) y reintentos/backoff donde aplique.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Internacionalizacion: resx centralizados (sustituir Globalize); catalogo minimo de mensajes; cultura/timezone del ContextService.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Accesibilidad AA: Navegacion por teclado/aria/contraste/focus visible en todos los componentes.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Remocion de dependencias legacy: sin ExtJS/Globalize/VisualTime; Verificacion de nombres/paths.
- [ ] (P2 | Backlog | Codex 2025-11-12) CI/linting: Verificacion TFM net9, builds para MAUI target elegidos; chequeo de estilos y accesibilidad (opcional).
 (seguimiento)

#### Bitacora 2025-11-12 (Codex)
- Emplyx.Blazor (Blazor Server .NET 9) y Emplyx.ApiAdapter añadidos a la solucion (`Emplyx.sln`), manteniendo `Emplyx.Shared` como nucleo compartido.
- Bootstrap 5 + Bootstrap Icons integrados; `wwwroot/css/site.css` define tokens iniciales, layout y componentes visuales alineados con el lineamiento 11.2.
- AppLayout implementado (topbar + sidebar + breadcrumbs) consumiendo `TenantContextState` para multi-tenant, con NavMenu priorizado segun 11.4.
- AppUiMessaging (MessageBox/ConfirmDialog/Toast), AppDataGrid (onPage/busqueda/sort) y AppSelector (debounce + multi-select) agregados como componentes reutilizables en `Emplyx.Blazor`.
- AppDataGrid y AppSelector utilizan `EmployeesMockDataSource` como provider `GetAsync/SearchAsync`, demostrando interaccion server-side y busqueda con debounce.
- AppTasksQueue disponible con `SampleTasksQueueService` para monitorear estados (pending/running/completed/failed) y reintentos; módulo Employees ya cuenta con un prototipo en `Emplyx.Blazor/Pages/Employees/Index.razor` + `Pages/Employees/Edit.razor` usando AppForm/AppField iniciales.
- AppTasksQueue disponible con `SampleTasksQueueService` para monitorear estados (pending/running/completed/failed) y reintentos.
- AppField refactorizado (Emplyx.Blazor/Components/Forms/AppField.razor) para soportar `TValue` generico (text/select/number) con validacion de tipos y `ValueChanged` consistente; `dotnet build Emplyx.sln` vuelve a compilar sin errores.
- Vista `/employees` consume `EmployeesMockDataSource` + AppDataGrid/AppSelector/AppTasksQueue con filtros status/module/group/forgotten-right y acciones mock para delete/get; `Pages/Employees/Index.razor` refleja contratos GET /employees del inventario 11.1.
- Vista `/access/groups` consume `AccessGroupsMockDataSource` + AppDataGrid/AppSelector con filtros status/zone/period/pending y acciones mock copy/delete/emptyEmployees; cubre AccessGroups.aspx (srvAccessGroups.ashx) dentro de 11.1.
- Vista `/access/periods` consume `AccessPeriodsMockDataSource` + AppDataGrid con filtros status/weekDay/month/special y acciones mock delete/get, alineado a AccessPeriods.aspx (srvAccessPeriods.ashx).
- Vista `/access/status` consume `AccessStatusMockDataSource` + AppDataGrid con filtros group/zone/severity/attention y acciones detail/block, reemplazando AccessStatusMonitor.aspx (srvAccessStatus.ashx).
- Vista `/access/zones` consume `AccessZonesMockDataSource` + AppDataGrid con filtros type/parent/critical y acciones detail/delete, sustituyendo AccessZones.aspx (srvAccessZones.ashx).
- Vista `/access/events` consume `AccessEventsMockDataSource` + AppDataGrid con filtros zone/group/status y acciones copy/delete, cubriendo EventScheduler.aspx.
- Vista `/scheduler/units` consume `ProductiveUnitsMockDataSource` + AppDataGrid con filtros plant/status/manager y acciones detail/delete, equivalente a AIScheduler/ProductiveUnit.aspx.
- Vista `/scheduler/budgets` consume `BudgetsMockDataSource` + AppDataGrid con filtros unit/period/status y acciones detail/delete, reproduciendo AIScheduler/Budget.aspx.
- Vista `/scheduler/moves` consume `MovesMockDataSource` + AppDataGrid con filtros employee/terminal/type/status y acciones detail/delete, reemplazando Scheduler/MovesDetail.aspx.
- Vista `/scheduler/coverage` consume `CoverageSummaryMockDataSource` + AppDateRangePicker/cards para mostrar coverage/incidents, replicando CalendarV2.
- AppForm (Emplyx.Blazor/Components/Forms/AppForm.razor) soporta `EditContext` externo, registro de validadores personalizados, `IsBusy` y cascada de modelos; AppDateRangePicker aplica `MinimumSpanDays`, clamps Min/Max e invoca `RangeChanged`/`ValidationMessage` por campo.
- ApiAdapterClient con envelope `{ok,data,meta,error}` y headers multi-tenant; configuracion `ApiAdapter` en `appsettings*.json`.
- ContextService (`TenantContextState` + `TenantContextProvider`) disponible via DI/CascadingValue para Blazor y adaptadores.

Comentario importante:
- Esta lista es viva. A medida que avancemos, cada punto debe actualizarse/expandirse para reflejar decisiones, dependencias, riesgos y pendientes reales.
- El punto 11.1 (relevamiento e inventario) es el mas importante: de ese trabajo saldra la guia definitiva para el desarrollo UI y alimentara los demas subpuntos (11.2-11.6).
- [ ] (P0 | En curso | Codex 2025-11-12) Registrar fecha/autor en actualizaciones clave del To Do para trazabilidad.

### 11.1 Relevamiento e inventario
- Estado: cobertura completa (v0). Inventario: `docs/Inventario 11.1 Emplyx.md`.
- Enlaces rapidos: Detalle Employees y Access en `docs/Inventario 11.1 Emplyx.md` (secciones "Detalle - Employees (v0)" y "Detalle - Access (v0)").
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/employees` en Emplyx.Blazor implementa listado, filtros (status/module/group/forgotten-right) y selector multi, siguiendo contratos GET /employees y SearchEmployees v0.1.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/access/groups` replica AccessGroups.aspx con AppDataGrid + filtros (status/zone/period/pending), selector y acciones mock (copy/delete/empty) apoyadas en `AccessGroupsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/access/periods` cubre AccessPeriods.aspx (filtros status/weekDay/month/special) con AppDataGrid y contratos compartidos (`AccessPeriodList*` + `AccessPeriodsMockDataSource`).
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/access/status` cubre AccessStatusMonitor.aspx (filtros group/zone/severity/attention) con AppDataGrid y `AccessStatusMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/access/zones` cubre AccessZones.aspx (filtros type/parent/critical) con AppDataGrid y `AccessZonesMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/access/events` replica srvEventsScheduler (copy/delete + filtros zone/group/status) con `AccessEventsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/scheduler/units` replica AIScheduler/ProductiveUnit.aspx (filtros plant/status/manager) con `ProductiveUnitsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/scheduler/budgets` replica AIScheduler/Budget.aspx (filtros unit/period/status) con `BudgetsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/scheduler/moves` replica Scheduler/MovesDetail.aspx (filtros employee/terminal/type/status) con `MovesMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Vista `/scheduler/coverage` replica CalendarV2 (rango fechas + status) con `CoverageSummaryMockDataSource`.
- Avance: parametros por accion extraidos y contratos v0.1 propuestos (Employees/Access/Scheduler/Security) con ejemplos.
- Nuevos: Terminals, Tasks y TaskTemplates inventariados con endpoints, parametros y contratos v0.1 sugeridos.
- Progreso reciente (Employees): listado migrado a `Emplyx.Blazor/Pages/Employees/Index.razor` utilizando AppDataGrid/AppSelector/AppTasksQueue como base para futuras pantallas del modulo.

Estado de cobertura (inventario 11.1):
- [x] VTLogin/Auth
- [x] Alerts, Cameras, AIScheduler, Wizards, Diagnostics, Audit
- [x] Employees, Access
- [x] Scheduler, Security
- [x] Terminals, Tasks, TaskTemplates
- [x] Shifts, Documents
- [x] Options, Requests, Indicators, SecurityChart, Concepts, Causes, Notifications, DataLink, LabAgree, DiningRoom, SDK, Absences, Base
- [ ] (P1 | Pendiente | Codex 2025-11-12) Revision fina de validaciones por ASPX (en progreso).

- [ ] (P0 | En curso | Codex 2025-11-12) Listar todas las paginas `.aspx` por modulo y su proposito.
- [ ] (P0 | En curso | Codex 2025-11-12) Identificar scripts JS y CSS por modulo, dependencias y uso.
- [ ] (P0 | En curso | Codex 2025-11-12) Mapear handlers `.ashx` y endpoints invocados por AJAX.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Releviar roles/permiso por ruta (autorizacion actual en `Web.config`).
- [ ] (P0 | En curso | Codex 2025-11-12) Documentar flujos criticos (login, Navegacion principal, alertas, camaras, scheduler).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Documentar reglas de negocio y validaciones por pantalla (sin copiar codigo).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Identificar calculos/formatos y edge cases en code-behind/JS para replicar comportamiento.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Validaciones afinadas por modulo: ver `docs/Inventario 11.1 Emplyx.md` (Anexo - validaciones afinadas) para Employees/Access/Scheduler/Security.
- [ ] (P0 | En curso | Codex 2025-11-12) Priorizar modulos por impacto/volumen: Employees, Access, Scheduler, Base (compartidos), Security, Terminals, Tasks, Shifts.
- [ ] (P0 | En curso | Codex 2025-11-12) Para modulos priorizados: completar listado de ASPX + proposito + dependencias (scripts/css) por pagina.
- [ ] (P0 | En curso | Codex 2025-11-12) Detectar llamadas AJAX y patrones de WebForms: `$.ajax`, `__doPostBack`, `PageMethods`, `[WebMethod]` en code-behind.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Clasificar handlers `.ashx` por dominio y extraer parametros de entrada/salida observables.
- [ ] (P0 | En curso | Codex 2025-11-12) Proponer contratos JSON v0 para top handlers (p.ej., `srvEmployees`, `srvScheduler`, `srvCameras`, `srvRequests`, `srvShifts`, `srvTerminals`, `srvIndicators`, `srvAccessStatus`, `srvNotifications`, `srvTaskTemplates`).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Documentar flujo de descargas/exports (p.ej., `Alerts/downloadFile.aspx`) y necesidades de seguridad/auditoria.
- [ ] (P0 | En curso | Codex 2025-11-12) Mantener `docs/Inventario 11.1 Emplyx.md` sincronizado con hallazgos (fecha/autor).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Contratos normalizados v1.0 top-20 cerrados + DTOs/enums (ver Inventario: Anexos Contratos/DTOs/Errores).

### 11.2 Diseno y arquitectura UI
- [ ] (P0 | En curso | Codex 2025-11-12) Definir layout principal, Navegacion y breadcrumbs.
- [ ] (P0 | En curso | Codex 2025-11-12) Definir tokens de diseno (colores, tipografias, espaciados) para Bootstrap.
- [ ] (P0 | En curso | Codex 2025-11-12) Seleccionar libreria de componentes (p.ej., BlazorBootstrap/BootstrapBlazor/Blazorise Bootstrap).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Definir patron de formularios y validaciones (EditForm + DataAnnotations + helpers).
- [ ] (P0 | Planificado | Codex 2025-11-12) Definir DataGrid estandar (paginacion, sorting, filtros, columnas configurables).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Lineamientos de accesibilidad e i18n.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Especificar reglas funcionales derivadas del legacy (como requerimientos), no como codigo a portar.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Mapear roles/permisos (RBAC/ABAC) por pagina/accion segun `Emplyx_Guia.md#2-modelo-organizativo-y-seguridad-autorizaciones`.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Definir gating por licencia (que se oculta/inhabilita/degrada) segun `Emplyx_Guia.md#9-licenciamiento-y-ediciones-vision`.
- [ ] (P2 | Backlog | Codex 2025-11-12) Incorporar plantilla de workflow por funcionalidad segun `Emplyx_Guia.md#6-plantilla-de-analisis-por-funcionalidad`.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Definir Patron de descargas/exports (streaming, feedback, seguridad/auditoria).
- [ ] (P0 | En curso | Codex 2025-11-12) Definir barra/selector de contexto multi-tenant (tenant/empresa/unidad) y breadcrumbs.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Definir patron de `MessageBox/ConfirmDialog` y de `TasksQueue` (cola) reutilizable.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Definir componente `Selector` generico (buscables con filtros) para entidades frecuentes.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Lineamientos de observiabilidad UI (telemetria de vistas, errores y latencias).
 - [ ] (P1 | Pendiente | Codex 2025-11-12) Mapear controles Base (ExtJS y propios) a componentes Blazor equivalentes (Dialog, DatePicker, DataGrid, FilterList/Selector, EmployeeSelector).
 - [ ] (P0 | Planificado | Codex 2025-11-12) Especificar contratos de adaptadores top-20 finales (a partir de v0.1) con ejemplos consolidados.

### 11.3 Infraestructura de proyectos (sin implementacion de logica)
- [x] (P0 | Completado | Codex 2025-11-12) Definir estructura de proyectos: `Emplyx.Blazor`, `Emplyx.Maui`, `Emplyx.Shared`, `Emplyx.ApiAdapter`.
- [x] (P0 | Completado | Codex 2025-11-12) Especificar dependencias de UI (Bootstrap 5, iconos, libreria de componentes).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Plan de estilado: variables CSS (Bootstrap) y equivalentes MAUI.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Definir rutas y convenciones de nombres para paginas y componentes.
- [ ] (P0 | En curso | Codex 2025-11-12) Confirmar que no se copiaran scripts/CSS legacy; plan de reescritura de componentes.
 - [ ] (P0 | En curso | Codex 2025-11-12) Fijar TFM objetivo: `net9.0` (Blazor/Shared) y `net9.0-*` para MAUI por plataforma.
 - [ ] (P0 | En curso | Codex 2025-11-12) Configurar Bootstrap 5 + tema (tokens) e iconografia (Bootstrap Icons).
 - [ ] (P0 | En curso | Codex 2025-11-12) Seleccionar y configurar libreria de DataGrid (Blazorise/BootstrapBlazor/BlazorBootstrap).

### 11.4 Mapeo y diseño por modulo (UI)
- [ ] (P0 | Planificado | Codex 2025-11-12) Login/Auth: pantallas y flujo, mensajes de error, expiracion sesion.
- [ ] (P0 | Planificado | Codex 2025-11-12) Home/Main: estructura de menu, landing, accesos rapidos.
- [ ] (P0 | Planificado | Codex 2025-11-12) Alerts: lista, detalle, cola; filtros; acciones en lote; descarga de archivos.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Cameras: grillas/visor, selector, modales de confirmacion.
- [ ] (P1 | Pendiente | Codex 2025-11-12) AI Scheduler: unidades, budgets, selector de fecha; componentes especificos.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Wizards: NewTerminal y EmergencyReports; pasos, validaciones y confirmaciones.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Diagnostics: Status y mensajes del sistema.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Para cada modulo: definir workflow (estados/transiciones), permisos por accion y condiciones de licencia.
 - [ ] Employees (listado/detalle, grupos) - prioridad alta.
 - [ ] Access (zonas, periodos, estados) - prioridad alta.
 - [ ] Scheduler (planificador/movimientos) - prioridad alta.
 - [ ] Security (opciones, supervisores) - prioridad alta.
 - [ ] Terminals (listado, wizard) - prioridad alta.
 - [ ] Tasks y TaskTemplates - prioridad media.
 - [ ] Shifts - prioridad media.
 - [ ] Documents - prioridad media.

### 11.5 Adaptadores y datos (minimos para UI)
- [ ] (P0 | En curso | Codex 2025-11-12) Identificar endpoints reutilizables (incluyendo `.ashx`) y su contrato actual.
- [ ] (P0 | En curso | Codex 2025-11-12) Definir contratos JSON temporales para UI (por modulo/pantalla).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Plan de transicion para reemplazar llamadas legacy por API estable cuando este disponible.
- [ ] (P0 | En curso | Codex 2025-11-12) Asegurar que los adaptadores tratan endpoints legacy como black-box (sin reutilizar implementacion).
 - [ ] (P0 | En curso | Codex 2025-11-12) Priorizar adaptadores iniciales (top-10) segun 11.1 y definir JSON contrato v0.
 - [ ] (P1 | Pendiente | Codex 2025-11-12) Definir manejo de errores/codigos y mapeo a mensajes UI (toasts, banners, inline).
 - [ ] (P0 | En curso | Codex 2025-11-12) Definir paginacion/sorting/filtrado en adaptadores para DataGrid.

### 11.6 QA, UAT y rollout
- [ ] (P1 | Pendiente | Codex 2025-11-12) Criterios de aceptacion por pantalla (paridad funcional minima).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Checklist de accesibilidad AA y responsive.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Validacion de rendimiento (tiempos razonables de carga/interaccion).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Plan de despliegue incremental y feature flags.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Plan de documentacion usuario/soporte (cambios de UI).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Plan de UAT por modulo y pruebas negativas (ver Inventario: Anexo - Plan de UAT y pruebas).
- [ ] (P1 | Pendiente | Codex 2025-11-12) Plan de cutover/convivencia y rollback por modulo (ver Inventario: Anexo - Plan de cutover y convivencia).

## 12) Anexo A - Inventario resumido (detectado)
- Paginas/Carpetas relevantes: `Main.aspx`, `LoginWeb.aspx`, `LoginRedirect.aspx`, `VTLogin/`, `Auth/`, `Alerts/` (incluye `Alerts.aspx`, `AlertsDetail.aspx`, `TasksQueue.aspx`, descargas), `Cameras/` (incluye `Cameras.aspx`, `CameraSelectorData.aspx`, `srvMsgBoxCameras.aspx`, `Handlers/srvCameras.ashx`), `AIScheduler/` (paginas de unidades productivas, budgets, `Scripts` y `Styles`), `Wizards/` (NewTerminal, EmergencyReports), `Diagnostics/Status.aspx`, `Audit/`.
- Activos front: varios `.js` por modulo y CSS especificos (p.ej., `Alerts/css/alerts.css`, `AIScheduler/Styles/Budgets.css`).
- Configuracion: `Web.config` y transformaciones (`Web.Debug.config`, `Web.Release.config`, etc.).

Notas: Este inventario es preliminar y se ampliara con el relevamiento detallado (To Do 11.1-11.6).

---

Fin del documento (borrador v0.2).









