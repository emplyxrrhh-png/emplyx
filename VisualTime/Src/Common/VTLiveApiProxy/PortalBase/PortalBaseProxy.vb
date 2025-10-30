Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Portal.Business
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class PortalBaseProxy
    Implements IPortalBaseSvc

    Public Function KeepAlive() As Boolean Implements IPortalBaseSvc.KeepAlive
        Return True
    End Function

    Public Function GetMainMenu(ByVal strAppName As String, ByVal intIDPassport As Integer, ByVal strFeatureType As FeatureTypes, ByVal strLanguatgeReference As String, ByVal oLicense As roVTLicense, ByVal oState As roWsState) As roGenericVtResponse(Of wscMenuElementList) Implements IPortalBaseSvc.GetMainMenu

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

    Public Function GetServerLicencse(ByVal oState As roWsState) As roGenericVtResponse(Of roVTLicense) Implements IPortalBaseSvc.GetServerLicencse

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roVTLicense)
        Dim oLicSupport = New roLicenseSupport(bState.Log)
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

    Public Function GetActiveEmployeesCount(ByVal xDateTime As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IPortalBaseSvc.GetActiveEmployeesCount

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = New roLicenseSupport(bState.Log).GetActiveEmployeesCount(xDateTime)

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

    Public Function GetActiveJobEmployeesCount(ByVal xDateTime As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements IPortalBaseSvc.GetActiveJobEmployeesCount

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        oResult.Value = New roLicenseSupport(bState.Log).GetActiveJobEmployeesCount(xDateTime)

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

    Public Function CheckLicenseLimits(ByVal xDateTime As DateTime, ByVal oState As roWsState, ByVal oLicenseService As roVTLicense) As roGenericVtResponse(Of Boolean) Implements IPortalBaseSvc.CheckLicenseLimits

        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oLicSupport As New roLicenseSupport(bState.Log)
        oResult.Value = oLicSupport.CheckLicenseLimits(xDateTime, oLicenseService)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function


    Public Function GetMenuItem(ByVal strAppName As String, ByVal strItemPath As String, ByVal strFeatureType As FeatureTypes, ByVal intIDPassport As Integer, ByVal strLanguatgeReference As String, ByVal oLicense As roVTLicense, ByVal oState As roWsState) As roGenericVtResponse(Of wscMenuElement) Implements IPortalBaseSvc.GetMenuItem

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of wscMenuElement)
        Dim objMenu As New wscMainMenu(strAppName, intIDPassport, strFeatureType, strLanguatgeReference, oLicense, bState)
        Dim oMenuElement As wscMenuElement = objMenu.MenuItem(strItemPath)
        oResult.Value = oMenuElement

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function


    Public Function GetPathActions(ByVal strGUIPath As String, ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roGuiAction)) Implements IPortalBaseSvc.GetPathActions


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


    Public Function CheckFtpConnection(ByVal strLocation As String, ByVal username As String, ByVal password As String, ByVal isSFTP As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPortalBaseSvc.CheckFtpConnection

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Try
            oResult.Value = roLiveSupport.CheckFtpConnection(strLocation, username, password, isSFTP)
        Catch ex As Exception
            oState.Result = GuiStateResultEnum.Exception
            oResult.Value = False
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult


    End Function


    Public Function CheckWriteOnDirectory(ByVal strLocation As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPortalBaseSvc.CheckWriteOnDirectory

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Try
            oResult.Value = roLiveSupport.CheckWriteOnDirectory(strLocation)
        Catch ex As Exception
            oState.Result = GuiStateResultEnum.Exception
            oResult.Value = False
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function


    Public Function GetAvailableLocations(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IPortalBaseSvc.GetAvailableLocations

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roLiveSupport.GetAvailableLocations()

        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If

        Else
            oState.Result = GuiStateResultEnum.Exception

        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function


    Public Function SaveAvailableLocations(ByVal tbLocations As DataSet, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IPortalBaseSvc.SaveAvailableLocations

        'cambio mi state genérico a un estado especifico
        Dim bState = New wscGuiState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        If roLiveSupport.SaveAvailableLocations(tbLocations.Tables(0)) Then
            oResult.Value = True
        Else
            oState.Result = GuiStateResultEnum.Exception
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function


End Class
