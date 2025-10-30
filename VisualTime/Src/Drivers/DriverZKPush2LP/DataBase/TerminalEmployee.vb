Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.Comms.Base
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace BusinesProtocol

    Public Class clsTerminalEmployee

        Private _Employee As roEmployee
        Private _EmployeeBus As New Employee.roEmployee
        Private _EmpState As Employee.roEmployeeState = New Employee.roEmployeeState

        Private _ID As Integer
        Private _Name As String = ""
        Private _Card As String = ""
        Private _CardDateStamp As DateTime
        Private _PIN As String = ""
        Private _PINDateStamp As DateTime
        Private _AllowCard As Boolean
        Private _AllowPIN As Boolean
        Private _AllowBio As Boolean
        Private _Finger(10) As Boolean
        Private _FingerData(10)() As Byte
        Private _FingerAlgorithmVersion(10) As String
        Private _FingerDateStamp(10) As DateTime
        Private _Face(1) As Boolean
        Private _FaceData(1)() As Byte
        Private _FaceAlgorithmVersion(1) As String
        Private _FaceDateStamp(1) As DateTime
        Private _Palm(5) As Boolean 'Sólo almacena 1 mano, y para esa mano envían 5 templates
        Private _PalmData(5)() As Byte
        Private _PalmAlgorithmVersion(5) As String
        Private _PalmDateStamp(5) As DateTime

        Private _language As String = ""
        Private mMerge As Generic.List(Of AuthenticationMethodMerge)

        Private NULL_DATE As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

        Private _FingerType As String = "RXFFNG"
        Private _FaceType As String = "ZKUNIFAC" 'Unified templates para biometría en terminales de ZK
        Private _PalmType As String = "ZKUNIPAL"

        Public Sub New()
            _Employee = Nothing
            _EmployeeBus = Nothing
            _ID = 0
            _Name = ""
            _Card = ""
            _PIN = ""
        End Sub

        Public Property ID() As Integer
            Get
                Return _ID
            End Get
            Set(ByVal Value As Integer)
                _ID = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        Public Property Card() As String
            Get
                Return _Card
            End Get
            Set(ByVal Value As String)
                _Card = Value
            End Set
        End Property

        Public Property CardDateStamp As DateTime
            Get
                Return Any2Date(_CardDateStamp)
            End Get
            Set(ByVal value As DateTime)
                _CardDateStamp = value
            End Set
        End Property

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(ByVal value As String)
                _PIN = value
            End Set
        End Property

        Public Property PINDateStamp As DateTime
            Get
                Return Any2Date(_PINDateStamp)
            End Get
            Set(ByVal value As DateTime)
                _PINDateStamp = value
            End Set
        End Property

        Public Property AllowCard() As Boolean
            Get
                Return _AllowCard
            End Get
            Set(ByVal Value As Boolean)
                _AllowCard = Value
            End Set
        End Property

        Public Property AllowBio() As Boolean
            Get
                Return _AllowBio
            End Get
            Set(ByVal Value As Boolean)
                _AllowBio = Value
            End Set
        End Property

        Public Property Language() As String
            Get
                Return _language
            End Get
            Set(ByVal value As String)
                _language = value
            End Set
        End Property

        Public ReadOnly Property HasBio(ByVal Index As Byte) As Boolean
            Get
                Try
                    Return _Finger(Index)
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

        Public Property BioData(ByVal Index As Byte) As Byte()
            Get
                If _Finger(Index) Then
                    Return _FingerData(Index)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Byte())
                _FingerData(Index) = value
                _FingerDateStamp(Index) = Now
            End Set
        End Property

        Public Property BioAlgorithmVersion(ByVal Index As Byte) As String
            Get
                Return roTypes.Any2String(_FingerAlgorithmVersion(Index))
            End Get
            Set(ByVal value As String)
                _FingerAlgorithmVersion(Index) = value
            End Set
        End Property

        Public Property BioTimeStamp(ByVal Index As Byte) As DateTime
            Get
                Return Any2Date(_FingerDateStamp(Index))
            End Get
            Set(ByVal value As DateTime)
                _FingerDateStamp(Index) = value
            End Set
        End Property

        Public ReadOnly Property HasFace(ByVal Index As Byte) As Boolean
            Get
                Try
                    Return _Face(Index)
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

        Public Property BioFaceData(ByVal Index As Byte) As Byte()
            Get
                If _Face(Index) Then
                    Return _FaceData(Index)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Byte())
                _FaceData(Index) = value
                _FaceDateStamp(Index) = Now
            End Set
        End Property

        Public Property BioFaceAlgorithmVersion(ByVal Index As Byte) As String
            Get
                Return roTypes.Any2String(_FaceAlgorithmVersion(Index))
            End Get
            Set(ByVal value As String)
                _FaceAlgorithmVersion(Index) = value
            End Set
        End Property

        Public Property BioFaceTimeStamp(ByVal Index As Byte) As DateTime
            Get
                Return Any2Date(_FaceDateStamp(Index))
            End Get
            Set(ByVal value As DateTime)
                _FaceDateStamp(Index) = value
            End Set
        End Property

        Public ReadOnly Property HasPalm(ByVal Index As Byte) As Boolean
            Get
                Try
                    Return _Palm(Index)
                Catch ex As Exception
                    Return False
                End Try
            End Get
        End Property

        Public Property BioPalmData(ByVal Index As Byte) As Byte()
            Get
                If _Palm(Index) Then
                    Return _PalmData(Index)
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As Byte())
                _PalmData(Index) = value
                _PalmDateStamp(Index) = Now
            End Set
        End Property

        Public Property BioPalmAlgorithmVersion(ByVal Index As Byte) As String
            Get
                Return roTypes.Any2String(_PalmAlgorithmVersion(Index))
            End Get
            Set(ByVal value As String)
                _PalmAlgorithmVersion(Index) = value
            End Set
        End Property

        Public Property BioPalmTimeStamp(ByVal Index As Byte) As DateTime
            Get
                Return Any2Date(_PalmDateStamp(Index))
            End Get
            Set(ByVal value As DateTime)
                _PalmDateStamp(Index) = value
            End Set
        End Property

        Public ReadOnly Property EmployeeBus As roEmployee
            Get
                Return _Employee
            End Get
        End Property

        Public Function HasActiveContract() As Boolean
            Try
                Dim sSQL As String = "@SELECT# IDEmployee from sysrosubvwCurrentEmployeePeriod where idemployee=" + _ID.ToString

                Return roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsEmployeeCustom::HasActiveContract:Error:", ex)
                Return False
            End Try
        End Function

        Public Function HasFieldPermision() As Boolean
            Try
                Dim EmpState As UserFields.roUserFieldState = New UserFields.roUserFieldState
                Dim EmpFld As UserFields.roEmployeeUserField = New UserFields.roEmployeeUserField
                EmpFld = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(_ID, "Entra", Now, EmpState)

                If Not EmpFld Is Nothing Then
                    Return Not EmpFld.FieldValue.ToString = "N"
                Else
                    Return True
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsEmployeeCustom::HasFieldPermision:Error:", ex)
                Return True
            End Try
        End Function

        Public Function getFieldMotive() As String
            Try
                Dim EmpState As UserFields.roUserFieldState = New UserFields.roUserFieldState
                Dim EmpFld As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(_ID, "Motivo", Now, EmpState)

                Return EmpFld.FieldValue.ToString
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsEmployeeCustom::getFieldMotive:Error:", ex)
                Return ""
            End Try
        End Function

        Public Function Load(ByVal ID As Integer, Optional ByVal card As String = "") As Boolean

            Try
                Dim IDEmployee As Integer = 0

                'Si nos pasan tarjeta la buscamos antes
                _Employee = New roEmployee
                If card.Length > 0 Then
                    Select Case clsParameters.CardCode
                        Case clsParameters.eCardCode.Numeric
                            IDEmployee = _Employee.GetEmployeeIDFromCardIDHex(roLog.GetInstance(), Hex(card))
                        Case Else
                            IDEmployee = _Employee.GetEmployeeIDFromCardIDV1(roLog.GetInstance(), card)
                    End Select
                Else
                    IDEmployee = ID
                End If
                _Employee.ID = IDEmployee
                _Employee.Load(roLog.GetInstance())

                'Si es un empleado valido lo carga
                If IDEmployee > 0 Then
                    Return LoadLive(IDEmployee)
                Else
                    Return False
                End If
            Catch ex As Exception
                Return False
            Finally

            End Try
        End Function

        Public Function EmployeeExists(ByVal ID As Integer) As Integer
            Try
                Dim sSQL As String = "@SELECT# count(*) from Employees where id =" + ID.ToString

                Return roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) > 0
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::EmployeeExists:Error:", ex)
                Return False
            End Try
        End Function

        Private Function LoadLive(ByVal IDEmployee As Integer) As Boolean
            Dim oEmployeesState As Employee.roEmployeeState

            Try

                Try
                    oEmployeesState = New Employee.roEmployeeState
                    _EmployeeBus = Employee.roEmployee.GetEmployee(IDEmployee, oEmployeesState)
                Catch ex As Exception
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::LoadLiveBus:{_ID}: ", ex)
                End Try
                Try
                    Dim oPassport As roPassport = roPassportManager.GetPassport(IDEmployee, LoadType.Employee, New roSecurityState(-1))
                    _ID = IDEmployee
                    If oPassport IsNot Nothing Then
                        _Name = oPassport.Name
                        Try
                            If oPassport.AuthenticationMethods.PinRow IsNot Nothing AndAlso
                               oPassport.AuthenticationMethods.PinRow.Enabled Then
                                _PIN = roConversions.Any2String(oPassport.AuthenticationMethods.PinRow.Password)
                                _AllowPIN = True
                                If oPassport.AuthenticationMethods.PinRow.TimeStamp = DateTime.MinValue Then
                                    _PINDateStamp = roTypes.Any2DateTime(oPassport.AuthenticationMethods.PinRow.TimeStamp)
                                End If
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::LoadLive:LoadPin:{_ID}: ", ex)
                        End Try

                        Try
                            If oPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Any AndAlso
                               oPassport.AuthenticationMethods.CardRows(0).Enabled Then
                                _Card = oPassport.AuthenticationMethods.CardRows(0).Credential
                                If _Card <> "0" AndAlso _Card.Length > 0 Then
                                    ' Miramos si existe un valor en la tabla CardAliases
                                    Dim strSQL As String = $"@SELECT# RealValue FROM CardAliases WHERE IDCard = {_Card}"
                                    Dim tbCardAliases As DataTable = CreateDataTable(strSQL)
                                    If tbCardAliases IsNot Nothing AndAlso tbCardAliases.Rows.Count > 0 Then
                                        _Card = roTypes.Any2String(tbCardAliases.Rows(0).Item("RealValue"))
                                    End If
                                End If
                                _AllowCard = True
                                If oPassport.AuthenticationMethods.CardRows(0).TimeStamp = DateTime.MinValue Then
                                    _CardDateStamp = roTypes.Any2DateTime(oPassport.AuthenticationMethods.CardRows(0).TimeStamp)
                                End If
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::LoadLive:LoadCard:{_ID}: ", ex)
                        End Try

                        If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.BiometricRows.Any Then
                            'Si es un terminal de huella las carga
                            ' Si el tipo de huella no está informado se trata del tipo por defecto (huella de ANVIZ -> RXA100)
                            If _FingerType = "" Then _FingerType = "RXA100"
                            For Each oBioRow As roPassportAuthenticationMethodsRow In oPassport.AuthenticationMethods.BiometricRows
                                If oBioRow IsNot Nothing AndAlso oBioRow.Enabled Then
                                    _AllowBio = True
                                    Select Case oBioRow.Version
                                        Case _FingerType
                                            ' Biometría dedo
                                            _Finger(oBioRow.BiometricID) = oBioRow.BiometricData.Length > 0
                                            _FingerData(oBioRow.BiometricID) = oBioRow.BiometricData
                                            _FingerDateStamp(oBioRow.BiometricID) = oBioRow.TimeStamp
                                        Case _FaceType
                                            ' Cargamos biometría facial (formato unificada)
                                            _Face(oBioRow.BiometricID) = oBioRow.BiometricData.Length > 0
                                            _FaceData(oBioRow.BiometricID) = oBioRow.BiometricData
                                            _FaceDateStamp(oBioRow.BiometricID) = oBioRow.TimeStamp
                                            _FaceAlgorithmVersion(oBioRow.BiometricID) = oBioRow.BiometricAlgorithm
                                        Case _PalmType
                                            ' Cargamos biometría palma de la mano (formato unificada)
                                            _Palm(oBioRow.BiometricID) = oBioRow.BiometricData.Length > 0
                                            _PalmData(oBioRow.BiometricID) = oBioRow.BiometricData
                                            _PalmDateStamp(oBioRow.BiometricID) = oBioRow.TimeStamp
                                            _PalmAlgorithmVersion(oBioRow.BiometricID) = oBioRow.BiometricAlgorithm
                                    End Select
                                End If
                            Next
                        End If

                        _language = oPassport.Language.Key

                        Return True
                    Else
                        'No tiene contrato activo
                        Return False
                    End If
                    oPassport = Nothing
                    Return False
                Catch ex As Exception
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::LoadLive:" + _ID.ToString + ": ", ex)
                    Return False
                End Try
            Catch ex As Exception
                Return False
            Finally

            End Try

        End Function

        Public Function SaveFinger(ByVal IDFinger As Byte, ByVal iIdTerminal As Integer) As Boolean
            Try
                Dim oPassport As roPassport = roPassportManager.GetPassport(_ID, LoadType.Employee, New roSecurityState(-1))
                If oPassport IsNot Nothing Then
                    Dim oPassportManager As roPassportManager = New roPassportManager
                    Dim oMethod As roPassportAuthenticationMethodsRow = Nothing

                    If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing Then
                        oMethod = oPassport.AuthenticationMethods.BiometricRows.ToList.Find(Function(x) x.Version = _FingerType AndAlso x.BiometricID = IDFinger)
                    End If

                    If oMethod IsNot Nothing Then
                        'ACTUALIZO HUELLA EXISTENTE

                        oMethod.BiometricData = _FingerData(IDFinger)
                        oMethod.TimeStamp = _FingerDateStamp(IDFinger)
                        oMethod.BiometricTerminalId = iIdTerminal
                        oMethod.RowState = RowState.UpdateRow

                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SaveFinger:Update finger in employee({_ID},{IDFinger})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SaveFinger:Error updating finger in employee({_ID},{IDFinger})")
                            Return False
                        End If

                        Return True
                    Else
                        ' NUEVA HUELLA
                        oMethod = New roPassportAuthenticationMethodsRow
                        oMethod.IDPassport = oPassport.ID
                        oMethod.Method = AuthenticationMethod.Biometry
                        oMethod.Credential = String.Empty
                        oMethod.Password = String.Empty
                        oMethod.BiometricID = IDFinger
                        oMethod.BiometricTerminalId = iIdTerminal
                        oMethod.BiometricAlgorithm = String.Empty
                        oMethod.BiometricData = _FingerData(IDFinger)
                        oMethod.TimeStamp = _FingerDateStamp(IDFinger)
                        oMethod.Enabled = True
                        oMethod.Version = _FingerType
                        oMethod.RowState = RowState.NewRow

                        Dim oBiometricRow As List(Of roPassportAuthenticationMethodsRow) = New List(Of roPassportAuthenticationMethodsRow)
                        oBiometricRow.Add(oMethod)
                        If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing Then
                            Dim oCurrentBiometricRows As New List(Of roPassportAuthenticationMethodsRow)
                            oCurrentBiometricRows = oPassport.AuthenticationMethods.BiometricRows.ToList
                            oPassport.AuthenticationMethods.BiometricRows = oCurrentBiometricRows.Append(oMethod).ToArray
                        Else
                            oPassport.AuthenticationMethods.BiometricRows = oBiometricRow.ToArray
                        End If

                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SaveFinger:Save finger in employee({_ID},{IDFinger})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SaveFinger:Error saving finger in employee({_ID},{IDFinger})")
                            Return False
                        End If

                        Return True
                    End If
                Else
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, $"clsTerminalEmployee::SaveFingerOnDBLive:Unable to load passport for employee {_ID}, IdFinger = {IDFinger}")
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::SaveFingerOnDBLive : ", ex)
                Return False
            End Try
        End Function

        Public Function SaveBiodata(ByVal idBiodata As Byte, ByVal sBioDataType As String, ByVal sAlgorithmVersion As String, ByVal iIdTerminal As Integer) As Boolean
            Try
                Dim sVersion As String = String.Empty
                Dim oPassport As roPassport = roPassportManager.GetPassport(_ID, LoadType.Employee, New roSecurityState(-1))
                If oPassport IsNot Nothing Then

                    Dim aBioData As Byte() = Nothing
                    Dim dTimestamp As Date
                    Select Case sBioDataType
                        Case "9"
                            aBioData = _FaceData(idBiodata)
                            dTimestamp = _FaceDateStamp(idBiodata)
                            sVersion = "ZKUNIFAC"
                        Case "8"
                            aBioData = _PalmData(idBiodata)
                            dTimestamp = _PalmDateStamp(idBiodata)
                            sVersion = "ZKUNIPAL"
                    End Select

                    If aBioData IsNot Nothing Then
                        Dim oMethod As roPassportAuthenticationMethodsRow = Nothing
                        Dim oPassportManager As roPassportManager = New roPassportManager

                        If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing Then
                            oMethod = oPassport.AuthenticationMethods.BiometricRows.ToList.Find(Function(x) x.Version = sVersion AndAlso x.BiometricID = idBiodata)
                        End If

                        If oMethod IsNot Nothing Then
                            'ACTUALIZO BIOMETRIA EXISTENTE
                            oMethod.BiometricData = aBioData
                            oMethod.TimeStamp = dTimestamp
                            oMethod.BiometricTerminalId = iIdTerminal
                            oMethod.BiometricAlgorithm = sAlgorithmVersion
                            oMethod.Version = sVersion
                            oMethod.RowState = RowState.UpdateRow

                            If oPassportManager.Save(oPassport) Then
                                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SaveFinger:Update finger in employee({_ID},{idBiodata})")
                            Else
                                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SaveFinger:Error updating finger in employee({_ID},{idBiodata})")
                                Return False
                            End If

                            Return True
                        Else
                            ' NUEVA BIOMETRIA
                            oMethod = New roPassportAuthenticationMethodsRow
                            oMethod.IDPassport = oPassport.ID
                            oMethod.Method = AuthenticationMethod.Biometry
                            oMethod.Version = sVersion
                            oMethod.Credential = String.Empty
                            oMethod.Password = String.Empty
                            oMethod.BiometricID = idBiodata
                            oMethod.BiometricTerminalId = iIdTerminal
                            oMethod.BiometricAlgorithm = sAlgorithmVersion
                            oMethod.BiometricData = aBioData
                            oMethod.TimeStamp = dTimestamp
                            oMethod.Enabled = True
                            oMethod.RowState = RowState.NewRow
                            Dim oBiometricRow As List(Of roPassportAuthenticationMethodsRow) = New List(Of roPassportAuthenticationMethodsRow)
                            oBiometricRow.Add(oMethod)
                            If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing Then
                                Dim oCurrentBiometricRows As New List(Of roPassportAuthenticationMethodsRow)
                                oCurrentBiometricRows = oPassport.AuthenticationMethods.BiometricRows.ToList
                                oPassport.AuthenticationMethods.BiometricRows = oCurrentBiometricRows.Append(oMethod).ToArray
                            Else
                                oPassport.AuthenticationMethods.BiometricRows = oBiometricRow.ToArray
                            End If

                            If oPassportManager.Save(oPassport) Then
                                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SaveFinger:Save finger in employee({_ID},{idBiodata})")
                            Else
                                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SaveFinger:Error saving finger in employee({_ID},{idBiodata})")
                                Return False
                            End If

                            Return True
                        End If
                    Else
                        roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, $"clsTerminalEmployee::SaveBiodata:Unable to save biodata {sVersion}  in employee ({_ID},{idBiodata})")
                        Return False
                    End If
                Else
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SaveBiodata:Unable to load passport for employee {_ID}, IdBiodata = {idBiodata}, BioType = {sVersion}")
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::SaveBiodata : ", ex)
                Return False
            End Try
        End Function

        Public Function SaveCard(ByVal scard As String) As Boolean
            Try

                Dim oPassport As roPassport = roPassportManager.GetPassport(_ID, LoadType.Employee, New roSecurityState(-1))

                If oPassport IsNot Nothing Then
                    Dim oPassportManager As roPassportManager = New roPassportManager
                    Dim oMethod As roPassportAuthenticationMethodsRow = Nothing
                    If oPassport.AuthenticationMethods.CardRows IsNot Nothing Then
                        oMethod = oPassport.AuthenticationMethods.CardRows.ToList.Find(Function(x) x.Version = String.Empty AndAlso x.BiometricID = 0)
                    End If

                    If oMethod IsNot Nothing Then
                        'ACTUALIZO TARJETA EXISTENTE
                        oMethod.Credential = scard
                        oMethod.Enabled = True
                        oMethod.TimeStamp = If(_CardDateStamp = DateTime.MinValue, Now, _CardDateStamp)
                        oMethod.RowState = RowState.UpdateRow

                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SaveCard:Update card for employee({_ID},{scard})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SaveCard:Error updating card for employee({_ID},{scard})")
                            Return False
                        End If

                        Return True
                    Else
                        ' NUEVA TARJETA
                        oMethod = New roPassportAuthenticationMethodsRow
                        oMethod.IDPassport = oPassport.ID
                        oMethod.Method = AuthenticationMethod.Card
                        oMethod.Version = String.Empty
                        oMethod.BiometricID = 0
                        oMethod.Password = String.Empty
                        oMethod.Credential = scard
                        oMethod.Enabled = True
                        oMethod.TimeStamp = If(_CardDateStamp = DateTime.MinValue, Now, _CardDateStamp)
                        oMethod.RowState = RowState.NewRow
                        Dim oCardRow As List(Of roPassportAuthenticationMethodsRow) = New List(Of roPassportAuthenticationMethodsRow)
                        oCardRow.Add(oMethod)
                        oPassport.AuthenticationMethods.CardRows = oCardRow.ToArray

                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SaveCard:Save card for employee({_ID},{scard})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SaveCard:Error saving card for employee({_ID},{scard})")
                            Return False
                        End If

                        Return True
                    End If
                Else
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, $"clsTerminalEmployee::SaveCard:Unable to load passport for employee {_ID}, Card = {scard}")
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::SaveCard : ", ex)
                Return False
            End Try
        End Function

        Public Function SavePIN(ByVal sPin As String) As Boolean
            Try
                Dim oPassport As roPassport = roPassportManager.GetPassport(_ID, LoadType.Employee, New roSecurityState(-1))

                If oPassport IsNot Nothing Then
                    Dim oPassportManager As roPassportManager = New roPassportManager
                    Dim oMethod As roPassportAuthenticationMethodsRow = oPassport.AuthenticationMethods.PinRow

                    If oMethod IsNot Nothing Then
                        'ACTUALIZO PIN EXISTENTE
                        oMethod.Password = sPin
                        oMethod.Enabled = True
                        oMethod.Version = ""
                        oMethod.TimeStamp = _PINDateStamp
                        oMethod.Credential = String.Empty
                        oMethod.RowState = RowState.UpdateRow

                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SavePIN:Update PIN for employee({_ID},{sPin})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SavePIN:Error updating PIN for employee({_ID},{sPin})")
                            Return False
                        End If

                        Return True
                    Else
                        ' NUEVO PIN
                        oMethod = New roPassportAuthenticationMethodsRow

                        oMethod.IDPassport = oPassport.ID
                        oMethod.Method = AuthenticationMethod.Pin
                        oMethod.Version = ""
                        oMethod.Credential = String.Empty
                        oMethod.Password = sPin
                        oMethod.BiometricID = 0
                        oMethod.Enabled = True

                        oMethod.TimeStamp = _PINDateStamp
                        oMethod.RowState = RowState.NewRow
                        oPassport.AuthenticationMethods.PinRow = oMethod


                        If oPassportManager.Save(oPassport) Then
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roInfo, $"clsTerminalEmployee::SavePIN:Save PIN for employee({_ID},{sPin})")
                        Else
                            roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, $"clsTerminalEmployee::SavePIN:Error saving PIN for employee({_ID},{sPin})")
                            Return False
                        End If

                        Return True
                    End If
                Else
                    roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, $"clsTerminalEmployee::SavePIN:Unable to load passport for employee {_ID}, Card = {sPin}")
                    Return False
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::SavePin : ", ex)
                Return False
            End Try
        End Function

    End Class

End Namespace