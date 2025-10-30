INSERT INTO dbo.sysroNotificationTypes VALUES(29,'Task exceeding Started date',null, 120)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1500, 29, 'Tareas donde se ha sobrepasado la fecha de inicio prevista','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO

CREATE TABLE [dbo].[AlertsTask](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[IDTask] [int] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[DateTime] [datetime] NOT NULL,
	[Comment] [text] NULL,
	[IsReaded] [bit] NULL,
	[IDPassportReaded] [int] NULL,
 CONSTRAINT [PK_AlertsTask] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AlertsTask] ADD  CONSTRAINT [DF_AlertsTask_IsReaded]  DEFAULT ((0)) FOR [IsReaded]
GO

ALTER VIEW [dbo].[sysroCurrentTasksStatus]
AS
SELECT     dbo.Tasks.ID, dbo.BusinessCenters.Name AS Center, dbo.BusinessCenters.ID AS IDCenter, dbo.Tasks.Name, dbo.Tasks.Status, dbo.Tasks.Color, dbo.Tasks.Project, 
                      dbo.Tasks.Tag, dbo.Tasks.Priority, dbo.Tasks.ActivationTask, Tasks_1.Color AS ColorTaskPrev, CONVERT(numeric(25, 4), ISNULL(dbo.Tasks.InitialTime, 0) 
                      + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) + ISNULL(dbo.Tasks.OtherTime, 0)) AS Duration,
                          (SELECT     ISNULL(CONVERT(numeric(25, 4), SUM(Value)), 0) AS Expr1
                            FROM          dbo.DailyTaskAccruals
                            WHERE      (IDTask = dbo.Tasks.ID)) AS Worked, CASE WHEN CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 0 THEN CASE WHEN
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 100.00 THEN 100 ELSE
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      END ELSE 0 END AS Progress, CASE WHEN CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) + isnull(dbo.Tasks.TimeChangedRequirements, 0) 
                      + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) 
                      + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) > 0 THEN CASE WHEN
                          (SELECT     isnull(CONVERT(numeric(25, 4), SUM(Value)), 0)
                            FROM          DailyTaskAccruals
                            WHERE      DailyTaskAccruals.IDTask = dbo.Tasks.ID) * 100 / CONVERT(numeric(25, 4), isnull(dbo.Tasks.InitialTime, 0) 
                      + isnull(dbo.Tasks.TimeChangedRequirements, 0) + isnull(dbo.Tasks.ForecastErrorTime, 0) + isnull(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + isnull(dbo.Tasks.EmployeeTime, 0) + isnull(dbo.Tasks.TeamTime, 0) + isnull(dbo.Tasks.MaterialTime, 0) + isnull(dbo.Tasks.OtherTime, 0)) 
                      > 100.00 THEN 1 ELSE 0 END ELSE 0 END AS Exceeded, dbo.Tasks.IDPassport, dbo.Tasks.ExpectedStartDate, dbo.Tasks.ExpectedEndDate, 
                      Tasks_1.Name AS NameTaskPrev, dbo.Tasks.ShortName, CONVERT(nvarchar(200),
                          (SELECT     COUNT(DISTINCT LastPunch.IDEmployee) AS Expr1
                            FROM          (SELECT     ID AS IDEmployee,
                                                                               (SELECT     TOP (1) TypeData
                                                                                 FROM          dbo.Punches
                                                                                 WHERE      (ActualType = 4) AND (IDEmployee = dbo.Employees.ID)
                                                                                 ORDER BY DateTime DESC) AS IDTask
                                                    FROM          dbo.Employees) AS LastPunch INNER JOIN
                                                   dbo.EmployeeStatus ON LastPunch.IDEmployee = dbo.EmployeeStatus.IDEmployee
                            WHERE      (LastPunch.IDTask = dbo.Tasks.ID) AND (dbo.EmployeeStatus.LastPunch >= DATEADD(day, - 4, GETDATE())))) + '@' + CONVERT(nvarchar(200),
                          (SELECT     COUNT(DISTINCT LastPunch_2.IDEmployee) AS Expr1
                            FROM          (SELECT     ID AS IDEmployee,
                                                                               (SELECT     TOP (1) DateTime
                                                                                 FROM          dbo.Punches AS Punches_2
                                                                                 WHERE      (ActualType = 4) AND (IDEmployee = Employees_2.ID) AND (TypeData = dbo.Tasks.ID)
                                                                                 ORDER BY DateTime DESC) AS PunchDate
                                                    FROM          dbo.Employees AS Employees_2) AS LastPunch_2 INNER JOIN
                                                   dbo.EmployeeStatus AS EmployeeStatus_2 ON LastPunch_2.IDEmployee = EmployeeStatus_2.IDEmployee
                            WHERE      (LastPunch_2.PunchDate > DATEADD(day, - 7, GETDATE())) AND (LastPunch_2.PunchDate IS NOT NULL) AND (LastPunch_2.IDEmployee NOT IN
                                                       (SELECT DISTINCT LastPunch_1.IDEmployee
                                                         FROM          (SELECT     ID AS IDEmployee,
                                                                                                            (SELECT     TOP (1) TypeData
                                                                                                              FROM          dbo.Punches AS Punches_1
                                                                                                              WHERE      (ActualType = 4) AND (IDEmployee = Employees_1.ID)
                                                                                                              ORDER BY DateTime DESC) AS IDTask
                                                                                 FROM          dbo.Employees AS Employees_1) AS LastPunch_1 INNER JOIN
                                                                                dbo.EmployeeStatus AS EmployeeStatus_1 ON LastPunch_1.IDEmployee = EmployeeStatus_1.IDEmployee
                                                         WHERE      (LastPunch_1.IDTask = dbo.Tasks.ID) AND (EmployeeStatus_1.LastPunch >= DATEADD(day, - 4, GETDATE())))))) AS Employees,
						(select COUNT(*) FROM dbo.AlertsTask WHERE dbo.AlertsTask.IDTask = dbo.Tasks.ID AND IsReaded =0) as Alerts                                                          
FROM         dbo.Tasks LEFT OUTER JOIN
                      dbo.Tasks AS Tasks_1 ON dbo.Tasks.ActivationTask = Tasks_1.ID INNER JOIN
                      dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID

GO

INSERT INTO dbo.sysroNotificationTypes VALUES(30,'Tasks with alerts',null, 5)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1550, 30, 'Tareas con alertas','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',1)
GO


