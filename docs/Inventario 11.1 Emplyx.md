# Inventario 11.1 — Emplyx (UI legado VTLive40 como referencia)

Fecha: {{actualizar}}
Alcance: Solo UI. Sin reutilizar código; lógica como referencia.

Origen analizado: `VisualTime/Src/VTLive40`

## Priorizacion inicial (Codex 2025-11-12)

- [ ] (P0 | En curso | Codex 2025-11-12) Employees y Access: cerrar parámetros por acción, validaciones guiadas y contratos JSON v0.1 (secciones "Detalle - Employees" y "Detalle - Access").
- [ ] (P0 | En curso | Codex 2025-11-12) Scheduler y Security: completar flujos end-to-end, handlers y reglas de permisos (secciones correspondientes).
- [ ] (P0 | En curso | Codex 2025-11-12) Base/Common: mapear controles compartidos (roCalendar, TasksQueue, MessageBox) para alimentar componentes Blazor.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Terminals y Tasks/TaskTemplates: afinar parámetros de broadcast/wizards y dependencias de scripts.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Shifts y Documents: documentar restricciones de copia/borrado y descargas auditadas.
- [ ] (P1 | Pendiente | Codex 2025-11-12) Requests/Notifications/Indicators: completar estados, transiciones y requisitos de auditoría.

## Resumen por módulo (cuentas)

- (root) — ASPX: 9, ASHX: 0, JS: 0, CSS: 0
- Absences — ASPX: 1, ASHX: 0, JS: 1, CSS: 0
- Access — ASPX: 28, ASHX: 5, JS: 19, CSS: 0
- AIScheduler — ASPX: 4, ASHX: 1, JS: 4, CSS: 1
- Alerts — ASPX: 4, ASHX: 0, JS: 2, CSS: 1
- Assignments — ASPX: 3, ASHX: 1, JS: 1, CSS: 0
- Audit — ASPX: 1, ASHX: 0, JS: 1, CSS: 0
- Auth — ASPX: 3, ASHX: 0, JS: 0, CSS: 0
- Base — ASPX: 32, ASHX: 3, JS: 235, CSS: 197
- Cameras — ASPX: 3, ASHX: 1, JS: 1, CSS: 0
- Causes — ASPX: 3, ASHX: 1, JS: 1, CSS: 0
- Common — ASPX: 1, ASHX: 0, JS: 0, CSS: 0
- Concepts — ASPX: 4, ASHX: 2, JS: 3, CSS: 0
- Content — ASPX: 0, ASHX: 0, JS: 0, CSS: 44
- DataLink — ASPX: 9, ASHX: 1, JS: 1, CSS: 0
- DevExpressScript — ASPX: 0, ASHX: 0, JS: 1, CSS: 0
- Diagnostics — ASPX: 2, ASHX: 0, JS: 1, CSS: 0
- DiningRoom — ASPX: 3, ASHX: 1, JS: 1, CSS: 0
- Documents — ASPX: 4, ASHX: 1, JS: 6, CSS: 1
- Emergency — ASPX: 3, ASHX: 0, JS: 1, CSS: 0
- Employees — ASPX: 44, ASHX: 2, JS: 10, CSS: 0
- Indicators — ASPX: 3, ASHX: 1, JS: 1, CSS: 0
- LabAgree — ASPX: 3, ASHX: 1, JS: 7, CSS: 0
- Notifications — ASPX: 3, ASHX: 1, JS: 1, CSS: 0
- Options — ASPX: 8, ASHX: 0, JS: 1, CSS: 0
- ReportScheduler — ASPX: 1, ASHX: 0, JS: 1, CSS: 0
- Requests — ASPX: 4, ASHX: 1, JS: 2, CSS: 0
- Scheduler — ASPX: 23, ASHX: 2, JS: 9, CSS: 3
- Scripts — ASPX: 0, ASHX: 0, JS: 83, CSS: 0
- SDK — ASPX: 2, ASHX: 1, JS: 1, CSS: 0
- Security — ASPX: 17, ASHX: 2, JS: 10, CSS: 0
- SecurityChart — ASPX: 4, ASHX: 1, JS: 1, CSS: 0
- Shifts — ASPX: 8, ASHX: 3, JS: 14, CSS: 2
- Tasks — ASPX: 14, ASHX: 1, JS: 7, CSS: 0
- TaskTemplates — ASPX: 5, ASHX: 2, JS: 4, CSS: 0
- Terminals — ASPX: 14, ASHX: 2, JS: 6, CSS: 0
- VTCheckSSO — ASPX: 1, ASHX: 0, JS: 0, CSS: 0
- VTLogin — ASPX: 2, ASHX: 0, JS: 0, CSS: 0
- Wizards — ASPX: 2, ASHX: 0, JS: 0, CSS: 0

Nota: Módulos no listados arriba presentan cuentas 0 o son carpetas técnicas.

## Páginas en raíz

- Default.aspx, LoginRedirect.aspx, LoginWeb.aspx, Main.aspx, Recover.aspx, srvMsgBoxUserTasks.aspx, Start.aspx, TestConnection.aspx, UserTasksCheck.aspx

## Detalle por módulos clave (UI)

### VTLogin
- ASPX: VTLogin/Default.aspx, VTLogin/VTPortalLogin.aspx
- JS: —
- CSS: —

### Auth
- ASPX: Auth/VTCheckAuth.aspx, Auth/VTLiveAuth.aspx, Auth/VTPortalAuth.aspx
- JS: —
- CSS: —

### Alerts
- ASPX: Alerts/Alerts.aspx, Alerts/AlertsDetail.aspx, Alerts/downloadFile.aspx, Alerts/TasksQueue.aspx
- JS: Alerts/Scripts/Alerts.js, Alerts/Scripts/TasksQueue.js
- CSS: Alerts/css/alerts.css

### Cameras
- ASPX: Cameras/Cameras.aspx, Cameras/CameraSelectorData.aspx, Cameras/srvMsgBoxCameras.aspx
- JS: Cameras/Scripts/Camera.js
- CSS: —

### AIScheduler
- ASPX: AIScheduler/Budget.aspx, AIScheduler/ProductiveUnit.aspx, AIScheduler/ProductiveUnitSelectorData.aspx, AIScheduler/srvMsgBoxAIScheduler.aspx
- JS: AIScheduler/Scripts/AIScheduler.js, AIScheduler/Scripts/Budgets.js, AIScheduler/Scripts/DateSelector.js, AIScheduler/Scripts/ProductiveUnit.js
- CSS: AIScheduler/Styles/Budgets.css

### Wizards
- ASPX: Wizards/EmergencyReports.aspx, Wizards/NewTerminalWizard.aspx
- JS: —
- CSS: —

### Diagnostics
- ASPX: Diagnostics/DownloadLogs.aspx, Diagnostics/Status.aspx
- JS: Diagnostics/Scripts/diagnostics.js
- CSS: —

### Audit
- ASPX: Audit/Audit.aspx
- JS: Audit/Scripts/audit.js
- CSS: —

## Handlers (.ashx) detectados

- Access/Handlers/srvAccessGroups.ashx
- Access/Handlers/srvAccessPeriods.ashx
- Access/Handlers/srvAccessStatus.ashx
- Access/Handlers/srvAccessZones.ashx
- Access/Handlers/srvEventsScheduler.ashx
- AIScheduler/Handlers/srvProductiveUnit.ashx
- Assignments/Handlers/srvAssignments.ashx
- Base/Handlers/srvMain.ashx
- Base/WebUserControls/handlers/srvUserFields.ashx
- Base/WebUserControls/roCalendar/Handlers/srvCalendarImport.ashx
- Cameras/Handlers/srvCameras.ashx
- Causes/Handlers/srvCauses.ashx
- Concepts/Handlers/srvConceptGroups.ashx
- Concepts/Handlers/srvConcepts.ashx
- DataLink/Handlers/srvDatalinkBusiness.ashx
- DiningRoom/Handlers/srvDiningRoom.ashx
- Documents/Handlers/srvDocumentTemplates.ashx
- Employees/Handlers/srvEmployees.ashx
- Employees/Handlers/srvGroups.ashx
- Indicators/Handlers/srvIndicators.ashx
- LabAgree/Handlers/srvLabAgree.ashx
- Notifications/Handlers/srvNotifications.ashx
- Requests/Handlers/srvRequests.ashx
- Scheduler/Handlers/srvMoves.ashx
- Scheduler/Handlers/srvScheduler.ashx
- SDK/Handlers/srvManagePunches.ashx
- Security/Handlers/srvSecurityOptions.ashx
- Security/Handlers/srvSupervisorsV3.ashx
- SecurityChart/Handlers/srvSecurityFunctions.ashx
- Shifts/Handlers/srvShifts.ashx
- Shifts/Handlers/srvShiftsGroups.ashx
- Shifts/Handlers/srvShiftsV2.ashx
- Tasks/Handlers/srvBusinessCenters.ashx
- TaskTemplates/Handlers/srvProjects.ashx
- TaskTemplates/Handlers/srvTaskTemplates.ashx
- Terminals/Handlers/srvTerminals.ashx
- Terminals/Handlers/srvTerminalsList.ashx

## Autorización (Web.config)

- Se detectó `<authorization>` en `Web.config` (raíz de VTLive40). No se hallaron reglas específicas adicionales por subcarpeta.

## Flujos críticos (borrador)

- Autenticación/Inicio:
  - VTLogin/VTPortalLogin.aspx → Auth/VTPortalAuth.aspx → LoginRedirect.aspx/Main.aspx
  - Alternativa: LoginWeb.aspx (login directo)
- Home: Main.aspx (navegación principal)
- Alertas: Alerts.aspx → AlertsDetail.aspx; cola de tareas (TasksQueue.aspx); descarga (downloadFile.aspx)
- Cámaras: Cameras.aspx ↔ CameraSelectorData.aspx; mensajes (srvMsgBoxCameras.aspx)
- AI Scheduler: ProductiveUnit.aspx, Budget.aspx; selector de unidad/fecha
- Wizards: NewTerminalWizard.aspx, EmergencyReports.aspx
- Diagnóstico: Diagnostics/Status.aspx, DownloadLogs.aspx

## Observaciones

- El módulo `Base` concentra muchos JS/CSS compartidos.
- Múltiples handlers `.ashx` por dominio funcional; para la UI nueva, gestionar vía `Emplyx.ApiAdapter` como black-box.
- No se reutiliza código; este inventario guía reescritura UI (Blazor/MAUI .NET 9) preservando reglas.

## Próximos pasos (11.1)

- Completar listado detallado de ASPX por módulos con alta cuenta (Employees, Scheduler, Security, Tasks, Terminals, Access, Base).
- Extraer dependencias JS/CSS referenciadas en cada ASPX (script/link) para planificar componentes.
- Mapear llamadas AJAX/métodos server-side usados por JS (buscar `$.ajax`, `PageMethods`, `WebMethod`).
- Anotar reglas/validaciones visibles por pantalla (mensajes, required, rangos) como requisitos de UI.
- Confirmar perfiles/roles por rutas según `Web.config` y principios de `Emplyx_Guia.md`.

---

## Detalle — Employees (v0)

Páginas ASPX principales:

- Employees/Employees.aspx (listado/detalle), Wizards (varios), edición programada (ausencias, incidencias, vacaciones, horas extra), contratos, grupos, permisos, mensajes, movilidad, etc.

Scripts detectados:

- Employees/Scripts/Employees_v2.js, EmployeeSummary.js, EmployeeContracts.js, EmployeeGroups.js, Groups_v2.js, GroupsLite.js, LockDate.js, PrintAbsences.js, ContractScheduleRules.js, QueryScheduleRules.js

Patrones AJAX/WebForms:

- __doPostBack en varias páginas de edición.
- Llamadas AJAX a handlers:
  - Handlers/srvEmployees.ashx acciones: getBarButtons, getEmployeeTab, chgName, chgForgottenRight, deleteEmp, DeleteBiometricDataByEmployee, deleteProgAus, deleteProgInc, deleteProgHolidays, deleteProgOvertime, AuditUserFieldQuery, DrawEmployeeSummary (Type: accrual|cause|tasks|centers).
  - Handlers/srvGroups.ashx acciones: getBarButtons, getGroupsTab, getBarButtonsLite, getGroupsTabLite, chgNameGroup, deleteGroup.
  - Seguridad: ../Security/Handlers/srvSupervisorsV3.ashx acciones: resetPassport, SendUsername, restoreCegidID.

Handlers relevantes:

- Employees/Handlers/srvEmployees.ashx
- Employees/Handlers/srvGroups.ashx
- Security/Handlers/srvSupervisorsV3.ashx (dependencia cruzada)

