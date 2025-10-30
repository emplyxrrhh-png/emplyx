Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Base.VTSelectorManager

Partial Class Wizards_AccGroupEmployeeWizard
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome
        frmEmployeesSelector
        frmFinish
    End Enum

    Private oActiveFrame As Frame

    <Runtime.Serialization.DataContract()>
    Private Class EmployeeStructField

        <Runtime.Serialization.DataMember()>
        Public attname As String

        <Runtime.Serialization.DataMember()>
        Public value As String

    End Class

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get

            Dim oFrames As New Generic.List(Of Frame)
            If Me.hdnFrames.Value = "" Then

                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmEmployeesSelector)
                oFrames.Add(Frame.frmFinish)
            Else

                For Each strItem As String In Me.hdnFrames.Value.Split("*")
                    oFrames.Add(strItem)
                Next

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            Me.hdnFrames.Value = ""
            Me.hdnFramesOnlyClient.Value = ""
            If value IsNot Nothing Then
                For Each oItem As Frame In value
                    Me.hdnFrames.Value &= "*" & oItem
                    Select Case oItem
                        Case Frame.frmWelcome
                            Me.hdnFramesOnlyClient.Value &= "*1"
                        Case Else
                            Me.hdnFramesOnlyClient.Value &= "*0"
                    End Select
                Next
                If Me.hdnFrames.Value <> "" Then Me.hdnFrames.Value = Me.hdnFrames.Value.Substring(1)
                If Me.hdnFramesOnlyClient.Value <> "" Then Me.hdnFramesOnlyClient.Value = Me.hdnFramesOnlyClient.Value.Substring(1)
            End If
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.IsPostBack Then

            If Me.HasFeaturePermission("Access.Groups.Assign", Permission.Write) Then

                Me.Frames = Nothing
                Me.Frames = Me.Frames

                Me.SetStepTitles()

                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpAssignEmployees")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpAssignEmployeesGrid")

                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

                Me.oActiveFrame = Frame.frmWelcome
            Else

                WLHelperWeb.RedirectAccessDenied(True)

            End If
        Else

            If Me.Request("action") = "CheckFrame" Then
            Else

                Me.oActiveFrame = Me.FrameByIndex(Me.hdnActiveFrame.Value)
                Dim oDiv As HtmlControl
                For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
                    oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
                    If n = Me.FrameIndex(Me.oActiveFrame) Then
                        oDiv.Style("display") = "block"
                    Else
                        oDiv.Style("display") = "none"
                    End If
                Next

            End If

        End If

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckFrame(Me.oActiveFrame) Then

            Dim oOldFrame As Frame = Me.oActiveFrame
            If Me.Frames.Count > Me.FramePos(Me.oActiveFrame) + 1 Then
                Me.oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) + 1)
            End If

            Me.FrameChange(oOldFrame, Me.oActiveFrame)

        End If

    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim oOldFrame As Frame = Me.oActiveFrame
        Me.oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) - 1)

        Me.FrameChange(oOldFrame, Me.oActiveFrame)

    End Sub

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckFrame(Me.oActiveFrame) Then

            Dim bolRet As Boolean = True

            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim strErrorInfo As String = ""

            Dim lstErrors As New Generic.List(Of String)

            Dim lstEmployees As Generic.List(Of Integer) = Nothing
            Dim lstGroups As Generic.List(Of Integer) = Nothing

            'obtener todos los empleados de los grupos seleccionados en el arbol v3
            roSelectorManager.ExtractIdsFromSelectionString(Me.hdnEmployeesSelected.Value, lstEmployees, lstGroups)

            Dim accessMode As Integer = Robotics.VTBase.roTypes.Any2Integer(HelperSession.AdvancedParametersCache("AdvancedAccessMode"))

            If accessMode = 0 Then
                If lstGroups IsNot Nothing Then
                    Dim strFilter As String = Me.hdnFilter.Value
                    Dim strFilterUser As String = Me.hdnFilterUser.Value
                    Dim tmp As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Me, lstGroups.ToArray, "Access", "U",
                                                                                                                                               strFilter, strFilterUser)
                    lstEmployees.AddRange(tmp)
                End If
            End If

            Dim oGroupsAuthorized As New Generic.List(Of Object)

            If accessMode = 1 Then
                For Each intID As Integer In lstGroups

                    Dim oGroupField As EmployeeStructField

                    Dim strGroupEmployee As String
                    Dim intIDGroup As Integer

                    intIDGroup = intID
                    strGroupEmployee = API.EmployeeGroupsServiceMethods.GetGroup(Me, intIDGroup, False).Name

                    If strGroupEmployee <> String.Empty Then
                        Dim oGroup As New Generic.List(Of EmployeeStructField)

                        oGroupField = New EmployeeStructField
                        oGroupField.attname = "icon"
                        oGroupField.value = ""
                        oGroup.Add(oGroupField)

                        oGroupField = New EmployeeStructField
                        oGroupField.attname = "idaccessgroup"
                        oGroupField.value = Request("IDAccessGroup")
                        oGroup.Add(oGroupField)

                        oGroupField = New EmployeeStructField
                        oGroupField.attname = "idobject"
                        oGroupField.value = "0_" & intID
                        oGroup.Add(oGroupField)

                        oGroupField = New EmployeeStructField
                        oGroupField.attname = "name"
                        oGroupField.value = strGroupEmployee
                        oGroup.Add(oGroupField)

                        oGroupField = New EmployeeStructField
                        oGroupField.attname = "type"
                        oGroupField.value = "g"
                        oGroup.Add(oGroupField)

                        oGroupsAuthorized.Add(oGroup)
                    End If

                Next
            End If

            Dim oEmployeeAuthorized As New Generic.List(Of Object)

            For Each intID As Integer In lstEmployees

                Dim oEmpField As EmployeeStructField

                Dim strNameEmployee As String
                Dim intIDEmployee As Integer

                intIDEmployee = intID
                strNameEmployee = API.EmployeeServiceMethods.GetEmployeeName(Me, intIDEmployee)

                If strNameEmployee <> String.Empty Then
                    Dim oEmp As New Generic.List(Of EmployeeStructField)

                    oEmpField = New EmployeeStructField
                    oEmpField.attname = "icon"
                    oEmpField.value = ""
                    oEmp.Add(oEmpField)

                    oEmpField = New EmployeeStructField
                    oEmpField.attname = "idaccessgroup"
                    oEmpField.value = Request("IDAccessGroup")
                    oEmp.Add(oEmpField)

                    oEmpField = New EmployeeStructField
                    oEmpField.attname = "idobject"
                    oEmpField.value = "1_" & intID
                    oEmp.Add(oEmpField)

                    oEmpField = New EmployeeStructField
                    oEmpField.attname = "name"
                    oEmpField.value = strNameEmployee
                    oEmp.Add(oEmpField)

                    oEmpField = New EmployeeStructField
                    oEmpField.attname = "type"
                    oEmpField.value = "e"
                    oEmp.Add(oEmpField)

                    oEmployeeAuthorized.Add(oEmp)
                End If

            Next

            Dim strJSON As String = "{groups : [ "

            If oGroupsAuthorized.Count > 0 Then
                For Each oObj As Object In oGroupsAuthorized
                    strJSON &= " {fields:"
                    Dim oEmpFld As Generic.List(Of EmployeeStructField)
                    oEmpFld = CType(oObj, Generic.List(Of EmployeeStructField))
                    strJSON &= roJSONHelper.Serialize(oEmpFld) & "} ,"
                Next
                strJSON = strJSON.Substring(0, Len(strJSON) - 2) & "]"
            Else
                strJSON = strJSON & "]"
            End If

            strJSON = strJSON & ", employees : ["

            If oEmployeeAuthorized.Count > 0 Then
                For Each oObj As Object In oEmployeeAuthorized
                    strJSON &= " {fields:"
                    Dim oEmpFld As Generic.List(Of EmployeeStructField)
                    oEmpFld = CType(oObj, Generic.List(Of EmployeeStructField))
                    strJSON &= roJSONHelper.Serialize(oEmpFld) & "} ,"
                Next
                strJSON = strJSON.Substring(0, Len(strJSON) - 2) & "]"
            Else
                strJSON = strJSON & "]"
            End If

            strJSON = strJSON & "}"

            hdnParams_PageBase.Value = "[" & strJSON & "]"

            Me.lblWelcome1.Text = Me.Language.Translate("End.EmployeeCopyWelcome1.Text", Me.DefaultScope)
            If lstErrors.Count = 0 Then

                Me.MustRefresh = "1"

                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.EmployeeCopyWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.EmployeeCopyWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Visible = False
                ' Mostramos los errores en pantalla
                Me.txtErrors.Visible = True
                For Each strError As String In lstErrors
                    Me.txtErrors.InnerText &= strError & vbCrLf
                Next
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function FrameIndex(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = CInt(oFrame)
        Return intRet
    End Function

    Private Function FramePos(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = 0
        For n As Integer = 0 To Me.Frames.Count - 1
            If Me.Frames(n) = oFrame Then
                intRet = n
                Exit For
            End If
        Next
        Return intRet
    End Function

    Private Function FrameByIndex(ByVal intIndex As Integer) As Frame
        Dim oRet As Frame = intIndex
        Return oRet
    End Function

    Private Function CheckFrame(ByVal Frame As Frame) As Boolean

        Dim strMsg As String = ""
        Dim bolRet As Boolean = True

        Select Case Frame
            Case Wizards_AccGroupEmployeeWizard.Frame.frmEmployeesSelector

                'Comprobar si hay algun empleado seleccionado
                If hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            Case Frame.frmEmployeesSelector
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

        End Select

        Select Case oActiveFrame
            Case Frame.frmEmployeesSelector
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeEmpAssignEmployees&FeatureAlias=Access&PrefixCookie=objContainerTreeV3_treeEmpAssignEmployeesGrid&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifEmployeesSelector.Disabled = False

        End Select

        Me.hdnActiveFrame.Value = Me.FrameIndex(oActiveFrame)

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & Me.FrameIndex(oOldFrame))
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If
        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & Me.FrameIndex(oActiveFrame))
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If Me.FramePos(oOldFrame) = Me.Frames.Count - 1 And Me.FramePos(oActiveFrame) = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(Me.FramePos(oActiveFrame) > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub SetStepTitles()

        Dim oLabel As Label
        Dim strStep As String = ""
        For n As Integer = 1 To System.Enum.GetValues(GetType(Frame)).Length - 1
            If n > 1 Then
                strStep = Me.hdnStepTitle2.Text.Replace("{0}", Me.FramePos(Me.FrameByIndex(n)))
                strStep = strStep.Replace("{1}", Me.Frames.Count - 1)
            End If
            oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Title")
            oLabel.Text = Me.hdnStepTitle.Text & strStep
            oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Info")
            If oLabel IsNot Nothing Then oLabel.Text = Me.hdnSetpInfo.Text
        Next

    End Sub

    Private Function GetEmployees(ByVal strIDs As String) As Generic.List(Of Integer)

        Dim EmployeeIDs As New Generic.List(Of Integer)

        Dim intIDEmployee As Integer

        For Each strID As String In strIDs.Split(",")

            If strID.StartsWith("B") Then

                intIDEmployee = strID.Substring(1)
                If Not EmployeeIDs.Contains(intIDEmployee) Then
                    EmployeeIDs.Add(intIDEmployee)
                End If

            ElseIf strID.StartsWith("A") Then

                For Each intIDEmployee In Me.GetEmployeesFromGroup(strID.Substring(1))
                    If Not EmployeeIDs.Contains(intIDEmployee) Then
                        EmployeeIDs.Add(intIDEmployee)
                    End If
                Next

                Dim tbGroups As DataTable = API.EmployeeGroupsServiceMethods.GetGroups(Me, "Employees")
                Dim oGroups() As DataRow = tbGroups.Select("(Path LIKE '%\" & strID.Substring(1) & "\%' OR Path LIKE '" & strID.Substring(1) & "\%') AND " &
                                                           "[ID] <> " & strID.Substring(1))
                For Each oRow As DataRow In oGroups
                    For Each intIDEmployee In Me.GetEmployeesFromGroup(oRow("ID"))
                        If Not EmployeeIDs.Contains(intIDEmployee) Then
                            EmployeeIDs.Add(intIDEmployee)
                        End If
                    Next
                Next

            End If
        Next

        Return EmployeeIDs

    End Function

    Private Function GetEmployeesFromGroup(ByVal _IDGroup As Integer) As Generic.List(Of Integer)

        Dim oRet As New Generic.List(Of Integer)

        Dim tb As DataTable = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroupWithType(Me, _IDGroup, "Employees")
        If tb IsNot Nothing Then
            For Each oRow As DataRow In tb.Rows
                oRet.Add(oRow("IDEmployee"))
            Next
        End If

        Return oRet

    End Function

#End Region

End Class