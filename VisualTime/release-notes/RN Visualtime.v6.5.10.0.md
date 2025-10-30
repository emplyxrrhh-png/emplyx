  Cegid Visualtime

 Visualtime 6.5.10.0

**Control de cambios**

Versión del documento: nº1.0  
Autor: VisualTime IDi Team  
Fecha: 18 de marzo de 2025

---

 Índice

Acerca de la versión 6.5.10.0

Notas de versión  
   Entornos destino y disponibilidad  
   Componentes impactados  
   Entregables  
      Manual de parámetros avanzados  
      Personalizaciones VSL  
      Personalizaciones Lear  
      Personalizaciones Liven  
   Procedimiento de despliegue  
      Publicación  
         Live y portales web  
         Aplicaciones móviles  
      Rollback  
   Procedimiento de Soporte  
      Mantenimiento sobre cachés de empresa  
      Reinicio de terminales familia rx  

Nuevo en esta versión  
   Migrar el informe "Ficha del usuario"de ST a HA. Estandar  
   Mejoras en la Exportación de Solicitudes  
      Nuevas columnas añadidas  
      Condiciones para la exportación del Horario y Fecha de Compensación  
   [API] Añadir horario base de vacaciones  
      Ampliación de la API: Horario Base en Vacaciones  
         Novedades  
         Comportamientos contemplados  
         Ajustes en la gestión de horarios flotantes y por horas  
   Reglas de Justificaciones en horarios, Diaria: Sustitución horaria  
   Se habilita fichaje con justificación en terminales rxFL, rxFProTD, rxFe y rxTe  

Corregido en esta versión

---

## Acerca de la versión 6.5.10.0

A continuación se detalla la información relevante sobre esta versión de Visualtime.

---

## Notas de versión

### Entornos destino y disponibilidad

- Producción1: 13 de marzo de 2025
- Producción2: 18 de marzo de 2025

### Componentes impactados

- Visualtime
- Motor de cálculo de justificaciones diarias
- Portal del empleado
- API REST
- Comunicaciones con terminales PUSH
- Informes Nueva Era

### Entregables

#### Manual de parámetros avanzados Nuevos parámetros avanzados

- **PushTerminal.RebootRxfAfterDelAllbios**: Habilita que, tras la tarea de borrado de toda la biometría (huella, palma o cara) en terminales de la familia rxF, se envía una instrucción de reinicio del terminal para refrescar cachés (algunas series de terminales no lo hacen automáticamente). El reinicio se produce un minuto después de que el terminal finalice su programación.

Manual: **Parámetros Avanzados Visualtime.pdf**

#### Personalizaciones VSL

Se añade el módulo de partes de trabajo para el cliente VSL. Este se encuentra accesible automáticamente en el cliente en cuestión en el menú ProductiV / Partes de trabajo.

Se tienen que ejecutar los 3 scripts de en la base de datos del cliente VSL (vsl3177).

Scripts TSQL: scripts

#### Personalizaciones Lear

Es necesario traspasar manualmente las plantillas de exportación 'Nómina Salaried' y 'Nómina ELM Operarios' desde el entorno anterior al nuevo. Si aún no se ha realizado este proceso, debe hacerse.

Se tiene que ejecutar el script en la base de del cliente Lear (lear7031).

Scripts TSQL: scripts

#### Personalizaciones Liven

Se tiene que ejecutar el script en la base de datos del cliente Liven (agro3592).

Scripts TSQL: scripts

En la pantalla de Importar/Exportar aparecerá una nueva opción llamada "Liven - Justificaciones diarias".

### Procedimiento de despliegue

#### Publicación

##### Live y portales web
Publicación de versión 6.5.10.0 mediante pipeline.
##### Aplicaciones móviles
Las aplicaciones móviles se desplegarán automáticamente desde las tiendas respectivas de Apple y Android tras ser publicadas. Los usuarios que no tengan configurada la actualización automática, deberán actualizar su aplicación manualmente.

#### Rollback
De ser necesario un rollback de versión, se realizará mediante Hotfix proporcionado por IDi.

### Procedimiento de Soporte

#### Mantenimiento sobre cachés de empresa
Se elimina la opción de reiniciar caché en la pantalla de diagnósticos. Al editar un parámetro avanzado se provoca un refresco de caché y licencia de la compañía en cuestión que afecta a toda la plataforma.
#### Reinicio de terminales familia rx
Se añade en la pantalla de Diagnósticos / Terminales un botón para forzar el reinicio de terminales rx PUSH. Se debe indicar el id interno del terminal, y se valida que se trate de un terminal PUSH compatible.

---

## Nuevo en esta versión

