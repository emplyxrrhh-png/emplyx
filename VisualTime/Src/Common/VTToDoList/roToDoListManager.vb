Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace VTToDoLists

    Public Class roToDoListManager

        Private oState As roToDoListState = Nothing

        Public ReadOnly Property State As roToDoListState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roToDoListState()
        End Sub

        Public Sub New(ByVal _State As roToDoListState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        Public Function GetAllToDoListsByType(ByVal listType As ToDoListType, Optional bGetEmployeeDetail As Boolean = False, Optional ByVal bAudit As Boolean = False) As List(Of roToDoList)

            Dim retToDoLists As New List(Of roToDoList)
            Dim strSQL As String

            Try

                oState.Result = ToDoListResultEnum.NoError

                Dim tb As DataTable

                strSQL = $"@SELECT# ToDoLists.* FROM ToDoLists 
                            INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature POE ON POE.IdPassport = {oState.IDPassport} AND POE.IDEmployee = ToDoLists.idemployee AND Convert(DATE,GETDATE()) BETWEEN POE.BeginDate AND POE.EndDate 
                                and POE.FeatureAlias = 'Employees' and POE.FeatureType = 'U' and POE.FeaturePermission >= 3
                            WHERE Type = {CInt(listType)} "


                tb = CreateDataTable(strSQL)

                Dim oToDoList As roToDoList

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oToDoList = New roToDoList
                        oToDoList = GetToDoList(oRow("Id"), ToDoListType.OnBoarding, bGetEmployeeDetail, bAudit)
                        retToDoLists.Add(oToDoList)
                    Next
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorRecoveringAllToDoLists
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetAllToDoListsByType")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorRecoveringAllToDoLists
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetAllToDoListsByType")
            End Try
            Return retToDoLists
        End Function

        Public Function GetLastToDoListsByType(ByVal listType As ToDoListType, Optional iTotal As Integer = 3, Optional bGetEmployeeDetail As Boolean = False, Optional ByVal bAudit As Boolean = False) As List(Of roToDoList)

            Dim retToDoLists As New List(Of roToDoList)
            Dim strSQL As String

            Try

                oState.Result = ToDoListResultEnum.NoError

                Dim tb As DataTable

                strSQL = $"@SELECT# TOP {iTotal} ToDoLists.* FROM ToDoLists 
                            INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature POE ON POE.IdPassport = {oState.IDPassport} AND POE.IDEmployee = ToDoLists.idemployee AND Convert(DATE,GETDATE()) BETWEEN POE.BeginDate AND POE.EndDate 
                                and POE.FeatureAlias = 'Employees' and POE.FeatureType = 'U' and POE.FeaturePermission >= 3
                            WHERE Type = {CInt(listType)} ORDER BY CreatedOn DESC"

                tb = CreateDataTable(strSQL)

                Dim oToDoList As roToDoList

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oToDoList = New roToDoList
                        oToDoList = GetToDoList(oRow("Id"), ToDoListType.OnBoarding, bGetEmployeeDetail, bAudit)
                        retToDoLists.Add(oToDoList)
                    Next
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorRecoveringAllToDoLists
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetLastToDoListsByType")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorRecoveringAllToDoLists
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetLastToDoListsByType")
            End Try
            Return retToDoLists
        End Function

        Public Function GetOnBoarding(ByVal idEmplooyee As Integer, Optional ByVal bAudit As Boolean = False) As roToDoList

            Dim retToDoList As roToDoList = Nothing
            Dim strSQL As String

            Try

                oState.Result = ToDoListResultEnum.ErrorRecoveringOnBoarding

                strSQL = $"@SELECT# ToDoLists.ID FROM ToDoLists 
                            INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature POE ON POE.IdPassport = {oState.IDPassport} AND POE.IDEmployee = ToDoLists.idemployee AND Convert(DATE,GETDATE()) BETWEEN POE.BeginDate AND POE.EndDate 
                                and POE.FeatureAlias = 'Employees' and POE.FeatureType = 'U' and POE.FeaturePermission >= 3
                            WHERE ToDoLists.IdEmployee = {idEmplooyee} "

                Dim idList As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))

                If idList > 0 Then
                    retToDoList = GetToDoList(idList, ToDoListType.OnBoarding, bAudit)
                Else
                    oState.Result = ToDoListResultEnum.SourceListNotFound
                End If

                oState.Result = ToDoListResultEnum.NoError
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetOnBoarding")
            End Try

            Return retToDoList
        End Function

        Public Function GetToDoList(ByVal idTodoList As Integer, Optional ByVal listType As ToDoListType = ToDoListType.OnBoarding, Optional bGetEmployeeDetail As Boolean = False, Optional ByVal bAudit As Boolean = False) As roToDoList

            Dim retToDoList As roToDoList = Nothing
            Dim strSQL As String

            Try

                oState.Result = ToDoListResultEnum.NoError

                Dim tb As DataTable

                strSQL = $"@SELECT# ToDoLists.*, Employees.Name AS EmployeeName, Employees.Image as EmployeeImage FROM ToDoLists 
                            INNER JOIN Employees ON Employees.ID = ToDoLists.IdEmployee 
                            INNER JOIN sysrovwSecurity_PermissionOverEmployeeAndFeature POE ON POE.IdPassport = {oState.IDPassport} AND POE.IDEmployee = Employees.id AND Convert(DATE,GETDATE()) BETWEEN POE.BeginDate AND POE.EndDate 
                                and POE.FeatureAlias = 'Employees' and POE.FeatureType = 'U' and POE.FeaturePermission >= 3
                            WHERE ToDoLists.Type = {CInt(listType)} And ToDoLists.Id = {idTodoList}"

                tb = CreateDataTable(strSQL)

                Dim oToDoList As roToDoList
                Dim oRow As DataRow
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oToDoList = New roToDoList
                    oRow = tb.Rows(0)
                    oToDoList.Comments = roTypes.Any2String(oRow("Comments"))
                    oToDoList.Id = roTypes.Any2Integer(oRow("Id"))
                    oToDoList.IdEmployee = roTypes.Any2String(oRow("IdEmployee"))
                    oToDoList.StartDate = roTypes.Any2DateTime(oRow("StartDate"))
                    oToDoList.CreatedOn = roTypes.Any2DateTime(oRow("CreatedOn"))
                    If bGetEmployeeDetail Then
                        oToDoList.EmployeeImage = If(IsDBNull(oRow("EmployeeImage")), Nothing, oRow("EmployeeImage"))
                        oToDoList.EmployeeName = oRow("EmployeeName")
                    End If
                    oToDoList.Type = listType
                    Dim oToDoTasks As New List(Of roToDoTask)
                    oToDoTasks = GetToDoListTasks(oToDoList.Id)
                    oToDoList.Tasks = oToDoTasks.ToArray
                    oToDoList.Status = oToDoTasks.FindAll(Function(x) x.Done = True).Count & "/" & oToDoTasks.Count
                    retToDoList = oToDoList
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoList")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoList")
            Finally

            End Try
            Return retToDoList
        End Function

        Public Function GetToDoListTasksByIdEmployee(idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As List(Of roToDoTask)

            Dim retToDoTasks As New List(Of roToDoTask)
            Dim strSQL As String

            Try

                oState.Result = ToDoListResultEnum.NoError

                Dim tb As DataTable

                strSQL = "@SELECT# tt.* from ToDoListTasks tt inner join ToDoLists t on t.id = tt.IdList where t.IdEmployee = " & idEmployee.ToString
                tb = CreateDataTable(strSQL)

                Dim oToDoTask As roToDoTask

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oToDoTask = New roToDoTask
                        oToDoTask = GetToDoTask(oRow("Id"))
                        retToDoTasks.Add(oToDoTask)
                    Next
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoListTasks
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoListTasksByIdEmployee")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoListTasks
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoListTasksByIdEmployee")
            Finally

            End Try
            Return retToDoTasks
        End Function

        Public Function GetToDoListTasks(idToDoList As Integer, Optional ByVal bAudit As Boolean = False) As List(Of roToDoTask)

            Dim retToDoTasks As New List(Of roToDoTask)
            Dim strSQL As String

            Try

                oState.Result = ToDoListResultEnum.NoError

                Dim tb As DataTable

                strSQL = "@SELECT# * FROM ToDoListTasks WHERE IdList = " & idToDoList.ToString
                tb = CreateDataTable(strSQL)

                Dim oToDoTask As roToDoTask

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oToDoTask = New roToDoTask
                        oToDoTask = GetToDoTask(oRow("Id"))
                        retToDoTasks.Add(oToDoTask)
                    Next
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoListTasks
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoListTasks")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoListTasks
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoListTasks")
            Finally

            End Try
            Return retToDoTasks
        End Function

        Public Function GetToDoTask(idTask As Integer, Optional ByVal bAudit As Boolean = False) As roToDoTask

            Dim retToDoTask As roToDoTask = Nothing
            Dim strSQL As String

            Try

                oState.Result = ToDoListResultEnum.NoError

                Dim tb As DataTable

                strSQL = "@SELECT# * FROM ToDoListTasks WHERE Id = " & idTask.ToString
                tb = CreateDataTable(strSQL)

                Dim oToDoTask As roToDoTask

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    oToDoTask = New roToDoTask
                    oToDoTask.Done = roTypes.Any2Boolean(oRow("Status"))
                    oToDoTask.IdList = roTypes.Any2Integer(oRow("IdList"))
                    oToDoTask.Id = roTypes.Any2Integer(oRow("Id"))
                    oToDoTask.LastChangeDate = roTypes.Any2DateTime(oRow("LastModified")).Date
                    oToDoTask.SupervisorName = roTypes.Any2String(oRow("LastModifiedBy"))
                    oToDoTask.TaskName = roTypes.Any2String(oRow("Title"))
                    retToDoTask = oToDoTask
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoTask
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoTask")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorRecoveringToDoTask
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::GetToDoTask")
            Finally

            End Try
            Return retToDoTask
        End Function

        Public Function CreateOrUpdateToDoList(oToDoList As roToDoList, Optional ByVal bAudit As Boolean = False) As roToDoList
            Dim ret As New roToDoList
            Dim bHaveToClose As Boolean = False
            Dim retTasks As New List(Of roToDoTask)
            Dim lFinalTasks As New List(Of roToDoTask)

            Dim bolIsNew As Boolean = True

            Try
                oState.Result = ToDoListResultEnum.NoError

                If Not DataLayer.roSupport.IsXSSSafe(oToDoList) Then
                    oState.Result = ToDoListResultEnum.XSSvalidationError
                    oToDoList.Tasks = {}
                    ret = oToDoList
                    Return ret
                End If

                If oToDoList.Tasks Is Nothing Then oToDoList.Tasks = {}

                ret = oToDoList

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                If (oToDoList.Id > 0) Then
                    bolIsNew = False
                    sqlCommand = $"@UPDATE# ToDoLists SET [Type]=@type, [IdEmployee]=@idemployee, [CreatedOn]=@createdon, [StartDate]=@startdate,[LastModifiedBy]=@lastmodifiedby, [Comments]=@comments WHERE [Id] = @id"
                Else
                    bolIsNew = True
                    sqlCommand = $"@INSERT# INTO ToDoLists ([Type],[IdEmployee],[CreatedOn],[StartDate],[LastModifiedBy],[Comments]) " &
                                 " OUTPUT INSERTED.ID VALUES (@type, @idemployee, @createdon, @startdate,@lastmodifiedby, @comments)"
                End If

                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oToDoList.Id))
                parameters.Add(New CommandParameter("@startdate", CommandParameter.ParameterType.tDateTime, oToDoList.StartDate))
                parameters.Add(New CommandParameter("@type", CommandParameter.ParameterType.tInt, oToDoList.Type))
                parameters.Add(New CommandParameter("@idemployee", CommandParameter.ParameterType.tInt, oToDoList.IdEmployee))
                parameters.Add(New CommandParameter("@createdon", CommandParameter.ParameterType.tDateTime, IIf(bolIsNew, Now, oToDoList.CreatedOn)))
                parameters.Add(New CommandParameter("@lastmodifiedby", CommandParameter.ParameterType.tString, oToDoList.LastModifiedBy))
                parameters.Add(New CommandParameter("@comments", CommandParameter.ParameterType.tString, oToDoList.Comments))

                Try
                    Dim iNew As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sqlCommand, parameters))
                    If bolIsNew Then oToDoList.Id = iNew
                Catch ex As Exception
                    oState.Result = ToDoListResultEnum.ErrorCreatingOrUpdatingToDoList
                End Try

                If (oState.Result = ToDoListResultEnum.NoError) Then
                    ' Actualizamos tareas de la lista
                    Dim lCurrentToDoListTasksIds As New List(Of Integer)
                    Dim lCurrentToDoListTasks As New List(Of roToDoTask)
                    lCurrentToDoListTasks = GetToDoListTasks(oToDoList.Id)
                    lCurrentToDoListTasksIds = lCurrentToDoListTasks.Select(Function(x) x.Id).ToList

                    Dim lIncomingToDoListTasksIds As New List(Of Integer)
                    If Not oToDoList.Tasks Is Nothing Then
                        lIncomingToDoListTasksIds = oToDoList.Tasks.ToList.Select(Function(z) z.Id).ToList
                        For Each oTodoTask As roToDoTask In oToDoList.Tasks.ToList
                            oTodoTask.IdList = oToDoList.Id
                        Next
                    End If

                    ' 1.- Añado las que están y no estaban
                    Dim lToDoTasksToAdd As New List(Of roToDoTask)
                    Dim lToDoTasksToAddIds As New List(Of Integer)

                    lToDoTasksToAddIds = lIncomingToDoListTasksIds.Except(lCurrentToDoListTasksIds).ToList
                    lToDoTasksToAdd = oToDoList.Tasks.ToList.FindAll(Function(w) lToDoTasksToAddIds.Contains(w.Id))
                    retTasks = CreateOrUpdateToDoTasks(lToDoTasksToAdd)
                    lFinalTasks.AddRange(retTasks)

                    If (oState.Result = ToDoListResultEnum.NoError) Then
                        ' 2.- Actualizo las que están y siguen estando
                        Dim lToDoTasksToUpdate As New List(Of roToDoTask)
                        Dim lToDoTasksToUpdateIds As New List(Of Integer)

                        lToDoTasksToUpdateIds = lIncomingToDoListTasksIds.Intersect(lCurrentToDoListTasksIds).ToList
                        lToDoTasksToUpdate = oToDoList.Tasks.ToList.FindAll(Function(w) lToDoTasksToUpdateIds.Contains(w.Id))

                        retTasks = CreateOrUpdateToDoTasks(lToDoTasksToUpdate)
                        lFinalTasks.AddRange(lToDoTasksToUpdate)

                        ret.Tasks = lFinalTasks.ToArray

                        If (oState.Result = ToDoListResultEnum.NoError) Then
                            ' 3.- Borro las que ya no están y estaban
                            Dim lToDoTasksToDelete As New List(Of roToDoTask)
                            Dim lToDoTasksToDeleteIds As New List(Of Integer)

                            lToDoTasksToDeleteIds = lCurrentToDoListTasksIds.Except(lIncomingToDoListTasksIds).ToList
                            lToDoTasksToDelete = lCurrentToDoListTasks.FindAll(Function(w) lToDoTasksToDeleteIds.Contains(w.Id))

                            DeleteToDoTasks(lToDoTasksToDelete)
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorCreatingOrUpdatingToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CreateOrUpdateToDoList")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorCreatingOrUpdatingToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CreateOrUpdateToDoList")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = ToDoListResultEnum.NoError)
            End Try
            Return ret
        End Function

        Public Function CreateOrUpdateToDoTasks(lToDoTasks As List(Of roToDoTask), Optional ByVal bAudit As Boolean = False) As List(Of roToDoTask)
            Dim ret As New List(Of roToDoTask)

            Dim bolIsNew As Boolean = True

            Try

                oState.Result = ToDoListResultEnum.NoError

                If Not DataLayer.roSupport.IsXSSSafe(lToDoTasks) Then
                    oState.Result = ToDoListResultEnum.XSSvalidationError
                    Return ret
                End If


                If lToDoTasks Is Nothing OrElse lToDoTasks.Count = 0 Then Return ret

                ret = lToDoTasks

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)
                Dim iNew As Integer

                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, 0))
                parameters.Add(New CommandParameter("@idlist", CommandParameter.ParameterType.tInt, 0))
                parameters.Add(New CommandParameter("@title", CommandParameter.ParameterType.tString, ""))
                parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tBoolean, False))
                parameters.Add(New CommandParameter("@lastmodified", CommandParameter.ParameterType.tDateTime, Now))
                parameters.Add(New CommandParameter("@lastmodifiedby", CommandParameter.ParameterType.tString, ""))

                For Each oToDoTask As roToDoTask In lToDoTasks
                    If oToDoTask.Id > 0 Then
                        bolIsNew = False
                        sqlCommand = $"@UPDATE# ToDoListTasks SET [IdList]=@idlist, [Title]=@title, [Status]=@status, [LastModified]=@lastmodified,[LastModifiedBy]=@lastmodifiedby WHERE [Id] = @id"
                    Else
                        bolIsNew = True
                        sqlCommand = $"@INSERT# INTO ToDoListTasks ([IdList],[Title],[Status],[LastModified],[LastModifiedBy]) " &
                                    " OUTPUT INSERTED.ID VALUES (@idlist, @title, @status, @lastmodified,@lastmodifiedby)"
                    End If

                    parameters.Item(0).Value = oToDoTask.Id
                    parameters.Item(1).Value = oToDoTask.IdList
                    parameters.Item(2).Value = oToDoTask.TaskName
                    parameters.Item(3).Value = oToDoTask.Done
                    parameters.Item(4).Value = oToDoTask.LastChangeDate
                    parameters.Item(5).Value = oToDoTask.SupervisorName

                    Try
                        iNew = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sqlCommand, parameters))
                        If bolIsNew Then oToDoTask.Id = iNew
                    Catch ex As Exception
                        oState.Result = ToDoListResultEnum.ErrorCreatingOrUpdatingToDoTask
                    End Try
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CreateOrUpdateToDoTasks")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CreateOrUpdateToDoTasks")
            End Try
            Return lToDoTasks
        End Function

        Public Function DeleteToDoTasks(lToDoTasks As List(Of roToDoTask), Optional ByVal bAudit As Boolean = False) As Boolean
            Dim strSQL As String = String.Empty
            Dim bolIsNew As Boolean = True

            Try
                If lToDoTasks Is Nothing OrElse lToDoTasks.Count = 0 Then Return True

                oState.Result = ToDoListResultEnum.ErrorDeletingToDoTasks

                Dim sToDoTasksIdsToDelete As String = String.Empty
                sToDoTasksIdsToDelete = String.Join(",", lToDoTasks.Select(Function(x) x.Id).ToArray)
                Dim sSQL As String = "@DELETE# ToDoListTasks WHERE Id IN (" & sToDoTasksIdsToDelete & ")"
                ExecuteSql(sSQL)

                oState.Result = ToDoListResultEnum.NoError
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::DeleteToDoTasks")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::DeleteToDoTasks")
            End Try
            Return (oState.Result = ToDoListResultEnum.NoError)
        End Function

        Public Function DeleteToDoList(oToDoList As roToDoList, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim ret As Boolean = False
            Dim bHaveToClose As Boolean = False

            Dim strSQL As String = String.Empty

            Try
                If oToDoList.Id <= 0 Then Return True

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                oState.Result = ToDoListResultEnum.ErrorDeletingToDoList

                Dim sToDoTasksIdsToDelete As String = String.Empty
                Dim sSQL As String
                sSQL = "@DELETE# ToDoListTasks WHERE IdList = " & oToDoList.Id
                ret = ExecuteSql(sSQL)
                If ret Then
                    sSQL = "@DELETE# ToDoLists WHERE Id = " & oToDoList.Id
                    ret = ExecuteSql(sSQL)
                End If

                oState.Result = ToDoListResultEnum.NoError
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::DeleteToDoList")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CreateOrUpdateOrDeleteToDoTasks")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, ret)
            End Try
            Return ret
        End Function

        Public Function CloneOnBoarding(oTargetToDoList As roToDoList, idSourceEmployee As Integer, Optional ByVal bAudit As Boolean = False) As roToDoList

            Dim retToDoList As New roToDoList

            Try

                oState.Result = ToDoListResultEnum.NoError

                ' Cargo lista origen
                Dim oSourceToDoList As roToDoList
                oSourceToDoList = GetOnBoarding(idSourceEmployee, bAudit)

                If oState.Result = ToDoListResultEnum.NoError Then
                    ' Sustituyo empleado y pongo valores por defecto
                    oTargetToDoList.Id = 0
                    oTargetToDoList.Tasks = oSourceToDoList.Tasks
                    InitializeTasks(oTargetToDoList.Tasks.ToList)
                    retToDoList = CreateOrUpdateToDoList(oTargetToDoList, bAudit)
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorCloningToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CloneOnBoarding")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorCloningToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CloneOnBoarding")
            Finally

            End Try
            Return retToDoList
        End Function

        Public Function CloneTodoList(idTargetEmployee As Integer, idSourceList As Integer, Optional lListType As ToDoListType = ToDoListType.OnBoarding, Optional ByVal bAudit As Boolean = False) As roToDoList

            Dim retToDoList As New roToDoList

            Try

                oState.Result = ToDoListResultEnum.NoError

                ' Cargo lista origen
                Dim oSourceToDoList As roToDoList
                oSourceToDoList = GetToDoList(idSourceList, lListType, bAudit)

                ' Sustituyo empleado y pongo valores por defecto
                oSourceToDoList.IdEmployee = idTargetEmployee
                oSourceToDoList.Id = 0
                InitializeTasks(oSourceToDoList.Tasks.ToList)
                retToDoList = CreateOrUpdateToDoList(oSourceToDoList, bAudit)
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorCloningToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CloneTodoList")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorCloningToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CloneTodoList")
            Finally

            End Try
            Return retToDoList
        End Function

        ''' <summary>
        ''' Clona las tareas de la lista idTarget a la lista idSource. Ambas deben existir
        ''' </summary>
        ''' <param name="idSourceList"></param>
        ''' <param name="idTargetList"></param>
        ''' <param name="lListType"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function CloneToDoListTasks(idSourceList As Integer, idTargetList As Integer, Optional lListType As ToDoListType = ToDoListType.OnBoarding, Optional ByVal bAudit As Boolean = False) As roToDoList

            Dim retToDoList As roToDoList = Nothing

            Try

                oState.Result = ToDoListResultEnum.NoError

                ' Cargo lista origen
                Dim oSourceToDoList As roToDoList
                oSourceToDoList = GetToDoList(idSourceList, lListType, bAudit)
                Dim oTargetToDoList As roToDoList
                oTargetToDoList = GetToDoList(idTargetList, lListType, bAudit)

                If oSourceToDoList Is Nothing OrElse oSourceToDoList.Id <> idSourceList Then
                    oState.Result = ToDoListResultEnum.SourceListNotFound
                ElseIf oTargetToDoList Is Nothing OrElse oTargetToDoList.Id <> idTargetList Then
                    oState.Result = ToDoListResultEnum.TargetListNotFound
                ElseIf Not oTargetToDoList.Tasks Is Nothing AndAlso oTargetToDoList.Tasks.Count > 0 Then
                    oState.Result = ToDoListResultEnum.TargetListAlreadyHasTasks
                Else
                    ' Inicializo
                    oTargetToDoList.Tasks = oSourceToDoList.Tasks
                    InitializeTasks(oTargetToDoList.Tasks.ToList)

                    retToDoList = CreateOrUpdateToDoList(oTargetToDoList, bAudit)

                    oState.Result = ToDoListResultEnum.NoError
                End If
            Catch ex As DbException
                oState.Result = ToDoListResultEnum.ErrorCloningToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CloneTodoListTasks")
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorCloningToDoList
                Me.oState.UpdateStateInfo(ex, "roToDoListManager::CloneTodoListTasks")
            Finally

            End Try
            Return retToDoList
        End Function

        Private Sub InitializeTasks(ByRef lTodoTasks As List(Of roToDoTask))
            Try
                For Each oTodoTask As roToDoTask In lTodoTasks
                    oTodoTask.Done = False
                    oTodoTask.Id = 0
                    oTodoTask.LastChangeDate = Now
                    oTodoTask.IdList = 0 'Le asignará el id de la lista a la que pertenecen
                    oTodoTask.SupervisorName = String.Empty
                Next
            Catch ex As Exception
                oState.Result = ToDoListResultEnum.ErrorInitializingTasks
            End Try
        End Sub

#End Region

    End Class

End Namespace