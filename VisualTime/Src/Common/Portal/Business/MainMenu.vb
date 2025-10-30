Imports System.Data.Common
Imports System.IO
Imports Robotics.Portal.DataSets
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Enum FeatureTypes
    ManageFeature = 0
    EmployeeFeature = 1
End Enum

Public Class wscMainMenu

#Region "Declarations - Constructor"

    Private objGui As GuiBusiness
    Dim oLanguage As New roLanguage

    Private strAppName As String
    Private strLanguageKey As String

    Private oStateInfo As wscGuiState

    Public Sub New()
        Me.strAppName = ""
        Me.oStateInfo = New wscGuiState
    End Sub

    Public Sub New(ByVal _AppName As String, ByVal intIDPassport As Integer, ByVal oFeatureType As FeatureTypes, ByVal LanguageKey As String, ByVal oLicense As roVTLicense, ByVal _State As wscGuiState)

        Me.strAppName = _AppName
        Me.strLanguageKey = LanguageKey

        Me.oStateInfo = _State
        Me.oStateInfo.UpdateStateInfo()

        Try
            Dim isProductivEmployee As Boolean = False
            Dim strFeatureType As String = ""
            Select Case oFeatureType
                Case FeatureTypes.ManageFeature
                    strFeatureType = "U"
                Case FeatureTypes.EmployeeFeature
                    strFeatureType = "E"
            End Select

            If WLHelper.IsProductiVEmployee(intIDPassport) Then
                isProductivEmployee = True
            End If

            objGui = New GuiBusiness(Me.strAppName, intIDPassport, isProductivEmployee, strFeatureType, oLicense, _State)
        Catch Ex As DbException
            Me.oStateInfo.UpdateStateInfo(Ex, "wscMainMenu::New")
        Catch Ex As Exception
            Me.oStateInfo.UpdateStateInfo(Ex, "wscMainMenu::New")
        End Try

    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property StateInfo() As wscGuiState
        Get
            Return Me.oStateInfo
        End Get
    End Property

#End Region

#Region "Methods - MainMenu"

    Public Function MainMenu() As wscMenuElementList

        Dim lstElements As New wscMenuElementList

        'ClearLanguageBuffer()

        lstElements.List.Clear()

        Me.oStateInfo.UpdateStateInfo()

        Try

            ' Find the root entry (not containing '\').
            Dim Rows As DataRow() = objGui.Table.Select("NOT IDPath LIKE '*/*'")
            If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                Dim RootRow As GuiDataSet.GuiRow = DirectCast(Rows(0), GuiDataSet.GuiRow)

                ' Add menu sections.
                lstElements = AddElements(RootRow)

            End If
        Catch Ex As DbException
            Me.oStateInfo.UpdateStateInfo(Ex, "wscMainMenu::MainMenu")
        Catch Ex As Exception
            Me.oStateInfo.UpdateStateInfo(Ex, "wscMainMenu::MainMenu")
        End Try

        Return lstElements

    End Function

    Private Function AddElements(ByVal parentRow As GuiDataSet.GuiRow) As wscMenuElementList

        Dim oLanguage As New roLanguage
        Dim lstGroups As New wscMenuElementList
        Dim objElement As wscMenuElement

        oLanguage.SetLanguageReference("LiveGUI", strLanguageKey)

        ' Create menu items according to root's content.
        For Each row As GuiDataSet.GuiRow In objGui.GetChilds(parentRow)

            If row.IDPath.IndexOf("\", parentRow.IDPath.Length + 1) = -1 Then

                objElement = New wscMenuElement()
                With objElement
                    .Path = row.IDPath
                    .Priority = row.Priority
                    If Not row.IsLanguageReferenceNull Then
                        .Name = oLanguage.Translate("GUI." & row.LanguageReference, "GUI")
                    End If
                    If Not row.IsURLNull Then .URL = row.URL
                    If Not row.IsIconURLNull AndAlso row.IconURL <> "" Then
                        .ImageUrl = row.IconURL
                        '.ElementImage = GetImageBytes(row.IconURL)
                    End If
                    If Not row.IsParametersNull Then .MainForm = row.Parameters
                End With

                objElement.Childs = AddElements(row)

                ' Sólo lo añadimos si contiene subelementos o si és una opción de menú con una URL definida
                If objElement.Childs.List.Count > 0 Or objElement.URL <> "" Then
                    lstGroups.List.Add(objElement)
                End If

            End If

        Next

        Return lstGroups

    End Function

    Public Function MenuItem(ByVal strItemPath As String) As wscMenuElement
        Dim oLanguage As New roLanguage
        Dim oMenuElement As wscMenuElement = Nothing

        'oLanguage.ClearLanguageBuffer()
        oLanguage.SetLanguageReference("LiveGUI", strLanguageKey)

        Me.oStateInfo.UpdateStateInfo()

        Try

            ' Find the root entry (not containing '\').
            Dim Rows As DataRow() = objGui.Table.Select("IDPath LIKE '" & Me.strAppName & "\" & strItemPath & "'")
            If Rows IsNot Nothing AndAlso Rows.Length > 0 Then
                Dim RootRow As GuiDataSet.GuiRow = DirectCast(Rows(0), GuiDataSet.GuiRow)

                oMenuElement = New wscMenuElement()
                With oMenuElement
                    .Path = RootRow.IDPath
                    If Not RootRow.IsLanguageReferenceNull Then
                        .Name = oLanguage.Translate("GUI." & RootRow.LanguageReference, "GUI")
                    End If
                    If Not RootRow.IsURLNull Then .URL = RootRow.URL
                    If Not RootRow.IsIconURLNull AndAlso RootRow.IconURL <> "" Then
                        .ImageUrl = RootRow.IconURL
                        .ElementImage = GetImageBytes(RootRow.IconURL)
                    End If
                    If Not RootRow.IsParametersNull Then .MainForm = RootRow.Parameters
                End With
                oMenuElement.Childs = AddElements(RootRow)

            End If
        Catch Ex As DbException
            Me.oStateInfo.UpdateStateInfo(Ex, "wscMainMenu::MenuItem")
        Catch Ex As Exception
            Me.oStateInfo.UpdateStateInfo(Ex, "wscMainMenu::MenuItem")
        End Try

        Return oMenuElement

    End Function

    Private Function GetImageBytes(ByVal strFileName As String) As Byte()

        Dim bRet() As Byte = Nothing

        Dim objFileStream As FileStream = Nothing
        Try

            'Dim iBinaryLen As Long
            'iBinaryLen = 1024
            strFileName = ImagesPath() & "\" & strFileName
            objFileStream = New FileStream(strFileName, FileMode.Open, FileAccess.Read)

            ReDim bRet(objFileStream.Length - 1)
            objFileStream.Read(bRet, 0, bRet.Length)

            ''Dim bf As BinaryFormatter = New BinaryFormatter()
            ''Dim ms As MemoryStream = New MemoryStream()
            ''bf.Serialize(ms, bRet)
            ''Return ms.ToArray()
        Catch Ex As Exception
        Finally
            If objFileStream IsNot Nothing Then objFileStream.Close()
        End Try

        Return bRet

    End Function

    Private Function ImagesPath() As String

        Dim strPath As String = ""

        Try

            Dim sKey As String = "Software\Robotics\VisualTime\Paths\VTLive"
            Dim oRegKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(sKey, False)
            If oRegKey IsNot Nothing Then
                strPath = oRegKey.GetValue("MainMenuImages", "")
            End If
        Catch ex As Exception
        End Try

        Return strPath

    End Function

#End Region

End Class