Propuesta de contratos JSON v0 (Emplyx.ApiAdapter):

- GET /employees?page,size,filter,sort -> { items:[{id,code,name,status,groups:[id,name],hasForgottenRight,language}], total }
- GET /employees/{id} -> { id, code, name, status, contacts, centers, contracts:[...], permissions:[...], mobility:{...} }
- PATCH /employees/{id} { name?, language?, hasForgottenRight? } -> { ok, message }
- GET /employees/{id}/summary?type=accrual|cause|tasks|centers&range=... -> { series:[...], meta:{...} }
- DELETE /employees/{id}/biometrics -> { ok }
- DELETE /employees/{id}/programmed/{kind:absence|incidence|holiday|overtime}/{key} -> { ok }
- Groups:
  - GET /groups/{id}/tabs/{tab} -> { html|model }
  - PATCH /groups/{id} { name } -> { ok }
  - DELETE /groups/{id} -> { ok }

Notas de permisos/licencia:

- Acciones de borrar datos biométricos y movilidad requieren rol elevado; auditar acción y motivo.
- Gating por licencia para funciones avanzadas (summary avanzado, grupos avanzados) si aplica.

Pendientes:

- [ ] Extraer parámetros exactos por cada acción desde JS para afinar contratos.
- [ ] Mapear validaciones en formularios (required/rangos) visibles por pantalla.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/employees` en Emplyx.Blazor/Pages/Employees/Index.razor cubre listado + filtros + selector + tareas usando `EmployeesMockDataSource` como adaptador temporal.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/access/groups` en Emplyx.Blazor/Pages/Access/Groups.razor cubre listado, filtros status/zone/period/pending y acciones copy/delete/empty usando `AccessGroupsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/access/periods` en Emplyx.Blazor/Pages/Access/Periods.razor implementa filtros status/weekDay/month/special y acciones delete mock usando `AccessPeriodsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/access/status` en Emplyx.Blazor/Pages/Access/Status.razor cubre monitor (filtros group/zone/severity/attention) y acciones detail/block contra `AccessStatusMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/access/zones` en Emplyx.Blazor/Pages/Access/Zones.razor cubre listado y filtros type/parent/critical con acciones view/delete usando `AccessZonesMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/access/events` en Emplyx.Blazor/Pages/Access/Events.razor cubre EventScheduler (filtros zone/group/status) y acciones copy/delete usando `AccessEventsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/scheduler/units` en Emplyx.Blazor/Pages/Scheduler/ProductiveUnits.razor cubre AIScheduler/ProductiveUnit.aspx (filtros plant/status/manager) usando `ProductiveUnitsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/scheduler/budgets` en Emplyx.Blazor/Pages/Scheduler/Budgets.razor cubre AIScheduler/Budget.aspx (filtros unit/period/status) usando `BudgetsMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/scheduler/moves` en Emplyx.Blazor/Pages/Scheduler/Moves.razor cubre Scheduler/MovesDetail.aspx (filtros employee/terminal/type/status) usando `MovesMockDataSource`.
- [x] (P0 | En curso | Codex 2025-11-12) Prototipo `/scheduler/coverage` en Emplyx.Blazor/Pages/Scheduler/Coverage.razor replica CalendarV2 (rango fechas + status) usando `CoverageSummaryMockDataSource`.

Parametros detectados por accion (desde JS):

- srvEmployees.ashx
  - action=AuditUserFieldQuery | params: FieldName, ID
  - action=chgForgottenRight | params: HasForgottenRight, ID
  - action=chgName | params: ID, IsProductiv, NewLang, NewName
  - action=DeleteBiometricDataByEmployee | params: ID
  - action=deleteEmp | params: ID
  - action=deleteProgAus | params: BeginDate, ID, IDCause
  - action=deleteProgHolidays | params: IDProgHoliday
  - action=deleteProgInc | params: BeginDate, ID, IDAbsence, IDCause
  - action=deleteProgOvertime | params: IDProgOvertime
  - action=DrawEmployeeSummary | params: ID, Range, Type
  - action=employeeHasData | params: ID
  - action=getBarButtons | params: ID
  - action=getEmployeeTab | params: aTab, ID
  - action=updateHighlightColor | params: —

- srvGroups.ashx
  - action=chgNameGroup | params: ID, NewName
  - action=deleteGroup | params: ID
  - action=getBarButtons | params: ID
  - action=getBarButtonsLite | params: ID
  - action=getGroupsTab | params: aTab, ID
  - action=getGroupsTabLite | params: aTab, ID

- srvSupervisorsV3.ashx (Security)
  - acciones detectadas en UI: resetPassport, SendUsername, restoreCegidID (parametros a confirmar)

Validaciones observadas (parcial):

- ContractScheduleRules (teletrabajo):
  - Rango de fechas valido: `From <= To`.
  - Maximos por periodo: si es por dias y periodo es mes: <= 31; si trimestre: <= 93.
  - Dias obligatorios/opcionales deben pertenecer al set permitido.
  - Validaciones numericas: `!isNaN(...)`, rangos y conteos de dias laborables.
- General (nombres/idioma): Nombre no vacio para cambios de nombre; idioma valido del catalogo; highlight color requiere valor valido.
-
Validaciones detalladas (afinadas):

- Seleccion requerida de grupo para operaciones en Employees/Groups (Error.MustSelectAGroup).
- Estados de grilla: no permitir operaciones mientras hay edicion pendiente (Error.ValidationGridIsEditingError).
- Campos basicos obligatorios al crear/editar empleado (Error.ValidationFieldsFailed): nombre, idioma valido, etc.
- Acciones destructivas (borrar empleado/biometria/progamaciones) requieren confirmacion UI -> registrar motivo si aplica.

Contratos v0.1 — ejemplos:

- GET /employees?page=1&size=20&filter=smith&sort=name:asc
  Respuesta: { "items": [{ "id": 123, "code": "E00123", "name": "Alice Smith", "status": "active", "groups": [{"id":10,"name":"HR"}], "hasForgottenRight": false, "language": "es" }], "total": 1543 }

- GET /employees/{id}
  Respuesta: { "id": 123, "code": "E00123", "name": "Alice Smith", "status": "active", "contacts": {"email":"a@c.com"}, "centers": ["MAD-01"], "contracts": [...], "permissions": [...], "mobility": {...} }

- PATCH /employees/{id}
  Cuerpo: { "name": "Alice S.", "language": "es", "hasForgottenRight": true }
  Respuesta: { "ok": true, "message": "updated" }

- GET /employees/{id}/summary?type=accrual&range=2025Q1
  Respuesta: { "series": [{"label":"Horas","data":[...]}], "meta": {"type":"accrual","range":"2025Q1"} }

- DELETE /employees/{id}/biometrics -> { "ok": true }
- DELETE /employees/{id}/programmed/absence/{key} -> { "ok": true }
- GET /groups/{id}/tabs/{tab} -> { "model": {...} }
- PATCH /groups/{id} {"name":"Produccion"} -> { "ok": true }

---

## Detalle — Access (v0)

Páginas ASPX principales:

- Access/AccessGroups.aspx, AccessPeriods.aspx, AccessStatus.aspx/Monitor/SelectorData, AccessZones.aspx/ZoomZone, Eventos y filtros (Plates/Punches), Wizards varios.

Scripts detectados:

- Access/Scripts/AccessGroup.js, AccessPeriod.js, AccessStatus.js/AccessStatu.js, AccessStatusMonitor.js, AccessZone.js, EventsV2.js, filtros/maps.

Patrones AJAX/WebForms:

- Llamadas a:
  - Handlers/srvAccessGroups.ashx acciones: getAccessGroupTab, getBarButtons, canSaveAccessGroups, deleteAccessGroup, copyXAccessGroup, emptyAccessGroupEmp.
  - Handlers/srvAccessPeriods.ashx acciones: getAccessPeriodTab, getBarButtons, deleteAccessPeriod, getAccessPeriodDescription.
  - Handlers/srvAccessStatus.ashx acciones: getAccessStatusTab, getBarButtons, getAccessStatusMainTab, getBarButtonsMain.
  - Handlers/srvAccessZones.ashx acciones: getAccessZoneTab, getBarButtons, getAccessZoneMainTab, getBarButtonsMain, deleteAccessZone.
  - Handlers/srvEventsScheduler.ashx acciones: getBarButtons, copyXEvent, deleteXEventScheduler y POST genérico.
- __doPostBack en algunos wizards.

Handlers relevantes:

- Access/Handlers/srvAccessGroups.ashx, srvAccessPeriods.ashx, srvAccessStatus.ashx, srvAccessZones.ashx, srvEventsScheduler.ashx

Propuesta de contratos JSON v0 (Emplyx.ApiAdapter):

- Access Groups:
  - GET /access/groups/{id}/tabs/{tab} -> { model }
  - GET /access/groups/{id}/actions -> { buttons:[...] }
  - POST /access/groups/{id}:delete -> { ok }
  - POST /access/groups/{id}:copy -> { ok, newId }
  - POST /access/groups/{id}:emptyEmployees -> { ok }
- Access Periods:
  - GET /access/periods/{id}/tabs/{tab} -> { model }
  - GET /access/periods/{id}/actions -> { buttons:[...] }
  - POST /access/periods/{id}:delete -> { ok }
  - POST /access/periods/{id}/description -> { text }
- Access Status/Monitor:
  - GET /access/status/{id}/tabs/{tab} -> { model }
  - GET /access/status/{id}/actions -> { buttons:[...] }
  - GET /access/status/main/{id}/tabs/{tab} -> { model }
- Access Zones:
  - GET /access/zones/{id}/tabs/{tab} -> { model }
  - GET /access/zones/{id}/actions -> { buttons:[...] }
  - POST /access/zones/{id}:delete -> { ok }
- Event Scheduler:
  - GET /access/events/{id}/actions -> { buttons:[...] }
  - POST /access/events/{id}:copy { newDate } -> { ok, newId }
  - POST /access/events/{id}:delete -> { ok }

Notas de permisos/licencia:

- Operaciones de borrar/copiar requieren roles y auditoría. Monitor en tiempo real puede requerir licencia avanzada.

Pendientes:

- [ ] Confirmar parámetros esperados (IDs, tabs) y normalizarlos para UI.
- [ ] Reglas de validación en altas/ediciones (nombres únicos, rangos horarios, zonas).

Parametros detectados por accion (desde JS):

- srvAccessGroups.ashx
  - action=canSaveAccessGroups | params: ID
  - action=copyXAccessGroup | params: ID
  - action=deleteAccessGroup | params: ID
  - action=emptyAccessGroupEmp | params: ID
  - action=getAccessGroupTab | params: aTab, ID
  - action=getBarButtons | params: ID

- srvAccessPeriods.ashx
  - action=deleteAccessPeriod | params: ID
  - action=getAccessPeriodDescription | params: —
  - action=getAccessPeriodTab | params: aTab, ID
  - action=getBarButtons | params: ID

- srvAccessStatus.ashx
  - action=getAccessStatusMainTab | params: ID
  - action=getAccessStatusTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=getBarButtonsMain | params: ID

- srvAccessZones.ashx
  - action=deleteAccessZone | params: ID
  - action=getAccessZoneMainTab | params: ID
  - action=getAccessZoneTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=getBarButtonsMain | params: ID

- srvEventsScheduler.ashx
  - action=copyXEvent | params: ID, NewDate
  - action=deleteXEventScheduler | params: ID
  - action=getBarButtons | params: ID

Validaciones observadas (parcial):

- AccessGroups/Periods/Zones:
  - Nombre requerido y unico por ambito.
  - Antes de borrar/copiar: confirmacion UI y chequeo de dependencias (arbol/relaciones).
  - Descripciones y parametros de periodos con rangos validos; horarias en formato esperado.
- AccessStatus/Monitor:
  - Filtros de estado obligatorios para vistas; polling/actualizacion segura.
- EventsScheduler:
  - Copia requiere `NewDate` valida (>= hoy opcional); operaciones idempotentes por seguridad.

Contratos v0.1 — ejemplos:

- GET /access/groups/{id}/tabs/{tab} -> { "model": {...} }
- GET /access/groups/{id}/actions -> { "buttons":["save","delete","copy"] }
- POST /access/groups/{id}:delete -> { "ok": true }
- POST /access/groups/{id}:copy { "newName": "Group Copy" } -> { "ok": true, "newId": 987 }
- POST /access/groups/{id}:emptyEmployees -> { "ok": true }

- GET /access/periods/{id}/tabs/{tab} -> { "model": {...} }
- GET /access/periods/{id}/actions -> { "buttons":["save","delete"] }
- POST /access/periods/{id}:delete -> { "ok": true }
- POST /access/periods/{id}/description -> { "text": "Horario especial verano" }

