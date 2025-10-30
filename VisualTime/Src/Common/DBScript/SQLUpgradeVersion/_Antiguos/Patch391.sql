INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Home','Home','Main.aspx','Robotics.png',NULL,NULL,NULL,NULL,1001,NULL,NULL)
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
     VALUES ('CalendarMode','1')
GO

UPDATE [dbo].[sysroGUI] SET [Parameters] = 'CalendarV1' WHERE IDPath ='Portal\ShiftControl\Scheduler'
GO

INSERT INTO [dbo].[sysroGUI]
           ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\ShiftControl\Calendar','GUI.Scheduler','Scheduler/Calendar.aspx','Calendar.png',NULL,'CalendarV2','Forms\Calendar',NULL,101,'NWR','U:Calendar=Read')
GO

delete from sysroGUI_Actions where IDGUIPath = 'Portal\ShiftControl\Calendar\Review'
GO

delete from sysroGUI_Actions where IDGUIPath = 'Portal\ShiftControl\Calendar\Planification'
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters] ([ParameterName],[Value]) VALUES ('ConceptHolidaysCalendar','')
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters] ([ParameterName],[Value]) VALUES ('ConceptCalendar','')
GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Incomplete','Portal\ShiftControl\Calendar\Review','IncompletedDays','Forms\Calendar',NULL,'ShowIncompletedDays()','btnTbDispID2',0,1)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Justified','Portal\ShiftControl\Calendar\Review','NotJustifiedDays','Forms\Calendar',NULL,'ShowNotJustifiedDays()','btnTbDispNJ2',0,2)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Reliabled','Portal\ShiftControl\Calendar\Review','NotReliabledDays','Forms\Calendar',NULL,'ShowNotReliabledDays()','btnTbDispNR2',0,3)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Reports','Portal\ShiftControl\Calendar\Review','ShowReports','Forms\Calendar',NULL,'ShowReports','btnTbPrint2',0,4)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('AssignCause','Portal\ShiftControl\Calendar\Review','AssignCauses','Forms\Calendar',NULL,'ShowAssignCausesWizard()','btnTbMassCause',1,1)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('JustifiedIncicences','Portal\ShiftControl\Calendar\Review','JustifiedIncicences','Forms\Calendar',NULL,'ShowIncidencesWizard()','btnTbMassIncidence',1,2)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('CostCenters','Portal\ShiftControl\Calendar\Review','AssignCenters','Feature\CostControl','U:BusinessCenters.Punches=Write','ShowAssignCentersWizard()','btnTbMassAssignCenters',1,3)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('MassPunch','Portal\ShiftControl\Calendar\Review','InsertMassPunch','Forms\Calendar',NULL,'ShowInsertMassPunchWizard()','btnTbMassPunch',1,4)

GO


INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('CopyEmp','Portal\ShiftControl\Calendar\Planification','CopyPlanBtwEmployees','Forms\Calendar','U:Calendar.Scheduler=Write','ShowCopySchedulerWizard()','btnTbCopyP2',0,1)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('ExportExcel','Portal\ShiftControl\Calendar\Planification','ExportPlanToExcel','Forms\Calendar','U:Calendar.Scheduler=Read','ExportPlanToExcel()','btnTbExcelExport',0,2)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('ImportExcel','Portal\ShiftControl\Calendar\Planification','ImportPlanFromExcel','Forms\Calendar','U:Calendar.Scheduler=Write','ImportPlanFromExcel()','btnTbExcelImport',0,3)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Reports','Portal\ShiftControl\Calendar\Planification','ShowReports','Forms\Calendar',NULL,'ShowReports','btnTbPrint2',0,4)

GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('WeekPlan','Portal\ShiftControl\Calendar\Planification','WeekPlan','Forms\Calendar','U:Calendar.Scheduler=Write','ShowWeekScheduleWizard()','btnTbPlanS2',1,1)

GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters] ([ParameterName],[Value]) VALUES ('ConceptNormalWorkCalendar','')
GO
INSERT INTO [dbo].[sysroLiveAdvancedParameters] ([ParameterName],[Value]) VALUES ('ConceptAbsenceCalendar','')
GO
INSERT INTO [dbo].[sysroLiveAdvancedParameters] ([ParameterName],[Value]) VALUES ('ConceptOverWorkingCalendar','')
GO

ALTER TABLE [dbo].[Shifts] ADD AllowComplementary BIT NULL DEFAULT 0
GO

UPDATE [dbo].[Shifts] SET AllowComplementary = 0 WHERE AllowComplementary IS NULL
GO

ALTER TABLE [dbo].[DailySchedule] ADD LayersDefinition [nvarchar](MAX) NULL
GO

ALTER TABLE dbo.DailySchedule ADD [ExpectedWorkingHours] [numeric](9, 6) NULL	
GO

ALTER TABLE dbo.Shifts ADD [BreakHours] [numeric](9, 6) NULL	
GO

ALTER TABLE dbo.Shifts ADD [AllowFloatingData] [bit]  NULL	 DEFAULT(0)
GO

UPDATE [dbo].[Shifts] SET AllowFloatingData = 0 WHERE AllowFloatingData IS NULL
GO


INSERT INTO dbo.sysroDailyIncidencesTypes (ID,Module,NamedID,Description,Stored,TreatAs,WorkingTime) VALUES (2000,'PRES','COM','Complementary',1,NULL,1)
GO

INSERT INTO dbo.sysroDailyIncidencesDescription (IDIncidence,Description) VALUES (2000,'Horas complementarias')
GO


UPDATE dbo.sysroParameters SET Data='391' WHERE ID='DBVersion'
GO
