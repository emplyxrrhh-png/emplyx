-- Crea tablas nuevas
CREATE TABLE [dbo].[EmployeeJobs] (
	[IDJob] [int] NOT NULL ,
	[IDEmployee] [int] NOT NULL ,
	[Cost] [numeric](18, 3) NULL
) ON [PRIMARY] 
GO

-- Crea constraints nuevos
ALTER TABLE [dbo].[EmployeeJobs] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeJobs] PRIMARY KEY  NONCLUSTERED 
	(	[IDJob],
		[IDEmployee]
	)  ON [PRIMARY] 

GO

ALTER TABLE [dbo].[EmployeeJobs] WITH NOCHECK ADD 
	CONSTRAINT [DF_EmployeeJobs_Cost] DEFAULT (0) FOR [Cost]
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='159' WHERE ID='DBVersion'
GO

