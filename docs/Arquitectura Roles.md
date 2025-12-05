# Arquitectura de Roles y Seguridad

## 1. Visión General
El modelo de seguridad de Emplyx se basa en un enfoque híbrido de **RBAC (Role-Based Access Control)** y **ABAC (Attribute-Based Access Control)**, diseñado para soportar una arquitectura multi-tenant jerárquica. El objetivo es garantizar el principio de menor privilegio, permitiendo flexibilidad granular y auditoría completa.

## 2. Catálogo de Roles Estándar

| Rol | Descripción | Alcance Típico |
| :--- | :--- | :--- |
| **Empleado** | Usuario base. Autoservicio para fichajes, consultas propias y solicitudes. | Datos propios (`Self`). |
| **Supervisor / Planner** | Responsable de equipo. Gestión operativa, aprobación de solicitudes y turnos. | Unidad organizativa o Grupo asignado. |
| **RRHH (HR Manager)** | Gestión integral de personas y organización. Altas/bajas, convenios, informes. | Empresa o Corporación. |
| **Security Admin** | Administración técnica. Configuración de tenant, usuarios, roles y auditoría. | Tenant completo. |
| **Auditor** | Solo lectura. Revisión de cumplimiento y trazas. | Transversal (según asignación). |
| **Partner** | Colaborador externo. Gestión delegada limitada. | Acotado por contrato/servicio. |
| **Administrador WEB** | Super-admin de la plataforma (SaaS). Gestión de tenants y licencias. | Global (Multi-tenant). |

## 3. Modelo de Permisos (RBAC + ABAC)

### 3.1. Scopes (Acciones)
Los permisos se definen por recurso y acción. Las acciones estándar son:
*   `read`: Consultar datos.
*   `create`: Crear nuevos registros.
*   `update`: Modificar existentes.
*   `delete`: Eliminar (lógico o físico).
*   `execute`: Ejecutar procesos (ej. recálculo de saldos).
*   `export`: Exportar datos masivos.
*   `approve`: Aprobar/Rechazar solicitudes.
*   `configure`: Modificar configuraciones estructurales.

### 3.2. Atributos (ABAC)
Para refinar el acceso más allá del rol, se aplican reglas basadas en atributos:
*   **Jerarquía**: Tenant, Corporación, Empresa.
*   **Organizativo**: Centro de Coste, Unidad, Grupo.
*   **Geográfico**: País, Zona, Ubicación física.
*   **Temporal**: Turno, Horario.

### 3.3. Reglas de Resolución
1.  **Deny > Allow**: Una denegación explícita siempre prevalece sobre un permiso.
2.  **Herencia**: Los permisos se heredan en la jerarquía organizativa (un admin de Corporación es admin de sus Empresas).
3.  **Granularidad**: Posibilidad de definir permisos a nivel de **campo** para datos sensibles (ej. salario, datos biométricos).

### 3.4. Interacción Rol-Permiso (Desacoplamiento)
Para permitir la creación de roles dinámicos sin modificar el código, el sistema utiliza **Autorización Basada en Permisos (Permission-Based Authorization)** en lugar de verificar roles directamente.

1.  **El Código verifica Permisos (Policies):**
    *   El código fuente (Controladores, Vistas, Servicios) **NUNCA** verifica el nombre del rol (ej. `if (User.IsInRole("RRHH"))` es una mala práctica).
    *   El código verifica la **capacidad** o permiso atómico: `[Authorize(Policy = "Employees.Create")]` o `if (AuthService.HasPermission("Vacations.Approve"))`.
    *   Estos permisos ("Employees.Create", "Vacations.Approve") son constantes definidas en el sistema y representan las unidades funcionales indivisibles.

2.  **La Base de Datos mapea Roles a Permisos:**
    *   Existe una tabla intermedia `RolePermissions` que vincula un `RolId` con múltiples `PermissionId`.
    *   Un Rol es simplemente un **contenedor** o agrupación de permisos.

3.  **Creación de Roles Dinámicos:**
    *   Al crear un nuevo rol en la UI (ej. "Asistente de Nómina"), el administrador selecciona de la lista de permisos disponibles cuáles asignar a este nuevo rol.
    *   Como el código sigue verificando los mismos permisos atómicos, el nuevo rol funciona inmediatamente sin necesidad de recompilar ni desplegar.

4.  **Ejemplo:**
    *   *Código:* Requiere permiso `Reports.ViewSalary`.
    *   *Rol Existente (RRHH):* Tiene `Reports.ViewSalary`. -> Acceso permitido.
    *   *Nuevo Rol (Auditor Externo):* Se crea en BD y se le asigna `Reports.ViewSalary`. -> Acceso permitido automáticamente.

