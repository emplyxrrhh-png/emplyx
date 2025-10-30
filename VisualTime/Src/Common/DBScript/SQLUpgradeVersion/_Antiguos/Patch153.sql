--
-- Actualiza Employees
--
ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Concepts_AllowQueriesOnTerminal] DEFAULT (1) FOR [AllowQueriesOnTerminal]
GO
ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Concepts_AllowQueryAccrualsOnTerminal] DEFAULT (1) FOR [AllowQueryAccrualsOnTerminal]
GO
ALTER TABLE [dbo].[Employees] ADD CONSTRAINT [DF_Concepts_AllowQueryShiftsOnTerminal] DEFAULT (1) FOR [AllowQueryShiftsOnTerminal]
GO
UPDATE [dbo].[Employees] SET [AllowQueriesOnTerminal]=1, 
	[AllowQueryAccrualsOnTerminal]=1,
	[AllowQueryShiftsOnTerminal]=1
GO
--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='153' WHERE ID='DBVersion'
GO

