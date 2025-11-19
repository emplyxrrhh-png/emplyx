# Migracion a Emplyx 4.0 - Plan de Adaptacion de UI (Blazor + MAUI con Bootstrap)
Version: 0.2 (borrador)
Fecha: {{actualizar}}
Autor: Equipo Emplyx

<!-- Actualizacion 2025-11-19: Estado de checklist sincronizado con código real Emplyx.Blazor / Emplyx.ApiAdapter / Emplyx.Shared -->

## 11) Lista To Do
_Leyenda de prioridad: P0 = inmediato (iteracion actual), P1 = siguiente iteracion, P2 = backlog. Formato: (Prioridad | Estado | Responsable + fecha)._ 

Actualizado por Codex el 2025-11-12.
Actualizado por Copilot el 2025-11-19 (sincronizacion con repositorio).

### 11.1 Relevamiento e inventario (estado 2025-11-19)
- [x] (P0 | Completado parcial | Codex 2025-11-12) Inventario inicial de módulos y handlers (ver `docs/Inventario 11.1 Emplyx.md`).
- [ ] (P0 | En curso | Copilot 2025-11-19) Listar todas las páginas `.aspx` por módulo priorizado (Employees, Access, Scheduler, Security, Terminals, Tasks, Shifts) con propósito; solo listado parcial presente.
- [x] (P0 | Completado | Codex 2025-11-12) Identificar handlers `.ashx` principales (lista en inventario).
- [ ] (P0 | En curso | Copilot 2025-11-19) Identificar scripts JS y CSS por página (solo conteos agregados, falta detalle por página crítica).
- [ ] (P0 | En curso | Copilot 2025-11-19) Mapear llamadas AJAX (`$.ajax`, `__doPostBack`, `PageMethods`) con parámetros completos (parcial: Employees, Access, Scheduler; falta Security, Terminals, Shifts, Documents, Tasks).
- [ ] (P1 | Pendiente) Releviar roles/permisos actuales por ruta (basado en Web.config y patrones de uso) y normalizar a acciones.
- [ ] (P0 | Pendiente) Documentar flujos críticos faltantes: Alerts (detalle + download), Terminals (broadcast), Security (reset passport), Requests (aprobación), Shifts (copy/delete con dependencias).
- [ ] (P1 | Pendiente) Validaciones detalladas restantes (ej: Documents, Options, DataLink, Indicators, LabAgree).
- [ ] (P0 | En curso) Priorización refinada por volumen/impacto: Employees, Access, Scheduler => activos; siguiente: Security, Terminals, Requests, Alerts.
- [ ] (P0 | Pendiente) Actualizar inventario con fecha/autor (última actualización no refleja cambios del código Blazor reciente).

### 11.3 Infraestructura de proyectos (estado 2025-11-19)
- [x] (P0 | Completado) Estructura multi-proyecto creada: Blazor, Shared, ApiAdapter, Domain, Application, Infrastructure, WebApp.
- [x] (P0 | Completado) TFM net9.0 aplicado; MAUI aún sin carpeta configurada (placeholder plan).
- [x] (P0 | Completado parcial) Bootstrap 5 + Icons integrados; tokens básicos en `wwwroot/css/site.css` (pendiente ampliación y documentación de design tokens).
- [ ] (P1 | Pendiente) Variables CSS temáticas (color scale, spacing scale, radius, elevation) formalizadas y export a MAUI (Resources/Styles.xaml).
- [ ] (P1 | Pendiente) Convenciones formales de rutas y nombres (actual ad-hoc; requerir documento `docs/Convenciones_UI.md`).
- [x] (P0 | Completado) No se reutiliza código legacy (verificación manual – faltan pruebas automatizadas de ausencia de referencia). Añadir script de verificación.
- [ ] (P0 | En curso) Selección DataGrid externo (evaluación); riesgo de mantener custom sin virtual scroll/filtros avanzados.
- [ ] (P1 | Pendiente) Pipeline CI: build multi-proyectos + análisis (StyleCop/roslyn analyzers) + validación accesibilidad (Pa11y / axe CLI) + localización keys.

