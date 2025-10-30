Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace VTVisits

    <DataContract>
    Public Class roEmployee
        Private iIDEmployee As Integer = 0
        Private sName As String = ""
        Private sGroupName As String = ""
        Private sResult As String = "NoError"

        <DataMember()>
        Public Property idemployee As Integer
            Get
                Return iIDEmployee
            End Get
            Set(value As Integer)
                iIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property name As String
            Get
                Return sName
            End Get
            Set(value As String)
                sName = value
            End Set
        End Property

        <DataMember()>
        Public Property groupname As String
            Get
                Return sGroupName
            End Get
            Set(value As String)
                sGroupName = value
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return sResult
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        Public Sub parserow(ByVal row As DataRow)
            Try
                If row.Table.Columns.Contains("IDEmployee") Then iIDEmployee = Any2Integer(row.Item("IDEmployee"))
                If row.Table.Columns.Contains("EmployeeName") Then sName = Any2String(row.Item("EmployeeName"))
                If row.Table.Columns.Contains("FullGroupName") Then sGroupName = Any2String(row.Item("FullGroupName"))
            Catch ex As Exception

            End Try
        End Sub

    End Class

    <DataContract>
    Public Class roEmployeeList

        Private lEmployees As New List(Of roEmployee)
        Private sResult As String = "NoError"
        Private sfilter As New Dictionary(Of String, String)

        <DataMember()>
        Public Property employees As List(Of roEmployee)
            Get
                Return lEmployees
            End Get
            Set(value As List(Of roEmployee))
                lEmployees = value
            End Set
        End Property

        <DataMember()>
        Public Property result As String
            Get
                Return sResult
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

        <DataMember()>
        Public Property filter As Dictionary(Of String, String)
            Get
                Return sfilter
            End Get
            Set(value As Dictionary(Of String, String))
                sfilter = value
            End Set
        End Property

        Public Function load(ByVal IDPassport As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Try
                Dim perm = Robotics.Security.WLHelper.GetFeaturePermission(IDPassport, "Visits.Create")

                If perm = Permission.Admin Then
                    Dim sSQL As String = "@SELECT# ceg.[IDEmployee], ceg.[EmployeeName], ceg.[FullGroupName] "
                    sSQL &= " from [sysrovwCurrentEmployeeGroups] ceg"
                    sSQL &= " inner join [sysrovwSecurity_PermissionOverEmployees] poe ON poe.IDPassport = " & IDPassport.ToString & " AND poe.IDEmployee=ceg.idemployee AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate "
                    sSQL &= " inner join [sysrovwSecurity_PermissionOverFeatures] pof on pof.IDPassport = " & IDPassport.ToString & " AND pof.IdFeature = 31 AND Permission > 3 "
                    sSQL &= " where CurrentEmployee=1 and CAST(GETDATE() AS DATE) between ceg.BeginDate and ceg.EndDate group by ceg.[IDEmployee], ceg.[EmployeeName], ceg.[FullGroupName]"
                    sSQL &= " union"
                    sSQL &= " @SELECT# ceg.[IDEmployee], [EmployeeName], [FullGroupName] "
                    sSQL += " from [sysrovwCurrentEmployeeGroups] ceg"
                    sSQL += " inner join  sysroPassports p"
                    sSQL += " on p.IDEmployee=ceg.IDEmployee"
                    sSQL += " where CurrentEmployee=1 and p.id=" + IDPassport.ToString + " and CAST(GETDATE() AS DATE) between ceg.BeginDate and ceg.EndDate group by ceg.[IDEmployee], [EmployeeName], [FullGroupName]"
                    sSQL &= "  order by EmployeeName"
                    Dim tb As DataTable = CreateDataTable(sSQL)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim emp As roEmployee
                        For Each row As DataRow In tb.Rows
                            emp = New roEmployee
                            emp.parserow(row)
                            lEmployees.Add(emp)
                        Next
                    End If
                ElseIf perm = Permission.Write Then
                    Dim sSQL As String = "@SELECT# ceg.[IDEmployee], [EmployeeName], [FullGroupName] "
                    sSQL += " from [sysrovwCurrentEmployeeGroups] ceg"
                    sSQL += " inner join  sysroPassports p"
                    sSQL += " on p.IDEmployee=ceg.IDEmployee"
                    sSQL += " where CurrentEmployee=1 and p.id=" + IDPassport.ToString + " and CAST(GETDATE() AS DATE) between ceg.BeginDate and ceg.EndDate group by ceg.[IDEmployee], [EmployeeName], [FullGroupName] order by EmployeeName"

                    Dim tb As DataTable = CreateDataTable(sSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim emp As roEmployee
                        For Each row As DataRow In tb.Rows
                            emp = New roEmployee
                            emp.parserow(row)
                            lEmployees.Add(emp)
                        Next
                    End If
                Else
                    sResult = roVisitorState.ResultEnum.NotPermissions
                    Return False

                End If
            Catch ex As Exception
                sResult = roVisitState.ResultEnum.Exception

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::roemployeelist::load::" + ex.Message)

                Return False
            End Try
            Return True
        End Function

    End Class

    Public Class roEmployeeDB

        Public Enum eEmployeeStatus
            stOut = 0
            stIn = 1
            stEnd = 2
            stAus = 3
            stUnknow = 100
        End Enum

        Public Shared Function getEmployeeStatus(ByVal IDEmployee As Integer) As eEmployeeStatus
            Try

                Dim sql As String = "@SELECT# [CurrentEmployee] from [sysrovwCurrentEmployeeGroups] where idemployee=" + IDEmployee.ToString
                If ExecuteScalar(sql) Then

                    Dim oempst As New Employee.roEmployeeState
                    Dim oempl As New Employee.roEmployeeStatus(IDEmployee, oempst, False)
                    If oempl.IsPresent Then
                        Return eEmployeeStatus.stIn
                    Else
                        Return eEmployeeStatus.stOut
                    End If
                Else
                    Return eEmployeeStatus.stEnd
                End If
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "Visits::roemployeedb::getemployeestatus::" + ex.Message)
            End Try
            Return eEmployeeStatus.stUnknow
        End Function

    End Class

End Namespace