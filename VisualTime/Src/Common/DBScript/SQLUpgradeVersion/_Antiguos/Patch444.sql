CREATE NONCLUSTERED INDEX [IDX_TerminalsSyncTasks] ON [dbo].[TerminalsSyncTasks]
(
	[IDTerminal] ASC,
	[Task] ASC,
	[IDEmployee] ASC,
	[IDFinger] ASC,
	[Parameter1] ASC,
	[Parameter2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

UPDATE dbo.sysroParameters SET Data='444' WHERE ID='DBVersion'
GO