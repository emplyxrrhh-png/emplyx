Imports System.Security.AccessControl
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees
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
        Private _PIN As String = ""
        Private _AllowCard As Boolean
        Private _AllowPIN As Boolean
        Private _AllowBio As Boolean
        Private _Finger(10) As Boolean
        Private _FingerData(10)() As Byte
        Private _FingerDateStamp(10) As DateTime
        Private _language As String = ""
        Private mMerge As Generic.List(Of AuthenticationMethodMerge)

        Private NULL_DATE As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

        Private _FingerType As String = "RXFFNG"

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

        Public Property PIN() As String
            Get
                Return _PIN
            End Get
            Set(ByVal value As String)
                _PIN = value
            End Set
        End Property

        Public Function EmployeeExists(ByVal ID As Integer) As Integer
            Try
                Dim sSQL As String = "@SELECT# count(*) from Employees where id =" + ID.ToString

                Return roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) > 0
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::EmployeeExists:Error:", ex)
                Return False
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
            End Try
        End Function

        Private Function LoadLive(ByVal IDEmployee As Integer) As Boolean
            Dim oEmployeesState As Employee.roEmployeeState
            Try
                oEmployeesState = New Employee.roEmployeeState
                _EmployeeBus = Employee.roEmployee.GetEmployee(IDEmployee, oEmployeesState)
            Catch ex As Exception
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::LoadLiveBus:" + _ID.ToString + ": ", ex)
            End Try
            Try
                Dim oPassport As New roPassport
                oPassport = roPassportManager.GetPassport(IDEmployee, LoadType.Employee)
                _ID = IDEmployee
                If oPassport IsNot Nothing Then
                    _Name = oPassport.Name
                    Try
                        If oPassport.AuthenticationMethods.PinRow IsNot Nothing AndAlso
                           oPassport.AuthenticationMethods.PinRow.Enabled Then
                            _PIN = Robotics.VTBase.roConversions.Any2String(oPassport.AuthenticationMethods.PinRow.Password)
                            _AllowPIN = True
                        End If
                    Catch ex As Exception
                        roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::LoadLive:LoadPin:" + _ID.ToString + ": ", ex)
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
                        End If
                    Catch ex As Exception
                        roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::LoadLive:LoadCard:" + _ID.ToString + ": ", ex)
                    End Try

                    If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.BiometricRows.Any AndAlso oPassport.AuthenticationMethods.BiometricRows.ToList.FindAll(Function(x) x.Enabled = 1).Any Then
                        _AllowBio = True
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
                roLog.GetInstance.logMessage(VTBase.roLog.EventType.roError, "clsTerminalEmployee::LoadLive:" + _ID.ToString + ": ", ex)
                Return False
            End Try

        End Function

    End Class

End Namespace