--- Añadimos importación de tareas
INSERT INTO [dbo].[ImportGuides]
       ([ID],[Name],[Template],[Mode],[Type],[FormatFilePath],[SourceFilePath],[Separator],[CopySource],[LastLog])
 VALUES
       (5,'Carga de tareas',0,1,1,'','','',0,NULL)
GO

ALTER TABLE [dbo].[Tasks] ADD BarCode nvarchar(50) NULL
GO

ALTER VIEW [dbo].[sysroTasksCube]
AS
SELECT     dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee, 
                      dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) 
                      AS Date_Month, DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.BusinessCenters.ID AS IDCenter, 
                      dbo.BusinessCenters.Name AS CenterName, dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '') 
                      AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0) 
                      AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task, 
                      ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '') 
                      AS Field3_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5), 
                      0) AS Field5_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6), 
                      dbo.DailyTaskAccruals.Value), 0) AS Value, ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport,
                      ISNULL(dbo.Tasks.TimeChangedRequirements,0)AS TimeChangedRequirements,ISNULL(dbo.Tasks.ForecastErrorTime,0)AS ForecastErrorTime,ISNULL(dbo.Tasks.NonProductiveTimeIncidence,0)AS NonProductiveTimeIncidence,
                      ISNULL(dbo.Tasks.EmployeeTime,0) AS EmployeeTime,ISNULL(dbo.Tasks.TeamTime,0)AS TeamTime,ISNULL(dbo.Tasks.MaterialTime,0)AS MaterialTime,ISNULL(dbo.Tasks.OtherTime,0)AS OtherTime,
                      (ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) 
                      + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) + ISNULL(dbo.Tasks.OtherTime, 0)) AS Duration, 
                      CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente'
                       END END END AS Estado
