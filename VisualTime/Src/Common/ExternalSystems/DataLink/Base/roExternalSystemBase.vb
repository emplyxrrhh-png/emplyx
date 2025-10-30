Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace DataLink


    Public MustInherit Class roExternalSystemBase


        Private oState As roDataLinkState
        Protected ReadOnly strImportPrimaryKeyUserField As String = ""
        Protected ReadOnly strPNResponsableIDUserField As String = ""
        Protected ReadOnly bolNIFIsImportSecondaryKey As Boolean = False
        Protected ReadOnly bolMobilityDateRequired As Boolean = True
        Protected ReadOnly strSystemUserIdField As String = ""
        Protected ReadOnly mCustomizationCode As String = String.Empty
        Protected ReadOnly mAbsencesOperationCodes As String = String.Empty
        Protected ReadOnly mAbsencesIgnoreColumnChar As String = String.Empty


        Public ReadOnly Property State As roDataLinkState
            Get
                Return oState
            End Get
        End Property


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            Me.oState = IIf(state Is Nothing, New roDataLinkState(-1), state)

            Try
                Me.strImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If Me.strImportPrimaryKeyUserField = String.Empty Then Me.strImportPrimaryKeyUserField = "NIF"
                If Me.strImportPrimaryKeyUserField <> "NIF" Then
                    bolNIFIsImportSecondaryKey = (New AdvancedParameter.roAdvancedParameter("ImportSecondaryKeyIsNIF", New AdvancedParameter.roAdvancedParameterState).Value = "1")
                End If
                Me.strPNResponsableIDUserField = "ID_HR"
                Me.bolMobilityDateRequired = (New AdvancedParameter.roAdvancedParameter("Import.Employees.MobilityDateRequired", New AdvancedParameter.roAdvancedParameterState).Value <> "0")

                Me.strSystemUserIdField = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar("@SELECT# FieldName from sysrouserfields where Alias='sysroVisualtimeID'"))

                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState)
                mCustomizationCode = roTypes.Any2String(oAdvParam.Value)

                oAdvParam = New AdvancedParameter.roAdvancedParameter("VTLive.Datalink.AbsencesImport.OperationCodes", New AdvancedParameter.roAdvancedParameterState)
                mAbsencesOperationCodes = roTypes.Any2String(oAdvParam.Value)
                If mAbsencesOperationCodes.Length <> 3 Then mAbsencesOperationCodes = "IUD"

                oAdvParam = New AdvancedParameter.roAdvancedParameter("VTLive.Datalink.AbsencesImport.IgnoreColumnChar", New AdvancedParameter.roAdvancedParameterState)
                mAbsencesIgnoreColumnChar = roTypes.Any2String(oAdvParam.Value)
                If mAbsencesIgnoreColumnChar.Length <> 1 Then mAbsencesIgnoreColumnChar = String.Empty
            Catch ex As Exception
                Me.strImportPrimaryKeyUserField = "NIF"
            End Try
        End Sub

        Public Function isEmployeeNew(ByVal ColumnsVal As String(), ByVal iKeyPositionVal As Integer, ByVal iNifPositionVal As Integer, ByRef oEmployeeState As UserFields.roUserFieldState) As Integer
            Return isEmployeeNew(ColumnsVal(iKeyPositionVal), ColumnsVal(iNifPositionVal), oEmployeeState)
        End Function

        Public Function isEmployeeNew(ByVal strPrimaryKeyValue As String, ByVal strNifValue As String, ByRef oEmployeeState As UserFields.roUserFieldState) As Integer
            Dim bCheckNkif As Boolean = True
            Dim tbEmployee As DataTable = Nothing
            Dim iIdEmployee As Integer = -1

            If Me.strImportPrimaryKeyUserField = Me.strSystemUserIdField Then
                If roTypes.Any2Integer(strPrimaryKeyValue) = 0 Then
                    iIdEmployee = -1
                Else
                    tbEmployee = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(Me.strImportPrimaryKeyUserField, strPrimaryKeyValue, Now, oEmployeeState)

                    If tbEmployee.Rows.Count > 0 Then
                        iIdEmployee = tbEmployee.Rows(0).Item("idemployee")
                    Else
                        iIdEmployee = 99999999
                    End If
                End If
            Else
                If strPrimaryKeyValue <> String.Empty Then
                    tbEmployee = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue(Me.strImportPrimaryKeyUserField, strPrimaryKeyValue, Now, oEmployeeState)

                    If tbEmployee.Rows.Count > 0 OrElse Not bolNIFIsImportSecondaryKey Then bCheckNkif = False
                End If

                If bCheckNkif AndAlso strNifValue <> String.Empty Then
                    tbEmployee = UserFields.roEmployeeUserField.GetIDEmployeesFromUserFieldValue("NIF", strNifValue, Now, oEmployeeState)
                End If

                If tbEmployee IsNot Nothing AndAlso tbEmployee.Rows.Count > 0 Then iIdEmployee = tbEmployee.Rows(0).Item("idemployee")
            End If

            Return iIdEmployee

        End Function

        Protected Friend Function createMandatoryFields(ByRef oEmployeeState As Employee.roEmployeeState, ByRef oUserFIeldState As UserFields.roUserFieldState, ByVal bIsSageMurano As Boolean) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oUserField As New UserFields.roUserField(oUserFIeldState.IDPassport) ', "NIF", Types.EmployeeField, oTrans, False)
                oUserField.Type = Types.EmployeeField
                oUserField.FieldName = "NIF"

                If Not oUserField.Load(False, False) Then
                    oUserField.FieldName = "NIF"
                    oUserField.Type = Types.EmployeeField
                    oUserField.FieldType = FieldTypes.tText
                    oUserField.Used = True
                    oUserField.History = False

                    If Not oUserField.Save(False) Then
                        bolRet = False
                        roBusinessState.CopyTo(oUserFIeldState, Me.State)
                    Else
                        bolRet = True
                    End If
                Else
                    bolRet = True
                    If Not oUserField.Used Then
                        oUserField.Used = True
                        If Not oUserField.Save(False) Then
                            bolRet = False
                            roBusinessState.CopyTo(oUserFIeldState, Me.State)
                        End If
                    End If
                End If

                Dim oPkField As New UserFields.roUserField(oUserFIeldState.IDPassport) ', Me.strImportPrimaryKeyUserField, Types.EmployeeField, oTrans, False)
                oPkField.Type = Types.EmployeeField
                oPkField.FieldName = Me.strImportPrimaryKeyUserField

                If Not oPkField.Load(False, False) Then
                    oPkField.FieldName = Me.strImportPrimaryKeyUserField
                    oPkField.Type = Types.EmployeeField
                    oPkField.FieldType = FieldTypes.tText
                    oPkField.Used = True
                    oPkField.History = False

                    If Not oPkField.Save(False) Then
                        bolRet = False
                        roBusinessState.CopyTo(oUserFIeldState, Me.State)
                    Else
                        bolRet = True
                    End If
                Else
                    bolRet = True
                    If oPkField.Used = False Or oPkField.History = False Then
                        oPkField.Used = True
                        oPkField.History = False
                        If Not oPkField.Save(False) Then
                            bolRet = False
                            roBusinessState.CopyTo(oUserFIeldState, Me.State)
                        End If
                    End If
                End If

                If bIsSageMurano Then
                    oUserField = New UserFields.roUserField(oUserFIeldState.IDPassport) ', "CODIGO_EMPLEADO", Types.EmployeeField, oTrans, False)
                    oUserField.Type = Types.EmployeeField
                    oUserField.FieldName = "CODIGO_EMPLEADO"

                    If Not oUserField.Load(False, False) Then
                        oUserField.FieldName = "CODIGO_EMPLEADO"
                        oUserField.Type = Types.EmployeeField
                        oUserField.FieldType = FieldTypes.tText
                        oUserField.Used = True
                        oUserField.History = False

                        If Not oUserField.Save(False) Then
                            bolRet = False
                            roBusinessState.CopyTo(oUserFIeldState, Me.State)
                        End If
                    Else
                        bolRet = True
                        oUserField.Used = True
                        If Not oUserField.Save(False) Then
                            bolRet = False
                            roBusinessState.CopyTo(oUserFIeldState, Me.State)
                        End If
                    End If

                    oUserField = New UserFields.roUserField(oUserFIeldState.IDPassport) ', "CODIGO_EMPRESA", Types.EmployeeField, oTrans, False)
                    oUserField.Type = Types.EmployeeField
                    oUserField.FieldName = "CODIGO_EMPRESA"

                    If Not oUserField.Load(False, False) Then
                        oUserField.FieldName = "CODIGO_EMPRESA"
                        oUserField.Type = Types.EmployeeField
                        oUserField.FieldType = FieldTypes.tText
                        oUserField.Used = True
                        oUserField.History = False

                        If Not oUserField.Save(False) Then
                            bolRet = False
                            roBusinessState.CopyTo(oUserFIeldState, Me.State)
                        End If
                    Else
                        bolRet = True
                        oUserField.Used = True
                        If Not oUserField.Save(False) Then
                            bolRet = False
                            roBusinessState.CopyTo(oUserFIeldState, Me.State)
                        End If
                    End If
                End If
            Catch ex As Exception
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Protected Friend Function createNIFIfNecessary(ByVal nifValue As String, ByVal iIdEmployee As Integer, ByRef oEmployeeState As Employee.roEmployeeState, ByRef oUserFIeldState As UserFields.roUserFieldState, Optional tbUserFieldList As DataTable = Nothing) As Boolean
            Dim bolRet As Boolean = True
            Dim DateField As Date = New Date(1900, 1, 1)

            Dim bolUsedInProcess As Boolean = False
            If tbUserFieldList IsNot Nothing Then
                Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='NIF'")
                If oFields.Length > 0 Then
                    If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                        bolUsedInProcess = True
                    End If

                    If roTypes.Any2Boolean(oFields(0).Item("History")) = True Then
                        DateField = Now.Date
                    End If

                End If
            End If

            Dim oEmployeeUserField As New UserFields.roEmployeeUserField(iIdEmployee, "NIF", DateField, oUserFIeldState)
            If oEmployeeUserField.FieldValue <> nifValue Then
                oEmployeeUserField.FieldValue = nifValue
                If tbUserFieldList IsNot Nothing Then
                    bolRet = oEmployeeUserField.Save(, , bolUsedInProcess)
                Else
                    bolRet = oEmployeeUserField.Save()
                End If
            End If

            Return bolRet

        End Function

        Protected Friend Function CreateImportPrimaryKeyIfNecessary(ByVal primaryKeyValue As String, ByVal iIdEmployee As Integer, ByRef oEmployeeState As Employee.roEmployeeState, ByRef oUserFIeldState As UserFields.roUserFieldState, Optional tbUserFieldList As DataTable = Nothing) As Boolean
            Dim bolRet As Boolean = True
            Dim DateField As Date = Now.Date
            If Me.strImportPrimaryKeyUserField <> Me.strSystemUserIdField Then
                ' Crea campo NIF
                Dim bolUsedInProcess As Boolean = False
                If tbUserFieldList IsNot Nothing Then
                    Dim oFields() As DataRow = tbUserFieldList.Select("FieldName ='" & Me.strImportPrimaryKeyUserField.Replace("'", "''") & "'")
                    If oFields.Length > 0 Then
                        If roTypes.Any2Boolean(oFields(0).Item("UsedInProcess")) = True Then
                            bolUsedInProcess = True
                        End If
                    End If
                End If

                Dim oEmployeeUserField As New UserFields.roEmployeeUserField(iIdEmployee, Me.strImportPrimaryKeyUserField, DateField, oUserFIeldState)
                If oEmployeeUserField.FieldValue <> primaryKeyValue Then
                    oEmployeeUserField.FieldValue = primaryKeyValue
                    If tbUserFieldList IsNot Nothing Then
                        bolRet = oEmployeeUserField.Save(, , bolUsedInProcess)
                    Else
                        bolRet = oEmployeeUserField.Save()
                    End If
                End If
            End If

            Return bolRet

        End Function

        Protected Friend Function createExcelUserFields(ByVal ColumnsVal() As String, ByVal ColumnsUsrName() As String, ByRef oEmployeeState As Employee.roEmployeeState, ByRef oUserFIeldState As UserFields.roUserFieldState, ByVal bIsSageMurano As Boolean) As Boolean
            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not bIsSageMurano Then

                    Dim idColumnInitialDate As Integer = -1
                    Dim strUsrName As String = ""
                    Dim intUsrCol As Integer = 0
                    For intCol As Integer = System.Enum.GetValues(GetType(RoboticsExternAccess.EmployeeColumns)).Length To ColumnsVal.Length - 1

                        strUsrName = ColumnsUsrName(intUsrCol)
                        intUsrCol += 1
                        If InitialDateField_Is(strUsrName) = False Then
                            ' Busca la columna que determina la fecha inicial del campo de usuario
                            idColumnInitialDate = InitialDateField_GetColumn(strUsrName, ColumnsUsrName)

                            Dim hasHistoric As Boolean = False

                            If idColumnInitialDate <> -1 Then hasHistoric = True

                            Dim oUserField = New UserFields.roUserField(oUserFIeldState.IDPassport) ', strUsrName, Types.EmployeeField, False, oTrans, False)
                            oUserField.Type = Types.EmployeeField
                            oUserField.FieldName = strUsrName

                            If Not oUserField.Load(False, False) Then
                                oUserField.FieldName = strUsrName
                                oUserField.Type = Types.EmployeeField
                                oUserField.FieldType = FieldTypes.tText
                                oUserField.Used = True
                                oUserField.History = Not (idColumnInitialDate = -1)
                                bolRet = oUserField.Save(False)
                            Else
                                oUserField.Used = True
                                bolRet = oUserField.Save(False)
                            End If
                        End If

                        If Not bolRet Then Exit For
                    Next
                Else
                    For intCol As Integer = EmployeeSageMurano.USR_EMAIL To EmployeeSageMurano.USR_SEX
                        Dim strUsrName As String = ""

                        Select Case intCol
                            Case EmployeeSageMurano.USR_EMAIL : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_EMAIL", "")
                            Case EmployeeSageMurano.USR_SS : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_SS", "")
                            Case EmployeeSageMurano.USR_ADDRESS : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_ADDRESS", "")
                            Case EmployeeSageMurano.USR_POSTALCODE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_POSTALCODE", "")
                            Case EmployeeSageMurano.USR_TOWN : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_TOWN", "")
                            Case EmployeeSageMurano.USR_PROVINCE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PROVINCE", "")
                            Case EmployeeSageMurano.USR_PERSONALPHONE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PERSONALPHONE", "")
                            Case EmployeeSageMurano.USR_PERSONALMOBILE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_PERSONALMOBILE", "")
                            Case EmployeeSageMurano.USR_BIRTHDATE : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_USR_BIRTHDATE", "")
                            Case EmployeeSageMurano.USR_SEX : strUsrName = Me.State.Language.Translate("ImportSageMurano.USR_SEX", "")
                        End Select

                        If strUsrName <> String.Empty Then
                            Dim oUserField = New UserFields.roUserField(oUserFIeldState.IDPassport) ', strUsrName, Types.EmployeeField, oTrans, False)
                            oUserField.Type = Types.EmployeeField
                            oUserField.FieldName = strUsrName
                            If Not oUserField.Load(False, False) Then
                                oUserField.FieldName = strUsrName
                                oUserField.Type = Types.EmployeeField
                                oUserField.FieldType = FieldTypes.tText
                                oUserField.Used = True
                                oUserField.History = False
                                If Not oUserField.Save(False) Then
                                    bolRet = False
                                    roBusinessState.CopyTo(oUserFIeldState, Me.State)
                                End If
                            Else
                                bolRet = True
                                oUserField.Used = True
                                If Not oUserField.Save(False) Then
                                    bolRet = False
                                    roBusinessState.CopyTo(oUserFIeldState, Me.State)
                                End If
                            End If
                        End If

                        If Not bolRet Then Exit For
                    Next
                End If
            Catch ex As Exception
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Protected Friend Function InitialDateField_Is(ByVal FieldName As String) As Boolean
            ' Comprueba longitud mínima del campo
            If FieldName.Length < 15 Then Return False

            FieldName = UCase(FieldName)

            ' Comprueba si empieza por USR_
            If InStr(1, FieldName, "_#FECHAINICIO#") = 0 Then Return False

            Return True
        End Function

        Protected Friend Function InitialDateField_GetColumn(ByVal FieldName As String, ByVal ColumnsUSRName() As String) As Short
            FieldName = UCase(FieldName) & "_#FECHAINICIO#"

            ' Busca la fecha inicial dentro del array de columnas
            For i As Integer = 0 To ColumnsUSRName.Length - 1
                If UCase(ColumnsUSRName(i)) = FieldName Then
                    Return i
                End If
            Next
            Return -1
        End Function

        Protected Friend Function RotateHex(sHex As String) As String
            Dim sRes As String = String.Empty
            Try
                If sHex.Length Mod 2 <> 0 Then sHex = "0" & sHex
                For i = 0 To (sHex.Length / 2) - 1 Mod 2
                    sRes = sRes & sHex.Substring(sHex.Length - 2 * i - 2, 2)
                Next
                Return sRes
            Catch ex As Exception
                Return String.Empty
            End Try
        End Function

    End Class
End Namespace