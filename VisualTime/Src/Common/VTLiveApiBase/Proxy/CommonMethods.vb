Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class CommonMethods

    Public Shared Function GetRuntimeId() As roGenericVtResponse(Of String)
        Dim _DefaultLanguage As New roGenericVtResponse(Of String)

        Dim sSql = "@SELECT# Data from sysroParameters where ID='RuntimeID'"
        _DefaultLanguage.Value = roTypes.Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSql))

        Return _DefaultLanguage
    End Function

    Public Shared Function DefaultLanguage() As roGenericVtResponse(Of String)
        Dim _DefaultLanguage As New roGenericVtResponse(Of String)

        Dim securityState As New roSecurityState()
        _DefaultLanguage.Value = securityState.GetLanguageKey()

        Return _DefaultLanguage
    End Function

    Public Shared Function GetAdvancedParameter(ByVal parameterName As String, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAdvancedParameter)
        Dim bState = New roAdvancedParameterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAdvancedParameter)
        oResult.Value = New roAdvancedParameter(parameterName, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetCompanyAdvancedParameter(ByVal companyName As String, ByVal parameterName As String, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAdvancedParameter)
        Dim bState = New roAdvancedParameterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAdvancedParameter)
        oResult.Value = New roAdvancedParameter(companyName, parameterName, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetAdvancedParameterList(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAdvancedParameter))

        Dim bState = New roAdvancedParameterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roAdvancedParameter))
        oResult.Value = roAdvancedParameter.GetAdvancedParametersList("", bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetAdvancedParameterDataTable(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of DataTable)
        Dim bState = New roAdvancedParameterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataTable)

        Dim tb As DataTable = roAdvancedParameter.GetAdvancedParametersDataTable("", bState)

        oResult.Value = tb

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    '////byref oAdvancedParameter
    Public Shared Function SaveAdvancedParameter(ByVal oAdvancedParameter As roAdvancedParameter, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roAdvancedParameterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oAdvancedParameter.State = bState
        oResult.Value = oAdvancedParameter.Save(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAdvancedParameter.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetVisualTimeEdition() As roGenericVtResponse(Of String)
        Dim versionCode As New roGenericVtResponse(Of String)
        versionCode.Value = String.Empty

        Dim oServerLicense As New roServerLicense()

        If oServerLicense.FeatureIsInstalled("Version\LiveExpress") Then
            versionCode.Value = "EXPRESS"
        Else
            If oServerLicense.FeatureIsInstalled("Feature\ONE") Then
                versionCode.Value = "ONE"
            Else
                versionCode.Value = "LIVE"
            End If
        End If

        Return versionCode
    End Function

End Class