- GET /access/status/{id}/tabs/{tab} -> { "model": {...} }
- GET /access/status/{id}/actions -> { "buttons":[...]} 
- GET /access/status/main/{id}/tabs/{tab} -> { "model": {...} }

- GET /access/zones/{id}/tabs/{tab} -> { "model": {...} }
- GET /access/zones/{id}/actions -> { "buttons":["save","delete"] }
- POST /access/zones/{id}:delete -> { "ok": true }

- GET /access/events/{id}/actions -> { "buttons":["copy","delete"] }
- POST /access/events/{id}:copy { "newDate": "2025-02-15" } -> { "ok": true, "newId": 654 }
- POST /access/events/{id}:delete -> { "ok": true }

---

## Detalle - Scheduler (v0)

Paginas ASPX principales:

- AnnualView, Calendar, DailyCoverage(+Planned), MovesNew, PunchesCorrector, Templates, AssignHolidays, ReorderMoves, ShiftSelector, Wizards (AssignCauses/Centers/CopySchedule/MassPunch/Incidences/TemplateAssign).

Scripts detectados:

- CalendarV2.js, MovesDetail.js/DevExpress.js/Extended.js, SchedulerDates.js, DateSelector.js, templateManagement.js, Wizards.js

Patrones AJAX/WebForms:

- srvScheduler.ashx: getBarButtons
- srvMoves.ashx: getDailyScheduleStatus (por empleado/fecha/vista)
- Dependencias a ../Employees/srvEmployees.ashx para borrar programaciones.
- Amplio uso de __doPostBack en wizards de asignacion/copia/incidencias.

Parametros detectados por accion (desde JS):

- srvScheduler.ashx
  - action=getBarButtons | params: ID

- srvMoves.ashx
  - action=getDailyScheduleStatus | params: IDEmployee, DateMove, View

- srvEmployees.ashx (dependencia)
  - action=deleteProgrammedAbsence | params: BeginDate, ID, IDCause
  - action=deleteProgInc | params: BeginDate, ID, IDAbsence, IDCause
  - action=deleteProgOvertime | params: IDProgOvertime
  - action=deleteProgHolidays | params: IDProgHoliday

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- GET /scheduler/moves/status/daily?employeeId={id}&date={yyyy-MM-dd}&view={day|week|month}
  -> { status:"ok", items:[{time:"08:00", type:"in", source:"punch"}], summary:{...} }
- GET /scheduler/bar-buttons?context={tab|view}&id={...} -> { buttons:[...] }
- POST /employees/{id}/programmed:delete { kind, key, beginDate?, causeId?, absenceId? } -> { ok }

Validaciones observadas (parcial):

- En wizards: flujo por pasos con confirmaciones; validaciones de seleccion de empleados, fechas y causas/turnos.
- En moves y calendar: consistencia de fechas/horas, reordenacion con restricciones; feedback en UI.

Notas permisos/licencia:

- Requiere permisos de planificacion para ver y editar; algunas vistas (coverage, analytics) podrian ser de licencia avanzada.

Pendientes:

- [ ] Parametria exacta de vistas view y formatos; reglas de restriccion en reordenacion/copia.

---

## Detalle - Security (v0)

Paginas ASPX principales:

- SecurityOptions, AdvancedSecurity, EmployeePermissions, License, SaaSAdmin(+Captcha), SupervisorsContent, Wizards (ChangePassword, CurrentLoggedUsers, NewPassportWizard, PassportSecurityActions), msg boxes.

Scripts detectados:

- SecurityOptions.js/SecurityOptionsData.js, Features.js, SupervisorsFrame.js, License.js, LockDb.js, AdvancedSecurity.js

Patrones AJAX/WebForms:

- srvSupervisorsV3.ashx: bar buttons, CRUD passport, checkPermission, cambios de nombre, reset/restore/SendUsername, feature permissions (SetFeaturePermission/SetDefaultFeaturePermission).
- srvSecurityOptions.ashx: get/new/save/delete (XSecurityOptions). Uso POST con AsyncCall.
- __doPostBack en AdvancedSecurity/LockDB y algunos wizards.

Parametros detectados por accion (desde JS):

- srvSupervisorsV3.ashx
  - action=getBarButtons | params: ID
  - action=deletePassport | params: ID
  - action=chgName | params: ID, NewName
  - action=savePassport | params: ID
  - action=checkPermission | params: (sin parametros)
  - action=resetPassport | params: ID, PassportType
  - action=restoreCegidID | params: ID, PassportType
  - action=SendUsername | params: ID, PassportType
  - action=SetFeaturePermission | params: ID, IDFeature, FeatureType, Permission
  - action=SetDefaultFeaturePermission | params: ID, IDFeature, FeatureType

- srvSecurityOptions.ashx
  - action=getXSecurityOptions | params: ID
  - action=newSecurityOptions | params: (sin parametros)
  - action=saveXSecurityOptions | params: (sin parametros)
  - action=deleteXSecurityOptions | params: ID

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- Supervisores/Passports:
  - GET /security/passports/{id}/actions -> { buttons:[...] }
  - GET /security/passports/{id}/permissions/check -> { allowed:true }
  - PATCH /security/passports/{id} { name? } -> { ok }
  - POST /security/passports/{id}:reset { type:"U|E" } -> { ok }
  - POST /security/passports/{id}:restoreCegid -> { ok }
  - POST /security/passports/{id}:send-username -> { ok }
  - POST /security/passports/{id}/features/{featureId}:set { type, permission } -> { ok }
  - POST /security/passports/{id}/features/{featureId}:set-default { type } -> { ok }

- Opciones de seguridad:
  - GET /security/options/{id} -> { controls:[...] }
  - POST /security/options { fields... } -> { ok, id }
  - PUT /security/options/{id} { fields... } -> { ok }
  - DELETE /security/options/{id} -> { ok }

Validaciones observadas (parcial):

- Cambios de opciones requieren consistencia y roles adecuados; Features: validaciones de tipo/permiso; bloqueo de DB con confirmacion y captcha.

Notas permisos/licencia:

- Requiere rol de seguridad/administracion; licencias gobiernan disponibilidad de features.

Pendientes:

- [ ] Precisar estructura de controls en SecurityOptions y catalogo de FeatureType/Permission.
-
---

## Detalle - Terminals (v0)

Paginas ASPX principales:

- Terminals.aspx, TerminalSelectorData.aspx, TerminalsList (en scripts), ZonePhoto/ZoomZone, TerminalSirens, ChangeIPTerminal, UploadDataFile, viewCam y Wizards (NewTerminal, ChangeCommsState/Password, RegisterMxC/RegisterWx1).

Scripts detectados:

- flReaderMap.js, frmAddSiren.js, frmCfgInteractive.js, TerminalsListV2.js, TerminalsSelector.js, TerminalsV2.js

Patrones AJAX/WebForms:

- srvTerminalsList.ashx: getTerminalsListTab, getBarButtons, getTerminalInfo, launchBroadcaster
- srvTerminals.ashx: getTerminalTab, getBarButtons, deleteXTerminal, launchBroadcaster
- __doPostBack en wizards de registro/cambios.

Parametros detectados por accion (desde JS):

- srvTerminalsList.ashx
  - action=getTerminalsListTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=getTerminalInfo | params: ID
  - action=launchBroadcaster | params: —

- srvTerminals.ashx
  - action=getTerminalTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=deleteXTerminal | params: ID
  - action=launchBroadcaster | params: —

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- GET /terminals/list/tabs/{tab}?id={...} -> { model }
- GET /terminals/list/{id}/actions -> { buttons:[...] }
- GET /terminals/{id}/tabs/{tab} -> { model }
- GET /terminals/{id}/actions -> { buttons:[...] }
- DELETE /terminals/{id} -> { ok }
- POST /terminals:broadcast { scope:"all|id", ids?:[] } -> { ok }

Validaciones observadas (parcial):

- Campos obligatorios en alta/registro; confirmacion para borrado; consistencia de IP/puertos; restricciones de zonas/fotos.

Notas permisos/licencia:

- Operaciones de broadcast/borrado requieren rol elevado; ciertas funciones (sirenas, maps) pueden ser edicion avanzada.

Pendientes:

- [ ] Precisar parametros de configuracion (comunicaciones, sirenas) y sus reglas.

---

## Detalle - Tasks (v0)

Paginas ASPX principales:

- Tasks.aspx, TaskField, TaskEmployeeStatus, TaskEditAssignments, TaskAlert, PartesTrabajo, BusinessCenters (Review/SelectorData), msg boxes y wizards (Create/CompleteTasks).

Scripts detectados:

- Task.js, TaskStatus.js, BusinessCentersV2.js, ReviewBusinessCenters.js, frmFilterBusinessCenters.js, AnalyticsTask.js, PartesTrabajo.js

Patrones AJAX/WebForms:

- srvBusinessCenters.ashx: getBusinessCentersTab, getBarButtons, deleteXBusinessCenter
- __doPostBack en wizards de completado de tareas.

Parametros detectados por accion (desde JS):

- srvBusinessCenters.ashx
  - action=getBusinessCentersTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=deleteXBusinessCenter | params: ID

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- Business Centers:
  - GET /tasks/business-centers/{id}/tabs/{tab} -> { model }
  - GET /tasks/business-centers/{id}/actions -> { buttons:[...] }
  - DELETE /tasks/business-centers/{id} -> { ok }
- Tasks:
  - GET /tasks/{id} -> { model }
  - PATCH /tasks/{id} { fields... } -> { ok }
  - POST /tasks/{id}:complete { notes?, evidences? } -> { ok }

Validaciones observadas (parcial):

- Nombres requeridos/unicos en BusinessCenters; confirmaciones para borrado; estados coherentes en TaskStatus; integridad de asignaciones.

Notas permisos/licencia:

- Acciones de completar/editar pueden requerir roles; analytics de tareas puede estar sujeta a licencia.

Pendientes:

- [ ] Parametria exacta de Task/PartesTrabajo y validaciones por formulario.

---

## Detalle - TaskTemplates (v0)

Paginas ASPX principales:

- TaskTemplates.aspx, TaskTemplateSelectorData, msg boxes y wizards (LaunchTaskTemplate, SelectTaskFields).

Scripts detectados:

- TaskTemplatesV2.js, ProjectsV2.js, TaskProjects.js, LaunchTaskTemplatesDev.js

Patrones AJAX/WebForms:

- srvTaskTemplates.ashx: getTaskTemplateTab, getBarButtons, deleteXTaskTemplate, getEmployeesSelected
- srvProjects.ashx: getProjectsTab, getBarButtons, deleteProject

Parametros detectados por accion (desde JS):

- srvTaskTemplates.ashx
  - action=getTaskTemplateTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=deleteXTaskTemplate | params: ID
  - action=getEmployeesSelected | params: IDTaskTemplate, NodesSelected

- srvProjects.ashx
  - action=getProjectsTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=deleteProject | params: ID

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- Templates:
  - GET /task-templates/{id}/tabs/{tab} -> { model }
  - GET /task-templates/{id}/actions -> { buttons:[...] }
  - DELETE /task-templates/{id} -> { ok }
  - GET /task-templates/{id}/employees-selected?nodes=... -> { items:[...] }
- Projects:
  - GET /task-projects/{id}/tabs/{tab} -> { model }
  - GET /task-projects/{id}/actions -> { buttons:[...] }
  - DELETE /task-projects/{id} -> { ok }

Validaciones observadas (parcial):

- Seleccion de empleados/campos consistente; confirmaciones antes de borrar; nombres unicos.

Notas permisos/licencia:

- Lanzamiento de templates puede ser feature licenciado; permisos de configuracion requeridos.

Pendientes:

- [ ] Confirmar estructura de `NodesSelected` y modelos de tabs.

---

## Detalle - Shifts (v0)

Paginas ASPX principales:

- Shifts.aspx/Shiftsv2.aspx, ShiftType, Schema/ShiftSelectorData, msg box, Standalone roDailyRule, wizard de grupos (NewShiftGroupWizard).

Scripts detectados:

- ShiftsV2.js, ShiftsCompact.js, ShiftsGroupsV2.js, SGroups.js/V2.js, frmEditShift*(Flexible/Mandatory/Break), frmDailyRule.js, frmNewTypeZone.js/V2.js, frmAddAssignment.js, frmAddZone.js

Patrones AJAX/WebForms:

- srvShifts.ashx / srvShiftsV2.ashx: getBarButtons, copyXShift, deleteXShift, shiftIsUsed/ForDel, selectedHolidayConceptIsAnualWork, retRuleDesc, (load/update/reload/delete)TypeZone
- srvShiftsGroups.ashx: getBarButtons, chgNameShiftGroup, deleteShiftGroup
- __doPostBack en wizard de grupos; varias vistas avanzadas (roDailyRule) con reglas.