### 11.4 Diseño por módulo — Checklist extendido (2025-11-19)
Employees
- [x] Listado inicial /employees (DataGrid + filtros)
- [ ] Detalle empleado (/employees/{id}) tabs: perfil, contratos, permisos, movilidad
- [ ] CRUD básico (crear/editar nombre, idioma, forgottenRight)
- [ ] Acciones destructivas (borrar empleado, biometría) con ConfirmDialog + auditoría
- [ ] Summary dinámico (accrual|cause|tasks|centers) -> componente `EmployeeSummary`
- [ ] Grupos empleado (selector multi + gestión grupos CRUD)
- [ ] Validaciones completas (nombre requerido, idioma soportado, color destacado válido)

Access
- [x] Grupos (/access/groups) lista + filtros + acciones mock
- [x] Periodos (/access/periods) lista + filtros
- [x] Status (/access/status) monitor
- [x] Zonas (/access/zones)
- [x] Eventos (/access/events)
- [ ] Detalle grupo (tabs: zonas, periodos, empleados, auditoría)
- [ ] Copiar grupo (POST copy) con progreso en TasksQueue
- [ ] Vaciar empleados del grupo (acción auditada)

Scheduler
- [x] Unidades (/scheduler/productiveunits)
- [x] Budgets (/scheduler/budgets)
- [x] Moves (/scheduler/moves)
- [x] Coverage (/scheduler/coverage) + DateRangePicker
- [ ] Selector de unidad (AppSelector especializado con cascada)
- [ ] Editor presupuesto (form con validaciones numéricas)
- [ ] Export cobertura CSV (patrón descargas)

Alerts
- [ ] Listado alertas (/alerts) con severidad, estado, filtros
- [ ] Detalle alerta (/alerts/{id}) + histórico
- [ ] Cola de tareas (/alerts/tasksqueue) reutiliza AppTasksQueue
- [ ] Descarga adjuntos (patrón descargas + auditoría)
- [ ] Filtros avanzados (rango fecha, severidad múltiple)

Cameras
- [ ] Listado cámaras (/cameras) + estado on/off
- [ ] Selector modal (AppSelector variante con thumbnails)
- [ ] Mensajería cámaras (MessageBox replacement)
- [ ] Acciones: reiniciar, habilitar, deshabilitar (auditadas)

Security
- [ ] Opciones seguridad (/security/options)
- [ ] Supervisores (/security/supervisors) listado + reset passport
- [ ] Features/licencias (/security/features) gating visual
- [ ] Delegaciones temporales (listado + aprobar/revocar)

Terminals
- [ ] Listado terminals (/terminals)
- [ ] Broadcast acciones (reinicio, sincronización) -> TasksQueue
- [ ] Wizard nuevo terminal (pasos + validaciones)
- [ ] Descarga logs terminal

Tasks / TaskTemplates
- [ ] Centros negocio (/tasks/centers) listado
- [ ] Tasks (/tasks) con estados y progreso
- [ ] Templates (/tasktemplates) CRUD + clonación

Shifts
- [ ] Lista turnos (/shifts) + filtros (grupo, tipo)
- [ ] Wizard grupos turno
- [ ] Copiar turno + borrar auditado

Documents
- [ ] Templates (/documents/templates) lista + filtros
- [ ] Editor template (rich text / JSON spec)
- [ ] Descargar documento generado (patrón descargas)

Requests
- [ ] Lista solicitudes (/requests) estados (pendiente/aprobado/rechazado)
- [ ] Detalle solicitud con timeline
- [ ] Acciones aprobar/rechazar con motivo

Options
- [ ] Config general (/options) segregada por secciones
- [ ] Gestión campos usuario (UserFields) map a formulario dinámico

Indicators / Metrics
- [ ] Dashboard indicadores (/indicators) tarjetas + tendencias
- [ ] Selector rango temporal avanzado

Concepts / Causes
- [ ] Lista conceptos (/concepts)
- [ ] Lista causas (/causes)
- [ ] Tablas con filtros y DataGrid server mode

Notifications
- [ ] Lista notificaciones (/notifications) + marcar leído
- [ ] Suscripciones usuario

DataLink
- [ ] Jobs DataLink (/datalink/jobs) reutiliza TasksQueue
- [ ] Descargas resultados integraciones

LabAgree / DiningRoom / SDK / Absences / SecurityChart / Base
- [ ] Inventariar pantallas específicas y definir si se migran tal cual o consolidan en vistas genéricas

