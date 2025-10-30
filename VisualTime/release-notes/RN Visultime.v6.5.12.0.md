Cegid Visualtime

Visualtime 6.5.10.0

**Control de cambios**

Versión del documento: nº1.0  
Autor: VisualTime IDi Team  
Fecha: 08 de mayo de 2025

---

 Índice

Acerca de la versión 6.5.12.0
Notas de versión
   Entornos destino y disponibilidad
   Componentes impactados
   Entregables
      Manual de parámetros avanzados
      Personalizaciones TISVOL
      Personalizaciones Ros Roca
   Procedimiento de despliegue
      Publicación
	     Live y portales web
         Personalizaciones TISVOL y Ros Roca
         Aplicaciones móviles
      Rollback
   Procedimiento de Soporte
      Desaparece aplicación VTSupervisor
Nuevo en esta versión
   Terminal Virtual - Centros de Coste
   Optimización de la pantalla de Auditoría
   Nuevo informe: Calendario anual para usuarios compacto (sin movilidades)
   Quitar VT Supervisor
Corregido en esta versión

## Acerca de la versión 6.5.12.0
A continuación se detalla la información relevante sobre esta versión de Visualtime.

---

## Notas de versión
 
### Entornos destino y disponibilidad

- Producción1	8 de mayo de 2025
- Producción2	13 de mayo de 2025
 
### Componentes impactados

- Visualtime
- Visualtime Portal
- Informes Nueva Era
- Genius

### Entregables

#### Manual de parámetros avanzados
Clarificación de algunos parámetros.
 
Manual: **Parámetros Avanzados Visualtime.pdf ** 

#### Personalizaciones TISVOL

Se han traspasado las exportaciones "Exportación Cálculo de Primas" y "Exportación Fichajes con Plantilla" del entorno ST a HA.

#### Personalizaciones Ros Roca
Se ha traspasado la exportación "Exportación a Dynamics (Ros Roca)" del entorno ST a HA.
 
### Procedimiento de despliegue

#### Publicación

##### Live y portales web

##### Publicación de versión 6.5.12.0 mediante pipeline.
 
##### Personalizaciones TISVOL y Ros Roca

##### Ejecutar los scripts TSQL proporcionados en las bases de datos correspondientes a cada cliente.
- TISVOL (BBDD remo6977):
    - Exportación Cálculo de Primas: InsertCalculoPrimasTISVOL.sql
    - Exportación Fichajes con Plantilla: InsertFichajesTISVOL.sql Ros Roca 
- (BBDD rosr2796):
    - Exportación a Dynamics: InsertRosRoca.sql
 
##### Aplicaciones móviles
Las aplicaciones móviles se desplegarán automáticamente desde las tiendas respectivas de Apple y Android tras ser publicadas.
 
Lo usuarios que no tengan configurada la actualización automática, deberán actualizar su aplicación manualmente.
 
##### Rollback
De ser necesario un rollback de versión, se realizará mediante Hotfix proporcionado por IDi.
 
### Procedimiento de Soporte

Desaparece aplicación VTSupervisor

La aplicación Visualtime Supervisor dejó de distribuirse por separado hace ya tiempo, incluyendo su funcionalidad en Visualtime Portal. En esta versión eliminamos la parte en la que se podían configurar desde Visualtime Live los permisos de acceso a la misma.
De tal manera, si un supervisor/empleado quiere acceder al portal el único permiso requerido es el de Visualtime Portal. Ya no se podrá entrar con el permiso de VTSupervisor.

Así mismo, en caso de querer acceder a la App, el permiso requerido es el de Visualtime Portal App. Ya no se podrá acceder con el permiso VTSupervisor.

Se adjunta un script que se puede ejecutar a petición del cliente si este quiere copiar los permisos de VTSupervisor a
VTPortal y de VTSupervisorApp a VTPortalApp en la wiki Copia de permisos

---

## Nuevo en esta versión
 
### Terminal Virtual - Centros de Coste
Se añade la posibilidad de fichar centro de coste en el terminal virtual. El centro de coste asignado será el configurado en la pantalla de terminales de Visualtime.
 
 
### Optimización de la pantalla de Auditoría
La pantalla de Auditoría ha sido rediseñada para optimizar su rendimiento y mejorar su usabilidad.
 
 
### Nuevo informe: Calendario anual para usuarios compacto (sin movilidades)
Se ha creado un nuevo informe denominado "Calendario anual para usuarios compacto (sin movilidades)", que muestra gráficamente el calendario de los empleados seleccionados durante el periodo indicado, sin dividir la información según los departamentos en los que haya estado cada empleado.
 
 
### Quitar VT Supervisor
La aplicación VTSupervisor se retira del ecosistema de aplicaciones de Visualtime

---

### Corregido en esta versión
 
