# Gestor Documental para RRHH — Arquitectura y Diseño

## 1. Visión y Alcance
- Centralizar documentos de RRHH (empleado y corporativos), con control de acceso, trazabilidad y cumplimiento.
- Cubrir ciclo de vida: carga → validación → publicación/consumo → versionado → archivado/retención.
- Garantizar seguridad, auditoría y experiencia usable para empleados y RRHH.

## 2. Actores y Roles
- Empleado: carga documentos propios, consulta lo publicado para él o general.
- RRHH (admin/con revisor): revisa, aprueba/rechaza, publica documentos individuales o generales, configura plantillas/categorías, permisos y notificaciones.
- Supervisor/Manager (opcional): puede ver documentos de su área si la política lo permite.
- Sistema externo (nóminas/portal empleado): consume o publica documentos vía API.

## 3. Clasificación y Permisos
- Niveles: Público / Interno / Confidencial / Alto Secreto.
- Ámbitos: Empleado, Empresa/General, Bajas, Otros (extensible).
- Modos de control:
  - Por categoría/carpeta segura (segregación de almacenamiento).
  - Por documento (ACL granular: usuarios/grupos).
  - Permiso “solo autor” hasta validación.
- Auditoría obligatoria en todas las acciones de lectura/descarga y cambio.

## 4. Organización y Metadatos
- Carpetas/categorías personalizadas + tags.
- Metadatos mínimos: título, tipo (plantilla), empleado, categoría, nivel de sensibilidad, fechas (subida, revisión, expiración opcional), estado (Pending/Validated/Rejected/Published), revisor, motivo rechazo, versión, firma/validación, usuario alta, comentarios/tags. Campos custom por empresa.

## 5. Flujos Clave
- Carga empleado: sube archivo + metadatos → estado Pending → notifica a RRHH → RRHH valida/rechaza → notifica al empleado → si aprobado, visible según permisos.
- Publicación RRHH:
  - General: publica a todos o grupos; visible en portal empleado.
  - Individual: asocia a empleado/expediente; visible a empleado y RRHH.
  - Carga masiva soportada.
- Versionado: nueva versión crea entrada de histórico; posibilidad de restaurar; numeración v1, v2, …
- Descarga/lectura: control de acceso + auditoría de acceso.
- Vencimientos: recordatorios y cambio de estado a Expired si aplica.
- Firma electrónica: solicitud, seguimiento de estado (Pending/Signed/Rejected), almacenamiento de evidencias.

## 6. Búsqueda e Indexación
- Búsqueda por metadatos (tipo, estado, categoría, fechas, empleado, sensibilidad, tags).
- Full-text y OCR opcional para PDF/imagen (indexación de contenido).
- Filtros de vencimiento, por usuario, por ámbito; resultados ordenables y exportables.

## 7. Requisitos No Funcionales
- Seguridad: cifrado en tránsito (TLS) y en reposo; claves gestionadas; hardening de buckets; política de contraseñas/SSO.
- Cumplimiento: GDPR/LOPDGDD (consentimiento, minimización, derecho al olvido, retención); auditoría inmutable.
- Escalabilidad: almacenamiento horizontal (blob storage), API escalable; límites de tamaño de archivo configurables.
- Rendimiento: subidas y descargas eficientes; paginación en listados y búsqueda.
- Disponibilidad: ≥99.5% objetivo; backups y DR.
- Observabilidad: logs estructurados, métricas (latencia, errores, tasa de descarga), trazas; paneles y alertas.

## 8. Arquitectura Lógica
- Frontend Portal (Empleado / RRHH):
  - Empleado: carga, consulta, descarga, firma, notificaciones, búsqueda.
  - RRHH: revisión/aprobación, publicación, gestión de plantillas/categorías/permisos, reportes.
- API Gateway / Backend:
  - Autenticación/Autorización (SSO/OIDC/JWT; roles/ACL).
  - Módulo Documentos: CRUD, versionado, metadatos, descarga segura (URLs firmadas/streaming).
  - Módulo Revisión/Flujos: estados, aprobaciones, motivos de rechazo.
  - Módulo Búsqueda/Indexación: metadatos + full-text/OCR (servicio externo o motor interno).
  - Módulo Notificaciones: eventos a email/in-app/push; recordatorios de vencimiento.
  - Módulo Firma: integración con proveedor de firma (gestión de solicitud/estado/evidencias).
  - Auditoría y Reporting.
- Storage:
  - Blob storage segregado por sensibilidad/ámbito (buckets/containers).
  - Base de datos relacional para metadatos, versiones, auditoría; caché opcional.
  - Índice de búsqueda (si full-text/OCR).
- Integraciones:
  - Nóminas / Portal Empleado / Email / Firma electrónica / OCR.