### 11.5 Adaptadores — Mapeo Handlers → Endpoints Propuestos (v0)
Ejemplos (formato handler ⇒ endpoint REST provisional):
- Employees/Handlers/srvEmployees.ashx ⇒ GET /employees, GET /employees/{id}, PATCH /employees/{id}, DELETE /employees/{id}, DELETE /employees/{id}/biometrics
- Employees/Handlers/srvGroups.ashx ⇒ GET /employee-groups, PATCH /employee-groups/{id}, DELETE /employee-groups/{id}
- Access/Handlers/srvAccessGroups.ashx ⇒ GET /access/groups, GET /access/groups/{id}, POST /access/groups/{id}:copy, POST /access/groups/{id}:empty
- Access/Handlers/srvAccessPeriods.ashx ⇒ GET /access/periods
- Access/Handlers/srvAccessZones.ashx ⇒ GET /access/zones
- Access/Handlers/srvAccessStatus.ashx ⇒ GET /access/status
- Access/Handlers/srvEventsScheduler.ashx ⇒ GET /access/events
- Scheduler/Handlers/srvScheduler.ashx ⇒ GET /scheduler/moves, GET /scheduler/coverage
- Scheduler/Handlers/srvMoves.ashx ⇒ GET /scheduler/moves/{id}
- AIScheduler/Handlers/srvProductiveUnit.ashx ⇒ GET /scheduler/productiveunits
- Cameras/Handlers/srvCameras.ashx ⇒ GET /cameras, PATCH /cameras/{id}:restart
- Security/Handlers/srvSupervisorsV3.ashx ⇒ GET /security/supervisors, POST /security/supervisors/{id}:reset-passport
- Security/Handlers/srvSecurityOptions.ashx ⇒ GET /security/options, PATCH /security/options/{key}
- Terminals/Handlers/srvTerminals.ashx ⇒ GET /terminals, POST /terminals/{id}:broadcast
- Terminals/Handlers/srvTerminalsList.ashx ⇒ GET /terminals/list
- Documents/Handlers/srvDocumentTemplates.ashx ⇒ GET /documents/templates, GET /documents/templates/{id}
- Requests/Handlers/srvRequests.ashx ⇒ GET /requests, POST /requests/{id}:approve, POST /requests/{id}:reject
- Shifts/Handlers/srvShifts.ashx ⇒ GET /shifts, POST /shifts/{id}:copy, DELETE /shifts/{id}

Pendientes:
- [ ] Definir esquemas de error comunes (code, message, traceId)
- [ ] Añadir soporte HEAD/OPTIONS para preflight UI si aplica
- [ ] Documentar versionado (/v1/) y estrategia futura cutover

### 11.6 QA / UAT / Rollout — Checklist Detallado
Accesibilidad
- [ ] Audit accesibilidad automática (axe) en páginas clave (Employees, Access, Scheduler)
- [ ] Focus management (ConfirmDialog, Toast aria-live, DataGrid keyboard nav)
- [ ] Color contrast tokens medidos (WCAG AA)

Rendimiento
- [ ] Medir TTI y TTFB en `/employees` y `/access/groups`
- [ ] Optimizar render DataGrid (>500 filas server-side paging)
- [ ] Evaluar prerender Blazor Server + caching lookups

Internacionalización
- [ ] Crear `Resources/Shared.es-ES.resx`, `Resources/Shared.en-US.resx`
- [ ] Extraer literales de componentes base (botones, estados)
- [ ] Middleware cultura (por TenantContext.Culture)

Observabilidad
- [ ] Servicio `UiTelemetry` + Application Insights (TrackEvent)
- [ ] CorrelationId por acción (GUID) propagado en headers adaptador
- [ ] Métrica latencia DataGrid (promedio, p95)

Seguridad / Permisos
- [ ] Mock `IPermissionService` con evaluaciones de acción
- [ ] Ocultar/Deshabilitar botones según permiso/rol/licencia
- [ ] Auditoría: registrar (view.opened, action.clicked, delete.confirmed)

Descargas / Exports
- [ ] Servicio `DownloadClient` (stream + filename) + manejo 403/404/410
- [ ] Componente `AppDownloadButton` estado: idle, downloading, error, success
- [ ] Auditoría eventos download.*

