Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTCommuniques
Imports Robotics.Portal.Business
Imports Robotics.VTBase.Extensions

Public Class PortalBaseMethods

    Public Shared Function GetMainMenu(ByVal strAppName As String, ByVal intIDPassport As Integer, ByVal strFeatureType As FeatureTypes, ByVal strLanguatgeReference As String, ByVal oLicense As roVTLicense, ByVal oState As roWsState) As roGenericVtResponse(Of wscMenuElementList)

        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of wscMenuElementList)
        Dim objMenu As New wscMainMenu(strAppName, intIDPassport, strFeatureType, strLanguatgeReference, oLicense, bState)
        Dim oMenuList As wscMenuElementList = objMenu.MainMenu()
        oResult.Value = oMenuList

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetServerLicencse(ByVal oState As roWsState) As roGenericVtResponse(Of roVTLicense)

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roVTLicense)
        Dim oLicSupport = New roLicenseSupport()
        oResult.Value = oLicSupport.GetVTLicenseInfo()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el número total de empleados en estado activo a la fecha indicada.
    ''' </summary>
    ''' <param name="xDateTime">Fecha indicada a recuperar el total de empleados en activo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Número de empleados en estado activo</returns>
    ''' <remarks></remarks>

    Public Shared Function GetActiveEmployeesCount(ByVal xDateTime As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = New roLicenseSupport().GetActiveEmployeesCount(xDateTime)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el número total de empleados en estado activo y con contrato a la fecha indicada.
    ''' </summary>
    ''' <param name="xDateTime">Fecha indicada a recuperar el total de empleados en activo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Número de empleados en estado activo</returns>
    ''' <remarks></remarks>

    Public Shared Function GetActiveJobEmployeesCount(ByVal xDateTime As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = New roLicenseSupport().GetActiveJobEmployeesCount(xDateTime)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Comprueba si no se pasa de los limites de la licencia en una fecha concreta
    ''' </summary>
    ''' <param name="xDateTime">Fecha indicada a recuperar el total de empleados en activo</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <param name="oLicenseService">Información de la lu de estado.</param>
    ''' <returns>Número de empleados en estado activo</returns>
    ''' <remarks></remarks>

    Public Shared Function CheckLicenseLimits(ByVal xDateTime As DateTime, ByVal oState As roWsState, ByVal oLicenseService As roVTLicense) As roGenericVtResponse(Of Boolean)

        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oLicSupport As New roLicenseSupport()
        oResult.Value = oLicSupport.CheckLicenseLimits(xDateTime, oLicenseService)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetMenuItem(ByVal strAppName As String, ByVal strItemPath As String, ByVal strFeatureType As FeatureTypes, ByVal intIDPassport As Integer, ByVal strLanguatgeReference As String, ByVal oLicense As roVTLicense, ByVal oState As roWsState) As roGenericVtResponse(Of wscMenuElement)

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of wscMenuElement)
        Dim objMenu As New wscMainMenu(strAppName, intIDPassport, strFeatureType, strLanguatgeReference, oLicense, bState)
        Dim oMenuElement As wscMenuElement = objMenu.MenuItem(strItemPath)
        If strItemPath = "/Communique" Then
            Dim oServerLicense As New roServerLicense
            Dim oCommunicateManager As New roCommuniqueManager(New roCommuniqueState(intIDPassport))
            If oServerLicense.FeatureIsInstalled("Feature\AdvancedCommuniques") = False AndAlso oCommunicateManager.GetAllCommuniques().Count = 0 Then
                oMenuElement = Nothing
            End If
        End If
        oResult.Value = oMenuElement

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetPathActions(ByVal strGUIPath As String, ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGuiAction))

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roGuiAction))
        Dim oActionsList As New Generic.List(Of roGuiAction)
        Dim tmpActList As Generic.List(Of GuiAction) = GuiAction.GetActionsBySection(strGUIPath, bState)

        For Each oList As GuiAction In tmpActList
            If oList.GuiAction IsNot Nothing Then oActionsList.Add(oList.GuiAction)
        Next

        oResult.Value = oActionsList

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function


End Class