Parametros detectados por accion (desde JS):

- srvShifts.ashx / srvShiftsV2.ashx
  - action=getBarButtons | params: ID
  - action=copyXShift | params: ID
  - action=deleteXShift | params: ID
  - action=shiftIsUsed | params: ID
  - action=shiftIsUsedForDel | params: ID
  - action=selectedHolidayConceptIsAnualWork | params: ID
  - action=retRuleDesc | params: —
  - action=loadTypeZone | params: —
  - action=updateTypeZone | params: —
  - action=reloadTypeZones | params: —
  - action=deleteTypeZone | params: —

- srvShiftsGroups.ashx
  - action=getBarButtons | params: ID
  - action=chgNameShiftGroup | params: ID, NewName
  - action=deleteShiftGroup | params: ID

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- GET /shifts/{id}/actions -> { buttons:[...] }
- POST /shifts/{id}:copy -> { ok, newId }
- DELETE /shifts/{id} -> { ok }
- GET /shifts/{id}/is-used -> { used:true, details?:{...} }
- GET /shifts/{id}/rules/description -> { text }
- TypeZones:
  - GET /shifts/type-zones -> { items:[...] }
  - POST /shifts/type-zones { fields... } -> { ok, id }
  - PUT /shifts/type-zones/{id} { fields... } -> { ok }
  - DELETE /shifts/type-zones/{id} -> { ok }

Validaciones observadas (parcial):

- Nombres de grupos, reglas y zonas; dependencias (shiftIsUsed antes de borrar/copiar); descripciones de reglas deben existir; coherencia de configuraciones flexible/mandatory/break.

Notas permisos/licencia:

- Configuración de turnos suele requerir roles elevados; ciertas características avanzadas pueden ser de licencia superior.

Pendientes:

- [ ] Detallar campos de TypeZone y formatos de reglas (roDailyRule) para derivar validaciones UI.

---

## Detalle - Documents (v0)

Paginas ASPX principales:

- DocumentTemplate(.aspx/Wizard), GridDocuments, srvMsgBoxDocument, downloadFile.

Scripts detectados:

- Documents.js, DocumentsWizard.js, DocumentTemplate.js/DocumentTemplateWizard.js, GridDocuments.js/GridDocumentsWizard.js

Patrones AJAX/WebForms:

- srvDocumentTemplates.ashx: getBarButtons, deleteDocumentTemplate
- Descargas: downloadFile.aspx (streaming)

Parametros detectados por accion (desde JS):

- srvDocumentTemplates.ashx
  - action=getBarButtons | params: ID
  - action=deleteDocumentTemplate | params: ID

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- GET /documents/templates/{id}/actions -> { buttons:[...] }
- DELETE /documents/templates/{id} -> { ok }
- GET /documents/templates/{id} -> { model }
- POST /documents/templates { fields... } -> { ok, id }
- PUT /documents/templates/{id} { fields... } -> { ok }
- GET /documents/download/{id} -> stream (autenticado, auditado)

Validaciones observadas (parcial):

- Nombres requeridos/unicos; confirmacion en borrado; verificaciones de versionado/uso antes de eliminar; seguridad en descargas (roles y auditoria).

Notas permisos/licencia:

- Templates/documentos pueden pertenecer a ediciones especificas; descargas bajo permisos y auditoria.

Pendientes:

- [ ] Modelos exactos de templates y campos; cabeceras/firmas en descargas.

---

## Detalle - Options (v0)

Paginas ASPX principales:

- ConfigurationOptions, EmergencyReport, FieldsManager, BusinessCenterField, TaskField, UserField, UploadCertificate, msg box.

Scripts detectados:

- options.js

Patrones AJAX/WebForms:

- __doPostBack en guardado/cancelacion; eliminacion masiva de biometria via `../Employees/srvEmployees.ashx?action=DeleteBiometricsAllEmployees`.
- options.js usa $.ajax (varias operaciones no visibles en este recorte).

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- GET /options/config -> { sections:[{key,fields:[{key,value,type,options..}]}] }
- PUT /options/config { changes:[{key,value}] } -> { ok }
- POST /options/biometrics:delete-all -> { ok }
- Campos (User/Task/BusinessCenterField): CRUD estandar en `/options/fields/*`.

Validaciones observadas (parcial):

- Confirmacion para operaciones masivas; restricciones por rol; formatos de certificados.

Pendientes:

- [ ] Extraer mapa de secciones/campos y operaciones exactas de options.js.

Ejemplos:

- GET /options/config -> { "sections": [{ "key": "general", "fields": [{"key":"ui.language","value":"es","type":"select","options":["es","en"]}] }] }
- PUT /options/config { "changes": [{"key":"ui.language","value":"en"}] } -> { "ok": true }
- POST /options/biometrics:delete-all -> { "ok": true }

---

## Detalle - Requests (v0)

Paginas ASPX principales:

- Requests, RequestApprovals, srvRequests, msg box.

Scripts detectados:

- Requests.js, roRequestListParams.js

Patrones AJAX/WebForms:

- srvRequests.ashx: getBarButtons, getSupervisorsPending(IDRequest), y llamadas $.ajax varias.

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- GET /requests?state=pending|approved|rejected&filter=... -> { items:[...], total }
- GET /requests/{id}/actions -> { buttons:[...] }
- GET /requests/{id}/supervisors-pending -> { items:[{id,name,role}] }
- POST /requests/{id}:approve { note? } -> { ok }
- POST /requests/{id}:reject { reason } -> { ok }

Ejemplos:

- GET /requests?state=pending&filter=employee:smith
  Respuesta: { "items": [{"id":771,"type":"absence","employee":"E00123","from":"2025-03-03","to":"2025-03-05","state":"pending"}], "total": 42 }
- POST /requests/771:approve { "note": "OK" } -> { "ok": true }

Validaciones observadas (parcial):

- Reglas de aprobacion por rol y niveles; motivos obligatorios en rechazo.

Pendientes:

- [ ] Listado de estados y transiciones por tipo de solicitud (ver Emplyx_Guia workflows).

---

## Detalle - Indicators (v0)

Paginas ASPX principales:

- Indicators, SelectorData, msg box.

Scripts detectados:

- IndicatorsV2.js

Patrones AJAX/WebForms:

- srvIndicators.ashx: getIndicatorsTab(aTab,ID), getBarButtons(ID), deleteXIndicator(ID)

Propuesta de contratos JSON v0.1:

- GET /indicators/{id}/tabs/{tab} -> { model }
- GET /indicators/{id}/actions -> { buttons:[...] }
- DELETE /indicators/{id} -> { ok }

Ejemplos:

- GET /indicators/10/tabs/overview -> { "model": { "name":"Tardanzas","desc":"% llegadas tarde","series":[...] } }
- DELETE /indicators/10 -> { "ok": true }

Validaciones observadas (parcial):

- Confirmacion en borrado; dependencias antes de eliminar.

---

## Detalle - SecurityChart (v0)

Paginas ASPX principales:

- SecurityFunctions, SelectorData, Wizard SupervisorCopy, msg box.

Scripts detectados:

- SecurityFunctions.js

Patrones AJAX/WebForms:

- srvSecurityFunctions.ashx: getSecurityFunctionTab(aTab,ID), getBarButtons(ID), copySecurityFunction(aTab,ID), deleteSecurityFunction(ID)

Propuesta de contratos JSON v0.1:

- GET /security/functions/{id}/tabs/{tab} -> { model }
- GET /security/functions/{id}/actions -> { buttons:[...] }
- POST /security/functions/{id}:copy -> { ok, newId }
- DELETE /security/functions/{id} -> { ok }

Ejemplos:

- GET /security/functions/5/tabs/details -> { "model": { "name":"Aprobar Solicitudes", "scopes":["approve"], "roles":["Supervisor"] } }
- POST /security/functions/5:copy -> { "ok": true, "newId": 11 }

Validaciones observadas (parcial):

- Reglas de copia; evitar duplicados; permisos de seguridad requeridos.

---

## Detalle - Concepts (v0)

Paginas ASPX principales:

- Concepts, ConceptGroups, Selectors, msg box.

Scripts detectados:

- ConceptsV2.js, ConceptsGroupsV2.js, CGroups.js

Patrones AJAX/WebForms:

- srvConcepts.ashx: getBarButtons(ID), (otros via AjaxCall)
- srvConceptGroups.ashx: getBarButtons(ID), deleteXConceptGroup(ID)

Propuesta de contratos JSON v0.1:

- GET /concepts/{id}/actions -> { buttons:[...] }
- DELETE /concepts/{id} -> { ok }
- GET /concept-groups/{id}/actions -> { buttons:[...] }
- DELETE /concept-groups/{id} -> { ok }

Ejemplos:

- GET /concepts/22/actions -> { "buttons":["save","delete"] }
- DELETE /concept-groups/7 -> { "ok": true }

Validaciones observadas (parcial):

- Dependencias antes de borrar; nombres unicos.

---

## Detalle - Causes (v0)

Paginas ASPX principales:

- Causes, SelectorData, msg box.

Scripts detectados:

- CausesV2.js

Patrones AJAX/WebForms:

- srvCauses.ashx: getCauseTab(aTab,ID), getBarButtons(ID), causeIsUsed(ID), deleteXCause(ID)

Propuesta de contratos JSON v0.1:

- GET /causes/{id}/tabs/{tab} -> { model }
- GET /causes/{id}/actions -> { buttons:[...] }
- GET /causes/{id}/is-used -> { used:true }
- DELETE /causes/{id} -> { ok }

Ejemplos:

- GET /causes/3/tabs/main -> { "model": { "name":"Baja Medica","type":"absence","params":{...} } }
- GET /causes/3/is-used -> { "used": true }

Validaciones observadas (parcial):

- Evitar eliminar causas en uso; reglas por tipo de causa.

---

## Detalle - Notifications (v0)

Paginas ASPX principales:

- Notifications, SelectorData, msg box.

Scripts detectados:

- Notificationsv2.js

Patrones AJAX/WebForms:

- srvNotifications.ashx: getBarButtons(ID), deleteXNotification(ID), (otros via AsyncCall)

Propuesta de contratos JSON v0.1:

- GET /notifications/{id}/actions -> { buttons:[...] }
- DELETE /notifications/{id} -> { ok }
- GET /notifications?filter=... -> { items:[...], total }

Ejemplos:

- GET /notifications?filter=state:active -> { "items": [{"id": 90, "title":"Mantenimiento","state":"active","audience":"All"}], "total": 8 }
- DELETE /notifications/90 -> { "ok": true }

Validaciones observadas (parcial):

- Reglas de envio/destinatarios; auditoria de notificaciones.

---

## Detalle - DataLink (v0)

Paginas ASPX principales:

- DataLinkBusiness, Selectors de import/export, wizards Import/Export y downloadFile.

Scripts detectados:

- DataLinksV2.js

Patrones AJAX/WebForms:

- srvDatalinkBusiness.ashx: (operaciones de negocio; acciones no visibles en recorte)

Propuesta de contratos JSON v0.1:

- GET /datalink/business/{id} -> { model }
- POST /datalink/import { config } -> { jobId }
- POST /datalink/export { config } -> { jobId }
- GET /datalink/jobs/{jobId}/status -> { state, progress }
- GET /datalink/download/{id} -> stream

Ejemplos:

- POST /datalink/import { "config": { "type":"employees", "format":"csv", "storage":"blob://..." } } -> { "jobId": "imp-2025-001" }
- GET /datalink/jobs/imp-2025-001/status -> { "state":"running", "progress": 45 }
- POST /datalink/export { "config": { "type":"punches", "range":"2025-01" } } -> { "jobId": "exp-2025-010" }

Validaciones observadas (parcial):

- Configuracion valida para import/export; seguimiento de jobs; permisos adecuados.

---

## Detalle - LabAgree (v0)

Paginas ASPX principales:

- LabAgree, SelectorData, msg box.

Scripts detectados:

- LabAgreeV2.js y formularios de reglas (edit cause limit, request validation, schedule rules, startup value) + grids.

Patrones AJAX/WebForms:

- srvLabAgree.ashx: (CRUD de reglas; acciones no visibles en recorte)

Propuesta de contratos JSON v0.1:

- GET /lab-agree/{id} -> { model }
- PUT /lab-agree/{id} { rules... } -> { ok }
- GET /lab-agree/{id}/rules/{rule} -> { model }

Ejemplos:

- GET /lab-agree/1 -> { "model": { "rules": { "schedule":"...", "limits":{...} } } }
- PUT /lab-agree/1 { "rules": { "limits": { "maxHoursDay": 10 } } } -> { "ok": true }

Validaciones observadas (parcial):

- Coherencia en reglas (limites, validaciones, horarios); auditoria de cambios.

---

## Detalle - DiningRoom (v0)

Paginas ASPX principales:

- DiningRoom, SelectorData, msg box.

Scripts detectados:

- DiningRooms.js

Patrones AJAX/WebForms:

- srvDiningRoom.ashx: (CRUD de comedores; acciones no visibles en recorte)

Propuesta de contratos JSON v0.1:

- GET /dining-rooms/{id} -> { model }
- PATCH /dining-rooms/{id} { fields... } -> { ok }
- DELETE /dining-rooms/{id} -> { ok }

Ejemplos:

- GET /dining-rooms/3 -> { "model": { "name":"Comedor Norte","capacity":120,"hours":{"open":"08:00","close":"16:00"} } }
- PATCH /dining-rooms/3 { "capacity": 140 } -> { "ok": true }

Validaciones observadas (parcial):

- Capacidad, horarios, ubicaciones; permisos para gestion.

---

## Detalle - SDK (v0)

Paginas ASPX principales:

- ManagePunches, msg box.

Scripts detectados:

- roSDK.js

Patrones AJAX/WebForms:

- srvManagePunches.ashx: (operaciones sobre fichajes)

Propuesta de contratos JSON v0.1:

- POST /sdk/punches { employeeId, time, type, device? } -> { ok, id }
- DELETE /sdk/punches/{id} -> { ok }
- GET /sdk/punches?employeeId=...&range=... -> { items:[...], total }

Ejemplos:

- POST /sdk/punches { "employeeId": 123, "time": "2025-02-01T08:03:00Z", "type":"in" } -> { "ok": true, "id": 9901 }
- GET /sdk/punches?employeeId=123&range=2025-02 -> { "items": [{"id":9901,"time":"2025-02-01T08:03:00Z","type":"in"}], "total": 28 }

Validaciones observadas (parcial):

- Coherencia de tiempos, device opcional, permisos.

---

## Detalle - Absences (v0)

Paginas ASPX principales:

- AbsencesStatus

Scripts detectados:

- AbsencesStatus.js

Patrones AJAX/WebForms:

- (no se detectan handlers en recorte; UI probablemente consulta via modulos relacionados)

Propuesta de contratos JSON v0.1:

- GET /absences/status?filter=... -> { items:[...], total }

---

## Detalle - Base (v0)

Paginas y controles:

- Paginas base: AccessDenied, Error/Oops, WarmUp, srvLoginMsgBox, popups comunes (AboutMe, GenericCaptchaValidator) y múltiples WebUserControls (EmployeeSelectorData, EventSelectorData, roCalendar, roFilterList...).

Recursos compartidos:

- 235 JS y 197 CSS aprox.; incluye Ext JS 3.4.0 (carpetas `ext-3.4.0`), estilos visuales, temas y componentes.

Handlers relevantes:

- Base/Handlers/srvMain.ashx
- Base/WebUserControls/handlers/srvUserFields.ashx
- Base/WebUserControls/roCalendar/Handlers/srvCalendarImport.ashx

Propuesta (migracion UI):

- Reemplazar controles legacy (ExtJS/propios) por componentes Blazor + Bootstrap.
- Definir equivalentes: Dialog/MessageBox, Calendar/DatePicker, FilterList/Selector, EmployeeSelector.

Contratos JSON v0.1 sugeridos:

- GET /ui/user-fields?entity=Employee -> { fields:[...] }
- POST /ui/calendar/import { file, options } -> { jobId }
- GET /ui/main/actions -> { buttons:[...] }

Notas:

- Base es una dependencia transversal; cada pantalla nueva debe mapear a componentes comunes para consistencia.

---

## Anexo — Validaciones afinadas (v0)

### Employees

- Selección requerida de grupo para operaciones en Employees/Groups (Error.MustSelectAGroup).
- Estados de grilla: no permitir operaciones mientras hay edición pendiente (Error.ValidationGridIsEditingError).
- Campos básicos obligatorios al crear/editar empleado (Error.ValidationFieldsFailed): nombre, idioma válido, etc.
- Reglas de teletrabajo (ContractScheduleRules):
  - Rango de fechas válido (From <= To); máximos por periodo (mes <= 31; trimestre <= 93).
  - Días obligatorios/opcionales pertenecen al set permitido; validaciones numéricas en conteos.

### Access

- Filtros y placas (AccessFilterPlates): al menos una placa o empleado; fecha inicial requerida; formato de día/mes con dos dígitos.
- Grupos de Acceso (AccessGroup): campos obligatorios; seleccionar/guardar turno antes de continuar (Info.SelectOrSaveShiftTitle).
- Periodos (AccessPeriod/frmNewPeriod):
  - Selección de día de semana; periodo horario correcto (desde/hasta válidos y ordenados).
  - Día numérico válido (no NaN) y dentro de rango; mes requerido.
- Autorizaciones (frmAddAuthorization): fechas de autorizaciones requeridas.
- Zonas (AccessZone): campos obligatorios y validación de filas seleccionadas antes de acciones.

### Scheduler

- Calendar/DateSelector:
  - Periodos válidos: al menos dos fechas seleccionadas para definir rango; errores específicos para rangos inválidos (Error.DatesPeriod).
  - Validar que haya empleados seleccionados cuando se requiera (`getSelectedIDEmployees().length > 0 || !requiereEmployee`).
  - Límite de longitud para consultas/URLs (Error.QueryLenghtExceed); mostrar alerta cuando exceda.
  - Mensajes de “sin cambios que validar” en operaciones (roNotChangesToValidate).
- Wizards (Assign/Centers/Copy/MassPunch/Incidences/TemplateAssign):
  - Validación de fechas de periodo; feedback de éxito/fracaso en copias; confirmaciones por paso.
- Moves/MovesDetail/DevExpress:
  - `DateMove` requerido; `statusNumber` debe ser numérico; valores por defecto para campos indefinidos (idaction, idterminal, idzone, idpassport...).
  - Validar formato/hora y no vacíos en tiempos; filas con índice válido (row default a -1 si vacío).

### Security

- AdvancedSecurity/LockDB:
  - Valores mínimos por defecto (p.ej., input min=3); validación general al guardar; confirmaciones en bloqueo de BD.
- SecurityIP Editor:
  - Octetos IP requeridos y numéricos; validación de rango (min/max) e IP no válidas (Error.NotValidIp).
  - Prevención de rangos duplicados (Error.Description.DuplicatedIpRange).
- Supervisors/Passports frame:
  - `allowedIPs` no vacío si aplica; validación general de campos requeridos (Error.ValidationFieldsFailed).
  - Formularios de categorías de solicitud: categoría, nivel y siguiente nivel obligatorios; evitar combinaciones inválidas (NextLevelInferior); evitar duplicados; al menos una categoría en grilla.
- SecurityOptions/Data:
  - Validaciones de longitud de colecciones/controles (arrFields); manejo de errores en creación/edición de opciones.

### Alerts

- Filtros obligatorios (rango de fechas o estado) para consultas pesadas.
- Confirmación en acciones masivas de cola (TasksQueue) y feedback de progreso.
- Descargas solo con permisos; manejar archivos no encontrados/expirados.

### Cameras

- Confirmación para borrar cámaras; validación de vínculos (ubicación/zonas) antes de eliminar.
- Validación de URL/stream y estado online/offline (si aplica en UI).

### AIScheduler

- Rangos válidos en resumen de unidades; selección requerida de unidad/fecha.
- Confirmación al eliminar unidades productivas; dependencias con Scheduler.

### Terminals

- IP/puerto con formato válido; credenciales/configuración requeridas.
- Confirmación para borrado y broadcast; límites de frecuencia (throttling) visibles en UI.

### Tasks / TaskTemplates

- BusinessCenters: nombre único y requerido; evitar borrar si está en uso.
- Tasks: integridad de asignaciones; notas/evidencias opcionales en completar; estados válidos.
- Templates/Projects: nombres únicos; selección consistente de empleados/campos; confirmación en borrado.

### Shifts

- Verificar uso antes de copiar/borrar (is-used/forDel); reglas coherentes; descripciones disponibles.
- TypeZones: campos requeridos; evitar duplicados; rangos coherentes.

### Documents

- Nombre único; evitar borrar en uso; validar plantillas (campos obligatorios); descargas con auditoría.

### Options

- Confirmación para operaciones masivas (biometría); formato de certificados; campos de configuración válidos.

### Indicators / SecurityChart

- Confirmación en borrado; evitar duplicados en copia; permisos de seguridad.

### Concepts / Causes / Notifications

- Causes: no eliminar en uso; tabs completos; validaciones por tipo.
- Concepts: nombres únicos; dependencias antes de eliminar.
- Notifications: destinatarios válidos; fechas de publicación; estados coherentes.

### DataLink / LabAgree / DiningRoom / SDK / Absences / Base

- DataLink: configuración mínima para import/export; límite de tamaño; pre‑flight checks; seguimiento de jobs.
- LabAgree: reglas coherentes y completas; auditoría.
- DiningRoom: capacidad y horarios; ubicación válida.
- SDK: coherencia temporal; evitar duplicados; permisos.
- Absences: filtros por rango y causa.
- Base: tamaño/formatos en importación de calendario; user‑fields válidos.

---

## Anexo — Matriz de validaciones por pantalla (v0)

Formato: Pantalla → Campo/Sección → Regla de validación → Mensaje/UI → Observaciones.

- Employees/Employees.aspx → Filtros → Nombre/ID opcionales; no bloquear sin filtros → Mensaje "Error.ValidationFieldsFailed" → Permitir búsqueda vacía con paginación.
- Employees/EditProgrammedAbsence.aspx → Fechas (From/To) → From ≤ To, formato fecha válido → "Error.ValidationTCDates" → Aplicar calendario y normalizar zona.
- Employees/Groups.aspx → Selección grupo → Requerido para acciones → "Error.MustSelectAGroup" → Deshabilitar botones si no hay selección.
- Access/AccessPeriods.aspx → Día de semana/mes → Requeridos; hora desde/hasta válidas y ordenadas → "Error.frmNewPeriod.*" → Aplicar máscaras HH:mm; validar NaN.
- Access/AccessZones.aspx → Selección en grilla → Requerida para acciones → "Error.ValidationFieldsFailed" → Resaltar fila requerida.
- Access/AccessStatusMonitor.aspx → Filtros → Requerir al menos un filtro para vistas pesadas → Toast de validación → Evitar DoS desde UI.
- Scheduler/CalendarV2.aspx → Rango de fechas → Al menos 2 fechas para rango → "Error.DatesPeriod" → Mostrar ayuda contextual.
- Scheduler/MovesDetail.aspx → Hora/Tipo/Terminal → No vacíos; numéricos válidos → Alert/Toast → Defaults 0 cuando indefinido.
- Requests/Requests.aspx → Aprobación/Rechazo → Motivo requerido en rechazo; transición válida → Error "REQUEST_REASON_REQUIRED"/"REQUEST_INVALID_TRANSITION" → Confirmación previa.
- Alerts/Alerts.aspx → Filtros → Rango de fechas o estado → Toast de validación → Prevenir listas masivas sin filtro.
- Cameras/Cameras.aspx → Borrado → Confirmación requerida → Dialog Confirm → Verificar dependencias.
- Terminals/Terminals.aspx → IP/Puerto → Formato IPv4 y puerto 1–65535 → Mensaje de formato → Mostrar ejemplo.
- Shifts/Shiftsv2.aspx → Borrado/Copia → Verificar uso antes (is-used) → Mensaje de dependencia → Forzar confirmación doble si usado.
- Documents/DocumentTemplate.aspx → Nombre/Versión → Requeridos; nombre único → Toast/Inline → Validar uso antes de borrar.
- Options/ConfigurationOptions.aspx → Cambios masivos → Confirmación requerida → Dialog Confirm → Mostrar resumen de cambios previos.

Nota: Esta matriz se ampliará con pantallas adicionales conforme se identifiquen validaciones específicas durante 11.2–11.4.

---

## Anexo — Estados y transiciones (workflows) por dominio (v0)

- Requests: pending → (approve) → approved; pending → (reject) → rejected; approved/rejected → (cancel) → cancelled (según rol). Reglas por categoría/nivel en Security.
- Alerts: open → resolved; resolved → reopen (según permisos).
- Tasks: open → in_progress → completed/cancelled; transiciones restringidas por rol; completar con notas/evidencias.
- Terminals: offline ↔ online ↔ unknown (monitorización); broadcast lanza job (queued → running → completed/failed).
- DataLink jobs: queued → running → completed/failed; reintentos con backoff.