Cutover & Rollout
- [ ] Feature flags por módulo (appsettings Feature:Employees=true)
- [ ] Plan rollback (toggle flag, fallback legacy URL)
- [ ] Documentación interna de cutover por módulo

Testing
- [ ] Unit tests DataGrid (sorting/filtering local vs server)
- [ ] Unit tests DateRangePicker (clamps, minimum span)
- [ ] Unit tests ApiAdapterClient (error mapping)
- [ ] Component tests (bUnit) para AppSelector y ConfirmDialog

CI Integración
- [ ] Pipeline: build + test + publish artefact + report accesibilidad
- [ ] Analyzers (nullable, StyleCop) 
- [ ] Generación documentación (DocFX o similar) para contratos DTO

Métricas de Salida (Definition of Done UI Sprint)
- DataGrid páginas clave con telemetría
- Literales principales externalizados (≥30%)
- Primer módulo adicional (Security o Alerts) visible behind flag
- Adapter endpoints mapeados en doc (≥10/20)

---

<!-- Fin de actualizacion 2025-11-19 -->


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
  - Nota: estos adaptadores consumen endpoints existentes como cajas negras, sin reutilizar ni portar codigo legacy. Son temporales hasta contar con APIs definitivas.
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
- Eventos: view.opened, action.clicked, data.load.started/completed/failed, download.started/completado/failed, job.poll.tick.
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
Actualizado por Copilot el 2025-11-19 (sincronizacion con repositorio).

### 11.2 Checklist - Componentes base y patrones (Emplyx)
- [x] (P0 | Completado | Codex 2025-11-12) Confirmar estructura de soluciones: Emplyx.Blazor, Emplyx.Shared, Emplyx.ApiAdapter, Emplyx.Maui (sin directorio VisualTime para nuevos artefactos).
- [x] (P0 | Completado | Codex 2025-11-12) Verificar TFM: net9.0 (Blazor/Shared) y net9.0-* (MAUI por plataforma).
- [x] (P0 | Completado | Codex 2025-11-12) Integrar Bootstrap 5 + Bootstrap Icons; definir tokens iniciales (site.css) – pendiente refinar tema (colores/radius/shadows) (ver 11.3 estilado).
- [ ] (P0 | En curso | Copilot 2025-11-19) Seleccionar DataGrid externo (BlazorBootstrap / BootstrapBlazor / Blazorise). Estado actual: DataGrid personalizado `AppDataGrid` funcional (sorting, paging, filtros, server-side) – falta decidir libreria definitiva o mantener implementación propia.
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppLayout (Sidebar/Topbar/Breadcrumbs/Content) accesible y responsive (archivo `Shared/MainLayout.razor`).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppMessageBox / ConfirmDialog / Toast (servicio `AppUiMessagingService` + host `AppUiMessagingHost`).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppDataGrid (server-side) con onPage/onSort/onFilter/onRowAction y estados vacío/error/cargando.
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppSelector genérico (debounce, multi, etiquetas).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppForm/AppField y AppDateRangePicker (validaciones rango/min span/clamps).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppTasksQueue (cola reutilizable) + SampleTasksQueueService.
- [x] (P0 | Completado | Codex 2025-11-12) Definir Contexto multi-tenant (TenantContextState + headers en ApiAdapterClient + uso en MainLayout). TenantBar integrada en layout; falta selector interactivo (pendiente UX).
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Gating por licencia: infraestructura de `LicenciaContracts` presente en Shared pero sin lógica de gating en UI/NavMenu/acciones.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Patrón de descargas: no existe componente ni servicio (faltan: método cliente fetch->blob, auditoría y manejo códigos 403/404/410/5xx). Ningún `Download` implementado.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Observabilidad UI: no hay instrumentación (faltan eventos view.opened, data.load.*, action.clicked, job.poll.tick). Integrar Application Insights (ver documento `VisualTime/.../ApplicationInsights-Configuration.md` como referencia, sin portar código).
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Internacionalización: se añadió `AddLocalization()` pero no hay recursos .resx ni uso de `IStringLocalizer`. Pendiente scaffolding y reemplazo de literales.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Accesibilidad AA: componentes básicos existen, faltan revisiones de foco, roles aria, labels explícitas en DataGrid/Selector y uso de `aria-live` para toasts.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Remoción de dependencias legacy: aún existe carpeta `VisualTime/` en repo (solo referencia). Verificar que no se consuma JS/CSS legacy (actualmente no se usa). Formalizar verificación y checklist de no-reutilización.
- [x] (P0 | Completado | Codex 2025-11-12) API Adapter: envelope {ok,data,meta,error}; mapeo básico de errores implementado (`ApiAdapterClient`). Falta estandarizar códigos (ver Anexo errores v1.0) y retry/backoff.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Retry/backoff en ApiAdapter: no implementado; añadir política (HttpClient Polly) y mapping de códigos.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Auditoría de acciones UI: no implementada (delete/copy/etc. solo toasts simulados).
- [ ] (P2 | Backlog | Copilot 2025-11-19) CI/linting: no hay tareas para análisis accesibilidad, localización ni verificación de TFM multi-plataforma MAUI.