### Migrar el informe "Ficha del usuario"de ST a HA. Estandar

El informe de Ficha del Usuario proporciona una visión detallada de la información del empleado registrada en el sistema. Este informe incluye los siguientes datos clave:

- Datos generales del empleado: Nombre, identificación y estado actual.
- Contratos: Historial de contratos, fechas de inicio y fin, y condiciones laborales.
- Movilidades: Cambios de ubicación o asignaciones dentro de la empresa.
- Medios de identificación
- Saldos
- Planificación: Horarios, turnos y asignaciones laborales.
- Ausencias prolongadas: Registros de bajas médicas, licencias y otras ausencias de larga duración.
- Históricos: Información detallada de los valores registrados a lo largo del tiempo, mostrando todos los cambios en lugar de solo los datos vigentes en la fecha consultada.

El informe está diseñado para ser consultado por distintos supervisores, con seguridad aplicada según el nivel de acceso del usuario. Además, se han realizado ajustes para que ciertas secciones solo se muestren si contienen datos.

### Mejoras en la Exportación de Solicitudes
Se han agregado nuevas columnas a la exportación de solicitudes para proporcionar información más detallada y estructurada sobre los intercambios y compensaciones de horarios.

#### Nuevas columnas añadidas

- Fecha de aprobación definitiva
- Usuario con el que se intercambia el horario (Empleado para intercambio)
- Horario con el que se intercambia
- Horario de compensación y Fecha de compensación (Horario que compensa y la fecha correspondiente)

Lo que anteriormente se registraba como "Horario" ahora se denomina Horario solicitado, tanto para las solicitudes de intercambio como para las de cambio de horario. Toda la información nueva sigue el mismo formato y estructura que los datos existentes.

#### Condiciones para la exportación del Horario y Fecha de Compensación

Para que en la solicitud de intercambio horario se incluyan las columnas de Horario de compensación y Fecha de compensación, es necesario que el parámetro:

VTLive.ExchangeShiftBetweenEmployees.CompensationRequired = 1

- Si el parámetro no está activo, las columnas de compensación no se incluirán en la exportación.
- Si el parámetro está activo, las columnas de compensación se incluirán correctamente.

### [API] Añadir horario base de vacaciones.

#### Ampliación de la API: Horario Base en Vacaciones

Se ha incorporado el Horario Base de Vacaciones en los endpoints de la API, permitiendo recuperar el nombre corto del horario base cuando hay vacaciones planificadas.

- **POST**: /api/v2/ScheduleService.svc/GetCalendar
- **POST**: /api/v2/ScheduleService.svc/GetCalendarByTimestamp

##### Novedades

- Se añade un nuevo objeto que incluye la información del Horario Base.
- Si el empleado no tiene vacaciones planificadas, el valor de IDShiftBase será NULL.
- La información del horario base es la misma que la del horario habitual cuando no hay vacaciones.

##### Comportamientos contemplados

Si hay vacaciones planificadas, se obtiene correctamente el nombre corto del horario base en todos los tipos de horario:

- Horario normal con franja rígida
- Horario normal con franja flexible
- Horario por horas
- Horario flotante

Si no hay planificación de vacaciones, IDShiftBase devuelve NULL.

Si hay vacaciones planificadas y se envía el parámetro LoadLayers = true, se obtiene correctamente la información de las capas del horario en ShiftBaseLayerDefinition:

- Horario normal con franja rígida
- Horario normal con franja flexible
- Horario por horas
- Horario flotante

Si no hay vacaciones planificadas o el parámetro LoadLayers no se envía o se establece en false, ShiftBaseLayerDefinition devuelve NULL.

El método CreateOrUpdate sigue funcionando correctamente sin importar los nuevos atributos enviados.

##### Ajustes en la gestión de horarios flotantes y por horas

- Si el horario flotante o por horas es el principal, StartHour y EndHour devuelven correctamente la hora de inicio y fin planificada.
- Si el horario flotante o por horas es el base y tiene un horario de vacaciones por encima, StartHour y EndHour devolverán la hora según la definición del horario.
- Para garantizar la compatibilidad, se han creado dos nuevas propiedades:
  - **StartBasePlanned** y **EndBasePlanned**: Devuelven la hora de inicio y fin planificadas cuando hay vacaciones por encima. En caso contrario, su valor será NULL.


### Reglas de Justificaciones en horarios, Diaria: Sustitución horaria

 Nueva regla de justificaciones basada en la sustitución horaria

Se incorpora una nueva regla de justificaciones que permite la sustitución horaria. Como parte de su configuración, es posible definir los horarios de origen que serán sustituidos por el horario de destino donde se aplique la regla. El comportamiento de las acciones asociadas a las justificaciones se mantiene sin cambios.

