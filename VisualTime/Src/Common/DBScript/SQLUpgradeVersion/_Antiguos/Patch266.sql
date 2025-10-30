DROP TABLE TMPDailyPartialAccruals
GO

CREATE TABLE [dbo].[TMPDailyPartialAccruals](
	[FullGroupName] [nvarchar](500) NOT NULL,
	[IDGroup] [int] NOT NULL,
	[GroupName] [nvarchar](200) NOT NULL,
	[Position] [int] NULL,
	[IDReportGroup] [int] NULL,
	[TotalValueGroup] [numeric](16, 2) NULL,
	[Path] [nvarchar](200) NULL,
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](100) NOT NULL,
	[TotalValueEmployee] [numeric](16, 2) NULL,
	[Day] [smalldatetime] NULL,
	[ShiftName] [nvarchar](200) NULL,
	[ExpectedWorkingHours] [numeric](16, 2) NULL,
	[Acum1] [numeric](16, 5) NULL,
	[Acum2] [numeric](16, 5) NULL,
	[Acum3] [numeric](16, 5) NULL,
	[Acum4] [numeric](16, 5) NULL,
	[Acum5] [numeric](16, 5) NULL,
	[Acum6] [numeric](16, 5) NULL,
	[Acum7] [numeric](16, 5) NULL,
	[Acum8] [numeric](16, 5) NULL,
	[Acum9] [numeric](16, 5) NULL,
	[Acum10] [numeric](16, 5) NULL,
	[Acum11] [numeric](16, 5) NULL,
	[Acum12] [numeric](16, 5) NULL,
	[Acum13] [numeric](16, 5) NULL
) ON [PRIMARY]
GO


-- Incidencias Previstas para Live
ALTER TABLE [dbo].[ProgrammedCauses] ADD 
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL
GO

INSERT INTO sysroFeatures  ([ID]
           ,[IDParent]
           ,[Alias]
           ,[Name]
           ,[Description]
           ,[Type]
           ,[PermissionTypes]
           ,[AppHasPermissionsOverEmployees])
VALUES (21150, 21100, 'Planification.Requests.PlannedCause', 'Incidencia prevista','', 'E', 'RW',NULL)
GO

INSERT INTO sysroFeatures  ([ID]
           ,[IDParent]
           ,[Alias]
           ,[Name]
           ,[Description]
           ,[Type]
           ,[PermissionTypes]
           ,[AppHasPermissionsOverEmployees])
VALUES (2550, 2500, 'Calendar.Requests.PlannedCause', 'Incidencia prevista','', 'U', 'RWA',NULL)
GO

ALTER TABLE [dbo].[Requests] ADD 
	[FromTime] [datetime] NULL,
	[ToTime] [datetime] NULL
GO


/* Función [GetRequestLevelsBelow] **********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetRequestLevelsBelow]
GO

 CREATE FUNCTION [dbo].[GetRequestLevelsBelow]
 	(
 		@idPassport int,
		@idRequest int
 	)
 RETURNS int
 AS
 BEGIN

 	DECLARE @LevelsBelow int
 	DECLARE @LevelOfAuthority int

	SELECT @LevelOfAuthority = CASE GroupType WHEN 'U' THEN sysroPassports.LevelOfAuthority ELSE (SELECT Parent.LevelOfAuthority FROM sysroPassports Parent WHERE Parent.ID = sysroPassports.IDParentPassport) END
	FROM sysroPassports 
	WHERE sysroPassports.ID = @idPassport

	DECLARE @featureAlias nvarchar(100),
			@EmployeefeatureID int,
 			@idEmployee int,
			@RequestLevel int
 	
	SELECT @featureAlias = CASE Requests.RequestType 
								WHEN 1 THEN 'Employees.UserFields.Requests' 
								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
								WHEN 6 THEN 'Calendar.Requests.Vacations'
								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
								WHEN 9 THEN 'Calendar.Requests.PlannedCause'
								ELSE 'Calendar.Requests.ShiftExchange' END,
		   @EmployeefeatureID = CASE Requests.RequestType
								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 9 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
		   @idEmployee = Requests.IDEmployee,
		   @RequestLevel = ISNULL(Requests.StatusLevel, 11)
	FROM Requests
	WHERE Requests.ID = @idRequest

	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
	SELECT @LevelsBelow = 
	(SELECT COUNT( DISTINCT Parents.LevelOfAuthority)
	 FROM sysroPassports INNER JOIN sysroPassports Parents
			ON sysroPassports.IDParentPassport = Parents.ID
	 WHERE sysroPassports.GroupType = '' AND
	 	  Parents.LevelOfAuthority IS NOT NULL AND
	 	  Parents.LevelOfAuthority > @LevelOfAuthority AND
		  Parents.LevelOfAuthority <= @RequestLevel AND
		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 0 AND
		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 0 
	 )

	IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
 	
 RETURN @LevelsBelow
 END
GO

/* Función [GetRequestPassportPermission] **********************************************************************************/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroupParent]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetRequestPassportPermission]
GO

