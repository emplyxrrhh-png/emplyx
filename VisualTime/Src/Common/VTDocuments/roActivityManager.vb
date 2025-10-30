Imports System.Data.Common
Imports System.Globalization
Imports System.Threading
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTDocuments
    Public Class roActivityManager
        Private oState As roActivityState = Nothing

#Region "Constructores"
        Public Sub New()
            oState = New roActivityState()
        End Sub

        Public Sub New(ByVal _State As roActivityState)
            oState = _State
        End Sub
#End Region

#Region "Métodos"
        ''' <summary>
        ''' Carga una actividad en base a su id
        ''' </summary>
        ''' <param name="idActivity"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns>La activodad como objeto</returns>
        Public Function Load(idActivity As Integer, Optional ByVal oActiveTransaction As DbTransaction = Nothing, Optional ByVal bAudit As Boolean = False) As roActivity
            Dim bolRet As roActivity = Nothing

            Dim oTrans As DbTransaction = Nothing
            Dim oCn As DbConnection = Nothing
            Dim bolCloseCn As Boolean

            Try
                oState.Result = ActivityResultEnum.NoError

                If oActiveTransaction IsNot Nothing Then
                    oTrans = oActiveTransaction
                    oCn = oActiveTransaction.Connection
                    bolCloseCn = False
                Else
                    oTrans = CreateTransaction()
                    oCn = oTrans.Connection
                    bolCloseCn = True
                End If

                ' Validamos si tiene licencia de Documentos
                Dim oLicense As New roServerLicense
                Dim bolLicenseDocuments As Boolean = oLicense.FeatureIsInstalled("Feature\Documents")

                bolRet = New roActivity
                Dim strSQL As String = "SELECT * FROM Activities WHERE ID = " & idActivity.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, , oCn, oTrans)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Id = oRow("ID")
                    bolRet.Name = oRow("Name")
                    bolRet.ShortName = oRow("ShortName")
                    bolRet.Description = oRow("Description")

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{ActivityName}", bolRet.Name, "", 1)
                        oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tActivity, bolRet.Name, tbParameters, -1, oCn, oTrans)
                    End If
                Else
                    bolRet.Id = -1
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::Load")
            Finally
                If bolCloseCn Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Define la acción a realizar, INSERT - UPDATE
        ''' </summary>
        ''' <param name="oActivity"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns>Entero con el id de la actividad actualizada o ingresada</returns>
        Public Function Save(oActivity As roActivity, Optional ByVal oActiveTransaction As DbTransaction = Nothing, Optional ByVal bAudit As Boolean = False) As Integer
            Dim bolRet As Integer = -1
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False
            Dim bolActionInsert As Boolean = False
            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                'Creamos o actualizamos una actividad
                If (oActivity.Id.Equals(-1)) Then
                    oActivity.Id = GetActivityNextID(oTrans)
                    bolActionInsert = True
                End If
                bolRet = If(SaveData(oActivity, oTrans), oActivity.Id, -1)

                If bolRet > 0 AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tActivity, oActivity.Name, tbParameters, -1, oTrans.Connection, oTrans)
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roActivityManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roActivityManager::Save")
            Finally
                If bolCloseTrans Then
                    If bolRet Then
                        oTrans.Commit()
                    Else
                        oTrans.Rollback()
                    End If
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Valida si la actividad se puede hacer CRUD
        ''' </summary>
        ''' <param name="oActivity"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>True - False</returns>
        Public Function Validate(ByVal oActivity As roActivity, ByVal oActiveTransaction As DbTransaction, Optional bolDelete As Boolean = False) As Boolean
            Dim bolRet As Boolean = True
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                oState.Result = ActivityResultEnum.NoError

                'validar que el nombre no esté en blanco y no exista
                If Not (bolDelete) Then
                    If (Not String.IsNullOrEmpty(oActivity.Name)) Then
                        If ExistsActivityName(oActivity.Name, oTrans) Then
                            oState.Result = ActivityResultEnum.InvalidName
                            Return False
                        End If
                    Else
                        oState.Result = ActivityResultEnum.EmptyName
                        Return False
                    End If
                End If

                'validar que el nombre corto no esté en blanco y no exista
                If Not (bolDelete) Then
                    If (Not String.IsNullOrEmpty(oActivity.ShortName)) Then
                        If ExistsActivityExportName(oActivity.ShortName, oTrans) Then
                            oState.Result = ActivityResultEnum.InvalidShortName
                            Return False
                        End If
                    Else
                        oState.Result = ActivityResultEnum.EmptyShortName
                        Return False
                    End If
                End If

                'Validar que ninguna empresa tenga asignada la actividad
                If IsUsedInCompany(oActivity.Id, oTrans) Then
                    oState.Result = ActivityResultEnum.IsUsedInCompany
                    Return False
                End If

                'Validar que ningun empleado tenga asignada la actividad
                If IsUsedInEmployee(oActivity.Id, oTrans) Then
                    oState.Result = ActivityResultEnum.IsUsedInEmployee
                    Return False
                End If

                'Validar que no tenga documentos asignanos
                If HasDocuments(oActivity.Id, oTrans) Then
                    oState.Result = ActivityResultEnum.HasDocuments
                    Return False
                End If


            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::Validate")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Elimina la actividad
        ''' </summary>
        ''' <param name="oActivity"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns>True-False</returns>
        Public Function Delete(oActivity As roActivity, Optional ByVal oActiveTransaction As DbTransaction = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False
            Dim bolActionInsert As Boolean = False
            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If
                Dim oServerLicense As New roServerLicense
                Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\Documents")

                If Validate(oActivity, oTrans, True) Then
                    Dim strQuery = "DELETE Activities WHERE ID = " & oActivity.Id.ToString
                    If Not ExecuteSql(strQuery, oTrans.Connection, oTrans) Then
                        oState.Result = ActivityResultEnum.ConnectionError
                        oState.ErrorText = ""
                        bolRet = False
                    Else
                        bolRet = True
                    End If

                    If bolRet And bAudit Then
                        ' Auditamos borrado de actividad
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{ActivitytName}", oActivity.Name, "", 1)
                        bolRet = oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tActivity, oActivity.Name, tbParameters, -1, oTrans.Connection, oTrans)
                    End If

                    If bolRet AndAlso bolOHPLicense Then roConnector.InitTask(roConnector.TasksType.BROADCASTER, oTrans)
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::Delete")
            Finally
                If bolCloseTrans Then
                    If bolRet Then
                        oTrans.Commit()
                    Else
                        oTrans.Rollback()
                    End If
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene un listado con todas las actividades
        ''' </summary>
        ''' <param name="strWhere">se puede tener el where para filtrar las actividades</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>Listado de objetos actividades</returns>
        Public Function GetActivities(Optional ByVal strWhere As String = "", Optional ByVal oActiveTransaction As DbTransaction = Nothing) As List(Of roActivity)
            Dim bolRet As List(Of roActivity) = Nothing
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False
            Dim tbActivities As DataTable = Nothing

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If
                Dim originalCulture As CultureInfo = Thread.CurrentThread.CurrentCulture
                Dim strQuery = " SELECT * FROM Activities "
                If strWhere <> "" Then strQuery &= "WHERE " & strWhere
                strQuery &= " ORDER BY Name"
                tbActivities = CreateDataTable(strQuery, , oTrans.Connection, oTrans)
                If (tbActivities IsNot Nothing AndAlso tbActivities.Rows.Count > 0) Then
                    bolRet = New List(Of roActivity)
                    For Each rowActivity As DataRow In tbActivities.Rows
                        Dim activity = New roActivity
                        activity.Id = rowActivity("Id")
                        activity.Name = rowActivity("Name")
                        activity.ShortName = rowActivity("ShortName")
                        activity.Description = roTypes.Any2String(rowActivity("Description"))
                        bolRet.Add(activity)
                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roActivityManager::Activities")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roActivityManager::Activities")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Guarda la información de una actividad
        ''' </summary>
        ''' <param name="oActivity"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function SaveData(oActivity As roActivity, oActiveTransaction As DbTransaction) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If
                Dim oServerLicense As New roServerLicense
                Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\Documents")
                Dim bolChangeName As Boolean = False
                Dim tb As New DataTable("Activities")
                Dim strSQL As String = "SELECT * FROM Activities WHERE Id = " & oActivity.Id.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL, oActiveTransaction.Connection, oActiveTransaction)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If (tb.Rows.Count.Equals(0)) Then
                    oRow = tb.NewRow()
                Else
                    oRow = tb.Rows(0)
                    bolChangeName = Not (roTypes.Any2String(oRow("Name")).Equals(oActivity.Name))
                End If
                oRow("ID") = oActivity.Id
                oRow("Name") = oActivity.Name
                oRow("ShortName") = oActivity.ShortName
                oRow("Description") = oActivity.Description

                If tb.Rows.Count = 0 Then tb.Rows.Add(oRow)
                da.Update(tb)
                bolRet = True
                If bolRet AndAlso bolOHPLicense AndAlso bolChangeName Then roConnector.InitTask(roConnector.TasksType.BROADCASTER, oTrans)

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::SaveData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::SaveData")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene las actividades que puede realizar una empresa
        ''' </summary>
        ''' <param name="idCompany"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetCompanyActivities(idCompany As Integer, Optional ByVal oActiveTransaction As DbTransaction = Nothing) As List(Of roActivity)
            Dim bolRet As New List(Of roActivity)
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False
            Dim tbActivities As DataTable = Nothing

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strQuery = " SELECT a.ID, a.Name FROM CompanyActivities ca
                                 join Activities a on ca.IdActivity = a.ID
                                 where ca.IdCompany = " & idCompany.ToString
                strQuery &= " ORDER BY IdActivity"
                tbActivities = CreateDataTable(strQuery, , oTrans.Connection, oTrans)
                If (tbActivities IsNot Nothing AndAlso tbActivities.Rows.Count > 0) Then
                    bolRet = New List(Of roActivity)
                    For Each rowActivity As DataRow In tbActivities.Rows
                        Dim activity = New roActivity
                        activity.Id = rowActivity("Id")
                        activity.Name = rowActivity("Name")
                        bolRet.Add(activity)
                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roActivityManager::GetCompanyActivities")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roActivityManager::GetCompanyActivities")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Guarda las actividades que puede realizar una empresa
        ''' </summary>
        ''' <param name="idCompany"></param>
        ''' <param name="lstActivities"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveCompanyActivities(idCompany As Integer, lstActivities As List(Of Integer), Optional ByVal oActiveTransaction As DbTransaction = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If
                Dim oServerLicense As New roServerLicense
                Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\Documents")

                'Borramos empleados y actividades asignadas
                Dim DeleteQuerys() As String = {"DELETE FROM CompanyActivities WHERE IdCompany = " & idCompany.ToString}
                For Each strSQLDelete As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQLDelete, oTrans.Connection, oTrans)
                    If Not bolRet Then Exit For
                Next

                'Asignamos las nuevas avtividades a la empresa
                If bolRet Then
                    'Asignamos los nuevos grupos
                    Dim i As Integer = 0
                    For Each idActivity As Integer In lstActivities
                        bolRet = ExecuteSql("INSERT INTO CompanyActivities (IdActivity, IdCompany) VALUES(" & idActivity.ToString & "," & idCompany.ToString & ")", oTrans.Connection, oTrans)
                        If Not bolRet Then Exit For
                    Next
                End If
                If bolRet AndAlso bolOHPLicense Then roConnector.InitTask(roConnector.TasksType.BROADCASTER, oTrans)
                bolRet = True

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::SaveCompanyActivities")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::SaveCompanyActivities")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene las actividades que puede realizar un empleado
        ''' </summary>
        ''' <param name="idContract"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetEmployeeActivities(idContract As String, Optional ByVal oActiveTransaction As DbTransaction = Nothing) As List(Of roActivity)
            Dim bolRet As New List(Of roActivity)
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False
            Dim tbActivities As DataTable = Nothing

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strQuery = "select a.Name, a.Id from EmployeeActivities ea INNER JOIN Activities a on ea.IdActivity = a.id
                                where IdContract = '" & idContract & "' "
                strQuery &= " ORDER BY a.Name"

                tbActivities = CreateDataTable(strQuery, , oTrans.Connection, oTrans)
                If (tbActivities IsNot Nothing AndAlso tbActivities.Rows.Count > 0) Then
                    bolRet = New List(Of roActivity)
                    For Each rowActivity As DataRow In tbActivities.Rows
                        Dim activity = New roActivity
                        activity.Id = rowActivity("Id")
                        activity.Name = rowActivity("Name")
                        bolRet.Add(activity)
                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roActivityManager::GetEmployeeActivities")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roActivityManager::GetEmployeeActivities")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Guarda las actividades que puede realizar un empleado.
        ''' </summary>
        ''' <param name="idContract"></param>
        ''' <param name="lstActivities"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveEmployeeActivities(idContract As Integer, lstActivities As List(Of Integer), Optional ByVal oActiveTransaction As DbTransaction = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If
                Dim oServerLicense As New roServerLicense
                Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\Documents")

                'Borramos empleados y actividades asignadas
                Dim DeleteQuerys() As String = {"DELETE FROM EmployeeActivities WHERE IdContract = " & idContract.ToString}
                For Each strSQLDelete As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQLDelete, oTrans.Connection, oTrans)
                    If Not bolRet Then Exit For
                Next

                'Asignamos las nuevas avtividades a la empresa
                If bolRet Then
                    'Asignamos los nuevos grupos
                    Dim i As Integer = 0
                    For Each idActivity As Integer In lstActivities
                        Dim insertSql = "Declare @idEmployee int
                           select @idEmployee = IDEmployee from EmployeeContracts where IDContract = '" & idContract & "'
                           INSERT INTO EmployeeActivities (IdActivity, IdEmployee, IdContract) VALUES(" & idActivity.ToString & ",@idEmployee,'" & idContract.ToString & "')"
                        bolRet = ExecuteSql(insertSql, oTrans.Connection, oTrans)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet AndAlso bolOHPLicense Then
                    roConnector.InitTask(roConnector.TasksType.BROADCASTER, oTrans)
                End If

                bolRet = True

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::SaveEmployeeActivities")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::SaveEmployeeActivities")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene las actividades que podría realizar un empleado en base a su empresa
        ''' </summary>
        ''' <param name="idContract"></param>
        ''' <param name="strWhere"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetActivitiesEmployeeByCompany(idContract As String, Optional ByVal strWhere As String = "", Optional ByVal oActiveTransaction As DbTransaction = Nothing) As List(Of roActivity)
            Dim bolRet As List(Of roActivity) = Nothing
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False
            Dim tbActivities As DataTable = Nothing

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strQuery = "declare @idEmployee int, @idGroup int, @idCompany int " &
                                "select @idEmployee = IDEmployee from EmployeeContracts where IDContract = '" & idContract & "' " &
                                "select @idGroup = IDGroup from sysrovwCurrentEmployeeGroups where IDEmployee = @idEmployee " &
                                "exec @idCompany = [dbo].[GetCompanyGroup] @idGroup " &
                                "select a.Name, a.Id from CompanyActivities ca INNER JOIN Activities a on ca.IdActivity = a.id
                                where IdCompany = @idCompany"
                strQuery &= " ORDER BY a.Name"

                tbActivities = CreateDataTable(strQuery, , oTrans.Connection, oTrans)
                If (tbActivities IsNot Nothing AndAlso tbActivities.Rows.Count > 0) Then
                    bolRet = New List(Of roActivity)
                    For Each rowActivity As DataRow In tbActivities.Rows
                        Dim activity = New roActivity
                        activity.Id = rowActivity("Id")
                        activity.Name = rowActivity("Name")
                        bolRet.Add(activity)
                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roActivityManager::Activities")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roActivityManager::Activities")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Guarda las actividades de los empleados creados masivamente.
        ''' </summary>
        ''' <param name="lstContracts"></param>
        ''' <param name="lstActivities"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveMultiEmployeeActivities(lstContracts As List(Of String), lstActivities As List(Of Integer), Optional ByVal oActiveTransaction As DbTransaction = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If
                Dim oServerLicense As New roServerLicense
                Dim bolOHPLicense As Boolean = oServerLicense.FeatureIsInstalled("Feature\Documents")

                'Guardamos a todos los empleados
                For Each employee As String In lstContracts
                    bolRet = SaveEmployeeActivities(employee, lstActivities, oTrans, True)
                Next
                If bolRet AndAlso bolOHPLicense Then
                    roConnector.InitTask(roConnector.TasksType.BROADCASTER, oTrans)
                End If

                bolRet = True

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::SaveMultiEmployeeActivities")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::SaveMultiEmployeeActivities")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function

        Public Function GetActivityById(idActivity As Integer, Optional ByVal oActiveTransaction As DbTransaction = Nothing) As roActivity
            Dim bolRet As roActivity = Nothing
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False
            Dim tbActivities As DataTable = Nothing

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strQuery = " SELECT * FROM Activities WHERE Id = " & idActivity
                strQuery &= " ORDER BY Name"
                tbActivities = CreateDataTable(strQuery, , oTrans.Connection, oTrans)
                If (tbActivities IsNot Nothing AndAlso tbActivities.Rows.Count > 0) Then
                    bolRet = New roActivity
                    For Each rowActivity As DataRow In tbActivities.Rows
                        bolRet.Id = rowActivity("Id")
                        bolRet.Name = rowActivity("Name")
                        bolRet.ShortName = rowActivity("ShortName")
                        bolRet.Description = roTypes.Any2String(rowActivity("Description"))
                        Exit For
                    Next
                End If

            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roActivityManager::GetActivityById")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roActivityManager::GetActivityById")
            Finally
                If bolCloseTrans Then
                    oTrans.Commit()
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Valida si un empleado puede realizar una actividad
        ''' </summary>
        ''' <param name="idActivity"></param>
        ''' <param name="idEmployee"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Public Function GetActivityPermissionByEmployee(idActivity As Integer, idEmployee As Integer, Optional oActiveTransaction As DbTransaction = Nothing) As Boolean
            ' Recupera el siguiente codigo de employee a usar
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Dim strQuery As String

            strQuery = " Select IdActivity from EmployeeActivities where  IdActivity = " & idActivity & " and idEmployee = " & idEmployee

            Try

                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim tb As DataTable = CreateDataTable(strQuery, , oTrans.Connection, oTrans)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    If (DBNull.Value.Equals(tb.Rows(0)(0))) Then
                        bolRet = True
                    End If
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::GetActivityNextID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::GetActivityNextID")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function


#End Region

#Region "Helper Methods"
        ''' <summary>
        ''' Valida si el nombre de la actividad ya ha sido utilizado.
        ''' </summary>
        ''' <param name="activitytName"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function ExistsActivityName(activitytName As String, oActiveTransaction As DbTransaction) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strSQL As String = "SELECT ID FROM Activities WHERE Name = LTRIM(RTRIM('" & activitytName & "'))"
                Dim tb As DataTable = CreateDataTable(strSQL, , oTrans.Connection, oTrans)

                bolRet = (tb.Rows.Count > 0)

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::ExistsActivityName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::ExistsActivityName")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Valida si el nombre corto de la actividad ya ha sido utilizado.
        ''' </summary>
        ''' <param name="activityExporttName"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function ExistsActivityExportName(activityExporttName As String, oActiveTransaction As DbTransaction) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strSQL As String = "SELECT ID FROM Activities WHERE ShortName = LTRIM(RTRIM( '" & activityExporttName & "'))"
                Dim tb As DataTable = CreateDataTable(strSQL, , oTrans.Connection, oTrans)

                bolRet = (tb.Rows.Count > 0)

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::ExistsActivityExportName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::ExistsActivityExportName")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Valida si una actividad ha sido asignada ya a una empresa.
        ''' </summary>
        ''' <param name="idActivity"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function IsUsedInCompany(idActivity As Integer, oActiveTransaction As DbTransaction) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strSQL As String = "SELECT 1 FROM CompanyActivities WHERE IdActivity = " & idActivity
                Dim tb As DataTable = CreateDataTable(strSQL, , oTrans.Connection, oTrans)

                bolRet = (tb.Rows.Count > 0)

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::IsUsedInCompany")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::IsUsedInCompany")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function

        Private Function IsUsedInEmployee(idActivity As Integer, oActiveTransaction As DbTransaction) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strSQL As String = "SELECT 1 FROM EmployeeActivities WHERE IdActivity = " & idActivity
                Dim tb As DataTable = CreateDataTable(strSQL, , oTrans.Connection, oTrans)

                bolRet = (tb.Rows.Count > 0)

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::IsUsedInEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::IsUsedInEmployee")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function
        ''' <summary>
        ''' Valida si se ha entregado ya documentos para una actividad
        ''' </summary>
        ''' <param name="idActivity"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function HasDocuments(idActivity As Integer, oActiveTransaction As DbTransaction) As Boolean
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Try
                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim strSQL As String = "SELECT 1 FROM DocumentTemplates WHERE Activity = " & idActivity
                Dim tb As DataTable = CreateDataTable(strSQL, , oTrans.Connection, oTrans)

                bolRet = (tb.Rows.Count > 0)

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::HasDocuments")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::HasDocuments")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Obtiene el siguiente id para la la creación de una actividad
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        Private Function GetActivityNextID(oActiveTransaction As DbTransaction) As Integer
            ' Recupera el siguiente codigo de employee a usar
            Dim bolRet As Boolean = False
            Dim oTrans As DbTransaction = oActiveTransaction
            Dim bolCloseTrans As Boolean = False

            Dim strQuery As String
            Dim intNextID As Integer

            intNextID = -1

            strQuery = " Select Max(ID) as Contador From Activities "

            Try

                If oTrans Is Nothing Then
                    oTrans = CreateTransaction()
                    bolCloseTrans = True
                End If

                Dim tb As DataTable = CreateDataTable(strQuery, , oTrans.Connection, oTrans)

                If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                    intNextID = roTypes.Any2Integer(tb.Rows(0).Item(0)) + 1
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityManager::GetActivityNextID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityManager::GetActivityNextID")
            Finally
                If bolCloseTrans Then
                    If oTrans.Connection IsNot Nothing AndAlso oTrans.Connection.State = ConnectionState.Open Then oTrans.Connection.Close()
                End If
            End Try

            Return intNextID

        End Function

#End Region
    End Class
End Namespace

