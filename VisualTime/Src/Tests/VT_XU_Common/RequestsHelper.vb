Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTRequests.Requests

Public Class RequestsHelper
    Public Property RemarksSaved As String = String.Empty
    Public Property NotReliableCauseSaved As String = String.Empty
    Public Function SaveWithParams()
        Robotics.Base.VTRequests.Requests.Fakes.ShimroRequest.AllInstances.SaveWithParamsBooleanBooleanBooleanStringBooleanBoolean = Function(oRequest As Robotics.Base.VTRequests.Requests.roRequest, bSave As Boolean, bSend As Boolean, bCheck As Boolean, sComment As String, bForce As Boolean, bCheckPermissions As Boolean)
                                                                                                                                         Return True
                                                                                                                                     End Function
    End Function

    Public Sub ForgottenTaskRequestFromAdapterStub()
        Robotics.DataLayer.Fakes.ShimAccessHelper.CreateDataAdapterDbCommandBoolean =
            Function(dbCommand As DbCommand, returnIdentity As Boolean) As DbDataAdapter
                Dim stubDbDataAdapter = New Fakes.StubDbDataAdapter With {
                .FillDataTableArrayInt32Int32IDbCommandCommandBehavior =
                    Function(dataTableArray, startRecord, maxRecords, dbCommand2, commandBehaviour)
                        Select Case True
                            Case dbCommand.CommandText.Contains("LevelOfAuthority")
                                Return FillMockWithLevelOfAuthority(dataTableArray, startRecord, maxRecords, dbCommand, commandBehaviour)
                            Case dbCommand.CommandText.Contains("DailySchedule")
                                Return FillMockWithDailySchedule(dataTableArray, startRecord, maxRecords, dbCommand, commandBehaviour)
                            Case Else
                                Return FillEmptyMock(dataTableArray, startRecord, maxRecords, dbCommand, commandBehaviour)
                        End Select

                    End Function,
                    .UpdateDataRowArrayDataTableMapping =
                    Function(dataRows, dataTable)
                        If dbCommand.CommandText.Contains("Punches") Then
                            If Not IsDBNull(dataRows(0)("Remarks")) Then
                                RemarksSaved = dataRows(0)("Remarks")
                            End If
                            If Not IsDBNull(dataRows(0)("NotReliableCause")) Then
                                NotReliableCauseSaved = dataRows(0)("NotReliableCause")
                            End If
                        End If
                    End Function
                }

                Return stubDbDataAdapter
            End Function
    End Sub

    Function FillEmptyMock(dataTableArray As DataTable(), startRecord As Integer, maxRecords As Integer, dbCommand As DbCommand, commandBehaviour As CommandBehavior)
        Dim dataTable As DataTable = dataTableArray.GetValue(startRecord)
        dataTable.Columns.Add("ID")
        dataTable.Columns.Add("IDEmployee")
        dataTable.Columns.Add("IDCredential")
        dataTable.Columns.Add("VerificationType")
        dataTable.Columns.Add("Type")
        dataTable.Columns.Add("ActualType")
        dataTable.Columns.Add("InvalidType")
        dataTable.Columns.Add("ShiftDate")
        dataTable.Columns.Add("DateTime")
        dataTable.Columns.Add("IDTerminal")
        dataTable.Columns.Add("IDReader")
        dataTable.Columns.Add("IDZone")
        dataTable.Columns.Add("ZoneIsNotReliable")
        dataTable.Columns.Add("TypeData")
        dataTable.Columns.Add("TypeDetails")
        dataTable.Columns.Add("Location")
        dataTable.Columns.Add("LocationZone")
        dataTable.Columns.Add("FullAddress")
        dataTable.Columns.Add("IP")
        dataTable.Columns.Add("Action")
        dataTable.Columns.Add("IDPassport")
        dataTable.Columns.Add("Field1")
        dataTable.Columns.Add("Field2")
        dataTable.Columns.Add("Field3")
        dataTable.Columns.Add("Field4")
        dataTable.Columns.Add("Field5")
        dataTable.Columns.Add("Field6")
        dataTable.Columns.Add("Exported")
        dataTable.Columns.Add("SystemDetails")
        dataTable.Columns.Add("IsNotReliable")
        dataTable.Columns.Add("TimeStamp")
        dataTable.Columns.Add("TimeZone")
        dataTable.Columns.Add("InTelecommute")
        dataTable.Columns.Add("IDRequest")
        dataTable.Columns.Add("MaskAlert")
        dataTable.Columns.Add("Timespan")
        dataTable.Columns.Add("CRC")
        dataTable.Columns.Add("Source")
        dataTable.Columns.Add("HasPhoto")
        dataTable.Columns.Add("PhotoOnAzure")
        dataTable.Columns.Add("TemperatureAlert")
        dataTable.Columns.Add("Workcenter")
        dataTable.Columns.Add("Remarks")
        dataTable.Columns.Add("NotReliableCause")
        Return startRecord
    End Function

    Function FillMockWithLevelOfAuthority(dataTableArray As DataTable(), startRecord As Integer, maxRecords As Integer, dbCommand As DbCommand, commandBehaviour As CommandBehavior)
        Dim dataTable As DataTable = dataTableArray.GetValue(startRecord)
        dataTable.Columns.Add("SupervisorLevelOfAuthority")
        dataTable.Columns("SupervisorLevelOfAuthority").DefaultValue = 4
        dataTable.Columns.Add("ApprovedAtLevel")
        dataTable.Columns("ApprovedAtLevel").DefaultValue = 1
        dataTable.Rows.Add(dataTable.NewRow())
        Return startRecord
    End Function

    Function FillMockWithDailySchedule(dataTableArray As DataTable(), startRecord As Integer, maxRecords As Integer, dbCommand As DbCommand, commandBehaviour As CommandBehavior)
        Dim dataTable As DataTable = dataTableArray.GetValue(startRecord)
        dataTable.Columns.Add("IDEmployee")
        dataTable.Columns("IDEmployee").DefaultValue = 4
        dataTable.Columns.Add("Date")
        dataTable.Columns("Date").DefaultValue = New Date(2025, 1, 1)
        dataTable.Columns.Add("TaskStatus")
        dataTable.Columns("TaskStatus").DefaultValue = 1
        dataTable.Rows.Add(dataTable.NewRow())
        Return startRecord
    End Function

    Function GetRequestApprovals()
        Robotics.Base.VTRequests.Requests.Fakes.ShimroRequestApproval.GetRequestApprovalsInt32roRequestState = Function(requestId, state)
                                                                                                                   Dim requestApprovals As New List(Of roRequestApproval)
                                                                                                                   Return requestApprovals
                                                                                                               End Function
    End Function

    Function GetRequestDays()
        Robotics.Base.VTRequests.Requests.Fakes.ShimroRequestDay.GetRequestDaysInt32roRequestState = Function(requestId, state)
                                                                                                         Dim dt As New DataTable
                                                                                                         dt.Columns.Add("Id")
                                                                                                         dt.Columns.Add("Day")
                                                                                                         dt.Columns.Add("DayType")
                                                                                                     End Function
    End Function

    Public Function GetApprovalStatus()
        Robotics.Base.VTRequests.Requests.Fakes.ShimroRequest.AllInstances.GetApprovalStatus = Function(oRequest As Robotics.Base.VTRequests.Requests.roRequest)
                                                                                                   Dim requestApproval As New roRequestApproval
                                                                                                   requestApproval.Status = 0
                                                                                                   Return requestApproval
                                                                                               End Function
    End Function


End Class