#### Bitacora 2025-11-19 (Copilot)
Verificación contra código real:
- Presentes: `AppDataGrid`, `AppSelector`, `AppForm`, `AppDateRangePicker`, `AppTasksQueue`, `AppUiMessagingService`, `TenantContextState`, `ApiAdapterClient` con headers multi-tenant.
- No presentes: gating licencia (solo DTOs), patrón descargas, telemetría eventos, recursos i18n, audit UI, retry/backoff, selector de tenant interactivo, controles dedicados de accesibilidad.
- DataGrid externo pendiente decisión (se mantiene implementación propia funcional).
- Se confirma no uso de scripts legacy en nuevas páginas; VisualTime se mantiene solo como referencia inventario.
- Próximo enfoque P0: instrumentación mínima (event bus + Application Insights), scaffolding i18n y patrón de descarga.

<!-- Fin de actualizacion 2025-11-19 -->

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
  - Nota: estos adaptadores consumen endpoints existentes como cajas negras, sin reutilizar ni portar codigo legacy. Son temporales hasta contar con APIs definitivas.
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
- Eventos: view.opened, action.clicked, data.load.started/completed/failed, download.started/completado/failed, job.poll.tick.
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
Actualizado por Copilot el 2025-11-19 (sincronizacion con repositorio).

### 11.2 Checklist - Componentes base y patrones (Emplyx)
- [x] (P0 | Completado | Codex 2025-11-12) Confirmar estructura de soluciones: Emplyx.Blazor, Emplyx.Shared, Emplyx.ApiAdapter, Emplyx.Maui (sin directorio VisualTime para nuevos artefactos).
- [x] (P0 | Completado | Codex 2025-11-12) Verificar TFM: net9.0 (Blazor/Shared) y net9.0-* (MAUI por plataforma).
- [x] (P0 | Completado | Codex 2025-11-12) Integrar Bootstrap 5 + Bootstrap Icons; definir tokens iniciales (site.css) – pendiente refinar tema (colores/radius/shadows) (ver 11.3 estilado).
- [ ] (P0 | En curso | Copilot 2025-11-19) Seleccionar DataGrid externo (BlazorBootstrap / BootstrapBlazor / Blazorise). Estado actual: DataGrid personalizado `AppDataGrid` funcional (sorting, paging, filtros, server-side) – falta decidir libreria definitiva o mantener implementación propia.
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppLayout (Sidebar/Topbar/Breadcrumbs/Content) accesible y responsive (archivo `Shared/MainLayout.razor`).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppMessageBox / ConfirmDialog / Toast (servicio `AppUiMessagingService` + host `AppUiMessagingHost`).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppDataGrid (server-side) con onPage/onSort/onFilter/onRowAction y estados vacío/error/cargando.
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppSelector genérico (debounce, multi, etiquetas).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppForm/AppField y AppDateRangePicker (validaciones rango/min span/clamps).
- [x] (P0 | Completado | Codex 2025-11-12) Implementar AppTasksQueue (cola reutilizable) + SampleTasksQueueService.
- [x] (P0 | Completado | Codex 2025-11-12) Definir Contexto multi-tenant (TenantContextState + headers en ApiAdapterClient + uso en MainLayout). TenantBar integrada en layout; falta selector interactivo (pendiente UX).
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Gating por licencia: infraestructura de `LicenciaContracts` presente en Shared pero sin lógica de gating en UI/NavMenu/acciones.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Patrón de descargas: no existe componente ni servicio (faltan: método cliente fetch->blob, auditoría y manejo códigos 403/404/410/5xx). Ningún `Download` implementado.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Observabilidad UI: no hay instrumentación (faltan eventos view.opened, data.load.*, action.clicked, job.poll.tick). Integrar Application Insights (ver documento `VisualTime/.../ApplicationInsights-Configuration.md` como referencia, sin portar código).
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Internacionalización: se añadió `AddLocalization()` pero no hay recursos .resx ni uso de `IStringLocalizer`. Pendiente scaffolding y reemplazo de literales.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Accesibilidad AA: componentes básicos existen, faltan revisiones de foco, roles aria, labels explícitas en DataGrid/Selector y uso de `aria-live` para toasts.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Remoción de dependencias legacy: aún existe carpeta `VisualTime/` en repo (solo referencia). Verificar que no se consuma JS/CSS legacy (actualmente no se usa). Formalizar verificación y checklist de no-reutilización.
- [x] (P0 | Completado | Codex 2025-11-12) API Adapter: envelope {ok,data,meta,error}; mapeo básico de errores implementado (`ApiAdapterClient`). Falta estandarizar códigos (ver Anexo errores v1.0) y retry/backoff.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Retry/backoff en ApiAdapter: no implementado; añadir política (HttpClient Polly) y mapping de códigos.
- [ ] (P1 | Pendiente | Copilot 2025-11-19) Auditoría de acciones UI: no implementada (delete/copy/etc. solo toasts simulados).
- [ ] (P2 | Backlog | Copilot 2025-11-19) CI/linting: no hay tareas para análisis accesibilidad, localización ni verificación de TFM multi-plataforma MAUI.

