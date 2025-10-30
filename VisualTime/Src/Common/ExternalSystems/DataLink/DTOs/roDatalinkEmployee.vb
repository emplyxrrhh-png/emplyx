Imports System.Runtime.Serialization
Imports System.Text
Imports SwaggerWcf.Attributes

Namespace DataLink.RoboticsExternAccess

    <DataContract>
    Public Class roDatalinkStandarEmployee
        Implements IDatalinkEmployee

        <SwaggerWcfProperty(Required:=True, Default:="John Smith", Description:="Name and Surname of the User")>
        <DataMember>
        Public Property NombreEmpleado As String

        <SwaggerWcfProperty(Required:=True, Default:="9876543210", Description:="Unique user identifier in the two systems")>
        <DataMember>
        Public Property UniqueEmployeeID As String

        <SwaggerWcfProperty(Required:=True, Default:="12345678A", Description:="User identity document. It is used as a secondary identifier if the user is not located by the UniqueEmployeeID")>
        <DataMember>
        Public Property NifEmpleado As String

        <SwaggerWcfProperty(Required:=True, Default:="1234.20210101", Description:="Contract identifier")>
        <DataMember>
        Public Property IDContract As String

        <SwaggerWcfProperty(Required:=False, Default:="General", Description:="Name of the Agreement to which the contract is adsigned")>
        <DataMember>
        Public Property LabAgreeName As String

        <SwaggerWcfProperty(Required:=False, Default:="Barcelona", Description:="Name of the work center associated with the contract")>
        <DataMember>
        Public Property WorkCenter As String

        <SwaggerWcfProperty(Required:=True, Default:="/Date(1609455600000)/", Description:="Contract start date. Epoch format (milliseconds from 1 January 1970)")>
        <DataMember>
        Public Property StartContractDate As Date

        <SwaggerWcfProperty(Required:=True, Default:="/Date(1609455600000)/", Description:="Date of completion of contract. Epoch format (milliseconds from 1 January 1970)")>
        <DataMember>
        Public Property EndContractDate As Nullable(Of Date)

        <SwaggerWcfProperty(Required:=True, Default:="/Date(1609455600000)/", Description:="Date of mobility at the indicated level. Epoch format (milliseconds from 1 January 1970)")>
        <DataMember>
        Public Property MobilityDate As Nullable(Of Date)

        <SwaggerWcfProperty(Required:=False, Default:="0", Description:="Type of contract identifier generated for new contracts")>
        <DataMember>
        Public Property CompositeContractType As eCompositeContractType

        <SwaggerWcfProperty(Required:=False, Default:="1234567", Description:="Proximity card number")>
        <DataMember>
        Public Property CardNumber As String

        <SwaggerWcfProperty(Required:=False, Default:="j.smith", Description:="Username for Portal access")>
        <DataMember>
        Public Property UserName As String

        <SwaggerWcfProperty(Required:=False, Default:="1", Description:="Supervision group. Only for security V2 or security V3")>
        <DataMember>
        Public Property SupervisorGroup As String

        <SwaggerWcfProperty(Required:=False, Default:="Mi Empresa", Description:="Root of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel0 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 1", Description:="Level 1 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel1 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 2", Description:="Level 2 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel2 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 3", Description:="Level 3 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel3 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 4", Description:="Level 4 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel4 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 5", Description:="Level 5 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel5 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 6", Description:="Level 6 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel6 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 7", Description:="Level 7 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel7 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 8", Description:="Level 8 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel8 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 9", Description:="Level 9 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel9 As String

        <SwaggerWcfProperty(Required:=False, Default:="Level 10", Description:="Level 10 of the user tree to which the user is assigned")>
        <DataMember>
        Public Property Nivel10 As String

        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Access Autorizations.")>
        <DataMember>
        Public Property ActiveAuthorizations As String()

        <SwaggerWcfProperty(Required:=False, Default:="", Description:="HR File Fields")>
        <DataMember>
        Public Property UserFields As roDatalinkEmployeeUserFieldValue()

        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Period for contract identifier")> 'TODO: LLUIS Aclarar
        <DataMember>
        Public Property Period As String

        <SwaggerWcfProperty(Required:=False, Default:="87654321A", Description:="NIF of the user to be used as template for the planning copy")>
        <DataMember>
        Public Property NifCopyPlan As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="Punch with photo required for the user")>
        <DataMember>
        Public Property RequirePunchWithPhoto As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="Punch with geolocation required for the user")>
        <DataMember>
        Public Property RequirePunchWithGeolocation As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="Enable access to Visualtime Desktop")>
        <DataMember>
        Public Property EnabledVTDesktop As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="Enable access to Visualtime Portal")>
        <DataMember>
        Public Property EnabledVTPortal As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="Enable access to Visualtime Portal App")>
        <DataMember>
        Public Property EnabledVTPortalApp As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="(Deprecated) Use EnabledVTPortal instead - Enable access through EnabledVTSupervisor has no effect")>
        <DataMember>
        Public Property EnabledVTSupervisor As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="(Deprecated) Use EnabledVTPortalApp instead - Enable access through EnabledVTSupervisorApp has no effect")>
        <DataMember>
        Public Property EnabledVTSupervisorApp As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="Enable access to Visualtime Visits")>
        <DataMember>
        Public Property EnabledVTVisits As String

        <SwaggerWcfProperty(Required:=False, Default:="false", Description:="Enable access to Visualtime without active contract")>
        <DataMember>
        Public Property LoginWithoutContract As String

        <DataMember>
        <SwaggerWcfProperty(Required:=True, Default:="iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAYAAACNiR0NAAAABHNCSVQICAgIfAhki...", Description:="Employee photo on base64 format")>
        Public Property UserPhoto As String

        <SwaggerWcfProperty(Required:=False, Default:="true", Description:="Enable biometric data")>
        <DataMember>
        Public Property EnableBiometricData As String

        <SwaggerWcfProperty(Required:=False, Default:="123456", Description:="Numeric Pin, length: 4-6; Enter 0 to delete PIN if it exists.", Minimum:="4", Maximum:="6")>
        <DataMember>
        Public Property Pin As String

        <SwaggerWcfProperty(Required:=False, Default:="", Description:="Data protection configuration")>
        <DataMember>
        Public Property Protection As roAPIProtectionData

        Public Function GetEmployeeColumnsDefinition(ByRef ColumnsVal() As String, ByRef ColumnsUsrName() As String, ByRef ColumnsPos() As Integer) As Boolean Implements DataLink.RoboticsExternAccess.IDatalinkEmployee.GetEmployeeColumnsDefinition
            Dim bolRet As Boolean = True

            Try

                Dim htFields As New Hashtable

                If UserFields IsNot Nothing Then
                    For Each oDatalinkUserField As roDatalinkEmployeeUserFieldValue In UserFields

                        htFields.Add(oDatalinkUserField.UserFieldName, oDatalinkUserField.UserFieldValue)

                        If oDatalinkUserField.UserFieldValueDate <> New Date(1970, 1, 1, 0, 0, 0) Then
                            htFields.Add(oDatalinkUserField.UserFieldName & "_#FECHAINICIO#", Format(oDatalinkUserField.UserFieldValueDate, "yyyy/MM/dd"))
                        End If
                    Next
                End If

                If UniqueEmployeeID <> String.Empty Then
                    Dim oParamValue = New Base.VTBusiness.Common.AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New Base.VTBusiness.Common.AdvancedParameter.roAdvancedParameterState)
                    If oParamValue.Value <> String.Empty Then

                        If htFields.ContainsKey(oParamValue.Value) Then
                            htFields(oParamValue.Value) = UniqueEmployeeID
                        Else
                            htFields.Add(oParamValue.Value, UniqueEmployeeID)
                        End If
                    End If
                End If

                If htFields.ContainsKey("NIF") Then
                    htFields("NIF") = NifEmpleado
                Else
                    htFields.Add("NIF", NifEmpleado)
                End If

                Dim iEmpresaColumnPosition As Integer = -1
                ReDim ColumnsPos(System.Enum.GetValues(GetType(EmployeeColumns)).Length + (htFields.Count - 1))

                'Definimos array con los valores de las columnas
                ReDim ColumnsVal(System.Enum.GetValues(GetType(EmployeeColumns)).Length + (htFields.Count - 1))

                ' Definimos array con los nombres de los campos personalizados
                ReDim ColumnsUsrName(-1)

                ' Inicializa los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For n As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsPos(n) = -1
                Next

                Dim intCol As Integer = System.Enum.GetValues(GetType(EmployeeColumns)).Length
                For Each strUserFieldName As String In htFields.Keys
                    Dim Column As String = strUserFieldName

                    If Not Column.StartsWith("USR_") Then Column = "USR_" & Column

                    ColumnsPos(intCol) = intCol
                    ColumnsVal(intCol) = htFields(strUserFieldName)

                    ' Si existe el campo empresa nos guardamos la posicion
                    If Column = "USR_EMPRESA" Then
                        iEmpresaColumnPosition = intCol
                    End If

                    ' Guardamos el nombre del campo de la ficha
                    ReDim Preserve ColumnsUsrName(ColumnsUsrName.Length)
                    ColumnsUsrName(ColumnsUsrName.Length - 1) = strUserFieldName

                    intCol = intCol + 1
                Next

                ColumnsPos(EmployeeColumns.Name) = CInt(EmployeeColumns.Name)
                ColumnsVal(CInt(EmployeeColumns.Name)) = NombreEmpleado

                ColumnsPos(EmployeeColumns.DNI) = CInt(EmployeeColumns.DNI)
                ColumnsVal(CInt(EmployeeColumns.DNI)) = NifEmpleado

                ColumnsPos(EmployeeColumns.ImportPrimaryKey) = CInt(EmployeeColumns.ImportPrimaryKey)
                ColumnsVal(CInt(EmployeeColumns.ImportPrimaryKey)) = UniqueEmployeeID

                ColumnsPos(EmployeeColumns.Contract) = CInt(EmployeeColumns.Contract)
                ColumnsVal(CInt(EmployeeColumns.Contract)) = IDContract

                ColumnsPos(EmployeeColumns.BeginDate) = CInt(EmployeeColumns.BeginDate)
                ColumnsVal(CInt(EmployeeColumns.BeginDate)) = StartContractDate.ToString("yyyy/MM/dd")

                ColumnsPos(EmployeeColumns.EndDate) = CInt(EmployeeColumns.EndDate)
                If EndContractDate.HasValue AndAlso EndContractDate.Value <> Date.MinValue Then
                    ColumnsVal(CInt(EmployeeColumns.EndDate)) = EndContractDate.Value.ToString("yyyy/MM/dd")
                Else
                    ColumnsVal(CInt(EmployeeColumns.EndDate)) = String.Empty
                End If

                ColumnsPos(EmployeeColumns.Surname1) = CInt(EmployeeColumns.Surname1) '6
                ColumnsPos(EmployeeColumns.Surname2) = CInt(EmployeeColumns.Surname2) '6

                ColumnsPos(EmployeeColumns.Card) = CInt(EmployeeColumns.Card)
                If CardNumber <> String.Empty Then
                    ColumnsVal(CInt(EmployeeColumns.Card)) = CardNumber
                Else
                    ColumnsVal(CInt(EmployeeColumns.Card)) = String.Empty
                End If

                If UserName <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Login) = CInt(EmployeeColumns.Login)
                    ColumnsVal(CInt(EmployeeColumns.Login)) = UserName
                Else
                    ColumnsPos(EmployeeColumns.Login) = -1
                End If

                If Nivel0 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level0) = CInt(EmployeeColumns.Level0)
                    ColumnsVal(CInt(EmployeeColumns.Level0)) = Nivel0
                Else
                    ColumnsPos(EmployeeColumns.Level0) = -1
                End If

                If Nivel1 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level1) = CInt(EmployeeColumns.Level1)
                    ColumnsVal(CInt(EmployeeColumns.Level1)) = Nivel1
                Else
                    ColumnsPos(EmployeeColumns.Level1) = -1
                End If

                If Nivel2 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level2) = CInt(EmployeeColumns.Level2)
                    ColumnsVal(CInt(EmployeeColumns.Level2)) = Nivel2
                Else
                    ColumnsPos(EmployeeColumns.Level2) = -1
                End If

                If Nivel3 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level3) = CInt(EmployeeColumns.Level3)
                    ColumnsVal(CInt(EmployeeColumns.Level3)) = Nivel3
                Else
                    ColumnsPos(EmployeeColumns.Level3) = -1
                End If

                If Nivel4 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level4) = CInt(EmployeeColumns.Level4)
                    ColumnsVal(CInt(EmployeeColumns.Level4)) = Nivel4
                Else
                    ColumnsPos(EmployeeColumns.Level4) = -1
                End If

                If Nivel5 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level5) = CInt(EmployeeColumns.Level5)
                    ColumnsVal(CInt(EmployeeColumns.Level5)) = Nivel5
                Else
                    ColumnsPos(EmployeeColumns.Level5) = -1
                End If

                If Nivel6 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level6) = CInt(EmployeeColumns.Level6)
                    ColumnsVal(CInt(EmployeeColumns.Level6)) = Nivel6
                Else
                    ColumnsPos(EmployeeColumns.Level6) = -1
                End If

                If Nivel7 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level7) = CInt(EmployeeColumns.Level7)
                    ColumnsVal(CInt(EmployeeColumns.Level7)) = Nivel7
                Else
                    ColumnsPos(EmployeeColumns.Level7) = -1
                End If

                If Nivel8 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level8) = CInt(EmployeeColumns.Level8)
                    ColumnsVal(CInt(EmployeeColumns.Level8)) = Nivel8
                Else
                    ColumnsPos(EmployeeColumns.Level8) = -1
                End If

                If Nivel9 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level9) = CInt(EmployeeColumns.Level9)
                    ColumnsVal(CInt(EmployeeColumns.Level9)) = Nivel9
                Else
                    ColumnsPos(EmployeeColumns.Level9) = -1
                End If

                If Nivel10 <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Level10) = CInt(EmployeeColumns.Level10)
                    ColumnsVal(CInt(EmployeeColumns.Level10)) = Nivel10
                Else
                    ColumnsPos(EmployeeColumns.Level10) = -1
                End If

                If ActiveAuthorizations IsNot Nothing AndAlso ActiveAuthorizations.Length > 0 Then
                    ColumnsPos(EmployeeColumns.Authorizations) = CInt(EmployeeColumns.Authorizations)
                    ColumnsVal(CInt(EmployeeColumns.Authorizations)) = String.Join(",", ActiveAuthorizations)
                Else
                    ColumnsPos(EmployeeColumns.Authorizations) = -1
                End If

                If LabAgreeName <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Convenio) = CInt(EmployeeColumns.Convenio)
                    ColumnsVal(CInt(EmployeeColumns.Convenio)) = LabAgreeName
                Else
                    ColumnsPos(EmployeeColumns.Convenio) = -1
                End If

                If SupervisorGroup <> String.Empty Then
                    ColumnsPos(EmployeeColumns.GrupoUsuarios) = CInt(EmployeeColumns.GrupoUsuarios)
                    ColumnsVal(CInt(EmployeeColumns.GrupoUsuarios)) = SupervisorGroup
                Else
                    ColumnsPos(EmployeeColumns.GrupoUsuarios) = -1
                End If

                If MobilityDate.HasValue AndAlso MobilityDate.Value <> Date.MinValue Then
                    ColumnsPos(EmployeeColumns.DateLevel10) = CInt(EmployeeColumns.DateLevel10)
                    ColumnsVal(CInt(EmployeeColumns.DateLevel10)) = MobilityDate.Value.ToString("yyyy/MM/dd")
                End If

                If Period <> String.Empty Then
                    ColumnsPos(EmployeeColumns.Period) = CInt(EmployeeColumns.Period)
                    ColumnsVal(CInt(EmployeeColumns.Period)) = Period
                Else
                    ColumnsPos(EmployeeColumns.Period) = -1
                End If

                If NifCopyPlan <> String.Empty Then
                    ColumnsPos(EmployeeColumns.DNICopyPlan) = CInt(EmployeeColumns.DNICopyPlan)
                    ColumnsVal(CInt(EmployeeColumns.DNICopyPlan)) = NifCopyPlan
                Else
                    ColumnsPos(EmployeeColumns.DNICopyPlan) = -1
                End If

                If WorkCenter <> String.Empty Then
                    ColumnsPos(EmployeeColumns.WorkCenter) = CInt(EmployeeColumns.WorkCenter)
                    ColumnsVal(CInt(EmployeeColumns.WorkCenter)) = WorkCenter
                Else
                    ColumnsPos(EmployeeColumns.WorkCenter) = -1
                End If

                If RequirePunchWithPhoto <> String.Empty Then
                    ColumnsPos(EmployeeColumns.RequirePunchWithPhoto) = CInt(EmployeeColumns.RequirePunchWithPhoto)
                    ColumnsVal(CInt(EmployeeColumns.RequirePunchWithPhoto)) = CBool(RequirePunchWithPhoto)
                Else
                    ColumnsPos(EmployeeColumns.RequirePunchWithPhoto) = CInt(EmployeeColumns.RequirePunchWithPhoto)
                    ColumnsVal(CInt(EmployeeColumns.RequirePunchWithPhoto)) = Nothing
                End If

                If RequirePunchWithGeolocation <> String.Empty Then
                    ColumnsPos(EmployeeColumns.RequirePunchWithGeolocation) = CInt(EmployeeColumns.RequirePunchWithGeolocation)
                    ColumnsVal(CInt(EmployeeColumns.RequirePunchWithGeolocation)) = CBool(RequirePunchWithGeolocation)
                Else
                    ColumnsPos(EmployeeColumns.RequirePunchWithGeolocation) = CInt(EmployeeColumns.RequirePunchWithGeolocation)
                    ColumnsVal(CInt(EmployeeColumns.RequirePunchWithGeolocation)) = Nothing
                End If

                If EnabledVTDesktop <> String.Empty Then
                    ColumnsPos(EmployeeColumns.EnabledVTDesktop) = CInt(EmployeeColumns.EnabledVTDesktop)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTDesktop)) = CBool(EnabledVTDesktop)
                Else
                    ColumnsPos(EmployeeColumns.EnabledVTDesktop) = CInt(EmployeeColumns.EnabledVTDesktop)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTDesktop)) = Nothing
                End If

                If EnabledVTPortal <> String.Empty Then
                    ColumnsPos(EmployeeColumns.EnabledVTPortal) = CInt(EmployeeColumns.EnabledVTPortal)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTPortal)) = CBool(EnabledVTPortal)
                Else
                    ColumnsPos(EmployeeColumns.EnabledVTPortal) = CInt(EmployeeColumns.EnabledVTPortal)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTPortal)) = Nothing
                End If

                If EnabledVTPortalApp <> String.Empty Then
                    ColumnsPos(EmployeeColumns.EnabledVTPortalApp) = CInt(EmployeeColumns.EnabledVTPortalApp)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTPortalApp)) = CBool(EnabledVTPortalApp)
                Else
                    ColumnsPos(EmployeeColumns.EnabledVTPortalApp) = CInt(EmployeeColumns.EnabledVTPortalApp)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTPortalApp)) = Nothing
                End If

                If EnabledVTVisits <> String.Empty Then
                    ColumnsPos(EmployeeColumns.EnabledVTVisits) = CInt(EmployeeColumns.EnabledVTVisits)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTVisits)) = CBool(EnabledVTVisits)
                Else
                    ColumnsPos(EmployeeColumns.EnabledVTVisits) = CInt(EmployeeColumns.EnabledVTVisits)
                    ColumnsVal(CInt(EmployeeColumns.EnabledVTVisits)) = Nothing
                End If

                If LoginWithoutContract <> String.Empty Then
                    ColumnsPos(EmployeeColumns.LoginWithoutContract) = CInt(EmployeeColumns.LoginWithoutContract)
                    ColumnsVal(CInt(EmployeeColumns.LoginWithoutContract)) = CBool(LoginWithoutContract)
                Else
                    ColumnsPos(EmployeeColumns.LoginWithoutContract) = CInt(EmployeeColumns.LoginWithoutContract)
                    ColumnsVal(CInt(EmployeeColumns.LoginWithoutContract)) = Nothing
                End If

                If UserPhoto IsNot Nothing Then
                    ColumnsPos(EmployeeColumns.UserPhoto) = CInt(EmployeeColumns.UserPhoto)
                    ColumnsVal(CInt(EmployeeColumns.UserPhoto)) = UserPhoto
                Else
                    ColumnsPos(EmployeeColumns.UserPhoto) = CInt(EmployeeColumns.UserPhoto)
                    ColumnsVal(CInt(EmployeeColumns.UserPhoto)) = Nothing
                End If

                If EnableBiometricData IsNot Nothing Then
                    ColumnsPos(EmployeeColumns.EnableBiometricData) = CInt(EmployeeColumns.EnableBiometricData)
                    ColumnsVal(CInt(EmployeeColumns.EnableBiometricData)) = EnableBiometricData
                Else
                    ColumnsPos(EmployeeColumns.EnableBiometricData) = CInt(EmployeeColumns.EnableBiometricData)
                    ColumnsVal(CInt(EmployeeColumns.EnableBiometricData)) = Nothing
                End If

                If Pin IsNot Nothing Then
                    ColumnsPos(EmployeeColumns.Pin) = CInt(EmployeeColumns.Pin)
                    ColumnsVal(CInt(EmployeeColumns.Pin)) = Pin
                Else
                    ColumnsPos(EmployeeColumns.Pin) = CInt(EmployeeColumns.Pin)
                    ColumnsVal(CInt(EmployeeColumns.Pin)) = Nothing
                End If

                ' TODO
                ' Verificar que los niveles que llegan informados son correlativos (no hay niveles "" en medio de la cadena)

                ' Comprueba que los valores obligatorios estén informados
                If ColumnsVal(ColumnsPos(EmployeeColumns.BeginDate)).Length > 0 And (ColumnsVal(ColumnsPos(EmployeeColumns.DNI)).Length > 0 OrElse ColumnsVal(ColumnsPos(EmployeeColumns.ImportPrimaryKey)).Length > 0) And
                   ColumnsVal(ColumnsPos(EmployeeColumns.Name)).Length > 0 And ColumnsVal(ColumnsPos(EmployeeColumns.Contract)).Length > 0 Then

                    If CompositeContractType <> eCompositeContractType.None Then
                        If CompositeContractType <> eCompositeContractType.ContractAndPeriod Then
                            If IsDate(ColumnsVal(ColumnsPos(EmployeeColumns.BeginDate))) Then
                                bolRet = True
                                Dim strBeginDate As String = Format(CDate(ColumnsVal(ColumnsPos(EmployeeColumns.BeginDate))), "yyyy/MM/dd")
                                strBeginDate = strBeginDate.Replace("/", "")
                                Select Case CompositeContractType
                                    Case eCompositeContractType.ContractAndDate
                                        ColumnsVal(ColumnsPos(EmployeeColumns.Contract)) = ColumnsVal(ColumnsPos(EmployeeColumns.Contract)) & "." & strBeginDate
                                    Case eCompositeContractType.CompanyContractAndDate
                                        '"EMPRESA_CONTRATO_FECHA ALTA"
                                        If iEmpresaColumnPosition <> -1 Then
                                            If ColumnsVal(iEmpresaColumnPosition).Length > 0 Then
                                                ColumnsVal(ColumnsPos(EmployeeColumns.Contract)) = ColumnsVal(iEmpresaColumnPosition) & "." & ColumnsVal(ColumnsPos(EmployeeColumns.Contract)) & "." & strBeginDate
                                            Else
                                                bolRet = False
                                            End If
                                        Else
                                            bolRet = False
                                        End If
                                End Select
                            Else
                                bolRet = False
                            End If
                        Else
                            ' CONTRATO_PERIODO
                            bolRet = True
                            Try
                                If ColumnsVal(ColumnsPos(EmployeeColumns.Period)).Length > 0 Then
                                    ColumnsVal(ColumnsPos(EmployeeColumns.Contract)) = ColumnsVal(ColumnsPos(EmployeeColumns.Contract)) & "." & ColumnsVal(ColumnsPos(EmployeeColumns.Period))
                                Else
                                    bolRet = False
                                End If
                            Catch ex As Exception
                                bolRet = False
                            End Try
                        End If
                    Else
                        bolRet = True
                    End If

                    If bolRet Then
                        If Not (ColumnsPos(EmployeeColumns.Contract) >= 0 AndAlso (ColumnsPos(EmployeeColumns.DNI) >= 0 OrElse ColumnsPos(EmployeeColumns.ImportPrimaryKey) >= 0) AndAlso
                                ColumnsPos(EmployeeColumns.BeginDate) >= 0 AndAlso ColumnsPos(EmployeeColumns.EndDate) >= 0 AndAlso ColumnsPos(EmployeeColumns.Name) >= 0) Then
                            bolRet = False
                        End If
                    End If

                End If
            Catch ex As Exception
                bolRet = False
            End Try

            Return bolRet
        End Function



    End Class

End Namespace