Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Security.Base

Namespace Analytics.Manager

    <DataContract()>
    Public Class roGeniusViewManager

#Region "Declarations - Constructor"

        Private oState As roAnalyticState

        Public Sub New()
            Me.oState = New roAnalyticState
        End Sub

        Public Sub New(ByVal _State As roAnalyticState)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Estado de la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <IgnoreDataMember>
        Public Property State() As roAnalyticState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAnalyticState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(ByVal _ID As Integer, Optional ByVal bolAudit As Boolean = True, Optional ByVal bTranslateName As Boolean = True, Optional ByVal bIgnorePermissions As Boolean = False) As roGeniusView

            Dim bolRet As New roGeniusView With {
                .Id = _ID
            }

            Try

                Dim strSQL As String = "@SELECT# ID,IdPassport,Name,Description,DS,TypeView,CreatedOn,Employees,DateFilterType,DateInf,DateSup,CubeLayout,Concepts,UserFields,BusinessCenters,CustomFields,DSFunction,Feature,RequieredFeature,CheckedCheckBoxes,RequieredLicense,ContextMenuOptions,Causes,IdSystemView " &
                                        " FROM GeniusViews WHERE ID = " & bolRet.Id.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                Dim tmpLang As New roLanguage
                tmpLang.SetLanguageReference("LiveGenius", Me.oState.Language.LanguageKey)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Id = roTypes.Any2Integer(oRow("Id"))
                    bolRet.IdPassport = roTypes.Any2Integer(oRow("IdPassport"))
                    bolRet.RequieredFeature = roTypes.Any2String(oRow("RequieredFeature"))
                    bolRet.RequieredLicense = roTypes.Any2String(oRow("RequieredLicense"))
                    bolRet.DSFunction = roTypes.Any2String(oRow("DSFunction"))

                    If (bIgnorePermissions OrElse bolRet.RequieredFeature = String.Empty OrElse Security.WLHelper.HasPermissionOverFeature(oState.IDPassport, bolRet.RequieredFeature, "U", Permission.Read)) And (Not bolRet.DSFunction.Contains("Genius_Supervisors") OrElse True) Then
                        If bolRet.IdPassport = 0 AndAlso bTranslateName = True Then
                            Dim tmpName As String = roTypes.Any2String(oRow("Name"))
                            bolRet.Name = tmpLang.Translate("templateName." & tmpName, "")
                            If bolRet.Name.ToUpper.Contains("NOTFOUND") Then bolRet.Name = tmpName
                        Else
                            bolRet.Name = roTypes.Any2String(oRow("Name"))
                        End If

                        bolRet.Description = roTypes.Any2String(oRow("Description"))
                        bolRet.DS = roConstants.GetGeniusEnumFromCode(roTypes.Any2String(oRow("DS")))
                        bolRet.TypeView = roTypes.Any2Integer(oRow("TypeView"))
                        bolRet.CreatedOn = roTypes.Any2DateTime(oRow("CreatedOn"))

                        bolRet.DateFilterType = roTypes.Any2String(oRow("DateFilterType"))
                        bolRet.DateInf = roTypes.Any2DateTime(oRow("DateInf"))
                        bolRet.DateSup = roTypes.Any2DateTime(oRow("DateSup"))

                        bolRet.CubeLayout = roTypes.Any2String(oRow("CubeLayout"))
                        bolRet.Concepts = roTypes.Any2String(oRow("Concepts"))
                        bolRet.Causes = roTypes.Any2String(oRow("Causes"))
                        bolRet.UserFields = roTypes.Any2String(oRow("UserFields"))
                        bolRet.BusinessCenters = roTypes.Any2String(oRow("BusinessCenters"))
                        bolRet.Employees = roTypes.Any2String(oRow("Employees"))
                        bolRet.DSFunction = roTypes.Any2String(oRow("DSFunction"))
                        bolRet.Feature = roTypes.Any2String(oRow("Feature"))

                        If roTypes.Any2String(oRow("CustomFields")) = String.Empty Then
                            bolRet.CustomFields = New roGeniusCustomFields
                        Else
                            bolRet.CustomFields = roJSONHelper.DeserializeNewtonSoft(roTypes.Any2String(oRow("CustomFields")), GetType(roGeniusCustomFields))
                        End If

                        bolRet.Executions = LoadExecutions(bolRet.Id, bolRet.CustomFields.DownloadBI)
                        If Not IsDBNull(oRow("CheckedCheckboxes")) AndAlso Not String.IsNullOrEmpty(oRow("CheckedCheckboxes")) Then bolRet.CheckedCheckBoxes = roTypes.Any2IntegerArray(oRow("CheckedCheckboxes"))
                        If Not IsDBNull(oRow("IdSystemView")) Then bolRet.IdSystemView = roTypes.Any2Integer(oRow("IdSystemView"))
                        bolRet.ContextMenu = roTypes.Any2String(oRow("ContextMenuOptions"))
                    Else
                        bolRet = Nothing
                    End If
                End If

                If bolRet IsNot Nothing AndAlso bolAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tGenius, bolRet.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function GetSharedList(ByVal idView As Integer) As List(Of String)

            Dim lstRet As New List(Of String)

            Try

                Dim strSQL As String = "@SELECT# DISTINCT p.id, p.Name from sysroPassports p JOIN GeniusViews gv ON gv.IdPassport = p.ID" &
                                " WHERE gv.IdPassport IN (@SELECT# sgv.IdPassport from GeniusViews sgv where sgv.IdParentShared = " & idView & ")"
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        lstRet.Add(roTypes.Any2String(oRow("Name")))
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::LoadExecutions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::LoadExecutions")
            Finally

            End Try

            Return lstRet

        End Function

        Public Function LoadByCheckBoxes(ByVal _CheckBoxes As Integer(), Optional ByVal bolAudit As Boolean = True, Optional ByVal bTranslateName As Boolean = True, Optional ByVal bIgnorePermissions As Boolean = False) As roGeniusView

            Dim bolRet As New roGeniusView()

            Try

                Dim _CheckBoxesStr As String = ""

                For Each checkbox As String In _CheckBoxes
                    If _CheckBoxesStr = "" Then
                        _CheckBoxesStr = checkbox
                    Else
                        _CheckBoxesStr += ", " + checkbox
                    End If
                Next

                Dim strSQL As String = "@SELECT# ID,IdPassport,Name,Description,DS,TypeView,CreatedOn,Employees,DateFilterType,DateInf,DateSup,CubeLayout,Concepts,Causes,UserFields,BusinessCenters,CustomFields,DSFunction,Feature,RequieredFeature FROM GeniusCustomReportCombs as RC inner join GeniusCustomReportViews RV on RC.Class = RV.Class inner join GeniusViews as GV on GV.Name = RV.NameViewResult