### Se habilita fichaje con justificación en terminales rxFL, rxFProTD, rxFe y rxTe

Se habilita el fichaje en uso de justificación en fichajes de presencia en terminales rxL, rxFe y rxTe. El terminal requiere configuración adicional que debe ser llevada a cabo por el departamento técnico de Cegid.

---

### Corregido en esta versión


**1731553: Al cambiar contraseña, obligar a cerrar sesiones abiertas**
  Ticket: NA  
Se realizan mejoras de seguridad para protegernos contra sesiones abiertas.

**1722323: El informe "Los que cumplen un criterio" siempre saca los datos agrupados por periodo, a pesar de que se escoja la opción diariamente, cuando se ejecuta en un idioma diferente al castellano**
  Ticket: NA  
Se corrige error por el cual no se detalla la información por fechas al solicitar el tipo de periodo "diariamente" al ejecutar el informe "Los que cumplen un criterio" en un idioma que no es castellano.

**1723289: Texto incorrecto al mostrar el último fichaje del usuario**
  Ticket: NA  
Se modifica el texto que se muestra en la sección de último fichaje del usuario dentro de la pantalla de expediente de RRHH para que se entienda mejor.

**1723441: Informe saldos día a día: No muestra valores diarios**
  Ticket:**175886  
Dependiendo de la configuración de los grupos de saldos, no se mostraban los valores diarios de los saldos.

**1723643: El informe "Los que cumplen un criterio" finaliza con errores cuando la tabla temporal está llena**
  Ticket:**175945  
Se evita el error que aparecía al ejecutar el informe de "Los que cumplen un criterio" cuando la tabla temporal se llenaba. Se hace limpieza de registros.

**1723695: Efecto visual: la caja que muestra la fecha de cierre no siempre muestra toda la información, a veces no se ve el año completo**
  Ticket:**176029  
Se corrige error por el cual, en algunos casos, no se visualizaba completa la fecha de cierre en Expediente HR y configuración global de Fecha de cierre.

**1726798: Error en registerfirebasetoken**
  Ticket: NA  
El proceso de registro de dispositivos para notificaciones PUSH podía fallar, lo que impedía que el dispositivo afectado recibiera notificaciones.

**1729197: No se recargan los saldos al forzar una recarga manual**
  Ticket:**176037  
Se arregla error por el que no permitía refrescar los saldos en el Portal una vez cargados ese día.

**1717615: Tras realizar el borrado de biometría, un empleado continua fichando mediante ese método**
  Ticket:**175627  
Después de eliminar todas las credenciales biométricas de los empleados almacenadas en los terminales rxF, se fuerza un reinicio del dispositivo. En algunos terminales, las credenciales biométricas siguen activas y permiten el fichaje de los usuarios hasta que se completa el reinicio.

**1718806: Los valores de los campos de la ficha en el árbol de empleados, no se les aplica ninguna transformación para que se entiendan**
  Ticket:**175718  
Se corrige un error en el que no se aplicaba la máscara del valor de un campo de ficha según su tipo al agrupar por este en el árbol de empleados.

**1718809: Informe "Porcentaje del absentismo" - No se visualiza completamente la fecha fin del contrato al exportarlo en PDF al tener un número de contrato muy largo**
  Ticket:**175759  
En el informe "Porcentaje del absentismo", ahora la fecha de fin de contrato se muestra completamente al exportar a PDF, incluso cuando el número de contrato es muy largo.

**1719383: El tamaño de correo supera el máximo permitido para las colas**
  Ticket: NA  
Se permite enviar correos con una longitud más extensa.

**1719897: Error al entrar en una solicitud de intercambio horario cuando el horario implicado ha sido obsoletizado**
  Ticket: NA  
Se corrige el error que ocurría al consultar una solicitud de intercambio de horario cuando el horario involucrado era obsoleto.

**1681382: Error de trasformación en ADD (\_UnknownError) -- > Parámetro avanzado no existe VTLive.MaxAllowedFileSize no hay valor por defecto.**
  Ticket: NA  
Si VTLive.MaxAllowedFileSize no hay valor en la base de datos daba un error. Esto se ha solucionado.

**1681935: Problema con solicitudes de asuntos propios en horarios con parámetro carryrule**
  Ticket:**1673756  
Se corrige error al solicitar vacaciones/permisos del año siguiente en el cliente APV.

**1682211: Reglas convenio: Horarios borrados aparecen con el Id**
  Ticket: NA  
Al eliminar un horario no se validaba si se estaba utilizando en las reglas de solicitudes de convenio. Ahora se valida y no se deja eliminar el horario.

