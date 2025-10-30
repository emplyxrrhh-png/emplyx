-- Asignacion de puestos a tareas
CREATE TABLE [dbo].[TaskAssignments](
	[IDTask] [int] NOT NULL,
	[IDAssignment] [smallint] NOT NULL,
 CONSTRAINT [PK_TaskAssignments] PRIMARY KEY CLUSTERED 
(
	[IDTask] ASC,
	[IDAssignment] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


-- Actualización de la vista sysroEmployeeGroups para los perfiles de los informes
DROP VIEW [dbo].[sysroEmployeeGroups]
GO

CREATE VIEW [dbo].[sysroEmployeeGroups]
AS
 SELECT dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
    dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName,
	(SELECT CASE LEFT(Path,CHARINDEX('\',Path)) WHEN '' THEN Path ELSE CONVERT(INT,LEFT(Path,CHARINDEX('\',Path)-1)) END FROM Groups WHERE ID = (dbo.Groups.ID)) AS IDCompany, CurrentEmployee
 FROM dbo.Groups 
	INNER JOIN dbo.EmployeeGroups ON dbo.Groups.ID = dbo.EmployeeGroups.IDGroup
	INNER JOIN dbo.sysrovwCurrentEmployeeGroups ON dbo.EmployeeGroups.IDEmployee = dbo.sysrovwCurrentEmployeeGroups.IDEmployee
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='316' WHERE ID='DBVersion'
GO

