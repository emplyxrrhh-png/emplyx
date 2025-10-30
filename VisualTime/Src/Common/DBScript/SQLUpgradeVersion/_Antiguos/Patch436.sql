IF NOT EXISTS (SELECT 1 FROM [dbo].sysroGUI_Actions WHERE IDPath = 'Assign' AND IDGUIPath = 'Portal\AISchedule\Budget\Schedule')
	INSERT INTO [dbo].sysroGUI_Actions ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
		VALUES ('Assign', 'Portal\AISchedule\Budget\Schedule','tbAssignAISchedule','Feature\HRScheduling',NULL,'RunAIPlanner(true)','btnTbRunAIPlanner2',0,1)
GO

UPDATE [dbo].[sysroGUI_Actions] SET ElementIndex = 3 WHERE [IDGUIPath] = 'Portal\AISchedule\Budget\Schedule' AND [IDPath] = 'Reports'
GO

DELETE  FROM [dbo].sysroGUI_Actions WHERE IDPath = 'Remove' AND IDGUIPath = 'Portal\AISchedule\Budget\Schedule'
GO

CREATE TABLE [dbo].[Visit_Types](
	[IDType] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_Visit_Types] PRIMARY KEY CLUSTERED 
(
	[IDType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 
GO

alter table [dbo].[Visit] add VisitType [smallint] 
go

alter table [dbo].[Visit_Fields] add VisitType [smallint]
go

UPDATE dbo.sysroParameters SET Data='436' WHERE ID='DBVersion'
GO
