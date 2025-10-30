Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Base.VTDataLink.DataLink
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Security
Imports Robotics.VTBase

Public Class roTools

    <Runtime.Serialization.DataContract()>
    Public Class PageCustomActions
        <Runtime.Serialization.DataMember(Name:="BarButtons")>
        Public BarButtons As String

        <Runtime.Serialization.DataMember(Name:="HasReports")>
        Public HasReports As Boolean

        <Runtime.Serialization.DataMember(Name:="ReportsAction")>
        Public ReportsAction As String

        <Runtime.Serialization.DataMember(Name:="HasAssistants")>
        Public HasAssistants As Boolean

        <Runtime.Serialization.DataMember(Name:="Assistants")>
        Public Assistants() As PageAssistant
    End Class

    <Runtime.Serialization.DataContract()>
    Public Class PageAssistant
        <Runtime.Serialization.DataMember(Name:="Action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="Icon")>
        Public Icon As String

        <Runtime.Serialization.DataMember(Name:="Text")>
        Public Text As String

        <Runtime.Serialization.DataMember(Name:="Description")>
        Public Description As String
    End Class

#Region "Funciones para Barras de Botones"

    Public Enum ToolBarDirection
        Horizontal
        Vertical
    End Enum

    Public Shared Function CreateButtonHorizontal(ByVal Tip As String, ByVal ToolTipText As String, ByVal strClass As String, ByVal strOnClickFuncion As String) As String
        Dim strButton As String = String.Empty
        Try

            Dim marginleft As Integer = -30

            marginleft = marginleft - (ToolTipText.Length * 2)

            strButton = "<div style=""float:right;padding: 10px;""><div id=""" & Tip & """ style=""position: absolute; margin-left: " & marginleft.ToString & "px; margin-top: 35px; display: none; z-index: 990; "">" &
                roTools.CreateToolTipHorizontal(ToolTipText) &
                "</div><a href=""javascript: void(0);"" style=""padding-bottom:5"" class=""ButtonBarVertical " & strClass & """ onmouseover=""showTbTip('" & Tip & "');"" onmouseout=""hideTbTip('" & Tip & "');"" " &
                "onclick=""" & strOnClickFuncion & """></a></div>"
        Catch ex As Exception
            strButton = String.Empty
        End Try
        Return strButton
    End Function

    Public Shared Function CreateButtonVertical(ByVal Tip As String, ByVal ToolTipText As String, ByVal strClass As String, ByVal strOnClickFuncion As String) As String

        Dim strButton As String = String.Empty

        Try

            strButton = "<div id=""" & Tip & """ style=""position: absolute; margin-left: 40px; margin-top: 1px; display: none; z-index: 990; width:auto;"">" &
                        roTools.CreateToolTipVertical(ToolTipText) &
                        "</div><a href=""javascript: void(0);"" class=""ButtonBarVertical " & strClass & """ onmouseover="" showTbTip('" & Tip & "');"" onmouseout=""hideTbTip('" & Tip & "');"" " &
                        "onclick=""" & strOnClickFuncion & """></a>"
        Catch ex As Exception
            strButton = String.Empty
        End Try

        Return strButton

    End Function

    Public Shared Function CreateButtonVerticalOLD(ByVal Tip As String, ByVal ToolTipText As String, ByVal strClass As String, ByVal strOnClickFuncion As String) As String

        Dim strButton As String = String.Empty

        Try

            strButton = "<div id=""" & Tip & """ style=""position: absolute; margin-left: 40px; margin-top: -5px; display: none; z-index: 990; width:auto;"">" &
                        roTools.CreateToolTipVertical(ToolTipText) &
                        "</div><a href=""javascript: void(0);"" class=""ButtonBarVertical " & strClass & """ onmouseover=""showTbTip('" & Tip & "');"" onmouseout=""hideTbTip('" & Tip & "');"" " &
                        "onclick=""" & strOnClickFuncion & """></a>"
        Catch ex As Exception
            strButton = String.Empty
        End Try

        Return strButton

    End Function

    Public Shared Function CreateToolTipHorizontal(ByVal ToolTipText As String) As String
        Dim strTip As String = String.Empty
        Dim strEstilo = " style=""border:solid 1px #A8A8A8; border-radius: 5px; -ms-border-radius: 5px; -moz-border-radius: 5px; -webkit-border-radius: 5px; -khtml-border-radius: 5px; padding:8px; white-space: nowrap; background-color: #FFFFCC; 	color: #2D4155;"""
        Try
            strTip = "<div" & strEstilo & ">"
            strTip &= ToolTipText
            strTip &= "</div>"
        Catch ex As Exception
            strTip = String.Empty
        End Try
        Return strTip
    End Function

    Public Shared Function CreateToolTipVertical(ByVal ToolTipText As String) As String

        Dim strTip As String = String.Empty

        Try
            strTip = "<div class=""VerticalToolTip"">"
            strTip &= ToolTipText
            strTip &= "</div>"
        Catch ex As Exception
            strTip = String.Empty
        End Try

        Return strTip

    End Function

    Public Shared Function BuildPageCustomActions(ByVal guiActions As Generic.List(Of roGuiAction), ByVal sID As String, ByVal oLanguage As roLanguageWeb, ByVal strDefaultScope As String, Optional reportsSection As String = "", Optional applyFilter As Boolean = False, Optional ByVal eToolbarDirection As ToolBarDirection = ToolBarDirection.Vertical) As PageCustomActions
        Dim oResponse As New roTools.PageCustomActions

        Try
            Dim destDiv As HtmlGenericControl = roTools.BuildCentralBar(guiActions, sID, oLanguage, strDefaultScope, reportsSection)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            destDiv.RenderControl(htw)

            oResponse.BarButtons = sw.ToString

            Dim reportsConfig As roGuiAction = guiActions.Find(Function(x) x.IDPath = "Reports")
            If reportsConfig IsNot Nothing Then
                oResponse.HasReports = True
                oResponse.ReportsAction = "ShowReports('" & oLanguage.TranslateJavaScript("Reports.Title", strDefaultScope) & "', '', '" & reportsSection & "'," & roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VTLive.DefaultReportsVersions")) & ",'" & Configuration.RootUrl & "');"
            Else
                oResponse.HasReports = False
            End If

            Dim popupsConfig As Generic.List(Of roGuiAction) = guiActions.FindAll(Function(x) x.AppearsOnPopup AndAlso x.IDPath <> "Reports")

            If popupsConfig IsNot Nothing AndAlso popupsConfig.Count > 0 Then
                Dim tmpList As New Generic.List(Of roTools.PageAssistant)
                For Each oAssistant As roGuiAction In popupsConfig
                    Dim jsReturn As String = String.Empty
                    Dim bInsert As Boolean = True

                    roTools.GenerateJSAction(sID, oLanguage, strDefaultScope, reportsSection, oAssistant, jsReturn, bInsert)
                    If bInsert Then
                        Dim oNewObj As New roTools.PageAssistant
                        oNewObj.Action = jsReturn
                        oNewObj.Icon = oAssistant.CssClass
                        oNewObj.Text = oLanguage.Translate(oAssistant.LanguageTag, strDefaultScope)
                        oNewObj.Description = oLanguage.Translate(oAssistant.LanguageTag & ".Desc", strDefaultScope)

                        tmpList.Add(oNewObj)
                    End If
                Next
                oResponse.HasAssistants = True
                oResponse.Assistants = tmpList.ToArray
            Else
                oResponse.HasAssistants = False
            End If
        Catch ex As Exception
            oResponse = New PageCustomActions() With {
                .BarButtons = "",
                .HasAssistants = False,
                .Assistants = {},
                .HasReports = False,
                .ReportsAction = ""
            }
        End Try

        Return oResponse
    End Function

    Public Shared Function BuildCentralBar(ByVal guiActions As Generic.List(Of roGuiAction), ByVal sID As String, ByVal oLanguage As roLanguageWeb, ByVal strDefaultScope As String, Optional reportsSection As String = "", Optional applyFilter As Boolean = False, Optional ByVal eToolbarDirection As ToolBarDirection = ToolBarDirection.Vertical) As HtmlGenericControl
        Dim strTopContent As String = String.Empty
        Dim strBottomContent As String = String.Empty

        Dim index As Integer = 1
        Dim tmpGuiActions As Generic.List(Of roGuiAction) = guiActions.FindAll(Function(x) x.AppearsOnPopup = False).ToList

        For Each oGuiAction As roGuiAction In tmpGuiActions
            Dim jsReturn As String = String.Empty
            Dim bolInsert As Boolean = True

            GenerateJSAction(sID, oLanguage, strDefaultScope, reportsSection, oGuiAction, jsReturn, bolInsert)

            If bolInsert Then
                Dim bLang As String = ""
                If oGuiAction.IDPath = "MaxMinimize" Then
                    bLang = oLanguage.Keyword("tbMaximize")
                Else
                    bLang = oLanguage.Translate(oGuiAction.LanguageTag, strDefaultScope)
                End If

                If eToolbarDirection = ToolBarDirection.Vertical Then
                    If oGuiAction.Section = 0 Then
                        If (applyFilter AndAlso oGuiAction.IDPath.ToUpper().Equals("FILTER")) Then
                            strTopContent &= roTools.CreateButtonVertical("tip" & index, bLang, oGuiAction.CssClass & "_Selected", jsReturn)
                        Else
                            strTopContent &= roTools.CreateButtonVertical("tip" & index, bLang, oGuiAction.CssClass, jsReturn)
                        End If
                    Else
                        strBottomContent &= roTools.CreateButtonVertical("tip" & index, bLang, oGuiAction.CssClass, jsReturn)
                    End If
                Else
                    If oGuiAction.Section = 0 Then
                        If (applyFilter AndAlso oGuiAction.IDPath.ToUpper().Equals("FILTER")) Then
                            strTopContent = roTools.CreateButtonHorizontal("tip" & index, bLang, oGuiAction.CssClass & "_Selected", jsReturn) & strTopContent
                        Else
                            strTopContent = roTools.CreateButtonHorizontal("tip" & index, bLang, oGuiAction.CssClass, jsReturn) & strTopContent
                        End If
                    Else
                        strBottomContent = roTools.CreateButtonHorizontal("tip" & index, bLang, oGuiAction.CssClass, jsReturn) & strBottomContent
                    End If
                End If

                index += index
            End If
        Next

        Dim destDiv As New HtmlGenericControl("div")

        If eToolbarDirection = ToolBarDirection.Vertical Then
            destDiv.Attributes("class") = "middleBarButtonsMain"

            Dim topDiv As New HtmlGenericControl("div")
            topDiv.Style("height") = "auto"
            topDiv.Style("vertical-align") = "top"
            topDiv.InnerHtml = strTopContent

            destDiv.Controls.Add(topDiv)
        Else
            Dim topDiv As New HtmlGenericControl("div")
            topDiv.Style("height") = "50%"
            topDiv.Style("vertical-align") = "top"
            topDiv.Style("float") = "left"
            topDiv.InnerHtml = strTopContent

            Dim bottomDiv As New HtmlGenericControl("div")
            bottomDiv.Style("height") = "50%"
            bottomDiv.Style("vertical-align") = "top"
            bottomDiv.Style("float") = "right"
            bottomDiv.Style("margin-right") = "30px"
            bottomDiv.InnerHtml = strBottomContent

            destDiv.Controls.Add(topDiv)
            'destDiv.Controls.Add(bottomDiv)
        End If

        Return destDiv
    End Function

    Public Shared Sub GenerateJSAction(sID As String, oLanguage As roLanguageWeb, strDefaultScope As String, reportsSection As String, oGuiAction As roGuiAction, ByRef jsReturn As String, ByRef bolInsert As Boolean)
        Select Case oGuiAction.AfterFunction
            Case "ShowReports"
                jsReturn = "ShowReports('" & oLanguage.TranslateJavaScript("Reports.Title", strDefaultScope) & "', '', '" & reportsSection & "'," & roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VTLive.DefaultReportsVersions")) & ",'" & Configuration.RootUrl & "');"
            Case "MaxMinimize"
                jsReturn = "MaximizeMinimizeContent('tip1','divTree','divContenido','" + oLanguage.Keyword("tbMaximize") + "','" + oLanguage.Keyword("tbMinimize") + "','resizeHeaders()');"
            Case "showAccessStatusMonitor", "showAccessStatusMonitorV2"
                If oGuiAction.AfterFunction = "showAccessStatusMonitor" Then
                    If roTypes.Any2Integer(HelperSession.AdvancedParametersCache("ShowAccessMonitor")) = 1 Then
                        jsReturn = oGuiAction.AfterFunction & "('" & Configuration.RootUrl & "');"
                    Else
                        jsReturn = ""
                        bolInsert = False
                    End If
                Else
                    jsReturn = oGuiAction.AfterFunction & "('" & Configuration.RootUrl & "','" & roTypes.Any2String(HelperSession.AdvancedParametersCache("AccessMonitor.User")) & "','" & roTypes.Any2String(HelperSession.AdvancedParametersCache("AccessMonitor.Pass")) & "');"
                End If

            Case "ConfirmLaunchBroadcaster"
                If roTypes.Any2Integer(HelperSession.AdvancedParametersCache("BroadcasterManually")) = 1 Then
                    jsReturn = "ConfirmLaunchBroadcaster();"
                Else
                    jsReturn = ""
                    bolInsert = False
                End If
            Case "copyAccessGroup('#ID#')"
                bolInsert = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("AdvancedAccessMode")) >= 1
                jsReturn = oGuiAction.AfterFunction.Replace("#ID#", sID) & ";"

            Case "CopyCurrentExport('#ID#')"
                Dim roExportGuide As New roExportGuide
                roExportGuide = API.DataLinkServiceMethods.GetExportGuide(Nothing, sID, True)
                If String.IsNullOrEmpty(roExportGuide.ProfileMask) Then
                    bolInsert = False
                Else
                    bolInsert = roExportGuide.ProfileMask.Substring(0, 3) = "adv"
                    jsReturn = oGuiAction.AfterFunction.Replace("#ID#", sID) & ";"
                End If
            Case "copyShift()"
                Dim oshift As New roShift()
                oshift = API.ShiftServiceMethods.GetShift(Nothing, sID, True)
                If oshift IsNot Nothing AndAlso oshift.AdvancedParameters IsNot Nothing AndAlso oshift.AdvancedParameters.Contains("Starter=[1]") Then
                    bolInsert = False
                Else
                    jsReturn = oGuiAction.AfterFunction & ";"
                End If
            Case Else
                ' Si el ID es -1 i la funcion depende del ID no se inserta ya que no se puede aplicar.
                If (oGuiAction.AfterFunction.Contains("#ID#") Or
                    oGuiAction.AfterFunction.Contains("#IDCAMERA#")) AndAlso roTypes.Any2Integer(sID) = -1 Then bolInsert = False

                If oGuiAction.IDPath = "MassMarkConsents" AndAlso Not roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VTLive.Consents.Enabled")) Then
                    bolInsert = False
                End If

                If bolInsert AndAlso oGuiAction.IDPath = "MassPunch" AndAlso roTypes.Any2Integer(HelperSession.AdvancedParametersCache("MassPunchWizardAvailable")) <> 1 Then
                    bolInsert = False
                End If

                If bolInsert Then
                    If oGuiAction.AfterFunction.Contains("#IDCAMERA#") AndAlso roTypes.Any2Integer(sID) > 0 Then
                        Dim oCurrentStatus As roZone = API.ZoneServiceMethods.GetZoneByID(Nothing, sID, True)
                        If oCurrentStatus.IDCamera.HasValue Then
                            jsReturn = oGuiAction.AfterFunction.Replace("#IDCAMERA#", oCurrentStatus.IDCamera.Value) & ";"
                        Else
                            bolInsert = False
                        End If
                    ElseIf oGuiAction.AfterFunction.Contains("#ID#") Then
                        jsReturn = oGuiAction.AfterFunction.Replace("#ID#", sID) & ";"
                    Else
                        jsReturn = oGuiAction.AfterFunction
                    End If

                    If WLHelperWeb.CurrentUserIsConsultantOrCegid Then
                        jsReturn = jsReturn.Replace("ShowRemoveEmployeeData", "ShowCaptchaRemoveEmployee")
                    End If

                End If

        End Select
    End Sub

