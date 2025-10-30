Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Base


    <DataContract>
    <Serializable>
    Public Class roBusinessState
        Inherits roState

#Region "Declarations - Constructor"

        Private strLanguageFileName As String

        Public Sub New(ByVal _ClassNameLog As String, ByVal _LanguageFileName As String, Optional ByVal _IDPassport As Integer = -1, Optional ByVal _Context As System.Web.HttpContext = Nothing, Optional ByVal _ClientAddress As String = "")
            MyBase.New(_IDPassport, _ClassNameLog, _Context, _ClientAddress)
            Me.strLanguageFileName = _LanguageFileName
        End Sub

#End Region

#Region "Properties"

        Public Shadows Function Audit(ByVal _Action As VTBase.Audit.Action, ByVal _ObjectType As VTBase.Audit.ObjectType, ByVal _ObjectName As String, ByVal _Parameters As DataTable, ByVal _SessionID As Integer) As Integer

            Dim strUserName As String = ""
            strUserName = GetPassport().Name

            Return MyBase.Audit(strUserName, _Action, _ObjectType, _ObjectName, _Parameters, _SessionID)

        End Function


#End Region

#Region "Methods"

        'Public Overrides Sub UpdateAccessTime()

        'End Sub
        Public Overrides Sub LoadLanguage()
            If Me.oLanguage Is Nothing Then
                Me.oLanguage = New roLanguage()
                Dim oPassport As roPassportTicket = Me.GetPassport()
                If oPassport IsNot Nothing Then
                    Me.oLanguage.SetLanguageReference(Me.strLanguageFileName, GetPassport().Language.Key)
                Else
                    Me.oLanguage.SetLanguageReference(Me.strLanguageFileName, "ESP")
                End If
            End If
        End Sub
        Public Overrides Sub LoadPassport()

            Try
                Me.oPassport = VTBase.roConstants.GetLoggedInPassportTicket()

                If Me.oPassport Is Nothing Then
                    Me.oPassport = roPassportManager.GetPassportTicket(Me.IDPassport, LoadType.Passport)

                    If Me.oPassport Is Nothing Then
                        Me.oPassport = New roPassportTicket With {
                            .Name = "Unkown",
                            .AuthCredential = "unkown",
                            .Language = New roPassportLanguage With {
                                    .Culture = "es-ES",
                                    .ID = 1,
                                    .Installed = True,
                                    .Key = "ESP",
                                    .ParametersXml = "<?xml version=""1.0""?><roCollection version=""2.0""><Item key=""ExtLanguage"" type=""8"">sp</Item><Item key=""ExtDatePickerFormat"" type=""8"">d/m/Y</Item><Item key=""ExtDatePickerStartDay"" type=""8"">1</Item></roCollection>"
                                }
                        }
                    End If

                End If
            Catch
            End Try

        End Sub

        ''' <summary>
        ''' Devuelve el tipo de passport ('U' o 'E') en función de la aplicación actual.
        ''' </summary>
        ''' <returns>Devuelve 'U' si se trata de un passport de usuario o 'E' si se trata de un passport de empleado.</returns>
        ''' <remarks>Utiliza el SessionID para determinar la aplicación desde la que se está accediendo.</remarks>
        Public Function ActivePassportType(Optional ByRef _IDPassportEmployee As Integer = -1) As String

            Dim strRet As String = "U"
            Dim _EmployeeApplications As String() = {"LivePortal", "VTAnywhere", "VTPortal", "VTPortal.TimeGate"}
            Dim _EmployeeNoSessionApplications As String() = {"VTAnywhere", "VTPortal", "VTPortal.TimeGate"}

            If Me.IDPassport > 0 Then

                If Me.SessionID <> "" Then
                    For Each strEmployeeApp As String In _EmployeeApplications
                        If Me.SessionID.ToLower.EndsWith("*" & strEmployeeApp.ToLower) Then
                            strRet = "E"
                            Exit For
                        End If
                    Next
                Else
                    For Each strEmployeeApp As String In _EmployeeNoSessionApplications
                        If Me.ClientAddress.ToLower.EndsWith(strEmployeeApp.ToLower) Then
                            strRet = "E"
                            Exit For
                        End If
                    Next
                End If

                _IDPassportEmployee = -1

                If strRet = "E" Then
                    ' Verificamos que el passport tenga un empleado asignado
                    Dim tmpPassport As roPassportTicket = GetPassport()
                    If (tmpPassport.IDEmployee.HasValue AndAlso tmpPassport.IDEmployee > 0) Then
                        _IDPassportEmployee = tmpPassport.IDEmployee
                    End If
                End If

            End If

            Return strRet

        End Function

#Region "helper methods"

        Public Shared Sub CopyTo(ByVal oSrcState As roBusinessState, ByVal oDstState As roBusinessState)

            With oDstState
                .IDPassport = oSrcState.IDPassport
                .Context = oSrcState.Context
                .ClientAddress = oSrcState.ClientAddress
                .SessionID = oSrcState.SessionID
                Try
                    CType(oDstState, Object).Result = CType(oSrcState, Object).Result
                Catch
                End Try
                .ErrorText = oSrcState.ErrorText
                .ErrorDetail = oSrcState.ErrorDetail
                .ErrorNumber = oSrcState.ErrorNumber
            End With

        End Sub

        Public Shared Sub CopyTo(ByVal oSrcState As roBaseState, ByVal oDstState As roBaseState)

            With oDstState
                .IDPassport = oSrcState.IDPassport
                .Context = oSrcState.Context
                .ClientAddress = oSrcState.ClientAddress
                .SessionID = oSrcState.SessionID
                Try
                    CType(oDstState, Object).Result = CType(oSrcState, Object).Result
                Catch
                End Try
                .ErrorText = oSrcState.ErrorText
                .ErrorDetail = oSrcState.ErrorDetail
                .ErrorNumber = oSrcState.ErrorNumber
            End With

        End Sub


#End Region

#End Region

    End Class
End Namespace