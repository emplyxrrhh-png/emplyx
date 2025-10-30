-- No borréis esta línea
CREATE TABLE [dbo].[TerminalsShellCommandsResult](
	[IDTerminal] [int] NOT NULL,
	[Task] [nvarchar](30) NOT NULL,
	[TaskData] [nvarchar](max) NULL,
	[TaskSent] [datetime] NULL,
	[ShellOut] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='806' WHERE ID='DBVersion'
GO
