Imports DevExpress.Web.ASPxHtmlEditor
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserControls_frmTerminalReaderV2
    Inherits UserControlBase
    Public IDReader As String
    Private bolCheckAccessAuthorizationOnNoAccessReaders As Boolean = False

#Region "String constants"

    Private CriteriaEquals As String
    Private CriteriaDifferent As String
    Private CriteriaStartsWith As String
    Private CriteriaContains As String
    Private CriteriaNoContains As String
    Private CriteriaMajor As String
    Private CriteriaMajorOrEquals As String
    Private CriteriaMinor As String
    Private CriteriaMinorOrEquals As String
    Private CriteriaTheValue As String
    Private CriteriaTheDate As String
    Private CriteriaTheDateActual As String
    Private CriteriaTheTime As String
    Private CriteriaTheTimeActual As String
    Private CriteriaTheValues As String
    Private CriteriaThePeriod As String
    Private Const UNESPECIFIED_ZONE As Integer = 255

#End Region

#Region "Validation fields"

    Private dTblTemplate As DataTable

#End Region

    Private Const FeatureAlias As String = "Terminals.Definition"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Me.frmCfgInteractive1.IDReader = Me.IDReader
        'Me.visibilityCriteria.Prefix = Me.visibilityCriteria.ClientID
        bolCheckAccessAuthorizationOnNoAccessReaders = False

        'If VTBase.roConstants.IsDistributedSystemEnabled() Then
        '    Me.tablePrinters.Style("display") = "none"
        'End If
    End Sub

    Public Function LoadDefaultValues(ByVal oTerminal As roTerminal, ByVal oTerminalReader As roTerminal.roTerminalReader, ByVal oPermission As Permission, ByVal bCheckAccessAuthorizationOnNoAccessReaders As Boolean) As Boolean
        Try

            If oPermission > Permission.Read Then
                aFEmployees.Attributes("onmouseover") = "DDown_Over('" & Me.ClientID & "');"
                aFEmployees.Attributes("onmouseout") = "DDown_Out('" & Me.ClientID & "');"

                divFloatMenuE.Attributes("onmouseover") = "document.getElementById('" & Me.ClientID & "_gBoxWhoPunches_divFloatMenuE').style.display='';"
                divFloatMenuE.Attributes("onmouseout") = "document.getElementById('" & Me.ClientID & "_gBoxWhoPunches_divFloatMenuE').style.display='none';"
            End If

            'cmbTerminalModes.ClientSideEvents.SelectedIndexChanged = "function(s,e){hasChanges(true," & Me.IDReader & ",'cmbTerminalModes');}"

            Me.bolCheckAccessAuthorizationOnNoAccessReaders = bCheckAccessAuthorizationOnNoAccessReaders

            cmbTerminalModes.ClientSideEvents.SelectedIndexChanged = "function(s,e){EnableReaderControls(" & Me.IDReader & ",s.GetSelectedItem().value);hasChanges(true);}"

            Me.hdnLabelDirectionIn.Value = Me.Language.Translate("TerminalMode.Direction.In", DefaultScope)
            Me.hdnLabelDirectionOut.Value = Me.Language.Translate("TerminalMode.Direction.Out", DefaultScope)
            Me.hdnLabelDirectionUndefined.Value = Me.Language.Translate("TerminalMode.Direction.Undefined", DefaultScope)

            chkUseDispKey.Attributes("onchange") = "hasChanges(true," & Me.IDReader & ",'chkUseDispKey');"

            cmbPosZoneIn.ClientInstanceName = "cmbPosZoneInClient" & Me.IDReader
            cmbPosZoneOut.ClientInstanceName = "cmbPosZoneOutClient" & Me.IDReader
            cmbCostCenters.ClientInstanceName = "cmbCostCentersClient" & Me.IDReader

            'Posem valors per omisio al webusercontrol
            Me.IDReader = oTerminalReader.ID

            If oTerminal.Type = "Time Gate" OrElse oTerminal.Type = "Suprema" Then
                Me.whoPunches1.Visible = False
                Me.whoPunches2.Visible = False
                Me.whoPunches3.Visible = False
                Me.simpleDispTab.Attributes("style") = "display: none;"
                Me.ticketTab.Attributes("style") = "display: none;"
                Me.validationTab.Attributes("style") = "display: none;"
            Else
                Me.whoPunches1.Visible = True
                Me.whoPunches2.Visible = True
                Me.whoPunches3.Visible = True
                If Me.simpleDispTab IsNot Nothing AndAlso Me.simpleDispTab.Attributes IsNot Nothing AndAlso Me.simpleDispTab.Attributes("style") IsNot Nothing AndAlso Me.simpleDispTab.Attributes("style").Contains("display: none;") Then
                    Me.simpleDispTab.Attributes("style") = Me.simpleDispTab.Attributes("style").Replace("display: none;", "").Trim()
                End If
                If Me.ticketTab IsNot Nothing AndAlso Me.ticketTab.Attributes IsNot Nothing AndAlso Me.ticketTab.Attributes("style") IsNot Nothing AndAlso Me.ticketTab.Attributes("style").Contains("display: none;") Then
                    Me.ticketTab.Attributes("style") = Me.ticketTab.Attributes("style").Replace("display: none;", "").Trim()
                End If
                If Me.validationTab IsNot Nothing AndAlso Me.validationTab.Attributes IsNot Nothing AndAlso Me.validationTab.Attributes("style") IsNot Nothing AndAlso Me.validationTab.Attributes("style").Contains("display: none;") Then
                    Me.validationTab.Attributes("style") = Me.validationTab.Attributes("style").Replace("display: none;", "").Trim()
                End If
            End If

                CreateDefaultValues(oTerminal, Me, oTerminalReader)

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

