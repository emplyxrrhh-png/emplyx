 ALTER FUNCTION [dbo].[WebLogin_GetPermissionOverEmployee] 
 	(
 		@idPassport int,
 		@idEmployee int,
 		@idApplication int,
 		@mode int,
 		@includeGroups bit,
 		@date datetime
 	)
 RETURNS int
 AS
 BEGIN
 	DECLARE @Result int
 	DECLARE @LoopPassport int
 	DECLARE @IDGroup int
 	DECLARE @GroupType nvarchar(50)
 	
 	/* First look at employees exceptions, on specified passport then on 
 	each of it's parents. If nothing is found, look at groups permissions. */
 	
 	/* Check permissions */
 	IF @mode <> 2
 		SET @LoopPassport = @idPassport
 	ELSE
 		SELECT @GroupType = isnull(GroupType, '')
 		FROM sysroPassports
 		WHERE ID = @idPassport
 		
 		if @GroupType = 'U'
 		begin
 			SET @LoopPassport = @idPassport
 		end
 		else
 		begin
 			SELECT @LoopPassport = @idPassport
 			FROM sysroPassports
 			WHERE ID = @idPassport
 		end
 	
 	
 	WHILE @Result IS NULL AND NOT @LoopPassport IS NULL
 	BEGIN
 		SELECT @Result = Permission
 		FROM sysroPassports_PermissionsOverEmployees
 		WHERE IDPassport = @LoopPassport AND
 			IDApplication = @idApplication AND
 			IDEmployee = @idEmployee
 		
 		SELECT @LoopPassport = IDParentPassport
 		FROM sysroPassports
 		WHERE ID = @LoopPassport
 	END
 	
 	IF @Result IS NULL AND @includeGroups = 1
 	BEGIN
 		/* If nothing is found directly on employee, 
 		look at groups permissions */
 		IF @Result IS NULL
 		BEGIN
 			SELECT @IDGroup = dbo.GetEmployeeGroup(@idEmployee, @date)
 			IF NOT @IDGroup IS NULL
 				SELECT @Result = dbo.WebLogin_GetPermissionOverGroup(@idPassport, @IDGroup, @idApplication, @mode)
 		END
 	END
 	
 	
 	/* Return result */
 	IF @Result IS NULL
 		SET @Result = 0
 		
 	RETURN @Result
 END
GO


-- Error en el informe de los que mas
ALTER TABLE dbo.TMPTOP
	DROP CONSTRAINT PK_TMPTOP
GO


DROP TABLE [dbo].[TMPTOP]
GO


CREATE TABLE [dbo].[TMPTOP](
	[ID] [numeric](19,0) IDENTITY (1, 1) NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](50) NOT NULL,
	[IDPrimaryConcept] [smallint] NOT NULL,
	[IDSecundaryConcept] [smallint] NULL,
	[ConceptPrimaryName] [nvarchar](50) NOT NULL,
	[ConceptSecundaryName] [nvarchar](50) NULL,
	[Date] [datetime] NOT NULL,
	[BeginTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
	[Value] [numeric](19, 6) NULL
) ON [PRIMARY]

GO

ALTER TABLE  [dbo].[TMPTOP] WITH NOCHECK ADD CONSTRAINT [PK_TMPTOP] PRIMARY KEY  NONCLUSTERED ([ID]) ON [PRIMARY]
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='325' WHERE ID='DBVersion'
GO

