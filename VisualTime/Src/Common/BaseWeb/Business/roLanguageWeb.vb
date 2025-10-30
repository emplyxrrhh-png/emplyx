Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.Web.Base.HelperWeb

Public Class roLanguageWeb
    Inherits roLanguage

    Public Sub New()
        Me.InitVariables()
    End Sub

    Protected Overrides Sub InitVariables()
        ' Inicializo el array de clases de controles que no debo inspeccionar
        oNotTranslatedControlsTypes.Clear()
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxTextBox")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxMemo")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxDateEdit")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxGridView")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxPivotView")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxHiddenField")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxHtmlEditor")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxCallback")
        oNotTranslatedControlsTypes.Add("DevExpress.Web.ASPxColorEdit")


        oNotTranslatedControlsTypes.Add("System.Web.UI.HtmlControls.HtmlInputHidden")
        oNotTranslatedControlsTypes.Add("System.Web.UI.ScriptManager")

        ' Inicializo el array de controles que no tienen tooltip
        oControlsWithoutTooltip.Clear()
        oControlsWithoutTooltip.Add("DevExpress.Web.ASPxTextBox")
        oControlsWithoutTooltip.Add("DevExpress.Web.ASPxMemo")
        oControlsWithoutTooltip.Add("DevExpress.Web.ASPxDateEdit")
        oControlsWithoutTooltip.Add("DevExpress.Web.ASPxGridView")
        oControlsWithoutTooltip.Add("DevExpress.Web.ASPxComboBox")
        oControlsWithoutTooltip.Add("System.Web.UI.LiteralControl")
        oControlsWithoutTooltip.Add("DevExpress.Web.ASPxCallbackPanel")
        oControlsWithoutTooltip.Add("System.Web.UI.HtmlControls.HtmlHead")
        oControlsWithoutTooltip.Add("DevExpress.Web.ASPxPopupControl")

        oControlsWithoutText.Clear()
        oControlsWithoutText.Add("DevExpress.Web.ASPxTextBox")
        oControlsWithoutText.Add("DevExpress.Web.ASPxSpinEdit")
        oControlsWithoutText.Add("System.Web.UI.HtmlControls.HtmlImage")
        oControlsWithoutText.Add("System.Web.UI.HtmlControls.HtmlHead")
        oControlsWithoutText.Add("DevExpress.Web.ASPxPopupControl")

        oControlsWithoutDescription.Clear()
        oControlsWithoutDescription.Add("DevExpress.Web.ASPxTextBox")
        oControlsWithoutDescription.Add("System.Web.UI.HtmlControls.HtmlImage")
        oControlsWithoutDescription.Add("System.Web.UI.HtmlControls.HtmlHead")
        oControlsWithoutDescription.Add("DevExpress.Web.ASPxMemo")
        oControlsWithoutDescription.Add("DevExpress.Web.ASPxDateEdit")
        oControlsWithoutDescription.Add("DevExpress.Web.ASPxGridView")
        oControlsWithoutDescription.Add("System.Web.UI.LiteralControl")
        oControlsWithoutDescription.Add("DevExpress.Web.ASPxCallbackPanel")
        oControlsWithoutDescription.Add("System.Web.UI.HtmlControls.HtmlHead")
        oControlsWithoutDescription.Add("DevExpress.Web.ASPxPopupControl")

        ' Inicializo el array de tokens
        aStringSystemTokens.Clear()
        aStringSystemTokens.Add("${Employees}")
        aStringSystemTokens.Add("${Card}")

        ' Inicializo array de propiedades a traducir
        oTranslatedProperties = New Generic.List(Of String) 'ArrayList
        With oTranslatedProperties
            .Clear()
            .Add("Text")
            .Add("ToolTip")
            .Add("EmptyValueMessage")
            .Add("InvalidValueMessage")
            .Add("TooltipMessage")
            .Add("Description")
            .Add("title")
        End With

    End Sub

    Public Function TranslateFromWS(ByVal strLanguageReference As String, ByVal strDefaultScope As String, ByVal strTextKey As String, Optional ByVal oParamList As Generic.List(Of String) = Nothing) As String
        Return Me.Translate(strLanguageKey, strDefaultScope & "." & strTextKey, oParamList)

    End Function

    Public Overloads Sub Translate(ByVal oPage As PageBase, Optional ByVal ControlTypes As Generic.List(Of String) = Nothing, Optional ByVal ControlProperties As Generic.List(Of String) = Nothing)

        InitVariables()

        ' Traducir título página
        Try
            Dim strTranslation As String = FindLanguageText(oPage.DefaultScope & ".Title", "NotFound")
            If strTranslation <> "NotFound" Then
                strTranslation = StringParse(strTranslation)
                If oPage.Header IsNot Nothing Then oPage.Title = strTranslation
            End If
        Catch
            ' Capturamos la excepción por si no se puede acceder al título de la página
        End Try
        ' Traducir controles
        TranslateControls(oPage.Controls, oPage.DefaultScope, ControlTypes, ControlProperties)

    End Sub

    Public Overloads Sub Translate(ByVal oPage As PageBasePortal, Optional ByVal ControlTypes As Generic.List(Of String) = Nothing, Optional ByVal ControlProperties As Generic.List(Of String) = Nothing)

        InitVariables()

        ' Traducir título página
        Try
            Dim strTranslation As String = FindLanguageText(oPage.DefaultScope & ".Title", "NotFound")
            If strTranslation <> "NotFound" Then
                strTranslation = StringParse(strTranslation)
                If Not oPage.DefaultScope.StartsWith("srv") Then oPage.Title = strTranslation
            End If
        Catch
            ' Capturamos la excepción por si no se puede acceder al título de la página
        End Try

        ' Traducir controles
        TranslateControls(oPage.Controls, oPage.DefaultScope, ControlTypes, ControlProperties)

    End Sub

    Public Overloads Sub Translate(ByVal oUserControl As UserControlBase, Optional ByVal ControlTypes As Generic.List(Of String) = Nothing, Optional ByVal ControlProperties As Generic.List(Of String) = Nothing)
        InitVariables()

        ' Traducir controles
        TranslateControls(oUserControl.Controls, oUserControl.DefaultScope, ControlTypes, ControlProperties)

    End Sub

    Public Overloads Function Translate(ByVal TextKey As String, ByVal Scope As String, Optional ByVal oParamList As Generic.List(Of String) = Nothing, Optional ByVal bRawText As Boolean = False, Optional ByVal sDefaultText As String = "") As String

        Dim strTranslation As String = ""

        Try

            InitVariables()

            Me.ClearUserTokens()
            If oParamList IsNot Nothing Then
                For i As Integer = 0 To oParamList.Count - 1
                    Me.AddUserToken(oParamList(i))
                Next
            End If

            strTranslation = Me.Translate(TextKey, Scope, bRawText, sDefaultText)
        Catch ex As Exception

        End Try

        Return strTranslation

    End Function

    Public Function TranslateJavaScript(ByVal strKey As String, ByVal Scope As String, Optional ByVal oParamList As Generic.List(Of String) = Nothing) As String
        Return Me.Translate(strKey, Scope, oParamList).Replace("'", "\'")
    End Function

    Public Function GetRawText(ByVal TextKey As String, ByVal Scope As String, Optional ByVal oParamList As Generic.List(Of String) = Nothing) As String
        Dim strRes As String = ""

        Try
            InitVariables()

            Me.ClearUserTokens()
            If oParamList IsNot Nothing Then
                For i As Integer = 0 To oParamList.Count - 1
                    Me.AddUserToken(oParamList(i))
                Next
            End If

            strRes = Me.Translate(TextKey, Scope, True)
            If strRes <> "NotFound" Then
                Return strRes
            Else
                Return String.Empty
            End If
        Catch ex As Exception

        End Try

        Return strRes
    End Function

    Private Function FindLanguageText(ByVal Key As String, ByVal DefaultValue As String, Optional ByVal FromDic As Boolean = False, Optional ByVal bRawText As Boolean = False) As String
        Return Me.Translate(Key, "",, bRawText, DefaultValue)
    End Function

    Private Sub TranslateControls(ByVal Controls As ControlCollection, ByVal strFormID As String, Optional ByVal ControlTypes As Generic.List(Of String) = Nothing, Optional ByVal ControlProperties As Generic.List(Of String) = Nothing)
        For Each oControl As Control In Controls
            If Not oNotTranslatedControlsTypes.Contains(oControl.GetType.ToString) AndAlso Not TypeOf oControl Is UserControlBase AndAlso Not oControl.GetType.ToString.StartsWith("DevExpress.Web.Internal.") Then
                If ControlTypes Is Nothing OrElse ControlTypes.Contains(oControl.GetType.ToString) Then
                    TranslateControl(oControl, strFormID, ControlProperties)
                End If

            End If
        Next

    End Sub

    Private Sub AnalizeControls(ByVal ParentControl As Object, ByVal FormName As String)
        ' Función recursiva que revisa los controles y sus hijos
        Dim oControl As Object = Nothing
        Dim oSubControl As Object

        Select Case ParentControl.GetType.ToString
            Case "DevComponents.DotNetBar.BubbleBarTab"
                For Each oControl In ParentControl.buttons

                    If Not oNotTranslatedControlsTypes.Contains(oControl.GetType.ToString) AndAlso Not oControl.GetType.ToString.StartsWith("DevExpress.Web.Internal.") Then
                        TranslateControl(oControl, FormName)
                    End If

                Next
            Case "DevComponents.DotNetBar.TabItem" ' Este control requiere un trato especial
                TranslateControl(ParentControl, FormName)
            Case "DevComponents.DotNetBar.RibbonControl"
                For Each oControl In ParentControl.Controls ' Analizo la primera lista
                    AnalizeControls(oControl, FormName)
                Next

                For Each oControl In oControl.items ' Analizo la segunda lista
                    TranslateControl(oControl, FormName)
                Next
            Case Else
                For Each oControl In ParentControl.Controls

                    If Not oNotTranslatedControlsTypes.Contains(oControl.GetType.ToString) AndAlso Not oControl.GetType.ToString.StartsWith("DevExpress.Web.Internal.") Then
                        TranslateControl(oControl, FormName)
                        Select Case oControl.GetType.ToString
                            Case "DevComponents.DotNetBar.BubbleBar" ' Este control requiere un trato especial
                                For Each oSubControl In oControl.tabs
                                    AnalizeControls(oSubControl, FormName)
                                Next
                            Case "DevComponents.DotNetBar.TabControl" ' Este control requiere un trato especial
                                For Each oSubControl In oControl.tabs
                                    AnalizeControls(oSubControl, FormName)
                                Next

                                For Each oSubControl In oControl.controls
                                    AnalizeControls(oSubControl, FormName)
                                Next
                            Case Else
                                AnalizeControls(oControl, FormName)
                        End Select
                    End If

                Next

        End Select
    End Sub

    Public Sub TranslateControl(ByVal oControl As Object, ByVal FormName As String, Optional ByVal ControlProperties As Generic.List(Of String) = Nothing)

        Dim strTranslation As String

        If oControl.ID <> "" Then

            'Dim oProperties As ArrayList = Me.oTranslatedProperties
            Dim oProperties As Generic.List(Of String) = Me.oTranslatedProperties
            If ControlProperties IsNot Nothing Then oProperties = ControlProperties

            Dim oContainer As Object = Nothing

            For Each strProperty As String In oProperties

                Dim bTranslateProperty As Boolean = True
                If oControlsWithoutTooltip.Contains(oControl.GetType.ToString) AndAlso strProperty.ToLower() = "tooltip" Then
                    bTranslateProperty = False
                End If

                If oControlsWithoutText.Contains(oControl.GetType.ToString) AndAlso strProperty.ToLower() = "text" Then
                    bTranslateProperty = False
                End If

                If oControlsWithoutDescription.Contains(oControl.GetType.ToString) AndAlso strProperty.ToLower() = "description" Then
                    bTranslateProperty = False
                End If

                If bTranslateProperty Then
                    Dim oProperty As System.Reflection.PropertyInfo = GetProperty(oControl, {strProperty}, oContainer)
                    If oProperty IsNot Nothing Then
                        Dim defaultValue As String

                        Try
                            defaultValue = roTypes.Any2String(oProperty.GetValue(oContainer))
                        Catch ex As Exception
                            defaultValue = "NotFound"
                        End Try



                        strTranslation = FindLanguageText(FormName & "." & oControl.ID & "." & strProperty, defaultValue)
                        If strTranslation <> "NotFound" Then
                            strTranslation = StringParse(strTranslation)
                            Try
                                If oProperty.GetSetMethod IsNot Nothing Then oProperty.SetValue(oContainer, strTranslation, Nothing)
                            Catch ex As Exception

                            End Try
                        End If

                    End If
                End If



            Next

            If TypeOf oControl Is GridView Then

                Dim oProperty As System.Reflection.PropertyInfo = GetProperty(oControl, {"Columns", "Count"}, oContainer)
                If oProperty IsNot Nothing Then

                    Dim intColumns As Integer = 0
                    If oProperty.GetValue(oContainer, Nothing) IsNot Nothing Then _
                        intColumns = oProperty.GetValue(oContainer, Nothing)
                    For n As Integer = 0 To intColumns - 1
                        oProperty = GetProperty(oControl, {"Columns#" & n, "HeaderText"}, oContainer)
                        If oProperty IsNot Nothing Then

                            strTranslation = FindLanguageText(FormName & "." & oControl.ID & ".Columns#" & n & ".HeaderText", "NotFound")
                            If strTranslation <> "NotFound" Then
                                strTranslation = StringParse(strTranslation)
                                If oProperty.GetSetMethod IsNot Nothing Then oProperty.SetValue(oContainer, strTranslation, Nothing)
                            End If

                        End If
                    Next

                End If

            ElseIf TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputButton Then

                If CType(oControl, System.Web.UI.HtmlControls.HtmlInputButton).Visible Then

                    Dim strProperty As String = "Value"

                    Dim oProperty As System.Reflection.PropertyInfo =
                        GetProperty(oControl, {strProperty}, oContainer)
                    If oProperty IsNot Nothing Then

                        strTranslation = FindLanguageText(FormName & "." & oControl.ID & "." & strProperty, "NotFound")
                        If strTranslation <> "NotFound" Then
                            strTranslation = StringParse(strTranslation)
                            If oProperty.GetSetMethod IsNot Nothing Then oProperty.SetValue(oContainer, strTranslation, Nothing)
                        End If

                    End If

                End If

            End If
        End If

        If Not TypeOf oControl Is GridView AndAlso Not TypeOf oControl Is System.Web.UI.HtmlControls.HtmlInputButton AndAlso oControl.Controls.Count > 0 Then
            TranslateControls(oControl.Controls, FormName, , ControlProperties)
        End If
    End Sub

End Class