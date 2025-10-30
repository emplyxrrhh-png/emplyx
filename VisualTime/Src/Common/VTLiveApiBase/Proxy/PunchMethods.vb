Imports System.Data
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class PunchMethods

    Public Shared Function GetPunch(ByVal intIDPunch As Long, ByVal bLoadPhoto As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roPunch)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPunch)
        oResult.Value = New roPunch(-1, intIDPunch, bLoadPhoto, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult.Value.State, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function SavePunch(ByVal oPunch As roPunch, ByVal bolAutomaticBeginJobCheck As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        oPunch.State = bState

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oPunch.Save(, , bolAutomaticBeginJobCheck)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPunch.State, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function SavePunches(ByVal ds As DataSet, ByVal bolAutomaticBeginJobCheck As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim lstPunches As New roPunchList(bState)
        oResult.Value = lstPunches.Save(ds.Tables(0), bolAutomaticBeginJobCheck, True, False)

        If ds IsNot Nothing AndAlso ds.Tables(0) IsNot Nothing AndAlso ds.Tables(0).Rows.Count > 0 Then
            Dim oContext As New roCollection
            oContext.Add("User.ID", ds.Tables(0).Rows(0)("IDEmployee"))
            roConnector.InitTask(TasksType.MOVES, oContext)
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(lstPunches.State, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function DeletePunch(ByVal oPunch As roPunch, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()
        oPunch.State = bState

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oPunch.Delete()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPunch.State, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function DeletePunchByID(ByVal intIDPunch As Long, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False
        Dim oPunch As New roPunch(-1, intIDPunch, bState)
        If oState.Result = PunchResultEnum.NoError AndAlso oPunch IsNot Nothing Then
            bolRet = oPunch.Delete()
        End If

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPunch.State, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function GetPunchesDataTable(ByVal dShiftDateBegin As Date, ByVal dShiftDateEnd As Date, ByVal dPeriodBegin As DateTime, ByVal dPeriodEnd As DateTime, ByVal iIDEmployee As String, ByVal iIDTerminal As String, ByVal iIDReader As String, ByVal iIDCause As String,
                                        ByVal iIDZone As String, ByVal iIsNotReliable As Integer, ByVal sTypes As String, ByVal sActualTypes As String, ByVal sInvalidTypes As String, ByVal sActions As String, ByVal sOrderBy As String,
                                        ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tbRet As DataTable = roPunch.GetPunchesDataTable(bState, dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause, iIDZone, iIsNotReliable, sTypes, sActualTypes, sInvalidTypes, sActions, sOrderBy)
        If oState.Result = PunchResultEnum.NoError Then
            ds = New DataSet
            ds.Tables.Add(tbRet)
        Else
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function GetLastPunchDataTable(ByVal dShiftDateBegin As Date, ByVal dShiftDateEnd As Date, ByVal dPeriodBegin As DateTime, ByVal dPeriodEnd As DateTime, ByVal iIDEmployee As String, ByVal iIDTerminal As String, ByVal iIDReader As String, ByVal iIDCause As String,
                                  ByVal iIDZone As String, ByVal iIsNotReliable As Integer, ByVal sTypes As String, ByVal sActualTypes As String, ByVal sInvalidTypes As String, ByVal sActions As String,
                                  ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tbRet As DataTable = roPunch.GetLastPunchDataTable(bState, dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause, iIDZone, iIsNotReliable, sTypes, sActualTypes, sInvalidTypes, sActions)
        If oState.Result = PunchResultEnum.NoError Then
            ds = New DataSet
            ds.Tables.Add(tbRet)
        Else
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function GetPunchesCountDataTable(ByVal dShiftDateBegin As Date, ByVal dShiftDateEnd As Date, ByVal dPeriodBegin As DateTime, ByVal dPeriodEnd As DateTime, ByVal iIDEmployee As String, ByVal iIDTerminal As String, ByVal iIDReader As String, ByVal iIDCause As String,
                                  ByVal iIDZone As String, ByVal iIsNotReliable As Integer, ByVal sTypes As String, ByVal sActualTypes As String, ByVal sInvalidTypes As String, ByVal sActions As String, ByVal sOrderBy As String,
                                  ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = roPunch.GetPunchesDataTableCount(bState, dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause, iIDZone, iIsNotReliable, sTypes, sActualTypes, sInvalidTypes, sActions)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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

    Public Shared Function DoSequencePunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCaptureBytes() As Byte,
                                    ByVal _Lat As Double, ByVal _Lon As Double, ByVal _LocationZone As String, ByVal _fullAddress As String, ByVal _TimeZone As String, ByVal _SaveData As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roDoSequencePunch)

        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDoSequencePunch)
        Dim tmpResult As New roDoSequencePunch

        Dim oInputCapture As System.Drawing.Image = Nothing
        If _InputCaptureBytes IsNot Nothing Then
            oInputCapture = roTypes.Bytes2Image(_InputCaptureBytes)
        End If

        Dim _Punch As New roPunch(bState)
        Dim _InputType As PunchStatus
        Dim _SeqStatus As PunchSeqStatus
        tmpResult.Saved = roPunch.DoSequencePunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDReader, _IDCause, oInputCapture, _Punch, _InputType, _SeqStatus, _SaveData, bState, _Lat, _Lon, _LocationZone, _fullAddress, _TimeZone)

        tmpResult.Punch = _Punch
        tmpResult.Status = _InputType
        tmpResult.SeqStatus = _SeqStatus

        oResult.Value = tmpResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function DoTaskPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDTask As Integer, ByVal _InputCaptureBytes() As Byte, ByVal _Lat As Double,
                                ByVal _Lon As Double, ByVal _LocationZone As String, ByVal _fullAddress As String, ByVal _TimeZone As String, ByVal Field1 As String, ByVal Field2 As String, ByVal Field3 As String,
                                ByVal Field4 As Double, ByVal Field5 As Double, ByVal Field6 As Double, ByVal oState As roWsState) As roGenericVtResponse(Of roDoSequencePunch)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDoSequencePunch)
        Dim tmpResult As New roDoSequencePunch

        Dim oInputCapture As System.Drawing.Image = Nothing
        If _InputCaptureBytes IsNot Nothing Then
            oInputCapture = roTypes.Bytes2Image(_InputCaptureBytes)
        End If

        Dim _Punch As New roPunch(bState)
        Dim _InputType As PunchStatus
        tmpResult.Saved = roPunch.DoTaskPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDTask, oInputCapture, _Punch, _InputType, bState, _Lat, _Lon, _LocationZone, _fullAddress, _TimeZone, Field1, Field2, Field3, Field4, Field5, Field6)

        tmpResult.Punch = _Punch
        tmpResult.Status = _InputType

        oResult.Value = tmpResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function DoCostCenterPunch(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDCostCenter As Integer,
                                      ByVal _InputCaptureBytes() As Byte, ByVal _Lat As Double, ByVal _Lon As Double, ByVal _LocationZone As String, ByVal _fullAddress As String,
                                      ByVal _TimeZone As String, ByVal oState As roWsState) As roGenericVtResponse(Of roDoSequencePunch)

        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDoSequencePunch)
        Dim tmpResult As New roDoSequencePunch

        Dim oInputCapture As System.Drawing.Image = Nothing
        If _InputCaptureBytes IsNot Nothing Then
            oInputCapture = roTypes.Bytes2Image(_InputCaptureBytes)
        End If

        Dim _Punch As New roPunch(bState)
        Dim _InputType As PunchStatus
        tmpResult.Saved = roPunch.DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, oInputCapture, _Punch, _InputType, bState, _Lat, _Lon, _LocationZone, _fullAddress, _TimeZone)

        tmpResult.Punch = _Punch
        tmpResult.Status = _InputType

        oResult.Value = tmpResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function ChangeState(ByVal _IDEmployee As Integer, ByVal _NowDateTime As DateTime, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer,
                                ByVal _InputCaptureBytes() As Byte, ByVal _Punch As roPunch, ByVal _SaveData As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roPunch)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPunch)
        Dim oInputCapture As System.Drawing.Image = Nothing
        If _InputCaptureBytes IsNot Nothing Then
            oInputCapture = roTypes.Bytes2Image(_InputCaptureBytes)
        End If

        If roPunch.ChangeState(_IDEmployee, _NowDateTime, _InputDateTime, _IDTerminal, _IDReader, _IDCause, oInputCapture, _Punch, _SaveData, bState) Then
            oResult.Value = _Punch
        Else
            oResult.Value = Nothing
            bState.Result = PunchResultEnum.Exception
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function ReorderPunches(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roPunch.ReorderPunches(intIDEmployee, xDate, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '''' <summary>
    '''' Obtiene los fichajes de presencia de un empleado para una fecha.
    '''' </summary>
    '''' <param name="intIDEmployee">Código empleado.</param>
    '''' <param name="xDate">Fecha</param>
    '''' <param name="bolIncludeCaptures">Indica si se quieren obtener las imágenes de los fichajes (si existen).</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <remarks></remarks>
    Public Shared Function GetPunchesPresFromEmployee(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet)

        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tbPunches As DataTable = roPunch.GetPunchesPres(intIDEmployee, xDate, xDate, bolIncludeCaptures, bState, strFilter)
        If bState.Result = PunchResultEnum.NoError AndAlso tbPunches IsNot Nothing Then
            ds = New DataSet
            ds.Tables.Add(tbPunches)
        Else
            ' Copiar variable estado
            ' ...
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function GetPunchesPresFromEmployeeInPeriod(ByVal intIDEmployee As Integer, ByVal xDate As Date, ByVal xEndDate As Date, ByVal bolIncludeCaptures As Boolean, ByVal oState As roWsState, ByVal strFilter As String) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tbPunches As DataTable = roPunch.GetPunchesPres(intIDEmployee, xDate, xEndDate, bolIncludeCaptures, bState, strFilter)
        If bState.Result = PunchResultEnum.NoError AndAlso tbPunches IsNot Nothing Then
            ds = New DataSet
            ds.Tables.Add(tbPunches)
        Else
            ' Copiar variable estado
            ' ...
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
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
    Public Shared Function GetAccessPunchesStatus(ByVal Feature As String, ByVal Type As String, ByVal IDAccessGroups As Generic.List(Of Integer), ByVal IDZones As Generic.List(Of Integer),
                                           ByVal oState As roWsState, ByVal bColImage As Boolean, ByVal bOnlyCounters As Boolean) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        oResult.Value = roPunch.GetAccessPunchesStatus(bState, Feature, Type, IDAccessGroups, IDZones, bColImage, bOnlyCounters)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '''' <summary>
    '''' Obtiene los registros de la tabla 'InvalidEntries' que no se han podido importar en VisualTime.
    '''' </summary>
    '''' <param name="oState">Información de estado actual.</param>
    '''' <returns>Tabla con las columnas: DateTime, IDCredential, IDTerminal, Type, IDCause, ID, IDReader </returns>
    '''' <remarks></remarks>
    Public Shared Function GetInvalidEntries(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roPunch.GetInvalidEntries(bState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                ds = New DataSet
                ds.Tables.Add(tb)
            Else
                ds = tb.DataSet
            End If
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '''' <summary>
    '''' Obtiene los registros de la tabla 'Punches' sin empleado asignado.
    '''' </summary>
    '''' <param name="oState">Información de estado actual.</param>
    '''' <returns>Tabla con las columnas: DateTime, IDCredential, IDTerminal, Type, IDCause, ID, IDReader </returns>
    '''' <remarks></remarks>
    Public Shared Function GetInvalidPunches(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roPunch.GetInvalidPunches(bState)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                ds = New DataSet
                ds.Tables.Add(tb)
            Else
                ds = tb.DataSet
            End If
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '''' <summary>
    '''' Obtiene los registros de la tabla 'Punches' sin empleado asignado con una tarjeta concreta.
    '''' </summary>
    '''' <param name="oState">Información de estado actual.</param>
    '''' <param name="lngIDCard">tarjeta seleccionada</param>
    '''' <returns>Tabla con las columnas: DateTime, IDCredential, IDTerminal, Type, IDCause, ID, IDReader </returns>
    '''' <remarks></remarks>
    Public Shared Function GetInvalidPunchesByIDCard(ByVal oState As roWsState, ByVal lngIDCard As Long) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roPunch.GetInvalidPunches(bState, lngIDCard)
        If tb IsNot Nothing Then
            If tb.DataSet Is Nothing Then
                ds = New DataSet
                ds.Tables.Add(tb)
            Else
                ds = tb.DataSet
            End If
        End If
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '''' <summary>
    '''' Guarda los fichajes invalidos no asignados a ningún empleado pasados.<br/>
    '''' </summary>
    '''' <param name="ds">Dataset con una tabla que contiene los fichajes a guardar.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Shared Function SaveInvalidPunches(ByVal ds As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roPunch.SaveInvalidPunches(ds.Tables(0), bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '''' <summary>
    '''' Reprocesa fichajes no procesados que quedaron en InvalidEntries<br/>
    '''' </summary>
    '''' <param name="ds">Dataset con una tabla que contiene los fichajes a guardar.</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>True o False</returns>
    '''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
    Public Shared Function ReprocessInvalidEntries(ByVal aInvalidEntries() As DTOs.roPunchInvalidEntry, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roPunch.ReprocessInvalidEntries(aInvalidEntries, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '''' <summary>
    '''' Retorna dataset con los ultimos fichajes realizados de accesos validos e invalidos
    '''' </summary>
    '''' <param name="ListIdZones">Lista de zonas sobre las que obtener datos</param>
    '''' <param name="oState">Información adicional de estado.</param>
    '''' <returns>Dataset</returns>
    Public Shared Function GetAccessPunchesMonitor(ByVal oState As roWsState, ByVal ListIdZones As Generic.List(Of Integer), ByVal ListIdFields As Generic.List(Of String),
                                            ByVal bColImage As Boolean) As roGenericVtResponse(Of DataSet)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        oResult.Value = roPunch.GetAccessPunchesMonitor(bState, ListIdZones, ListIdFields, bColImage)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAllowedTasksByEmployee(ByVal IDEmployee As Long, ByVal oState As roWsState, ByVal checkActualTask As Boolean, ByVal actualTaskId As Integer) As roGenericVtResponse(Of roTask())
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTask())
        oResult.Value = roPunch.GetAllowTasksByEmployeeOnPunch(IDEmployee, bState, checkActualTask, actualTaskId).ToArray()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAllowedTasksByEmployeeFiltered(ByVal IDEmployee As Long, ByVal oState As roWsState, ByVal checkActualTask As Boolean, ByVal actualTaskId As Integer, ByVal taskFilter As String) As roGenericVtResponse(Of roTask())
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roTask())
        oResult.Value = roPunch.GetAllowTasksByEmployeeOnPunchFiltered(IDEmployee, bState, checkActualTask, actualTaskId, taskFilter).ToArray()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    'Public Shared Function GetAllowedTasksByEmployeeFiltered(ByVal IDEmployee As Long, ByVal oState As roWsState, ByVal checkActualTask As Boolean, ByVal actualTaskId As Integer, ByVal taskFilter As String) As roTask()
    '    If oState Is Nothing Then oState = New roPunchState(-1)
    '    oState.UpdateStateInfo()
    '    Return
    'End Function

    Public Shared Function GetLastTaskInfoByEmployee(ByVal IDEmployee As Long, ByVal oState As roWsState) As roGenericVtResponse(Of roLastTaskInfoByEmployee)
        Dim bState = New roPunchState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roLastTaskInfoByEmployee)
        Dim tmpResult As New roLastTaskInfoByEmployee

        Dim oPunch As New roPunch(bState)
        oPunch.IDEmployee = IDEmployee
        Dim lastPunchID As Integer = -1
        Dim lastPunchDateTime As DateTime = DateTime.Now
        Dim oLastPunchType As PunchTypeEnum = PunchTypeEnum._OUT
        oPunch.GetLastPunchTask(oLastPunchType, lastPunchDateTime, lastPunchID)

        tmpResult.LastPunchDateTime = lastPunchDateTime
        tmpResult.LastPunchType = oLastPunchType
        tmpResult.LastPunchID = lastPunchID

        oResult.Value = tmpResult

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPunch.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class