**1683483: El informe de auditoría de fichajes no muestra los que son del tipo Accesos + presencia**
  Ticket:**171765  
Se incluyen fichajes de acceso + presencia al informe de auditoría de fichajes.

**1689331: Texto incorrecto en castellano-Intentar completar tarea sin fichar presencia**
  Ticket: NA  
Se corrige mensaje incorrecto que aparecía al intentar completar una tarea sin haber fichado presencia.

**1693071: Informe Saldos día a día: columnas sin saldos aparecen con valores 0**
  Ticket: NA  
Se ocultan los valores de las columnas que no tengan un saldo relacionado.

**1693634: Logados en inglés, campo Tramo del Resumen de Vacaciones (México) con formato de fecha incorrecto, aparece dd/MM/yyyy**
  Ticket: NA  
Al entrar en la aplicación logado en inglés, en la sección "Resumen de vacaciones" del expediente del empleado, en el campo Tramo no se estaba mostrando el formato correcto.

**1694875: ENG: Calendario -- >distintos formatos de fecha**
  Ticket: NA  
Se adapta el formato de fecha a los diferentes idiomas en la pantalla de calendario.

**1696831: Notificación incorrecta cuando se planificaba un comunicado cuyo id coincidía con el de un empleado**
  Ticket:**169976  
Cuando se planificaba un comunicado, se emitía una notificación errónea con datos incorrectos.

**1696980: En algunos casos no se realiza el fichaje automático de salida cuando el empleado lo olvida, incluso con horarios diurnos**
  Ticket:**173577  
A pesar de estar configurado en el horario, en algunos casos, cuando un empleado olvidaba fichar la salida, esta no se generaba automáticamente al final de la jornada.

**1697954: Incidencia al guardar datos de un empleado en pantalla de Expediente RRHH**
  Ticket:**174012  
Al intentar guardar los cambios en los datos de un empleado desde la pantalla de Expediente RRHH, se mostraba un error si previamente se había editado otro empleado, faltaba algún dato obligatorio y no se había guardado correctamente.

**1698153: No se puede hacer copia masiva del horario en VT desde asistente de copia masiva**
  Ticket:**174248  
Se ha vuelto a activar desde el asistente de copia masiva del Expediente RRHH la opción de "Copiar planificación".

**1698588: VTPortal: no se puede hacer declaración de jornada al tener Chrome en inglés**
  Ticket: NA  
Se corrige la problemática de no poder hacer la declaración de jornada al tener Chrome con idioma por defecto inglés.

**1703001: ENG: Informes: las fechas de los encabezados no se muestran en inglés**
  Ticket: NA  
Se corrige el formato de las fechas de la cabecera de los informes para que se adapten al idioma del mismo.

**1707217: ENG: Informes: selectores de fechas no muestran formato correcto**
  Ticket: NA  
El formato del selector de fechas de informes se ajusta al idioma seleccionado. Así, en castellano mostrará dd/MM/aaaa, mientras que en inglés mostrará MM/dd/yyyy.

**1709248: Notificación 1010- Empleados que tienen que estar presentes -- >Al tener el parámetro de aplicación MultiTimeZoneEnabled=true, no es capaz de crear la notificación**
  Ticket: NA  
Se arregla problema de envío de la notificación "1010-Empleados que tienen que estar presentes" cuando el parámetro "MultizoneEnabled" tiene valor "true".

**1710958: Informe Justificaciones con detalle: en el selector de justificaciones, se pierde la selección hecha en la última ejecución**
  Ticket: NA  
Se corrige el problema por el que no recordaba la última selección de justificaciones en el asistente de parámetros del informe.

**1711322: "Los que cumplen los criterios" en idiomas diferentes a castellano se agrega un cero o un uno**
  Ticket: NA  
Se corrige error en el informe "Los que cumplen un criterio" por el cual la fecha se mostraba en un formato incorrecto si el idioma es diferente a castellano.

**1712285: No permite cambiar el texto de una notificación sin notificaciones creadas**
  Ticket:**174630  
Se corrige un problema visual de la pantalla de notificaciones.

**1712289: Se eliminan alertas de VTPortal que no pueden tratarse en el mismo**
  Ticket:**174664  
Se sincronizan las listas de alertas de supervisor visibles en el portal del empleado y Visualtime Live.

**1715501: En el apartado coincidencias el sistema muestra personas que están de baja y personas que no pertenecen al mismo equipo**
  Ticket:**175256  
Se corrige un error en el que se podían mostrar empleados a los que no se supervisaba si se disponía de permiso sobre el solicitante de la solicitud de forma directa, sin supervisar su grupo.