#Region "Load Reader Values"

    Private Sub CreateDefaultValues(ByRef oCurrentTerminal As roTerminal, ByRef hControl As WebUserControls_frmTerminalReaderV2, ByVal oReader As roTerminal.roTerminalReader)
        Try

            'GroupBox Que se ficha ------------------------------------------------------

            'Dim oActs As New Generic.List(Of ActivityService.roActivity)
            'oActs = ActivityService.ActivityServiceMethods.GetActivities(Me.Page, False)
            'cmbActivity.Items.Clear()
            'cmbActivity.ValueType = GetType(Integer)
            'cmbActivity.Items.Add(New DevExpress.Web.ListEditItem("", -1))
            'For Each oAct As ActivityService.roActivity In oActs
            '    cmbActivity.Items.Add(New DevExpress.Web.ListEditItem(oAct.Name, oAct.ID))
            'Next

            'Cargamos los centros de coste disponibles
            Dim tbBusinessCenters As DataTable
            tbBusinessCenters = API.TasksServiceMethods.GetBusinessCenters(Me.Page, False)
            cmbCostCenters.Items.Clear()
            cmbCostCenters.ValueType = GetType(Integer)
            cmbCostCenters.Items.Add(New DevExpress.Web.ListEditItem("", -1))
            For Each oRow As DataRow In tbBusinessCenters.Rows
                cmbCostCenters.Items.Add(New DevExpress.Web.ListEditItem(oRow("Name"), oRow("ID")))
            Next
            '/-------------------------------------------------------------------------

            Dim oRelays() As String = oCurrentTerminal.SupportedOutputs.Split(",")
            cmbOutputRelay.Items.Clear()
            cmbInvalidOutputRelay.Items.Clear()
            cmbOutPutCustom.Items.Clear()
            cmbOutputRelay.Items.Add(New DevExpress.Web.ListEditItem("", "0"))
            cmbInvalidOutputRelay.Items.Add(New DevExpress.Web.ListEditItem("", "0"))
            For Each oRelay As String In oRelays
                cmbOutputRelay.Items.Add(New DevExpress.Web.ListEditItem(oRelay, oRelay))
                cmbInvalidOutputRelay.Items.Add(New DevExpress.Web.ListEditItem(oRelay, oRelay))
                cmbOutPutCustom.Items.Add(New DevExpress.Web.ListEditItem(oRelay, oRelay))
            Next

            '/------------------------------------------------------------------------
            'Zones
            cmbPosZoneIn.Items.Clear()
            cmbPosZoneOut.Items.Clear()
            Dim dTbl As DataTable = API.ZoneServiceMethods.GetZones(Me.Page)
            cmbPosZoneIn.Items.Add("", "_" & oReader.ID & "_0,0,0,0,000000_")
            cmbPosZoneOut.Items.Add("", "_" & oReader.ID & "_0,0,0,0,000000_")
            For Each dRow As DataRow In dTbl.Rows
                If Not dRow("IDParent") Is DBNull.Value And roTypes.Any2Integer(dRow("ID")) <> UNESPECIFIED_ZONE Then
                    'If roTypes.Any2Boolean(dRow("IsWorkingZone")) Then
                    cmbPosZoneIn.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID") & "_" & oReader.ID & "_" & RetrievePosZone(dRow("ID")) & "_" & dRow("IDPlane") & "_" & dRow("IsWorkingZone")))
                    'Else
                    cmbPosZoneOut.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID") & "_" & oReader.ID & "_" & RetrievePosZone(dRow("ID")) & "_" & dRow("IDPlane") & "_" & dRow("IsWorkingZone")))
                    'End If
                End If
            Next

            '/------------------------------------------------------------------------
            'Cameras
            Dim dTblCam As DataTable = API.CameraServiceMethods.GetCameras(Me.Page)
            cmbCameraIn.Items.Clear()
            cmbCameraOut.Items.Clear()
            cmbCameraIn.Items.Add("", "")
            cmbCameraIn.ValueType = GetType(Integer)
            cmbCameraOut.Items.Add("", "")
            cmbCameraOut.ValueType = GetType(Integer)
            For Each dRowCam As DataRow In dTblCam.Rows
                cmbCameraIn.Items.Add(New DevExpress.Web.ListEditItem(dRowCam("Name"), dRowCam("ID")))
                cmbCameraOut.Items.Add(New DevExpress.Web.ListEditItem(dRowCam("Name"), dRowCam("ID")))
            Next

            FillBehavioursCombo(oCurrentTerminal, oReader)
        Catch ex As Exception
            Response.Write(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Private Sub FillBehavioursCombo(ByVal oCurrentTerminal As roTerminal, ByVal oReader As roTerminal.roTerminalReader)
        If dTblTemplate Is Nothing Then dTblTemplate = API.TerminalServiceMethods.GetTerminalsReadersTemplate(Me.Page, oCurrentTerminal.Type)

        cmbTerminalModes.Items.Clear()
        cmbTerminalModes.ValueType = GetType(String)
        cmbTerminalModes.Items.Add("", "")

        Dim strFilter2 As String = "IDReader = " & Me.IDReader

        Dim oModeRows() As DataRow = dTblTemplate.Select(strFilter2, "")

        Dim dinningRoomEnabled As Boolean = False
        Dim portalEnabled As Boolean = False
        Dim productiVEnabled As Boolean = False
        Dim costCenterEnabled As Boolean = False
        Dim productionEnbaled As Boolean = False

        Dim isAccessInstalled As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Forms\Access")
        Dim isDinningInstalled As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Forms\DiningRoom")
        Dim isCostCenterInstalled As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

        If oModeRows.Length > 0 Then
            Dim strModeValues As String = ""
            For Each oRow As DataRow In oModeRows

                strModeValues = roTypes.Any2String(oRow("ScopeMode"))

                If strModeValues.IndexOf("EIP") >= 0 Then portalEnabled = True
                If strModeValues.IndexOf("DIN") >= 0 Then dinningRoomEnabled = True
                If strModeValues = "TSK" Then productiVEnabled = True
                If strModeValues = "CO" Then costCenterEnabled = True
                If strModeValues = "JOB" Then productionEnbaled = True
            Next

            strModeValues = ""
            Dim taskModeEnabled As Boolean = False
            Dim eipModeEnabled As Boolean = False
            Dim coModeEnabled As Boolean = False
            Dim jobModeEnabled As Boolean = False
            Dim dinModeEnabled As Boolean = False

            For Each oRow As DataRow In oModeRows

                strModeValues = roTypes.Any2String(oRow("ScopeMode"))

                If strModeValues.IndexOf("TSK") >= 0 Then taskModeEnabled = True Else taskModeEnabled = False
                If strModeValues.IndexOf("EIP") >= 0 Then eipModeEnabled = True Else eipModeEnabled = False
                If strModeValues.IndexOf("CO") >= 0 AndAlso isCostCenterInstalled Then coModeEnabled = True Else coModeEnabled = False
                If strModeValues.IndexOf("JOB") >= 0 AndAlso jobModeEnabled Then jobModeEnabled = True Else jobModeEnabled = False
                If strModeValues.IndexOf("DIN") >= 0 AndAlso isDinningInstalled Then dinModeEnabled = True Else dinModeEnabled = False

                strModeValues = strModeValues.Replace("EIP", "")
                strModeValues = strModeValues.Replace("TSK", "")
                strModeValues = strModeValues.Replace("DIN", "")
                strModeValues = strModeValues.Replace("CO", "")

                Dim actualItems As New Generic.List(Of DevExpress.Web.ListEditItem)

                'La cadena de modo esta formatada de la siguiente manera (0100100_F_X_0_1000_2_0)
                '0100100 --> Modo de trabajo
                'F --> Modo interacción (B:Blind, F:Fast, I:Interactive, M:Manual(equivale a blind)
                'X --> Acción (E:Entrada, S:Salida, X:Automatico, O,L: Accesos)
                '0 --> PRL activo(1:Si, 0:No)
                '1000 --> Modos de validación activos por defecto (1000:ServerLocal, 0100:LocalServer, 0010:Local, 0001: Server)
                '2 --> Numero de zonas activas (1: 1 zona, 2: 2 zonas)
                '0 --> Reles activos (1:si, 0:no)
                '0 --> Zona 1 entrada/salida/cualquiera (1:entrada, 0:salida, 2:cualquiera)
                '0 --> Zona 2 entrada/salida/cualquiera (1:entrada, 0:salida, 2:cualquiera)

                Dim defaultModeWithoutEip As String = "0010"
                If oCurrentTerminal.Type.ToUpper.StartsWith("MX") Then
                    defaultModeWithoutEip = "1000"
                End If

                If Me.IDReader > 1 Then
                    actualItems.Add(New DevExpress.Web.ListEditItem("", "0000000_B_X_1_0000_1_1_2_2"))
                End If

                Select Case strModeValues
                    Case "ACC"
                        If isAccessInstalled Then
                            Dim strLanguageMode = "TerminalMode.OnlyAccess_" & IIf(taskModeEnabled, "TSK", "") & IIf(eipModeEnabled, "EIP", "") & IIf(dinModeEnabled, "DIN", "") & IIf(coModeEnabled, "CO", "")
                            Dim strMode = "10" & IIf(jobModeEnabled, "1", "0") & IIf(taskModeEnabled, "1", "0") & IIf(eipModeEnabled, "1", "0") & IIf(dinModeEnabled, "1", "0") & IIf(coModeEnabled, "1", "0") & "_B_O_1_0001_1_1_2_2"

                            actualItems.Add(New DevExpress.Web.ListEditItem(GenerateLanguageStringFromScopeMode(strLanguageMode), strMode))
                        End If
                    Case "ACCTA"
                        If isAccessInstalled Then
                            Dim strIntMode As String = roTypes.Any2String(oRow("InteractionMode"))
                            Dim strIntAction As String = roTypes.Any2String(oRow("InteractionAction"))

                            Dim strLanguageMode = ""
                            Dim strMode = ""
                            If strIntAction.Contains("X") Then
                                strLanguageMode = "TerminalMode.A1P1_" & IIf(taskModeEnabled, "TSK", "") & IIf(eipModeEnabled, "EIP", "") & IIf(dinModeEnabled, "DIN", "") & IIf(coModeEnabled, "CO", "")
                                strMode = "11" & IIf(jobModeEnabled, "1", "0") & IIf(taskModeEnabled, "1", "0") & IIf(eipModeEnabled, "1", "0") & IIf(dinModeEnabled, "1", "0") & IIf(coModeEnabled, "1", "0") & "_I_L_1_0001_1_2_2_2"
                            Else
                                strLanguageMode = "TerminalMode.A1P2_" & IIf(taskModeEnabled, "TSK", "") & IIf(eipModeEnabled, "EIP", "") & IIf(dinModeEnabled, "DIN", "") & IIf(coModeEnabled, "CO", "")
                                strMode = "11" & IIf(jobModeEnabled, "1", "0") & IIf(taskModeEnabled, "1", "0") & IIf(eipModeEnabled, "1", "0") & IIf(dinModeEnabled, "1", "0") & IIf(coModeEnabled, "1", "0") & "_I_L_1_0001_2_2_2_2"
                            End If

                            actualItems.Add(New DevExpress.Web.ListEditItem(GenerateLanguageStringFromScopeMode(strLanguageMode), strMode))
                        End If
                    Case "TA"
                        Dim strIntMode As String = roTypes.Any2String(oRow("InteractionMode"))
                        Dim strIntAction As String = roTypes.Any2String(oRow("InteractionAction"))

                        If strIntMode = "Fast" AndAlso strIntAction.Contains("X") Then

                            Dim strLanguageMode = "TerminalMode.P2_" & IIf(taskModeEnabled, "TSK", "") & IIf(eipModeEnabled, "EIP", "") & IIf(dinModeEnabled, "DIN", "") & IIf(coModeEnabled, "CO", "")
                            Dim strMode = "01" & IIf(jobModeEnabled, "1", "0") & IIf(taskModeEnabled, "1", "0") & IIf(eipModeEnabled, "1", "0") & IIf(dinModeEnabled, "1", "0") & IIf(coModeEnabled, "1", "0") & "_F_X_1_" & IIf(eipModeEnabled, "0001", defaultModeWithoutEip) & "_2_1_1_0"

                            actualItems.Add(New DevExpress.Web.ListEditItem(GenerateLanguageStringFromScopeMode(strLanguageMode), strMode))
                        End If

                        If strIntAction.Contains("E") Then

                            Dim strLanguageMode = "TerminalMode.P1OnlyIn_" & IIf(taskModeEnabled, "TSK", "") & IIf(eipModeEnabled, "EIP", "") & IIf(dinModeEnabled, "DIN", "") & IIf(coModeEnabled, "CO", "")
                            Dim strMode = "01" & IIf(jobModeEnabled, "1", "0") & IIf(taskModeEnabled, "1", "0") & IIf(eipModeEnabled, "1", "0") & IIf(dinModeEnabled, "1", "0") & IIf(coModeEnabled, "1", "0") & "_M_E_1_" & IIf(eipModeEnabled, "0001", defaultModeWithoutEip) & "_2_1_1_2"

                            actualItems.Add(New DevExpress.Web.ListEditItem(GenerateLanguageStringFromScopeMode(strLanguageMode), strMode))
                        End If

                        If strIntAction.Contains("S") Then
                            Dim strLanguageMode = "TerminalMode.P1OnlyOut_" & IIf(taskModeEnabled, "TSK", "") & IIf(eipModeEnabled, "EIP", "") & IIf(dinModeEnabled, "DIN", "") & IIf(coModeEnabled, "CO", "")
                            Dim strMode = "01" & IIf(jobModeEnabled, "1", "0") & IIf(taskModeEnabled, "1", "0") & IIf(eipModeEnabled, "1", "0") & IIf(dinModeEnabled, "1", "0") & IIf(coModeEnabled, "1", "0") & "_M_S_1_" & IIf(eipModeEnabled, "0001", defaultModeWithoutEip) & "_2_1_2_0"

                            actualItems.Add(New DevExpress.Web.ListEditItem(GenerateLanguageStringFromScopeMode(strLanguageMode), strMode))
                        End If

                        If strIntMode <> "Fast" AndAlso strIntAction.Contains("X") Then
                            Dim strLanguageMode = "TerminalMode.P2Automatic_" & IIf(taskModeEnabled, "TSK", "") & IIf(eipModeEnabled, "EIP", "") & IIf(dinModeEnabled, "DIN", "") & IIf(coModeEnabled, "CO", "")
                            Dim strMode = "01" & IIf(jobModeEnabled, "1", "0") & IIf(taskModeEnabled, "1", "0") & IIf(eipModeEnabled, "1", "0") & IIf(dinModeEnabled, "1", "0") & IIf(coModeEnabled, "1", "0") & "_M_X_1_" & IIf(eipModeEnabled, "0001", defaultModeWithoutEip) & "_2_1_1_0"

                            actualItems.Add(New DevExpress.Web.ListEditItem(GenerateLanguageStringFromScopeMode(strLanguageMode), strMode))
                        End If
                End Select


                For Each oItem As DevExpress.Web.ListEditItem In actualItems
                    If cmbTerminalModes.Items.FindByText(oItem.Text) Is Nothing Then
                        cmbTerminalModes.Items.Add(oItem)
                    End If
                Next
            Next

            Dim unorderedItems As New Generic.List(Of DevExpress.Web.ListEditItem)
            For Each oItem As DevExpress.Web.ListEditItem In cmbTerminalModes.Items
                unorderedItems.Add(oItem)
            Next

            cmbTerminalModes.Items.Clear()

            unorderedItems.Sort(
                    Function(x As DevExpress.Web.ListEditItem, y As DevExpress.Web.ListEditItem)
                        Return x.Text.CompareTo(y.Text)
                    End Function)

            cmbTerminalModes.Items.AddRange(unorderedItems)

            If productionEnbaled Then
                cmbTerminalModes.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TerminalMode.OnlyJOB", Me.DefaultScope), "0010000_I_X_1_0001_1_2_2"))
            End If

            If productiVEnabled Then
                cmbTerminalModes.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TerminalMode.OnlyTSK", Me.DefaultScope), "0001000_I_X_1_0001_1_2_2"))
            End If

            If portalEnabled Then
                cmbTerminalModes.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TerminalMode.OnlyEIP", Me.DefaultScope), "0000100_M_X_1_0001_2_2_2"))
            End If

            If dinningRoomEnabled AndAlso isDinningInstalled Then
                cmbTerminalModes.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TerminalMode.OnlyDIN", Me.DefaultScope), "0000010_M_E_1_0001_1_2_2"))
            End If

            If costCenterEnabled AndAlso isCostCenterInstalled Then
                cmbTerminalModes.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TerminalMode.OnlyCO", Me.DefaultScope), "0000001_M_X_1_0001_2_2_2"))
            End If
        End If

    End Sub

    Private Function GenerateLanguageStringFromScopeMode(ByVal scopeMode As String) As String
        Dim scopeModelng As String = ""

        Dim lngParts As String() = scopeMode.Split("_")
        scopeModelng = Me.Language.Translate(lngParts(0), Me.DefaultScope)

        Dim plusModes As String = lngParts(1)
        If plusModes <> String.Empty Then
            Dim connectWords As Integer = 0
            Dim bEIP As Boolean = False
            Dim bJOB As Boolean = False
            Dim bTSK As Boolean = False
            Dim bDIN As Boolean = False
            Dim bCO As Boolean = False

            Dim bFirst As Boolean = True

            If plusModes.IndexOf("EIP") >= 0 Then
                connectWords += 1
                bEIP = True
            End If

            If plusModes.IndexOf("JOB") >= 0 Then
                connectWords += 1
                bJOB = True
            End If

            If plusModes.IndexOf("TSK") >= 0 Then
                connectWords += 1
                bTSK = True
            End If

            If plusModes.IndexOf("DIN") >= 0 Then
                connectWords += 1
                bDIN = True
            End If

            If plusModes.IndexOf("CO") >= 0 Then
                connectWords += 1
                bCO = True
            End If

            If connectWords > 0 Then
                scopeModelng = scopeModelng & " " & Me.Language.Translate("TerminalMode.connectWords", Me.DefaultScope)
            End If

            If bEIP Then
                scopeModelng = scopeModelng & " " & Me.Language.Translate("TerminalMode.EIP", Me.DefaultScope) & " "
                connectWords -= 1

                bFirst = False
            End If

            If bJOB Then
                If connectWords > 0 AndAlso connectWords = 1 Then
                    If bFirst Then
                        scopeModelng = scopeModelng & " "
                    Else
                        scopeModelng = scopeModelng & " " & Me.Language.Translate("TerminalMode.endWords", Me.DefaultScope) & " "
                    End If
                Else
                    scopeModelng = scopeModelng & ", "
                End If
                scopeModelng = scopeModelng & Me.Language.Translate("TerminalMode.JOB", Me.DefaultScope)
                connectWords -= 1

                bFirst = False
            End If

            If bTSK Then
                If connectWords > 0 AndAlso connectWords = 1 Then
                    If bFirst Then
                        scopeModelng = scopeModelng & " "
                    Else
                        scopeModelng = scopeModelng & " " & Me.Language.Translate("TerminalMode.endWords", Me.DefaultScope) & " "
                    End If
                Else
                    scopeModelng = scopeModelng & ", "
                End If
                scopeModelng = scopeModelng & Me.Language.Translate("TerminalMode.TSK", Me.DefaultScope)
                connectWords -= 1

                bFirst = False
            End If

            If bDIN Then
                If connectWords > 0 AndAlso connectWords = 1 Then
                    If bFirst Then
                        scopeModelng = scopeModelng & " "
                    Else
                        scopeModelng = scopeModelng & " " & Me.Language.Translate("TerminalMode.endWords", Me.DefaultScope) & " "
                    End If
                Else
                    scopeModelng = scopeModelng & ", "
                End If
                scopeModelng = scopeModelng & Me.Language.Translate("TerminalMode.DIN", Me.DefaultScope)
                connectWords -= 1

                bFirst = False
            End If

            If bCO Then
                If connectWords > 0 AndAlso connectWords = 1 Then
                    If bFirst Then
                        scopeModelng = scopeModelng & " "
                    Else
                        scopeModelng = scopeModelng & " " & Me.Language.Translate("TerminalMode.endWords", Me.DefaultScope) & " "
                    End If
                Else
                    scopeModelng = scopeModelng & ", "
                End If
                scopeModelng = scopeModelng & Me.Language.Translate("TerminalMode.CO", Me.DefaultScope)
                connectWords -= 1

                bFirst = False
            End If

        End If

        Return scopeModelng
    End Function

    Private Function RetrievePosZone(ByVal IDZone As Integer) As String
        Dim retPos As String = "0,0,0,0,000000"
        Try
            Dim oZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, IDZone, False)
            If oZone IsNot Nothing Then
                Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(oZone.Color)
                Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(auxColor)

                If oZone.X1 <> -1 Then
                    retPos = oZone.X1 & "," & oZone.Y1 & "," & oZone.X2 & "," & oZone.Y2 & "," & oHTMLColor.Replace("#", "")
                End If

            End If
        Catch ex As Exception
        End Try
        Return retPos
    End Function

    Public Function LoadReaderValueFromSource(ByVal oTerminal As roTerminal, ByVal oPermission As Permission, ByVal bLegacyModeEnabled As Boolean, Optional ByVal strFieldChanged As String = "ScopeMode") As roTerminal.roTerminalReader
        Dim oTerminalReader As New roTerminal.roTerminalReader

        oTerminalReader.IDTerminal = oTerminal.ID
        oTerminalReader.ID = Me.IDReader
        oTerminalReader.Description = "Reader " & Me.IDReader
        oTerminalReader.UseDispKey = chkUseDispKey.Checked
        oTerminalReader.Type = oTerminal.Readers(Me.IDReader - 1).Type
        oTerminalReader.InteractiveConfig = oTerminal.Readers(Me.IDReader - 1).InteractiveConfig

        SetBehaviour(oTerminalReader)

        If Not bLegacyModeEnabled Then
            oTerminalReader.EmployeesLimit = New Generic.List(Of Integer)
        Else
            Dim lstEmployees As Generic.List(Of Integer) = Nothing
            Dim lstGroups As Generic.List(Of Integer) = Nothing

            Dim employeesSelected As String() = Me.hdnEmployees.Value.Split(",")

            For Each oStr As String In employeesSelected
                If oStr <> String.Empty Then
                    If lstEmployees Is Nothing Then lstEmployees = New Generic.List(Of Integer)
                    If lstGroups Is Nothing Then lstGroups = New Generic.List(Of Integer)
                    If oStr.StartsWith("A") Then
                        lstGroups.Add(roTypes.Any2Integer(oStr.Replace("A", "")))
                    Else
                        lstEmployees.Add(roTypes.Any2Integer(oStr.Replace("B", "")))
                    End If
                End If
            Next

            If lstGroups IsNot Nothing Then

                Dim strFilter As String = roTypes.Any2String(hdnFilter.Value)
                Dim strFilterUser As String = roTypes.Any2String(hdnFilterUser.Value)
                Dim tmp As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Nothing, lstGroups.ToArray,
                                                                                                                                           "Employees", "U", strFilter, strFilterUser)
                lstEmployees.AddRange(tmp)
            End If

            'Eliminar posibles duplicados de la lista de empleados y guarda
            If lstEmployees IsNot Nothing Then
                oTerminalReader.EmployeesLimit = lstEmployees.Distinct().ToList()
            Else
                oTerminalReader.EmployeesLimit = New Generic.List(Of Integer)
            End If
        End If

        If cmbPosZoneIn.SelectedItem IsNot Nothing Then
            Dim selZone As String = roTypes.Any2String(cmbPosZoneIn.SelectedItem.Value).Split("_")(0)
            If selZone <> String.Empty Then
                oTerminalReader.IDZone = roTypes.Any2Integer(selZone)

                If FlReadersIn.Value <> "" AndAlso FlReadersIn.Value <> "0,0" Then
                    oTerminalReader.PictureX = roTypes.Any2Double(FlReadersIn.Value.Split(",")(0))
                    oTerminalReader.PictureY = roTypes.Any2Double(FlReadersIn.Value.Split(",")(1))
                End If
            Else
                oTerminalReader.IDZone = Nothing
            End If
        End If
        If cmbCameraIn.SelectedItem IsNot Nothing Then oTerminalReader.IDCamera = cmbCameraIn.SelectedItem.Value

        If cmbPosZoneOut.SelectedItem IsNot Nothing Then
            Dim selZone As String = roTypes.Any2String(cmbPosZoneOut.SelectedItem.Value).Split("_")(0)
            If selZone <> String.Empty Then
                oTerminalReader.IDZoneOut = roTypes.Any2Integer(selZone)

                If FlReadersOut.Value <> "" AndAlso FlReadersOut.Value <> "0,0" Then
                    oTerminalReader.PictureXOut = roTypes.Any2Double(FlReadersOut.Value.Split(",")(0))
                    oTerminalReader.PictureYOut = roTypes.Any2Double(FlReadersOut.Value.Split(",")(1))
                End If
            Else
                oTerminalReader.IDZoneOut = Nothing
            End If
        End If
        If cmbCameraOut.SelectedItem IsNot Nothing Then oTerminalReader.IDCameraOut = cmbCameraOut.SelectedItem.Value

        If optServerLocal.Checked Then
            oTerminalReader.ValidationMode = ValidationMode.ServerLocal
        ElseIf optServer.Checked Then
            oTerminalReader.ValidationMode = ValidationMode.Server
        ElseIf optLocalServer.Checked Then
            oTerminalReader.ValidationMode = ValidationMode.LocalServer
        ElseIf optLocal.Checked Then
            oTerminalReader.ValidationMode = ValidationMode.Local
        End If

        'Dispositivos simples (fichaje valido)
        If cmbOutputRelay.SelectedItem IsNot Nothing Then oTerminalReader.Output = roTypes.Any2Integer(cmbOutputRelay.SelectedItem.Value)
        If txtOutPutDuration.Value <> String.Empty Then oTerminalReader.Duration = roTypes.Any2Integer(txtOutPutDuration.Value)
        If Not optChkOutPut.Checked Then
            oTerminalReader.Output = Nothing
        End If

        If cmbInvalidOutputRelay.SelectedItem IsNot Nothing Then oTerminalReader.InvalidOutput = roTypes.Any2Integer(cmbInvalidOutputRelay.SelectedItem.Value)
        If txtInvalidOutPutDuration.Value <> String.Empty Then oTerminalReader.InvalidDuration = roTypes.Any2Integer(txtInvalidOutPutDuration.Value)
        If Not optChkInvalidOutPut.Checked Then
            oTerminalReader.InvalidOutput = Nothing
        End If

        If optChkPrintTicket.Checked AndAlso cmbPrinters.Text IsNot Nothing Then
            If oTerminalReader.InteractiveConfig Is Nothing Then
                Dim oIntConfig As New roTerminal.roTerminalReader.roInteractiveConfig()
                oTerminalReader.InteractiveConfig = oIntConfig
            End If
            oTerminalReader.InteractiveConfig.PrinterName = cmbPrinters.Text
        Else
            optChkPrintTicket.Checked = False
        End If

        If optChkCustomButtons.Checked Then
            If oTerminalReader.CustomButtons.Count = 0 Then
                Dim oCustButton As New roTerminal.roTerminalReader.roCustomButton()
                oTerminalReader.CustomButtons.Add(oCustButton)
            End If

            oTerminalReader.CustomButtons(0).Label = txtTextButton.Value
            If cmbOutPutCustom.SelectedItem IsNot Nothing Then oTerminalReader.CustomButtons(0).Output = cmbOutPutCustom.SelectedItem.Value
            If txtOutputCustomDuration.Value <> "" Then oTerminalReader.CustomButtons(0).Duration = txtOutputCustomDuration.Value
            oTerminalReader.CustomButtons(0).OnlyEntries = chkOnlyEntrance.Checked
        End If

        Dim htmlContent As String = dxPrinterNameEditor.Html
        If Not htmlContent.StartsWith("<p>") AndAlso Not String.IsNullOrEmpty(htmlContent.Trim()) Then
            oTerminalReader.InteractiveConfig.PrinterText = $"<p>{htmlContent}</p>"
        Else
            oTerminalReader.InteractiveConfig.PrinterText = htmlContent
        End If

        Return oTerminalReader
    End Function

    Public Function LoadReaderValue(ByVal oTerminal As roTerminal, ByVal oTerminalReader As roTerminal.roTerminalReader, ByVal oPermission As Permission) As Boolean
        Try
            Dim nCountReader As Integer = 0

            Dim oDisable As Boolean = False
            If oPermission < Permission.Write Then
                oDisable = True
            End If

            For Each cTerminalReader As roTerminal.roTerminalReader In oTerminal.Readers
                nCountReader = nCountReader + 1
                If cTerminalReader.ID = oTerminalReader.ID Then Exit For
            Next

            chkUseDispKey.Checked = oTerminalReader.UseDispKey

            Dim strModeBehaviour As String = Me.RecoverBehaviour(oTerminalReader)
            cmbTerminalModes.SelectedItem = cmbTerminalModes.Items.FindByValue("")
            Me.hdnTerminalModeSelected.Value = ""
            For Each oModeItem As DevExpress.Web.ListEditItem In cmbTerminalModes.Items
                If oModeItem.Value.ToString.StartsWith(strModeBehaviour) Then
                    cmbTerminalModes.SelectedItem = oModeItem
                    Me.hdnTerminalModeSelected.Value = oModeItem.Value
                    Exit For
                End If
            Next

            'EmployeesLimit
            Dim oEmpLim As New Generic.List(Of Integer)
            Dim strEmpLim As String = ""
            For Each oEL As Integer In oTerminalReader.EmployeesLimit
                oEmpLim.Add(oEL)
                strEmpLim &= "B" & oEL.ToString & ","
            Next

            Me.divModeLegacy.Style("display") = "none"
            Me.divNobodyCanPunch.Style("display") = "none"
            Me.divCanPunchNoRestriction.Style("display") = "none"
            Me.divCanPunchRestrictedAuthorizations.Style("display") = "none"

            Me.tobAuthorizations.Tokens.Clear()
            If Not oTerminalReader.LegacyRestrictionModeAllowed Then

                If oTerminalReader.ScopeMode.ToString.ToUpper.Contains("ACC") OrElse Me.bolCheckAccessAuthorizationOnNoAccessReaders Then

                    If oTerminalReader.IDZone.HasValue AndAlso oTerminalReader.IDZone.Value > 0 Then
                        Dim oAccessGroups = AccessGroupServiceMethods.GetAuthorizationsByZone(Me.Page, oTerminalReader.IDZone.Value)

                        If oAccessGroups IsNot Nothing AndAlso oAccessGroups.Length > 0 Then
                            For Each oGroup In oAccessGroups
                                Me.tobAuthorizations.Tokens.Add(oGroup.Name)
                            Next

                            Me.tobAuthorizations.ShowDropDownOnFocus = False

                            Me.divCanPunchRestrictedAuthorizations.Style("display") = ""
                        Else
                            Me.divNobodyCanPunch.Style("display") = ""
                        End If
                    Else
                        Me.divNobodyCanPunch.Style("display") = ""
                    End If
                Else
                    Me.divCanPunchNoRestriction.Style("display") = ""
                End If
            Else

                Me.divModeLegacy.Style("display") = ""

                If strEmpLim.Length > 0 Then
                    strEmpLim = strEmpLim.Substring(0, Len(strEmpLim) - 1)
                    hdnEmployees.Value = strEmpLim
                    hdnFilter.Value = ""
                    hdnFilterUser.Value = ""
                    aFEmployees.InnerHtml = oEmpLim.Count & " " & Me.Language.Translate("SelectedEmployees", DefaultScope)
                Else
                    hdnEmployees.Value = ""
                    hdnFilter.Value = ""
                    hdnFilterUser.Value = ""
                    aFEmployees.InnerHtml = Me.Language.Translate("lblAllEmp", DefaultScope)
                End If

                'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS TreeV3
                HelperWeb.roSelector_Initialize("objContainerTreeV3_" & Me.ClientID & "_AccFilterTreeTermLimitEmp")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_" & Me.ClientID & "_AccFilterTreeTermLimitEmpGrid")
                HelperWeb.roSelector_SetSelection(oEmpLim, New Generic.List(Of Integer), "objContainerTreeV3_" & Me.ClientID & "_AccFilterTreeTermLimitEmpGrid", False)

            End If

            cmbCostCenters.SelectedItem = cmbCostCenters.Items.FindByValue(-1)
            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl") AndAlso oTerminalReader.IDCostCenter.HasValue Then
                cmbCostCenters.SelectedItem = cmbCostCenters.Items.FindByValue(oTerminalReader.IDCostCenter)
            End If

            'Como se valida?
            optServerLocal.Checked = False
            optLocalServer.Checked = False
            optLocal.Checked = False
            optServer.Checked = False

            Select Case oTerminalReader.ValidationMode
                Case ValidationMode.ServerLocal
                    HDNVAL.Value = "0"
                    optServerLocal.Checked = True
                Case ValidationMode.LocalServer
                    HDNVAL.Value = "1"
                    optLocalServer.Checked = True
                    HDNVAL.Value = "2"
                Case ValidationMode.Local, ValidationMode.None
                    HDNVAL.Value = "3"
                    optLocal.Checked = True
                Case ValidationMode.Server
                    HDNVAL.Value = "4"
                    optServer.Checked = True
            End Select

            'Posicio FLASH, etc.
            Dim iIDZone As String = ""
            If oTerminalReader.IDZone.HasValue Then iIDZone = oTerminalReader.IDZone.Value
            For Each cVal As DevExpress.Web.ListEditItem In cmbPosZoneIn.Items
                If roTypes.Any2String(cVal.Value).StartsWith(iIDZone.ToString & "_") Then
                    cmbPosZoneIn.SelectedItem = cVal
                    Me.hdnCmbPosZoneInSelection.Value = cVal.Value
                    Exit For
                End If
            Next

            'Si hi ha zona, la recuperem al planol
            If iIDZone <> "" Then
                Dim oZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, oTerminalReader.IDZone.Value, False)
                If oZone IsNot Nothing Then
                    Dim strPosition As String = ""
                    Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(oZone.Color)
                    Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(auxColor)

                    If oZone.X1 <> -1 Then
                        strPosition = oZone.X1 & "," & oZone.Y1 & "," & oZone.X2 & "," & oZone.Y2 & "," & oHTMLColor.Replace("#", "")
                    End If
                    FlPositionIn.Value = strPosition

                    'Recuperem el ID de la imatge a carregar la zona
                    Dim imgID As String = ""

                    If oZone.IDPlane.HasValue Then
                        imgID = oZone.IDPlane.Value
                    End If

                    FlZoneImgIn.Value = imgID
                End If
            Else
                FlZoneImgIn.Value = ""
            End If

            Dim oPosReaders As String = ""

            If oTerminalReader.PictureX.HasValue And oTerminalReader.PictureY.HasValue Then
                FlReadersIn.Value = oTerminalReader.PictureX & "," & oTerminalReader.PictureY
            Else
                FlReadersIn.Value = "0,0"
            End If

            'Posicio actual del reader
            FlActualReaderIn.Value = nCountReader

            Dim iIDZoneOut As String = ""
            If oTerminalReader.IDZoneOut.HasValue Then iIDZoneOut = oTerminalReader.IDZoneOut.Value
            For Each cVal As DevExpress.Web.ListEditItem In cmbPosZoneOut.Items
                If roTypes.Any2String(cVal.Value).StartsWith(iIDZoneOut.ToString & "_") Then
                    cmbPosZoneOut.SelectedItem = cVal
                    Me.hdnCmbPosZoneOutSelection.Value = cVal.Value
                    Exit For
                End If
            Next

            'Si hi ha zona, la recuperem al planol
            If iIDZoneOut <> "" Then
                Dim oZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, oTerminalReader.IDZoneOut.Value, False)
                If oZone IsNot Nothing Then
                    Dim strPosition As String = ""
                    Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(oZone.Color)
                    Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(auxColor)

                    If oZone.X1 <> -1 Then
                        strPosition = oZone.X1 & "," & oZone.Y1 & "," & oZone.X2 & "," & oZone.Y2 & "," & oHTMLColor.Replace("#", "")
                    End If
                    FlPositionOut.Value = strPosition

                    'Recuperem el ID de la imatge a carregar la zona
                    Dim imgID As String = ""

                    If oZone.IDPlane.HasValue Then
                        imgID = oZone.IDPlane.Value
                    End If

                    FlZoneImgOut.Value = imgID
                End If
            Else
                FlZoneImgOut.Value = ""
            End If

            oPosReaders = ""

            If oTerminalReader.PictureXOut.HasValue And oTerminalReader.PictureYOut.HasValue Then
                FlReadersOut.Value = oTerminalReader.PictureXOut & "," & oTerminalReader.PictureYOut
            Else
                FlReadersOut.Value = "0,0"
            End If

            'Dispositivos simples (fichaje valido)
            optChkOutPut.Checked = False
            cmbOutputRelay.SelectedItem = cmbOutputRelay.Items.FindByValue("")
            txtOutPutDuration.Value = ""
            If oTerminalReader.Output.HasValue Then
                If oTerminalReader.Output.Value > 0 Then
                    optChkOutPut.Checked = True
                    cmbOutputRelay.SelectedItem = cmbOutputRelay.Items.FindByValue(oTerminalReader.Output.Value.ToString)
                    txtOutPutDuration.Value = oTerminalReader.Duration.ToString
                End If
            End If

            'Dispositivos simples (fichaje invalido)
            optChkInvalidOutPut.Checked = False
            cmbInvalidOutputRelay.SelectedItem = cmbInvalidOutputRelay.Items.FindByValue("")
            txtInvalidOutPutDuration.Value = ""
            If oTerminalReader.InvalidOutput.HasValue Then
                If oTerminalReader.InvalidOutput.Value > 0 Then
                    optChkInvalidOutPut.Checked = True
                    cmbInvalidOutputRelay.SelectedItem = cmbInvalidOutputRelay.Items.FindByValue(oTerminalReader.InvalidOutput.Value)
                    txtInvalidOutPutDuration.Value = oTerminalReader.InvalidDuration.ToString
                End If
            End If

            'Dispositivos simples (imprimir ticket)
            optChkPrintTicket.Checked = False
            Dim sPrinterName As String = String.Empty

            If oTerminalReader.InteractiveConfig IsNot Nothing Then
                sPrinterName = roTypes.Any2String(oTerminalReader.InteractiveConfig.PrinterName)
                'cmbCustomizeTicket.Text = roTypes.Any2String(oTerminalReader.InteractiveConfig.PrinterText)
                'Fase2
                dxPrinterNameEditor.Html = oTerminalReader.InteractiveConfig.PrinterText
                'Temps entre marcatges (InteractiveConfig)
                'Dim oTimeRTIn As String = Format(CDate(roTypes.Any2Time(oTerminalReader.InteractiveConfig.PunchPeriodRTIn / 60).Value), "HH:mm")
                'Dim oTimeRTOut As String = Format(CDate(roTypes.Any2Time(oTerminalReader.InteractiveConfig.PunchPeriodRTOut / 60).Value), "HH:mm")

                'If sPrinterName <> String.Empty AndAlso sPrinterName = "cloud" AndAlso roTypes.Any2String(HelperSession.AdvancedParametersCache("DinnerPrintOnFile", True)) <> String.Empty Then
                '    sPrinterName = roTypes.Any2String(HelperSession.AdvancedParametersCache("DinnerPrintOnFile"))
                'Else
                '    sPrinterName = String.Empty
                'End If
            End If

            If sPrinterName <> String.Empty Then
                optChkPrintTicket.Checked = True
            End If

            dxPrinterNameEditor.Settings.AllowHtmlView = False
            dxPrinterNameEditor.Settings.AllowPreview = False
            dxPrinterNameEditor.SettingsHtmlEditing.AllowIFrames = False
            dxPrinterNameEditor.SettingsHtmlEditing.AllowScripts = False
            dxPrinterNameEditor.SettingsHtmlEditing.AllowFormElements = False
            dxPrinterNameEditor.SettingsHtmlEditing.EnablePasteOptions = False
            dxPrinterNameEditor.SettingsHtmlEditing.PasteMode = HtmlEditorPasteMode.PlainText
            dxPrinterNameEditor.Placeholders.Add("GroupName")
            dxPrinterNameEditor.Placeholders.Add("NameEmployee")
            dxPrinterNameEditor.Placeholders.Add("IdEmployee")
            dxPrinterNameEditor.Placeholders.Add("TurnName")
            dxPrinterNameEditor.Placeholders.Add("PunchDate")
            dxPrinterNameEditor.Placeholders.Add("PunchTime")
            'Campos de la ficha
            Dim dTblUF As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "Used=1", False)
            For Each dRow As DataRow In dTblUF.Rows
                If Not IsDBNull(dRow("FieldType")) Then
                    dxPrinterNameEditor.Placeholders.Add("USR_" & dRow("FieldName"))
                End If
            Next
            cmbPrinters.Text = sPrinterName

            'Dispositivos simples (CustomButtons(0) ----------)
            txtTextButton.Value = ""
            cmbOutPutCustom.SelectedItem = cmbOutPutCustom.Items.FindByValue("")
            txtOutputCustomDuration.Value = ""
            optChkCustomButtons.Checked = False
            chkOnlyEntrance.Checked = False
            If oTerminalReader.CustomButtons IsNot Nothing Then
                If oTerminalReader.CustomButtons.Count > 0 Then
                    optChkCustomButtons.Checked = True
                    txtTextButton.Value = oTerminalReader.CustomButtons(0).Label
                    cmbOutPutCustom.SelectedItem = cmbOutPutCustom.Items.FindByValue(oTerminalReader.CustomButtons(0).Output.ToString)
                    txtOutputCustomDuration.Value = oTerminalReader.CustomButtons(0).Duration.ToString
                    chkOnlyEntrance.Checked = oTerminalReader.CustomButtons(0).OnlyEntries
                End If
            End If

            If oTerminalReader.IDCamera.HasValue Then
                cmbCameraIn.SelectedItem = cmbCameraIn.Items.FindByValue(oTerminalReader.IDCamera)
            End If

            If oTerminalReader.IDCameraOut.HasValue Then
                cmbCameraOut.SelectedItem = cmbCameraOut.Items.FindByValue(oTerminalReader.IDCameraOut)
            End If

            'If oTerminalReader.CustomButtons IsNot Nothing Then
            '    If oTerminalReader.CustomButtons.Length > 0 Then
            '        If oTerminalReader.CustomButtons(0).Conditions IsNot Nothing Then
            '            If oTerminalReader.CustomButtons(0).Conditions.Length > 0 Then 'Conditions
            '                loadValuesCriteria(Me, oTerminalReader.CustomButtons(0).Conditions(0))
            '            End If
            '        End If
            '    End If
            'End If

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub SetBehaviour(ByRef oTerminalReader As roTerminal.roTerminalReader)
        If cmbTerminalModes.SelectedItem IsNot Nothing Then
            Dim strBehaviourMode As String = cmbTerminalModes.SelectedItem.Value

            If strBehaviourMode <> String.Empty Then
                Dim strParameters As String() = strBehaviourMode.Split("_")

                oTerminalReader.ScopeMode = DTOs.ScopeMode.UNDEFINED
                Dim scopeMode As String = ""
                If strParameters(0)(0) = "1" Then scopeMode &= "ACC" Else scopeMode &= ""
                If strParameters(0)(1) = "1" Then scopeMode &= "TA" Else scopeMode &= ""
                If strParameters(0)(2) = "1" Then scopeMode &= "JOB" Else scopeMode &= ""
                If strParameters(0)(3) = "1" Then scopeMode &= "TSK" Else scopeMode &= ""
                If strParameters(0)(4) = "1" Then scopeMode &= "EIP" Else scopeMode &= ""
                If strParameters(0)(5) = "1" Then scopeMode &= "DIN" Else scopeMode &= ""
                If strParameters(0)(6) = "1" Then scopeMode &= "CO" Else scopeMode &= ""

                Try
                    oTerminalReader.ScopeMode = [Enum].Parse(GetType(ScopeMode), scopeMode)
                Catch ex As Exception
                    oTerminalReader.ScopeMode = DTOs.ScopeMode.UNDEFINED
                End Try

                If oTerminalReader.ScopeMode <> DTOs.ScopeMode.UNDEFINED Then
                    oTerminalReader.IDActivity = Nothing

                    oTerminalReader.OHP = False
                    'If cmbActivity.SelectedItem IsNot Nothing Then
                    '    If cmbActivity.SelectedItem.Value <> -1 Then
                    '        oTerminalReader.IDActivity = cmbActivity.SelectedItem.Value
                    '        oTerminalReader.OHP = True
                    '    End If
                    'End If

                    oTerminalReader.IDCostCenter = Nothing
                    If cmbCostCenters.SelectedItem IsNot Nothing Then
                        If cmbCostCenters.SelectedItem.Value <> -1 Then
                            oTerminalReader.IDCostCenter = cmbCostCenters.SelectedItem.Value
                        End If
                    End If

                    Select Case strParameters(2)
                        Case "E"
                            oTerminalReader.InteractionMode = InteractionMode.Blind
                            oTerminalReader.InteractionAction = InteractionAction.E
                        Case "S"
                            oTerminalReader.InteractionMode = InteractionMode.Blind
                            oTerminalReader.InteractionAction = InteractionAction.S
                        Case "X"
                            If strParameters(1) = "M" Then
                                oTerminalReader.InteractionMode = InteractionMode.Blind
                                oTerminalReader.InteractionAction = InteractionAction.X
                            ElseIf strParameters(1) = "I" Then
                                oTerminalReader.InteractionMode = InteractionMode.Interactive
                                oTerminalReader.InteractionAction = InteractionAction.X
                            Else
                                oTerminalReader.InteractionMode = InteractionMode.Fast
                                oTerminalReader.InteractionAction = InteractionAction.ES
                            End If
                        Case "L"
                            Select Case strParameters(5)
                                Case "1"
                                    oTerminalReader.InteractionMode = InteractionMode.Blind
                                    oTerminalReader.InteractionAction = InteractionAction.X
                                Case "2"
                                    oTerminalReader.InteractionMode = InteractionMode.Blind
                                    oTerminalReader.InteractionAction = InteractionAction.ES
                            End Select
                    End Select
                End If
            Else
                oTerminalReader.ScopeMode = ScopeMode.UNDEFINED
            End If
        Else
            oTerminalReader.ScopeMode = ScopeMode.UNDEFINED
        End If
    End Sub

    Public Function RecoverBehaviour(ByVal oTerminalReader As roTerminal.roTerminalReader) As String
        Dim strBehaviour As String = ""

        If oTerminalReader.ScopeMode = ScopeMode.UNDEFINED Then
            strBehaviour = "0000000_B_X"
        Else
            Dim strScopeMode As String = oTerminalReader.ScopeMode.ToString()

            If strScopeMode.ToUpper.Contains("ACC") Then strBehaviour &= "1" Else strBehaviour &= "0"
            If strScopeMode.ToUpper.Contains("TA") Then strBehaviour &= "1" Else strBehaviour &= "0"
            If strScopeMode.ToUpper.Contains("JOB") Then strBehaviour &= "1" Else strBehaviour &= "0"
            If strScopeMode.ToUpper.Contains("TSK") Then strBehaviour &= "1" Else strBehaviour &= "0"
            If strScopeMode.ToUpper.Contains("EIP") Then strBehaviour &= "1" Else strBehaviour &= "0"
            If strScopeMode.ToUpper.Contains("DIN") Then strBehaviour &= "1" Else strBehaviour &= "0"
            If strScopeMode.ToUpper.Contains("CO") Then strBehaviour &= "1" Else strBehaviour &= "0"

            If strScopeMode.ToUpper.StartsWith("ACC") Then
                If Not strScopeMode.ToUpper.Contains("TA") Then
                    strBehaviour &= "_B_O"
                Else
                    strBehaviour &= "_I_L"
                End If
            ElseIf strScopeMode.ToUpper.StartsWith("TSK") Then
                strBehaviour &= "_I_X"
            ElseIf strScopeMode.ToUpper.StartsWith("EIP") Then
                strBehaviour &= "_M_X"
            ElseIf strScopeMode.ToUpper.StartsWith("DIN") Then
                strBehaviour &= "_M_E"
            ElseIf strScopeMode.ToUpper.StartsWith("CO") Then
                strBehaviour &= "_M_X"
            End If
        End If

        If strBehaviour.StartsWith("1") Then
            Select Case oTerminalReader.InteractionAction
                Case InteractionAction.X
                    strBehaviour = strBehaviour & "_1_0001_1"
                Case InteractionAction.ES
                    strBehaviour = strBehaviour & "_1_0001_2"
                Case Else
                    strBehaviour = strBehaviour & "_1_0001_1"
            End Select
        ElseIf strBehaviour.StartsWith("0000") Then
            strBehaviour = strBehaviour & "_1"
        ElseIf strBehaviour.StartsWith("000100") Then
            strBehaviour = strBehaviour & "_1"
        Else
            Select Case oTerminalReader.InteractionAction
                Case InteractionAction.E
                    strBehaviour = strBehaviour & "_M_E_1"
                Case InteractionAction.S
                    strBehaviour = strBehaviour & "_M_S_1"
                Case InteractionAction.X
                    strBehaviour = strBehaviour & "_M_X_1"
                Case Else
                    strBehaviour = strBehaviour & "_F_X_1"
            End Select
        End If

        Return strBehaviour
    End Function

#End Region

End Class