FROM         dbo.Employees INNER JOIN
                      dbo.DailyTaskAccruals ON dbo.DailyTaskAccruals.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Tasks ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyTaskAccruals.IDEmployee INNER JOIN
                      dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID
WHERE     (dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate)

GO


INSERT INTO [dbo].[sysroSchedulerViews]
           ([ID]
           ,[IdView]
           ,[IdPassport]
           ,[NameView]
           ,[Description]
           ,[DateView]
           ,[Employees]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[TypeView]
           ,[FilterData]
           ,[Concepts])
     VALUES 
  			( ISNULL((select MAX(ID) + 1 from dbo.sysroSchedulerViews),1),
			1,
			-1,
			'Diario',
			'',
			convert(smalldatetime,'2013-05-27',120),
			'',
			convert(smalldatetime,'2013-05-01',120),
			convert(smalldatetime,'2013-06-24',120),
			'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8gGgd/IdAKF38iEA8gGgd/IdAKF38iIA8gKgd/IdAKB38h6goHfyI/ICoHfyJHfyJaB38iZ38iehd/J78gDyAPJ8DiNMYXlvdXRWZXJzaW9uABNPcHRpb25zTG9hZGluZ1BhbmVsBFRleHQQQ2FyZ2FuZG8maGVsbGlwOwxPcHRpb25zUGFnZXILUm93c1BlclBhZ2ULT3B0aW9uc1ZpZXcXU2hvd0hvcml6b250YWxTY3JvbGxCYXIGRmllbGRzBUl0ZW0xBUluZGV4EVNvcnRCeVN1bW1hcnlJbmZvEkZpZWxkQ29tcG9uZW50TmFtZQpDb25kaXRpb25zDEZpbHRlclZhbHVlcwZWYWx1ZXMOflh0cmEjQXJyYXkwLCAKU2hvd0JsYW5rcwpGaWx0ZXJUeXBlCEV4Y2x1ZGVkDEN1c3RvbVRvdGFscwlGaWVsZE5hbWUJR3JvdXBOYW1lBE5hbWUPZmllbGRHcm91cE5hbWUxBEFyZWEHUm93QXJlYQ9WYWx1ZVRvdGFsU3R5bGUJVmlld1N0YXRlE1RvcEFsaWduZWRSb3dWYWx1ZXMCSUQLSGVhZGVyU3R5bGUJQ2VsbFN0eWxlClZhbHVlU3R5bGUJQXJlYUluZGV4DkFjdHVhbFNvcnRNb2RlB0RlZmF1bHQQU3VtbWFyeVZhcmlhdGlvbgROb25lBUl0ZW0yDEVtcGxveWVlTmFtZRJmaWVsZEVtcGxveWVlTmFtZTEFSXRlbTMJRGF0ZV9ZZWFyDmZpZWxkRGF0ZVllYXIxCkNvbHVtbkFyZWEFSXRlbTQKRGF0ZV9Nb250aA9maWVsZERhdGVNb250aDEFSXRlbTUIRGF0ZV9EYXkNZmllbGREYXRlRGF5MQVJdGVtNgREYXRlCmZpZWxkRGF0ZTEFSXRlbTcKQ2VudGVyTmFtZRBmaWVsZENlbnRlck5hbWUxBUl0ZW04CFRhc2tOYW1lDmZpZWxkVGFza05hbWUxBUl0ZW05C0luaXRpYWxUaW1lEWZpZWxkSW5pdGlhbFRpbWUxCERhdGFBcmVhB1Zpc2libGUGSXRlbTEwC0ZpZWxkMV9UYXNrEGZpZWxkRmllbGQxVGFzazEGSXRlbTExC0ZpZWxkMl9UYXNrEGZpZWxkRmllbGQyVGFzazEGSXRlbTEyC0ZpZWxkM19UYXNrEGZpZWxkRmllbGQzVGFzazEGSXRlbTEzCkNlbGxGb3JtYXQMRm9ybWF0U3RyaW5nCCMsIyMwLjAwCkZvcm1hdFR5cGUHTnVtZXJpYwtGaWVsZDRfVGFzaxBmaWVsZEZpZWxkNFRhc2sxBkl0ZW0xNAtGaWVsZDVfVGFzaxBmaWVsZEZpZWxkNVRhc2sxBkl0ZW0xNQtGaWVsZDZfVGFzaxBmaWVsZEZpZWxkNlRhc2sxBkl0ZW0xNgxGaWVsZDFfVG90YWwRZmllbGRGaWVsZDFUb3RhbDEGSXRlbTE3DEZpZWxkMl9Ub3RhbBFmaWVsZEZpZWxkMlRvdGFsMQZJdGVtMTgMRmllbGQzX1RvdGFsEWZpZWxkRmllbGQzVG90YWwxBkl0ZW0xOQxGaWVsZDRfVG90YWwRZmllbGRGaWVsZDRUb3RhbDEGSXRlbTIwDEZpZWxkNV9Ub3RhbBFmaWVsZEZpZWxkNVRvdGFsMQZJdGVtMjEMRmllbGQ2X1RvdGFsEWZpZWxkRmllbGQ2VG90YWwxBkl0ZW0yMgVWYWx1ZQtmaWVsZFZhbHVlMQdPcHRpb25zCUFsbG93RHJhZwVGYWxzZQZJdGVtMjMHUHJvamVjdA1maWVsZFByb2plY3QxBkl0ZW0yNANUYWcJZmllbGRUYWcxBkl0ZW0yNQZFc3RhZG8MZmllbGRFc3RhZG8xBkdyb3Vwcw==',
			'T',
			NULL,
			NULL)
