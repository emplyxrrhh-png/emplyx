IF NOT EXISTS (SELECT 1 FROM sysroGUI_Actions WHERE IDPath = 'Filter' AND IDGUIPath='Portal\CostControl\BusinessCenters\Management')
  BEGIN
	INSERT INTO sysroGUI_Actions (IDPath,IDGUIPath,LanguageTag,RequieredFunctionalities,AfterFunction,CssClass,Section,ElementIndex) VALUES
	('Filter','Portal\CostControl\BusinessCenters\Management','tbAccFilter','U:BusinessCenters.Definition=Admin','frmFilterBusinessCenters_Show()','btnTbFilter2',0,1)
  END
GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Copy','Portal\Security\SecurityChart\SecurityFunctions','tbCopyFunction','Forms\Passports','U:Administration.Security=Write','copySecurityFunction()','btnTbCopySecurityFunction',0,4)
GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('Copy','Portal\Security\SecurityChart\Supervisors','tbCopySupervisor','Forms\Passports','U:Administration.Security=Write','copySupervisors()','btnTbCopySupervisor',0,4)
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TMPEmergencyBasic]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TMPEmergencyBasic](
	[IDEmployee] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[UserFieldValue] [nvarchar](100) NOT NULL,
	[TerminalDescription] [nvarchar](50) NULL,
	[DateTime] [datetime] NULL,
	[EmployeeVisited] [nvarchar](50) NULL,
	[EmpOrVisit] [nvarchar](10) NOT NULL,
	[IDReportTask] [numeric](16, 0) NOT NULL CONSTRAINT [DF_TMPEmergencyBasic_IDReportTask]  DEFAULT ((0)),
 CONSTRAINT [PK_TMPEmergencyBasic] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[UserFieldValue] ASC,
	[EmpOrVisit] ASC,
	[IDReportTask] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

 
UPDATE dbo.sysroParameters SET Data='389' WHERE ID='DBVersion'
GO
