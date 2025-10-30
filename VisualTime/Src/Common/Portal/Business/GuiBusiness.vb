'Imports System.Drawing
Imports Robotics.Base.DTOs
Imports Robotics.Portal.DataAccess
Imports Robotics.Portal.DataSets
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

''' <summary>
''' Gives access to an application menus layout and checks prerequisites.
''' </summary>
Public Class GuiBusiness

#Region " Declarations / New "

    Private _dataSet As New GuiDataSet()
    Private _idPassport As Integer
    Private _featureType As String
    Private _isProductivUser As Boolean
    Private _oState As VTBase.roBaseState
    Private _oServerLicense As roVTLicense = Nothing

    ''' <summary>
    ''' Initializes a new instance of the GuiBusiness class for the specified application.
    ''' </summary>
    ''' <param name="applicationName">The application for which to load menu entries.</param>
    ''' <param name="idPassport">The logged in passport's id.</param>
    ''' <param name="featureType">'U' to load elements of users, 'E' for employees.</param>
    Public Sub New(ByVal applicationName As String, ByVal idPassport As Integer, ByVal isProductivUser As Boolean, ByVal featureType As String, ByVal oLicense As roVTLicense, ByVal oState As VTBase.roBaseState)
        _idPassport = idPassport
        _featureType = featureType
        _isProductivUser = isProductivUser
        _oState = oState
        _oServerLicense = oLicense
        GuiAccess.GetGui(_dataSet.Gui, applicationName)
        DeleteInvalidEntries()
    End Sub

    'Public Sub New()

    'End Sub

#End Region

#Region " DeleteInvalidEntries "

    ''' <summary>
    ''' Deletes entries for which prerequisites are not met by specified user.
    ''' </summary>
    Private Sub DeleteInvalidEntries()

        Try
            Dim bolDelete As Boolean = False
            Dim oLicSupport As New roLicenseSupport()

            Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()
            Dim bLicenseLimitsCorrect As Boolean = oLicSupport.CheckLicenseLimits(DateTime.Now.Date, _oServerLicense)

            Dim isRoboticsUser As Boolean = roPassportManager.IsRoboticsUserOrConsultant(_idPassport)
            Dim isJune6610 As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId), "June6610"))
            Dim isSDKEnabled As Boolean = VTBase.roTypes.Any2Boolean(DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CompanyId), "IsSDKEnabled"))

            For Each row As GuiDataSet.GuiRow In _dataSet.Gui.Rows
                ' Por defecto no borro
                bolDelete = False

                If row.RowState <> DataRowState.Deleted Then
                    If oLicInfo.Edition <> roServerLicense.roVisualTimeEdition.NotSet Then
                        Dim strEdition As String = String.Empty
                        If Not row.IsEditionNull Then
                            strEdition = roTypes.Any2String(row.Edition)
                        End If
                        Select Case oLicInfo.Edition
                            Case roServerLicense.roVisualTimeEdition.One
                                If Not strEdition.Contains("One") Then
                                    bolDelete = True
                                End If
                            Case roServerLicense.roVisualTimeEdition.Starter
                                If Not strEdition.Contains("Starter") Then
                                    bolDelete = True
                                End If
                            Case roServerLicense.roVisualTimeEdition.Advanced
                                If Not strEdition.Contains("Advanced") Then
                                    bolDelete = True
                                End If
                            Case roServerLicense.roVisualTimeEdition.Premium
                                If Not strEdition.Contains("Premium") Then
                                    bolDelete = True
                                End If
                        End Select
                    End If

                    If Not bolDelete Then
                        If Not row.IsRequiredFeaturesNull AndAlso row.RequiredFeatures.Trim.Length > 0 AndAlso Not LicenseCheck.IsAuthorized(row.RequiredFeatures, _idPassport) Then
                            bolDelete = True
                        ElseIf Not row.IsRequiredFunctionalitiesNull() AndAlso row.RequiredFunctionalities.Trim.Length > 0 AndAlso Not FeaturesCheck.IsAuthorized(row.RequiredFunctionalities, _featureType, _idPassport) Then
                            bolDelete = True
                        Else
                            If Not row.IsParametersNull AndAlso row.Parameters.Trim.Length > 0 Then
                                If Not _isProductivUser AndAlso row.Parameters.Contains("ProductivEmployee") Then
                                    bolDelete = True
                                ElseIf row.Parameters.Contains("MonoTenant") Then
                                    bolDelete = True
                                ElseIf row.Parameters.Contains("OnPremise") Then
                                    bolDelete = True
                                ElseIf row.Parameters.Contains("MultiTenant") Then
                                    bolDelete = False
                                ElseIf row.Parameters.Contains("RoboticsEmployee") Then
                                    If (isRoboticsUser) Then
                                        bolDelete = False
                                    Else
                                        bolDelete = True
                                    End If
                                ElseIf row.Parameters.Contains("Admin") Then
                                    If (roPassportManager.GetPassportTicket(_idPassport, LoadType.Passport).IDParentPassport = 3) Then
                                        bolDelete = False
                                    Else
                                        bolDelete = True
                                    End If
                                ElseIf row.Parameters.Contains("admin") Then
                                    If isSDKEnabled Then
                                        bolDelete = False
                                    Else
                                        bolDelete = True
                                    End If
                                ElseIf row.Parameters.ToLower.Contains("securityv") Then

                                    Dim reqParameters = row.Parameters.ToLower.Split(";")

                                    bolDelete = True
                                    For Each param In reqParameters
                                        Dim securityModeRequiered = roTypes.Any2Integer(param.ToLower.Replace("securityv", ""))

                                        If securityModeRequiered = 3 Then
                                            bolDelete = False
                                        End If

                                    Next

                                ElseIf row.Parameters.Contains("CalendarV2") Then
                                    If row.Parameters.Contains("CalendarV2") Then
                                        bolDelete = False
                                    End If
                                ElseIf row.Parameters.Contains("June6610") Then
                                    If (isJune6610) Then
                                        bolDelete = False
                                    Else
                                        bolDelete = True
                                    End If
                                End If
                            Else
                                bolDelete = False
                            End If
                        End If
                    End If

                    If Not bLicenseLimitsCorrect AndAlso Not (row.IDPath = "Portal" OrElse row.IDPath = "Portal\Users" OrElse
                        row.IDPath = "Portal\Users\Employees" OrElse row.IDPath = "Portal\Reports" OrElse
                        row.IDPath = "Portal\Reports\DataLink" OrElse row.IDPath = "Portal\Security" OrElse
                        row.IDPath = "Portal\Security\Diagnostics" OrElse row.IDPath = "Portal\Security\Aministration" OrElse
                        row.IDPath = "Portal\Security\License") Then

                        bolDelete = True
                    End If

                    'No debe ver Canales si es Consultor
                    If isRoboticsUser AndAlso (row.IDPath = "Portal\Communications\Channels" OrElse row.IDPath = "Portal\Security\LogBook") AndAlso Not bolDelete Then
                        bolDelete = True
                    End If

                    If bolDelete Then
                        For Each childRow As DataRow In GetChilds(row)
                            childRow.Delete()
                        Next
                        row.Delete()
                    End If
                End If
            Next
        Catch ex As Exception
            'do nothing
        End Try

        _dataSet.AcceptChanges()
    End Sub

    ''' <summary>
    ''' Checks wether prerequisites are respected for specified row.
    ''' </summary>
    ''' <param name="row">The row to validate.</param>
    ''' <remarks>This function is called recursively for each child entries.</remarks>
    Private Function ValidateRow(ByVal row As GuiDataSet.GuiRow) As Boolean
        Dim ChildRows As DataRow() = GetChilds(row)
        Dim IsValid As Boolean = False

        IsValid = True

        If Not row.IsRequiredFunctionalitiesNull() AndAlso row.RequiredFunctionalities.Trim.Length > 0 AndAlso
            Not FeaturesCheck.IsAuthorized(row.RequiredFunctionalities, _featureType, _idPassport) Then

            For Each ChildRow As DataRow In ChildRows
                ChildRow.Delete()
            Next
            row.Delete()
            IsValid = False
        End If

        Return IsValid
    End Function

#End Region

#Region " Properties "

    ''' <summary>
    ''' Returns a DataSet containing the application menus layout
    ''' with valid entries only.
    ''' </summary>
    Public Property DataSet() As GuiDataSet
        Get
            Return _dataSet
        End Get
        Set(ByVal value As GuiDataSet)
            _dataSet = value
        End Set
    End Property

    ''' <summary>
    ''' Returns a DataTable containing the application menus layout
    ''' with valid entries only.
    ''' </summary>
    Public ReadOnly Property Table() As GuiDataSet.GuiDataTable
        Get
            Return _dataSet.Gui
        End Get
    End Property

    ''' <summary>
    ''' Returns all childs of selected GUI element.
    ''' </summary>
    ''' <param name="row">The element to get childs of.</param>
    ''' <remarks>Indirect childs are returned too.</remarks>
    Public Function GetChilds(ByVal row As GuiDataSet.GuiRow) As DataRow()
        ' First LIKE is to select childs, second one is to exclude recursive childs.
        Dim QueryString As String = String.Format(
            "IDPath LIKE '{0}\*'",
            row.IDPath.Replace("*", "[*]"))
        Dim Rows As DataRow() = _dataSet.Gui.Select(QueryString, "Priority")
        Return Rows
    End Function

#End Region

End Class