GO

INSERT INTO [dbo].[sysroSchedulerViews]
           ([ID]
           ,[IdView]
           ,[IdPassport]
           ,[NameView]
           ,[Description]
           ,[DateView]
           ,[Employees]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[TypeView]
           ,[FilterData]
           ,[Concepts])
     VALUES 
			( ISNULL((select MAX(ID) + 1 from dbo.sysroSchedulerViews),1),
			1,
			-1,
			'Totales - desvios',
			'',
			convert(smalldatetime,'2013-05-27',120),
			'',
			convert(smalldatetime,'2013-05-01',120),
			convert(smalldatetime,'2013-06-24',120),
			'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eWxlClZhbHVlU3R5bGUJQXJlYUluZGV4DkFjdHVhbFNvcnRNb2RlB0RlZmF1bHQQU3VtbWFyeVZhcmlhdGlvbgROb25lBUl0ZW0yDEVtcGxveWVlTmFtZRJmaWVsZEVtcGxveWVlTmFtZTEFSXRlbTMJRGF0ZV9ZZWFyDmZpZWxkRGF0ZVllYXIxBUl0ZW00CkRhdGVfTW9udGgPZmllbGREYXRlTW9udGgxBUl0ZW01CERhdGVfRGF5DWZpZWxkRGF0ZURheTEFSXRlbTYERGF0ZQpmaWVsZERhdGUxBUl0ZW03CkNlbnRlck5hbWUQZmllbGRDZW50ZXJOYW1lMQRBcmVhB1Jvd0FyZWEFSXRlbTgIVGFza05hbWUOZmllbGRUYXNrTmFtZTEFSXRlbTkLSW5pdGlhbFRpbWURZmllbGRJbml0aWFsVGltZTEGSXRlbTEwC0ZpZWxkMV9UYXNrEGZpZWxkRmllbGQxVGFzazEHVmlzaWJsZQZJdGVtMTELRmllbGQyX1Rhc2sQZmllbGRGaWVsZDJUYXNrMQZJdGVtMTILRmllbGQzX1Rhc2sQZmllbGRGaWVsZDNUYXNrMQZJdGVtMTMKQ2VsbEZvcm1hdAxGb3JtYXRTdHJpbmcIIywjIzAuMDAKRm9ybWF0VHlwZQdOdW1lcmljC0ZpZWxkNF9UYXNrEGZpZWxkRmllbGQ0VGFzazEGSXRlbTE0C0ZpZWxkNV9UYXNrEGZpZWxkRmllbGQ1VGFzazEGSXRlbTE1C0ZpZWxkNl9UYXNrEGZpZWxkRmllbGQ2VGFzazEGSXRlbTE2DEZpZWxkMV9Ub3RhbBFmaWVsZEZpZWxkMVRvdGFsMQZJdGVtMTcMRmllbGQyX1RvdGFsEWZpZWxkRmllbGQyVG90YWwxBkl0ZW0xOAxGaWVsZDNfVG90YWwRZmllbGRGaWVsZDNUb3RhbDEGSXRlbTE5DEZpZWxkNF9Ub3RhbBFmaWVsZEZpZWxkNFRvdGFsMQZJdGVtMjAMRmllbGQ1X1RvdGFsEWZpZWxkRmllbGQ1VG90YWwxBkl0ZW0yMQxGaWVsZDZfVG90YWwRZmllbGRGaWVsZDZUb3RhbDEGSXRlbTIyBVZhbHVlC2ZpZWxkVmFsdWUxCERhdGFBcmVhB09wdGlvbnMJQWxsb3dEcmFnBUZhbHNlBkl0ZW0yMwdQcm9qZWN0DWZpZWxkUHJvamVjdDEGSXRlbTI0A1RhZwlmaWVsZFRhZzEGSXRlbTI1F1RpbWVDaGFuZ2VkUmVxdWlyZW1lbnRzHWZpZWxkVGltZUNoYW5nZWRSZXF1aXJlbWVudHMxBkl0ZW0yNhFGb3JlY2FzdEVycm9yVGltZRdmaWVsZEZvcmVjYXN0RXJyb3JUaW1lMQZJdGVtMjcaTm9uUHJvZHVjdGl2ZVRpbWVJbmNpZGVuY2UgZmllbGROb25Qcm9kdWN0aXZlVGltZUluY2lkZW5jZTEGSXRlbTI4DEVtcGxveWVlVGltZRJmaWVsZEVtcGxveWVlVGltZTEGSXRlbTI5CFRlYW1UaW1lDmZpZWxkVGVhbVRpbWUxBkl0ZW0zMAxNYXRlcmlhbFRpbWUSZmllbGRNYXRlcmlhbFRpbWUxBkl0ZW0zMQlPdGhlclRpbWUPZmllbGRPdGhlclRpbWUxBkl0ZW0zMghEdXJhdGlvbg5maWVsZER1cmF0aW9uMQZJdGVtMzMGRXN0YWRvDGZpZWxkRXN0YWRvMQZHcm91cHM=',
			'T',
			NULL,
			NULL)
