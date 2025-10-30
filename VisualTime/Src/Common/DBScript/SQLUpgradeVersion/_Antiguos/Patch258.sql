--- Creamos tablas para los nuevos mx7+
IF OBJECT_ID('dbo.[TerminalsSyncTasks]','U') IS NOT NULL
DROP TABLE dbo.[TerminalsSyncTasks]
GO

CREATE TABLE [dbo].[TerminalsSyncTasks](
	[IDTerminal] [int] NOT NULL,
	[Task] [nvarchar](20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IDEmployee] [int] NULL,
	[IDFinger] [smallint] NULL,
	[DeleteOnConfirm] [bit] NOT NULL CONSTRAINT [DF_TerminalsSyncTasks_DeleteOnConfirm]  DEFAULT ((0)),
	[TaskDate] [datetime] NOT NULL CONSTRAINT [DF_TerminalsSyncTasks_TaskDate]  DEFAULT (getdate()),
	[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Assignments] ADD [CostField] nvarchar(100) NULL 
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='258' WHERE ID='DBVersion'
GO
