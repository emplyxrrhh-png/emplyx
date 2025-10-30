--Vista para el informe de emergencia
create view sysrovwCurrentEmployeesPresenceStatus
as
SELECT DISTINCT IDEmployee, EmployeeName,
 (SELECT TOP 1 DateTime FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS DateTime,
 (SELECT TOP 1 IDReader FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS IDReader,
 (SELECT TOP 1 Status FROM sysrovwEmployeesInOutMoves si WHERE si.IDEmployee = sc.IDEmployee ORDER BY DateTime DESC) AS Status
FROM dbo.sysrovwCurrentEmployeeGroups sc
GO
--Añadimos tabla temporal que usará el informe de Los que más
CREATE TABLE [dbo].[TMPTOP](
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](50) NOT NULL,
	[IDConcept] [smallint] NOT NULL,
	[ConceptName] [nvarchar](50) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Value] [numeric](19, 6) NULL,
 CONSTRAINT [PK_TMPTOP] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDConcept] ASC,
	[Date] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='216' WHERE ID='DBVersion'
GO
