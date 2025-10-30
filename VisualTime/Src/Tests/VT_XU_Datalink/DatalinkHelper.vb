Imports Robotics.Base
Imports Robotics.ExternalSystems
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess

Public Class DatalinkHelper
    Public Property GetCalendarCalled As Boolean = False
    Public Property ValidateTokenCalled As Boolean = False

    Function isEmployeeNewStub(Optional idReturn As Integer = 0)
        Robotics.ExternalSystems.DataLink.Fakes.ShimroExternalSystemBase.AllInstances.isEmployeeNewStringStringroUserFieldStateRef =
            Function(oExternalSystemBase As Robotics.ExternalSystems.DataLink.roExternalSystemBase, ByVal employeeID As String, ByVal userFieldState As String, ByRef roUserFieldState As Robotics.Base.VTUserFields.UserFields.roUserFieldState)
                If idReturn <> 0 Then
                    Return idReturn
                Else
                    Return employeeID
                End If
            End Function
    End Function

    Function GetCalendarSpy()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.fakes.ShimRoboticsExternAccess.AllInstances.GetCalendarroCalendarCriteriaListOfroCalendarRefReturnCodeRefStringRefNullableOfDateTimeNullableOfBoolean =
                    Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal criteria As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendarCriteria, ByRef calendar As List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roCalendar), ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef message As String, ByVal lastUpdate As Nullable(Of DateTime), ByVal encryptMD5 As Nullable(Of Boolean))
                        returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                        GetCalendarCalled = True
                    End Function
    End Function

    Function ValidateTokenSpy()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.ValidateTokenStringStringReturnCodeRefBoolean =
                    Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal token As String, ByVal requestIP As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, encryptMD5 As Boolean)
                        returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                        ValidateTokenCalled = True
                        Return True
                    End Function
    End Function

    Function GetAbsencesStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetAbsencesroDatalinkStandarAbsenceCriteriaroDatalinkStandarAbsenceResponseRef =
                    Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal criteria As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAbsenceCriteria, ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAbsenceResponse)
                        response = New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAbsenceResponse
                        response.Absences = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAbsence)
                        response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    End Function
    End Function

    Function CreateOrUpdateAbsenceStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.CreateOrUpdateAbsenceroDatalinkStandarAbsenceReturnCodeRef =
        Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal absence As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAbsence, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode)
            returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
        End Function
    End Function

    Function GetAccrualsStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetAccrualsroDatalinkStandarAccrualCriteriaroDatalinkStandarAccrualResponseRefBoolean =
        Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal criteria As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAccrualCriteria, ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAccrualResponse, ByVal encryptMD5 As Boolean)
            response = New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAccrualResponse
            response.Accruals = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarAccrual)
            response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
            Return True
        End Function
    End Function

    Function CreateOrUpdateDocumentStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.CreateOrUpdateDocumentroDatalinkStandardDocumentStringReturnCodeRefStringRef =
        Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal document As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardDocument, ByVal documentType As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef message As String)
            returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
        End Function

    End Function

    Function GetEmployeesStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetEmployeesListOfroEmployeeRefBooleanBooleanStringStringStringReturnCodeRefStringRefNullableOfDateTime =
            Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByRef employees As List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roEmployee), ByVal includeInactive As Boolean, ByVal includeTerminated As Boolean, ByVal employeeId As String, ByVal firstName As String, ByVal lastName As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef message As String, ByVal lastUpdate As Nullable(Of DateTime))
                returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
            End Function
    End Function

    Function CreateOrUpdateEmployeeStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.CreateOrUpdateEmployeeroDatalinkStandarEmployeeReturnCodeRefInt32Ref =
            Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal employee As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandarEmployee, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef employeeId As Integer)
                returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
            End Function
    End Function

    Function GetGroupsStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetGroupsListOfroGroupRefBooleanStringReturnCodeRefStringRefString =
            Function(oRoboticsExternAccess As RoboticsExternAccess, ByRef groups As List(Of roGroup), ByVal includeInactive As Boolean, ByVal groupName As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef message As String, ByVal lastUpdate As String)
                returnCode = Core.DTOs.ReturnCode._OK
            End Function

    End Function

    Function GetPunchesExStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetPunchesExPunchFilterTypeDateTimeDateTimeDateTimeroDatalinkStandardPunchResponseRefString =
            Function(oRoboticsExternAccess As RoboticsExternAccess, ByVal filterType As DTOs.PunchFilterType, ByVal startDate As Date, ByVal endDate As Date, ByVal lastUpdate As Date, ByRef response As roDatalinkStandardPunchResponse, ByVal employeeID As String)
                response = New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse
                response.Punches = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch)
                response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                Return True
            End Function
    End Function

    Function GetPunchesStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetPunchesPunchFilterTypeDateTimeDateTimeDateTimeroDatalinkStandardPunchResponseRefString =
            Function(oRoboticsExternAccess As RoboticsExternAccess, ByVal filterType As DTOs.PunchFilterType, ByVal startDate As Date, ByVal endDate As Date, ByVal lastUpdate As Date, ByRef response As roDatalinkStandardPunchResponse, ByVal employeeID As String)
                response = New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse
                response.Punches = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch)
                response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                Return True
            End Function

    End Function

    Function AddPunchesStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.AddPunchesListOfroDatalinkStandardPunchroDatalinkStandardPunchResponseRef =
            Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal punches As List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch), ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse)
                response = New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse
                response.Punches = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch)
                response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                Return True
            End Function

    End Function

    Function UpdatePunchesStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.UpdatePunchesListOfroDatalinkStandardPunchroDatalinkStandardPunchResponseRefroPunchCriteriaArray =
            Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal punches As List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch), ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse, ByVal oCriteria As roPunchCriteria())
                response = New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse
                response.Punches = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch)
                response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                Return True
            End Function
    End Function

    Function UpdatePunchesStub(returnPunches As List(Of roDatalinkStandardPunch))
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.UpdatePunchesListOfroDatalinkStandardPunchroDatalinkStandardPunchResponseRefroPunchCriteriaArray =
            Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal punches As List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch), ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse, ByVal oCriteria As roPunchCriteria())
                response.Punches = returnPunches
                Return True
            End Function
    End Function

    Function UpdateGetPunchesWithIDExStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetPunchesWithIDExListOfStringroDatalinkStandardPunchResponseRef =
        Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal employeeID As List(Of String), ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse)
            response.Punches = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch)
            response.Punches.Add(New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch With {.ID = 1, .DateTime = Date.Now, .Type = 1})
            response.Punches.Add(New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch With {.ID = 2, .DateTime = Date.Now, .Type = 2})

            response.PunchesListError = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch)
            response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
            Return True
        End Function
    End Function

    Function UpdateGetPunchesWithIDExStub(returnPunches As List(Of roDatalinkStandardPunch))
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetPunchesWithIDExListOfStringroDatalinkStandardPunchResponseRef =
        Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal employeeID As List(Of String), ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse)
            response.Punches = returnPunches
            Return True
        End Function
    End Function

    Function DeletePunchesStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.DeletePunchesListOfroDatalinkStandardPunchroDatalinkStandardPunchResponseRef =
            Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByVal punches As List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch), ByRef response As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse)
                response = New Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunchResponse
                response.Punches = New List(Of Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkStandardPunch)
                response.ResultCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                Return True
            End Function
    End Function


    Sub GenerateSupervisorImportStub(groupName As String, supervisorType As String, idSupervisor As String)
        Robotics.ExternalSystems.DataLink.Fakes.ShimroDataLinkImport.AllInstances.GetSheetsCount = Function(a As Robotics.ExternalSystems.DataLink.roDataLinkImport) 1
        Robotics.ExternalSystems.DataLink.Fakes.ShimroDataLinkImport.AllInstances.SetActiveSheetObject = Function(a As Robotics.ExternalSystems.DataLink.roDataLinkImport, sheet As Object) Nothing
        Robotics.ExternalSystems.DataLink.Fakes.ShimroDataLinkImport.AllInstances.CountLinesExcelInt32RefInt32RefInt32 = Function(a As Robotics.ExternalSystems.DataLink.roDataLinkImport, ByRef beginLine As Integer, ByRef endLine As Integer, c As Integer)
                                                                                                                             beginLine = 0
                                                                                                                             endLine = 1
                                                                                                                             Return 2
                                                                                                                         End Function
        Robotics.ExternalSystems.DataLink.Fakes.ShimroDataLinkImport.AllInstances.GetCellValueWithoutFormatInt32Int32 = Function(a As Robotics.ExternalSystems.DataLink.roDataLinkImport, rowIndex As Integer, colIndex As Integer)
                                                                                                                            Dim tbl As DataTable = New DataTable()
                                                                                                                            tbl.Columns.Add("GroupName")
                                                                                                                            tbl.Columns.Add("ResponsibleType")
                                                                                                                            tbl.Columns.Add("ResponsibleId")
                                                                                                                            tbl.Rows.Add("NOMBRE UNIDAD ORGANIZATIVA", "TIPO RESPONSABLE", "ID RESPONSABLE")
                                                                                                                            tbl.Rows.Add(groupName, supervisorType, idSupervisor)
                                                                                                                            Return tbl(rowIndex)(colIndex)
                                                                                                                        End Function
        Robotics.ExternalSystems.DataLink.Fakes.ShimroDataLinkImport.AllInstances.GetCellValueInt32Int32Boolean = Function(a As Robotics.ExternalSystems.DataLink.roDataLinkImport, rowIndex As Integer, colIndex As Integer, d As Boolean)
                                                                                                                      Dim tbl As DataTable = New DataTable()
                                                                                                                      tbl.Columns.Add("GroupName")
                                                                                                                      tbl.Columns.Add("ResponsibleType")
                                                                                                                      tbl.Columns.Add("ResponsibleId")
                                                                                                                      tbl.Rows.Add("NOMBRE UNIDAD ORGANIZATIVA", "TIPO RESPONSABLE", "ID RESPONSABLE")
                                                                                                                      tbl.Rows.Add(groupName, supervisorType, idSupervisor)
                                                                                                                      Return tbl(rowIndex)(colIndex)
                                                                                                                  End Function
    End Sub



    Function GetEmployeeByIdStub()
        Robotics.ExternalSystems.DataLink.RoboticsExternAccess.Fakes.ShimRoboticsExternAccess.AllInstances.GetEmployeeByIdroEmployeeRefStringReturnCodeRefStringRef =
        Function(oRoboticsExternAccess As Robotics.ExternalSystems.DataLink.RoboticsExternAccess.RoboticsExternAccess, ByRef oEmployee As roEmployee, ByVal employeeID As String, ByRef returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode, ByRef returnText As String)
            oEmployee = New roEmployee()
            returnCode = DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode._OK
        End Function
    End Function
End Class