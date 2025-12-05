# Arquitectura del Menú de Navegación (Emplyx.React)

Este documento define la arquitectura, diseño y comportamiento del menú de navegación para el nuevo proyecto de frontend **Emplyx.React**, el cual consumirá las APIs de backend existentes en .NET 9.

## 1. Principios de Diseño (Look & Feel)

*   **Estilo**: Innovador, alegre y fresco. Se busca romper con la rigidez de los paneles de administración clásicos.
*   **Experiencia de Usuario (UX)**: Fluida y minimalista. El contenido es el protagonista, por lo que la navegación debe ser eficiente y poco intrusiva.

## 2. Estructura del Layout

La aplicación se distribuye en tres áreas principales, optimizando el espacio de trabajo:

### A. Cabecera (Header)
*   **Ubicación**: Superior.
*   **Contenido**:
    *   **Derecha**: Área de usuario. Debe mostrar claramente la **foto de perfil** y el **nombre del usuario**.

### B. Barra Lateral Izquierda (Sidebar)
Es el elemento principal de navegación y tiene tres secciones diferenciadas verticalmente:
1.  **Superior (Contexto)**:
    *   **Selector de Empresa**: Un desplegable (dropdown) que permite al usuario cambiar la empresa sobre la que está operando.
2.  **Central (Navegación)**:
    *   Lista de ítems del menú principal.
    *   Barra de desplazamiento (scroll) independiente si el contenido excede la altura.
3.  **Inferior (Configuración)**:
    *   El acceso a la **Configuración** se ancla al final del menú, separado visualmente del resto de la navegación operativa.

### C. Pie de página (Footer)
*   **Ubicación**: Inferior.
*   **Contenido**: Información legal, copyright o versión de la aplicación.

## 3. Comportamiento del Menú

### Modo Colapsado (Default/User Action)
*   El menú debe tener la capacidad de **colapsarse**, reduciendo su ancho al mínimo necesario para mostrar únicamente los **iconos** de los elementos principales.
*   Esto maximiza el área de trabajo para las tablas y formularios de la aplicación.

### Interacción con Sub-ítems (Hover)
*   **Desaparición/Aparición**: Los sub-ítems no deben estar siempre visibles ni expandir el menú verticalmente (acordeón) de forma intrusiva en el modo colapsado.
*   **Hover**: Al posicionar el cursor sobre un ítem principal (especialmente en modo colapsado), los sub-ítems deben aparecer en un **panel flotante (popover/tooltip avanzado)** adyacente al ícono.

## 4. Estructura Jerárquica (Contenido)

A continuación se detalla la jerarquía funcional adaptada al nuevo diseño.

```json
[
  { "label": "Inicio", "icon": "Home" },
  {
    "label": "Organización",
    "icon": "Business",
    "children": [
      { "label": "Convenios" }
    ]
  },
  {
    "label": "Usuarios",
    "icon": "People",
    "children": [
      { "label": "Expediente de RRHH" },
      { "label": "OnBoarding" },
      { "label": "Solicitudes" },
      { "label": "Encuestas de RRHH" }
    ]
  },
  {
    "label": "Gestión Horaria",
    "icon": "Schedule",
    "children": [
      { "label": "Calendario" },
      { "label": "Estado del absentismo" }
    ]
  },
  { "label": "Gestor documental", "icon": "Description" },
  { "label": "Estado de las Zonas", "icon": "Map" },
  {
    "label": "Comunicaciones",
    "icon": "Chat",
    "children": [
      { "label": "Comunicados" }
    ]
  },
  { "label": "Bots", "icon": "SmartToy" },
  {
    "label": "Análisis, informes y datos",
    "icon": "Analytics",
    "children": [
      { "label": "Analytics by Genius" },
      { "label": "Informes" },
      { "label": "Informes solicitados" },
      { "label": "Importar/Exportar" }
    ]
  },
  {
    "label": "RealCost",
    "icon": "Euro",
    "children": [
      { "label": "Centros de coste" }
    ]
  },
  { "label": "Alertas", "icon": "Notifications" },
  {
    "label": "Seguridad y auditoría",
    "icon": "Security",
    "children": [
      { "label": "Auditoría" },
      { "label": "Licencia" },
      { "label": "Fecha de cierre" },
      { "label": "Seguridad avanzada" }
    ]
  },
  {
    "label": "Configuración gestión horaria",
    "icon": "Tune",
    "children": [
      { "label": "Horarios" },
      { "label": "Justificaciones" },
      { "label": "Saldos" },
      { "label": "Indicadores" }
    ]
  },
  {
    "label": "Configuración",
    "icon": "Settings",
    "position": "bottom", // Renderizar en la sección inferior fija
    "children": [
      { "label": "Ficha" },
      { "label": "Visualtime" },
      { "label": "Configuración del Portal" },
      { "label": "Contraseña" },
      { "label": "Informe de emergencia" },
      { "label": "Login integrado" },
      { "label": "Zonas" },
      { "label": "Cámaras" },
      { "label": "Notificaciones" },
      { "label": "Terminales" }
    ]
  }
]
```

## 5. Implementación Técnica (React)

### Stack Sugerido
*   **Framework**: React.
*   **Lenguaje**: TypeScript (para mantener tipado fuerte con los DTOs de .NET).
*   **Estilos**: CSS Modules, Styled Components o Tailwind CSS para facilitar el diseño "fresco" y personalizado.
*   **Iconos**: Material Icons, FontAwesome o similar (necesarios para el modo colapsado).

### Componentes Clave
1.  **`MainLayout`**: Grid/Flex container que orquesta Header, Sidebar y Content.
2.  **`Sidebar`**: Gestiona el estado `isCollapsed`.
3.  **`CompanySelector`**: Componente dropdown en la cima del Sidebar.
4.  **`NavMenu`**: Itera sobre la configuración JSON.
5.  **`NavItem`**: Renderiza el icono y etiqueta. Maneja los eventos `onMouseEnter`/`onMouseLeave` para mostrar los submenús.
6.  **`UserProfile`**: Componente visual en el Header.