### 3.5. Contexto y Jerarquía de Aplicación (Scoping)
El rol define **QUÉ** puede hacer el usuario, pero el Contexto define **DÓNDE** puede hacerlo. Un usuario no "es" Supervisor en abstracto, sino "Supervisor del Grupo A" o "RRHH de la Empresa X".

1.  **Niveles de Contexto (Jerarquía):**
    El sistema maneja una jerarquía estricta para la propagación de permisos:
    *   **Global / System**: (Solo Super-Admins) Afecta a toda la plataforma.
    *   **Tenant**: Afecta a toda la suscripción del cliente.
    *   **Corporación**: Agrupación de empresas (opcional).
    *   **Empresa**: Entidad legal principal.
    *   **Centro de Trabajo**: Ubicación física o lógica dentro de una empresa.
    *   **Grupo / Departamento**: Unidad organizativa de empleados.

2.  **Asignación de Roles con Contexto:**
    En la base de datos, la relación no es `Usuario <-> Rol`, sino `Usuario <-> Rol <-> Contexto (EntidadId, TipoEntidad)`.
    *   *Ejemplo:* Usuario Juan es `RRHH` en `Empresa: Acme S.A.`
    *   *Ejemplo:* Usuario Ana es `Supervisor` en `Grupo: Desarrollo`.

3.  **Herencia Descendente (Cascada):**
    Los permisos otorgados en un nivel superior se heredan automáticamente en los niveles inferiores.
    *   Si Juan es `RRHH` en `Empresa: Acme`, automáticamente tiene permisos de RRHH sobre todos los `Centros de Trabajo` y `Grupos` que pertenezcan a Acme.
    *   Si Ana es `Supervisor` en `Grupo: Desarrollo`, solo ve a los empleados de ese grupo.

4.  **Resolución en Tiempo de Ejecución:**
    Cuando se solicita acceso a un recurso (ej. "Ver Ficha de Empleado X"), el sistema verifica:
    1.  ¿Tiene el usuario el permiso `Employees.Read`? (Vía Rol).
    2.  ¿Pertenece el Empleado X a un contexto (Empresa/Grupo) sobre el cual el usuario tiene ese rol asignado?

## 4. Implementación Técnica

### 4.1. Estructura de Datos (Backend)
La implementación se encuentra distribuida en los proyectos de Dominio e Infraestructura, reflejando la arquitectura ternaria.

*   **Entidad `Usuario`** (`Emplyx.Domain.Entities.Usuarios.Usuario.cs`):
    *   Raíz del agregado. Gestiona la colección de roles.
    *   Método `AssignRol(Guid rolId, Guid? contextoId)`: Lógica de negocio para asignar roles. Valida duplicados considerando el contexto.
    *   Método `RemoveRol(Guid rolId, Guid? contextoId)`: Elimina una asignación específica.

*   **Entidad `UsuarioRol`** (`Emplyx.Domain.Entities.Usuarios.UsuarioRol.cs`):
    *   Tabla intermedia (Join Entity) que materializa la relación.
    *   **Clave Primaria Compuesta**: `UsuarioId` + `RolId` + `ContextoId`.
    *   Propiedad `ContextoId` (Guid): Define el alcance. Si es `Guid.Empty`, se considera un rol global (Tenant-wide).

*   **Entidad `Rol`** (`Emplyx.Domain.Entities.Roles.Rol.cs`):
    *   Define el catálogo de roles disponibles (Nombre, Descripción, IsSystem).
    *   Actúa como contenedor de `Permisos`.

*   **Entidad `Permiso`** (`Emplyx.Domain.Entities.Permisos.Permiso.cs`):
    *   Define la capacidad atómica (ej. `EMPLOYEES.READ`).
    *   Utilizado por las Policies de autorización.

*   **Persistencia (EF Core)** (`UsuarioRolConfiguration.cs`):
    *   Mapea la entidad `UsuarioRol` a la tabla `UsuarioRoles`.
    *   Configura la PK compuesta para permitir múltiples asignaciones del mismo rol en diferentes contextos.

*   **Contratos (DTOs)** (`UsuarioContracts.cs`):
    *   `UsuarioRolDto`: Expone `RolId` y `ContextoId` (nullable) para que el frontend pueda renderizar el alcance de cada rol asignado.