CREATE FUNCTION [dbo].[GetRequestPassportPermission]
(
	@idPassport int,
	@idRequest int	
)
RETURNS int
AS
BEGIN

	DECLARE @FeatureAlias nvarchar(100),
			@EmployeefeatureID int,
			@idEmployee int

	SELECT @FeatureAlias = CASE Requests.RequestType 
								WHEN 1 THEN 'Employees.UserFields.Requests' 
								WHEN 2 THEN 'Calendar.Punches.Requests.Forgotten'
								WHEN 3 THEN 'Calendar.Punches.Requests.Justify'
								WHEN 4 THEN 'Calendar.Punches.Requests.ExternalParts'
								WHEN 5 THEN 'Calendar.Requests.ShiftChange'
								WHEN 6 THEN 'Calendar.Requests.Vacations'
								WHEN 7 THEN 'Calendar.Requests.PlannedAbsence'
								WHEN 9 THEN 'Calendar.Requests.PlannedCause'
								ELSE 'Calendar.Requests.ShiftExchange' END,
		   @EmployeefeatureID = CASE Requests.RequestType 
								WHEN 1 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Employees') 
								WHEN 2 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 3 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 4 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 5 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 6 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 7 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								WHEN 9 THEN (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar')
								ELSE (SELECT ID FROM sysroFeatures WHERE Alias = 'Calendar') END,
		   @idEmployee = Requests.IDEmployee
	FROM Requests
	WHERE Requests.ID = @idRequest


	DECLARE @Permission int

	SET @Permission = (select dbo.WebLogin_GetPermissionOverFeature(@idPassport,@FeatureAlias, 'U', 2)) 
	
	IF @Permission > 0 
		BEGIN

			SET @Permission = (select dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@idEmployee,@EmployeefeatureID,2,1,getdate()))

		END
		   
	RETURN @Permission

END
GO

CREATE TABLE [dbo].[TMPWhoShouldComeAndInst](
	[IDEmployee] [int] NOT NULL,
	[Shifts] [nvarchar](500) NULL,
	[Start] [nvarchar](50) NULL,
	[Cause] [nvarchar](500) NULL,
	[AbsenceType] [nvarchar](1) NULL,
	[Duration] [nvarchar](10) NULL
) ON [PRIMARY]
GO

-- Creamos funcion de SQL para convertir a hexadecimal una tarjeta
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create function [dbo].[fn_decToBase] 
( 
    
@val as BigInt, 
    
@base as int
) 
returns varchar(63) 
as
Begin
   
      If (@val<0) OR (@base < 2) OR (@base> 36) Return Null; 
 
Declare @answer as varchar(63); 
 
Declare @alldigits as varchar(36);
Set @alldigits='0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'

Set @answer=''; 
While @val>0 
Begin
Set @answer=Substring(@alldigits,@val % @base + 1,1) + @answer; 
Set @val = @val / @base; 
End
return @answer;
End
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='266' WHERE ID='DBVersion'
GO