## Anexo — Flujos end-to-end (v0)

Nota: Cada flujo describe la secuencia UI → Adaptador (Emplyx.ApiAdapter) → Legacy (.ashx/.aspx) → Respuesta → UI, con validaciones y permisos clave. Durante la transición, los adaptadores consumen endpoints legacy como caja negra.

### Login / Auth / SSO

- UI: VTLogin (Default/VTPortalLogin) o Login Emplyx → POST /auth/login { username, password }
- Adapter: valida credenciales contra legacy (Auth/*.aspx) o proveedor actual; gestiona cookies/sesiones.
- UI: redirige a Home tras éxito; en SSO → GET /auth/sso/check → redirección según resultado.
- Validaciones: usuario/clave requeridos; errores de autenticación; bloqueo por intentos; MFA (si aplica en Emplyx futuro).
- Permisos: rol inicial y tenant/empresa cargados al contexto.

### Home / Main / Menú

- UI: Main.aspx → carga menú y accesos rápidos.
- Adapter: GET /ui/main/actions → botones/acciones según permisos/licencia.
- UI: renderiza navegación; breadcrumbs según contexto.

### Employees — Lista/Detalle/Ediciones comunes

- UI: Employees.aspx (lista) → filtros/paginación.
- Adapter: GET /employees?page,size,filter,sort → items/total.
- UI: abrir detalle → GET /employees/{id}; tabs (grupos, contratos, permisos, movilidad) → GET /groups/{id}/tabs/{tab} …
- Acciones:
  - Cambiar nombre/idioma → PATCH /employees/{id}.
  - Borrar biometría → DELETE /employees/{id}/biometrics.
  - Borrar programaciones (ausencias/inc/holidays/overtime) → DELETE /employees/{id}/programmed/{kind}/{key}.
- Validaciones: campos básicos; grilla no en edición; teletrabajo (rangos/máximos/set de días); confirmaciones para acciones destructivas; auditoría.
- Permisos/licencia: operaciones elevadas (biometría, movilidad) y features avanzadas bajo licencia.

### Access — Grupos/Periodos/Estados/Zonas/Events

- UI: AccessGroups/Periods/Status/Zones → tabs.
- Adapter: GET /access/{dominio}/{id}/tabs/{tab} y /actions.
- Acciones: delete/copy/emptyEmployees según dominio; EventScheduler copy/delete (fecha requerida).
- Validaciones: campos obligatorios; periodos con día/mes/horas válidos; autorizaciones con fechas; zonas con selección válida; monitor con filtros.
- Permisos: roles de seguridad; algunas vistas (monitor) sujetas a licencia.

### Scheduler — Calendario/Estados/Wizards

- UI: CalendarV2 (+ DateSelector) → selección rango/fechas.
- Adapter: GET /scheduler/moves/status/daily?employeeId&date&view → estado del día.
- Wizards: Assign/Centers/Copy/MassPunch/Incidences/TemplateAssign → pasos con confirmaciones.
- Dependencias: borrado de programaciones vía endpoints Employees.
- Validaciones: mínimo 2 fechas; rangos válidos; empleados seleccionados; límite de URL; defaults y tipos numéricos en moves.

### Requests — Aprobaciones

- UI: Requests/RequestApprovals → lista y detalle.
- Adapter: GET /requests?state=…; GET /requests/{id}/supervisors-pending.
- Acciones: approve/reject → POST /requests/{id}:approve|reject.
- Validaciones: motivo obligatorio en rechazo; transiciones válidas por rol y nivel.
- Permisos: supervisores y RRHH; auditoría.

### Alerts — Lista/Detalle/Cola/Descarga

- UI: Alerts.aspx → filtros y lista; abrir detalle; TasksQueue; downloadFile.
- Adapter: GET /alerts?…; GET /alerts/{id}; GET /alerts/tasks-queue; GET /alerts/download/{id} → stream.
- Validaciones: rango de fechas/filtros; confirmación de acciones masivas en cola; seguridad en descargas.

### Cameras — Tabs/Acciones

- UI: Cameras.aspx → tabs.
- Adapter: GET /cameras/{id}/tabs/{tab}; GET /cameras/{id}/actions; DELETE /cameras/{id}.
- Validaciones: confirmación de borrado; dependencias.

### Terminals — Lista/Info/Broadcast/Delete

- UI: Terminals.aspx/TerminalsList → tabs e info.
- Adapter: GET /terminals/list/tabs/{tab}?id=…; GET /terminals/{id}/tabs/{tab}; GET /terminals/{id}/actions; DELETE /terminals/{id}; POST /terminals:broadcast { scope, ids? }.
- Validaciones: IP/puertos y credenciales; confirmación; permisos altos para broadcast.

### Tasks — Business Centers/Tasks

- UI: BusinessCenters y Tasks.
- Adapter: GET /tasks/business-centers/{id}/tabs/{tab}; /actions; DELETE /…; GET/PATCH /tasks/{id}; POST /tasks/{id}:complete.
- Validaciones: nombres únicos; integridad de asignaciones; estados coherentes en TaskStatus.

### TaskTemplates — Templates/Projects

- UI: TaskTemplates, wizards.
- Adapter: GET /task-templates/{id}/tabs/{tab}; /actions; DELETE; GET employees-selected; projects.*
- Validaciones: selección consistente; confirmaciones; nombres únicos.

### Shifts — Copia/Borrado/TypeZones

- UI: Shifts/Shiftsv2, wizard grupos.
- Adapter: GET /shifts/{id}/actions; POST /{id}:copy; DELETE /{id}; GET is-used; rules/description; CRUD type-zones.
- Validaciones: verificar uso antes de borrar/copiar; reglas coherentes.

### Documents — Templates/Descargas

- UI: DocumentTemplate/Wizard; GridDocuments; downloadFile.
- Adapter: GET/POST/PUT/DELETE /documents/templates/{id}; GET /documents/download/{id}.
- Validaciones: nombres únicos; uso antes de eliminar; seguridad/auditoría en descargas.

### Options — Configuración/Campos/Biometría

- UI: ConfigurationOptions, FieldsManager, UploadCertificate.
- Adapter: GET/PUT /options/config; POST /options/biometrics:delete-all; CRUD `/options/fields/*`.
- Validaciones: confirmaciones para operaciones masivas; formatos de certificados; roles adecuados.

### Indicators — Tabs/Acciones

- UI: Indicators.
- Adapter: GET /indicators/{id}/tabs/{tab}; /actions; DELETE /{id}.
- Validaciones: confirmaciones; dependencias.

### SecurityChart — Funciones de seguridad

- UI: SecurityFunctions; wizard copy.
- Adapter: GET /security/functions/{id}/tabs/{tab}; /actions; POST /{id}:copy; DELETE /{id}.
- Validaciones: evitar duplicados; permisos de seguridad.

### Concepts / Causes / Notifications

- Concepts: GET /concepts/{id}/actions; DELETE; groups análogos.
- Causes: GET /causes/{id}/tabs/{tab}; /actions; GET /{id}/is-used; DELETE.
- Notifications: GET /notifications?…; /{id}/actions; DELETE /{id}.
- Validaciones: causas en uso; destinatarios; auditoría de envíos.

### DataLink — Import/Export/Jobs

- UI: wizards Import/Export, download.
- Adapter: POST /datalink/import|export { config } → { jobId }; GET /datalink/jobs/{jobId}/status; GET /datalink/download/{id}.
- Validaciones: config válida; seguimiento de jobs; permisos.

### LabAgree — Reglas de acuerdos

- UI: LabAgree (+ formularios de reglas).
- Adapter: GET /lab-agree/{id}; PUT /{id} { rules… }; GET /{id}/rules/{rule}.
- Validaciones: coherencia; auditoría de cambios.

### DiningRoom — CRUD

- UI: DiningRoom.
- Adapter: GET /dining-rooms/{id}; PATCH /{id}; DELETE /{id}.
- Validaciones: capacidad/horarios/ubicación; permisos.

### SDK — Fichajes

- UI: ManagePunches.
- Adapter: POST /sdk/punches; DELETE /sdk/punches/{id}; GET /sdk/punches?employeeId&range.
- Validaciones: coherencia temporal; device opcional; permisos.

### Absences — Estado

- UI: AbsencesStatus.
- Adapter: GET /absences/status?filter=… → items/total.
- Validaciones: filtros por rango/causa.

### Base — Controles y servicios transversales

- UI: Pages y WebUserControls.
- Adapter: GET /ui/user-fields?entity=…; POST /ui/calendar/import; GET /ui/main/actions.
- Validaciones: tamaño/formato de importaciones; permisos y consistencia de user-fields.

---

## Anexo — Matriz de trazabilidad Legacy → Emplyx.ApiAdapter (v0)

Ejemplos (no exhaustivo; ampliar en 11.1 continuo):

- Employees/Handlers/srvEmployees.ashx → /employees, /employees/{id}, /employees/{id}/summary, /employees/{id}/biometrics, /employees/{id}/programmed/{kind}/{key}
- Employees/Handlers/srvGroups.ashx → /groups/{id}/tabs/{tab}, /groups/{id}
- Access/Handlers/srvAccessGroups.ashx → /access/groups/{id}/tabs/{tab}, /access/groups/{id}:copy, /access/groups/{id}:delete, /access/groups/{id}:emptyEmployees
- Access/Handlers/srvAccessPeriods.ashx → /access/periods/{id}/tabs/{tab}, /access/periods/{id}:delete, /access/periods/{id}/description
- Access/Handlers/srvAccessStatus.ashx → /access/status/{id}/tabs/{tab}, /access/status/main/{id}/tabs/{tab}, /access/status/{id}/actions
- Access/Handlers/srvAccessZones.ashx → /access/zones/{id}/tabs/{tab}, /access/zones/{id}/actions, /access/zones/{id}:delete
- Access/Handlers/srvEventsScheduler.ashx → /access/events/{id}:copy, /access/events/{id}:delete, /access/events/{id}/actions
- Scheduler/Handlers/srvMoves.ashx → /scheduler/moves/status/daily
- Scheduler/Handlers/srvScheduler.ashx → /scheduler/bar-buttons
- AIScheduler/Handlers/srvProductiveUnit.ashx → /scheduler/units/{id}/tabs/{tab}, /scheduler/units/{id}/actions, /scheduler/units/{id}:delete, /scheduler/units/{id}/summary
- Cameras/Handlers/srvCameras.ashx → /cameras/{id}/tabs/{tab}, /cameras/{id}/actions, /cameras/{id}
- Tasks/Handlers/srvBusinessCenters.ashx → /tasks/business-centers/{id}/tabs/{tab}, /tasks/business-centers/{id}/actions, /tasks/business-centers/{id}
- TaskTemplates/Handlers/srvTaskTemplates.ashx → /task-templates/{id}/tabs/{tab}, /task-templates/{id}/actions, /task-templates/{id}
- TaskTemplates/Handlers/srvProjects.ashx → /task-projects/{id}/tabs/{tab}, /task-projects/{id}/actions, /task-projects/{id}
- Shifts/Handlers/srvShifts(.ashx|V2.ashx) → /shifts/{id}/actions, /shifts/{id}:copy, /shifts/{id}, /shifts/{id}/is-used, /shifts/rules/description, /shifts/type-zones/*
- Shifts/Handlers/srvShiftsGroups.ashx → /shifts/groups/{id}/actions, /shifts/groups/{id}
- Documents/Handlers/srvDocumentTemplates.ashx → /documents/templates/{id}, /documents/templates/{id}/actions, /documents/download/{id}
- Options/scripts/options.js + (legacy) → /options/config, /options/fields/*, /options/biometrics:delete-all
- Indicators/Handlers/srvIndicators.ashx → /indicators/{id}/tabs/{tab}, /indicators/{id}/actions, /indicators/{id}
- SecurityChart/Handlers/srvSecurityFunctions.ashx → /security/functions/{id}/tabs/{tab}, /security/functions/{id}/actions, /security/functions/{id}:copy, /security/functions/{id}
- Concepts/Handlers/srvConcepts.ashx → /concepts/{id}/actions, /concepts/{id}
- Concepts/Handlers/srvConceptGroups.ashx → /concept-groups/{id}/actions, /concept-groups/{id}
- Causes/Handlers/srvCauses.ashx → /causes/{id}/tabs/{tab}, /causes/{id}/actions, /causes/{id}/is-used, /causes/{id}
- Notifications/Handlers/srvNotifications.ashx → /notifications, /notifications/{id}/actions, /notifications/{id}
- DataLink/Handlers/srvDatalinkBusiness.ashx → /datalink/import, /datalink/export, /datalink/jobs/{jobId}/status, /datalink/download/{id}
- LabAgree/Handlers/srvLabAgree.ashx → /lab-agree/{id}, /lab-agree/{id}/rules/{rule}
- DiningRoom/Handlers/srvDiningRoom.ashx → /dining-rooms/{id}
- SDK/Handlers/srvManagePunches.ashx → /sdk/punches, /sdk/punches/{id}
- Base/WebUserControls/handlers/srvUserFields.ashx → /ui/user-fields
- Base/WebUserControls/roCalendar/Handlers/srvCalendarImport.ashx → /ui/calendar/import

## Mapa de navegación (borrador)

- Inicio: Home/Main → menú por dominios (Employees, Scheduler, Access, Tasks, Shifts, …).
- SelectorData: patrones de selección en subpáginas (Employee/Concept/Zone/Status/…); reemplazar por Selectors estándar en UI.
- MessageBox: `srvMsgBox*.aspx` → Dialog/ConfirmDialog estándar en Emplyx.
- Cola de tareas: Alerts TasksQueue → componente común de colas para otras áreas (DataLink jobs, exportaciones).

---

## Dependencias cruzadas y relaciones

- Scheduler ↔ Employees: borrado de programaciones desde Scheduler usa `srvEmployees.ashx`.
- Access ↔ Shifts: selección/validación de turnos; copy/delete pueden depender de uso.
- Requests ↔ Security: categorías/niveles de aprobación configuradas en Security.
- Options ↔ Employees: operación de borrado de biometría masivo.
- Base: user-fields y calendar import usados en múltiples módulos.

---

## Anexo — Contratos de Adaptadores Top-20 (v1.0)

Estado: Congelado (v1.0). Se adoptan convenciones de envelope, errores y paginación.

Convenciones v1.0 (normalizadas):

- Respuesta: { ok: boolean, data?: T, meta?: { page?, size?, total?, sort?, filter? }, error?: { code: string, message: string, details?: any } }
- Errores estandar: VALIDATION_ERROR, NOT_FOUND, FORBIDDEN, UNAUTHORIZED, CONFLICT, DEPENDENCY_IN_USE, LIMIT_EXCEEDED, MALFORMED_REQUEST, BACKEND_UNAVAILABLE, TIMEOUT, UNKNOWN_ERROR
- Fechas: date "YYYY-MM-DD", datetime ISO-8601 UTC "YYYY-MM-DDTHH:mm:ssZ"
- Paginacion: query page (1..n), size (1..200), sort (campo:asc|desc[,..]), filter (expresion simple tipo clave:valor; listas separadas por coma)

1) GET /employees
- Query: page, size, sort, filter
- 200 ok: { ok:true, data:{ items:[{ id, code, name, status, groups:[{id,name}], hasForgottenRight, language }], meta:{ page, size, total, sort, filter } } }
- 400 err: { ok:false, error:{ code:"VALIDATION_ERROR", message:"filter malformed" } }

2) GET /employees/{id}
- 200: { ok:true, data:{ id, code, name, status, contacts, centers, contracts:[], permissions:[], mobility:{} } }
- 404: { ok:false, error:{ code:"NOT_FOUND", message:"employee not found" } }

3) PATCH /employees/{id}
- Body: { name?, language?, hasForgottenRight? }
- 200: { ok:true, data:{ id } }
- 409: { ok:false, error:{ code:"CONFLICT", message:"duplicate name" } }

4) DELETE /employees/{id}/biometrics
- 200: { ok:true }
- 403: { ok:false, error:{ code:"FORBIDDEN", message:"insufficient role" } }

5) DELETE /employees/{id}/programmed/{kind}/{key}
- kind: absence|incidence|holiday|overtime
- 200: { ok:true }
- 409: { ok:false, error:{ code:"DEPENDENCY_IN_USE", message:"cannot delete" } }

6) GET /employees/{id}/summary?type=accrual|cause|tasks|centers&range=YYYYQn|YYYY-MM
- 200: { ok:true, data:{ series:[], meta:{ type, range } } }

7) GET /access/groups/{id}/tabs/{tab}
- 200: { ok:true, data:{ model:{} } }
- 404: { ok:false, error:{ code:"NOT_FOUND", message:"group not found" } }

8) POST /access/groups/{id}:copy
- Body: { newName? }
- 200: { ok:true, data:{ newId } }
- 409: { ok:false, error:{ code:"CONFLICT", message:"name exists" } }

9) GET /access/periods/{id}/tabs/{tab}
- 200: { ok:true, data:{ model:{} } }

10) POST /access/periods/{id}:delete
- 200: { ok:true }
- 409: { ok:false, error:{ code:"DEPENDENCY_IN_USE", message:"period in use" } }

11) GET /access/status/{id}/tabs/{tab}
- 200: { ok:true, data:{ model:{} } }

12) GET /access/zones/{id}/tabs/{tab}
- 200: { ok:true, data:{ model:{} } }
- 200 actions: GET /access/zones/{id}/actions -> { ok:true, data:{ buttons:[] } }

13) POST /access/events/{id}:copy
- Body: { newDate: "YYYY-MM-DD" }
- 200: { ok:true, data:{ newId } }
- 400: { ok:false, error:{ code:"VALIDATION_ERROR", message:"newDate required" } }

14) GET /scheduler/moves/status/daily?employeeId&date&view
- 200: { ok:true, data:{ items:[{ time, type, source }], summary:{} } }
- 400: { ok:false, error:{ code:"VALIDATION_ERROR", message:"date invalid" } }

15) GET /scheduler/bar-buttons?context&id
- 200: { ok:true, data:{ buttons:["copy","delete",...] } }

16) POST /requests/{id}:approve
- Body: { note? }
- 200: { ok:true }
- 409: { ok:false, error:{ code:"CONFLICT", message:"state transition not allowed" } }

17) POST /requests/{id}:reject
- Body: { reason }
- 200: { ok:true }
- 400: { ok:false, error:{ code:"VALIDATION_ERROR", message:"reason required" } }

18) GET /terminals/{id}/tabs/{tab}
- 200: { ok:true, data:{ model:{} } }
- 403: { ok:false, error:{ code:"FORBIDDEN", message:"insufficient role" } }

19) POST /terminals:broadcast
- Body: { scope:"all"|"id", ids?:[] }
- 202: { ok:true, data:{ jobId } }
- 429: { ok:false, error:{ code:"LIMIT_EXCEEDED", message:"rate limit" } }

20) GET /documents/download/{id}
- 200: stream (headers: Content-Type, Content-Disposition)
- 403: { ok:false, error:{ code:"FORBIDDEN", message:"no permission" } }

---

## Anexo — DTOs y Enumeraciones (v1.0)

Objetivo: unificar estructuras mínimas para facilitar Blazor/MAUI y pruebas.

- Employee: { id:int, code:string, name:string, status:"active|inactive|locked", language:"es|en|fr|de", groups:[{ id:int, name:string }], contacts:{ email?:string, phone?:string }, centers?:string[], contracts?:any[], permissions?:any[], mobility?:any }
- Group: { id:int, name:string, type?:"employee|access|task|…" }
- AccessGroup: { id:int, name:string, zones?:number[], periods?:number[], rules?:any }
- AccessPeriod: { id:int, name:string, weekDay?:number, from?:string, to?:string, month?:number }
- AccessStatus: { id:int, name:string, main?:boolean }
- AccessZone: { id:int, name:string, type?:"room|door|gate|area|other", parentId?:number }
- SchedulerDailyItem: { time:string, type:"in|out|break|unknown", source:"punch|manual|calc", meta?:any }
- ProductiveUnit: { id:int, code?:string, name:string, summary?:any }
- Terminal: { id:int, name:string, ip?:string, port?:number, model?:string, state?:"online|offline|unknown" }
- Task: { id:int, title:string, status:"open|in_progress|completed|cancelled", assignees?:number[], dueDate?:string }
- TaskTemplate: { id:int, name:string, fields?:any[], employeesSelected?:number[] }
- Shift: { id:int, name:string, used?:boolean, rules?:{ flexible?:any, mandatory?:any, breaks?:any } }
- DocumentTemplate: { id:int, name:string, version?:string, fields?:any[] }
- Notification: { id:int, title:string, state:"draft|active|archived", audience?:"all|role|group", schedule?:{ from?:string, to?:string } }
- Request: { id:int, type:"absence|incidence|schedule|other", employeeId:int, state:"pending|approved|rejected|cancelled", createdAt:string }
- Alert: { id:int, type:"system|security|processing|custom", severity:"info|warning|error|critical", title:string, date:string, state:"open|resolved", attachments?:any[] }
- Camera: { id:int, name:string, url?:string, location?:string, state?:"online|offline|unknown" }

Enumeraciones base:
- Error.code: VALIDATION_ERROR | NOT_FOUND | FORBIDDEN | UNAUTHORIZED | CONFLICT | DEPENDENCY_IN_USE | LIMIT_EXCEEDED | MALFORMED_REQUEST | BACKEND_UNAVAILABLE | TIMEOUT | UNKNOWN_ERROR
- Common.state: active|inactive|locked ; request.state: pending|approved|rejected|cancelled ; alert.severity: info|warning|error|critical ; notification.state: draft|active|archived ; scheduler.view: day|week|month ; terminals.broadcast.scope: all|id

Notas: DTOs podrán ampliarse en 11.2–11.4 con tipos fuertes y validaciones.

---

## Anexo — Catálogo de errores por dominio (v1.0)

Convención: code estable, message para usuario (localizable), details opcional para depuración. HTTP sugerido entre paréntesis.

- Employees
  - EMPLOYEE_NOT_FOUND (404)
  - EMPLOYEE_DUPLICATE_NAME (409)
  - EMPLOYEE_FORBIDDEN (403)
  - EMPLOYEE_PROGRAMMED_DEPENDENCY (409)
  - EMPLOYEE_VALIDATION (400)
- Access
  - ACCESS_GROUP_NOT_FOUND (404)
  - ACCESS_PERIOD_IN_USE (409)
  - ACCESS_ZONE_IN_USE (409)
  - ACCESS_VALIDATION (400)
- Scheduler
  - SCHED_DATE_INVALID (400)
  - SCHED_EMPLOYEE_REQUIRED (400)
  - SCHED_VIEW_INVALID (400)
- Requests
  - REQUEST_INVALID_TRANSITION (409)
  - REQUEST_REASON_REQUIRED (400)
- Terminals
  - TERMINAL_NOT_FOUND (404)
  - TERMINAL_BROADCAST_RATE_LIMIT (429)
  - TERMINAL_FORBIDDEN (403)
- Documents
  - DOC_TEMPLATE_IN_USE (409)
  - DOC_DOWNLOAD_FORBIDDEN (403)
- Alerts
  - ALERT_NOT_FOUND (404)
  - ALERT_DOWNLOAD_EXPIRED (410)
- General
  - VALIDATION_ERROR (400), NOT_FOUND (404), FORBIDDEN (403), UNAUTHORIZED (401), CONFLICT (409), DEPENDENCY_IN_USE (409), LIMIT_EXCEEDED (429), MALFORMED_REQUEST (400), BACKEND_UNAVAILABLE (502|503), TIMEOUT (504), UNKNOWN_ERROR (500)

Ejemplos localizados vs técnicos:

- Localizado (es-ES): { ok:false, error:{ code:"FORBIDDEN", message:"No tienes permisos para realizar esta acción", details:{ locale:"es-ES" } } }
- Técnico (log): { ok:false, error:{ code:"DEPENDENCY_IN_USE", message:"Shift is referenced by schedule entries", details:{ id:234 } } }


---

## Anexo — Permisos RBAC/ABAC y Gating por licencia (v0)

Convenciones: acciones por recurso y roles mínimos; condiciones ABAC comunes (tenant/empresa/unidad) y gating por edición/licencia.

- Employees:
  - Acciones: list, read, update, deleteBiometrics, deleteProgrammed.
  - Roles mínimos: supervisor (list/read), hr_manager (update), security_admin (deleteBiometrics).
  - ABAC: scope empresa/unidad; acceso a empleados del grupo/supervisión.
  - Licencia: módulos avanzados (summary extendido).
- Access:
  - Acciones: read tabs/actions, delete/copy/empty.
  - Roles: access_admin; ABAC por centro/instalación.
  - Licencia: monitor en tiempo real.
- Scheduler:
  - Acciones: read status, moves ops (según rol), wizards.
  - Roles: planner/supervisor; ABAC por unidad/grupo.
- Security:
  - Acciones: passports CRUD, features set/default, security options CRUD.
  - Roles: security_admin; ABAC restricto por tenant.
- Terminals:
  - Acciones: read tabs/actions, delete, broadcast.
  - Roles: devices_admin; broadcast requiere elevación.
- Tasks/TaskTemplates:
  - Acciones: read/update/complete, templates CRUD/copy/delete.
  - Roles: ops_manager/supervisor.
- Shifts/Documents/Options/Indicators/… seguir patrón similar; ver `Emplyx_Guia.md` para principios.

---

## Anexo — Contratos complementarios (v1.0, adicionales)

- GET /alerts … ; GET /alerts/{id} … ; GET /alerts/tasks-queue … (ver sección Alerts)
- GET /cameras/{id}/tabs/{tab} … ; DELETE /cameras/{id}
- GET /scheduler/units/{id}/tabs/{tab} … ; GET /scheduler/units/{id}/summary
- GET /task-templates/{id}/tabs/{tab} … ; GET /task-projects/{id}/tabs/{tab} …
- GET /indicators/{id}/tabs/{tab} … ; DELETE /indicators/{id}
- GET /concepts/{id}/actions ; DELETE /concepts/{id} ; GET /concept-groups/{id}/actions ; DELETE /concept-groups/{id}
- GET /causes/{id}/tabs/{tab} … ; GET /causes/{id}/is-used ; DELETE /causes/{id}
- GET /notifications?… ; GET /notifications/{id}/actions ; DELETE /notifications/{id}
- POST /datalink/import|export { config } → { jobId } ; GET /datalink/jobs/{jobId}/status ; GET /datalink/download/{id}

Errores HTTP sugeridos:
- 200 OK, 201 Created, 202 Accepted, 204 No Content
- 400 Bad Request (VALIDATION_ERROR, MALFORMED_REQUEST), 401 Unauthorized, 403 Forbidden, 404 Not Found, 409 Conflict (CONFLICT, DEPENDENCY_IN_USE), 429 Too Many Requests (LIMIT_EXCEEDED)
- 500 Internal Server Error (UNKNOWN_ERROR), 502 Bad Gateway/ 503 Service Unavailable (BACKEND_UNAVAILABLE), 504 Gateway Timeout (TIMEOUT)

---

## Anexo — Selectores y Descargas (patrones comunes)

- SelectorData.aspx → componente Selector (búsqueda con filtros, paginación, multi‑selección). Contrato: GET /selectors/{entity}?filter=&page=&size=
- downloadFile.aspx → patrón de descarga: GET /{dominio}/download/{id}; headers estándar, registro de auditoría; manejo de expiración.

---

## Anexo — i18n, tiempo y formatos (v0)

- Uso actual de Globalize.formatMessage → mapear a recursos resx centralizados.
- Fechas en UI: zona configurable por tenant/empresa; almacenar en UTC; mostrar en zona local.
- Números/monedas (si aplica) con cultura de usuario; validaciones coherentes.

---

## Anexo — Rendimiento, colas y límites (v0)

- Paginación por defecto size=50; máximo 200.
- Throttling en endpoints intensivos (broadcast, imports/exports, analytics); backoff exponencial en UI.
- Jobs (DataLink, broadcasts): polling con `jobs/status` + TasksQueue reutilizable.

---

## Anexo — Preguntas abiertas (para cierre de inventario)

1) Listado oficial de tabs (aTab) por módulo para tipar UI y evitar valores “mágicos”.
2) Catálogo de roles/funciones finales por dominio (mapeo RBAC a acciones UI y a endpoints).
3) Errores localizados vs técnicos: estrategia (i18n de mensajes de usuario, códigos técnicos estables).
4) Descargas con expiración y firmas: ¿cabeceras específicas requeridas? ¿formatos (csv/pdf/zip)?
5) Políticas de retención de auditoría/logs por tenant y export firmado.
6) Límite de tamaño de import/export y validaciones previas (pre‑flight checks).
7) Estrategia SSO/identidad para Emplyx (OIDC/OAuth2) en fase de transición.

---

## Anexo — Checklist de paridad por módulo (v0)

Para cada módulo, verificar como mínimo:

- Navegación/tabs equivalentes y accesibles según permisos.
- Filtros y paginación; búsquedas y ordenación.
- Acciones CRUD y masivas; confirmaciones y mensajes de error/success.
- Descargas/exports y colas de tareas (si aplica).
- Validaciones de UI (requeridos, rangos, formatos) y estados de grilla.
- Gating por licencia y RBAC/ABAC aplicado; auditoría.
- Rendimiento aceptable (tiempos objetivos acordados) y accesibilidad AA.

Sugerido: mantener checklist por módulo en issue/tracker vinculado a este inventario.

---

## Anexo — Plan de UAT y pruebas (v0)

Estrategia:

- Pruebas por módulo con casos prioritarios de negocio; smoke de navegación; pruebas negativas (errores/forbidden).
- Pruebas de rendimiento UX (primer render, filtrados, acciones típicas); pruebas de accesibilidad.
- Pruebas de seguridad (roles mínimos, ABAC por tenant/empresa/unidad; descargas con auditoría).

Estructura de casos (por módulo):

- Lista básica (filtros, paginación, detalle, tabs).
- Acciones (crear/editar/borrar/copia/empty/etc.).
- Descargas/exports/colas.
- Validaciones (campos, mensajes, bloqueos en edición).
- Permisos/licencias (visibilidad/acciones habilitadas, gating correcto).

---

## Anexo — Plan de cutover y convivencia (v0)

Fases:

1) Habilitar módulos en Emplyx gradualmente (feature flags), con rutas paralelas a legacy.
2) Redirecciones desde legacy a nueva UI por módulo terminado; fallback a legacy ante incidentes.
3) Migración de bookmarks/enlaces; comunicación a usuarios.
4) Cierre de legacy (por módulo) tras UAT y estabilización; congelamiento de cambios legacy.

Requisitos:

- Monitoreo y observabilidad desde el día 0; métricas de uso y errores.
- Plan de rollback por módulo; checklist de activación.

## Detalle - Alerts (v0)

Páginas ASPX principales:

- Alerts.aspx, AlertsDetail.aspx, TasksQueue.aspx, downloadFile.aspx

Scripts detectados:

- Alerts/Scripts/Alerts.js, Alerts/Scripts/TasksQueue.js

Patrones AJAX/WebForms:

- JS usa $.ajax (acciones no visibles en recorte); patrón de cola de tareas y descarga de archivos.

Propuesta de contratos JSON v0.1 (Emplyx.ApiAdapter):

- GET /alerts?filter=...&page=&size= -> { items:[{id,type,severity,title,date,state}], total }
- GET /alerts/{id} -> { id, title, description, severity, createdAt, source, attachments:[...] }
- GET /alerts/tasks-queue -> { items:[{id,type,status,progress}] }
- GET /alerts/download/{id} -> stream (autenticado, auditado)

Validaciones observadas (parcial):

- Filtros obligatorios cuando aplique; confirmaciones para acciones en cola; control de descargas.

---

## Detalle - Cameras (v0)

Páginas ASPX principales:

- Cameras.aspx, CameraSelectorData.aspx, srvMsgBoxCameras.aspx

Scripts detectados:

- Cameras/Scripts/Camera.js

Patrones AJAX/WebForms:

- srvCameras.ashx: getCameraTab(aTab,ID), getBarButtons(ID), deleteCamera(ID)

Parámetros detectados por acción (desde JS):

- srvCameras.ashx
  - action=getCameraTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=deleteCamera | params: ID

Propuesta de contratos JSON v0.1:

- GET /cameras/{id}/tabs/{tab} -> { model }
- GET /cameras/{id}/actions -> { buttons:[...] }
- DELETE /cameras/{id} -> { ok }

Validaciones observadas (parcial):

- Confirmación para borrado; dependencias y permisos.

---

## Detalle - AIScheduler (v0)

Páginas ASPX principales:

- ProductiveUnit.aspx, Budget.aspx, ProductiveUnitSelectorData.aspx, srvMsgBoxAIScheduler.aspx

Scripts detectados:

- AIScheduler/Scripts/AIScheduler.js, Budgets.js, DateSelector.js, ProductiveUnit.js

Patrones AJAX/WebForms:

- srvProductiveUnit.ashx: getProductiveUnitTab(aTab,ID), getBarButtons(ID), deleteProductiveUnit(ID), GetSummary(ID,Range), DrawSummary(ID,Range)

Parámetros detectados por acción (desde JS):

- srvProductiveUnit.ashx
  - action=getProductiveUnitTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=deleteProductiveUnit | params: ID
  - action=GetSummary | params: ID, Range
  - action=DrawSummary | params: ID, Range

Propuesta de contratos JSON v0.1:

- GET /scheduler/units/{id}/tabs/{tab} -> { model }
- GET /scheduler/units/{id}/actions -> { buttons:[...] }
- DELETE /scheduler/units/{id} -> { ok }
- GET /scheduler/units/{id}/summary?range=... -> { series:[...], meta:{...} }

Validaciones observadas (parcial):

- Rangos válidos en resúmenes; confirmaciones para eliminar.

---

## Detalle - Wizards (v0)

Páginas ASPX principales:

- EmergencyReports.aspx, NewTerminalWizard.aspx

Patrones y notas:

- Uso intensivo de __doPostBack por pasos; los wizards delegan en endpoints del dominio (Cámaras, Terminales, etc.).

Propuesta:

- Estándar de wizards en UI (Blazor) con validaciones por paso, resumen final y confirmación; reusar adaptadores del dominio.

---

## Detalle - Diagnostics (v0)

Páginas ASPX principales:

- Status.aspx, DownloadLogs.aspx

Scripts detectados:

- Diagnostics/Scripts/diagnostics.js

Propuesta de contratos JSON v0.1:

- GET /diagnostics/status -> { services:[{name,state,details}] }
- GET /diagnostics/download-logs?component=... -> stream

Validaciones observadas (parcial):

- Permisos para lectura de estado/descargas.

---

## Detalle - Audit (v0)

Páginas ASPX principales:

- Audit.aspx

Scripts detectados:

- Audit/Scripts/audit.js

Propuesta de contratos JSON v0.1:

- GET /audit?filter=...&page=&size= -> { items:[{when,who,action,entity,entityId}], total }
- GET /audit/{id} -> { record:{...} }

Validaciones observadas (parcial):

- Filtros por rango/usuario/acción; compliance de retención.

---

## Detalle - VTLogin/Auth (v0)

Páginas ASPX principales:

- VTLogin: Default.aspx, VTPortalLogin.aspx
- Auth: VTCheckAuth.aspx, VTLiveAuth.aspx, VTPortalAuth.aspx

Propuesta (UI nueva):

- Login Blazor que llama a Emplyx.ApiAdapter:
  - POST /auth/login { username, password } -> { ok, token?|cookie? }
  - GET /auth/sso/check -> { ok, redirect?|tenant? }
- Emplyx.ApiAdapter actúa como black-box ante las páginas ASPX actuales durante transición.

Notas:

- Respetar sesiones/cookies actuales; plan para OIDC/OAuth2 futuro sin cambiar legacy en esta fase.

---

## Detalle - VTCheckSSO (v0)

Páginas ASPX principales:

- VTCheckSSO/Default.aspx

Propuesta:

- Exponer SSO check via adapter: GET /auth/sso/check y redirecciones según resultado; UI muestra estados claros.

---

## Detalle - Emergency (v0)

Páginas ASPX principales:

- Emergency.aspx, srvEmergency.aspx, srvMsgBoxEmergency.aspx

Scripts detectados:

- Emergency/Scripts/Emergency.js

Propuesta de contratos JSON v0.1:

- GET /emergency -> { model }
- POST /emergency/actions/{action} { payload } -> { ok }

Validaciones observadas (parcial):

- Confirmaciones para acciones críticas; permisos especiales.

---

## Detalle - Assignments (v0)

Páginas ASPX principales:

- Assignments.aspx, AssignmentsSelectorData.aspx, srvMsgBoxAssignment.aspx

Scripts detectados:

- Assignments/Scripts/Assignments.js

Patrones AJAX/WebForms:

- srvAssignments.ashx: getAssignmentsTab(aTab,ID), getBarButtons(ID), deleteAssignment(ID)

Parámetros detectados por acción (desde JS):

- srvAssignments.ashx
  - action=getAssignmentsTab | params: aTab, ID
  - action=getBarButtons | params: ID
  - action=deleteAssignment | params: ID

Propuesta de contratos JSON v0.1:

- GET /assignments/{id}/tabs/{tab} -> { model }
- GET /assignments/{id}/actions -> { buttons:[...] }
- DELETE /assignments/{id} -> { ok }