### 4.2. Gestión de Sesión y Tokens
*   **JWT Claims**: El token debe incluir un claim `token_version` que es un hash de `SecurityVersion` (usuario) + `RoleVersion` (roles asignados).
*   **Invalidación en Caliente**:
    *   Al cambiar permisos, se actualiza `SecurityVersion` en BD.
    *   El middleware verifica `token_version` contra la caché/BD.
    *   Si difieren, se fuerza un refresco de token o re-login transparente, actualizando la UI (Blazor/MAUI) sin desconectar al usuario si es posible.

### 4.3. Caché y Rendimiento
*   **SecurityDirectoryCache**: Caché distribuida (Redis/Memory) por usuario (TTL ~5 min) que almacena el grafo de permisos resuelto.
*   Minimiza round-trips a BD en cada evaluación de autorización (`CanActivate`, `AuthorizeView`).

### 4.4. Auditoría
*   Todo cambio en roles o asignaciones genera un evento de auditoría inmutable.
*   Registro de "quién otorgó qué permiso a quién y cuándo".

## 5. Funcionalidades de Gestión (UI)

### 5.1. Consola de Administración (`/admin/roles`)
*   **Lista de Usuarios**: Acciones masivas (asignar rol, desactivar).
*   **Detalle de Usuario (`UsuarioDetailPanel`)**:
    *   Tabs: Perfil, Seguridad, Roles, Contextos.
    *   Dual-list para asignación de roles (`Disponibles` vs `Asignados`).
    *   Visualización de permisos efectivos (calculados).
*   **Gestión de Roles**:
    *   Crear/Duplicar roles personalizados.
    *   Definir permisos por árbol de funcionalidades.
    *   Bloqueo de edición para roles de sistema.

### 5.2. Delegación y Suplantación (Impersonation)
*   Permite a un usuario (ej. Admin) actuar en nombre de otro o cambiar de contexto temporalmente.
*   **Flujo**: Seleccionar usuario destino -> Definir alcance/tiempo -> Activar.
*   **Telemetría**: Eventos `ContextSwitchRequested`, `DelegationActivated`.

### 5.3. Workflow: Creación de un Rol Personalizado
El sistema permite a los administradores definir nuevos roles componiendo permisos existentes sin intervención de desarrollo.

1.  **Inicio**: Acceder a `Configuración > Seguridad > Roles`.
2.  **Definición Básica**:
    *   Nombre único (ej. "Gestor de Ausencias").
    *   Descripción funcional.
    *   *Opcional*: Clonar desde un rol existente (ej. copiar "Supervisor" y quitar permisos de "Evaluación").
3.  **Selección de Permisos (Permission Picker)**:
    *   Interfaz de árbol jerárquico: `Módulo -> Funcionalidad -> Acción`.
    *   Ejemplo: `Tiempos -> Solicitudes -> Aprobar`.
    *   Selectores de granularidad: `Read-Only`, `Full Access`, o selección manual de checkboxes.
4.  **Restricciones de Campo (Field Level Security)**:
    *   Para permisos de lectura/edición, se pueden definir máscaras sobre campos sensibles (ej. "Ver Ficha" pero ocultar "Salario").
5.  **Publicación**:
    *   Al guardar, el rol se genera con un `RoleId` nuevo.
    *   Inmediatamente disponible para ser asignado a usuarios en la pantalla de "Usuarios".

## 6. Referencias
*   `docs/Emplyx_Guia.md`: Sección 2 y 17.
*   `docs/Inventario 11.1 Emplyx.md`: Inventario de controles y requisitos de seguridad.

## 7. Roadmap de Desarrollo (TODOs)

### Backend
- [x] **Base de Datos**: Generar y aplicar migración EF Core para reflejar los cambios en `UsuarioRoles` (PK compuesta + columna `ContextoId`).
- [x] **API**: Actualizar endpoints de asignación de roles (ej. `POST /api/users/{id}/roles`) para aceptar `ContextoId` opcional en el cuerpo de la petición.
- [ ] **Seguridad**: Implementar o actualizar el `AuthorizationHandler` para que sea "Context-Aware". Debe verificar que el usuario tenga el permiso requerido **Y** que el contexto actual coincida con el alcance del rol asignado.
- [ ] **Tests**: Crear pruebas unitarias para `Usuario.AssignRol` verificando la unicidad de la terna Usuario-Rol-Contexto.

### Frontend
- [x] **UI de Asignación**: Actualizar el modal de asignación de roles para incluir un selector de Contexto (Empresa/Grupo) opcional.
- [x] **Visualización**: En la lista de roles del usuario, mostrar una columna o etiqueta con el alcance (ej. "Global" o "Grupo IT").
- [x] **Gestión de Roles**: Implementar la pantalla de "Crear Rol" con el selector de permisos (Permission Picker).
