Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Base

    <DataContract>
    Public Class roSecurityOptions

#Region "Declarations - Constructors"

        Private oState As roSecurityOptionsState

        Private intIDPassport As Integer
        Private intPasswordSecurityLevel As PasswordSecurityLevelEnum
        Private intPreviousPasswordsStored As Integer
        Private intDaysPasswordExpired As Integer
        Private intAccessAttempsTemporaryBlock As Integer
        Private intAccessAttempsPermanentBlock As Integer
        Private strOnlyAllowedAdress As String
        Private bolOnlySameVersionApp As Boolean
        Private bolRequestValidationCode As Boolean
        Private intSaveAuthorizedPointDays As Integer
        Private intDifferentAccessPointExceeded As Integer
        Private bolBlockAccessVTPortal As Boolean
        Private bolBlockAccessVTDesktop As Boolean
        Private bolEnabledAccessSupportRobotics As Boolean

        Private bolCalendarLock As Boolean

        Public Sub New()

            Me.oState = New roSecurityOptionsState
            Me.intIDPassport = -1
            Me.bolCalendarLock = True
            Me.intPasswordSecurityLevel = PasswordSecurityLevelEnum._LOW

        End Sub

        Public Sub New(ByVal _ID As Long, ByVal _State As roSecurityOptionsState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.intIDPassport = _ID
            Me.bolCalendarLock = True
            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal _Row As DataRow, ByVal _State As roSecurityOptionsState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State
            Me.bolCalendarLock = True
            Me.LoadFromRow(_Row, _Audit)

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        <XmlIgnore()>
        Public Property State() As roSecurityOptionsState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roSecurityOptionsState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property Passport() As Integer
            Get
                Return Me.intIDPassport
            End Get
            Set(ByVal value As Integer)
                Me.intIDPassport = value
            End Set
        End Property

        <DataMember>
        Public Property PasswordSecurityLevel() As PasswordSecurityLevelEnum
            Get
                Return Me.intPasswordSecurityLevel
            End Get
            Set(ByVal value As PasswordSecurityLevelEnum)
                Me.intPasswordSecurityLevel = value
            End Set
        End Property

        <DataMember>
        Public Property PreviousPasswordsStored() As Integer
            Get
                Return Me.intPreviousPasswordsStored
            End Get
            Set(ByVal value As Integer)
                Me.intPreviousPasswordsStored = value
            End Set
        End Property

        <DataMember>
        Public Property DaysPasswordExpired() As Integer
            Get
                Return Me.intDaysPasswordExpired
            End Get
            Set(ByVal value As Integer)
                Me.intDaysPasswordExpired = value
            End Set
        End Property

        <DataMember>
        Public Property AccessAttempsTemporaryBlock() As Integer
            Get
                Return Me.intAccessAttempsTemporaryBlock
            End Get
            Set(ByVal value As Integer)
                Me.intAccessAttempsTemporaryBlock = value
            End Set
        End Property

        <DataMember>
        Public Property AccessAttempsPermanentBlock() As Integer
            Get
                Return Me.intAccessAttempsPermanentBlock
            End Get
            Set(ByVal value As Integer)
                Me.intAccessAttempsPermanentBlock = value
            End Set
        End Property

        <DataMember>
        Public Property OnlyAllowedAdress() As String
            Get
                Return Me.strOnlyAllowedAdress
            End Get
            Set(ByVal value As String)
                Me.strOnlyAllowedAdress = value
            End Set
        End Property

        <DataMember>
        Public Property OnlySameVersionApp() As Boolean
            Get
                Return bolOnlySameVersionApp
            End Get
            Set(ByVal value As Boolean)
                bolOnlySameVersionApp = value
            End Set
        End Property

        <DataMember>
        Public Property RequestValidationCode() As Boolean
            Get
                Return bolRequestValidationCode
            End Get
            Set(ByVal value As Boolean)
                bolRequestValidationCode = value
            End Set
        End Property

        <DataMember>
        Public Property SaveAuthorizedPointDays() As Integer
            Get
                Return Me.intSaveAuthorizedPointDays
            End Get
            Set(ByVal value As Integer)
                Me.intSaveAuthorizedPointDays = value
            End Set
        End Property

        <DataMember>
        Public Property DifferentAccessPointExceeded() As Integer
            Get
                Return Me.intDifferentAccessPointExceeded
            End Get
            Set(ByVal value As Integer)
                Me.intDifferentAccessPointExceeded = value
            End Set
        End Property

        <DataMember>
        Public Property BlockAccessVTPortal() As Boolean
            Get
                Return bolBlockAccessVTPortal
            End Get
            Set(ByVal value As Boolean)
                bolBlockAccessVTPortal = value
            End Set
        End Property

        <DataMember>
        Public Property BlockAccessVTDesktop() As Boolean
            Get
                Return bolBlockAccessVTDesktop
            End Get
            Set(ByVal value As Boolean)
                bolBlockAccessVTDesktop = value
            End Set
        End Property

        <DataMember>
        Public Property EnabledAccessSupportRobotics() As Boolean
            Get
                Return bolEnabledAccessSupportRobotics
            End Get
            Set(ByVal value As Boolean)
                bolEnabledAccessSupportRobotics = value
            End Set
        End Property

        <DataMember>
        Public Property CalendarLock() As Boolean
            Get
                Return bolCalendarLock
            End Get
            Set(ByVal value As Boolean)
                bolCalendarLock = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@SELECT# * FROM sysroSecurityParameters WHERE IDPassport = " & Me.intIDPassport.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intIDPassport = Any2Integer(oRow("IDPassport"))
                    Me.intPasswordSecurityLevel = Any2Integer(oRow("PasswordSecurityLevel"))

                    Me.intPreviousPasswordsStored = Any2Integer(oRow("PreviousPasswordsStored"))

                    Me.intDaysPasswordExpired = Any2Integer(oRow("DaysPasswordExpired"))
                    Me.intAccessAttempsTemporaryBlock = Any2Integer(oRow("AccessAttempsTemporaryBlock"))
                    Me.intAccessAttempsPermanentBlock = Any2Integer(oRow("AccessAttempsPermanentBlock"))
                    Me.strOnlyAllowedAdress = Any2String(oRow("OnlyAllowedAdress"))
                    Me.bolOnlySameVersionApp = Any2Boolean(oRow("OnlySameVersionApp"))
                    Me.bolRequestValidationCode = Any2Boolean(oRow("RequestValidationCode"))
                    Me.intSaveAuthorizedPointDays = Any2Integer(oRow("SaveAuthorizedPointDays"))
                    Me.intDifferentAccessPointExceeded = Any2Integer(oRow("DifferentAccessPointExceeded"))

                    Me.bolBlockAccessVTPortal = Any2Boolean(oRow("BlockAccessVTPortal"))
                    Me.bolBlockAccessVTDesktop = Any2Boolean(oRow("BlockAccessVTDesktop"))

                    Me.bolEnabledAccessSupportRobotics = Any2Boolean(oRow("EnabledAccessSupportRobotics"))
                    Me.bolCalendarLock = Any2Boolean(oRow("CalendarLock"))

                    bolRet = True

                    ' Auditar lectura
                    If _Audit Then
                        Dim strObjectName As String = ""
                        Dim intIDPassport As Integer = Me.intIDPassport
                        If intIDPassport <> 0 Then
                            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(intIDPassport, LoadType.Passport)
                            If oPassport IsNot Nothing Then strObjectName = oPassport.Name
                        End If

                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tSecurityOptions, strObjectName, Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roSecurityOptions::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roSecurityOptions::Load")
            End Try

            Return bolRet

        End Function

        Private Function LoadFromRow(ByVal oRow As DataRow, Optional ByVal _Audit As Boolean = False)

            Dim bolRet As Boolean = False

            If oRow IsNot Nothing Then

                Me.intIDPassport = Any2Integer(oRow("IDPassport"))
                Me.intPasswordSecurityLevel = Any2Integer(oRow("PasswordSecurityLevel"))

                Me.intPreviousPasswordsStored = Any2Integer(oRow("PreviousPasswordsStored"))

                Me.intDaysPasswordExpired = Any2Integer(oRow("DaysPasswordExpired"))
                Me.intAccessAttempsTemporaryBlock = Any2Integer(oRow("AccessAttempsTemporaryBlock"))
                Me.intAccessAttempsPermanentBlock = Any2Integer(oRow("AccessAttempsPermanentBlock"))
                Me.strOnlyAllowedAdress = Any2String(oRow("OnlyAllowedAdress"))
                Me.bolOnlySameVersionApp = Any2Boolean(oRow("OnlySameVersionApp"))
                Me.bolRequestValidationCode = Any2Boolean(oRow("RequestValidationCode"))
                Me.intSaveAuthorizedPointDays = Any2Integer(oRow("SaveAuthorizedPointDays"))
                Me.intDifferentAccessPointExceeded = Any2Integer(oRow("DifferentAccessPointExceeded"))

                Me.bolBlockAccessVTPortal = Any2Boolean(oRow("BlockAccessVTPortal"))
                Me.bolBlockAccessVTDesktop = Any2Boolean(oRow("BlockAccessVTDesktop"))

                Me.bolEnabledAccessSupportRobotics = Any2Boolean(oRow("EnabledAccessSupportRobotics"))
                Me.bolCalendarLock = Any2Boolean(oRow("CalendarLock"))

                bolRet = True

                ' Auditar lectura
                If _Audit Then
                    'Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    'oState.AddAuditParameter(tbParameters, "{Passport}", Me.intIDPassport, "", 1)
                    'bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tSecurityOptions, "", tbParameters, -1)
                End If

            End If

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByRef oStateP As roSecurityOptionsState = Nothing) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            If oStateP IsNot Nothing Then Me.oState = oStateP
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.State.Result = SecurityOptionsResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = AccessHelper.StartTransaction()

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("sysroSecurityParameters")
                    Dim strSQL As String = "@SELECT# * FROM sysroSecurityParameters " &
                                           "WHERE IDPassport = " & Me.intIDPassport.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim iDefaultSecuriyLevel As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar("@SELECT# PasswordSecurityLevel FROM sysroSecurityParameters WHERE IDPassport = 0"))

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("PasswordSecurityLevel") = iDefaultSecuriyLevel
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("IDPassport") = Me.intIDPassport
                    oRow("PreviousPasswordsStored") = Me.intPreviousPasswordsStored
                    oRow("DaysPasswordExpired") = Me.intDaysPasswordExpired
                    oRow("AccessAttempsTemporaryBlock") = Me.intAccessAttempsTemporaryBlock
                    oRow("AccessAttempsPermanentBlock") = Me.intAccessAttempsPermanentBlock
                    oRow("OnlyAllowedAdress") = Me.strOnlyAllowedAdress
                    oRow("OnlySameVersionApp") = Me.bolOnlySameVersionApp
                    oRow("RequestValidationCode") = Me.bolRequestValidationCode
                    oRow("SaveAuthorizedPointDays") = Me.intSaveAuthorizedPointDays
                    oRow("DifferentAccessPointExceeded") = Me.intDifferentAccessPointExceeded
                    oRow("BlockAccessVTPortal") = Me.bolBlockAccessVTPortal
                    oRow("BlockAccessVTDesktop") = Me.bolBlockAccessVTDesktop
                    oRow("EnabledAccessSupportRobotics") = Me.bolEnabledAccessSupportRobotics
                    oRow("CalendarLock") = Me.bolCalendarLock

                    If tb.Rows.Count <= 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    Dim where As String = ""
                    If Me.Passport <> 0 Then
                        where = " WHERE IDPassport IN (@SELECT# ID FROM GetPassportChilds(" & Me.intIDPassport.ToString & "))"
                    Else
                        where = " WHERE IDPassport <> 0"
                    End If

                    Dim strSQLChilds As String = ""

                    If bolRet Then
                        strSQLChilds = "@UPDATE# sysroSecurityParameters SET PasswordSecurityLevel=" & iDefaultSecuriyLevel & " WHERE IDPassport <> 0"
                        bolRet = ExecuteSql(strSQLChilds)
                    End If

                    If bolRet AndAlso Me.bolOnlySameVersionApp = True Then
                        strSQLChilds = "@UPDATE# sysroSecurityParameters SET OnlySameVersionApp=0" & where
                        bolRet = ExecuteSql(strSQLChilds)
                    End If

                    If bolRet AndAlso Me.bolRequestValidationCode = False AndAlso Me.Passport = 0 Then
                        strSQLChilds = "@DELETE# FROM sysroPassports_AuthorizedAdress "
                        bolRet = ExecuteSql(strSQLChilds)
                    End If

                    If bolRet AndAlso Me.bolCalendarLock = False Then
                        strSQLChilds = "@UPDATE# sysroSecurityParameters SET CalendarLock=0" & where
                        bolRet = ExecuteSql(strSQLChilds)
                    Else
                        strSQLChilds = "@UPDATE# sysroSecurityParameters SET CalendarLock=1" & where
                        bolRet = ExecuteSql(strSQLChilds)
                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String = ""
                        Dim intIDPassport As Integer = 0
                        If oAuditAction = Audit.Action.aInsert Then
                            intIDPassport = oAuditDataNew("IDPassport")
                        Else
                            intIDPassport = oAuditDataOld("IDPassport")
                        End If
                        If intIDPassport <> 0 Then
                            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(intIDPassport, LoadType.Passport)
                            If oPassport IsNot Nothing Then strObjectName = oPassport.Name
                        End If

                        Me.oState.Audit(oAuditAction, Audit.ObjectType.tSecurityOptions, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityOptions::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityOptions::Save")
            Finally
                AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                If Me.AccessAttempsPermanentBlock < Me.AccessAttempsTemporaryBlock Then
                    oState.Result = SecurityOptionsResultEnum.PermanenBlockAccessError
                    bolRet = False
                End If

                If Me.RequestValidationCode AndAlso (Me.DifferentAccessPointExceeded <= 0 OrElse Me.SaveAuthorizedPointDays <= 0) Then
                    oState.Result = SecurityOptionsResultEnum.InvalidKeyRequierementValues
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityOptions::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityOptions::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim strArray As New ArrayList
            Dim strSql As String

            Try

                strArray.Add("@DELETE# sysroSecurityParameters WHERE IDPassport= " & Me.intIDPassport.ToString)

                For Each strSql In strArray
                    bolRet = ExecuteSql(strSql)
                    If Not bolRet Then Exit For
                Next

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim strObjectName As String = ""
                    Dim intIDPassport As Integer = Me.Passport
                    If intIDPassport <> 0 Then
                        Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(intIDPassport, LoadType.Passport)
                        If oPassport IsNot Nothing Then strObjectName = oPassport.Name
                    End If

                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tSecurityOptions, strObjectName, Nothing, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityOptions::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityOptions::Delete")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve true o false si la contraseña no es ninguna de las anteriores
        ''' </summary>
        ''' <param name="intIDPassport"></param>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function IsValidPwdHistory(ByVal intIDPassport As Integer, ByVal strPwd As String, ByVal PreviousPasswordsStored As Integer, ByVal oState As roSecurityOptionsState) As Boolean
            Dim oRet As Boolean = False

            Try

                Dim tbPasswordHistory As DataTable
                Dim bolExist As Boolean = False
                tbPasswordHistory = CreateDataTable("@SELECT# top " & PreviousPasswordsStored & " *  from sysroPassports_PasswordHistory WHERE IDPassport= " & intIDPassport & " order by TimeStamp desc")
                If tbPasswordHistory.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbPasswordHistory.Rows
                        ' Si la contraseña coincide no es valido
                        If oRow("Password") = strPwd Then
                            bolExist = True
                            Exit For
                        End If
                    Next
                End If

                oRet = Not bolExist
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityOptions::IsValidPwdHistory")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityOptions::IsValidPwdHistory")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetInheritedSecurityOptions(ByVal idPassport As Integer, ByVal oState As roSecurityOptionsState, Optional ByVal bAudit As Boolean = False) As roSecurityOptions
            Dim passportSecurityOptions As roSecurityOptions = Nothing

            Try

                If oState.Result = SecurityOptionsResultEnum.NoError Then

                    'cargamos la configuración global
                    Dim globalSecurityOptions As New roSecurityOptions(0, oState)
                    passportSecurityOptions = InitPassportFromGlobalSecurity(globalSecurityOptions)


                    Dim strSQL As String = "@SELECT# * FROM sysroSecurityParameters WHERE IDPassport IN (@SELECT# ID FROM GetPassportParents(" & idPassport.ToString & "))"
                    Dim inheritedPermissions As DataTable = CreateDataTable(strSQL)

                    For Each oRow As DataRow In inheritedPermissions.Rows

                        If roTypes.Any2String(oRow("OnlyAllowedAdress")) <> String.Empty Then
                            If Not passportSecurityOptions.OnlyAllowedAdress = String.Empty Then passportSecurityOptions.OnlyAllowedAdress = passportSecurityOptions.OnlyAllowedAdress & "#"
                            passportSecurityOptions.OnlyAllowedAdress = passportSecurityOptions.OnlyAllowedAdress & roTypes.Any2String(oRow("OnlyAllowedAdress"))
                        End If

                        If globalSecurityOptions.OnlySameVersionApp Then
                            passportSecurityOptions.OnlySameVersionApp = globalSecurityOptions.OnlySameVersionApp
                        Else
                            If Any2Boolean(oRow("OnlySameVersionApp")) Then passportSecurityOptions.OnlySameVersionApp = Any2Boolean(oRow("OnlySameVersionApp"))
                        End If

                        If Any2Boolean(oRow("RequestValidationCode")) Then
                            passportSecurityOptions.RequestValidationCode = Any2Boolean(oRow("RequestValidationCode"))

                            If Any2Integer(oRow("SaveAuthorizedPointDays")) < passportSecurityOptions.SaveAuthorizedPointDays Then
                                passportSecurityOptions.SaveAuthorizedPointDays = Any2Integer(oRow("SaveAuthorizedPointDays"))
                            End If

                            If Any2Integer(oRow("DifferentAccessPointExceeded")) < passportSecurityOptions.DifferentAccessPointExceeded Then
                                passportSecurityOptions.DifferentAccessPointExceeded = Any2Integer(oRow("DifferentAccessPointExceeded"))
                            End If
                        End If

                        If Not globalSecurityOptions.CalendarLock Then
                            passportSecurityOptions.CalendarLock = globalSecurityOptions.CalendarLock
                        Else
                            If Not Any2Boolean(oRow("CalendarLock")) Then passportSecurityOptions.CalendarLock = Any2Boolean(oRow("CalendarLock"))
                        End If

                    Next
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roSecurityOptions::GetInheritedSecurityOptions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityOptions::GetInheritedSecurityOptions")
            End Try

            Return passportSecurityOptions
        End Function

        Private Shared Function InitPassportFromGlobalSecurity(ByVal globalSecurityOptions As roSecurityOptions) As roSecurityOptions
            Dim inheritedSecurityOptions As New roSecurityOptions

            If globalSecurityOptions.OnlyAllowedAdress <> String.Empty Then
                inheritedSecurityOptions.OnlyAllowedAdress = globalSecurityOptions.OnlyAllowedAdress
            End If

            If globalSecurityOptions.OnlySameVersionApp Then
                inheritedSecurityOptions.OnlySameVersionApp = globalSecurityOptions.OnlySameVersionApp
            End If

            If globalSecurityOptions.RequestValidationCode Then
                inheritedSecurityOptions.RequestValidationCode = globalSecurityOptions.RequestValidationCode
                inheritedSecurityOptions.SaveAuthorizedPointDays = globalSecurityOptions.SaveAuthorizedPointDays
                inheritedSecurityOptions.DifferentAccessPointExceeded = globalSecurityOptions.DifferentAccessPointExceeded
            End If

            inheritedSecurityOptions.OnlyAllowedAdress = globalSecurityOptions.OnlyAllowedAdress
            inheritedSecurityOptions.RequestValidationCode = globalSecurityOptions.RequestValidationCode
            inheritedSecurityOptions.SaveAuthorizedPointDays = globalSecurityOptions.SaveAuthorizedPointDays
            inheritedSecurityOptions.DifferentAccessPointExceeded = globalSecurityOptions.DifferentAccessPointExceeded
            inheritedSecurityOptions.CalendarLock = globalSecurityOptions.CalendarLock

            Return inheritedSecurityOptions
        End Function

        Public Shared Function IsValidPwd(ByVal strPwd As String, ByVal loggedInPassport As roPassportTicket, ByVal idPassport As Integer, ByRef oState As roSecurityState, Optional ByVal validateHistory As Boolean = False, Optional ByVal actualPwd As String = "",
                                          Optional ByVal validateActualPwd As Boolean = False) As PasswordLevelError
            '
            ' En funcion del nivel de complejidad de contraseña validamos que sea correcta
            '

            Dim errorValidation As PasswordLevelError


            Try
                Dim _Passport As roPassportTicket = Nothing

                If loggedInPassport IsNot Nothing Then
                    _Passport = loggedInPassport
                Else
                    _Passport = roPassportManager.GetPassportTicket(idPassport, LoadType.Passport)
                End If


                If _Passport Is Nothing Then Return PasswordLevelError.No_Passport_Error

                Dim strLogin As String = _Passport.AuthCredential

                ' Obtenemos el nivel de seguridad
                Dim PasswordSecurityLevel As PasswordSecurityLevelEnum = PasswordSecurityLevelEnum._LOW
                Dim oSecurityOptions As roSecurityOptions = New roSecurityOptions(0, New roSecurityOptionsState)
                If oSecurityOptions.State.Result = SecurityOptionsResultEnum.NoError Then
                    PasswordSecurityLevel = oSecurityOptions.PasswordSecurityLevel
                End If

                Dim actualPwdHashed = roTypes.Any2String(AccessHelper.ExecuteScalar($"@SELECT# password from sysroPassports_AuthenticationMethods where IDPassport = {_Passport.ID} and Method = 1 "))
                Dim bContinue As Boolean = True

                If validateActualPwd AndAlso actualPwd = String.Empty OrElse (actualPwd <> String.Empty AndAlso actualPwdHashed <> CryptographyHelper.EncryptWithMD5(actualPwd)) Then
                    bContinue = False
                End If

                If bContinue Then

                    Select Case PasswordSecurityLevel
                        Case PasswordSecurityLevelEnum._LOW
                            errorValidation = ValidateLowSecurityPassword(strPwd, validateHistory, actualPwd, _Passport, strLogin, oSecurityOptions)

                        Case PasswordSecurityLevelEnum._MEDIUM
                            errorValidation = ValidateMediumSecurityPassword(strPwd, validateHistory, actualPwd, _Passport, strLogin, oSecurityOptions)

                        Case PasswordSecurityLevelEnum._HIGH
                            errorValidation = ValidateHighSecurityPassword(strPwd, validateHistory, actualPwd, _Passport, strLogin, oSecurityOptions)

                    End Select
                Else
                    Select Case PasswordSecurityLevel
                        Case PasswordSecurityLevelEnum._LOW
                            errorValidation = PasswordLevelError.Low_Error
                        Case PasswordSecurityLevelEnum._MEDIUM
                            errorValidation = PasswordLevelError.Medium_Error
                        Case PasswordSecurityLevelEnum._HIGH
                            errorValidation = PasswordLevelError.High_Error
                    End Select
                End If
            Catch ex As Exception
                oState.Result = SecurityResultEnum.Exception
            End Try

            Return errorValidation

        End Function

        Private Shared Function ValidateHighSecurityPassword(strPwd As String, validateHistory As Boolean, actualPwd As String, passport As roPassportTicket, strLogin As String, oSecurityOptions As roSecurityOptions) As PasswordLevelError
            Dim errorValidation As PasswordLevelError = PasswordLevelError.High_Error

            Dim upper As New System.Text.RegularExpressions.Regex("[A-Z]")
            Dim lower As New System.Text.RegularExpressions.Regex("[a-z]")
            Dim number As New System.Text.RegularExpressions.Regex("[0-9]")
            Dim special As New System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]")

            ' La contraseña tiene que tener una longitud minima de 10 caracteres y contener letras (mayusculas y minusculas),numeros y caracteres especiales
            ' no coincidir con el nombre de usuario
            ' y que no coincida con las anteriores
            If strPwd.Length > 0 AndAlso strPwd.Length >= 10 AndAlso strPwd <> strLogin AndAlso
                upper.Matches(strPwd).Count > 0 AndAlso lower.Matches(strPwd).Count > 0 AndAlso
                number.Matches(strPwd).Count > 0 AndAlso special.Matches(strPwd).Count > 0 Then

                If validateHistory AndAlso strPwd <> actualPwd Then
                    If roSecurityOptions.IsValidPwdHistory(passport.ID, CryptographyHelper.EncryptWithMD5(strPwd), oSecurityOptions.PreviousPasswordsStored, New roSecurityOptionsState) Then
                        errorValidation = PasswordLevelError.No_Error
                    End If
                Else
                    errorValidation = PasswordLevelError.No_Error
                End If
            End If

            Return errorValidation
        End Function

        Private Shared Function ValidateMediumSecurityPassword(strPwd As String, validateHistory As Boolean, actualPwd As String, passport As roPassportTicket, strLogin As String, oSecurityOptions As roSecurityOptions) As PasswordLevelError
            Dim errorValidation As PasswordLevelError = PasswordLevelError.Medium_Error
            Dim upper As New System.Text.RegularExpressions.Regex("[A-Z]")
            Dim lower As New System.Text.RegularExpressions.Regex("[a-z]")
            Dim number As New System.Text.RegularExpressions.Regex("[0-9]")
            Dim special As New System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]")

            ' La contraseña tiene que tener una longitud minima de 6 caracteres y contener letras (mayusculas y minusculas) y numeros
            ' no coincidir con el nombre de usuario
            ' y que no coincida con las anteriores
            If strPwd.Length > 0 AndAlso strPwd.Length >= 6 AndAlso strPwd <> strLogin AndAlso
                upper.Matches(strPwd).Count > 0 AndAlso lower.Matches(strPwd).Count > 0 AndAlso
                number.Matches(strPwd).Count > 0 Then

                If validateHistory AndAlso strPwd <> actualPwd Then
                    If roSecurityOptions.IsValidPwdHistory(passport.ID, CryptographyHelper.EncryptWithMD5(strPwd), oSecurityOptions.PreviousPasswordsStored, New roSecurityOptionsState) Then
                        errorValidation = PasswordLevelError.No_Error
                    End If
                Else
                    errorValidation = PasswordLevelError.No_Error
                End If
            End If

            Return errorValidation
        End Function

        Private Shared Function ValidateLowSecurityPassword(strPwd As String, validateHistory As Boolean, actualPwd As String, passport As roPassportTicket, strLogin As String, oSecurityOptions As roSecurityOptions) As PasswordLevelError
            Dim errorValidation As PasswordLevelError = PasswordLevelError.Low_Error

            Dim upper As New System.Text.RegularExpressions.Regex("[A-Z]")
            Dim lower As New System.Text.RegularExpressions.Regex("[a-z]")
            Dim number As New System.Text.RegularExpressions.Regex("[0-9]")
            Dim special As New System.Text.RegularExpressions.Regex("[^a-zA-Z0-9]")

            ' La contraseña tiene que tener una longitud minima de 4 caracteres y contener letras y numeros
            ' no coincidir con el nombre de usuario
            ' y que no coincida con las anteriores
            If strPwd.Length > 0 AndAlso strPwd.Length >= 4 AndAlso strPwd <> strLogin AndAlso
                (upper.Matches(strPwd).Count > 0 OrElse lower.Matches(strPwd).Count > 0) AndAlso
                number.Matches(strPwd).Count > 0 Then

                If validateHistory Then
                    If strPwd <> actualPwd AndAlso roSecurityOptions.IsValidPwdHistory(passport.ID, CryptographyHelper.EncryptWithMD5(strPwd), oSecurityOptions.PreviousPasswordsStored, New roSecurityOptionsState) Then
                        errorValidation = PasswordLevelError.No_Error
                    End If
                Else
                    errorValidation = PasswordLevelError.No_Error
                End If
            End If

            Return errorValidation
        End Function

#End Region

    End Class

End Namespace