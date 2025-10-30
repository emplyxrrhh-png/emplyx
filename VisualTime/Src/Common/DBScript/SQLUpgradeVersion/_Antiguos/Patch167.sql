--
-- Actualizamos tablas para el control de centros de coste
--

CREATE TABLE [dbo].[CostCenters] (
	[ID] [int] NOT NULL ,
	[Name] [nvarchar] (50) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeCostCenters] (
	[IDEmployee] [int] NOT NULL ,
	[IDRelatedCost] [numeric](18, 0) NOT NULL ,
	[BeginDate] [smalldatetime] NOT NULL ,
	[EndDate] [smalldatetime] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[EmployeeRelatedCostCenters] (
	[IDRelatedEmployeeCost] [numeric](18, 0) NOT NULL ,
	[IDCost] [int] NOT NULL ,
	[Percentage] [numeric](18, 2) NOT NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CostCenters] WITH NOCHECK ADD 
	CONSTRAINT [PK_CostCenters] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[EmployeeCostCenters] WITH NOCHECK ADD 
	CONSTRAINT [PK_EmployeeCosts] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[IDRelatedCost]
	)  ON [PRIMARY] 
GO

ALTER TABLE [dbo].[EmployeeRelatedCostCenters] WITH NOCHECK ADD 
	CONSTRAINT [PK_RelatedCosts] PRIMARY KEY  NONCLUSTERED 
	(
		[IDRelatedEmployeeCost],
		[IDCost]
	)  ON [PRIMARY] 
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='167' WHERE ID='DBVersion'
GO

