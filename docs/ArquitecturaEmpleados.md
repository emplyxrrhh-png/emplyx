# Arquitectura de Empleados

## 1. Estructura Organizativa
La jerarquía principal para la gestión de empleados será:
**Tenant > Grupo de Empresas > Empresa > Centro de Trabajo > Empleados**

## 2. Gestión de Empleados (UI/UX)

### Vista Principal: Lista de Empleados
- **Título:** Empleados
- **Ubicación:** Accesible desde el menú principal, bajo la sección de gestión o configuración, dependiendo del rol.
- **Botón de Acción:** "Nuevo empleado" (situado en la parte superior derecha).
- **Visualización:** Lista o Tabla de empleados pertenecientes al nodo seleccionado en el árbol de organización (Empresa o Centro de Trabajo).

### Formulario de Nuevo Empleado / Edición
Al hacer clic en "Nuevo empleado" o editar uno existente, se abrirá un formulario organizado en **"Cortinas" (Secciones desplegables/Acordeón)** para mejorar la usabilidad.

#### Secciones del Formulario (Cortinas)

1.  **Datos Generales (Identificación)**
    *   **ID:** Identificador único (interno).
    *   **Nombre:** Nombre completo del empleado.
    *   **Alias:** Nombre corto o alias (EmployeeAlias).
    *   **Grupo:** Nombre del grupo al que pertenece (GroupName).
    *   **Tipo:** Tipo de empleado (Type).
    *   **Imagen:** Fotografía del empleado.
    *   **Estado:** Estado actual (EmployeeStatus).

2.  **Control y Accesos**
    *   **ID Grupo de Acceso:** (IDAccessGroup).
    *   **ID Biométrico:** (BiometricID).
    *   **Control de Presencia:** (AttControlled) - Checkbox/Bit.
    *   **Control de Acceso:** (AccControlled) - Checkbox/Bit.
    *   **Control de Tareas:** (JobControlled) - Checkbox/Bit.
    *   **Control Externo:** (ExtControlled) - Checkbox/Bit.
    *   **Control de Riesgos:** (RiskControlled) - Checkbox/Bit.

3.  **Credenciales Web / Directorio Activo**
    *   **Login Web:** (WebLogin).
    *   **Password Web:** (WebPassword).
    *   **Active Directory:** (ActiveDirectory) - Checkbox/Bit.

4.  **Campos Personalizados (User Fields)**
    Esta sección es dinámica y permite la configuración de campos adicionales según las necesidades de la organización.
    
    **Tipos de datos soportados:**
    *   **Texto (Text):** Cadenas de caracteres alfanuméricos.
    *   **Número (Numeric):** Valores enteros o decimales.
    *   **Bit (Boolean):** Valores verdadero/falso (Checkboxes).
    *   **Desplegable (List):** Selección única de una lista predefinida de valores.
    *   **Fecha (Date):** Fechas.
        *   **Formato de almacenamiento en BD:** Internacional `yyyy/MM/dd` (Año/Mes/Día) para asegurar consistencia.

## 3. Funcionalidades Avanzadas (Heredadas)

### Árbol de Navegación y Movimientos
*   **Panel Izquierdo:** Árbol colapsable (Tenant > Empresa > Centro de Trabajo > Empleados).
*   **Drag & Drop:** Funcionalidad para mover empleados entre centros de trabajo o empresas arrastrando el nodo del empleado.
    *   **Modal de Confirmación:** Al soltar, se solicita:
        *   Fecha efectiva del movimiento.
        *   Opción para cerrar asignación anterior.
*   **Histórico:** Registro de movimientos (Asignaciones) con fecha de inicio y fin.

### Búsqueda
*   Barra de búsqueda global para localizar empleados rápidamente sin navegar por todo el árbol.

## 4. Consideraciones Técnicas
*   **Base de Datos:**
    *   Tabla `Employees` para datos fijos.
    *   Tabla `EmployeeUserFields` (o similar) para los campos personalizados, vinculados por `EmployeeID` y `FieldDefinitionID`.
    *   Las fechas en campos personalizados se deben serializar/deserializar respetando el formato `yyyy/MM/dd`.