#End Region

    'PPR desactivado temporalmente NO ELIMINAR-->

#Region "Funciones de QueryString"

    Public Shared Function GetEmployeeGroupPath(ByVal intIDEmployee As Integer) As String

        Dim strRet As String = ""

        Try

            ' Obtener el grupo actual del empleado
            Dim oMobility As roMobility = API.EmployeeServiceMethods.GetCurrentMobility(Nothing, intIDEmployee)
            If roWsUserManagement.SessionObject.States.EmployeeState.Result = EmployeeResultEnum.NoError Then

                If oMobility IsNot Nothing Then

                    ' Obtener el path del grupo del empleado
                    Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Nothing, oMobility.IdGroup, False)
                    If roWsUserManagement.SessionObject.States.EmployeeGroupState.Result = GroupResultEnum.NoError Then
                        If oGroup.Path.Split("\").Length >= 1 Then
                            For n As Integer = 0 To oGroup.Path.Split("\").Length - 1


                                Dim iTmpIdGroup = roTypes.Any2Integer(oGroup.Path.Split("\")(n))

                                If (WLHelper.GetPermissionOverGroup(WLHelperWeb.CurrentPassportID, iTmpIdGroup, 1) >= Permission.Read) Then
                                    strRet &= "A" & oGroup.Path.Split("\")(n) & "/"
                                End If

                            Next
                            If strRet <> "" Then strRet = "/" & strRet.Substring(0, strRet.Length - 1)
                            strRet = "/source" & strRet & "/B" & intIDEmployee
                        End If
                    End If

                End If

            End If
        Catch ex As Exception
        End Try

        Return strRet

    End Function

    Public Shared Function GetGroupPath(ByVal intIDGroup As Integer) As String

        Dim strRet As String = ""

        Try

            ' Obtener el path del grupo
            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Nothing, intIDGroup, False)
            If roWsUserManagement.SessionObject.States.EmployeeGroupState.Result = GroupResultEnum.NoError Then
                If oGroup.Path.Split("\").Length > 1 Then
                    For n As Integer = 0 To oGroup.Path.Split("\").Length - 2
                        strRet &= "A" & oGroup.Path.Split("\")(n) & "/"
                    Next
                    If strRet <> "" Then strRet = "/" & strRet.Substring(0, strRet.Length - 1)
                    strRet = "/source" & strRet & "/A" & intIDGroup
                End If
            End If
        Catch ex As Exception
        End Try

        Return strRet

    End Function

    Public Shared Function GetPassportPath(ByVal intIDPassport As Integer) As String

        Dim strRet As String = ""

        Try

            Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Nothing, intIDPassport, LoadType.Passport)

            If oPassport IsNot Nothing Then
                strRet = "/" & oPassport.ID

                While oPassport.IDParentPassport.HasValue
                    oPassport = API.UserAdminServiceMethods.GetPassport(Nothing, oPassport.IDParentPassport, LoadType.Passport)

                    If oPassport IsNot Nothing Then
                        strRet = "/" & oPassport.ID & strRet
                    End If
                End While
            End If

            strRet = "/source" & strRet
        Catch ex As Exception
        End Try

        Return strRet

    End Function

#End Region

End Class