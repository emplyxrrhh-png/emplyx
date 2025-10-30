Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AuditState
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Audit

Public Class AuditProxy
    Implements IAuditSvc

    Public Function KeepAlive() As Boolean Implements IAuditSvc.KeepAlive
        Return True
    End Function

    Public Function GetAudit(ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal strUserName As String, ByVal ActionID As Action, ByVal ObjectType As ObjectType, ByVal strClientLocation As String, ByVal intPageNumber As Integer, ByVal intPageRows As Integer, ByVal strOrderField As String, ByVal bolOrderAsc As Boolean, ByVal intPagesCount As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAuditSvc.GetAudit
        Return AuditMethods.GetAudit(xBeginDate, xEndDate, strUserName, ActionID, ObjectType, strClientLocation, intPageNumber, intPageRows, strOrderField, bolOrderAsc, intPagesCount, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de tipos de acciones auditadas.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAuditActions(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAuditSvc.GetAuditActions
        Return AuditMethods.GetAuditActions(oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista la tipo de objetos auditados.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAuditObjectTypes(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAuditSvc.GetAuditObjectTypes
        Return AuditMethods.GetAuditObjectTypes(oState)
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Audit(ByVal _Action As Audit.Action, ByVal _ObjectType As Audit.ObjectType, ByVal _ObjectName As String, ByVal _ParametersName As List(Of String), ByVal _ParametersValue As List(Of String), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IAuditSvc.Audit
        Return AuditMethods.Audit(_Action, _ObjectType, _ObjectName, _ParametersName, _ParametersValue, oState)
    End Function


End Class
