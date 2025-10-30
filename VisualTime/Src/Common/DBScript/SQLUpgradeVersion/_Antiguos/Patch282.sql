-- Creamos vista que devuelve las reglas de acumulados en vigor y asignadas a algun empleado
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[sysrovwCurrentAppliedAccrualsRules]
AS
SELECT     IdAccrualsRule, Name, Description, Definition, Schedule, BeginDate, EndDate, Priority, ExecuteFromLastExecDay
FROM         dbo.AccrualsRules
WHERE     (GETDATE() BETWEEN BeginDate AND EndDate) and IdAccrualsRule in (select idAccrualsRules from EmployeeAccrualsRules)

GO

-- Creamos vista que devuelve los acumulados con valor inicial para este año
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[sysrovwCurrentStartupValues]
AS
SELECT idconcept FROM EmployeeConceptAnnualLimits WHERE IDYear=year(GETDATE())

GO
---- Generamos columnas para marcar los fichajes para la premigracion
alter table moves ADD checked_in bit not null default 0 
GO
alter table moves ADD checked_out bit not null default 0 
GO
alter table entries ADD checked bit not null default 0 
GO

---- Tabla para copiar los permisos por si hay que recuperarlos
CREATE TABLE [dbo].[TMPsysroGUI](
	[IDPath] [nvarchar](200) NOT NULL,
	[LanguageReference] [nvarchar](64) NULL,
	[URL] [nvarchar](200) NULL,
	[IconURL] [nvarchar](200) NULL,
	[Type] [nvarchar](64) NULL,
	[Parameters] [nvarchar](200) NULL,
	[RequiredFeatures] [nvarchar](200) NULL,
	[SecurityFlags] [nvarchar](99) NULL,
	[Priority] [int] NOT NULL,
	[AllowedSecurity] [nvarchar](25) NULL,
	[RequiredFunctionalities] [nvarchar](200) NULL,
 CONSTRAINT [PK_TMPsysroGUI] PRIMARY KEY NONCLUSTERED 
(
	[IDPath] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TMPsysroGUI] ADD  CONSTRAINT [DF_TMPsysroGUI_Priority]  DEFAULT (999999) FOR [Priority]
GO


UPDATE ImportGuides SET NAME = 'Carga de dotación teórica' WHERE ID=3
GO 

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='282' WHERE ID='DBVersion'
GO

