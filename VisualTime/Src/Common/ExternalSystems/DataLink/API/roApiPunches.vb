Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink



    Public Class roApiPunches
        Inherits roDataLinkApi


        Protected ReadOnly Property ImportEngine As roEmployeeImport
            Get
                Return CType(Me.oDataImport, roEmployeeImport)
            End Get
        End Property


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roEmployeeImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If
        End Sub



        Public Function GetPunches(ByVal oPunchFilterType As PunchFilterType, ByVal Timestamp As DateTime, ByVal StartDate As Date, ByVal EndDate As Date, ByRef lPunches As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef strErrorMsg As String, ByRef iReturnCode As Integer, Optional ByVal EmployeeID As String = "") As Boolean
            Dim bolRet As Boolean = False

            Try

                lPunches = New Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch)
                bolRet = True

                If bolRet Then
                    ' Obtenemos los fichajes a partir del timestamp indicado  (max 1000 registros)
                    Dim aState As New Punch.roPunchState
                    Dim PunchesDt As DataTable = Punch.roPunch.GetPunchesExternalAPI(oPunchFilterType, Timestamp, StartDate, EndDate, aState, EmployeeID)
                    If PunchesDt IsNot Nothing AndAlso PunchesDt.Rows.Count > 0 Then

                        Dim importKeyFieldName As String = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value)
                        If importKeyFieldName.Length = 0 Then
                            importKeyFieldName = "NIF"
                        End If

                        Dim dtCauses = CreateDataTable("@SELECT# id,Name,isnull(ShortName,'') as ShortName from causes", "Causes")

                        Dim oEmployeeUSR As New VTUserFields.UserFields.roEmployeeUserField

                        For Each oRow As DataRow In PunchesDt.Rows
                            Dim oDatalinkStandarPunch As New RoboticsExternAccess.roDatalinkStandardPunch

                            oEmployeeUSR = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oRow("IDEmployee"), importKeyFieldName, Now, New VTUserFields.UserFields.roUserFieldState(-1), False)

                            If oEmployeeUSR IsNot Nothing Then
                                oDatalinkStandarPunch.UniqueEmployeeID = oEmployeeUSR.FieldValue
                                If importKeyFieldName = "NIF" Then
                                    oDatalinkStandarPunch.NifEmpleado = oEmployeeUSR.FieldValue
                                End If
                            End If

                            If importKeyFieldName <> "NIF" Then
                                oEmployeeUSR = VTUserFields.UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oRow("IDEmployee"), "NIF", Now, New VTUserFields.UserFields.roUserFieldState(-1), False)
                                If oEmployeeUSR IsNot Nothing Then
                                    oDatalinkStandarPunch.NifEmpleado = oEmployeeUSR.FieldValue
                                End If
                            End If
                            If Not IsDBNull(oRow("ID")) Then
                                oDatalinkStandarPunch.ID = Any2String(oRow("ID"))
                            End If
                            oDatalinkStandarPunch.Type = oRow("Type")
                            oDatalinkStandarPunch.ActualType = oRow("ActualType")
                            oDatalinkStandarPunch.ShiftDate = oRow("ShiftDate")
                            oDatalinkStandarPunch.DateTime = oRow("DateTime")
                            If Not IsDBNull(oRow("IDTerminal")) Then
                                oDatalinkStandarPunch.IDTerminal = Any2Integer(oRow("IDTerminal"))
                            End If

                            If Not IsDBNull(oRow("TypeData")) AndAlso Any2Integer(oRow("TypeData")) > 0 Then
                                Dim dRows As DataRow() = dtCauses.Select("ID= " & oRow("TypeData").ToString)
                                If dRows.Length > 0 Then
                                    oDatalinkStandarPunch.TypeData = Any2String(dRows(0)("ShortName"))
                                End If
                            End If

                            If Not IsDBNull(oRow("Location")) Then
                                oDatalinkStandarPunch.GPS = Any2String(oRow("Location"))
                            End If
                            If Not IsDBNull(oRow("Field1")) Then
                                oDatalinkStandarPunch.Field1 = Any2String(oRow("Field1"))
                            End If
                            If Not IsDBNull(oRow("Field2")) Then
                                oDatalinkStandarPunch.Field2 = Any2String(oRow("Field2"))
                            End If
                            If Not IsDBNull(oRow("Field3")) Then
                                oDatalinkStandarPunch.Field3 = Any2Double(oRow("Field3"))
                            End If
                            If Not IsDBNull(oRow("Field4")) Then
                                oDatalinkStandarPunch.Field4 = Any2Double(oRow("Field4"))
                            End If
                            If Not IsDBNull(oRow("Field5")) Then
                                oDatalinkStandarPunch.Field5 = Any2Double(oRow("Field5"))
                            End If
                            If Not IsDBNull(oRow("Field6")) Then
                                oDatalinkStandarPunch.Field6 = Any2Double(oRow("Field6"))
                            End If

                            If Not IsDBNull(oRow("TimeStamp")) Then
                                oDatalinkStandarPunch.Timestamp = oRow("TimeStamp")
                            End If

                            ' Añadimos el fichaje a la lista
                            lPunches.Add(oDatalinkStandarPunch)
                        Next

                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK

                    End If
                End If

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetPunches")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Function GetPunchesEx(ByVal oPunchFilterType As PunchFilterType, ByVal Timestamp As DateTime, ByVal StartDate As Date, ByVal EndDate As Date, ByRef lPunches As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef strErrorMsg As String, ByRef iReturnCode As Integer, Optional ByVal EmployeeID As String = "") As Boolean
            Dim bolRet As Boolean = False

            Try

                lPunches = New Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch)
                bolRet = True

                If bolRet Then
                    ' Obtenemos los fichajes a partir del timestamp indicado  (max 1000 registros)
                    Dim aState As New Punch.roPunchState
                    Dim PunchesDt As DataTable = Punch.roPunch.GetPunchesExternalAPIEx(oPunchFilterType, Timestamp, StartDate, EndDate, aState, EmployeeID)
                    If PunchesDt IsNot Nothing AndAlso PunchesDt.Rows.Count > 0 Then

                        For Each oRow As DataRow In PunchesDt.Rows
                            Dim oDatalinkStandarPunch As New roDatalinkStandardPunch

                            If Not IsDBNull(oRow("ID")) Then
                                oDatalinkStandarPunch.ID = Any2String(oRow("ID"))
                            End If
                            If Not IsDBNull(oRow("IdImport")) Then
                                oDatalinkStandarPunch.UniqueEmployeeID = Any2String(oRow("IdImport"))
                            End If
                            If Not IsDBNull(oRow("NIF")) Then
                                oDatalinkStandarPunch.NifEmpleado = Any2String(oRow("NIF"))
                            End If
                            If Not IsDBNull(oRow("Type")) Then
                                oDatalinkStandarPunch.Type = oRow("Type")
                            End If
                            If Not IsDBNull(oRow("ActualType")) Then
                                oDatalinkStandarPunch.ActualType = oRow("ActualType")
                            End If
                            If Not IsDBNull(oRow("ShiftDate")) Then
                                oDatalinkStandarPunch.ShiftDate = oRow("ShiftDate")
                            End If
                            If Not IsDBNull(oRow("DateTime")) Then
                                oDatalinkStandarPunch.DateTime = oRow("DateTime")
                            End If
                            If Not IsDBNull(oRow("IDTerminal")) Then
                                oDatalinkStandarPunch.IDTerminal = Any2Integer(oRow("IDTerminal"))
                            End If
                            If Not IsDBNull(oRow("ShortName")) Then
                                oDatalinkStandarPunch.TypeData = Any2String(oRow("ShortName"))
                            End If
                            If Not IsDBNull(oRow("Location")) Then
                                oDatalinkStandarPunch.GPS = Any2String(oRow("Location"))
                            End If
                            If Not IsDBNull(oRow("Field1")) Then
                                oDatalinkStandarPunch.Field1 = Any2String(oRow("Field1"))
                            End If
                            If Not IsDBNull(oRow("Field2")) Then
                                oDatalinkStandarPunch.Field2 = Any2String(oRow("Field2"))
                            End If
                            If Not IsDBNull(oRow("Field3")) Then
                                oDatalinkStandarPunch.Field3 = Any2Double(oRow("Field3"))
                            End If
                            If Not IsDBNull(oRow("Field4")) Then
                                oDatalinkStandarPunch.Field4 = Any2Double(oRow("Field4"))
                            End If
                            If Not IsDBNull(oRow("Field5")) Then
                                oDatalinkStandarPunch.Field5 = Any2Double(oRow("Field5"))
                            End If
                            If Not IsDBNull(oRow("Field6")) Then
                                oDatalinkStandarPunch.Field6 = Any2Double(oRow("Field6"))
                            End If

                            If Not IsDBNull(oRow("TimeStamp")) Then
                                oDatalinkStandarPunch.Timestamp = oRow("TimeStamp")
                            End If

                            ' Añadimos el fichaje a la lista
                            lPunches.Add(oDatalinkStandarPunch)
                        Next

                        iReturnCode = Core.DTOs.ReturnCode._OK

                    End If
                End If

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetPunches")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

        Public Function GetPunchesWithIDEx(ByVal lIDPunches As List(Of String), ByRef lPunches As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try
                lPunches = New Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch)
                bolRet = True

                If bolRet Then
                    ' Obtenemos los fichajes a partir del timestamp indicado  (max 1000 registros)
                    Dim aState As New Punch.roPunchState
                    Dim PunchesDt As DataTable = Punch.roPunch.GetPunchesWithIDExternalAPIEx(lIDPunches, aState)
                    If PunchesDt IsNot Nothing AndAlso PunchesDt.Rows.Count > 0 Then

                        For Each oRow As DataRow In PunchesDt.Rows
                            Dim oDatalinkStandardPunch As New roDatalinkStandardPunch

                            If Not IsDBNull(oRow("ID")) Then
                                oDatalinkStandardPunch.ID = Any2String(oRow("ID"))
                            End If
                            If Not IsDBNull(oRow("IdImport")) Then
                                oDatalinkStandardPunch.UniqueEmployeeID = Any2String(oRow("IdImport"))
                            End If
                            If Not IsDBNull(oRow("NIF")) Then
                                oDatalinkStandardPunch.NifEmpleado = Any2String(oRow("NIF"))
                            End If
                            If Not IsDBNull(oRow("Type")) Then
                                oDatalinkStandardPunch.Type = oRow("Type")
                            End If
                            If Not IsDBNull(oRow("ActualType")) Then
                                oDatalinkStandardPunch.ActualType = oRow("ActualType")
                            End If
                            If Not IsDBNull(oRow("DateTime")) Then
                                oDatalinkStandardPunch.DateTime = oRow("DateTime")
                            End If
                            If Not IsDBNull(oRow("IDTerminal")) Then
                                oDatalinkStandardPunch.IDTerminal = Any2Integer(oRow("IDTerminal"))
                            End If
                            If Not IsDBNull(oRow("ShortName")) Then
                                oDatalinkStandardPunch.TypeData = Any2String(oRow("ShortName"))
                            End If
                            If Not IsDBNull(oRow("Location")) Then
                                oDatalinkStandardPunch.GPS = Any2String(oRow("Location"))
                            End If
                            If Not IsDBNull(oRow("Field1")) Then
                                oDatalinkStandardPunch.Field1 = Any2String(oRow("Field1"))
                            End If
                            If Not IsDBNull(oRow("Field2")) Then
                                oDatalinkStandardPunch.Field2 = Any2String(oRow("Field2"))
                            End If
                            If Not IsDBNull(oRow("Field3")) Then
                                oDatalinkStandardPunch.Field3 = Any2Double(oRow("Field3"))
                            End If
                            If Not IsDBNull(oRow("Field4")) Then
                                oDatalinkStandardPunch.Field4 = Any2Double(oRow("Field4"))
                            End If
                            If Not IsDBNull(oRow("Field5")) Then
                                oDatalinkStandardPunch.Field5 = Any2Double(oRow("Field5"))
                            End If
                            If Not IsDBNull(oRow("Field6")) Then
                                oDatalinkStandardPunch.Field6 = Any2Double(oRow("Field6"))
                            End If

                            If Not IsDBNull(oRow("TimeStamp")) Then
                                oDatalinkStandardPunch.Timestamp = oRow("TimeStamp")
                            End If

                            ' Añadimos el fichaje a la lista
                            lPunches.Add(oDatalinkStandardPunch)
                        Next

                        iReturnCode = Core.DTOs.ReturnCode._OK

                    End If
                End If

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetPunchesWithID")
                bolRet = False
            Finally

            End Try

            Return bolRet
        End Function

        Public Function AddPunches(ByVal oPunchList As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef oInvalidPunchList As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try

                Me.State.Result = DataLinkResultEnum.Exception

                oInvalidPunchList = New Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch)
                bolRet = True

                Dim strInvalidMessage As String = ""
                Dim bolNewPunches As Boolean = False
                If oPunchList IsNot Nothing AndAlso oPunchList.Count > 0 Then
                    Dim dtCauses = CreateDataTable("@SELECT# id,Name,isnull(ShortName,'') as ShortName from causes", "Causes")

                    For Each oPunch As RoboticsExternAccess.roDatalinkStandardPunch In oPunchList
                        Dim oNewPunch As New Punch.roPunch()
                        Dim bolRetEmployee As Boolean = False

                        Try
                            oPunch.ResultCode = 0
                            ' El empleado destino debe existir
                            Dim oUserFieldState As New UserFields.roUserFieldState(1)
                            Dim intIDEmployee As Integer = Me.ImportEngine.isEmployeeNew(oPunch.UniqueEmployeeID, oPunch.NifEmpleado, oUserFieldState)
                            If intIDEmployee > 0 Then
                                If oPunch.Type = PunchTypeEnum._IN OrElse oPunch.Type = PunchTypeEnum._OUT OrElse PunchTypeEnum._AUTO OrElse PunchTypeEnum._AV Then
                                    bolRetEmployee = True
                                    'Verificar si el terminal es de tipo AV
                                    If oPunch.Type = PunchTypeEnum._AV Then
                                        Dim oTerminal As New Terminal.roTerminal(oPunch.IDTerminal, New Terminal.roTerminalState())
                                        If oPunch.IDTerminal > 0 AndAlso oTerminal IsNot Nothing Then
                                            Dim sSQL = "@SELECT# count(id) from TerminalReaders where mode = 'ACC' and IDTerminal = " & oPunch.IDTerminal
                                            If roTypes.Any2Integer(ExecuteScalar(sSQL)) = 0 Then
                                                oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidTerminalBehavior
                                                oPunch.ResultDescription = "Invalid terminal behavior, should be AV"
                                                bolRetEmployee = False
                                            End If
                                        Else
                                            oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._TerminalNotExist
                                            oPunch.ResultDescription = "Terminal not found"
                                            bolRetEmployee = False
                                        End If
                                    End If

                                    If bolRetEmployee Then
                                        bolRetEmployee = False
                                        If oPunch.DateTime <= DateTime.Now Then

                                            Dim xFreezeDate = roBusinessSupport.GetEmployeeLockDatetoApply(intIDEmployee, False, oUserFieldState)

                                            If xFreezeDate < oPunch.DateTime Then
                                                bolRetEmployee = True
                                                oNewPunch.IDEmployee = intIDEmployee
                                                oNewPunch.Type = oPunch.Type
                                                oNewPunch.IDTerminal = oPunch.IDTerminal
                                                If oPunch.TypeData <> "" Then
                                                    Dim dRows() As DataRow = dtCauses.Select("ShortName = '" & oPunch.TypeData & "'")
                                                    If dRows.Length > 0 Then
                                                        oNewPunch.TypeData = dRows(0)("id")
                                                    Else
                                                        oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidCause
                                                        oPunch.ResultDescription = "Invalid TypeData, not exist"
                                                        bolRetEmployee = False
                                                    End If
                                                End If
                                                oNewPunch.DateTime = oPunch.DateTime
                                                oNewPunch.Location = oPunch.GPS
                                            Else
                                                oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InCloseDate
                                                oPunch.ResultDescription = "Invalid date, not allowed"
                                                bolRetEmployee = False
                                            End If
                                        Else
                                            oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._FutureDateTime
                                            oPunch.ResultDescription = "Future date, not supported"
                                            bolRetEmployee = False
                                        End If
                                    End If
                                Else
                                    oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidPunchType
                                    oPunch.ResultDescription = "Invalid type, not supported"
                                    bolRetEmployee = False
                                End If
                            Else
                                oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidEmployee
                                oPunch.ResultDescription = "Invalid employee, not exist"
                                bolRetEmployee = False
                            End If

                            ' Si todo ha ido bien , guardamos el fichaje
                            If bolRetEmployee Then
                                oNewPunch.Source = PunchSource.WebService
                                bolRetEmployee = oNewPunch.Save(False)
                                If Not bolRetEmployee Then
                                    oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._ErrorSavingPunch
                                    oPunch.ResultDescription = "Error saving punch. " & oNewPunch.State.ErrorNumber & " " & oNewPunch.State.ErrorText
                                Else
                                    bolNewPunches = True
                                End If
                            End If
                        Catch ex As Exception
                            Me.State.Result = DataLinkResultEnum.Exception
                            Me.State.UpdateStateInfo(ex, "roDataLinkImport::AddPunches_Punch")
                            oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._SqlError
                            oPunch.ResultDescription = "SQL Error"
                            bolRetEmployee = False
                        End Try

                        If Not bolRetEmployee Then
                            ' Si el fichaje no se ha podido guardar en la BBDD lo añadimos a la lista de fichajes incorectos
                            oPunch.Timestamp = Now
                            oInvalidPunchList.Add(oPunch)
                        End If

                    Next
                End If

                ' Notificamos al servidor que calcule los nuevos fichajes
                If bolNewPunches Then roConnector.InitTask(TasksType.MOVES)

                Me.State.Result = DataLinkResultEnum.NoError
                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::AddPunches")
                bolRet = False
            Finally
            End Try

            Return bolRet
        End Function

        Public Function UpdatePunches(ByVal oPunchList As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef oInvalidPunchList As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef strErrorMsg As String, ByRef iReturnCode As Integer, ByVal oPunchCriteria As roPunchCriteria()) As Boolean
            Dim bolRet As Boolean = False

            Try

                Me.State.Result = DataLinkResultEnum.Exception

                oInvalidPunchList = New Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch)
                bolRet = True

                Dim bolNewPunches As Boolean = False
                If oPunchList IsNot Nothing AndAlso oPunchList.Count > 0 Then
                    For Each oPunch As RoboticsExternAccess.roDatalinkStandardPunch In oPunchList
                        Try
                            oPunch.ResultCode = 0

                            'Asignamos al fichaje los datos que se quieren actualizar
                            Dim oCriteriaToUpdate As roPunchCriteria = oPunchCriteria.Where(Function(x) x.ID = oPunch.ID).FirstOrDefault()
                            If oCriteriaToUpdate IsNot Nothing Then
                                Dim oDateTimeToUpdate = roTypes.Any2DateTime(oCriteriaToUpdate.DateTimeToUpdate.Data)
                                If oDateTimeToUpdate <> DateTime.MinValue Then
                                    oPunch.DateTime = oDateTimeToUpdate
                                End If
                                oPunch.Type = oCriteriaToUpdate.Type
                            Else
                                oPunch.Timestamp = Now
                                oInvalidPunchList.Add(oPunch)
                                Continue For
                            End If

                            Dim iIDEmployee = Me.ImportEngine.isEmployeeNew(oPunch.UniqueEmployeeID, oPunch.UniqueEmployeeID, New UserFields.roUserFieldState)
                            If (iIDEmployee = -1) Then
                                oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidEmployee
                                oPunch.ResultDescription = "Error updating punch, invalid employee " & oPunch.UniqueEmployeeID
                                oPunch.Timestamp = Now
                                oInvalidPunchList.Add(oPunch)
                                Continue For
                            End If

                            If oPunch.ID > 0 Then
                                'Añadimos el _AV???
                                If oPunch.Type = PunchTypeEnum._IN OrElse oPunch.Type = PunchTypeEnum._OUT OrElse PunchTypeEnum._AUTO Then
                                    'Revisamos que el fichaje esté dentro de contrato actual.
                                    Dim oActiveContract As Contract.roContract = Contract.roContract.GetContractInDate(iIDEmployee, oPunch.DateTime, New roContractState)
                                    If oActiveContract IsNot Nothing AndAlso oActiveContract.BeginDate >= oPunch.DateTime AndAlso oActiveContract.EndDate <= oPunch.DateTime Then
                                        oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidPunchDate
                                        oPunch.ResultDescription = "Invalid datetime, must be between contract begin and end date"
                                        oPunch.Timestamp = Now
                                        oInvalidPunchList.Add(oPunch)
                                        Continue For
                                    End If

                                    'Comprobamos que el fichaje no esté en periodo de congelación
                                    Dim xFreezeDate = roBusinessSupport.GetEmployeeLockDatetoApply(iIDEmployee, False, Me.State)
                                    If xFreezeDate > oPunch.DateTime Then
                                        oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidPunchDate
                                        oPunch.ResultDescription = "Invalid datetime, must be after freeze date"
                                        oPunch.Timestamp = Now
                                        oInvalidPunchList.Add(oPunch)
                                        Continue For
                                    End If

                                    'Comprobamos que el fichaje no sea a futuro
                                    If oPunch.DateTime > Now Then
                                        oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._FutureDateTime
                                        oPunch.ResultDescription = "Invalid datetime, can't be a future punch"
                                        oPunch.Timestamp = Now
                                        oInvalidPunchList.Add(oPunch)
                                        Continue For
                                    End If
                                Else
                                    oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidPunchType
                                    oPunch.ResultDescription = "Invalid type, not supported"
                                    oPunch.Timestamp = Now
                                    oInvalidPunchList.Add(oPunch)
                                    Continue For
                                End If
                            Else
                                oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._PunchNotFound
                                oPunch.ResultDescription = "Invalid punch, not exist"
                                oPunch.Timestamp = Now
                                oInvalidPunchList.Add(oPunch)
                                Continue For
                            End If

                            'Guardamos el fichaje actualizado
                            Dim oPunchState = New Punch.roPunchState()
                            Dim punchToUpdate As New VTBusiness.Punch.roPunch(iIDEmployee, oPunch.ID, oPunchState)

                            punchToUpdate.Type = oPunch.Type
                            punchToUpdate.ActualType = oPunch.ActualType
                            punchToUpdate.DateTime = oPunch.DateTime
                            punchToUpdate.ShiftDate = New Date(oPunch.DateTime.Year, oPunch.DateTime.Month, oPunch.DateTime.Day) 'Actualizamos el ShiftDate en base al DateTime

                            If Not punchToUpdate.Save() Then
                                oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._ErrorUpdatingPunch
                                oPunch.ResultDescription = "Error updating punch #" & oPunch.ID
                            Else
                                bolNewPunches = True
                            End If
                        Catch ex As Exception
                            Me.State.Result = DataLinkResultEnum.Exception
                            Me.State.UpdateStateInfo(ex, "roDataLinkImport::UpdatePunches_Punch")
                            oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._SqlError
                            oPunch.ResultDescription = "Error updating punch #" & oPunch.ID
                            oPunch.Timestamp = Now
                            oInvalidPunchList.Add(oPunch)
                        End Try
                    Next
                End If

                ' Notificamos al servidor que calcule los nuevos fichajes
                If bolNewPunches Then roConnector.InitTask(TasksType.MOVES)

                Me.State.Result = DataLinkResultEnum.NoError
                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::UpdatePunches")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Function DeletePunches(ByVal oPunchList As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef oInvalidPunchList As Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch), ByRef strErrorMsg As String, ByRef iReturnCode As Integer) As Boolean
            Dim bolRet As Boolean = False
            'test
            Try

                Me.State.Result = DataLinkResultEnum.Exception

                oInvalidPunchList = New Generic.List(Of RoboticsExternAccess.roDatalinkStandardPunch)
                bolRet = True

                Dim strInvalidMessage As String = ""
                Dim bolRecalcPunches As Boolean = False
                If oPunchList IsNot Nothing AndAlso oPunchList.Count > 0 Then
                    For Each oPunch As RoboticsExternAccess.roDatalinkStandardPunch In oPunchList
                        Dim bolRetEmployee As Boolean = True
                        Try
                            oPunch.ResultCode = 0

                            Dim iIDEmployee = Me.ImportEngine.isEmployeeNew(oPunch.UniqueEmployeeID, oPunch.UniqueEmployeeID, New UserFields.roUserFieldState)
                            If (iIDEmployee = -1) Then
                                oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidEmployee
                                oPunch.ResultDescription = "Error deleting punch, invalid employee " & oPunch.UniqueEmployeeID
                                bolRetEmployee = False
                            Else
                                If oPunch.ID > 0 Then
                                    If oPunch.Type = PunchTypeEnum._IN OrElse oPunch.Type = PunchTypeEnum._OUT OrElse oPunch.Type = PunchTypeEnum._AUTO Then
                                        'Comprobamos que el fichaje no esté en periodo de congelación
                                        Dim xFreezeDate = roBusinessSupport.GetEmployeeLockDatetoApply(iIDEmployee, False, Me.State)
                                        If bolRetEmployee AndAlso xFreezeDate > oPunch.DateTime Then
                                            oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidPunchDate
                                            oPunch.ResultDescription = "Invalid datetime, must be after freeze date"
                                            bolRetEmployee = False
                                        End If
                                    Else
                                        oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidPunchType
                                        oPunch.ResultDescription = "Invalid type, not supported. Only types 1, 2 and 3 are supported."
                                        bolRetEmployee = False
                                    End If
                                Else
                                    oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._PunchNotFound
                                    oPunch.ResultDescription = "Invalid punch, not exist"
                                    bolRetEmployee = False
                                End If
                            End If

                            ' Si todo ha ido bien , eliminamos el fichaje
                            If bolRetEmployee Then
                                Dim oPunchState = New Punch.roPunchState()
                                Dim punchToDelete As New VTBusiness.Punch.roPunch(iIDEmployee, oPunch.ID, oPunchState)

                                bolRetEmployee = punchToDelete.Delete()
                                If Not bolRetEmployee Then
                                    oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._ErrorUpdatingPunch
                                    oPunch.ResultDescription = "Error deleting punch #" & oPunch.ID
                                Else
                                    bolRecalcPunches = True
                                End If
                            End If
                        Catch ex As Exception
                            Me.State.Result = DataLinkResultEnum.Exception
                            Me.State.UpdateStateInfo(ex, "roDataLinkImport::DeletePunches_Punch")
                            oPunch.ResultCode = RoboticsExternAccess.Core.DTOs.ReturnCode._SqlError
                            oPunch.ResultDescription = "Error deleting punch #" & oPunch.ID
                            bolRetEmployee = False
                        End Try

                        If Not bolRetEmployee Then
                            ' Si el fichaje no se ha podido eliminar de la BBDD lo añadimos a la lista de fichajes incorectos
                            oPunch.Timestamp = Now
                            oInvalidPunchList.Add(oPunch)
                        End If

                    Next
                End If

                ' Notificamos al servidor que calcule los fichajes
                If bolRecalcPunches Then roConnector.InitTask(TasksType.MOVES)

                Me.State.Result = DataLinkResultEnum.NoError
                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::DeletePunches")
                bolRet = False
            Finally
            End Try

            Return bolRet
        End Function



    End Class


End Namespace