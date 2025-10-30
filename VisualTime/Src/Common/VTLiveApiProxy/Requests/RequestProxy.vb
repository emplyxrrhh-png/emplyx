Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Security

Public Class RequestProxy
    Implements IRequestSvc

    Public Function KeepAlive() As Boolean Implements IRequestSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Obtiene la solicitud (Requests) con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID de solicitud</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve la solicitud (roRequest)</returns>
    ''' <remarks></remarks>

    Public Function GetRequestByID(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roRequest) Implements IRequestSvc.GetRequestByID
        Return RequestMethods.GetRequestByID(ID, oState, bAudit)
    End Function


    Public Function GetRequestPendingSupervisors(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of String) Implements IRequestSvc.GetRequestPendingSupervisors

        Return RequestMethods.GetRequestPendingSupervisors(ID, oState, bAudit)
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

    Public Function GetRequestsByEmployee(ByVal IDEmployee As Integer, ByVal SQLFilter As String, ByVal SQLOrder As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IRequestSvc.GetRequestsByEmployee

        Return RequestMethods.GetRequestsByEmployee(IDEmployee, SQLFilter, SQLOrder, oState)
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

    Public Function GetRequestsSupervisor(ByVal _IDPassport As Integer, ByVal _SQLFilter As String, ByVal _SQLOrder As String, ByVal NumRequestToLoad As Integer, ByVal LevelsBelow As String,
                                              ByVal IdCause As Integer, ByVal IdSupervisor As Integer, ByVal bIncludeAutomaticRequests As Boolean, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IRequestSvc.GetRequestsSupervisor
        Return RequestMethods.GetRequestsSupervisor(_IDPassport, _SQLFilter, _SQLOrder, NumRequestToLoad, LevelsBelow, IdCause, IdSupervisor, bIncludeAutomaticRequests, _Audit, oState)
    End Function

    ''' <summary>Devuelve un Datatable con el total de tipos de solicitudes por Niveles a las que tiene acceso un supervisor o validador</summary>
    ''' <param name="_IDPassport">ID de passaporte del usuario supervisor</param>
    ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'RequestType = 1 And Reque...')</param>
    ''' <param name="oState"></param>

    Public Function GetRequestsSupervisorCountByType(ByVal _IDPassport As Integer, ByVal _SQLFilter As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IRequestSvc.GetRequestsSupervisorCountByType
        Return RequestMethods.GetRequestsSupervisorCountByType(_IDPassport, _SQLFilter, oState)
    End Function

    ''' <summary>
    ''' Devuelve un Datatable con todos los empleados que puede gestionar o validar un supervisor (Lista de Empleados)
    ''' </summary>
    ''' <param name="_IDPassport">ID de passaporte del usuario supervisor</param>
    ''' <param name="_SQLFilter">Filtro SQL para el Where</param>
    ''' <param name="_Status">Si el empleado esta dentro o fuera IN, OUT</param>
    ''' <param name="_SQLOrder">Ordenación SQL</param>
    ''' <param name="_IncludeImages">Indica si queremos que retorne la foto del empleado</param>
    ''' <param name="_Audit">Auditar consulta masiva</param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetListEmployeesSupervisor(ByVal _IDPassport As Integer, ByVal _SQLFilter As String, ByVal _Status As String, ByVal _SQLOrder As String, ByVal _Audit As Boolean, ByVal _IncludeImages As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IRequestSvc.GetListEmployeesSupervisor
        Return RequestMethods.GetListEmployeesSupervisor(_IDPassport, _SQLFilter, _Status, _SQLOrder, _Audit, _IncludeImages, oState)
    End Function

    ''' <summary>
    ''' Valida los datos de la solicitud.<br/>
    ''' Comprueba que:<br/>
    ''' </summary>
    ''' <param name="oRequest">La solicitud a validar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha validado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>

    Public Function ValidateRequest(ByVal oRequest As roRequest, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IRequestSvc.ValidateRequest
        Return RequestMethods.ValidateRequest(oRequest, oState)
    End Function

    ''' <summary>
    ''' Guarda los datos de la solicitud. Si és nuevo, se actualiza el ID de la solicitud pasado.<br/>
    ''' Se comprueba ValidateRequest()<br/>
    ''' </summary>
    ''' <param name="oRequest">Solicitud a guardar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha guardado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function SaveRequest(ByVal oRequest As roRequest, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IRequestSvc.SaveRequest
        Return RequestMethods.SaveRequest(oRequest, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina la solicitud con el ID indicado<br/>
    ''' Realiza lo siguiente:<br/>
    ''' </summary>
    ''' <param name="ID">ID de solicitud a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo.</remarks>

    Public Function DeleteRequest(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IRequestSvc.DeleteRequest
        Return RequestMethods.DeleteRequest(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Devuelve la definición de seguridad para un tipo de solicitud.
    ''' </summary>
    ''' <param name="eRequestType">Tipo de solicitud</param>
    ''' <param name="oState"></param>
    ''' <returns>Objeto 'roRequestTypeSecurity' con la información necesaria para determinar los permisos a aplicar en función del tipo de solicitud.</returns>
    ''' <remarks></remarks>

    Public Function GetRequestTypeSecurity(ByVal eRequestType As eRequestType, ByVal oState As roWsState) As roGenericVtResponse(Of roRequestTypeSecurity) Implements IRequestSvc.GetRequestTypeSecurity
        Return RequestMethods.GetRequestTypeSecurity(eRequestType, oState)
    End Function

    ''' <summary>
    ''' Devuelve la lista de definiciones de seguridad para todos los tipos de solicitud.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetRequestTypeSecurityListAll(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roRequestTypeSecurity)) Implements IRequestSvc.GetRequestTypeSecurityListAll
        Return RequestMethods.GetRequestTypeSecurityListAll(oState)
    End Function

    ''' <summary>
    ''' Devuelve la lista de definiciones de seguridad para los tipo de solicitud indicados.
    ''' </summary>
    ''' <param name="RequestTypes"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetRequestTypeSecurityList(ByVal RequestTypes As eRequestType(), ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roRequestTypeSecurity)) Implements IRequestSvc.GetRequestTypeSecurityList
        Return RequestMethods.GetRequestTypeSecurityList(RequestTypes, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de tipos de solicitudes con sus identificadores y descripciones
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetRequestTypes(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IRequestSvc.GetRequestTypes
        Return RequestMethods.GetRequestTypes(oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de estados de solicitudes con sus identificadores y descripciones
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetRequestStates(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IRequestSvc.GetRequestStates
        Return RequestMethods.GetRequestStates(oState)
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

    Public Function ApproveRefuse(ByVal _IDRequest As Integer, ByVal _IDPassport As Integer, ByVal _ApproveRefuse As Boolean, ByVal _Comments As String, ByVal _CheckLockedDays As Boolean, ByVal _forceApprove As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IRequestSvc.ApproveRefuse
        Return RequestMethods.ApproveRefuse(_IDRequest, _IDPassport, _ApproveRefuse, _Comments, _CheckLockedDays, _forceApprove, oState)
    End Function

    ''' <summary>
    ''' Obtiene la descripción de la solicitud
    ''' </summary>
    ''' <param name="_IDRequest">Número de solicitud</param>
    ''' <param name="bolDetail"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetRequestInfo(ByVal _IDRequest As Integer, ByVal bolDetail As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IRequestSvc.GetRequestInfo
        Return RequestMethods.GetRequestInfo(_IDRequest, bolDetail, oState)
    End Function


    Public Function GetFilterRequests(ByVal IdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IRequestSvc.GetFilterRequests
        Return RequestMethods.GetFilterRequests(IdPassport, oState)
    End Function


    Public Function SetFilterRequests(ByVal IdPassport As Integer, ByVal Filter As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IRequestSvc.SetFilterRequests
        Return RequestMethods.SetFilterRequests(IdPassport, Filter, oState)
    End Function

End Class
