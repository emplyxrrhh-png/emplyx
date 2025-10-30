Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.VTBase

Public Class PunchProxy
    Implements IPunchSvc

    Public Function KeepAlive() As Boolean Implements IPunchSvc.KeepAlive
        Return True
    End Function

    Public Function GetPunch(ByVal intIDPunch As Long, ByVal oState As roWsState) As roGenericVtResponse(Of roPunch) Implements IPunchSvc.GetPunch
        Return PunchMethods.GetPunch(intIDPunch, oState)
    End Function

    '''' <summary>
    '''' Guarda un fichaje. <br/>
    '''' Se le puede indicar si tiene que verificar la parametrización para iniciar la tarea de producción en caso de una entrada.<br/>
    '''' Se notifica al servidor el cambio para que recalcule los datos necesarios.<br/>
    '''' * Se audita la grabación.
    '''' </summary>
    '''' <param name="oPunch">Definición del fichaje.</param>
    '''' <param name="bolAutomaticBeginJobCheck">Para indicar si tiene que iniciar la tarea de producción. <br/>
    '''' Sólo en el caso de que el parámetro AutomaticBeginJob este activado y que sea una entrada.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function SavePunch(ByVal oPunch As roPunch, ByVal bolAutomaticBeginJobCheck As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.SavePunch
        Return PunchMethods.SavePunch(oPunch, bolAutomaticBeginJobCheck, oState)
    End Function

    '''' <summary>
    '''' Guarda los fichajes pasados.<br/>
    '''' Se notifica al servidor los cambios para que recalcule los datos necesarios.<br/>
    '''' * Se audita la acción.
    '''' </summary>
    '''' <param name="ds">Dataset con una tabla que contiene los fichajes a guardar.<br/>
    '''' Columnas: IDEmployee, DateTime, IDReader, IDCause, ShiftDate, ID, IDZone, IDReaderType, IsNotReliable</param>
    '''' <param name="bolAutomaticBeginJobCheck">Para indicar si tiene que iniciar la tarea de producción. <br/>
    '''' Sólo en el caso de que el parámetro AutomaticBeginJob este activado y que sea una entrada.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function SavePunches(ByVal ds As DataSet, ByVal bolAutomaticBeginJobCheck As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.SavePunches
        Return PunchMethods.SavePunches(ds, bolAutomaticBeginJobCheck, oState)
    End Function

    '''' <summary>
    '''' Elimina un fichaje.<br/>
    '''' Se notifica al servidor los cambios para que recalcule los datos necesarios.<br/>
    '''' * Se audita la acción.
    '''' </summary>
    '''' <param name="oPunch">Objeto 'roPunch' con la definición del fichaje a eliminar.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function DeletePunch(ByVal oPunch As roPunch, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.DeletePunch
        Return PunchMethods.DeletePunch(oPunch, oState)
    End Function

    '''' <summary>
    '''' Elimina un fichaje.<br/>
    '''' Se notifica al servidor los cambios para que recalcule los datos necesarios.<br/>
    '''' * Se audita la acción.
    '''' </summary>
    '''' <param name="intIDPunch">Código que identifica el fichaje.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function DeletePunchByID(ByVal intIDPunch As Long, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.DeletePunchByID
        Return PunchMethods.DeletePunchByID(intIDPunch, oState)
    End Function

    '''' <summary>
    '''' Obtiene los registros de la tabla fichajes según los parámetros de filtrado.        
    '''' </summary>
    '''' <param name="dShiftDateBegin">Para filtrar según la fecha asignada inicial.</param>
    '''' <param name="dShiftDateEnd">Para filtrar según la fecha asignada final.</param>
    '''' <param name="dPeriodBegin">Para filtrar según la fecha/hora inicial.</param>
    '''' <param name="dPeriodEnd">Para filtrar según la fecha/hora  final.</param>
    '''' <param name="iIDEmployee">Para filtrar según el código de empleado</param>
    '''' <param name="iIDTerminal">Para filtrar según el código de terminal.</param>
    '''' <param name="iIDReader">Para filtrar según el código de lector.</param>
    '''' <param name="iIDCause">Para filtrar según el código de justificación.</param>
    '''' <param name="iIDZone">Para filtrar según el código de zona.</param>
    '''' <param name="iIsNotReliable">Para filtrar según si es un movimiento fiable o no.</param>
    '''' <param name="sTypes">Para filtrar por tipos de fichaje real</param>
    '''' <param name="sActualTypes">Para filtrar por tipos de fichaje actual.</param>
    '''' <param name="sInvalidTypes">Para filtrar por tipos de fichaje invalido de accesos.</param>
    '''' <param name="sActions">Para filtrar por tipos de acciones.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>Tabla con la columnas: ID,IDCredential,IDEmployee,Type,ActualType,InvalidType,ShiftDate,DateTime,IDTerminal,IDReader,IDZone,IDCause,Position,IP,IsNotReliable,Action,IDPassport</returns>
    '''' <remarks></remarks>
    Public Function GetPunchesDataTable(ByVal dShiftDateBegin As Date, ByVal dShiftDateEnd As Date, ByVal dPeriodBegin As DateTime, ByVal dPeriodEnd As DateTime, ByVal iIDEmployee As String, ByVal iIDTerminal As String, ByVal iIDReader As String, ByVal iIDCause As String,
                                        ByVal iIDZone As String, ByVal iIsNotReliable As Integer, ByVal sTypes As String, ByVal sActualTypes As String, ByVal sInvalidTypes As String, ByVal sActions As String, ByVal sOrderBy As String,
                                        ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetPunchesDataTable
        Return PunchMethods.GetPunchesDataTable(dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause,
                                        iIDZone, iIsNotReliable, sTypes, sActualTypes, sInvalidTypes, sActions, sOrderBy, oState)
    End Function

    '''' <summary>
    '''' Obtiene el último registros de la tabla fichajes según los parámetros de filtrado.        
    '''' </summary>
    '''' <param name="dShiftDateBegin">Para filtrar según la fecha asignada inicial.</param>
    '''' <param name="dShiftDateEnd">Para filtrar según la fecha asignada final.</param>
    '''' <param name="dPeriodBegin">Para filtrar según la fecha/hora inicial.</param>
    '''' <param name="dPeriodEnd">Para filtrar según la fecha/hora  final.</param>
    '''' <param name="iIDEmployee">Para filtrar según el código de empleado</param>
    '''' <param name="iIDTerminal">Para filtrar según el código de terminal.</param>
    '''' <param name="iIDReader">Para filtrar según el código de lector.</param>
    '''' <param name="iIDCause">Para filtrar según el código de justificación.</param>
    '''' <param name="iIDZone">Para filtrar según el código de zona.</param>
    '''' <param name="iIsNotReliable">Para filtrar según si es un movimiento fiable o no.</param>
    '''' <param name="sTypes">Para filtrar por tipos de fichaje real</param>
    '''' <param name="sActualTypes">Para filtrar por tipos de fichaje actual.</param>
    '''' <param name="sInvalidTypes">Para filtrar por tipos de fichaje invalido de accesos.</param>
    '''' <param name="sActions">Para filtrar por tipos de acciones.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>Tabla con la columnas: ID,IDCredential,IDEmployee,Type,ActualType,InvalidType,ShiftDate,DateTime,IDTerminal,IDReader,IDZone,IDCause,Position,IP,IsNotReliable,Action,IDPassport</returns>
    '''' <remarks></remarks>
    Public Function GetLastPunchDataTable(ByVal dShiftDateBegin As Date, ByVal dShiftDateEnd As Date, ByVal dPeriodBegin As DateTime, ByVal dPeriodEnd As DateTime, ByVal iIDEmployee As String, ByVal iIDTerminal As String, ByVal iIDReader As String, ByVal iIDCause As String,
                                  ByVal iIDZone As String, ByVal iIsNotReliable As Integer, ByVal sTypes As String, ByVal sActualTypes As String, ByVal sInvalidTypes As String, ByVal sActions As String,
                                  ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetLastPunchDataTable
        Return PunchMethods.GetLastPunchDataTable(dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause,
                                  iIDZone, iIsNotReliable, sTypes, sActualTypes, sInvalidTypes, sActions, oState)
    End Function

    '''' <summary>
    '''' Obtiene el número de registros de la tabla fichajes según los parámetros de filtrado.        
    '''' </summary>
    '''' <param name="dShiftDateBegin">Para filtrar según la fecha asignada inicial.</param>
    '''' <param name="dShiftDateEnd">Para filtrar según la fecha asignada final.</param>
    '''' <param name="dPeriodBegin">Para filtrar según la fecha/hora inicial.</param>
    '''' <param name="dPeriodEnd">Para filtrar según la fecha/hora  final.</param>
    '''' <param name="iIDEmployee">Para filtrar según el código de empleado</param>
    '''' <param name="iIDTerminal">Para filtrar según el código de terminal.</param>
    '''' <param name="iIDReader">Para filtrar según el código de lector.</param>
    '''' <param name="iIDCause">Para filtrar según el código de justificación.</param>
    '''' <param name="iIDZone">Para filtrar según el código de zona.</param>
    '''' <param name="iIsNotReliable">Para filtrar según si es un movimiento fiable o no.</param>
    '''' <param name="sTypes">Para filtrar por tipos de fichaje real</param>
    '''' <param name="sActualTypes">Para filtrar por tipos de fichaje actual.</param>
    '''' <param name="sInvalidTypes">Para filtrar por tipos de fichaje invalido de accesos.</param>
    '''' <param name="sActions">Para filtrar por tipos de acciones.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' ''' <returns>Número de registros</returns>
    '''' <remarks></remarks>

    Public Function GetPunchesCountDataTable(ByVal dShiftDateBegin As Date, ByVal dShiftDateEnd As Date, ByVal dPeriodBegin As DateTime, ByVal dPeriodEnd As DateTime, ByVal iIDEmployee As String, ByVal iIDTerminal As String, ByVal iIDReader As String, ByVal iIDCause As String,
                                  ByVal iIDZone As String, ByVal iIsNotReliable As Integer, ByVal sTypes As String, ByVal sActualTypes As String, ByVal sInvalidTypes As String, ByVal sActions As String, ByVal sOrderBy As String,
                                  ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IPunchSvc.GetPunchesCountDataTable
        Return PunchMethods.GetPunchesCountDataTable(dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause,
                                  iIDZone, iIsNotReliable, sTypes, sActualTypes, sInvalidTypes, sActions, sOrderBy, oState)
    End Function

    '''' <summary>
    '''' Añade un fichaje de presencia para un empleado. El tipo de fichaje generado se calcula en función del estado actual del empleado.
    '''' </summary>
    '''' <param name="_IDEmployee">Código del empleado</param>
    '''' <param name="_InputDateTime">Fecha y hora del movimiento a generar</param>
    '''' <param name="_IDTerminal">Código del terminal por el que se realiza el movimiento</param>
    '''' <param name="_IDReader">Número del lector por el que se realiza el movimiento</param>
    '''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
    '''' <param name="_InputCaptureBytes">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
    '''' <param name="_Punch">Devuelve el fichaje generado.</param>
    '''' <param name="_InputType">Devuelve el tipo de fichaje generado</param>
    '''' <param name="_SeqStatus">Devuelve el estado de la secuencia resultado de la acción</param>
    '''' <param name="_SaveData">Indica si se tiene que guardar el movimiento generado o no.</param>
    '''' <param name="oState">Información adicional de estado.</param>        
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>

    Public Function DoSequencePunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCaptureBytes() As Byte,
                                    ByVal _Lat As Double, ByVal _Lon As Double, ByVal _LocationZone As String, ByVal _fullAddress As String, ByVal _TimeZone As String, ByVal _SaveData As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roDoSequencePunch) Implements IPunchSvc.DoSequencePunch

        Return PunchMethods.DoSequencePunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDReader, _IDCause, _InputCaptureBytes,
                                  _Lat, _Lon, _LocationZone, _fullAddress, _TimeZone, _SaveData, oState)
    End Function


    '''' <summary>
    '''' Añade un fichaje de tarea para un empleado. 
    '''' </summary>
    '''' <param name="_IDEmployee">Código del empleado</param>
    '''' <param name="_InputDateTime">Fecha y hora del movimiento a generar</param>
    '''' <param name="_IDTerminal">Código del terminal por el que se realiza el movimiento</param>
    '''' <param name="_IDTask">Código de la tarea.</param>
    '''' <param name="_InputCaptureBytes">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
    '''' <param name="_Punch">Devuelve el fichaje generado.</param>
    '''' <param name="_InputType">Devuelve el tipo de fichaje generado</param>
    '''' <param name="oState">Información adicional de estado.</param>        
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function DoTaskPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDTask As Integer, ByVal _InputCaptureBytes() As Byte, ByVal _Lat As Double,
                                ByVal _Lon As Double, ByVal _LocationZone As String, ByVal _fullAddress As String, ByVal _TimeZone As String, ByVal Field1 As String, ByVal Field2 As String, ByVal Field3 As String,
                                ByVal Field4 As Double, ByVal Field5 As Double, ByVal Field6 As Double, ByVal oState As roWsState) As roGenericVtResponse(Of roDoSequencePunch) Implements IPunchSvc.DoTaskPunch
        Return PunchMethods.DoTaskPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDTask, _InputCaptureBytes,
                                  _Lat, _Lon, _LocationZone, _fullAddress, _TimeZone, Field1, Field2, Field3, Field4, Field5, Field6, oState)
    End Function

    '''' <summary>
    '''' Añade un fichaje de tarea para un empleado. 
    '''' </summary>
    '''' <param name="_IDEmployee">Código del empleado</param>
    '''' <param name="_InputDateTime">Fecha y hora del movimiento a generar</param>
    '''' <param name="_IDTerminal">Código del terminal por el que se realiza el movimiento</param>
    '''' <param name="_IDCostCenter">Código de la tarea.</param>
    '''' <param name="_InputCaptureBytes">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
    '''' <param name="_Punch">Devuelve el fichaje generado.</param>
    '''' <param name="_InputType">Devuelve el tipo de fichaje generado</param>
    '''' <param name="oState">Información adicional de estado.</param>        
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function DoCostCenterPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDCostCenter As Integer,
                                      ByVal _InputCaptureBytes() As Byte, ByVal _Lat As Double, ByVal _Lon As Double, ByVal _LocationZone As String, ByVal _fullAddress As String,
                                      ByVal _TimeZone As String, ByVal oState As roWsState) As roGenericVtResponse(Of roDoSequencePunch) Implements IPunchSvc.DoCostCenterPunch

        Return PunchMethods.DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, _InputCaptureBytes,
                                  _Lat, _Lon, _LocationZone, _fullAddress, _TimeZone, oState)
    End Function

    '''' <summary>
    '''' Cambia el estado de presencia actual del empleado generando un fichaje a la hora indicada.
    '''' </summary>
    '''' <param name="_IDEmployee">Código del empleado</param>
    '''' <param name="_NowDateTime">Fecha y hora actual</param>
    '''' <param name="_InputDateTime">Hora en la que se generará el fichaje olvidado para el cambio de estado</param>
    '''' <param name="_IDTerminal">Código de terminal por el que se realiza la operación</param>
    '''' <param name="_IDReader">Número de lector por el que se realiza la operación</param>
    '''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
    '''' <param name="_InputCaptureBytes">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
    '''' <param name="_Punch">Devuelve el fichaje generado</param>
    '''' <param name="_SaveData">Indica si se tiene que guardar el fichaje generado o no.</param>
    '''' <param name="oState">Información adicional de estado.</param>        
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function ChangeState(ByVal _IDEmployee As Integer, ByVal _NowDateTime As DateTime, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer,
                                ByVal _InputCaptureBytes() As Byte, ByVal _Punch As roPunch, ByVal _SaveData As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roPunch) Implements IPunchSvc.ChangeState
        Return PunchMethods.ChangeState(_IDEmployee, _NowDateTime, _InputDateTime, _IDTerminal, _IDReader, _IDCause,
                                  _InputCaptureBytes, _Punch, _SaveData, oState)
    End Function

    '''' <summary>
    '''' Reorganiza los fichajes de un empleado y dia.
    '''' </summary>
    '''' <param name="intIDEmployee">Código empleado.</param>
    '''' <param name="xDate">Fecha fichaje</param>
    '''' <param name="bolIncludeCaptures">Columna captura imagen (Si/No) (no se utiliza)</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function ReorderPunches(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.ReorderPunches
        Return PunchMethods.ReorderPunches(intIDEmployee, xDate, bolIncludeCaptures, oState)
    End Function

    '''' <summary>
    '''' Obtiene los fichajes de presencia de un empleado para una fecha.
    '''' </summary>
    '''' <param name="intIDEmployee">Código empleado.</param>
    '''' <param name="xDate">Fecha</param>
    '''' <param name="bolIncludeCaptures">Indica si se quieren obtener las imágenes de los fichajes (si existen).</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <remarks></remarks>
    Public Function GetPunchesPresFromEmployee(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetPunchesPresFromEmployee
        Return PunchMethods.GetPunchesPresFromEmployee(intIDEmployee, xDate, bolIncludeCaptures, oState, strFilter)
    End Function

    '''' <summary>
    '''' Obtiene los fichajes de presencia de un empleado para una fecha.
    '''' </summary>
    '''' <param name="intIDEmployee">Código empleado.</param>
    '''' <param name="xDate">Fecha</param>
    '''' <param name="xEndDate">Fecha final</param>
    '''' <param name="bolIncludeCaptures">Indica si se quieren obtener las imágenes de los fichajes (si existen).</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <remarks></remarks>
    Public Function GetPunchesPresFromEmployeeInPeriod(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal xEndDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetPunchesPresFromEmployeeInPeriod
        Return PunchMethods.GetPunchesPresFromEmployeeInPeriod(intIDEmployee, xDate, xEndDate, bolIncludeCaptures, oState, strFilter)
    End Function

    '''' <summary>
    '''' Devuelve un DataSet con 4 Tablas (Contadores accesos, Accesos, Salida, Incorrectos)
    '''' </summary>
    '''' <param name="Feature">FeaturePermission</param>
    '''' <param name="Type">Tipus</param>
    '''' <param name="IDAccessGroups">ID Grupo de accesos</param>
    '''' <param name="IDZones">ID Zona</param>
    '''' <param name="oState">oState</param>
    '''' <param name="bColImage">Columna imagen (solo crea la columna vacia)</param>
    '''' <returns>Devuelve un DataSet amb 4 Taules (Contadores accesos, Accesos, Salida, Incorrectos)</returns>
    '''' <remarks></remarks>
    Public Function GetAccessPunchesStatus(ByVal Feature As String, ByVal Type As String, ByVal IDAccessGroups As Generic.List(Of Integer), ByVal IDZones As Generic.List(Of Integer),
                                           ByVal oState As roWsState, ByVal bColImage As Boolean, ByVal bOnlyCounters As Boolean) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetAccessPunchesStatus
        Return PunchMethods.GetAccessPunchesStatus(Feature, Type, IDAccessGroups, IDZones, oState, bColImage, bOnlyCounters)
    End Function



    '''' <summary>
    '''' Obtiene los registros de la tabla 'InvalidEntries' que no se han podido importar en VisualTime.
    '''' </summary>
    '''' <param name="oState">Información de estado actual.</param>
    '''' <returns>Tabla con las columnas: DateTime, IDCredential, IDTerminal, Type, IDCause, ID, IDReader </returns>
    '''' <remarks></remarks>
    Public Function GetInvalidEntries(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetInvalidEntries
        Return PunchMethods.GetInvalidEntries(oState)
    End Function

    '''' <summary>
    '''' Obtiene los registros de la tabla 'Punches' sin empleado asignado.
    '''' </summary>
    '''' <param name="oState">Información de estado actual.</param>
    '''' <returns>Tabla con las columnas: DateTime, IDCredential, IDTerminal, Type, IDCause, ID, IDReader </returns>
    '''' <remarks></remarks>
    Public Function GetInvalidPunches(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetInvalidPunches
        Return PunchMethods.GetInvalidPunches(oState)
    End Function


    '''' <summary>
    '''' Obtiene los registros de la tabla 'Punches' sin empleado asignado con una tarjeta concreta.
    '''' </summary>
    '''' <param name="oState">Información de estado actual.</param>
    '''' <param name="lngIDCard">tarjeta seleccionada</param>
    '''' <returns>Tabla con las columnas: DateTime, IDCredential, IDTerminal, Type, IDCause, ID, IDReader </returns>
    '''' <remarks></remarks>
    Public Function GetInvalidPunchesByIDCard(ByVal oState As roWsState, ByVal lngIDCard As Long) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetInvalidPunchesByIDCard
        Return PunchMethods.GetInvalidPunchesByIDCard(oState, lngIDCard)
    End Function

    '''' <summary>
    '''' Guarda los fichajes invalidos no asignados a ningún empleado pasados.<br/>
    '''' </summary>
    '''' <param name="ds">Dataset con una tabla que contiene los fichajes a guardar.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function SaveInvalidPunches(ByVal ds As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.SaveInvalidPunches
        Return PunchMethods.SaveInvalidPunches(ds, oState)
    End Function


    '''' <summary>
    '''' Reprocesa fichajes no procesados que quedaron en InvalidEntries<br/>
    '''' </summary>
    '''' <param name="ds">Dataset con una tabla que contiene los fichajes a guardar.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Function ReprocessInvalidEntries(ByVal aInvalidEntries() As DTOs.roPunchInvalidEntry, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.ReprocessInvalidEntries
        Return PunchMethods.ReprocessInvalidEntries(aInvalidEntries, oState)
    End Function

    '''' <summary>
    '''' Retorna dataset con los ultimos fichajes realizados de accesos validos e invalidos
    '''' </summary>
    '''' <param name="ListIdZones">Lista de zonas sobre las que obtener datos</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>Dataset</returns>
    Public Function GetAccessPunchesMonitor(ByVal oState As roWsState, ByVal ListIdZones As Generic.List(Of Integer), ByVal ListIdFields As Generic.List(Of String),
                                            ByVal bColImage As Boolean) As roGenericVtResponse(Of DataSet) Implements IPunchSvc.GetAccessPunchesMonitor
        Return PunchMethods.GetAccessPunchesMonitor(oState, ListIdZones, ListIdFields, bColImage)
    End Function


    Public Function HasCapturePunch(ByVal intIDPunch As Long, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPunchSvc.HasCapturePunch
        Return PunchMethods.HasCapturePunch(intIDPunch, oState)
    End Function

    Public Function GetCapturePunch(ByVal IDPunch As Long, ByVal oState As roWsState) As roGenericVtResponse(Of Byte()) Implements IPunchSvc.GetCapturePunch
        Return PunchMethods.GetCapturePunch(IDPunch, oState)
    End Function


    Public Function GetAllowedTasksByEmployee(ByVal IDEmployee As Long, ByVal oState As roWsState, ByVal checkActualTask As Boolean, ByVal actualTaskId As Integer) As roGenericVtResponse(Of roTask()) Implements IPunchSvc.GetAllowedTasksByEmployee
        Return PunchMethods.GetAllowedTasksByEmployee(IDEmployee, oState, checkActualTask, actualTaskId)
    End Function

    Public Function GetAllowedTasksByEmployeeFiltered(ByVal IDEmployee As Long, ByVal oState As roWsState, ByVal checkActualTask As Boolean, ByVal actualTaskId As Integer, ByVal taskFilter As String) As roGenericVtResponse(Of roTask()) Implements IPunchSvc.GetAllowedTasksByEmployeeFiltered
        Return PunchMethods.GetAllowedTasksByEmployeeFiltered(IDEmployee, oState, checkActualTask, actualTaskId, taskFilter)
    End Function
    'Public Function GetAllowedTasksByEmployeeFiltered(ByVal IDEmployee As Long, ByVal oState As roWsState, ByVal checkActualTask As Boolean, ByVal actualTaskId As Integer, ByVal taskFilter As String) As roTask()
    '    If oState Is Nothing Then oState = New roPunchState(-1)
    '    oState.UpdateStateInfo()
    '    Return 
    'End Function


    Public Function GetLastTaskInfoByEmployee(ByVal IDEmployee As Long, ByVal oState As roWsState) As roGenericVtResponse(Of roLastTaskInfoByEmployee) Implements IPunchSvc.GetLastTaskInfoByEmployee
        Return PunchMethods.GetLastTaskInfoByEmployee(IDEmployee, oState)
    End Function

End Class
