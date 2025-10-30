Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Public Class roEmployeeSupport

    Public Shared Function SetAppConfigurationToEmployees(ByVal lstEmployeesSelected As Generic.List(Of Integer), ByVal Feature As String,
                                                       ByRef oState As roBusinessState, ByVal FromEmployee As Integer) As Boolean

        Dim bolRet As Boolean = False
        Dim strSQL As String = ""

        Try

            Dim oFromPassport As roPassport = Nothing

            Try
                Dim oPassportManager As New roPassportManager
                oFromPassport = oPassportManager.LoadPassport(FromEmployee, LoadType.Employee)
            Catch ex As Exception
                oFromPassport = Nothing
            End Try

            If oFromPassport Is Nothing Then
                bolRet = True
                Return bolRet
            End If

            Dim loginWithoutContract = 0

            Try
                loginWithoutContract = roTypes.Any2Integer(ExecuteScalar("@SELECT# LoginWithoutContract FROM sysroPassPorts WHERE IDEmployee = " & FromEmployee & ""))
            Catch ex As Exception

            End Try

            Dim lstEmployees As New Generic.List(Of Integer)

            Dim idSystemPassport As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())

            For Each IdEmployee As Integer In lstEmployeesSelected
                If idSystemPassport = oState.IDPassport OrElse WLHelper.HasFeaturePermissionByEmployee(oState.IDPassport, Feature, Permission.Write, IdEmployee) Then
                    lstEmployees.Add(IdEmployee)
                End If
            Next

            Dim strEmployeeNamesForAudit As String = roBusinessSupport.GetAuditEmployeeNames(lstEmployees, oState)
            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()
            bolRet = True

            For Each IdEmployee As Integer In lstEmployees
                If IdEmployee <> FromEmployee Then
                    strSQL = "@UPDATE# sysroPassports SET EnabledVTDesktop = " & If(oFromPassport.EnabledVTDesktop, "1", "0") & ", " &
                                                   "EnabledVTPortal  = " & If(oFromPassport.EnabledVTPortal, "1", "0") & ", " &
                                                   "EnabledVTPortalApp = " & If(oFromPassport.EnabledVTPortalApp, "1", "0") & ", " &
                                                   "EnabledVTVisits  = " & If(oFromPassport.EnabledVTVisits, "1", "0") & ", " &
                                                   "EnabledVTVisitsApp  = " & If(oFromPassport.EnabledVTVisitsApp, "1", "0") & ", " &
                                                   "PhotoRequiered  = " & If(oFromPassport.PhotoRequiered, "1", "0") & ", " &
                                                   "LocationRequiered  = " & If(oFromPassport.LocationRequiered, "1", "0") & ", " &
                                                   "LoginWithoutContract  = " & loginWithoutContract &
                                " WHERE IDEmployee= " & IdEmployee.ToString

                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                    If Not bolRet Then Exit For
                End If
            Next

            Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)

            'Auditoría
            If bolRet Then
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                oState.AddAuditParameter(tbParameters, "{EmployeesNames}", strEmployeeNamesForAudit, "", 1)
                oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tPassport, "", tbParameters, -1)

            End If
        Catch ex As DbException
            bolRet = False
            oState.UpdateStateInfo(ex, "roEmployeeSupport::SetAppConfigurationToEmployees")
        End Try

        Return bolRet

    End Function

    Public Shared Function SetPermissionsToEmployees(ByVal lstEmployeesSelected As Generic.List(Of Integer), ByVal Feature As String,
                                                   ByRef oState As roBusinessState, ByVal FromEmployee As Integer) As Boolean

        Dim bolRet As Boolean = False
        Dim strSQL As String = ""

        Try

            Dim oFromPassport As roPassport = Nothing

            Try
                Dim oPassportManager As New roPassportManager
                oFromPassport = oPassportManager.LoadPassport(FromEmployee, LoadType.Employee)
            Catch ex As Exception
                oFromPassport = Nothing
            End Try

            If oFromPassport Is Nothing Then
                bolRet = True
                Return bolRet
            End If

            Dim idSystemPassport As Integer = roTypes.Any2Integer(roConstants.GetSystemUserId())
            Dim oPassport As roPassport = Nothing

            Dim lstEmployees As New Generic.List(Of Integer)

            For Each IdEmployee As Integer In lstEmployeesSelected
                If oState.IDPassport = idSystemPassport OrElse WLHelper.HasFeaturePermissionByEmployee(oState.IDPassport, Feature, Permission.Write, IdEmployee) Then
                    lstEmployees.Add(IdEmployee)
                End If
            Next

            Dim strEmployeeNamesForAudit As String = ""

            Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

            bolRet = True

            For Each IdEmployee As Integer In lstEmployees
                If IdEmployee <> FromEmployee Then
                    Try
                        Dim oPassportManager As New roPassportManager
                        oPassport = oPassportManager.LoadPassport(IdEmployee, LoadType.Employee)
                    Catch ex As Exception
                        oPassport = Nothing
                    End Try

                    If oPassport IsNot Nothing Then
                        ' Eliminamos todos los permisos del empleado
                        bolRet = ExecuteSqlWithoutTimeOut("@DELETE# FROM sysroPassports_PermissionsOverFeatures WHERE IDPassport=" & oPassport.ID.ToString & " AND IDFeature IN(@SELECT# ID FROM sysroFeatures WHERE Type ='E')")

                        ' Generamos los mismos que tiene el empleado origen
                        strSQL = "@INSERT# INTO sysroPassports_PermissionsOverFeatures(IDPassport, IDFeature, Permission) " &
                                    "  @SELECT# " & oPassport.ID.ToString & ", IDFeature, Permission " &
                                    " FROM sysroPassports_PermissionsOverFeatures WHERE IDPassport= " & oFromPassport.ID.ToString &
                                    " AND sysroPassports_PermissionsOverFeatures.IDFeature IN(@SELECT# ID FROM sysroFeatures WHERE Type = 'E') "
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        If Not bolRet Then Exit For
                        strEmployeeNamesForAudit += oPassport.Name + ","

                    End If
                End If
            Next

            Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)

            'Auditoría
            If bolRet Then
                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                oState.AddAuditParameter(tbParameters, "{EmployeesNames}", strEmployeeNamesForAudit, "", 1)
                oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tPassport, "", tbParameters, -1)

            End If
        Catch ex As DbException
            oState.UpdateStateInfo(ex, "roEmployeeSupport::SetPermissionsToEmployees")
        Catch ex As Exception
            oState.UpdateStateInfo(ex, "roEmployeeSupport::SetPermissionsToEmployees")
        End Try

        Return bolRet

    End Function

End Class