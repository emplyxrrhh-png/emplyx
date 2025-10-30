Imports System.ServiceModel

Public Class DatalinkTestWS
    Private strInitialContract As String = String.Empty
    Private strDocumentPath As String = String.Empty
    Private strDocumentData As String = String.Empty
    Private bEmployeeFromJSON As Boolean = False
    Private bSAGE200c As Boolean = False

    Private Sub DatalinkTestWS_Load(sender As Object, e As EventArgs) Handles Me.Load
        cmbAbsenceAction.SelectedItem = cmbAbsenceAction.Items(0)
        dtBeginDate.Value = DateTime.Now.Date


        If ckMultitenantApi.Checked Then
            Me.txtCompanyClientCode.Enabled = True
        Else
            Me.txtCompanyClientCode.Enabled = False
        End If

    End Sub

    Private Sub btnCreateEmployee_Click(sender As Object, e As EventArgs) Handles btnCreateEmployee.Click

        Dim sendEmployeeThread As Threading.Thread = New Threading.Thread(AddressOf SendBatchEmployee)
        sendEmployeeThread.Start()

    End Sub

    Private Sub btnCreateEmployeeFromJSON_Click(sender As Object, e As EventArgs) Handles btnEmployeeFromJson.Click
        bEmployeeFromJSON = True

        If txtEmployeesJson.Text.Trim.Length > 0 Then
            Dim sendEmployeeThread As Threading.Thread = New Threading.Thread(AddressOf SendBatchEmployee)
            sendEmployeeThread.Start()
        Else
            MsgBox("Debe informar una cadena JSON válida con los datos del empleado")
        End If
    End Sub


    Private Sub btnSAGE200cCreateEmployeeFromJSON_Click(sender As Object, e As EventArgs) Handles Button4.Click
        bEmployeeFromJSON = True
        bSAGE200c = True

        If txtEmployeesJson.Text.Trim.Length > 0 Then
            Dim sendEmployeeThread As Threading.Thread = New Threading.Thread(AddressOf SendBatchEmployee)
            sendEmployeeThread.Start()
        Else
            MsgBox("Debe informar una cadena JSON válida con los datos del empleado")
        End If
    End Sub

    Private Sub SendBatchEmployee()
        strInitialContract = txtContractID.Text

        Dim sendEmployeeThread As Threading.Thread = New Threading.Thread(AddressOf SendEmployee)
        sendEmployeeThread.Start(1)
    End Sub

    Private Function SendEmployee(ByVal idEmployee As Integer) As Integer
        Dim result As Integer
        Dim oSvc As Object
        Try

            Dim curIndex As Integer = idEmployee


            Dim strContractID = txtContractID.Text ' strInitialContract & "." & idEmployee.ToString
            Dim strImportKey = txtImportKey.Text ' strInitialContract & "." & idEmployee.ToString
            Dim strNifPunch = txtNifPunch.Text ' strInitialContract & "." & idEmployee.ToString
            Dim strName = txtName.Text ' "Employee WS " & idEmployee.ToString
            Dim strCard = txtCard.Text ' "1000000" & curIndex.ToString

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If

            Dim htUserFields = Nothing
            Dim oFieldsArray = Nothing
            Dim strAuthorizationsArray = Nothing

            Dim strLevels As String() = txtCompany.Text.Split("\\")

            Dim xBeginContract As Date = dtBeginDate.Value.Date
            Dim xEndContract As Nullable(Of Date) = If(ckActiveContract.Checked, Nothing, dtEndDate.Value.Date)

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then

                    htUserFields = New List(Of ExternalApi2SSL.roDatalinkEmployeeUserFieldValue)
                    For Each oField As String In txtUserFields.Text.Split("#")
                        Dim oFieldValue As String() = oField.Split(":")
                        If oFieldValue.Length = 2 Then htUserFields.Add(New ExternalApi2SSL.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = oFieldValue(0), .UserFieldValue = oFieldValue(1), .UserFieldValueDate = New Date(1970, 1, 1)})
                    Next

                    Dim oNewEmployee As ExternalApi2SSL.roDatalinkStandarEmployee

                    If Not bEmployeeFromJSON Then
                        oNewEmployee = New ExternalApi2SSL.roDatalinkStandarEmployee With {
                                .NombreEmpleado = strName,
                                .UniqueEmployeeID = strImportKey,
                                .IDContract = strContractID,
                                .NifEmpleado = strNifPunch,
                                .StartContractDate = xBeginContract,
                                .EndContractDate = xEndContract,
                                .MobilityDate = IIf(ckAppliMobility.Checked, dtMobility.Value.Date, Nothing),
                                .CompositeContractType = ExternalApiSSL.eCompositeContractType.ContractAndDate,
                                .UserFields = htUserFields.ToArray(),
                                .CardNumber = strCard,
                                .Nivel0 = If(strLevels.Length > 0, strLevels(0), ""),
                                .Nivel1 = If(strLevels.Length > 1, strLevels(1), ""),
                                .Nivel2 = If(strLevels.Length > 2, strLevels(2), ""),
                                .Nivel3 = If(strLevels.Length > 3, strLevels(3), ""),
                                .Nivel4 = If(strLevels.Length > 4, strLevels(4), ""),
                                .Nivel5 = If(strLevels.Length > 5, strLevels(5), ""),
                                .Nivel6 = If(strLevels.Length > 6, strLevels(6), ""),
                                .Nivel7 = If(strLevels.Length > 7, strLevels(7), ""),
                                .Nivel8 = If(strLevels.Length > 8, strLevels(8), ""),
                                .Nivel9 = If(strLevels.Length > 9, strLevels(9), ""),
                                .Nivel10 = If(strLevels.Length > 10, strLevels(10), ""),
                                .ActiveAuthorizations = txtActiveAuthorizations.Text.Split(";"),
                                .LabAgreeName = Me.txtLabAgreeName.Text,
                                .SupervisorGroup = Me.txtSupervisorGroup.Text,
                                .UserName = txtLogin.Text
                            }
                    Else
                        Dim strJson As String = txtEmployeesJson.Text '"{'NombreEmpleado':'PARRA DIAZ, JORGE DAVID','UniqueEmployeeID':'06331','NifEmpleado':'06331','IDContract':'06331','LabAgreeName':'Conven 24 lab','StartContractDate':'2018-09-14T00:00:00','MobilityDate':'2020-11-01T00:00:00','CompositeContractType':'ContractAndDate', 'UserName':'06331','Nivel1':'52993329H','UserFields':[{'UserFieldName':'MANAGER','UserFieldValue':'09819','UserFieldValueDate':'2020-11-01'}]}"
                        oNewEmployee = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(strJson, GetType(ExternalApi2SSL.roDatalinkStandarEmployee))
                    End If

                    result = oSvc.CreateOrUpdateEmployee(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewEmployee)
                Else
                    htUserFields = New List(Of ExternalApiSSL.roDatalinkEmployeeUserFieldValue)

                    For Each oField As String In txtUserFields.Text.Split("#")
                        Dim oFieldValue As String() = oField.Split(":")
                        If oFieldValue.Length = 2 Then htUserFields.Add(New ExternalApiSSL.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = oFieldValue(0), .UserFieldValue = oFieldValue(1), .UserFieldValueDate = New Date(1970, 1, 1)})
                    Next

                    oFieldsArray = New ExternalApiSSL.ArrayOfRoDatalinkEmployeeUserFieldValue
                    oFieldsArray.AddRange(htUserFields.ToArray())

                    strAuthorizationsArray = New ExternalApiSSL.ArrayOfString
                    strAuthorizationsArray.AddRange(txtActiveAuthorizations.Text.Split(";"))

                    Dim oNewEmployee As ExternalApiSSL.roDatalinkStandarEmployee

                    If Not bEmployeeFromJSON Then
                        oNewEmployee = New ExternalApiSSL.roDatalinkStandarEmployee With {
                                    .NombreEmpleado = strName,
                                    .UniqueEmployeeID = strImportKey,
                                    .IDContract = strContractID,
                                    .NifEmpleado = strNifPunch,
                                    .StartContractDate = xBeginContract,
                                    .EndContractDate = xEndContract,
                                    .MobilityDate = IIf(ckAppliMobility.Checked, dtMobility.Value.Date, Nothing),
                                    .CompositeContractType = ExternalApiSSL.eCompositeContractType.ContractAndDate,
                                    .UserFields = oFieldsArray,
                                    .CardNumber = strCard,
                                    .Nivel0 = If(strLevels.Length > 0, strLevels(0), ""),
                                    .Nivel1 = If(strLevels.Length > 1, strLevels(1), ""),
                                    .Nivel2 = If(strLevels.Length > 2, strLevels(2), ""),
                                    .Nivel3 = If(strLevels.Length > 3, strLevels(3), ""),
                                    .Nivel4 = If(strLevels.Length > 4, strLevels(4), ""),
                                    .Nivel5 = If(strLevels.Length > 5, strLevels(5), ""),
                                    .Nivel6 = If(strLevels.Length > 6, strLevels(6), ""),
                                    .Nivel7 = If(strLevels.Length > 7, strLevels(7), ""),
                                    .Nivel8 = If(strLevels.Length > 8, strLevels(8), ""),
                                    .Nivel9 = If(strLevels.Length > 9, strLevels(9), ""),
                                    .Nivel10 = If(strLevels.Length > 10, strLevels(10), ""),
                                    .ActiveAuthorizations = strAuthorizationsArray,
                                    .LabAgreeName = Me.txtLabAgreeName.Text,
                                    .SupervisorGroup = Me.txtSupervisorGroup.Text,
                                    .UserName = txtLogin.Text
                                }
                    Else
                        Dim strJson As String = txtEmployeesJson.Text '"{'NombreEmpleado':'PARRA DIAZ, JORGE DAVID','UniqueEmployeeID':'06331','NifEmpleado':'06331','IDContract':'06331','LabAgreeName':'Conven 24 lab','StartContractDate':'2018-09-14T00:00:00','MobilityDate':'2020-11-01T00:00:00','CompositeContractType':'ContractAndDate', 'UserName':'06331','Nivel1':'52993329H','UserFields':[{'UserFieldName':'MANAGER','UserFieldValue':'09819','UserFieldValueDate':'2020-11-01'}]}"
                        oNewEmployee = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(strJson, GetType(ExternalApiSSL.roDatalinkStandarEmployee))
                    End If


                    result = oSvc.CreateOrUpdateEmployee(Me.txtUserName.Text, Me.txtPassword.Text, oNewEmployee)
                End If

            Else
                If ckMultitenantApi.Checked Then
                    htUserFields = New List(Of ExternalApi2.roDatalinkEmployeeUserFieldValue)

                    For Each oField As String In txtUserFields.Text.Split("#")
                        Dim oFieldValue As String() = oField.Split(":")
                        If oFieldValue.Length = 2 Then htUserFields.Add(New ExternalApi2.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = oFieldValue(0), .UserFieldValue = oFieldValue(1), .UserFieldValueDate = New Date(1970, 1, 1)})
                    Next
                    Dim oNewEmployee As ExternalApi2.roDatalinkStandarEmployee

                    If Not bEmployeeFromJSON Then
                        oNewEmployee = New ExternalApi2.roDatalinkStandarEmployee With {
                                .NombreEmpleado = strName,
                                .UniqueEmployeeID = strImportKey,
                                .IDContract = strContractID,
                                .NifEmpleado = strNifPunch,
                                .StartContractDate = xBeginContract,
                                .EndContractDate = xEndContract,
                                .MobilityDate = IIf(ckAppliMobility.Checked, dtMobility.Value.Date, Nothing),
                                .CompositeContractType = ExternalApi.eCompositeContractType.ContractAndDate,
                                .UserFields = htUserFields.ToArray(),
                                .CardNumber = strCard,
                                .Nivel0 = If(strLevels.Length > 0, strLevels(0), ""),
                                .Nivel1 = If(strLevels.Length > 1, strLevels(1), ""),
                                .Nivel2 = If(strLevels.Length > 2, strLevels(2), ""),
                                .Nivel3 = If(strLevels.Length > 3, strLevels(3), ""),
                                .Nivel4 = If(strLevels.Length > 4, strLevels(4), ""),
                                .Nivel5 = If(strLevels.Length > 5, strLevels(5), ""),
                                .Nivel6 = If(strLevels.Length > 6, strLevels(6), ""),
                                .Nivel7 = If(strLevels.Length > 7, strLevels(7), ""),
                                .Nivel8 = If(strLevels.Length > 8, strLevels(8), ""),
                                .Nivel9 = If(strLevels.Length > 9, strLevels(9), ""),
                                .Nivel10 = If(strLevels.Length > 10, strLevels(10), ""),
                                .ActiveAuthorizations = txtActiveAuthorizations.Text.Split(";"),
                                .LabAgreeName = Me.txtLabAgreeName.Text,
                                .SupervisorGroup = Me.txtSupervisorGroup.Text,
                                .UserName = txtLogin.Text
                            }
                    Else
                        Dim strJson As String = txtEmployeesJson.Text '"{'NombreEmpleado':'PARRA DIAZ, JORGE DAVID','UniqueEmployeeID':'06331','NifEmpleado':'06331','IDContract':'06331','LabAgreeName':'Conven 24 lab','StartContractDate':'2018-09-14T00:00:00','MobilityDate':'2020-11-01T00:00:00','CompositeContractType':'ContractAndDate', 'UserName':'06331','Nivel1':'52993329H','UserFields':[{'UserFieldName':'MANAGER','UserFieldValue':'09819','UserFieldValueDate':'2020-11-01'}]}"
                        oNewEmployee = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(strJson, GetType(ExternalApi2.roDatalinkStandarEmployee))
                    End If

                    result = oSvc.CreateOrUpdateEmployee(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewEmployee)
                Else
                    htUserFields = New List(Of ExternalApi.roDatalinkEmployeeUserFieldValue)

                    For Each oField As String In txtUserFields.Text.Split("#")
                        Dim oFieldValue As String() = oField.Split(":")
                        If oFieldValue.Length = 2 Then htUserFields.Add(New ExternalApi.roDatalinkEmployeeUserFieldValue() With {.UserFieldName = oFieldValue(0), .UserFieldValue = oFieldValue(1), .UserFieldValueDate = New Date(1970, 1, 1)})
                    Next

                    oFieldsArray = New ExternalApi.ArrayOfRoDatalinkEmployeeUserFieldValue
                    oFieldsArray.AddRange(htUserFields.ToArray())

                    strAuthorizationsArray = New ExternalApi.ArrayOfString
                    strAuthorizationsArray.AddRange(txtActiveAuthorizations.Text.Split(";"))

                    Dim oNewEmployee As ExternalApi.roDatalinkStandarEmployee

                    If Not bEmployeeFromJSON Then
                        oNewEmployee = New ExternalApi.roDatalinkStandarEmployee With {
                                .NombreEmpleado = strName,
                                .UniqueEmployeeID = strImportKey,
                                .IDContract = strContractID,
                                .NifEmpleado = strNifPunch,
                                .StartContractDate = xBeginContract,
                                .EndContractDate = xEndContract,
                                .MobilityDate = IIf(ckAppliMobility.Checked, dtMobility.Value.Date, Nothing),
                                .CompositeContractType = ExternalApi.eCompositeContractType.ContractAndDate,
                                .UserFields = oFieldsArray,
                                .CardNumber = strCard,
                                .Nivel0 = If(strLevels.Length > 0, strLevels(0), ""),
                                .Nivel1 = If(strLevels.Length > 1, strLevels(1), ""),
                                .Nivel2 = If(strLevels.Length > 2, strLevels(2), ""),
                                .Nivel3 = If(strLevels.Length > 3, strLevels(3), ""),
                                .Nivel4 = If(strLevels.Length > 4, strLevels(4), ""),
                                .Nivel5 = If(strLevels.Length > 5, strLevels(5), ""),
                                .Nivel6 = If(strLevels.Length > 6, strLevels(6), ""),
                                .Nivel7 = If(strLevels.Length > 7, strLevels(7), ""),
                                .Nivel8 = If(strLevels.Length > 8, strLevels(8), ""),
                                .Nivel9 = If(strLevels.Length > 9, strLevels(9), ""),
                                .Nivel10 = If(strLevels.Length > 10, strLevels(10), ""),
                                .ActiveAuthorizations = strAuthorizationsArray,
                                .LabAgreeName = Me.txtLabAgreeName.Text,
                                .SupervisorGroup = Me.txtSupervisorGroup.Text,
                                .UserName = txtLogin.Text
                        }
                    Else
                        Dim strJson As String = txtEmployeesJson.Text '"{'NombreEmpleado':'PARRA DIAZ, JORGE DAVID','UniqueEmployeeID':'06331','NifEmpleado':'06331','IDContract':'06331','LabAgreeName':'Conven 24 lab','StartContractDate':'2018-09-14T00:00:00','MobilityDate':'2020-11-01T00:00:00','CompositeContractType':'ContractAndDate', 'UserName':'06331','Nivel1':'52993329H','UserFields':[{'UserFieldName':'MANAGER','UserFieldValue':'09819','UserFieldValueDate':'2020-11-01'}]}"
                        oNewEmployee = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(strJson, GetType(ExternalApi.roDatalinkStandarEmployee))

                        If bSAGE200c Then
                            Dim strContractsJSON As String = txtContractsJSON.Text
                            Dim oContracts As ExternalApi.roDatalinkEmployeeContractsHistory
                            oContracts = Robotics.VTBase.roJSONHelper.DeserializeNewtonSoft(strContractsJSON, GetType(ExternalApi.roDatalinkEmployeeContractsHistory))

                            result = oSvc.CreateOrUpdateEmployeeSAGE200c(Me.txtUserName.Text, Me.txtPassword.Text, oContracts, oNewEmployee)
                        End If
                    End If

                    If Not bSAGE200c Then
                        result = oSvc.CreateOrUpdateEmployee(Me.txtUserName.Text, Me.txtPassword.Text, oNewEmployee)
                    End If

                End If

            End If

        Catch ex As Exception
            result = -1
        End Try


        txtResult.Invoke(Sub()
                             If [Enum].IsDefined(GetType(Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode), CInt(result)) Then
                                 txtResult.Text = "ResultCode -> " & result.ToString & " " & [Enum].Parse(GetType(Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode), result.ToString).ToString
                             Else
                                 txtResult.Text = "ResultCode -> " & result.ToString
                             End If

                         End Sub)

        Return result
    End Function

    Private Sub btnCreateAbsence_Click(sender As Object, e As EventArgs) Handles btnCreateAbsence.Click
        Dim result As Integer = -1

        Try
            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If

            Dim strAction As String = "I"

            If cmbAbsenceAction.SelectedIndex = 0 Then
                strAction = "I"
            ElseIf cmbAbsenceAction.SelectedIndex = 1 Then
                strAction = "U"
            ElseIf cmbAbsenceAction.SelectedIndex = 2 Then
                strAction = "D"
            End If

            Dim iDuration As Long = txtDuration.Value.Hour + txtDuration.Value.Minute


            Dim xEndContract As Nullable(Of Date) = If(rbEndDate.Checked, dtAbsenceEnd.Value.Date, Nothing)
            Dim xAbsenceStartHour As Nullable(Of Date) = If(iDuration = 0, Nothing, dtAbsenceStartHour.Value)
            Dim xAbsenceEndHour As Nullable(Of Date) = If(iDuration = 0, Nothing, dtAbsenceEndHour.Value)
            Dim xDuration As Nullable(Of Date) = If(iDuration = 0, Nothing, txtDuration.Value)
            Dim iMaxDays As Integer = If(rbLastingDays.Checked, CInt(dtMaxLastingDays.Text), 0)

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oNewabsence As New ExternalApi2SSL.roDatalinkStandarAbsence With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .CauseShortName = txtCauseShortName.Text,
                            .NifLetter = "",
                            .StartAbsenceDate = dtAbsenceStart.Value.Date,
                            .EndAbsenceDate = xEndContract,
                            .MaxDays = iMaxDays,
                            .BeginHour = xAbsenceStartHour,
                            .EndHour = xAbsenceEndHour,
                            .Duration = xDuration
                        }
                    result = oSvc.CreateOrUpdateAbsence(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewabsence)
                Else
                    Dim oNewabsence As New ExternalApiSSL.roDatalinkStandarAbsence With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .CauseShortName = txtCauseShortName.Text,
                            .NifLetter = "",
                            .StartAbsenceDate = dtAbsenceStart.Value.Date,
                            .EndAbsenceDate = xEndContract,
                            .MaxDays = iMaxDays,
                            .BeginHour = xAbsenceStartHour,
                            .EndHour = xAbsenceEndHour,
                            .Duration = xDuration
                        }
                    result = oSvc.CreateOrUpdateAbsence(Me.txtUserName.Text, Me.txtPassword.Text, oNewabsence)
                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim oNewabsence As New ExternalApi2.roDatalinkStandarAbsence With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .CauseShortName = txtCauseShortName.Text,
                            .NifLetter = "",
                            .StartAbsenceDate = dtAbsenceStart.Value.Date,
                            .EndAbsenceDate = xEndContract,
                            .MaxDays = iMaxDays,
                            .BeginHour = xAbsenceStartHour,
                            .EndHour = xAbsenceEndHour,
                            .Duration = xDuration
                        }
                    result = oSvc.CreateOrUpdateAbsence(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewabsence)
                Else
                    Dim oNewabsence As New ExternalApi.roDatalinkStandarAbsence With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .CauseShortName = txtCauseShortName.Text,
                            .NifLetter = "",
                            .StartAbsenceDate = dtAbsenceStart.Value.Date,
                            .EndAbsenceDate = xEndContract,
                            .MaxDays = iMaxDays,
                            .BeginHour = xAbsenceStartHour,
                            .EndHour = xAbsenceEndHour,
                            .Duration = xDuration
                        }
                    result = oSvc.CreateOrUpdateAbsence(Me.txtUserName.Text, Me.txtPassword.Text, oNewabsence)
                End If


            End If

        Catch ex As Exception
            result = -1
        End Try

        txtResult.Text = result.ToString
    End Sub

    Private Sub btnCreateHolidays_Click(sender As Object, e As EventArgs) Handles btnCreateHolidays.Click
        Dim result As Integer = -1

        Try
            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If


            Dim strAction As String = "I"

            If cmbAbsenceAction.SelectedIndex = 0 Then
                strAction = "I"
            ElseIf cmbAbsenceAction.SelectedIndex = 1 Then
                strAction = "U"
            ElseIf cmbAbsenceAction.SelectedIndex = 2 Then
                strAction = "D"
            End If

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oNewHolidays As New ExternalApi2SSL.roDatalinkStandarHolidays With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .ShiftKey = txtCauseShortName.Text,
                            .NifLetter = "",
                            .PlanDate = dtAbsenceStart.Value.Date
                        }
                    result = oSvc.CreateOrUpdateHolidays(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewHolidays)
                Else
                    Dim oNewHolidays As New ExternalApiSSL.roDatalinkStandarHolidays With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .ShiftKey = txtCauseShortName.Text,
                            .NifLetter = "",
                            .PlanDate = dtAbsenceStart.Value.Date
                        }
                    result = oSvc.CreateOrUpdateHolidays(Me.txtUserName.Text, Me.txtPassword.Text, oNewHolidays)
                End If

            Else
                If ckMultitenantApi.Checked Then
                    Dim oNewHolidays As New ExternalApi2.roDatalinkStandarHolidays With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .ShiftKey = txtCauseShortName.Text,
                            .NifLetter = "",
                            .PlanDate = dtAbsenceStart.Value.Date
                        }
                    result = oSvc.CreateOrUpdateHolidays(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewHolidays)
                Else
                    Dim oNewHolidays As New ExternalApi.roDatalinkStandarHolidays With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .Action = strAction,
                            .ShiftKey = txtCauseShortName.Text,
                            .NifLetter = "",
                            .PlanDate = dtAbsenceStart.Value.Date
                        }
                    result = oSvc.CreateOrUpdateHolidays(Me.txtUserName.Text, Me.txtPassword.Text, oNewHolidays)
                End If



            End If

        Catch ex As Exception
            result = -1
        End Try

        txtResult.Text = result.ToString
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim result As Integer = -1

        Try
            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If

            Dim strAction As String = "I"


            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oNewCalendar As New ExternalApi2SSL.roDatalinkStandarCalendar With {
                            .UniqueEmployeeID = txtCalendarKey.Text,
                            .NifEmpleado = txtCalendarNIF.Text,
                            .ShiftKey = txtCalendarShift.Text,
                            .NifLetter = "",
                            .PlanDate = txtCalendarDate.Value.Date
                        }
                    result = oSvc.CreateOrUpdateCalendar(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                Else
                    Dim oNewCalendar As New ExternalApiSSL.roDatalinkStandarCalendar With {
                            .UniqueEmployeeID = txtCalendarKey.Text,
                            .NifEmpleado = txtCalendarNIF.Text,
                            .ShiftKey = txtCalendarShift.Text,
                            .NifLetter = "",
                            .PlanDate = txtCalendarDate.Value.Date
                        }
                    result = oSvc.CreateOrUpdateCalendar(Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim oNewCalendar As New ExternalApi2.roDatalinkStandarCalendar With {
                            .UniqueEmployeeID = txtCalendarKey.Text,
                            .NifEmpleado = txtCalendarNIF.Text,
                            .ShiftKey = txtCalendarShift.Text,
                            .NifLetter = "",
                            .PlanDate = txtCalendarDate.Value.Date
                        }
                    result = oSvc.CreateOrUpdateCalendar(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                Else
                    Dim oNewCalendar As New ExternalApi.roDatalinkStandarCalendar With {
                            .UniqueEmployeeID = txtCalendarKey.Text,
                            .NifEmpleado = txtCalendarNIF.Text,
                            .ShiftKey = txtCalendarShift.Text,
                            .NifLetter = "",
                            .PlanDate = txtCalendarDate.Value.Date
                        }
                    result = oSvc.CreateOrUpdateCalendar(Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                End If


            End If


        Catch ex As Exception
            result = -1
        End Try

        txtResult.Text = result.ToString
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim intResult As Integer = -1
        Try
            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If


            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oNewResponse As New ExternalApi2SSL.roDatalinkStandarAbsenceResponse
                    Dim oNewCalendar As New ExternalApi2SSL.roDatalinkStandarAbsenceCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAbsencePeriod = dtAbsenceStart.Value.Date,
                            .EndAbsencePeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }
                    oNewResponse = oSvc.GetAbsences(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                    intResult = oNewResponse.ResultCode
                Else
                    Dim oNewResponse As New ExternalApiSSL.roDatalinkStandarAbsenceResponse
                    Dim oNewCalendar As New ExternalApiSSL.roDatalinkStandarAbsenceCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAbsencePeriod = dtAbsenceStart.Value.Date,
                            .EndAbsencePeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }
                    oNewResponse = oSvc.GetAbsences(Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                    intResult = oNewResponse.ResultCode
                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim oNewResponse As New ExternalApi2.roDatalinkStandarAbsenceResponse
                    Dim oNewCalendar As New ExternalApi2.roDatalinkStandarAbsenceCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAbsencePeriod = dtAbsenceStart.Value.Date,
                            .EndAbsencePeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }
                    oNewResponse = oSvc.GetAbsences(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar, oNewResponse)
                    intResult = oNewResponse.ResultCode
                Else
                    Dim oNewResponse As New ExternalApi.roDatalinkStandarAbsenceResponse
                    Dim oNewCalendar As New ExternalApi.roDatalinkStandarAbsenceCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAbsencePeriod = dtAbsenceStart.Value.Date,
                            .EndAbsencePeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }


                    oNewCalendar.OnlyChanges = chkOnlyChanges.Checked

                    oNewResponse = oSvc.GetAbsences(Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                    intResult = oNewResponse.ResultCode
                End If


            End If


        Catch ex As Exception
            intResult = -1
        End Try

        txtResult.Text = intResult.ToString

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim intResult As Integer = -1
        Try
            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oNewResponse As New ExternalApi2SSL.roDatalinkStandarAccrualResponse
                    Dim oNewCalendar As New ExternalApi2SSL.roDatalinkStandarAccrualCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAccrualPeriod = dtAbsenceStart.Value.Date,
                            .EndAccrualPeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }
                    oNewResponse = oSvc.GetAccruals(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                    intResult = oNewResponse.ResultCode
                Else
                    Dim oNewResponse As New ExternalApiSSL.roDatalinkStandarAccrualResponse
                    Dim oNewCalendar As New ExternalApiSSL.roDatalinkStandarAccrualCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAccrualPeriod = dtAbsenceStart.Value.Date,
                            .EndAccrualPeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }
                    oNewResponse = oSvc.GetAccruals(Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                    intResult = oNewResponse.ResultCode
                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim oNewResponse As New ExternalApi2.roDatalinkStandarAccrualResponse
                    Dim oNewCalendar As New ExternalApi2.roDatalinkStandarAccrualCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAccrualPeriod = dtAbsenceStart.Value.Date,
                            .EndAccrualPeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }
                    oNewResponse = oSvc.GetAccruals(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar, oNewResponse)
                    intResult = oNewResponse.ResultCode
                Else
                    Dim oNewResponse As New ExternalApi.roDatalinkStandarAccrualResponse
                    Dim oNewCalendar As New ExternalApi.roDatalinkStandarAccrualCriteria With {
                            .UniqueEmployeeID = txtAbsenceImportkey.Text,
                            .NifEmpleado = txtAbsenceNif.Text,
                            .StartAccrualPeriod = dtAbsenceStart.Value.Date,
                            .EndAccrualPeriod = dtAbsenceEnd.Value.Date,
                            .NifLetter = ""
                        }
                    oNewResponse = oSvc.GetAccruals(Me.txtUserName.Text, Me.txtPassword.Text, oNewCalendar)
                    intResult = oNewResponse.ResultCode
                End If


            End If


        Catch ex As Exception
            intResult = -1
        End Try

        txtResult.Text = intResult.ToString

    End Sub

    Private Sub btnLoadDocument_Click(sender As Object, e As EventArgs) Handles btnLoadDocument.Click
        OpenFileDialog1.ShowDialog()
        strDocumentPath = OpenFileDialog1.FileName
        If strDocumentPath = "OpenFileDialog1" Then strDocumentPath = "..."
        lblDocumentPath.Text = strDocumentPath

        If strDocumentPath.Length > 0 AndAlso strDocumentPath <> "..." Then
            strDocumentData = Convert.ToBase64String(System.IO.File.ReadAllBytes(strDocumentPath))
            If chkDocumentbase64.Checked Then
                txtDocumentData.Text = strDocumentData
            End If
        End If

    End Sub

    Private Sub btnCreateDocument_Click(sender As Object, e As EventArgs) Handles btnCreateDocument.Click
        Dim iReturn As Integer = 0
        Dim sReturn As String = String.Empty

        Try
            txtResult.Text = ""
            If strDocumentPath.Length = 0 OrElse strDocumentPath = "..." Then
                MsgBox("Debes cargar un documento")
                Exit Sub
            End If

            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If



            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oDocument As New ExternalApi2SSL.roDatalinkStandardDocument
                    Dim result As ExternalApi2SSL.roDatalinkStandarDocumentResponse

                    oDocument.NifEmpleado = txtDocNIF.Text
                    oDocument.UniqueEmployeeID = txtDocImportKey.Text
                    oDocument.DocumentExtension = txtDocExtension.Text
                    oDocument.DocumentRemarks = txtDocRemarks.Text
                    oDocument.DocumentTitle = txtDocumentTitle.Text
                    oDocument.DocumentType = txtDocTemplatename.Text

                    If chkDocumentbase64.Checked Then
                        oDocument.DocumentData = txtDocumentData.Text
                    Else
                        oDocument.DocumentData = strDocumentData
                    End If

                    result = oSvc.CreateOrUpdateDocument(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oDocument)
                    iReturn = result.ResultCode
                    sReturn = result.ResultDetails
                Else
                    Dim oDocument As New ExternalApiSSL.roDatalinkStandardDocument
                    Dim result As ExternalApiSSL.roDatalinkStandarDocumentResponse

                    oDocument.NifEmpleado = txtDocNIF.Text
                    oDocument.UniqueEmployeeID = txtDocImportKey.Text
                    oDocument.DocumentExtension = txtDocExtension.Text
                    oDocument.DocumentRemarks = txtDocRemarks.Text
                    oDocument.DocumentTitle = txtDocumentTitle.Text
                    oDocument.DocumentType = txtDocTemplatename.Text

                    If chkDocumentbase64.Checked Then
                        oDocument.DocumentData = txtDocumentData.Text
                    Else
                        oDocument.DocumentData = strDocumentData
                    End If

                    result = oSvc.CreateOrUpdateDocument(Me.txtUserName.Text, Me.txtPassword.Text, oDocument)
                    iReturn = result.ResultCode
                    sReturn = result.ResultDetails
                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim oDocument As New ExternalApi2.roDatalinkStandardDocument
                    Dim result As ExternalApi2.roDatalinkStandarDocumentResponse

                    oDocument.NifEmpleado = txtDocNIF.Text
                    oDocument.UniqueEmployeeID = txtDocImportKey.Text
                    oDocument.DocumentExtension = txtDocExtension.Text
                    oDocument.DocumentRemarks = txtDocRemarks.Text
                    oDocument.DocumentTitle = txtDocumentTitle.Text
                    oDocument.DocumentType = txtDocTemplatename.Text

                    If chkDocumentbase64.Checked Then
                        oDocument.DocumentData = txtDocumentData.Text
                    Else
                        oDocument.DocumentData = strDocumentData
                    End If

                    result = oSvc.CreateOrUpdateDocument(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oDocument)
                    iReturn = result.ResultCode
                    sReturn = result.ResultDetails
                Else
                    Dim oDocument As New ExternalApi.roDatalinkStandardDocument
                    Dim result As ExternalApi.roDatalinkStandarDocumentResponse

                    oDocument.NifEmpleado = txtDocNIF.Text
                    oDocument.UniqueEmployeeID = txtDocImportKey.Text
                    oDocument.DocumentExtension = txtDocExtension.Text
                    oDocument.DocumentRemarks = txtDocRemarks.Text
                    oDocument.DocumentTitle = txtDocumentTitle.Text
                    oDocument.DocumentType = txtDocTemplatename.Text

                    If chkDocumentbase64.Checked Then
                        oDocument.DocumentData = txtDocumentData.Text
                    Else
                        oDocument.DocumentData = strDocumentData
                    End If

                    result = oSvc.CreateOrUpdateDocument(Me.txtUserName.Text, Me.txtPassword.Text, oDocument)
                    iReturn = result.ResultCode
                    sReturn = result.ResultDetails
                End If


            End If

        Catch ex As Exception
            iReturn = -1
            sReturn = "Excepción: " + ex.Message
        End Try

        txtResult.Text = iReturn.ToString + " - " + sReturn
    End Sub

    Private Sub ckMultitenantApi_CheckedChanged(sender As Object, e As EventArgs) Handles ckMultitenantApi.CheckedChanged
        If ckMultitenantApi.Checked Then
            Me.txtCompanyClientCode.Enabled = True
        Else
            Me.txtCompanyClientCode.Enabled = False
        End If
    End Sub

    Private Sub btnGetPunches_Click(sender As Object, e As EventArgs) Handles btnGetPunches.Click
        Dim iReturn As Integer = 0
        Dim sReturn As String = String.Empty

        Try
            txtResult.Text = ""

            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oPunch As New ExternalApi2SSL.roDatalinkStandardPunch
                    Dim result As ExternalApi2SSL.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunches(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, TimeStamp.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode
                Else

                    Dim oPunch As New ExternalApiSSL.roDatalinkStandardPunch
                    Dim result As ExternalApiSSL.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunches(Me.txtUserName.Text, Me.txtPassword.Text, TimeStamp.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode

                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim oPunch As New ExternalApi2.roDatalinkStandardPunch
                    Dim result As ExternalApi2.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunches(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, TimeStamp.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode
                Else

                    Dim oPunch As New ExternalApi.roDatalinkStandardPunch
                    Dim result As ExternalApi.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunches(Me.txtUserName.Text, Me.txtPassword.Text, TimeStamp.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode
                End If
            End If

        Catch ex As Exception
            iReturn = -1
            sReturn = "Excepción: " + ex.Message
        End Try


        txtResult.Text = iReturn.ToString
    End Sub

    Private Sub btnGetPunchesBetweenDates_Click(sender As Object, e As EventArgs) Handles btnGetPunchesBetweenDates.Click
        Dim iReturn As Integer = 0
        Dim sReturn As String = String.Empty

        Try
            txtResult.Text = ""

            Dim oSvc = Nothing

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim oPunch As New ExternalApi2SSL.roDatalinkStandardPunch
                    Dim result As ExternalApi2SSL.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunchesBetweenDates(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, roStartdate.Value, roEnddate.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode
                Else

                    Dim oPunch As New ExternalApiSSL.roDatalinkStandardPunch
                    Dim result As ExternalApiSSL.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunchesBetweenDates(Me.txtUserName.Text, Me.txtPassword.Text, roStartdate.Value, roEnddate.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode

                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim oPunch As New ExternalApi2.roDatalinkStandardPunch
                    Dim result As ExternalApi2.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunchesBetweenDates(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, roStartdate.Value, roEnddate.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode
                Else

                    Dim oPunch As New ExternalApi.roDatalinkStandardPunch
                    Dim result As ExternalApi.roDatalinkStandardPunchResponse
                    result = oSvc.GetPunchesBetweenDates(Me.txtUserName.Text, Me.txtPassword.Text, roStartdate.Value, roEnddate.Value)

                    For Each x In result.Punches
                        sReturn += x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode

                End If




            End If

        Catch ex As Exception
            iReturn = -1
            sReturn = "Excepción: " + ex.Message
        End Try


        txtResult.Text = iReturn.ToString
    End Sub

    Private Sub btnAddPunches_Click(sender As Object, e As EventArgs) Handles btnAddPunches.Click
        Dim iReturn As Integer = 0
        Dim sReturn As String = String.Empty

        Try
            txtResult.Text = ""

            Dim oSvc = Nothing
            'Dim oSvc = New ExternalApi2SSL.ExternalApiClient()

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2SSL.ExternalApiClient()
                Else
                    oSvc = New ExternalApiSSL.ExternalApiSoapClient()
                End If
            Else
                If ckMultitenantApi.Checked Then
                    oSvc = New ExternalApi2.ExternalApiClient()
                Else
                    oSvc = New ExternalApi.ExternalApiSoapClient()
                End If
            End If

            If ckMultitenantApi.Checked Then
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text & "/soap")
            Else
                oSvc.Endpoint.Address = New EndpointAddress(Me.txtWSUrl.Text)
            End If

            If ckSSL.Checked Then
                If ckMultitenantApi.Checked Then
                    Dim result As ExternalApi2SSL.roDatalinkStandardPunchResponse
                    Dim oPunchList As New Generic.List(Of ExternalApi2SSL.roDatalinkStandardPunch)
                    Dim oPunch As New ExternalApi2SSL.roDatalinkStandardPunch

                    oPunch.UniqueEmployeeID = txtKey.Text
                    oPunch.NifEmpleado = txtNIF.Text
                    oPunch.Type = IIf(roIN.Checked, ExternalApi.PunchTypeEnum._IN, ExternalApi.PunchTypeEnum._OUT)
                    oPunch.TypeData = txtCause.Text
                    oPunch.DateTime = roPunchTime.Value
                    If IsNumeric(txtTerminal.Text) Then
                        oPunch.IDTerminal = CInt(txtTerminal.Text)
                    End If


                    oPunchList.Add(oPunch)

                    result = oSvc.AddPunches(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oPunchList.ToArray)

                    For Each x In result.PunchesListError
                        sReturn += x.ResultCode & ":" & x.ResultDescription & ";" & x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next

                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode
                Else

                    Dim result As ExternalApiSSL.roDatalinkStandardPunchResponse
                    Dim oPunchList As New Generic.List(Of ExternalApiSSL.roDatalinkStandardPunch)
                    Dim oPunch As New ExternalApiSSL.roDatalinkStandardPunch

                    oPunch.UniqueEmployeeID = txtKey.Text
                    oPunch.NifEmpleado = txtNIF.Text
                    oPunch.Type = IIf(roIN.Checked, ExternalApi.PunchTypeEnum._IN, ExternalApi.PunchTypeEnum._OUT)
                    oPunch.TypeData = txtCause.Text
                    oPunch.DateTime = roPunchTime.Value
                    If IsNumeric(txtTerminal.Text) Then
                        oPunch.IDTerminal = CInt(txtTerminal.Text)
                    End If


                    oPunchList.Add(oPunch)

                    Dim oPunches = New ExternalApiSSL.ArrayOfRoDatalinkStandardPunch
                    oPunches.AddRange(oPunchList.ToArray())

                    result = oSvc.AddPunches(Me.txtUserName.Text, Me.txtPassword.Text, oPunches)

                    For Each x In result.PunchesListError
                        sReturn += x.ResultCode & ":" & x.ResultDescription & ";" & x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next


                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode

                End If


            Else
                If ckMultitenantApi.Checked Then
                    Dim result As ExternalApi2.roDatalinkStandardPunchResponse
                    Dim oPunchList As New Generic.List(Of ExternalApi2.roDatalinkStandardPunch)
                    Dim oPunch As New ExternalApi2.roDatalinkStandardPunch

                    oPunch.UniqueEmployeeID = txtKey.Text
                    oPunch.NifEmpleado = txtNIF.Text
                    oPunch.Type = IIf(roIN.Checked, ExternalApi.PunchTypeEnum._IN, ExternalApi.PunchTypeEnum._OUT)
                    oPunch.TypeData = txtCause.Text
                    oPunch.DateTime = roPunchTime.Value
                    If IsNumeric(txtTerminal.Text) Then
                        oPunch.IDTerminal = CInt(txtTerminal.Text)
                    End If

                    oPunchList.Add(oPunch)

                    result = oSvc.AddPunches(Me.txtCompanyClientCode.Text, Me.txtUserName.Text, Me.txtPassword.Text, oPunchList.ToArray)

                    For Each x In result.PunchesListError
                        sReturn += x.ResultCode & ":" & x.ResultDescription & ";" & x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next


                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode
                Else
                    Dim result As ExternalApi.roDatalinkStandardPunchResponse
                    Dim oPunchList As New Generic.List(Of ExternalApi.roDatalinkStandardPunch)
                    Dim oPunch As New ExternalApi.roDatalinkStandardPunch

                    oPunch.UniqueEmployeeID = txtKey.Text
                    oPunch.NifEmpleado = txtNIF.Text
                    oPunch.Type = IIf(roIN.Checked, ExternalApi.PunchTypeEnum._IN, ExternalApi.PunchTypeEnum._OUT)
                    oPunch.TypeData = txtCause.Text
                    oPunch.DateTime = roPunchTime.Value
                    If IsNumeric(txtTerminal.Text) Then
                        oPunch.IDTerminal = CInt(txtTerminal.Text)
                    End If


                    oPunchList.Add(oPunch)

                    Dim oPunches = New ExternalApi.ArrayOfRoDatalinkStandardPunch
                    oPunches.AddRange(oPunchList.ToArray())

                    result = oSvc.AddPunches(Me.txtUserName.Text, Me.txtPassword.Text, oPunches)

                    For Each x In result.PunchesListError
                        sReturn += x.ResultCode & ":" & x.ResultDescription & ";" & x.NifEmpleado & ";" & x.UniqueEmployeeID & ";" & x.DateTime & ";" & x.IDTerminal & ";" & x.Type.ToString & ";" & x.TypeData & vbNewLine
                    Next



                    txtPunches.Text = sReturn
                    iReturn = result.ResultCode

                End If
            End If

        Catch ex As Exception
            iReturn = -1
            sReturn = "Excepción: " + ex.Message
        End Try


        txtResult.Text = iReturn.ToString
    End Sub

End Class
