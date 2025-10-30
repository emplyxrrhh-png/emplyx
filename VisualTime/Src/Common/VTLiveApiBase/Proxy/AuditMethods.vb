Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AuditState
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Audit
Imports Robotics.VTBase.Extensions

Public Class AuditMethods

    Public Shared Function GetAudit(ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByVal strUserName As String, ByVal ActionID As Action, ByVal ObjectType As ObjectType, ByVal strClientLocation As String, ByVal intPageNumber As Integer, ByVal intPageRows As Integer, ByVal strOrderField As String, ByVal bolOrderAsc As Boolean, ByVal intPagesCount As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New wscAuditState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport)
        If oPassport IsNot Nothing Then
            Dim ds As DataSet = Nothing
            Dim tb As DataTable = roAudit.GetAudit(oPassport.Language.Key, xBeginDate, xEndDate, strUserName, ActionID, ObjectType, strClientLocation, intPageNumber, intPageRows, strOrderField, bolOrderAsc, intPagesCount)
            If tb IsNot Nothing Then
                If tb.DataSet IsNot Nothing Then
                    ds = tb.DataSet
                Else
                    ds = New DataSet
                    ds.Tables.Add(tb)
                End If
            End If

            oResult.Value = ds
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de tipos de acciones auditadas.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAuditActions(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New wscAuditState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport)
        If oPassport IsNot Nothing Then
            Dim ds As DataSet = Nothing
            Dim tb As DataTable = roAudit.GetAuditActions(oPassport.Language.Key)
            If tb IsNot Nothing Then
                If tb.DataSet IsNot Nothing Then
                    ds = tb.DataSet
                Else
                    ds = New DataSet
                    ds.Tables.Add(tb)
                End If
            End If

            oResult.Value = ds
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista la tipo de objetos auditados.
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetAuditObjectTypes(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New wscAuditState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oState.IDPassport)
        If oPassport IsNot Nothing Then
            Dim ds As DataSet = Nothing
            Dim tb As DataTable = roAudit.GetAuditObjectTypes(oPassport.Language.Key)
            If tb IsNot Nothing Then
                If tb.DataSet IsNot Nothing Then
                    ds = tb.DataSet
                Else
                    ds = New DataSet
                    ds.Tables.Add(tb)
                End If
            End If

            oResult.Value = ds
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Audit(ByVal _Action As Audit.Action, ByVal _ObjectType As Audit.ObjectType, ByVal _ObjectName As String, ByVal _ParametersName As List(Of String), ByVal _ParametersValue As List(Of String), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New wscAuditState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roLiveSupport.Audit(_Action, _ObjectType, _ObjectName, _ParametersName, _ParametersValue, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

End Class