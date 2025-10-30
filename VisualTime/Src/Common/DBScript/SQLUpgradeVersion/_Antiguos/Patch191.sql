-- Nuevo campo de idiomas a nivel de usuario
alter table sysrousers add language nvarchar(3)
GO

-- Nueva opción en el menú para modificar el idioma
insert into sysrogui (IDPath, LanguageReference, URL, IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity)
values('Menu\Tools\ChangeLanguage','ChangeLanguage','fn:\\ShowLanguage',NULL,NULL,NULL,NULL,'3111111111111111111111111111111111111111',586,'NR')
GO

-- Nuevos elementos para auditoría
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(34,'EmployeeUserFields'	,NULL)
GO
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(35,'EmployeeUserFieldsData'	,NULL)
GO

CREATE TABLE [sysroReportProfile] (
 [ID] [int] IDENTITY (1, 1) NOT NULL ,
 [Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
 [Definition] [nvarchar] (2096) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
 CONSTRAINT [PK_sysroReportProfile] PRIMARY KEY  CLUSTERED 
 (
  [ID]
 )  ON [PRIMARY] 
) ON [PRIMARY]
GO 

-- Nueva botón para informes de Crystal (y el resto)
insert into sysrogui (IDPath, LanguageReference, URL, IconURL,Type,Parameters,RequiredFeatures,SecurityFlags,Priority,AllowedSecurity)
values('NavBar\FirstTab\ReportsCR','ReportsCR','roFormReportsCR.vbd','reportsCR.ico',NULL,NULL,'Forms\Reports','3111111111111111111111111111111111111111',870,'NR')
GO

-- Creación de una vista para informes de los empleados futuros
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
 

CREATE   VIEW sysrovwFutureEmployeeGroups
AS
SELECT Groups.Name AS GroupName, Groups.Path, 
    Groups.SecurityFlags, EmployeeGroups.IDEmployee, 
    EmployeeGroups.IDGroup, 
    Employees.Name AS EmployeeName, 
    EmployeeGroups.BeginDate, EmployeeGroups.EndDate, 
    '0' 
    AS CurrentEmployee
FROM dbo.EmployeeGroups INNER JOIN dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID 
	INNER JOIN dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID 
	LEFT OUTER JOIN dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
WHERE (dbo.EmployeeGroups.BeginDate > CONVERT(smalldatetime, 
    CONVERT(nvarchar, GETDATE(), 102), 102))
	AND Employees.ID not in (Select IDEmployee from dbo.sysrovwCurrentEmployeeGroups)
 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- Creación de la vista para poder usar los filtros del VTSelector en los informes
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
 

CREATE   VIEW sysrovwAllEmployeeGroups
AS
SELECT     *
FROM         sysrovwCurrentEmployeeGroups
UNION
SELECT     *
FROM         sysrovwFutureEmployeeGroups
 

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- Añadimos campos a la tabla de Acumulados para indicar si el acumulado es de absentismo, y si es así si es remunerado
alter table Concepts
	add IsAbsentiism bit default(0),
	AbsentiismRewarded bit default(0)
GO

-- Añadimos la tabla para el informe MonthyEmployeeCalendar
CREATE TABLE [TMPMONTHLYEMPLOYEECALENDAR] (
 [Days] [smalldatetime])
GO 

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='191' WHERE ID='DBVersion'
GO