#### Bitacora 2025-11-19 (Copilot)
Verificación contra código real:
- Presentes: `AppDataGrid`, `AppSelector`, `AppForm`, `AppDateRangePicker`, `AppTasksQueue`, `AppUiMessagingService`, `TenantContextState`, `ApiAdapterClient` con headers multi-tenant.
- No presentes: gating licencia (solo DTOs), patrón descargas, telemetría eventos, recursos i18n, audit UI, retry/backoff, selector de tenant interactivo, controles dedicados de accesibilidad.
- DataGrid externo pendiente decisión (se mantiene implementación propia funcional).
- Se confirma no uso de scripts legacy en nuevas páginas; VisualTime se mantiene solo como referencia inventario.
- Próximo enfoque P0: instrumentación mínima (event bus + Application Insights), scaffolding i18n y patrón de descarga.

<!-- Fin de actualizacion 2025-11-19 -->


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
  - Nota: estos adaptadores consumen endpoints existentes como cajas negras, sin reutilizar ni portar codigo legacy. Son temporales hasta contar con APIs definitivas.
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
- Eventos: view.opened, action.clicked, data.load.started/completed/failed, download.started/completado/failed, job.poll.tick.
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
Actualizado por Copilot el 2025-11-19 (sincronizacion con repositorio).

### 11.4 Diseño por módulo — Checklist extendido (2025-11-19)
Employees
- [x] Listado inicial /employees (DataGrid + filtros)
- [ ] Detalle empleado (/employees/{id}) tabs: perfil, contratos, permisos, movilidad
- [ ] CRUD básico (crear/editar nombre, idioma, forgottenRight)
- [ ] Acciones destructivas (borrar empleado, biometría) con ConfirmDialog + auditoría
- [ ] Summary dinámico (accrual|cause|tasks|centers) -> componente `EmployeeSummary`
- [ ] Grupos empleado (selector multi + gestión grupos CRUD)
- [ ] Validaciones completas (nombre requerido, idioma soportado, color destacado válido)

Access
- [x] Grupos (/access/groups) lista + filtros + acciones mock
- [x] Periodos (/access/periods) lista + filtros
- [x] Status (/access/status) monitor
- [x] Zonas (/access/zones)
- [x] Eventos (/access/events)
- [ ] Detalle grupo (tabs: zonas, periodos, empleados, auditoría)
- [ ] Copiar grupo (POST copy) con progreso en TasksQueue
- [ ] Vaciar empleados del grupo (acción auditada)

