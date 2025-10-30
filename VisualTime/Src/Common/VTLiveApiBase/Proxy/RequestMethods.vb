Imports System.Data
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Security
Imports Robotics.Security.Base

Public Class RequestMethods

    ''' <summary>
    ''' Obtiene la solicitud (Requests) con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID de solicitud</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve la solicitud (roRequest)</returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roRequest)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roRequest)
        oResult.Value = New roRequest(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetRequestPendingSupervisors(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of String)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = roRequest.GetRequestNextSupervisorsNames(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve todas las solicitudes del empleado
    ''' </summary>
    ''' <param name="IDEmployee">ID del empleado a recuperar las solicitudes</param>
    ''' <param name="SQLFilter">Filtro SQL para el Where (ejemplo: 'RequestType = 1 And Reque...')</param>
    ''' <param name="SQLOrder">Ordenación SQL (ejemplo: 'RequestType ASC' o 'RequestDate ASC')</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve un DataSet con la tabla Request ordenado por Name </returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestsByEmployee(ByVal IDEmployee As Integer, ByVal SQLFilter As String, ByVal SQLOrder As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roRequest.GetRequestsByEmployee(IDEmployee, SQLFilter, SQLOrder, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve un Datatable con todos las solicitudes a las que tiene acceso un supervisor o validador
    ''' </summary>
    ''' <param name="_IDPassport">ID de passaporte del usuario supervisor</param>
    ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'RequestType = 1 And Reque...')</param>
    ''' <param name="_SQLOrder">Ordenación SQL (ejemplo: 'RequestType ASC' o 'RequestDate ASC')</param>
    ''' <param name="_Audit">Auditar consulta masiva</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestsSupervisor(ByVal _IDPassport As Integer, ByVal _SQLFilter As String, ByVal _SQLOrder As String, ByVal NumRequestToLoad As Integer, ByVal LevelsBelow As String,
                                              ByVal IdCause As Integer, ByVal IdSupervisor As Integer, ByVal bIncludeAutomaticRequests As Boolean, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roRequest.GetRequestsSupervisor(_IDPassport, _SQLFilter, _SQLOrder, NumRequestToLoad, LevelsBelow, IdCause, IdSupervisor, bIncludeAutomaticRequests, bState, _Audit, canSeeRequestsWithReadPermission:=True)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else

                For Each oRequestRow As DataRow In tb.Rows
                    If oRequestRow("RequestType") = eRequestType.DailyRecord Then
                        Dim oDailyRecord As roDailyRecord = Nothing
                        Dim oDailyRecordManager As New VTDailyRecord.roDailyRecordManager(New VTDailyRecord.roDailyRecordState(oState.IDPassport))
                        oDailyRecord = oDailyRecordManager.LoadDailyRecord(oRequestRow("ID"), Nothing)
                        oRequestRow("RequestInfo") = oDailyRecord.DailyRecordInfo
                    End If
                Next

                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetRequestsDashboardResume(ByVal _IDPassport As Integer, ByVal _SQLFilter As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roRequest.GetRequestsDashboardResume(_IDPassport, bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function


    ''' <summary>
    ''' Valida los datos de la solicitud.<br/>
    ''' Comprueba que:<br/>
    ''' </summary>
    ''' <param name="oRequest">La solicitud a validar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha validado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Shared Function ValidateRequest(ByVal oRequest As roRequest, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oRequest.Validate()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos de la solicitud. Si és nuevo, se actualiza el ID de la solicitud pasado.<br/>
    ''' Se comprueba ValidateRequest()<br/>
    ''' </summary>
    ''' <param name="oRequest">Solicitud a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha guardado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Shared Function SaveRequest(ByVal oRequest As roRequest, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oRequest.State = bState
        oResult.Value = oRequest.Save(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Elimina la solicitud con el ID indicado<br/>
    ''' Realiza lo siguiente:<br/>
    ''' </summary>
    ''' <param name="ID">ID de solicitud a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Shared Function DeleteRequest(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oRequest As New roRequest(ID, bState, False)

        oResult.Value = oRequest.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la definición de seguridad para un tipo de solicitud.
    ''' </summary>
    ''' <param name="eRequestType">Tipo de solicitud</param>
    ''' <param name="oState"></param>
    ''' <returns>Objeto 'roRequestTypeSecurity' con la información necesaria para determinar los permisos a aplicar en función del tipo de solicitud.</returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestTypeSecurity(ByVal eRequestType As eRequestType, ByVal oState As roWsState) As roGenericVtResponse(Of roRequestTypeSecurity)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roRequestTypeSecurity)
        oResult.Value = New roRequestTypeSecurity(eRequestType, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la lista de definiciones de seguridad para todos los tipos de solicitud.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestTypeSecurityListAll(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roRequestTypeSecurity))

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roRequestTypeSecurity))
        oResult.Value = New Generic.List(Of roRequestTypeSecurity)

        Try
            'cambio mi state genérico a un estado especifico
            Dim bState = New roRequestState(-1)
            roWsStateManager.CopyTo(oState, bState)
            bState.UpdateStateInfo()

            For Each eRequestType As eRequestType In System.Enum.GetValues(GetType(eRequestType))
                oResult.Value.Add(New roRequestTypeSecurity(eRequestType, bState))
            Next

            Dim newGState As New roWsState
            roWsStateManager.CopyTo(bState, newGState)
            oResult.Status = newGState
        Catch ex As Exception

        End Try

        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve la lista de definiciones de seguridad para los tipo de solicitud indicados.
    ''' </summary>
    ''' <param name="RequestTypes"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestTypeSecurityList(ByVal RequestTypes As eRequestType(), ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roRequestTypeSecurity))

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roRequestTypeSecurity))
        oResult.Value = New Generic.List(Of roRequestTypeSecurity)

        For Each eRequestType As eRequestType In RequestTypes
            oResult.Value.Add(New roRequestTypeSecurity(eRequestType, bState))
        Next

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene la lista de tipos de solicitudes con sus identificadores y descripciones
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestTypes(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport)

        If oPassport IsNot Nothing Then
            Dim tb As DataTable = roRequest.GetRequestTypes(oPassport.Language.Key)

            If tb IsNot Nothing Then
                If tb.DataSet IsNot Nothing Then
                    ds = tb.DataSet
                Else
                    ds = New DataSet
                    ds.Tables.Add(tb)
                End If
            End If

            oResult.Value = ds
        Else
            bState.Result = RequestResultEnum.InvalidPassport

        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Obtiene la lista de estados de solicitudes con sus identificadores y descripciones
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestStates(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport)

        If oPassport IsNot Nothing Then
            Dim tb As DataTable = roRequest.GetRequestStates(oPassport.Language.Key)

            If tb IsNot Nothing Then
                If tb.DataSet IsNot Nothing Then
                    ds = tb.DataSet
                Else
                    ds = New DataSet
                    ds.Tables.Add(tb)
                End If
            End If

            oResult.Value = ds
        Else
            bState.Result = RequestResultEnum.InvalidPassport

        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Aprueba o denega la solicitud.
    ''' </summary>
    ''' <param name="_IDRequest">Número de solicitud</param>
    ''' <param name="_IDPassport">Código del passport asociado al supervisor</param>
    ''' <param name="_ApproveRefuse">True - aprobar, False - denegar</param>
    ''' <param name="_CheckLockedDays">Sólo para solicitudes de tipo planificación: True- verifica si hay algún día bloqueado y devuelve un error si lo hay, False- no verifica si hay algún día bloqueado y planifica todo el periodo. </param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function ApproveRefuse(ByVal _IDRequest As Integer, ByVal _IDPassport As Integer, ByVal _ApproveRefuse As Boolean, ByVal _Comments As String, ByVal _CheckLockedDays As Boolean, ByVal _forceApprove As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oRequest As New roRequest(_IDRequest, bState, False)

        If oRequest.State.Result = RequestResultEnum.NoError Then
            oResult.Value = oRequest.ApproveRefuse(_IDPassport, _ApproveRefuse, _Comments, _CheckLockedDays, _forceApprove)

            Dim newGState As New roWsState
            roWsStateManager.CopyTo(oRequest.State, newGState)
            oResult.Status = newGState

            Return oResult
        Else
            oResult.Value = False
            Return oResult
        End If

    End Function

    ''' <summary>
    ''' Obtiene la descripción de la solicitud
    ''' </summary>
    ''' <param name="_IDRequest">Número de solicitud</param>
    ''' <param name="bolDetail"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetRequestInfo(ByVal _IDRequest As Integer, ByVal bolDetail As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        Dim oRequest As New roRequest(_IDRequest, bState, False)

        If oRequest.State.Result = RequestResultEnum.NoError Then

            If oRequest.RequestType <> eRequestType.DailyRecord Then
                oResult.Value = oRequest.RequestInfo(bolDetail)
            Else
                Dim oDailyRecord As roDailyRecord = Nothing
                Dim oDailyRecordManager As New VTDailyRecord.roDailyRecordManager(New VTDailyRecord.roDailyRecordState(oState.IDPassport))
                oDailyRecord = oDailyRecordManager.LoadDailyRecord(oRequest.ID, oRequest)
                oResult.Value = oDailyRecord.DailyRecordInfo
            End If

            Dim newGState As New roWsState
            roWsStateManager.CopyTo(oRequest.State, newGState)
            oResult.Status = newGState

            Return oResult
        Else
            oResult.Value = ""
            Return oResult
        End If

    End Function

    Public Shared Function GetFilterRequests(ByVal IdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        Dim strFilter As String = roRequest.GetFilterRequests(IdPassport, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        oResult.Value = strFilter

        Return oResult
    End Function

    Public Shared Function SetFilterRequests(ByVal IdPassport As Integer, ByVal Filter As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roRequestState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolret As Boolean = roRequest.SetFilterRequests(IdPassport, Filter, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        oResult.Value = bolret

        Return oResult

    End Function

End Class