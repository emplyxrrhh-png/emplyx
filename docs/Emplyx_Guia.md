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