Scheduler
- [x] Unidades (/scheduler/productiveunits)
- [x] Budgets (/scheduler/budgets)
- [x] Moves (/scheduler/moves)
- [x] Coverage (/scheduler/coverage) + DateRangePicker
- [ ] Selector de unidad (AppSelector especializado con cascada)
- [ ] Editor presupuesto (form con validaciones numéricas)
- [ ] Export cobertura CSV (patrón descargas)

Alerts
- [ ] Listado alertas (/alerts) con severidad, estado, filtros
- [ ] Detalle alerta (/alerts/{id}) + histórico
- [ ] Cola de tareas (/alerts/tasksqueue) reutiliza AppTasksQueue
- [ ] Descarga adjuntos (patrón descargas + auditoría)
- [ ] Filtros avanzados (rango fecha, severidad múltiple)

Cameras
- [ ] Listado cámaras (/cameras) + estado on/off
- [ ] Selector modal (AppSelector variante con thumbnails)
- [ ] Mensajería cámaras (MessageBox replacement)
- [ ] Acciones: reiniciar, habilitar, deshabilitar (auditadas)

Security
- [ ] Opciones seguridad (/security/options)
- [ ] Supervisores (/security/supervisors) listado + reset passport
- [ ] Features/licencias (/security/features) gating visual
- [ ] Delegaciones temporales (listado + aprobar/revocar)

Terminals
- [ ] Listado terminals (/terminals)
- [ ] Broadcast acciones (reinicio, sincronización) -> TasksQueue
- [ ] Wizard nuevo terminal (pasos + validaciones)
- [ ] Descarga logs terminal

Tasks / TaskTemplates
- [ ] Centros negocio (/tasks/centers) listado
- [ ] Tasks (/tasks) con estados y progreso
- [ ] Templates (/tasktemplates) CRUD + clonación

Shifts
- [ ] Lista turnos (/shifts) + filtros (grupo, tipo)
- [ ] Wizard grupos turno
- [ ] Copiar turno + borrar auditado

Documents
- [ ] Templates (/documents/templates) lista + filtros
- [ ] Editor template (rich text / JSON spec)
- [ ] Descargar documento generado (patrón descargas)

Requests
- [ ] Lista solicitudes (/requests) estados (pendiente/aprobado/rechazado)
- [ ] Detalle solicitud con timeline
- [ ] Acciones aprobar/rechazar con motivo

Options
- [ ] Config general (/options) segregada por secciones
- [ ] Gestión campos usuario (UserFields) map a formulario dinámico

Indicators / Metrics
- [ ] Dashboard indicadores (/indicators) tarjetas + tendencias
- [ ] Selector rango temporal avanzado

Concepts / Causes
- [ ] Lista conceptos (/concepts)
- [ ] Lista causas (/causes)
- [ ] Tablas con filtros y DataGrid server mode

Notifications
- [ ] Lista notificaciones (/notifications) + marcar leído
- [ ] Suscripciones usuario

DataLink
- [ ] Jobs DataLink (/datalink/jobs) reutiliza TasksQueue
- [ ] Descargas resultados integraciones

LabAgree / DiningRoom / SDK / Absences / SecurityChart / Base
- [ ] Inventariar pantallas específicas y definir si se migran tal cual o consolidan en vistas genéricas

