Imports System.Data.Common
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class GuiAction
    Private oState As wscGuiState = Nothing

    Private oServerLicense As Robotics.VTBase.Extensions.roServerLicense
    Private oGuiAction As roGuiAction
    Private strFeatureType As String = "U"

#Region "Properties"

    <XmlIgnore()>
    Public Property State() As wscGuiState
        Get
            Return Me.oState
        End Get
        Set(ByVal value As wscGuiState)
            Me.oState = value
        End Set
    End Property

    <XmlIgnore()>
    Public Property ServerLicense() As Robotics.VTBase.Extensions.roServerLicense
        Get
            Return Me.oServerLicense
        End Get
        Set(ByVal value As Robotics.VTBase.Extensions.roServerLicense)
            Me.oServerLicense = value
        End Set
    End Property

    Public Property GuiAction() As roGuiAction
        Get
            Return Me.oGuiAction
        End Get
        Set(ByVal value As roGuiAction)
            Me.oGuiAction = value
        End Set
    End Property

#End Region

#Region "Constructor"

    Public Sub New()
        Me.oState = New wscGuiState
        Me.oServerLicense = New Robotics.VTBase.Extensions.roServerLicense
        Me.oGuiAction = New roGuiAction()
    End Sub

    Public Sub New(ByVal _State As wscGuiState)
        Me.oState = _State
        Me.oServerLicense = New Robotics.VTBase.Extensions.roServerLicense
        Me.oGuiAction = New roGuiAction()
    End Sub

    Public Sub New(ByVal IDPath As String, ByVal IDGuiSection As String, ByVal FeatureType As String, ByVal _State As wscGuiState)
        Me.oState = _State
        Me.strFeatureType = FeatureType
        Me.oServerLicense = New Robotics.VTBase.Extensions.roServerLicense
        Me.oGuiAction = Me.Load(IDPath, IDGuiSection)
    End Sub

#End Region

#Region "Methods"

    Private Function Load(ByVal IDPath As String, ByVal IDGuiSection As String, Optional ByVal bAudit As Boolean = False) As roGuiAction

        Dim bolRet As roGuiAction = Nothing

        Dim rd As DbDataReader = Nothing

        Try

            Dim strSQL As String = "@SELECT# * FROM sysroGUI_Actions WHERE IDPath ='" & IDPath.Trim & "' AND IDGUIPath = '" & IDGuiSection.Trim & "' ORDER BY ElementIndex"

            Dim tbIDs As DataTable = CreateDataTable(strSQL, )
            If tbIDs IsNot Nothing AndAlso tbIDs.Rows.Count > 0 Then

                Dim strRequieredFeatrues As String = String.Empty
                Dim strRequieredFunctionalities As String = String.Empty

                If Not IsDBNull(tbIDs.Rows(0)("RequieredFeatures")) Then strRequieredFeatrues = tbIDs.Rows(0)("RequieredFeatures")
                If Not IsDBNull(tbIDs.Rows(0)("RequieredFunctionalities")) Then strRequieredFunctionalities = tbIDs.Rows(0)("RequieredFunctionalities")

                If ValidateRow(strRequieredFeatrues, strRequieredFunctionalities) Then
                    bolRet = New roGuiAction
                    bolRet.IDPath = IDPath
                    bolRet.IDGuiPath = IDGuiSection
                    If Not IsDBNull(tbIDs.Rows(0)("LanguageTag")) Then bolRet.LanguageTag = tbIDs.Rows(0)("LanguageTag")
                    If Not IsDBNull(tbIDs.Rows(0)("AfterFunction")) Then bolRet.AfterFunction = tbIDs.Rows(0)("AfterFunction")
                    If Not IsDBNull(tbIDs.Rows(0)("CssClass")) Then bolRet.CssClass = tbIDs.Rows(0)("CssClass")
                    bolRet.Order = VTBase.roTypes.Any2Integer(tbIDs.Rows(0)("ElementIndex"))
                    bolRet.Section = VTBase.roTypes.Any2Integer(tbIDs.Rows(0)("Section"))
                    bolRet.AppearsOnPopup = VTBase.roTypes.Any2Boolean(tbIDs.Rows(0)("AppearsOnPopup"))
                End If
            End If
        Catch ex As DbException
            Me.oState.UpdateStateInfo(ex, "GuiAction:Load")
            Me.oGuiAction = Nothing
        Catch ex As Exception
            Me.oState.UpdateStateInfo(ex, "GuiAction:Load")
            Me.oGuiAction = Nothing
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return bolRet

    End Function

    Private Function ValidateRow(ByVal requieredFeature As String, ByVal requieredFunctuionality As String) As Boolean
        Dim IsValid As Boolean = False

        IsValid = True

        If requieredFeature.Trim <> String.Empty AndAlso Not Me.oServerLicense.FeatureIsInstalled(requieredFeature) Then
            IsValid = False
        End If

        If IsValid AndAlso requieredFunctuionality <> String.Empty AndAlso Not FeaturesCheck.IsAuthorized(requieredFunctuionality, Me.strFeatureType, Me.oState.IDPassport) Then
            IsValid = False
        End If

        Return IsValid
    End Function

#End Region

#Region "HelperMethods"

    Public Shared Function GetActionsBySection(ByVal idGuiPath As String, ByRef _State As wscGuiState) As Generic.List(Of GuiAction)
        Dim bolRet As New Generic.List(Of GuiAction)

        Dim rd As DbDataReader = Nothing

        Try

            Dim oLicSupport As New Extensions.roLicenseSupport()
            Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()
            Dim sVTEdition As String = String.Empty
            If oLicInfo.Edition <> roServerLicense.roVisualTimeEdition.NotSet Then
                sVTEdition = oLicInfo.Edition.ToString
            End If

            Dim strSQL As String = "@SELECT# IDPath FROM sysroGUI_Actions WHERE IDGUIPath = '" & idGuiPath.Trim & "' " & If(sVTEdition.Length > 0, " AND CHARINDEX('Starter',Edition) > 0 ", "") & " ORDER BY Section ASC, ElementIndex ASC"

            Dim tbIDs As DataTable = CreateDataTable(strSQL, )
            If tbIDs IsNot Nothing AndAlso tbIDs.Rows.Count > 0 Then
                For Each oRow As DataRow In tbIDs.Rows
                    Dim tmpObj As New GuiAction(oRow("IDPath"), idGuiPath, "U", _State)
                    If tmpObj IsNot Nothing Then bolRet.Add(tmpObj)
                Next
            End If
        Catch ex As DbException
            _State.UpdateStateInfo(ex, "GuiAction:Load")
        Catch ex As Exception
            _State.UpdateStateInfo(ex, "GuiAction:Load")
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

        Return bolRet
    End Function

#End Region

End Class