Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTSurveys

    Public Class roSurveyManager

        Private oState As roSurveyState = Nothing

        Public ReadOnly Property State As roSurveyState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roSurveyState()
        End Sub

        Public Sub New(ByVal _State As roSurveyState)
            oState = _State
        End Sub

#End Region

#Region "Methods"

        ''' <summary>
        ''' Recupera toda la información de una encuesta
        ''' </summary>
        ''' <param name="idSurvey"></param>
        ''' <param name="bLoadDetails"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetSurvey(idSurvey As Integer, Optional bLoadDetails As Boolean = True, Optional ByVal bAudit As Boolean = False) As roSurvey
            Dim retSurvey As New roSurvey
            Dim strSQL As String
            Dim lEmployees As List(Of Integer)
            Dim lGroups As List(Of Integer)
            Dim lResponses As List(Of roSurveyEmployee)

            Try

                oState.Result = SurveyResultEnum.NoError

                Dim tb As DataTable

                strSQL = "@SELECT# [Id],[Title],[IdCreatedBy],[CreatedOn],[ModifiedOn],[SentOn] "
                If bLoadDetails Then strSQL = strSQL & ",[Content]"
                strSQL = strSQL & ",[Mandatory],[ResponseMaxPercentage],[ExpirationDate],[Status],[Anonymous],[SurveyMode]
                      FROM [dbo].[Surveys] WHERE Id = " & idSurvey.ToString

                tb = CreateDataTable(strSQL)

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    retSurvey = New roSurvey
                    retSurvey.Id = roTypes.Any2Integer(oRow("Id"))
                    retSurvey.Title = roTypes.Any2String(oRow("Title"))
                    retSurvey.CreatedBy = roTypes.Any2Integer(oRow("IdCreatedBy"))
                    retSurvey.CreatedOn = roTypes.Any2DateTime(oRow("CreatedOn"))
                    retSurvey.ModifiedOn = roTypes.Any2DateTime(oRow("ModifiedOn"))
                    retSurvey.SentOn = roTypes.Any2DateTime(oRow("SentOn"))
                    Dim iTotal As Integer = 0
                    Dim iResponses As Integer = 0
                    lResponses = New List(Of roSurveyEmployee)
                    retSurvey.CurrentPercentage = GetSurveyStatus(retSurvey, iTotal, iResponses, lResponses, bLoadDetails)
                    If bLoadDetails Then
                        retSurvey.Content = roTypes.Any2String(oRow("Content"))
                        retSurvey.CurrentEmployeeResponses = lResponses.ToArray
                    Else
                        retSurvey.Content = String.Empty
                    End If
                    retSurvey.IsMandatory = roTypes.Any2Boolean(oRow("Mandatory"))
                    retSurvey.Anonymous = roTypes.Any2Boolean(oRow("Anonymous"))
                    retSurvey.AdvancedMode = roTypes.Any2Boolean(oRow("SurveyMode"))
                    retSurvey.ResponseMaxPercentage = roTypes.Any2Integer(oRow("ResponseMaxPercentage"))
                    retSurvey.ExpirationDate = roTypes.Any2DateTime(oRow("ExpirationDate"))
                    retSurvey.Status = roTypes.Any2Integer(oRow("Status"))
                    Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(retSurvey.CreatedBy)
                    retSurvey.CreatedByName = oPassport.Name
                    retSurvey.Progress = iResponses.ToString & "/" & iTotal.ToString
                End If

                '1.- Cargo Empleados
                tb = CreateDataTable("@SELECT# * FROM SurveyEmployees WHERE IdSurvey = " & retSurvey.Id)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    lEmployees = New List(Of Integer)
                    For Each oRow As DataRow In tb.Rows
                        lEmployees.Add(oRow("IdEmployee"))
                    Next
                    retSurvey.Employees = lEmployees.ToArray
                Else
                    retSurvey.Employees = {}
                End If

                '2.- Cargo Grupos
                tb = CreateDataTable("@SELECT# * FROM SurveyGroups WHERE IdSurvey = " & retSurvey.Id)
                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    lGroups = New List(Of Integer)
                    For Each oRow As DataRow In tb.Rows
                        lGroups.Add(oRow("IdGroup"))
                    Next
                    retSurvey.Groups = lGroups.ToArray
                Else
                    retSurvey.Groups = {}
                End If

                If (oState.Result = SurveyResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tSurvey, retSurvey.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorRecoveringSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurvey")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorRecoveringSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurvey")
            End Try
            Return retSurvey
        End Function

        ''' <summary>
        ''' Recupera todas las encuestas. Si se solicita como supervisor, recupera todas las creadas por el propio supervisor. Si se solicita como usuario, recupera todas a las que se han creado y que le incluyen como audiencia
        ''' </summary>
        ''' <param name="idEmployee"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function GetAllSurveys(Optional idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As List(Of roSurvey)

            Dim retSurveys As New List(Of roSurvey)
            Dim strSQL As String
            Dim bForEmployee As Boolean = (idEmployee > 0)

            Try

                oState.Result = SurveyResultEnum.NoError

                Dim tb As DataTable

                ' Actualizamos estado por si alguna encuesta que vaya por fecha ha expirado con el cambio de día
                Dim strSQLToday As String = roTypes.Any2Time(Now.Date).SQLSmallDateTime

                strSQL = "@UPDATE# Surveys SET Status = " & SurveyStatusEnum.Expired & " WHERE ExpirationDate < " & strSQLToday
                ExecuteSql(strSQL)

                If bForEmployee Then
                    ' Un empleado puede ver todas las encuestas activas creadas por un supervsor suyo (sysrovwSurveysEmployees)
                    strSQL = "@SELECT# Surveys.Id from Surveys " &
                             " INNER JOIN sysrovwSurveysEmployees ON Surveys.id = sysrovwSurveysEmployees.IdSurvey " &
                             " INNER JOIN sysrovwSurveyStatistics ON sysrovwSurveyStatistics.IdEmployee = sysrovwSurveysEmployees.IdEmployee AND sysrovwSurveyStatistics.IdSurvey = Surveys.id " &
                             " WHERE sysrovwSurveysEmployees.IdEmployee = " & idEmployee &
                             " AND ExpirationDate >= " & strSQLToday &
                             " AND sysrovwSurveyStatistics.ResponseTimeStamp IS NULL " &
                             " AND Status = " & SurveyStatusEnum.Online &
                             " ORDER BY Mandatory DESC"
                Else
                    ' Un supervisor puede ver sólo las encuestas que creó él directamente
                    strSQL = "@SELECT# Id FROM Surveys WHERE IdCreatedBy = " & Me.oState.IDPassport
                End If

                tb = CreateDataTable(strSQL)

                Dim oSurvey As roSurvey

                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        oSurvey = GetSurvey(oRow("Id"), False)
                        retSurveys.Add(oSurvey)
                    Next
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveys
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetAllSurveys")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveys
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetAllSurveys")
            Finally

            End Try
            Return retSurveys
        End Function

        Public Function HasEmployeeSurveysWithAlert(idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim strSQL As String
            Try

                oState.Result = SurveyResultEnum.NoError

                Dim tb As DataTable

                Dim strSQLToday As String = roTypes.Any2Time(Now.Date).SQLSmallDateTime
                ' Un empleado puede ver todas las encuestas activas creadas por un supervsor suyo (sysrovwSurveysEmployees)
                strSQL = "@SELECT# Surveys.Id, Surveys.Mandatory from Surveys " &
                             " INNER JOIN sysrovwSurveysEmployees ON Surveys.id = sysrovwSurveysEmployees.IdSurvey " &
                             " INNER JOIN sysrovwSurveyStatistics ON sysrovwSurveyStatistics.IdEmployee = sysrovwSurveysEmployees.IdEmployee AND sysrovwSurveyStatistics.IdSurvey = Surveys.id " &
                             " WHERE sysrovwSurveysEmployees.IdEmployee = " & idEmployee &
                             " AND ExpirationDate >= " & strSQLToday &
                             " AND sysrovwSurveyStatistics.ResponseTimeStamp IS NULL " &
                             " AND Status = " & SurveyStatusEnum.Online &
                             " ORDER BY Mandatory DESC"

                tb = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        If roTypes.Any2Boolean(oRow("Mandatory")) Then Return True
                    Next
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveys
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::HasEmployeeSurveysWithAlert")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveys
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::HasEmployeeSurveysWithAlert")
            End Try

            Return False
        End Function

        ''' <summary>
        ''' Crea o actualiza una encuesta
        ''' </summary>
        ''' <param name="oSurvey"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function CreateOrUpdateSurvey(ByRef oSurvey As roSurvey, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                oState.Result = SurveyResultEnum.NoError

                If Not DataLayer.roSupport.IsXSSSafe(oSurvey) Then
                    oState.Result = SurveyResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                If (oSurvey.Id > 0) Then
                    bolIsNew = False
                    sqlCommand = $"@UPDATE# Surveys SET [Title]=@title, " &
                                        " [ModifiedOn]=@modifiedon, [SurveyMode]=@surveymode, [SentOn]=@senton, [Content]=@content, [Anonymous]=@anonymous, " &
                                        " [Mandatory]=@mandatory, [ResponseMaxPercentage]=@responsemaxpercentage, " &
                                        " [ExpirationDate]=@expirationdate, [Status]=@status WHERE [Id] = @id"
                Else
                    bolIsNew = True
                    sqlCommand = $"@INSERT# INTO Surveys ([Title], [IdCreatedBy], " &
                                        " [CreatedOn], " &
                                        " [ModifiedOn], [SentOn], [Content], " &
                                        " [Mandatory], [ResponseMaxPercentage], " &
                                        " [ExpirationDate], [Status],[Anonymous],[SurveyMode]) " &
                                        " OUTPUT INSERTED.ID " &
                                        " VALUES (@title, @idcreatedby, " &
                                        " @createdon, " &
                                        " @modifiedon, @senton, @content, " &
                                        " @mandatory, @responsemaxpercentage, " &
                                        " @expirationdate, @status,@anonymous, @surveymode)"
                End If

                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oSurvey.Id))
                parameters.Add(New CommandParameter("@title", CommandParameter.ParameterType.tString, oSurvey.Title))
                If bolIsNew Then parameters.Add(New CommandParameter("@idcreatedby", CommandParameter.ParameterType.tInt, oSurvey.CreatedBy))
                If bolIsNew Then parameters.Add(New CommandParameter("@createdon", CommandParameter.ParameterType.tDateTime, oSurvey.CreatedOn))
                parameters.Add(New CommandParameter("@modifiedon", CommandParameter.ParameterType.tDateTime, oSurvey.ModifiedOn))
                If oSurvey.SentOn = Date.MinValue Then
                    parameters.Add(New CommandParameter("@senton", CommandParameter.ParameterType.tDateTime, New Date(1900, 1, 1)))
                Else
                    parameters.Add(New CommandParameter("@senton", CommandParameter.ParameterType.tDateTime, oSurvey.SentOn))
                End If
                parameters.Add(New CommandParameter("@content", CommandParameter.ParameterType.tString, roTypes.Any2String(oSurvey.Content)))
                parameters.Add(New CommandParameter("@mandatory", CommandParameter.ParameterType.tBoolean, oSurvey.IsMandatory))
                parameters.Add(New CommandParameter("@anonymous", CommandParameter.ParameterType.tBoolean, oSurvey.Anonymous))
                parameters.Add(New CommandParameter("@surveymode", CommandParameter.ParameterType.tBoolean, oSurvey.AdvancedMode))
                parameters.Add(New CommandParameter("@responsemaxpercentage", CommandParameter.ParameterType.tInt, oSurvey.ResponseMaxPercentage))
                parameters.Add(New CommandParameter("@expirationdate", CommandParameter.ParameterType.tDateTime, oSurvey.ExpirationDate))

                parameters.Add(New CommandParameter("@status", CommandParameter.ParameterType.tInt, oSurvey.Status))

                Try
                    Dim iNew As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sqlCommand, parameters))
                    bolRet = True
                    If bolIsNew Then oSurvey.Id = iNew
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roSurveyManager::ErrorCreatingOrUpdatingSurvey")
                    oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                End Try

                If (oState.Result = SurveyResultEnum.NoError) Then
                    ' 1.- Guardamos empleados destinatarios
                    If Not bolIsNew Then
                        sqlCommand = "@DELETE# SurveyEmployees WHERE IdSurvey = " & oSurvey.Id.ToString
                        bolRet = ExecuteSql(sqlCommand)
                    End If

                    If bolRet AndAlso Not oSurvey.Employees Is Nothing AndAlso oSurvey.Employees.Length > 0 Then
                        For Each idEmployee As Integer In oSurvey.Employees
                            sqlCommand = "@INSERT# INTO SurveyEmployees (IdSurvey, IdEmployee) VALUES (" & oSurvey.Id.ToString & "," & idEmployee.ToString & ")"
                            bolRet = ExecuteSql(sqlCommand)
                            If Not bolRet Then
                                oState.Result = SurveyResultEnum.ErrorSettingSurveyEmployees
                                Exit For
                            End If
                            If oSurvey.Status = SurveyStatusEnum.Online Then
                                VTBase.Extensions.roPushNotification.SendNotificationPushToPassport(idEmployee, LoadType.Employee, Me.oState.Language.Translate("Push.SurveySubject", "", False), Me.oState.Language.Translate("Push.SurveyBody", "", False))
                            End If
                        Next
                    End If

                    ' 2.- Guardamos grupos destino
                    If Not bolIsNew Then
                        sqlCommand = "@DELETE# SurveyGroups WHERE IdSurvey = " & oSurvey.Id.ToString
                        bolRet = ExecuteSql(sqlCommand)
                    End If

                    If bolRet AndAlso Not oSurvey.Groups Is Nothing AndAlso oSurvey.Groups.Length > 0 Then
                        For Each idGroup As Integer In oSurvey.Groups
                            sqlCommand = "@INSERT# INTO SurveyGroups (IdSurvey, IdGroup) VALUES (" & oSurvey.Id.ToString & "," & idGroup.ToString & ")"
                            bolRet = ExecuteSql(sqlCommand)
                            If Not bolRet Then
                                oState.Result = SurveyResultEnum.ErrorSettingSurveyGroups
                                Exit For
                            End If
                        Next
                    End If
                End If

                If (oState.Result = SurveyResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(If(bolIsNew, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tSurvey, oSurvey.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::CreateOrUpdateSurvey")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::CreateOrUpdateSurvey")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = SurveyResultEnum.NoError)
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Actualiza la definición de una encuesta
        ''' </summary>
        ''' <param name="oSurvey"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function UpdateSurveyContent(oSurvey As roSurvey, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = True

            Try
                oState.Result = SurveyResultEnum.NoError

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                If (oSurvey.Id > 0) Then
                    bolIsNew = False
                    sqlCommand = $"@UPDATE# Surveys SET [Content]=@content WHERE [Id] = @id"
                End If

                parameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oSurvey.Id))
                parameters.Add(New CommandParameter("@content", CommandParameter.ParameterType.tString, oSurvey.Content))

                If Not AccessHelper.ExecuteSql(sqlCommand, parameters) Then
                    oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                End If

                If (oState.Result = SurveyResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tSurvey, oSurvey.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::UpdateSurveyContent")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::UpdateSurveyContent")
            End Try
            Return bolRet
        End Function

        ''' <summary>
        ''' Elimina una encuesta y todo lo relacionado con ella
        ''' </summary>
        ''' <param name="oSurvey"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function DeleteSurvey(oSurvey As roSurvey, Optional bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Dim strSQL As String = String.Empty

            Try
                If oSurvey.Id <= 0 Then Return True

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                oState.Result = SurveyResultEnum.ErrorDeletingSurvey

                Dim sToDoTasksIdsToDelete As String = String.Empty
                Dim sSQL As String

                ' 1.- Borramos destinatarios
                strSQL = "@DELETE# SurveyEmployees WHERE IdSurvey = " & oSurvey.Id.ToString
                bolRet = ExecuteSql(strSQL)

                ' 2.- Borramos grupos destino
                If bolRet Then
                    strSQL = "@DELETE# SurveyGroups WHERE IdSurvey = " & oSurvey.Id.ToString
                    bolRet = ExecuteSql(strSQL)
                End If

                ' 2.- Borramos respuestas
                If bolRet Then
                    strSQL = "@DELETE# SurveyEmployeeResponses WHERE IdSurvey = " & oSurvey.Id.ToString
                    bolRet = ExecuteSql(strSQL)
                End If

                sSQL = "@DELETE# Surveys WHERE Id = " & oSurvey.Id
                bolRet = ExecuteSql(sSQL)

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tSurvey, oSurvey.Title, tbParameters, -1)
                End If

                oState.Result = SurveyResultEnum.NoError
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::DeleteSurvey")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::DeleteSurvey")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        Public Function GetSurveyTemplates(Optional bAudit As Boolean = False) As roSurveyTemplates

            Dim strSQL As String
            Dim oRet As New roSurveyTemplates
            Dim lSurveyTemplates As List(Of roSurveyTemplate) = New List(Of roSurveyTemplate)

            Dim tmpLang As New roLanguage
            tmpLang.SetLanguageReference("SurveyService", Me.oState.Language.LanguageKey)

            Try

                oState.Result = SurveyResultEnum.NoError

                Dim tb As DataTable
                Dim sTemplate As roSurveyTemplate = Nothing

                strSQL = "@SELECT# * FROM SurveyTemplates"

                tb = CreateDataTable(strSQL)
                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    oRet = New roSurveyTemplates
                    For Each oRow As DataRow In tb.Rows
                        sTemplate = New roSurveyTemplate
                        sTemplate.Id = roTypes.Any2Integer(oRow("Id"))
                        sTemplate.Key = roTypes.Any2String(oRow("Key"))
                        If sTemplate.Key <> "" Then
                            sTemplate.DefaultTitle = tmpLang.Translate("templateName." & sTemplate.Key, "")
                        Else
                            sTemplate.DefaultTitle = roTypes.Any2String(oRow("DefaultTitle"))
                        End If
                        sTemplate.Content = roTypes.Any2String(oRow("Content"))
                        lSurveyTemplates.Add(sTemplate)
                    Next
                End If

                oRet.Templates = lSurveyTemplates.ToArray
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveyTemplates
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyTemplates")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveyTemplates
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyTemplates")
            Finally

            End Try
            Return oRet
        End Function

        Public Function GetSurveyResponses(idSurvey As Integer, Optional bAudit As Boolean = False) As roSurveyResponses

            Dim strSQL As String
            Dim oRet As New roSurveyResponses
            Dim lSurveyResponses As List(Of String) = New List(Of String)
            Dim oSurvey As New roSurvey

            Try

                oState.Result = SurveyResultEnum.NoError

                oSurvey = Me.GetSurvey(idSurvey, False)

                Dim tb As DataTable
                Dim sResponse As String = Nothing

                strSQL = "@SELECT# * FROM SurveyEmployeeResponses WHERE IdSurvey = " & idSurvey

                tb = CreateDataTable(strSQL)
                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    oRet = New roSurveyResponses
                    For Each oRow As DataRow In tb.Rows
                        sResponse = roTypes.Any2String(oRow("SurveyResponse"))
                        lSurveyResponses.Add(sResponse)
                    Next
                End If

                oRet.ResultCount = lSurveyResponses.Count
                oRet.Data = lSurveyResponses.ToArray

                If (oState.Result = SurveyResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tSurveyResults, oSurvey.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveyResponses
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyResponses")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveyResponses
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyResponses")
            Finally

            End Try
            Return oRet
        End Function

        Public Function GetSurveyResponsesByIdEmployee(idSurvey As Integer, idEmployee As Integer(), Optional bAudit As Boolean = False) As roSurveyResponses

            Dim strSQL As String
            Dim oRet As New roSurveyResponses
            Dim lSurveyResponses As List(Of String) = New List(Of String)
            Dim oSurvey As New roSurvey

            Try

                oState.Result = SurveyResultEnum.NoError

                oSurvey = Me.GetSurvey(idSurvey, False)

                Dim tb As DataTable
                Dim sResponse As String = Nothing

                strSQL = "@SELECT# * FROM SurveyEmployeeResponses INNER JOIN Surveys ON Surveys.Id = SurveyEmployeeResponses.IdSurvey WHERE IdSurvey = " & idSurvey & " AND IdEmployee IN (" & String.Join(",", idEmployee) & ") AND Surveys.Anonymous = 0"

                tb = CreateDataTable(strSQL)
                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    oRet = New roSurveyResponses
                    For Each oRow As DataRow In tb.Rows
                        sResponse = roTypes.Any2String(oRow("SurveyResponse"))
                        lSurveyResponses.Add(sResponse)
                    Next
                End If

                oRet.ResultCount = lSurveyResponses.Count
                oRet.Data = lSurveyResponses.ToArray

                If (oState.Result = SurveyResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tSurveyResults, oSurvey.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveyResponses
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyResponsesByIdEmployee")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorRecoveringAllSurveyResponses
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyResponsesByIdEmployee")
            Finally

            End Try
            Return oRet
        End Function

        ''' <summary>
        ''' Guarda la respuesta de un usuario a una encuesta
        ''' </summary>
        ''' <param name="oSurveyResponse"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveSurveyResponse(oSurveyResponse As roSurveyResponse, Optional bAudit As Boolean = False) As Boolean
            Dim ret As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                Dim oSurvey As New roSurvey
                oState.Result = SurveyResultEnum.NoError

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                sqlCommand = $"@INSERT# INTO SurveyEmployeeResponses ([IdSurvey],[IdEmployee],[SurveyResponse],[ResponseTimeStamp]) " &
                                        " VALUES (@idsurvey, @idemployee,@surveyresponse,@surveytimestamp)"

                parameters.Add(New CommandParameter("@idsurvey", CommandParameter.ParameterType.tInt, oSurveyResponse.IdSurvey))
                parameters.Add(New CommandParameter("@idemployee", CommandParameter.ParameterType.tInt, oSurveyResponse.IdEmployee))
                parameters.Add(New CommandParameter("@surveyresponse", CommandParameter.ParameterType.tString, oSurveyResponse.ResponseData))
                parameters.Add(New CommandParameter("@surveytimestamp", CommandParameter.ParameterType.tDateTime, oSurveyResponse.Timestamp))

                Try
                    If Not ExecuteSql(sqlCommand, parameters) Then
                        oState.Result = SurveyResultEnum.ErrorSavingSurveyResponse
                    Else
                        ' Actualizo el estado de la encuesta si se cierra por tanto por ciento de respuestas
                        oSurvey = GetSurvey(oSurveyResponse.IdSurvey, True)
                        If ShouldSurveyBeCompleted(oSurvey) Then
                            oSurvey.Status = SurveyStatusEnum.Expired
                            If Not CreateOrUpdateSurvey(oSurvey) Then
                                oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                            End If
                        End If
                    End If
                Catch ex As Exception
                    oState.Result = SurveyResultEnum.ErrorSavingSurveyResponse
                End Try

                If (oState.Result = SurveyResultEnum.NoError) AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tSurveyResponse, oSurvey.Title, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::CreateOrUpdateSurvey")
            Catch ex As Exception
                oState.Result = SurveyResultEnum.ErrorCreatingOrUpdatingSurvey
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::CreateOrUpdateSurvey")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, oState.Result = SurveyResultEnum.NoError)
            End Try
            Return ret
        End Function

        ''' <summary>
        ''' Devuelve el tanto por ciento de respuestas recibidas
        ''' </summary>
        ''' <param name="oSurvey"></param>
        ''' <returns></returns>
        Private Function GetSurveyStatus(oSurvey As roSurvey, ByRef iTotal As Integer, ByRef iResponses As Integer, ByRef lResponses As List(Of roSurveyEmployee), ByVal bLoadDetails As Boolean) As Integer
            Dim dRet As Double = 0
            Dim retSurvey As New roSurvey
            Dim strSQL As String = String.Empty

            Try
                strSQL = "@SELECT# IdEmployee, Name, GroupName, SurveyResponse FROM sysrovwSurveyStatistics WHERE IdSurvey = " & oSurvey.Id
                Dim dTable As New DataTable
                dTable = CreateDataTable(strSQL)
                If Not dTable Is Nothing AndAlso dTable.Rows.Count > 0 Then
                    iTotal = dTable.Rows.Count
                    Dim dView As New DataView(dTable)
                    dView.RowFilter = "SurveyResponse Is Not Null"
                    iResponses = dView.ToTable.Rows.Count
                    If bLoadDetails Then
                        For Each oRow As DataRow In dView.ToTable.Rows
                            Dim sEmployeeName As String = String.Empty
                            sEmployeeName = roTypes.Any2String(oRow("Name")) + " (" + roTypes.Any2String(oRow("GroupName")) + ")"
                            lResponses.Add(New roSurveyEmployee(oRow("IdEmployee"), sEmployeeName))
                        Next
                    End If
                Else
                    Return 0
                End If

                dRet = (iResponses * 100) / iTotal
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyStatus")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::GetSurveyStatus")
            End Try

            Return Math.Round(dRet)
        End Function

        ''' <summary>
        ''' Devuelve el tanto por ciento de respuestas recibidas
        ''' </summary>
        ''' <param name="oSurvey"></param>
        ''' <returns></returns>
        Private Function ShouldSurveyBeCompleted(oSurvey As roSurvey) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Si respondió todo el mundo
                If oSurvey.CurrentPercentage = 100 Then Return True

                ' Si va por tanto por ciento y se ha alcanzado el configurado
                If oSurvey.ResponseMaxPercentage > 0 AndAlso oSurvey.CurrentPercentage >= oSurvey.ResponseMaxPercentage Then Return True

                ' Si va por fecha y ya se ha alcanzado
                If oSurvey.ResponseMaxPercentage = 0 AndAlso oSurvey.ExpirationDate < Now.Date Then Return True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roSurveyManager::ShouldSurveyBeCompleted")
            End Try

            Return bolRet
        End Function

#End Region

    End Class

End Namespace