## 9. Modelo de Datos (entidades base)
- Document(id, title, type_id, employee_id?, category_id?, scope, sensitivity, status, version, current_blob, created_at/by, reviewed_at/by, rejection_reason, expires_at?, sign_status, sign_evidence_ref?, tags, custom_fields JSON).
- DocumentVersion(id, document_id, version, blob_ref, checksum, created_at/by, change_note).
- DocumentTemplate(id, name, type_code, required_metadata, allowed_scopes, default_sensitivity, notification_rules, sign_required?, retention_policy).
- Category/Folder(id, name, parent_id?, scope, sensitivity_default).
- AccessControl(id, document_id or category_id, principal (user/group/role), permission (read/download/write/admin)).
- AuditLog(id, actor, action, target_type/id, timestamp, metadata (ip, user_agent, result)).
- Notification(id, event, target_user/group, channel, status, payload).
- SignRequest(id, document_id/version_id, provider, status, evidence_ref, requested_at, completed_at).

## 10. Contratos API (ejemplos simplificados)
- POST `/documents` (multipart o pre-signed upload): crear doc con metadatos; respuesta incluye `document_id`, `version`, `upload_url` si aplica.
- POST `/documents/{id}/versions`: nueva versión sobre doc existente.
- POST `/documents/{id}/review`: body `{action: approve|reject, reason?}`.
- GET `/documents` con filtros (scope, employee, status, category, sensitivity, text, date ranges, tags); paginado.
- GET `/documents/{id}`: detalle + versiones + permisos efectivos.
- GET `/documents/{id}/download` → URL firmada/streaming; registra auditoría.
- POST `/documents/{id}/sign` → inicia firma; GET `/sign-requests/{id}` para estado.
- Admin: CRUD de plantillas, categorías, permisos, reglas de notificación.

## 11. Seguridad
- AuthN: OIDC/SSO; tokens de acceso cortos; refresh controlado.
- AuthZ: roles + ACL por documento/carpeta; políticas por sensibilidad/ámbito; managers restringidos a su unidad.
- Datos: cifrado en reposo; URLs firmadas temporales para descarga/subida; verificación de checksum; AV scan opcional.
- Auditoría: todas las lecturas/descargas y cambios; logs inmutables/append-only.
- Retención y borrado: políticas por tipo/plantilla; borrado seguro; bloqueo legal si aplica.

## 12. Notificaciones y Eventos
- Triggers: subida (empleado), pendiente de revisar (RRHH), recordatorios, resultado revisión, nuevo doc publicado (general/individual), vencimiento, solicitud/resultado de firma.
- Canales: email, in-app, opcional push.
- Config por plantilla/tipo y por usuario/grupo.

## 13. Despliegue y Operación
- Contenedores para API/servicios; almacenamiento gestionado (blob, DB, search).
- Entornos: dev/qa/prod con config por entorno (buckets/keys/proveedores firma/OCR).
- CI/CD: tests, escaneo de seguridad, deploy automatizado.
- Backups DB + blobs; pruebas de restauración periódicas.

## 14. Observabilidad
- Logs estructurados (json) con correlación de request-id/user-id/doc-id.
- Métricas: tasa de subida/descarga, latencia, errores por endpoint, tasa de revisión, expiraciones.
- Alertas: errores 5xx, tiempo de respuesta, fallos de firma/OCR, colas de notificación atascadas.
- Trazas distribuidas para flujos de subida/descarga/firma.

## 15. Riesgos y Supuestos
- Supuesto: hay IdP corporativo (SSO) y directorio de empleados integrado.
- Riesgo: cumplimiento de retención/expurgo si no se parametriza por tipo; mitigar con políticas en plantillas.
- Riesgo: tamaño de ficheros grandes → usar carga fragmentada/presigned URLs.
- Riesgo: OCR puede ser costoso → habilitar por tipo/volumen, con colas async.

## 16. Backlog / Roadmap (incluye gaps de VisualTime)
1) Versionado completo:
   - Campo `Version` en `roDocument`; entidad de histórico; endpoint de nueva versión; restaurar versión.
2) Auditoría de lectura/descarga:
   - En `roDocumentManager`, auditar `download/view`; logs estructurados.
3) Notificaciones granulares:
   - Disparadores por evento (subida, pendiente, aprobado, rechazado, vencimiento, nuevo publicado); plantilla y preferencia de canal.
4) Búsqueda/OCR:
   - Integrar indexación de metadatos + full-text; OCR para PDF/imagen; endpoints de búsqueda avanzada.
5) Firma electrónica:
   - Flujo completo con proveedor; estados `Pending/Signed/Rejected`; evidencias almacenadas.
6) Frontend Portal:
   - Vistas Empleado (mis docs, generales, carga, firma, notificaciones) y RRHH (revisión, publicación, plantillas, permisos).
7) Gestión de plantillas/categorías/permisos:
   - UI + APIs; reglas de sensibilidad/retención/notificación por plantilla.
8) Retención/expurgo:
   - Políticas por tipo/plantilla; jobs de expiración/expurgo con auditoría.
9) Integraciones externas:
   - Nóminas/Portal Empleado: publicar nóminas y docs; single sign-on.
10) Observabilidad y operativa:
    - Métricas, trazas, alertas; backups/DR; hardening de storage.

## 17. Checklist de entrega (para cada incremento)
- Autorización y auditoría implementadas en nuevos endpoints.
- Pruebas de subida/descarga/versionado/flujo de revisión.
- Notificaciones verificadas en sandbox.
- Policies de retención y permisos por sensibilidad comprobadas.
- Logs y métricas visibles en observabilidad.


C