### 11.5 Adaptadores — Mapeo Handlers → Endpoints Propuestos (v0)
Ejemplos (formato handler ⇒ endpoint REST provisional):
- Employees/Handlers/srvEmployees.ashx ⇒ GET /employees, GET /employees/{id}, PATCH /employees/{id}, DELETE /employees/{id}, DELETE /employees/{id}/biometrics
- Employees/Handlers/srvGroups.ashx ⇒ GET /employee-groups, PATCH /employee-groups/{id}, DELETE /employee-groups/{id}
- Access/Handlers/srvAccessGroups.ashx ⇒ GET /access/groups, GET /access/groups/{id}, POST /access/groups/{id}:copy, POST /access/groups/{id}:empty
- Access/Handlers/srvAccessPeriods.ashx ⇒ GET /access/periods
- Access/Handlers/srvAccessZones.ashx ⇒ GET /access/zones
- Access/Handlers/srvAccessStatus.ashx ⇒ GET /access/status
- Access/Handlers/srvEventsScheduler.ashx ⇒ GET /access/events
- Scheduler/Handlers/srvScheduler.ashx ⇒ GET /scheduler/moves, GET /scheduler/coverage
- Scheduler/Handlers/srvMoves.ashx ⇒ GET /scheduler/moves/{id}
- AIScheduler/Handlers/srvProductiveUnit.ashx ⇒ GET /scheduler/productiveunits
- Cameras/Handlers/srvCameras.ashx ⇒ GET /cameras, PATCH /cameras/{id}:restart
- Security/Handlers/srvSupervisoresV3.ashx ⇒ GET /security/supervisors, POST /security/supervisors/{id}:reset-passport
- Security/Handlers/srvSecurityOptions.ashx ⇒ GET /security/options, PATCH /security/options/{key}
- Terminals/Handlers/srvTerminals.ashx ⇒ GET /terminals, POST /terminals/{id}:broadcast
- Terminals/Handlers/srvTerminalsList.ashx ⇒ GET /terminals/list
- Documents/Handlers/srvDocumentTemplates.ashx ⇒ GET /documents/templates, GET /documents/templates/{id}
- Requests/Handlers/srvRequests.ashx ⇒ GET /requests, POST /requests/{id}:approve, POST /requests/{id}:reject
- Shifts/Handlers/srvShifts.ashx ⇒ GET /shifts, POST /shifts/{id}:copy, DELETE /shifts/{id}

Pendientes:
- [ ] Definir esquemas de error comunes (code, message, traceId)
- [ ] Añadir soporte HEAD/OPTIONS para preflight UI si aplica
- [ ] Documentar versionado (/v1/) y estrategia futura cutover

### 11.6 QA / UAT / Rollout — Checklist Detallado
Accesibilidad
- [ ] Audit accesibilidad automática (axe) en páginas clave (Employees, Access, Scheduler)
- [ ] Focus management (ConfirmDialog, Toast aria-live, DataGrid keyboard nav)
- [ ] Color contrast tokens medidos (WCAG AA)

Rendimiento
- [ ] Medir TTI y TTFB en `/employees` y `/access/groups`
- [ ] Optimizar render DataGrid (>500 filas server-side paging)
- [ ] Evaluar prerender Blazor Server + caching lookups

Internacionalización
- [ ] Crear `Resources/Shared.es-ES.resx`, `Resources/Shared.en-US.resx`
- [ ] Extraer literales de componentes base (botones, estados)
- [ ] Middleware cultura (por TenantContext.Culture)

Observabilidad
- [ ] Servicio `UiTelemetry` + Application Insights (TrackEvent)
- [ ] CorrelationId por acción (GUID) propagado en headers adaptador
- [ ] Métrica latencia DataGrid (promedio, p95)

Seguridad / Permisos
- [ ] Mock `IPermissionService` con evaluaciones de acción
- [ ] Ocultar/Deshabilitar botones según permiso/rol/licencia
- [ ] Auditoría: registrar (view.opened, action.clicked, delete.confirmed)

Descargas / Exports
- [ ] Servicio `DownloadClient` (stream + filename) + manejo 403/404/410
- [ ] Componente `AppDownloadButton` estado: idle, downloading, error, success
- [ ] Auditoría eventos download.*

Cutover & Rollout
- [ ] Feature flags por módulo (appsettings Feature:Employees=true)
- [ ] Plan rollback (toggle flag, fallback legacy URL)
- [ ] Documentación interna de cutover por módulo

Testing
- [ ] Unit tests DataGrid (sorting/filtering local vs server)
- [ ] Unit tests DateRangePicker (clamps, minimum span)
- [ ] Unit tests ApiAdapterClient (error mapping)
- [ ] Component tests (bUnit) para AppSelector y ConfirmDialog

CI Integración
- [ ] Pipeline: build + test + publish artefact + report accesibilidad
- [ ] Analyzers (nullable, StyleCop) 
- [ ] Generación documentación (DocFX o similar) para contratos DTO

Métricas de Salida (Definition of Done UI Sprint)
- DataGrid páginas clave con telemetría
- Literales principales externalizados (≥30%)
- Primer módulo adicional (Security o Alerts) visible behind flag
- Adapter endpoints mapeados en doc (≥10/20)

--- Fin de checklist extendido.

