**1739645: ENG: Informes: fecha ejecución con formato incorrecto.**
Ticket: NA
Se ha corregido un error que provocaba que la fecha de ejecución o planificación de los informes se mostrara siempre en formato castellano (dd/mm/aaaa), incluso cuando el idioma configurado era otro, como el inglés en el que el formato debe cambiar (MM/dd/aaaa).
 
 
**1756220: Error al mostrar el listado de justificaciones al lanzar un informe.**
Ticket: **177689
Se ha corregido un error que, en determinados casos, impedía mostrar el listado completo de justificaciones al generar el informe 'Justificaciones con detalle'.
 
 
**1756356: Selector de rango de fechas en pantalla de calendario no mostraba los textos traducidos en algunos idiomas.**
Ticket: **177747
El selector de rango de fechas en pantalla de calendario sólo mostraba textos correctos en idioma castellano, catalán e inglés.
 
 
**1756358: Incidencia con el formato de tiempo de Asuntos propios en VTPortal.**
Ticket: **177195
Se modifica el formato de las Vacaciones expresadas en horas al formato hh:mm (00:00)
 
 
**1763325: Descuadre valores sumados de saldos entre Genius y alguna otra fuente de datos de Live.**
Ticket: **177871
Se quita redondeo en los valores decimales de los estudios genius. De esta forma, las fórmulas serán más exactas. En caso de querer redondeos, debe utilizarse el formato del componente.
 
 
**1768928: Informes nueva era: Los selectores de incidencias, justificaciones y saldos permiten búsqueda por nombre.**
Ticket: NA
A partir de ahora, los selectores de incidencias, justificaciones y saldos de los informes permiten la búsqueda por nombre. Al escribir algunas letras, se mostrarán únicamente las opciones que contengan ese texto en su nombre, facilitando así la localización y selección.
 
 
**17635**17: Si en una plantilla de exportación de saldos se le define la ordenación, da un error al ejecutar en modo texto.**
Ticket: **178067
Se corrige un error que impedía lanzar una exportación de saldos con plantilla en modo texto si esta tenía definida una ordenación.
 
 
**1763586: Usuarios con contrato finalizado reciben notificaciones Push de comunicados.**
Ticket: **178086
Se dejan de enviar notificaciones push de comunicados a usuarios con contrato finalizado.
 
 
**1763599: La hora de última actualización en la pantalla de alertas muestra la hora del servidor, que en zonas horarias distintas no coincide con la del PC.**
Ticket: **178129
Se corrige un error en el que no se mostraba la hora de refresco de las alertas con la hora local del usuario.
 
 
**1765955: La notificación Aviso a usuario de día con justificación pendiente no se está enviando.**
Ticket: **177812
Se añade la notificación "Aviso a usuario de día con justificación pendiente" con opción de repetición.
 
 
**1767140: Al personalizar el teletrabajo en el contrato tarda mucho en guardar.**
Ticket: **178336
Se optimiza el rendimiento de la actualización de teletrabajo desde la pantalla de contratos.
 
 
**1773047: En catalán el selector de empleados del calendario no carga.**
Ticket: **178770
Se corrige un error que impedía que se visualizara correctamente el selector de empleados en la pantalla de calendario si se utilizaba el idioma catalán.
 
 
**1773345: No se guarda el formato de celda porcentaje en genius.**
Ticket: **178845
Se ha corregido un error que hacía que el formato de porcentaje (%) aplicado en las celdas del diseño de un estudio Genius no se conservara al ejecutar nuevamente el estudio.
 
**1777246: No es posible adjuntar documentos relativos a ausencias en la aplicación móvil del portal del empleado.**
Ticket: **179102
Se corrige error que impedía adjuntar documentos desde la aplicación de android.
 
 
**1777538: Informe "Registro de la jornada con Justificaciones" no muestra los totalizadores de los saldos seleccionados en el lanzamiento.**
Ticket: **179101
El informe "Registro de la jornada con justificaciones" no mostraba la sección de saldos al final de cada empleado, aunque se hubiera pedido detalle de los mismos.
 
 
**1777943: En clientes ONE No se muestra el resumen de los horarios planificados en la vista de planificación anual.**
Ticket: **178993
Se ha solucionado un problema en la versión One que impedía la visualización de los totales por horarios en la vista anual del Calendario.
 
 
**1778003: [Conector] No se importan fichajes con parser por posiciones en lugar de por separadores.**
Ticket: **179167
El conector de fichajes no admitía ficheros de fichajes estructurados con posiciones fijas
 
 
**1778205: Optimización de la carga de VTPortal.**
Ticket: NA
Se reduce la cantidad de llamadas que hace VTPortal en estado de reposo para mejorar el rendimiento del servicio.
 
 
**1778348: Informe lista de tareas solo visible como consultor.**
Ticket: **178850
Se corrige error por el cual los supervisores no veían el informe de Lista de tareas.
 
 
**1778704: Cuando se accede al portal, no es posible utilizar el botón de fichaje rápido hasta que no se cambia de pantalla y se vuelve a la principal.**
Ticket: **179235
Se ha corregido un error que impedía la activación de los botones en la pantalla de inicio del portal en los dispositivos móviles cuando la pantalla estaba minificada.
 
**1779393: iOS Dispositivo móvil: no funcionan los enlaces externos.**
Ticket: NA
Se corrige un error que impedía abrir enlaces externos desde Visualtime portal en iOS 18 o superior.
 
 
**1763395: El informe planificado "Registro mensual de la jornada" aleatoriamente no muestra datos al producirse errores en su generación.**
Ticket: **177655
Se corrige un error por el que en ocasiones no mostraba datos.
 
 
**1756180: El informe pagos por contrato devuelve valores incorrectos.**
Ticket: **177588
Se corrige un error en el informe de pagos que podía mostrar datos incorrectos si el usuario tenía movilidades a futuro.