WHERE idpassport IN (-1,0) and (idcheck in (" & _CheckBoxesStr & ")) and (@SELECT# count(*) from GeniusCustomReportCombs where class = rc.class) = " & _CheckBoxes.Length.ToString() & "
                and (@SELECT# count(*) from GeniusCustomReportCombs where class = rc.class and idcheck not in (" & _CheckBoxesStr & ")) = 0"

                Dim tb As DataTable = CreateDataTable(strSQL)

                Dim tmpLang As New roLanguage
                tmpLang.SetLanguageReference("LiveGenius", Me.oState.Language.LanguageKey)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Id = roTypes.Any2Integer(oRow("Id"))
                    bolRet.IdPassport = roTypes.Any2Integer(oRow("IdPassport"))
                    bolRet.RequieredFeature = roTypes.Any2String(oRow("RequieredFeature"))

                    If bIgnorePermissions OrElse bolRet.RequieredFeature = String.Empty OrElse Security.WLHelper.HasPermissionOverFeature(oState.IDPassport, bolRet.RequieredFeature, "U", Permission.Read) Then
                        If bolRet.IdPassport = 0 AndAlso bTranslateName = True Then
                            Dim tmpName As String = roTypes.Any2String(oRow("Name"))
                            bolRet.Name = tmpLang.Translate("templateName." & tmpName, "")
                            If bolRet.Name.ToUpper.Contains("NOTFOUND") Then bolRet.Name = tmpName
                        Else
                            bolRet.Name = roTypes.Any2String(oRow("Name"))
                        End If

                        bolRet.Description = roTypes.Any2String(oRow("Description"))
                        bolRet.DS = roConstants.GetGeniusEnumFromCode(roTypes.Any2String(oRow("DS")))
                        bolRet.TypeView = roTypes.Any2Integer(oRow("TypeView"))
                        bolRet.CreatedOn = roTypes.Any2DateTime(oRow("CreatedOn"))

                        bolRet.DateFilterType = roTypes.Any2String(oRow("DateFilterType"))
                        bolRet.DateInf = roTypes.Any2DateTime(oRow("DateInf"))
                        bolRet.DateSup = roTypes.Any2DateTime(oRow("DateSup"))

                        bolRet.CubeLayout = roTypes.Any2String(oRow("CubeLayout"))
                        bolRet.Concepts = roTypes.Any2String(oRow("Concepts"))
                        bolRet.Causes = roTypes.Any2String(oRow("Causes"))
                        bolRet.UserFields = roTypes.Any2String(oRow("UserFields"))
                        bolRet.BusinessCenters = roTypes.Any2String(oRow("BusinessCenters"))
                        bolRet.Employees = roTypes.Any2String(oRow("Employees"))
                        bolRet.DSFunction = roTypes.Any2String(oRow("DSFunction"))
                        bolRet.Feature = roTypes.Any2String(oRow("Feature"))

                        If roTypes.Any2String(oRow("CustomFields")) = String.Empty Then
                            bolRet.CustomFields = New roGeniusCustomFields
                        Else
                            bolRet.CustomFields = roJSONHelper.DeserializeNewtonSoft(roTypes.Any2String(oRow("CustomFields")), GetType(roGeniusCustomFields))
                        End If

                        bolRet.Executions = LoadExecutions(bolRet.Id, bolRet.CustomFields.DownloadBI)
                    Else
                        bolRet = Nothing
                    End If
                End If

                If bolRet IsNot Nothing AndAlso bolAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tGenius, bolRet.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function LoadByTask(ByVal _IDTask As Integer, Optional ByVal bolAudit As Boolean = True) As roGeniusView

            Dim bolRet As New roGeniusView With {
                .Id = -1
            }

            Try

                Dim strSQL As String = "@SELECT# IDGeniusView FROM GeniusExecutions WHERE IDTask = " & _IDTask & " AND ExecutionDate >= " & roTypes.SQLSmallDateTime(Now.Date.AddDays(-3))

                Dim iId = roTypes.Any2Integer(ExecuteScalar(strSQL))

                bolRet = Me.Load(iId, False)

                If bolRet IsNot Nothing Then
                    bolRet.Executions = bolRet.Executions.FindAll(Function(x) x.IdTask = _IDTask)
                End If

                If bolRet IsNot Nothing AndAlso bolAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{GeniusViewName}", bolRet.Name, String.Empty, 1)
                    oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tGenius, bolRet.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function LoadExecutions(ByVal idView As Integer, Optional ByVal downloadBI As Boolean = False) As List(Of roGeniusExecution)

            Dim lstRet As New List(Of roGeniusExecution)

            Try
                Dim strSQL As String = "@SELECT# ID,IdGeniusView,FileLink,CubeLayout,ExecutionDate,IdTask, SASLink FROM geniusExecutions where IdGeniusView =" & idView & " AND ExecutionDate >= " & roTypes.SQLSmallDateTime(Now.Date.AddDays(-3)) & " Order By ExecutionDate Desc"
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If (downloadBI) Then
                        For Each oRow As DataRow In tb.Rows
                            lstRet.Add(New roGeniusExecution() With {
                                .ExecutionDate = roTypes.Any2DateTime(oRow("ExecutionDate")),
                                .IdGeniusView = roTypes.Any2Integer(oRow("IdGeniusView")),
                                .CubeLayout = roTypes.Any2String(oRow("CubeLayout")),
                                .FileLink = roTypes.Any2String(oRow("FileLink")),
                                .Id = roTypes.Any2Integer(oRow("Id")),
                                .IdTask = roTypes.Any2Integer(oRow("IdTask")),
                                .AzureSaSKey = Azure.RoAzureSupport.GetFileSaSTokenWithURI(roTypes.Any2String(oRow("FileLink")), roLiveQueueTypes.analyticsbi, True, Azure.RoAzureSupport.GetCompanyName()),
                                .SASLink = roTypes.Any2String(oRow("SASLink"))
                            })
                        Next
                    Else
                        For Each oRow As DataRow In tb.Rows
                            lstRet.Add(New roGeniusExecution() With {
                                .ExecutionDate = roTypes.Any2DateTime(oRow("ExecutionDate")),
                                .IdGeniusView = roTypes.Any2Integer(oRow("IdGeniusView")),
                                .CubeLayout = roTypes.Any2String(oRow("CubeLayout")),
                                .FileLink = roTypes.Any2String(oRow("FileLink")),
                                .Id = roTypes.Any2Integer(oRow("Id")),
                                .IdTask = roTypes.Any2Integer(oRow("IdTask"))
                            })
                        Next
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::LoadExecutions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::LoadExecutions")
            Finally

            End Try

            Return lstRet

        End Function

        Public Function Validate(ByVal oSchedule As roGeniusView) As Boolean

            Dim bolRet As Boolean = True

            Try

                Return True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Share(ByRef oSchedule As roGeniusView, ByVal idPassport As Integer, Optional ByVal bAudit As Boolean = False, Optional ByVal idParentShared As Integer = Nothing) As Boolean
            Dim bolRet As Boolean = False

            Try

                If idParentShared.Equals(Nothing) Then
                    oSchedule.IdParentShared = oSchedule.Id
                Else
                    oSchedule.IdParentShared = idParentShared
                End If
                'oSchedule.IdParentShared = idParentShared |oSchedule.Id
                oSchedule.Id = -1
                oSchedule.IdPassport = idPassport
                oSchedule.Employees = String.Empty
                oSchedule.BusinessCenters = String.Empty

                oSchedule.Executions = New List(Of roGeniusExecution)

                Dim checkSQLName As String = "@SELECT# count(*) FROM geniusViews where IdPassport =" & oSchedule.IdPassport & " And Name like '" & oSchedule.Name & "%'"
                Dim iTemplateCount = roTypes.Any2Integer(ExecuteScalar(checkSQLName))
                If iTemplateCount > 0 Then
                    oSchedule.Name = oSchedule.Name & "(" & (iTemplateCount) & ")"
                End If

                bolRet = Save(oSchedule, bAudit, True)
            Catch ex As Exception
            Finally

            End Try

            Return bolRet
        End Function

        Public Function Save(ByRef oSchedule As roGeniusView, Optional ByVal bAudit As Boolean = False, Optional ByVal bIgnoreRoboticsUser As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Not DataLayer.roSupport.IsXSSSafe(oSchedule) Then
                    oState.Result = AnalyticsResultEnum.XSSvalidationError
                    Return False
                End If

                Dim bolIsNew As Boolean = False
                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)
                If (oSchedule.Id > 0) Then
                    bolIsNew = False
                    sqlCommand = $"@UPDATE# GeniusViews SET [IdPassport]=@IdPassport, [Name]=@Name, [Description]=@Description, [DS]=@dS, [TypeView]=@typeView,[CreatedOn]=@CreatedOn, [Employees]=@Employees, " &
                                        "[DateFilterType]=@DateFilterType, [DateInf]=@DateInf,[DateSup]=@DateSup, [CubeLayout]=@CubeLayout, [Concepts]=@Concepts, [UserFields]=@UserFields, [BusinessCenters]=@BusinessCenters, " &
                                        "[CustomFields]=@customFields,[DSFunction]=@DSFunction,[Feature]=@Feature,[RequieredFeature]=@RequieredFeature, [RequieredLicense]=@RequieredLicense, [IdParentShared]=@IdParentShared, " &
                                        "[Causes]=@causes,[CheckedCheckboxes]=@CheckedCheckboxes, [ContextMenuOptions]=@ContextMenuOptions WHERE ID=@Id"

                    parameters.Add(New CommandParameter("@Id", CommandParameter.ParameterType.tInt, oSchedule.Id))
                Else
                    bolIsNew = True
                    sqlCommand = $"@INSERT# INTO GeniusViews ([IdPassport],[Name],[Description],[DS],[TypeView],[CreatedOn],[Employees],[DateFilterType],[DateInf],[DateSup],[CubeLayout],[Concepts],[UserFields],[BusinessCenters],[CustomFields],[DSFunction],[Feature],[RequieredFeature],[RequieredLicense],[IdParentShared],[CheckedCheckboxes],[ContextMenuOptions],[Causes],[IdSystemView]) " &
                                 " output INSERTED.ID VALUES (@IdPassport, @Name, @Description, @dS, @typeView, @CreatedOn, @Employees, @DateFilterType, @DateInf,@DateSup, @CubeLayout, @Concepts, @UserFields, @BusinessCenters, @customFields,@DSFunction,@Feature,@RequieredFeature,@RequieredLicense,@IdParentShared,@CheckedCheckboxes,@ContextMenuOptions,@causes,@IdSystemView)"
                End If

                parameters.Add(New CommandParameter("@Description", CommandParameter.ParameterType.tString, roTypes.Any2String(oSchedule.Description)))
                parameters.Add(New CommandParameter("@ds", CommandParameter.ParameterType.tString, VTBase.roConstants.GetGeniusCodeFromEnum(oSchedule.DS)))
                parameters.Add(New CommandParameter("@typeView", CommandParameter.ParameterType.tInt, oSchedule.TypeView))
                parameters.Add(New CommandParameter("@CreatedOn", CommandParameter.ParameterType.tDateTime, oSchedule.CreatedOn))
                parameters.Add(New CommandParameter("@Employees", CommandParameter.ParameterType.tString, oSchedule.Employees))
                parameters.Add(New CommandParameter("@DateFilterType", CommandParameter.ParameterType.tString, oSchedule.DateFilterType))
                parameters.Add(New CommandParameter("@DateInf", CommandParameter.ParameterType.tDateTime, oSchedule.DateInf))
                parameters.Add(New CommandParameter("@DateSup", CommandParameter.ParameterType.tDateTime, oSchedule.DateSup))
                parameters.Add(New CommandParameter("@Concepts", CommandParameter.ParameterType.tString, roTypes.Any2String(oSchedule.Concepts)))
                parameters.Add(New CommandParameter("@UserFields", CommandParameter.ParameterType.tString, roTypes.Any2String(oSchedule.UserFields)))
                parameters.Add(New CommandParameter("@BusinessCenters", CommandParameter.ParameterType.tString, roTypes.Any2String(oSchedule.BusinessCenters)))
                parameters.Add(New CommandParameter("@customFields", CommandParameter.ParameterType.tString, roJSONHelper.SerializeNewtonSoft(oSchedule.CustomFields)))
                parameters.Add(New CommandParameter("@DSFunction", CommandParameter.ParameterType.tString, oSchedule.DSFunction))
                parameters.Add(New CommandParameter("@Feature", CommandParameter.ParameterType.tString, oSchedule.Feature))
                'parameters.Add(New CommandParameter("@CubeLayout", CommandParameter.ParameterType.tString, Me.TranslateCubeLayout(oSchedule.CubeLayout, oSchedule.DS, oSchedule.TypeView, oSchedule.UserFields.Split(","), oSchedule.CustomFields.LanguageKey, oSchedule.DSFunction)))
                parameters.Add(New CommandParameter("@CubeLayout", CommandParameter.ParameterType.tString, oSchedule.CubeLayout))
                parameters.Add(New CommandParameter("@RequieredFeature", CommandParameter.ParameterType.tString, oSchedule.RequieredFeature))
                parameters.Add(New CommandParameter("@RequieredLicense", CommandParameter.ParameterType.tString, roTypes.Any2String(oSchedule.RequieredLicense)))
                parameters.Add(New CommandParameter("@IdParentShared", CommandParameter.ParameterType.tInt, oSchedule.IdParentShared))
                parameters.Add(New CommandParameter("@CheckedCheckboxes", CommandParameter.ParameterType.tString, roTypes.IntegerArray2String(oSchedule.CheckedCheckBoxes)))
                parameters.Add(New CommandParameter("@ContextMenuOptions", CommandParameter.ParameterType.tString, roTypes.Any2String(oSchedule.ContextMenu)))
                parameters.Add(New CommandParameter("@causes", CommandParameter.ParameterType.tString, roTypes.Any2String(oSchedule.Causes)))
                parameters.Add(New CommandParameter("@IdSystemView", CommandParameter.ParameterType.tInt, roTypes.Any2Integer(oSchedule.IdSystemView)))

                Dim oOwner As roPassportTicket = roPassportManager.GetPassportTicket(Me.State.IDPassport, LoadType.Passport)

                parameters.Add(New CommandParameter("@Name", CommandParameter.ParameterType.tString, oSchedule.Name))
                parameters.Add(New CommandParameter("@IdPassport", CommandParameter.ParameterType.tInt, oSchedule.IdPassport))

                Try
                    bolRet = True
                    Dim iNew As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(sqlCommand, parameters))
                    If bolIsNew Then oSchedule.Id = iNew
                Catch ex As Exception
                    oState.Result = AnalyticsResultEnum.SqlError
                    bolRet = False
                End Try

                If bolRet AndAlso bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{GeniusViewName}", oSchedule.Name, String.Empty, 1)
                    oState.Audit(IIf(bolIsNew, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tGenius, oSchedule.Name, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Save")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GetAnalyticExecutionType(ByVal analyticType As AnalyticsTypeEnum) As Integer
            Select Case analyticType
                Case AnalyticsTypeEnum._ACCESS
                    Return 2
                Case AnalyticsTypeEnum._COSTCENTERS
                    Return 1
                Case AnalyticsTypeEnum._PRODUCTIV
                    Return 3
                Case AnalyticsTypeEnum._SCHEDULER
                    Return 0
                Case Else
                    Return -1
            End Select
        End Function

        Public Function Execute(ByVal oScheduleExecution As roGeniusView, Optional ByVal bAudit As Boolean = False, Optional ByVal bLoaded As Boolean = False, Optional ByVal bIsScheduled As Boolean = False) As roGeniusExecution

            Dim bolRet As New roGeniusExecution

            Try

                Dim oSchedule As roGeniusView = Nothing

                If Not bLoaded Then
                    oSchedule = Load(oScheduleExecution.Id, False)
                Else
                    oSchedule = oScheduleExecution
                End If

                If oSchedule Is Nothing Then Return Nothing

                Dim bResult As Boolean = True
                Dim sqlCommand As String = String.Empty
                Dim parameters As New List(Of CommandParameter)

                sqlCommand = $"@INSERT# INTO GeniusExecutions ([IdGeniusView],[FileLink],[ExecutionDate],[CubeLayout],[IDTask],[SASLink]) output INSERTED.ID VALUES (@idGeniusView, @fileLink, @executionDate,@cubeLayout,@idTask,@SASLink)"

                bolRet = New roGeniusExecution() With {
                        .Id = -1,
                        .CubeLayout = oSchedule.CubeLayout,
                        .ExecutionDate = DateTime.Now,
                        .FileLink = "",
                        .IdGeniusView = oSchedule.Id,
                        .SASLink = ""
                    }

                parameters.Add(New CommandParameter("@idGeniusView", CommandParameter.ParameterType.tInt, bolRet.IdGeniusView))
                parameters.Add(New CommandParameter("@fileLink", CommandParameter.ParameterType.tString, ""))
                parameters.Add(New CommandParameter("@executionDate", CommandParameter.ParameterType.tDateTime, bolRet.ExecutionDate))
                parameters.Add(New CommandParameter("@cubeLayout", CommandParameter.ParameterType.tString, bolRet.CubeLayout))
                parameters.Add(New CommandParameter("@idTask", CommandParameter.ParameterType.tInt, -1))
                parameters.Add(New CommandParameter("@SASLink", CommandParameter.ParameterType.tString, ""))

                Try
                    Dim iNew As Integer = DataLayer.AccessHelper.ExecuteScalar(sqlCommand, parameters)
                    bolRet.Id = iNew

                    Dim oTask As New roLiveTask

                    Dim oParameters As New roCollection

                    oParameters.Add("AnalyticType", GetAnalyticExecutionType(oSchedule.DS))
                    oParameters.Add("IdView", oSchedule.Id)
                    oParameters.Add("IdLayout", bolRet.Id)
                    oParameters.Add("ScheduleBeginDate", oScheduleExecution.DateInf.ToString("yyyy/MM/dd HH:mm"))
                    oParameters.Add("ScheduleEndDate", oScheduleExecution.DateSup.ToString("yyyy/MM/dd HH:mm"))
                    If (oScheduleExecution.TimeInf <> Nothing) Then
                        oParameters.Add("ScheduleBeginTime", oScheduleExecution.TimeInf.ToString("yyyy/MM/dd HH:mm"))
                    End If
                    If (oScheduleExecution.TimeSup <> Nothing) Then
                        oParameters.Add("ScheduleEndTime", oScheduleExecution.TimeSup.ToString("yyyy/MM/dd HH:mm"))
                    End If

                    oParameters.Add("Employees", roTypes.Any2String(oSchedule.Employees))
                    oParameters.Add("Concepts", roTypes.Any2String(oSchedule.Concepts))
                    oParameters.Add("Causes", roTypes.Any2String(oSchedule.Causes))
                    oParameters.Add("UserFields", roTypes.Any2String(oSchedule.UserFields))
                    oParameters.Add("BusinessCenters", roTypes.Any2String(oSchedule.BusinessCenters))

                    oParameters.Add("APIVersion", roConstants.GeniusAnalytic)

                    oParameters.Add("IncludeZeroValues", If(oSchedule.CustomFields.IncludeZeros, "1", "0"))
                    oParameters.Add("IncludeZeroCauseValues", If(oSchedule.CustomFields.IncludeZeroCauses, "1", "0"))
                    oParameters.Add("IncludeEntriesWithoutBusinessCenter", If(oSchedule.CustomFields.IncludeZeroBusinessCenter, "1", "0"))
                    oParameters.Add("DSLanguage", roTypes.Any2String(oSchedule.CustomFields.LanguageKey))
                    oParameters.Add("RequestTypes", roTypes.Any2String(oSchedule.CustomFields.RequestTypes))
                    oParameters.Add("Feature", roTypes.Any2String(oSchedule.Feature))
                    oParameters.Add("DSFunction", roTypes.Any2String(oSchedule.DSFunction))
                    oParameters.Add("GeniusViewName", roTypes.Any2String(oSchedule.Name))
                    oParameters.Add("SendEmail", If(oSchedule.CustomFields.SendEmail, "1", "0"))
                    oParameters.Add("OverwriteResults", If(oSchedule.CustomFields.OverwriteResults, "1", "0"))
                    oParameters.Add("DownloadBI", If(oSchedule.CustomFields.DownloadBI, "1", "0"))
                    oParameters.Add("IsScheduled", If(bIsScheduled, "1", "0"))

                    Dim idTask = roLiveTask.CreateLiveTask(roLiveTaskTypes.AnalyticsTask, oParameters, New roLiveTaskState(Me.State.IDPassport), False)

                    sqlCommand = $"@UPDATE# GeniusExecutions set IDTask = @idTask where Id=@id"

                    Dim updateParameters As New List(Of CommandParameter)
                    updateParameters.Add(New CommandParameter("@idTask", CommandParameter.ParameterType.tInt, idTask))
                    updateParameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, bolRet.Id))

                    bolRet.IdTask = idTask

                    DataLayer.AccessHelper.ExecuteSql(sqlCommand, updateParameters)
                    Azure.RoAzureSupport.SendTaskToQueue(idTask, Azure.RoAzureSupport.GetCompanyName(), roLiveTaskTypes.AnalyticsTask)
                Catch ex As Exception
                    oState.Result = AnalyticsResultEnum.SqlError
                End Try

                If bResult And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    bResult = Me.oState.Audit(Audit.Action.aExecuted, Audit.ObjectType.tGenius, oSchedule.Name, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::AddExecutionToGeniusView")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::AddExecutionToGeniusView")
            End Try

            Return bolRet

        End Function

        Public Function Delete(ByVal oSchedule As roGeniusView, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False
            Dim oLicense As New roServerLicense

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                oSchedule = Load(oSchedule.Id, False)

                Dim obolBIIntegrationInstalled As Boolean = oLicense.FeatureIsInstalled("Feature\BIIntegration")

                If oSchedule IsNot Nothing Then
                    For Each oExecution As roGeniusExecution In oSchedule.Executions
                        bolRet = bolRet AndAlso DeleteExecution(oExecution.Id, oExecution.FileLink, , obolBIIntegrationInstalled AndAlso oSchedule.CustomFields.DownloadBI)
                    Next

                    Dim sqlCommand As String = String.Empty
                    Dim deleteParameters As New List(Of CommandParameter)
                    deleteParameters.Add(New CommandParameter("@id", CommandParameter.ParameterType.tInt, oSchedule.Id))

                    If roPassportManager.IsRoboticsUserOrConsultant(Me.State.IDPassport) Then
                        sqlCommand = $"@DELETE# GeniusViews where Id=@id"
                    Else
                        sqlCommand = $"@DELETE# GeniusViews where Id=@id and IdPassport=@idPassport"
                        deleteParameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, Me.State.IDPassport))
                    End If

                    bolRet = DataLayer.AccessHelper.ExecuteSql(sqlCommand, deleteParameters)
                Else
                    bolRet = False
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tGenius, oSchedule.Name, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

        Public Function DeleteExecution(ByVal executionId As Integer, ByVal strFileName As String, Optional ByVal bAudit As Boolean = False, Optional ByVal bDownloadBi As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Try
                Dim DelQuerys As String = "@DELETE# FROM GeniusExecutions WHERE ID = " & executionId.ToString
                If Not ExecuteSql(DelQuerys) Then
                    oState.Result = AnalyticsResultEnum.ConnectionError
                Else
                    If (bDownloadBi) Then
                        Robotics.Azure.RoAzureSupport.DeleteFileFromCompanyContainer(strFileName, "", roLiveQueueTypes.analyticsbi)
                    Else
                        Robotics.Azure.RoAzureSupport.DeleteFileFromAzure(strFileName, roLiveQueueTypes.analytics)
                    End If
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Delete")
                bolRet = False
            Catch ex As Exception
                bolRet = False
                oState.UpdateStateInfo(ex, "roGeniusViewManager::Delete")
            End Try

            Return bolRet

        End Function

        Public Function GetGeniusViewsTemplates(Optional ByVal bAudit As Boolean = False) As Generic.List(Of roGeniusView)

            Dim oRet As New Generic.List(Of roGeniusView)

            Try

                Dim strSQL As String = "@SELECT# ID from GeniusViews Where IdPassport = 0"

                strSQL &= " Order By Name"

                Dim dTbl As DataTable = CreateDataTable(strSQL)

                If dTbl IsNot Nothing Then
                    Dim oManager As New roGeniusViewManager(Me.State)
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oReportScheduler As roGeniusView = oManager.Load(dRow("ID"), False)
                        If (oReportScheduler IsNot Nothing) Then oRet.Add(oReportScheduler)
                    Next
                End If

                If oRet IsNot Nothing AndAlso oRet.Count > 0 Then
                    If bAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = String.Empty
                        For Each oView As roGeniusView In oRet
                            strAuditName &= IIf(strAuditName <> String.Empty, ",", String.Empty) & oView.Name
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{GeniusViewNames}", strAuditName, String.Empty, 1)
                        Me.State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetGeniusViews")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetGeniusViews")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetGeniusViews(Optional ByVal bAudit As Boolean = False) As Generic.List(Of roGeniusView)

            Dim oRet As New Generic.List(Of roGeniusView)

            Try

                Dim strSQL As String = String.Empty

                Dim oOwner As roPassportTicket = roPassportManager.GetPassportTicket(Me.State.IDPassport, LoadType.Passport)

                strSQL = "@SELECT# ID from GeniusViews Where IdPassport =" & Me.State.IDPassport

                strSQL &= " Order By Name"

                Dim dTbl As DataTable = CreateDataTable(strSQL)

                Dim oLicense As New roServerLicense

                If dTbl IsNot Nothing Then
                    Dim oManager As New roGeniusViewManager(Me.State)
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oReportScheduler As roGeniusView = oManager.Load(dRow("ID"), False)
                        If (oReportScheduler IsNot Nothing) Then
                            If oLicense.FeatureIsInstalled("Feature\BIIntegration") Then
                                oRet.Add(oReportScheduler)
                            Else
                                'Si no tenemos licencia BI, no mostraremos los estudios BI
                                If Not oReportScheduler.CustomFields.DownloadBI Then
                                    oRet.Add(oReportScheduler)
                                End If
                            End If
                        End If
                    Next
                End If

                If oRet IsNot Nothing AndAlso oRet.Count > 0 Then
                    If bAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = String.Empty
                        For Each oView As roGeniusView In oRet
                            strAuditName &= IIf(strAuditName <> String.Empty, ",", String.Empty) & oView.Name
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{GeniusViewNames}", strAuditName, String.Empty, 1)
                        Me.State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetGeniusViews")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetGeniusViews")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetGeniusExecutionById(ByVal intExecution As Integer, Optional ByVal bAudit As Boolean = False) As roGeniusExecution

            Dim oRet As roGeniusExecution = Nothing

            Try

                Dim strAuditName As String = String.Empty
                Dim strSQL As String = "@SELECT# top 1 ge.ID,ge.IdGeniusView,ge.FileLink,ge.CubeLayout,ge.ExecutionDate,ge.IdTask,gv.Name, ge.SASLink FROM geniusExecutions ge inner join geniusViews gv on ge.IdGeniusView = gv.Id where ge.Id=" & intExecution

                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oRow As DataRow In tb.Rows
                    strAuditName = oRow("Name")

                    Dim oGenius = Me.Load(roTypes.Any2Integer(oRow("IdGeniusView")))
                    If oGenius IsNot Nothing Then
                        oRet = New roGeniusExecution() With {
                        .ExecutionDate = roTypes.Any2DateTime(oRow("ExecutionDate")),
                        .IdGeniusView = roTypes.Any2Integer(oRow("IdGeniusView")),
                        .CubeLayout = roTypes.Any2String(oRow("CubeLayout")),
                        .FileLink = roTypes.Any2String(oRow("FileLink")),
                        .Id = roTypes.Any2Integer(oRow("Id")),
                        .IdTask = roTypes.Any2Integer(oRow("IdTask")),
                        .SASLink = roTypes.Any2String(oRow("SASLink"))
                    }
                    End If
                Next

                If bAudit Then
                    ' Auditamos consulta masiva
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Me.State.Audit(Audit.Action.aConnect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetGeniusExecutionByIdWithSasKey(ByVal intExecution As Integer, Optional ByVal bAudit As Boolean = False) As roGeniusExecution

            Dim oRet As roGeniusExecution = Nothing

            Try

                Dim strAuditName As String = String.Empty
                Dim strSQL As String = "@SELECT# top 1 ge.ID,ge.IdGeniusView,ge.FileLink,ge.CubeLayout,ge.ExecutionDate,ge.IdTask,gv.Name, ge.SASLink, l.Culture FROM geniusExecutions ge inner join geniusViews gv on ge.IdGeniusView = gv.Id inner join sysroLanguages as l on l.LanguageKey = JSON_VALUE(gv.CustomFields, '$.LanguageKey')   where ge.Id=" & intExecution

                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oRow As DataRow In tb.Rows
                    strAuditName = oRow("Name")

                    Dim oGenius = Me.Load(roTypes.Any2Integer(oRow("IdGeniusView")))
                    If oGenius IsNot Nothing Then
                        oRet = New roGeniusExecution() With {
                        .ExecutionDate = roTypes.Any2DateTime(oRow("ExecutionDate")),
                        .IdGeniusView = roTypes.Any2Integer(oRow("IdGeniusView")),
                        .CubeLayout = roTypes.Any2String(oRow("CubeLayout")),
                        .FileLink = roTypes.Any2String(oRow("FileLink")),
                        .Id = roTypes.Any2Integer(oRow("Id")),
                        .IdTask = roTypes.Any2Integer(oRow("IdTask")),
                        .ExecutionLanguage = roTypes.Any2String(oRow("Culture")).Split("-")(0),
                        .AzureSaSKey = Azure.RoAzureSupport.GetFileSaSTokenWithURI(roTypes.Any2String(oRow("FileLink")), roLiveQueueTypes.analytics, False, Azure.RoAzureSupport.GetCompanyName()),
                        .FileSize = Azure.RoAzureSupport.GetFileSizeFromAzure(roTypes.Any2String(oRow("FileLink")), roLiveQueueTypes.analytics, False, Azure.RoAzureSupport.GetCompanyName()),
                        .SASLink = roTypes.Any2String(oRow("SASLink"))
                    } 'Revisar esto porqué se hizo, ya que vuelve a generar los links. Mirar si se puede elminar el AzureSaSKey al existir el SASKey
                    End If
                Next

                If bAudit Then
                    ' Auditamos consulta masiva
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Me.State.Audit(Audit.Action.aConnect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Finally

            End Try

            Return oRet

        End Function

        Public Function GetGeniusSharedPassports(ByVal intExecution As Integer, Optional ByVal bAudit As Boolean = False) As roGeniusExecution
            'TODO: RECUPERAR LISTADO DE PERSONAS CON LAS QUE HA COMPARTIDO EL REPORT GENIUS. AÑADIR ID REPORT AL PARAM
            Dim oRet As roGeniusExecution = Nothing

            Try

                Dim strAuditName As String = String.Empty
                Dim strSQL As String = "@SELECT# top 1 ge.ID,ge.IdGeniusView,ge.FileLink,ge.CubeLayout,ge.ExecutionDate,ge.IdTask,gv.Name, ge.SASLink FROM geniusExecutions ge inner join geniusViews gv on ge.IdGeniusView = gv.Id where ge.Id=" & intExecution

                Dim tb As DataTable = CreateDataTable(strSQL)

                For Each oRow As DataRow In tb.Rows
                    strAuditName = oRow("Name")

                    Dim oGenius = Me.Load(roTypes.Any2Integer(oRow("IdGeniusView")))
                    If oGenius IsNot Nothing Then
                        oRet = New roGeniusExecution() With {
                        .ExecutionDate = roTypes.Any2DateTime(oRow("ExecutionDate")),
                        .IdGeniusView = roTypes.Any2Integer(oRow("IdGeniusView")),
                        .CubeLayout = roTypes.Any2String(oRow("CubeLayout")),
                        .FileLink = roTypes.Any2String(oRow("FileLink")),
                        .Id = roTypes.Any2Integer(oRow("Id")),
                        .IdTask = roTypes.Any2Integer(oRow("IdTask")),
                        .SASLink = roTypes.Any2String(oRow("SASLink"))
                    }
                    End If
                Next

                If bAudit Then
                    ' Auditamos consulta masiva
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Me.State.Audit(Audit.Action.aConnect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Finally

            End Try

            Return oRet

        End Function

        Public Function UpdateGeniusViewLayout(ByVal oGeniusExecution As roGeniusExecution, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim oRet As Boolean = False

            Try

                Dim sqlCommand As String = "@UPDATE# geniusExecutions set CubeLayout=@cubeLayout where Id=" & oGeniusExecution.Id
                Dim sqlGeniusCommand As String = "@UPDATE# geniusViews set CubeLayout=@cubeLayout where Id=" & oGeniusExecution.IdGeniusView

                Dim oView As roGeniusView = Load(oGeniusExecution.IdGeniusView, False)

                If oView IsNot Nothing Then
                    Dim oParameters As New List(Of CommandParameter)
                    oParameters.Add(New CommandParameter("@CubeLayout", CommandParameter.ParameterType.tString, oGeniusExecution.CubeLayout))

                    oRet = DataLayer.AccessHelper.ExecuteSql(sqlCommand, oParameters)
                    oRet = oRet AndAlso DataLayer.AccessHelper.ExecuteSql(sqlGeniusCommand, oParameters)
                Else
                    oRet = False
                End If

                If bAudit Then
                    ' Auditamos consulta masiva
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Dim strAuditName As String = String.Empty
                    Extensions.roAudit.AddParameter(tbAuditParameters, "{GeniusViewNames}", strAuditName, String.Empty, 1)
                    Me.State.Audit(Audit.Action.aConnect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            End Try

            Return oRet

        End Function

        Public Function UpdateGeniusExecution(ByVal oGeniusExecution As roGeniusExecution, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim oRet As Boolean = False

            Try

                Dim sqlCommand As String = "@UPDATE# geniusExecutions set CubeLayout=@CubeLayout, FileLink=@fileLink, SASLink=@SASLink where Id=" & oGeniusExecution.Id
                Dim sqlGeniusCommand As String = "@UPDATE# geniusViews set CubeLayout=@CubeLayout where Id=" & oGeniusExecution.IdGeniusView

                Dim oView As roGeniusView = Load(oGeniusExecution.IdGeniusView, False)

                If oView IsNot Nothing Then
                    Dim oParameters As New List(Of CommandParameter)
                    oParameters.Add(New CommandParameter("@CubeLayout", CommandParameter.ParameterType.tString, oGeniusExecution.CubeLayout))
                    oParameters.Add(New CommandParameter("@fileLink", CommandParameter.ParameterType.tString, oGeniusExecution.FileLink))
                    oParameters.Add(New CommandParameter("@SASLink", CommandParameter.ParameterType.tString, oGeniusExecution.SASLink))

                    oRet = DataLayer.AccessHelper.ExecuteSql(sqlCommand, oParameters)
                    oRet = oRet AndAlso DataLayer.AccessHelper.ExecuteSql(sqlGeniusCommand, oParameters)

                    If bAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = String.Empty
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{GeniusViewNames}", strAuditName, String.Empty, 1)
                        Me.State.Audit(Audit.Action.aConnect, Audit.ObjectType.tGenius, strAuditName, tbAuditParameters, -1)
                    End If
                Else
                    oRet = False
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::GetExecutionInfo")
            End Try

            Return oRet

        End Function

        Public Function SendEmail(ByVal oGeniusExecution As roGeniusExecution, ByVal idTask As Integer) As Boolean

            Dim oRet As Boolean = False

            Try

                Dim oView As roGeniusView = Load(oGeniusExecution.IdGeniusView, False)
                If oView IsNot Nothing Then
                    'Comprovamos si existe la notificación activa de envío de emails por genius
                    Dim oSQLIDNotification As String = "@SELECT# ID from Notifications where Activated = 1 and IDType = 85"

                    Dim dtNotifications As DataTable = AccessHelper.CreateDataTable(oSQLIDNotification)

                    If dtNotifications IsNot Nothing AndAlso dtNotifications.Rows.Count > 0 Then
                        For Each oRow As DataRow In dtNotifications.Rows

                            Dim sSqlNotification As String = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, key3DateTime, Parameters ) VALUES " &
                                "(" & roTypes.Any2Integer(oRow("ID")) & ", " & oView.IdPassport & "," & oView.Id & "," & roTypes.Any2Time(Now).SQLDateTime & ",'" & idTask & "')"

                            oRet = ExecuteSql(sSqlNotification)

                        Next
                    End If
                Else
                    oRet = False
                End If
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::SendEmail")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roGeniusViewManager::SendEmail")
            Finally

            End Try

            Return oRet

        End Function

    End Class

End Namespace