GO
INSERT INTO [dbo].[sysroSchedulerViews]
           ([ID]
           ,[IdView]
           ,[IdPassport]
           ,[NameView]
           ,[Description]
           ,[DateView]
           ,[Employees]
           ,[DateInf]
           ,[DateSup]
           ,[CubeLayout]
           ,[TypeView]
           ,[FilterData]
           ,[Concepts])
     VALUES 
  			( ISNULL((select MAX(ID) + 1 from dbo.sysroSchedulerViews),1),
			1,
			-1,
			'Totales - teórico',
			'',
			convert(smalldatetime,'2013-05-27',120),
			'',
			convert(smalldatetime,'2013-05-01',120),
			convert(smalldatetime,'2013-06-24',120),
			'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cmVhSW5kZXgOQWN0dWFsU29ydE1vZGUHRGVmYXVsdBBTdW1tYXJ5VmFyaWF0aW9uBE5vbmUFSXRlbTIMRW1wbG95ZWVOYW1lEmZpZWxkRW1wbG95ZWVOYW1lMQVJdGVtMwlEYXRlX1llYXIOZmllbGREYXRlWWVhcjEFSXRlbTQKRGF0ZV9Nb250aA9maWVsZERhdGVNb250aDEFSXRlbTUIRGF0ZV9EYXkNZmllbGREYXRlRGF5MQVJdGVtNgREYXRlCmZpZWxkRGF0ZTEFSXRlbTcKQ2VudGVyTmFtZRBmaWVsZENlbnRlck5hbWUxBEFyZWEHUm93QXJlYQVJdGVtOAhUYXNrTmFtZQ5maWVsZFRhc2tOYW1lMQVJdGVtOQtJbml0aWFsVGltZRFmaWVsZEluaXRpYWxUaW1lMQZJdGVtMTALRmllbGQxX1Rhc2sQZmllbGRGaWVsZDFUYXNrMQdWaXNpYmxlBkl0ZW0xMQtGaWVsZDJfVGFzaxBmaWVsZEZpZWxkMlRhc2sxBkl0ZW0xMgtGaWVsZDNfVGFzaxBmaWVsZEZpZWxkM1Rhc2sxBkl0ZW0xMwpDZWxsRm9ybWF0DEZvcm1hdFN0cmluZwgjLCMjMC4wMApGb3JtYXRUeXBlB051bWVyaWMLRmllbGQ0X1Rhc2sQZmllbGRGaWVsZDRUYXNrMQZJdGVtMTQLRmllbGQ1X1Rhc2sQZmllbGRGaWVsZDVUYXNrMQZJdGVtMTULRmllbGQ2X1Rhc2sQZmllbGRGaWVsZDZUYXNrMQZJdGVtMTYMRmllbGQxX1RvdGFsEWZpZWxkRmllbGQxVG90YWwxBkl0ZW0xNwxGaWVsZDJfVG90YWwRZmllbGRGaWVsZDJUb3RhbDEGSXRlbTE4DEZpZWxkM19Ub3RhbBFmaWVsZEZpZWxkM1RvdGFsMQZJdGVtMTkMRmllbGQ0X1RvdGFsEWZpZWxkRmllbGQ0VG90YWwxBkl0ZW0yMAxGaWVsZDVfVG90YWwRZmllbGRGaWVsZDVUb3RhbDEGSXRlbTIxDEZpZWxkNl9Ub3RhbBFmaWVsZEZpZWxkNlRvdGFsMQZJdGVtMjIFVmFsdWULZmllbGRWYWx1ZTEIRGF0YUFyZWEHT3B0aW9ucwlBbGxvd0RyYWcFRmFsc2UGSXRlbTIzB1Byb2plY3QNZmllbGRQcm9qZWN0MQZJdGVtMjQDVGFnCWZpZWxkVGFnMQZJdGVtMjUXVGltZUNoYW5nZWRSZXF1aXJlbWVudHMdZmllbGRUaW1lQ2hhbmdlZFJlcXVpcmVtZW50czEGSXRlbTI2EUZvcmVjYXN0RXJyb3JUaW1lF2ZpZWxkRm9yZWNhc3RFcnJvclRpbWUxBkl0ZW0yNxpOb25Qcm9kdWN0aXZlVGltZUluY2lkZW5jZSBmaWVsZE5vblByb2R1Y3RpdmVUaW1lSW5jaWRlbmNlMQZJdGVtMjgMRW1wbG95ZWVUaW1lEmZpZWxkRW1wbG95ZWVUaW1lMQZJdGVtMjkIVGVhbVRpbWUOZmllbGRUZWFtVGltZTEGSXRlbTMwDE1hdGVyaWFsVGltZRJmaWVsZE1hdGVyaWFsVGltZTEGSXRlbTMxCU90aGVyVGltZQ9maWVsZE90aGVyVGltZTEGSXRlbTMyCER1cmF0aW9uDmZpZWxkRHVyYXRpb24xBkl0ZW0zMwZFc3RhZG8MZmllbGRFc3RhZG8xBkdyb3Vwcw==',
			'T',
			NULL,
			NULL)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='305' WHERE ID='DBVersion'
GO
