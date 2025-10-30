-- creamos nuevas tablas para los terminales SX
--

CREATE TABLE [EmployeeBiometricDataSX](
 [IDEmployee] [int] NOT NULL,
 [IDFinger] [smallint] NOT NULL,
 [Data] [nvarchar](800) NOT NULL,
 [TimeStamp] [smalldatetime],
 [IDTerminal] [int] NULL,
 CONSTRAINT [PK_EmployeeBiometricDataSX] PRIMARY KEY CLUSTERED 
(
 [IDEmployee] ASC,
 [IDFinger] ASC
))
GO
 
CREATE TABLE [TerminalsTasksSX](
 [IDTerminal] [int] NOT NULL,
 [Task] [nvarchar](20) NOT NULL,
 [IDEmployee] [int],
 [Finger] [smallint],
 [DeleteConfirm] [bit],
 [TaskDate] [smalldatetime] NOT NULL CONSTRAINT [DF_TerminalsTasksSX_TaskDate]  DEFAULT (getdate())
)
GO


-- Funcion que devuelve el nombre completo con toda la ruta de un grupo dado su ID

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetFullGroupPathName] (@GroupId int)
	RETURNS varchar(300) 
AS
BEGIN
	declare @path varchar(200);
	declare @index int;
	declare @delimiter char(1);
	declare @i int;
	declare @resultado varchar(300);
	declare @id varchar(100);
	declare @nombre varchar(200);
	declare @longitud varchar(100);
	declare @contador int;
	declare @anterior int;
	declare @siguiente int;
	declare @digitos int;

	set @contador=0
	set @resultado=''
	set @index=1
	set @id=''
	set @i=1
	set @delimiter='\'
	-- Seleccionamos el Path del grupo que nos manda el usuario
	select @path=Path from Groups where ID=str(@GroupId)
	-- print 'El path es: '+@path
	-- Miramos la longitud total del Path
	select @longitud = len(@path)
	--print 'La longitud es: '+str(@longitud)
	-- Controlamos el primer valor del grupo
	while (@contador=0)
	begin
		select @id=left(@path,1)
		select @contador=@contador+1
		select @nombre=Name from Groups where ID=@id
		select @resultado=@nombre
	end
	-- Controlamos que hay alguna \ en el path
	while (@index!=0)
	begin
		-- Buscamos la posicion de la \
		select @index = CHARINDEX(@delimiter,@path,@i)
		-- Guardamos la localizacion de la primera \
		select @anterior = @index
		-- Si encuentra la \
		if (@index!=0) 
		begin
			-- Colocamos el cursor en la siguiente posicion		
			select @i=@index+1
			-- Volvemos a buscar \ por si es la ultima
			select @index = CHARINDEX(@delimiter,@path,@i)
			--print 'Posicion de la \: '+str(@index)
			-- Si existe otra barra por delante
			if (@index!=0)
			begin
				-- Restamos 1 porque entre las \ albergan minimo 2 espacios
				select @digitos=@index-@anterior-1
				--Comprobamos si el grupo tiene 1 digito
				if (@digitos=1)
				begin
					select @id=right(left(@path,@i),1)
					-- Buscamos el id en la base de datos
					select @nombre=Name from Groups where ID=@id
					--print 'Grupo: '+@nombre
					-- Concatenamos el valor al resultado final
					select @resultado=@resultado+' \ '+@nombre
				end
				-- Si el id del grupo esta formado por dos digitos
				if (@digitos=2)
				begin
					-- Movemos el puntero para poder recortar el id del grupo
					select @i=@i+1
					select @id=right(left(@path,@i),2)
					select @nombre=Name from Groups where ID=@id
					-- Concatenamos el valor al resultado final
					select @resultado=@resultado+' \ '+@nombre
				end
				-- Si el id del grupo esta formado por tres digitos
				if (@digitos=3)
				begin
					select @i=@i+2
					select @id=right(left(@path,@i),3)
					select @nombre=Name from Groups where ID=@id
					-- Concatenamos el valor al resultado final
					select @resultado=@resultado+' \ '+@nombre
				end
			end
			-- Sino no encuentra \
			else
			begin
				select @digitos=@longitud-@anterior
				-- Si el id del grupo esta formado por un digito
				if (@digitos=1)
				begin
				-- Movemos el puntero para poder recortar el id del grupo
				select @i=@i+2
				select @id=right(left(@path,@i),1)
				select @nombre=Name from Groups where ID=@id
				-- Concatenamos el valor al resultado final
				select @resultado=@resultado+' \ '+@nombre
				end
				-- Si el id del grupo esta formado por dos digitos
				if (@digitos=2)
				begin
					select @i=@i+3
					select @id=right(left(@path,@i),2)
					select @nombre=Name from Groups where ID=@id
					-- Concatenamos el valor al resultado final
					select @resultado=@resultado+' \ '+@nombre
				end
				-- Si el id del grupo esta formado por tres digitos
				if (@digitos=3)
				begin
				select @i=@i+2
				select @id=right(left(@path,@i),3)
				select @nombre=Name from Groups where ID=@id
				-- Concatenamos el valor al resultado final
				select @resultado=@resultado+' \ '+@nombre
				return @resultado
				--return 'El nombre es:'+@resultado
				end
			end
		end
	end
	return (@resultado)
END
GO




-------------------------- EMPLOYEE GROUPS ----------------------------------

DROP VIEW [dbo].[sysroEmployeeGroups]
GO
/****** Objeto:  View [dbo].[sysroEmployeeGroups]    Fecha de la secuencia de comandos: 01/08/2008 12:28:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sysroEmployeeGroups]
AS
SELECT     dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.GetFullGroupPathName(dbo.Groups.ID) AS FullGroupName
FROM         dbo.Groups INNER JOIN
                      dbo.EmployeeGroups ON dbo.Groups.ID = dbo.EmployeeGroups.IDGroup
GO


-------------------------- EMPLOYEES SHIFTS ---------------------------------
DROP VIEW [dbo].[sysroEmployeesShifts]
GO
/****** Objeto:  View [dbo].[sysroEmployeesShifts]    Fecha de la secuencia de comandos: 01/08/2008 12:29:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sysroEmployeesShifts]
AS
SELECT     dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, 
                      dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, dbo.Shifts.Name AS ShiftName, dbo.Shifts.ExpectedWorkingHours, 
                      dbo.DailySchedule.Date AS CurrentDate, dbo.sysroEmployeeGroups.SecurityFlags, dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.DailySchedule INNER JOIN
                      dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.Employees.ID = dbo.sysroEmployeeGroups.IDEmployee AND 
                      dbo.DailySchedule.Date >= dbo.sysroEmployeeGroups.BeginDate AND 
                      dbo.DailySchedule.Date <= dbo.sysroEmployeeGroups.EndDate LEFT OUTER JOIN
                      dbo.Shifts ON dbo.DailySchedule.IDShiftUsed = dbo.Shifts.ID

GO




-------------------------- ALL EMPLOYEE GROUPS ------------------------------

DROP VIEW [dbo].[sysrovwAllEmployeeGroups]
GO
/****** Objeto:  View [dbo].[sysrovwAllEmployeeGroups]    Fecha de la secuencia de comandos: 01/08/2008 12:32:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sysrovwAllEmployeeGroups]
AS
SELECT     GroupName, Path, SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.sysrovwCurrentEmployeeGroups
UNION
SELECT     GroupName, Path, SecurityFlags, IDEmployee, IDGroup, EmployeeName, BeginDate, EndDate, CurrentEmployee, dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.sysrovwFutureEmployeeGroups

GO

-------------------------- CURRENT EMPLOYEE GROUPS --------------------------

DROP VIEW [dbo].[sysrovwCurrentEmployeeGroups]
GO
/****** Objeto:  View [dbo].[sysrovwCurrentEmployeeGroups]    Fecha de la secuencia de comandos: 01/08/2008 12:32:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sysrovwCurrentEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, 
                      COUNT(dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee) AS CurrentEmployee, dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
GROUP BY dbo.Groups.Name, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate
HAVING      (dbo.EmployeeGroups.EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND 
                      (dbo.EmployeeGroups.BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102))

GO

-------------------------- EMERGENCY REPORT ---------------------------------
DROP VIEW [dbo].[sysrovwEmergencyReport]
GO
/****** Objeto:  View [dbo].[sysroEmergencyReport]    Fecha de la secuencia de comandos: 01/08/2008 12:16:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[sysrovwEmergencyReport]
AS
SELECT     'Empleados' AS Type, dbo.Employees.Name, m1.InDateTime, CASE charindex('#', description) WHEN 0 THEN description ELSE LEFT(description, 
                      charindex('#', description) - 1) END AS Terminal, dbo.Terminals.ID AS IDTerminal, CGR.GroupName, CGR.IDEmployee,dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.Moves AS m1 INNER JOIN
                      dbo.Employees ON m1.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Terminals ON m1.InIDReader = dbo.Terminals.ID INNER JOIN
                      dbo.sysrovwCurrentEmployeeGroups AS CGR ON m1.IDEmployee = CGR.IDEmployee
WHERE     (m1.OutDateTime IS NULL) AND (m1.InDateTime IN
                          (SELECT     MAX(InDateTime) AS Expr1
                            FROM          dbo.Moves AS m2
                            WHERE      (m1.IDEmployee = IDEmployee)))
UNION
SELECT     'Visitantes' AS Type, VT.Name, VM.BeginTime, CASE charindex('#', description) WHEN 0 THEN description ELSE LEFT(description, charindex('#', 
                      description) - 1) END AS Terminal, TR.ID AS IDTerminal, CGR.EmployeeName, CGR.IDEmployee,dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.VisitPlan AS VP INNER JOIN
                      dbo.VisitMoves AS VM ON VP.ID = VM.VisitPlanId INNER JOIN
                      dbo.Visitors AS VT ON VP.VisitorId = VT.ID INNER JOIN
                      dbo.Employees AS emp ON VP.EmpVisitedId = emp.ID INNER JOIN
                      dbo.Moves ON VP.EmpVisitedId = dbo.Moves.IDEmployee INNER JOIN
                      dbo.Terminals AS TR ON dbo.Moves.InIDReader = TR.ID INNER JOIN
                      dbo.sysrovwCurrentEmployeeGroups AS CGR ON VP.EmpVisitedId = CGR.IDEmployee
WHERE     (VM.EndTime IS NULL) AND (dbo.Moves.InDateTime =
                          (SELECT     MAX(InDateTime) AS Expr1
                            FROM          dbo.Moves AS m1
                            WHERE      (moves.IdEmployee = IDEmployee)))
UNION
SELECT     'Empleados' AS Type, CGR.EmployeeName, m1.InDateTime, 'Indeterminado' AS Terminal, '' AS IDTerminal, CGR.GroupName, CGR.IDEmployee,dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.Moves AS m1 INNER JOIN
                      dbo.sysrovwCurrentEmployeeGroups AS CGR ON m1.IDEmployee = CGR.IDEmployee
WHERE     (m1.OutDateTime IS NULL) AND (m1.InDateTime IN
                          (SELECT     MAX(InDateTime) AS Expr1
                            FROM          dbo.Moves AS m2
                            WHERE      (m1.IDEmployee = IDEmployee))) AND (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.Terminals AS T
                            WHERE      (m1.InIDReader = ID)))
UNION
SELECT     'Visitantes' AS Type, CONVERT(nvarchar(50), VT.Name) AS Expr1, VM.BeginTime, 'Indeterminado' AS Terminal, 0 AS Expr2, CGR.GroupName, 
                      CGR.IDEmployee,dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.VisitPlan AS VP INNER JOIN
                      dbo.VisitMoves AS VM ON VP.ID = VM.VisitPlanId INNER JOIN
                      dbo.Visitors AS VT ON VP.VisitorId = VT.ID INNER JOIN
                      dbo.Employees AS emp ON VP.EmpVisitedId = emp.ID INNER JOIN
                      dbo.Moves ON VP.EmpVisitedId = dbo.Moves.IDEmployee INNER JOIN
                      dbo.sysrovwCurrentEmployeeGroups AS CGR ON VP.EmpVisitedId = CGR.IDEmployee CROSS JOIN
                      dbo.Terminals AS TERM
WHERE     (VM.EndTime IS NULL) AND (dbo.Moves.InDateTime =
                          (SELECT     MAX(InDateTime) AS Expr1
                            FROM          dbo.Moves AS m1
                            WHERE      (moves.IdEmployee = IDEmployee))) AND (NOT EXISTS
                          (SELECT     1 AS Expr1
                            FROM          dbo.Terminals
                            WHERE      (dbo.Moves.InIDReader = ID)))
UNION
SELECT     'Empleados' AS Type, dbo.Employees.Name AS EmployeeName, m1.DateTime, CASE charindex('#', terminals.description) 
                      WHEN 0 THEN terminals.description ELSE LEFT(terminals.description, charindex('#', terminals.description) - 1) END AS Terminal, 
                      dbo.Terminals.ID AS IDTerminal, CGR.GroupName, CGR.IDEmployee,dbo.GetFullGroupPathName(IDGroup) AS FullGroupName
FROM         dbo.AccessMoves AS m1 INNER JOIN
                      dbo.Employees ON m1.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Terminals ON m1.IDReader = dbo.Terminals.ID INNER JOIN
                      dbo.sysrovwCurrentEmployeeGroups AS CGR ON m1.IDEmployee = CGR.IDEmployee INNER JOIN
                      dbo.Zones ON m1.IDZone = dbo.Zones.ID
WHERE     (m1.DateTime IN
                          (SELECT     MAX(DateTime) AS Expr1
                            FROM          dbo.AccessMoves AS m2
                            WHERE      (m1.IDEmployee = IDEmployee))) AND (dbo.Zones.IsWorkingZone = 1)
GO

-----------------------------------FutureEmployeeGroups----------------------
/****** Objeto:  View [dbo].[sysrovwFutureEmployeeGroups]    Fecha de la secuencia de comandos: 01/08/2008 12:54:46 ******/
DROP VIEW [dbo].[sysrovwFutureEmployeeGroups]
GO
/****** Objeto:  View [dbo].[sysrovwFutureEmployeeGroups]    Fecha de la secuencia de comandos: 01/08/2008 12:53:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[sysrovwFutureEmployeeGroups]
AS
SELECT     dbo.Groups.Name AS GroupName, dbo.Groups.Path, dbo.Groups.SecurityFlags, dbo.EmployeeGroups.IDEmployee, dbo.EmployeeGroups.IDGroup, 
                      dbo.Employees.Name AS EmployeeName, dbo.EmployeeGroups.BeginDate, dbo.EmployeeGroups.EndDate, '0' AS CurrentEmployee, 
                      dbo.GetFullGroupPathName(dbo.EmployeeGroups.IDGroup) AS FullGroupName
FROM         dbo.EmployeeGroups INNER JOIN
                      dbo.Employees ON dbo.EmployeeGroups.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Groups ON dbo.EmployeeGroups.IDGroup = dbo.Groups.ID LEFT OUTER JOIN
                      dbo.sysrosubvwCurrentEmployeePeriod ON dbo.Employees.ID = dbo.sysrosubvwCurrentEmployeePeriod.IDEmployee
WHERE     (dbo.EmployeeGroups.BeginDate > CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND (dbo.Employees.ID NOT IN
                          (SELECT     IDEmployee
                            FROM          dbo.sysrovwCurrentEmployeeGroups))

GO



--- Nueva extensión de análisis de datos
CREATE TABLE [dbo].[Cubes](
	[ID] [smallint] NOT NULL,
	[Version] [smallint] NOT NULL CONSTRAINT [DF_Cubes_Version]  DEFAULT ((0)),
	[Name] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Description] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[ParametersSQL] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Parent] [smallint] NULL CONSTRAINT [DF_Cubes_Parent]  DEFAULT ((0)),
	[Layout] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_Cubes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)ON [PRIMARY]
) ON [PRIMARY] 
GO


CREATE TABLE [dbo].[sysroDailyIncidencesDescription](
	[IDIncidence] [smallint] NOT NULL,
	[Description] [nvarchar](100) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
 CONSTRAINT [PK_sysroDailyIncidencesDescription] PRIMARY KEY CLUSTERED 
(
	[IDIncidence] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity, RequiredFeatures) 
	VALUES ('NavBar\FirstTab\Analytics','Analytics','roFormAnalytics.vbd','Analytics.ico','311111111111111111111111111111',1200,'NWR','')
GO



-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='206' WHERE ID='DBVersion'
GO





