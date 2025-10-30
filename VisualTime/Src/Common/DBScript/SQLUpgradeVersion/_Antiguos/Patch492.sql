update dbo.sysrogui set Parameters = 'SecurityV1' where Parameters = 'SingleSecurity'
GO

update dbo.sysrogui set Parameters = 'SecurityV2' where Parameters = 'AdvancedSecurity'
GO

update dbo.sysrogui set Parameters = 'SecurityV2;SecurityV3' where IDPath ='Portal\Company\SecurityFunctions'
GO

insert into dbo.sysrogui values ('Portal\Company\AdvSupervisors', 'Gui.Supervisors', 'Security/Supervisors.aspx', 'Supervisors.png', null, 'SecurityV3', 'Forms\Passports', null, 105, null, 'U:Administration.Security=Read')
GO

insert into dbo.sysroGUI_Actions(IDPath,IDGUIPath,LanguageTag,RequieredFeatures,RequieredFunctionalities,AfterFunction,CssClass,Section,ElementIndex,AppearsOnPopup) 
	select IDPath,'Portal\Security\Passports\Supervisors', LanguageTag,RequieredFeatures,RequieredFunctionalities,AfterFunction,CssClass,Section,ElementIndex,AppearsOnPopup from sysroGUI_Actions where IDGUIPath = 'Portal\Security\SecurityChart\Supervisors'
GO

delete from dbo.sysroGUI where IDPath like 'Menu\%'
GO

delete from dbo.sysroGUI where IDPath like 'NavBar\%'
GO

delete from dbo.sysroGUI where IDPath = 'NavBar'
GO

delete from dbo.sysroGUI where IDPath = 'Menu'
GO

-- tipos de categorias
CREATE TABLE [dbo].[sysroCategoryTypes](
	[ID] [int] NOT NULL,
	[Description] [nvarchar](MAX) NOT NULL,
 CONSTRAINT [PK_sysroCategoryTypes] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

INSERT INTO [dbo].[sysroCategoryTypes] ([ID] ,[Description]) VALUES(0,'Prevention')
GO
INSERT INTO [dbo].[sysroCategoryTypes] ([ID] ,[Description]) VALUES(1,'Labor')
GO
INSERT INTO [dbo].[sysroCategoryTypes] ([ID] ,[Description]) VALUES(2,'Legal')
GO
INSERT INTO [dbo].[sysroCategoryTypes] ([ID] ,[Description]) VALUES(3,'Security')
GO
INSERT INTO [dbo].[sysroCategoryTypes] ([ID] ,[Description]) VALUES(4,'Quality')
GO
INSERT INTO [dbo].[sysroCategoryTypes] ([ID] ,[Description]) VALUES(5,'Planning')
GO
INSERT INTO [dbo].[sysroCategoryTypes] ([ID] ,[Description]) VALUES(6,'Attendance Control')
GO

-- Asignar categoría por defecto a cada tipo de solicitud
ALTER TABLE [dbo].[sysroRequestType] ADD [IDCategory] [int] NULL
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 1 WHERE IdType=1 -- Campo de la ficha / Labor
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 6 WHERE IdType=2 -- Fichaje olvidado / Attendance control
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 6 WHERE IdType=3 -- Justificar fichaje / Attendance control
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 6 WHERE IdType=4 -- Parte de trabajo externo / Attendance control
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 5 WHERE IdType=5 -- Cambio de horario / Planning
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 5 WHERE IdType=6 -- Vacaciones o permisos / Planning
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = -1 WHERE IdType=7 -- Prevision ausencia por dia / Sin asignar
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 5 WHERE IdType=8 -- Intercambio de horarios entre empleados / Planning
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = -1 WHERE IdType=9 -- Prevision ausencia por horas / Sin asignar
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 6 WHERE IdType=10 -- Fichaje olvidado de productiv/ Attendance control
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 5 WHERE IdType=11 -- Cancelacion de vacaciones/ Planning
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 6 WHERE IdType=12 -- Fichaje olvidado centro de coste/ Attendance control
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = -1 WHERE IdType=13 -- Prevision de vacaciones por horas/ sin asignar
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = -1 WHERE IdType=14 -- Prevision de exceso por horas/ sin asignar
GO
UPDATE [dbo].[sysroRequestType] SET [IDCategory] = 6 WHERE IdType=15 -- Resumen de trabajo externo semanal/ Attendance control
GO


-- Asignamos por defecto una categoria a cada tipo de notificacion
-- inicialmente por defecto todas como Control de presencia
ALTER TABLE [dbo].[sysroNotificationTypes] ADD [IDCategory] [INT] null DEFAULT(0)
GO
UPDATE sysroNotificationTypes SET IDCategory=6 where IDCategory is null
GO


-- Categorias que puede gestionar un supervisor y su nivel de mando
CREATE TABLE [dbo].[sysroPassports_Categories](
	[IDPassport] [int] NOT NULL,
	[IDCategory] [int] NOT NULL,
	[LevelOfAuthority] [tinyint] NOT NULL,
	[ShowFromLevel] [tinyint] NOT NULL,
 CONSTRAINT [PK_sysroPassports_Categories] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC,
	[IDCategory] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO



--- El supervisor puede aprobar sus propias solicitudes
ALTER TABLE [dbo].[sysroPassports] ADD [CanApproveOwnRequests] [bit] NULL DEFAULT(1)
GO 
update [sysroPassports] set [CanApproveOwnRequests]=1 where [CanApproveOwnRequests] is Null
GO

--- Indicamos el role de permisos al supervisor
ALTER TABLE [dbo].[sysroPassports] ADD [IDGroupFeature] [int] NULL DEFAULT(0)
GO 

-- Asignamos el rol actual a los supervisores existentes en el caso que tuvieran
UPDATE sysroPassports SET IDGroupFeature = (SELECT top 1 IDGroupFeature FROM sysroSecurityNode_Passports WHERE  sysroSecurityNode_Passports.IDPassport = sysroPassports.ID) 
GO

-- Grupos asignados al supervisor
CREATE TABLE [dbo].[sysroPassports_Groups](
	[IDPassport] [int] NOT NULL,
	[IDGroup] [int] NOT NULL,
 CONSTRAINT [PK_sysroPassports_Groups] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC,
	[IDGroup] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


-- Justificaciones
-- Asignar categoria
-- Empezar en el nivel de autorización X
-- Considerar aprobada al llegara nivel X
ALTER TABLE [dbo].[Causes] ADD [IDCategory] [int] NULL DEFAULT(6), 	
	[MinLevelOfAuthority] [tinyint] NULL DEFAULT(11),
	[ApprovedAtLevel] [tinyint] NULL DEFAULT(1)
GO 
UPDATE [Causes] SET MinLevelOfAuthority=11 WHERE MinLevelOfAuthority IS NULL
GO
UPDATE [Causes] SET ApprovedAtLevel=1 WHERE ApprovedAtLevel IS NULL
GO
UPDATE [Causes] SET IDCategory=6 WHERE IDCategory IS NULL
GO




ALTER FUNCTION [dbo].[GetPassportLevelOfAuthority]  
  (  
   @idPassport int,
   @Type int,	  
   @ID int,
   @Version int  
  )  
  RETURNS int  
  AS  
  BEGIN 
   declare @levelOfAuthority int  
 	, @pidPassport int = @idPassport,
	@pType int = @Type,
	@pID int = @ID,
	@pVersion int = @Version
   ;

   if (@pVersion = 3)
   -- si es seguridad v3
   begin

		if (@pID = -2 or @pID = -3)
	   begin
   			if (@pID = -2 )
			-- si son notificaciones 
			--  verificamos el nivel de autorizacion del supervisor sobre la categoría del tipo de la notificacion
			  select @levelOfAuthority = (select LevelOfAuthority FROM sysroPassports_Categories WHERE IDPassport = @pidPassport AND IDCategory IN(select IDCategory FROM sysroNotificationTypes where Id= @pType));
			else
			-- si son documentos  -3
			--  verificamos el nivel de autorizacion del supervisor sobre la categoría que nos pasan
			  select @levelOfAuthority = (select LevelOfAuthority FROM sysroPassports_Categories WHERE IDPassport = @pidPassport AND IDCategory =@pType);
	   end
	   else
	   begin
			-- si son solicitudes, en funcion del tipo
		  if (@pType = 7 or @pType = 9 or @pType = 13 or @pType = 14 )
		  begin
  			-- si es una solicitud de Prevision de ausencia o de exceso
			--  verificamos el nivel de autorizacion del supervisor sobre la categoría de la justificación de la solicitud
			select @levelOfAuthority = (select LevelOfAuthority FROM sysroPassports_Categories WHERE IDPassport = @pidPassport AND IDCategory IN(select IDCategory FROM Causes where Id= @pID));
		  end
		  else
		  begin
			-- si es cualquier otra solicitud
			--  verificamos el nivel de autorizacion del supervisor sobre la categoría del tipo de la solicitud
				select @levelOfAuthority = (select LevelOfAuthority FROM sysroPassports_Categories WHERE IDPassport = @pidPassport AND IDCategory IN(select IDCategory FROM sysroRequestType where IdType= @pType));
		   end
	   end

	   IF @levelOfAuthority is null set @levelOfAuthority = 15  
   end

   if (@pVersion <> 3)
   -- si es seguridad v1 o v2
   begin
	  with cte (id,idparentpassport,levelofauthority)  
	  as  
	  (  
	  select ID,IDParentPassport, LevelOfAuthority from sysroPassports where id = @pidPassport   
	   union all   
	  select t.id,t.IDParentPassport, t.LevelOfAuthority  
	  from sysroPassports t join cte c on t.id = c.idparentpassport  
	  )  
	  select @levelOfAuthority =  (select top 1 levelofauthority from cte where not levelofauthority is null)  
	  IF @levelOfAuthority is null set @levelOfAuthority = 1  
   end

    RETURN @levelOfAuthority  
END
GO

ALTER FUNCTION [dbo].[GetRequestLevelsBelow]
     	(
     	@idPassport int,
     	@idRequest int,
		@Version int 
     	)
     RETURNS int
     AS
     BEGIN
     	DECLARE @LevelsBelow int,
     			@LevelOfAuthority int,
     			@RequestLevel int,
				@pVersion int = @Version;
     	
		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
     	
     		SELECT @RequestLevel = ISNULL(Requests.StatusLevel, 12), @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport,Requests.RequestType,isnull(Requests.IDCause,0),3)
			FROM Requests
     		WHERE Requests.ID = @idRequest
     	
     		/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
     		SELECT @LevelsBelow = 
  		(
  			SELECT COUNT( DISTINCT trpp.LevelOfAuthority) from 
  			(SELECT dbo.GetPassportLevelOfAuthority(sysroPassports.ID,Requests.RequestType,isnull(Requests.IDCause,0),3) AS LevelOfAuthority, IDRequest FROM sysroPermissionsOverRequests INNER JOIN Requests ON sysroPermissionsOverRequests.IDRequest = Requests.ID INNER JOIN sysroPassports ON sysroPermissionsOverRequests.IDParentPassport = sysroPassports.IDParentPassport WHERE IDRequest = @idRequest)trpp
  			WHERE trpp.LevelOfAuthority > @LevelOfAuthority AND trpp.LevelOfAuthority <= @RequestLevel
  		)
		  IF @RequestLevel = 12  SET @LevelsBelow = @LevelsBelow + 1
     	end

		 if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport,0,0,1)
    	
    		SELECT @RequestLevel = ISNULL(Requests.StatusLevel, 11)
		   FROM Requests
    		WHERE Requests.ID = @idRequest
    	
    		/* Obtiene el nÃºmero de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
    		SELECT @LevelsBelow = 
 		(
 			SELECT COUNT( DISTINCT trpp.LevelOfAuthority) from 
 			(SELECT dbo.GetPassportLevelOfAuthority(IDParentPassport,0,0,1) AS LevelOfAuthority, IDRequest FROM sysroPermissionsOverRequests WHERE IDRequest = @idRequest)trpp
 			WHERE trpp.LevelOfAuthority > @LevelOfAuthority AND trpp.LevelOfAuthority <= @RequestLevel
 		)
		 IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1

		end


     	RETURN @LevelsBelow
END
GO

ALTER FUNCTION [dbo].[GetRequestNextLevel]
  (	
    	@idRequest int,
		@Version int 	
  )
  RETURNS int
  AS
  BEGIN
    	DECLARE @NextLevel int, @pVersion int = @Version;
		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
    		SELECT @NextLevel = 
    		(SELECT MAX(dbo.GetPassportLevelOfAuthority(sysroPassports.ID,Requests.RequestType,isnull(Requests.IDCause,0),3))
    		FROM sysroPassports, Requests  
    		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
    			  AND sysroPassports.Description not like '%@@ROBOTICS@@%' AND 
    			  dbo.GetPassportLevelOfAuthority(sysroPassports.ID,Requests.RequestType,isnull(Requests.IDCause,0),3) < ISNULL(Requests.StatusLevel, 12) AND
    			  dbo.GetRequestPassportPermission(sysroPassports.ID, Requests.ID) > 3 AND Requests.ID = @idRequest)
    	end

		 if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			SELECT @NextLevel = 
   			(SELECT MAX(dbo.GetPassportLevelOfAuthority(Parents.ID,0,0,1))
   			FROM sysroPassports  INNER JOIN sysroPassports Parents
 				ON sysroPassports.IDParentPassport = Parents.ID
   			WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
   				  AND sysroPassports.Description not like '%@@ROBOTICS@@%' AND 
   				  dbo.GetPassportLevelOfAuthority(Parents.ID,0,0,1) < ISNULL(Requests.StatusLevel, 11) AND
   				  dbo.GetRequestPassportPermission(sysroPassports.ID, Requests.ID) > 3)
   			FROM Requests
   			WHERE Requests.ID = @idRequest 
		end

		
			   
    	RETURN @NextLevel
END
GO

ALTER FUNCTION [dbo].[GetRequestNextLevelPassports]
   (	
     	@idRequest int,
		@Version int 		
   )
   RETURNS nvarchar(1000)
   AS
   BEGIN
     	DECLARE @RetNames nvarchar(1000), @pVersion int = @Version;
		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
     		SET @RetNames = ''
 			SET @RetNames = (SELECT sysroPassports.Name + ','  AS [text()]
     				 FROM Requests , sysroPassports 
     				 INNER JOIN (SELECT sysroPassports.id, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,Requests.RequestType,isnull(Requests.IDCause,0),3) AS Level FROM sysroPassports, Requests WHERE Requests.ID = @idRequest) gpl
     				 on sysroPassports.ID=gpl.ID 
     				 WHERE Requests.ID = @idRequest  AND sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
     				 AND sysroPassports.Description not like '%@@ROBOTICS@@%'
     				 AND gpl.level=dbo.GetRequestNextLevel(@idRequest,3)
   					 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 3
     				 ORDER BY sysroPassports.Name For XML PATH (''))
    			 
 			IF @RetNames <> ''
     			BEGIN
     				SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
     			END
		end

		 if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			SET @RetNames = ''

			SET @RetNames = (SELECT sysroPassports.Name + ','  AS [text()]
    				 FROM sysroPassports 
    				 INNER JOIN (SELECT id, dbo.GetPassportLevelOfAuthority(ID,0,0,1) AS Level FROM sysroPassports) gpl
    				 on sysroPassports.ID=gpl.ID 
    				 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
    				 AND sysroPassports.Description not like '%@@ROBOTICS@@%'
    				 AND gpl.level=dbo.GetRequestNextLevel(@idRequest,1)
  					 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 3
    				 ORDER BY sysroPassports.Name For XML PATH (''))

			IF @RetNames <> ''
    			BEGIN
    				SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    			END
		end
		
     	RETURN @RetNames
END
GO

ALTER FUNCTION [dbo].[GetRequestNextLevelPassportsIDs]
   (	
     	@idRequest int	,
		@Version int 	
   )
   RETURNS nvarchar(1000)
   AS
   BEGIN
 		DECLARE @RetNames nvarchar(1000), @pVersion int = @Version;
		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
     		SET @RetNames = ''
 			SET @RetNames = (SELECT CONVERT(nvarchar(max),sysroPassports.ID)  + ','  AS [text()]
     				 FROM Requests ,sysroPassports 
     				 INNER JOIN (SELECT sysroPassports.id, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,Requests.RequestType,isnull(Requests.IDCause,0),3) AS Level FROM sysroPassports, Requests WHERE Requests.ID = @idRequest) gpl
     				 on sysroPassports.ID=gpl.ID 
     				 WHERE Requests.ID = @idRequest  AND sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
 					 AND sysroPassports.Description not like '%@@ROBOTICS@@%'
     				 AND gpl.level=dbo.GetRequestNextLevel(@idRequest,3)
   					 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 3
     				 ORDER BY sysroPassports.Name For XML PATH (''))
 			IF @RetNames <> ''
     			BEGIN
     				SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
     			END
		end

		 if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			SET @RetNames = ''

			SET @RetNames = (SELECT CONVERT(NVARCHAR(4000),sysroPassports.ID)  + ','  AS [text()]
    				 FROM sysroPassports 
    				 INNER JOIN (SELECT id, dbo.GetPassportLevelOfAuthority(ID,0,0,1) AS Level FROM sysroPassports) gpl
    				 on sysroPassports.ID=gpl.ID 
    				 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
					 AND sysroPassports.Description not like '%@@ROBOTICS@@%'
    				 AND gpl.level=dbo.GetRequestNextLevel(@idRequest,1)
  					 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 3
    				 ORDER BY sysroPassports.Name For XML PATH (''))

			IF @RetNames <> ''
    			BEGIN
    				SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    			END
		end


     	RETURN @RetNames
END
GO

ALTER PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverGroups]
 (	
		@Version int 	
   )
     AS
     BEGIN
   	
 	DELETE FROM sysroPermissionsOverGroups

 	DECLARE @featureEmployeeID int, @pVersion int = @Version;
  	DECLARE EmployeeFeatureCursor CURSOR
  	FOR 
  		SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null
  	OPEN EmployeeFeatureCursor
  	FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID
  	WHILE @@FETCH_STATUS = 0
  	BEGIN	
  		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
  			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
  					select supPassports.ID, Groups.ID, @featureEmployeeID, 1,
  							dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
  					from sysroPassports supPassports, Groups
  					where supPassports.GroupType = 'U'
		end

		if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
 					select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),
 							dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
 					from sysroPassports supPassports, Groups
 					where supPassports.GroupType = 'U'

  		end

  		FETCH NEXT FROM EmployeeFeatureCursor 
  		INTO @featureEmployeeID
  	END 
  	CLOSE EmployeeFeatureCursor
  	DEALLOCATE EmployeeFeatureCursor
 	    	
END
GO


ALTER PROCEDURE [dbo].[sysro_UpdatePassportPermissionsOverGroups]  
   (  
    @idPassport int ,@Version int 	 
   )  
      AS  
      BEGIN  
   DECLARE @featureEmployeeID int, 
   @pidPassport int = @idPassport  , @pVersion int = @Version;
   DECLARE @updatePassportIDs table(IDPassport int)  
   ;WITH cte AS   
   (  
   SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType  
   FROM sysroPassports a  
   WHERE Id = @pidPassport and GroupType = 'U'  
   UNION ALL  
   SELECT a.Id, a.IDParentPassport, a.Name, a.GroupType  
   FROM sysropassports a JOIN cte c ON a.IDParentPassport = c.id  
   where a.GroupType = 'U'  
   )  
   INSERT INTO @updatePassportIDs SELECT Id FROM cte  
   DELETE FROM sysroPermissionsOverGroups where PassportID in (select IDPassport from @updatePassportIDs)  
    DECLARE EmployeeFeatureCursor CURSOR  
    FOR   
     SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null  
    OPEN EmployeeFeatureCursor  
    FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID  
    WHILE @@FETCH_STATUS = 0  
    BEGIN   
		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
		 INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
		   select supPassports.ID, Groups.ID, @featureEmployeeID, 1,  
			 dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)  
		   from sysroPassports supPassports, Groups  
		   where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)  
	   end
		if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
 						select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),
 								dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
 						from sysroPassports supPassports, Groups
 						where supPassports.GroupType = 'U' and supPassports.id in(select IDPassport from @updatePassportIDs)
		end

     FETCH NEXT FROM EmployeeFeatureCursor   
     INTO @featureEmployeeID  
    END   
    CLOSE EmployeeFeatureCursor  
    DEALLOCATE EmployeeFeatureCursor  
      END
GO

ALTER PROCEDURE [dbo].[InsertGroupPassportPermission]  
   (  
    @idGroup int  ,@Version int
   )  
      AS  
      BEGIN  
   DECLARE @pidGroup int = @idGroup  , @pVersion int = @Version;  
   DELETE FROM sysroPermissionsOverGroups WHERE EmployeeGroupID = @pidGroup  
   DECLARE @featureEmployeeID int  
    DECLARE EmployeeFeatureCursor CURSOR  
    FOR   
     SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null  
    OPEN EmployeeFeatureCursor  
    FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID  
    WHILE @@FETCH_STATUS = 0  
    BEGIN   
		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
		 INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
		   select supPassports.ID, Groups.ID, @featureEmployeeID, 1,  
			 dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)  
		   from sysroPassports supPassports, Groups  
		   where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup  
		end
		if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
			INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission)   
		  select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),  
			dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)  
		  from sysroPassports supPassports, Groups  
		  where supPassports.GroupType = 'U' AND Groups.ID = @pidGroup  

		end

     FETCH NEXT FROM EmployeeFeatureCursor   
     INTO @featureEmployeeID  
    END   
    CLOSE EmployeeFeatureCursor  
    DEALLOCATE EmployeeFeatureCursor  
      END
GO

ALTER PROCEDURE [dbo].[sysro_GenerateAllPermissionsOverGroupsByEmployeeFeatureID]
  (
      		@EmployeeFeatureID int,@Version int
      	)
      AS
      BEGIN
    	DECLARE @featureEmployeeID int, @pVersion int = @Version; 
  	DELETE FROM sysroPermissionsOverGroups where employeefeatureid= @EmployeeFeatureID
   	DECLARE EmployeeFeatureCursor CURSOR
   	FOR 
   		SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid = @EmployeeFeatureID
   	OPEN EmployeeFeatureCursor
   	FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID
   	WHILE @@FETCH_STATUS = 0
   	BEGIN	
   		if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
   		INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
   				select supPassports.ID, Groups.ID, @featureEmployeeID, 1,
   						dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
   				from sysroPassports supPassports, Groups
   				where supPassports.GroupType = 'U'
		end
		if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
		INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
  				select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID,0,0,1),
  						dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
  				from sysroPassports supPassports, Groups
  				where supPassports.GroupType = 'U'

		end
   		
   		FETCH NEXT FROM EmployeeFeatureCursor 
   		INTO @featureEmployeeID
   	END 
   	CLOSE EmployeeFeatureCursor
   	DEALLOCATE EmployeeFeatureCursor
  	    	
      END
GO

 ALTER PROCEDURE [dbo].[WebLogin_Passports_Select] 
   (
   	@idPassport int,@Version int
   )
   AS
   DECLARE  @pVersion int = @Version;
   if (@pVersion = 3)
	   -- si es seguridad v3
	   begin
      SELECT ID,
      	IDParentPassport, GroupType, Name, Description,	Email, IDUser, IDEmployee, IDLanguage, 1 AS LevelOfAuthority,
      	ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State], EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp ,
   		EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode, EnabledVTVisits, EnabledVTVisitsApp, 
 		PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
      FROM sysroPassports
      WHERE ID = @idPassport
	  end

	  if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
		 SELECT ID,
     	IDParentPassport, GroupType, Name, Description,	Email, IDUser, IDEmployee, IDLanguage, dbo.GetPassportLevelOfAuthority(@idPassport,0,0,1) AS LevelOfAuthority,
     	ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State], EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp ,
  		EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode, EnabledVTVisits, EnabledVTVisitsApp, 
		PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
     FROM sysroPassports
     WHERE ID = @idPassport

		end
     	
  RETURN
  GO

  CREATE FUNCTION [dbo].[GetDirectSupervisorByNotification]
   (	
     	@idEmployee int,
     	@idNotification int
   )
   RETURNS nvarchar(1000)
   AS
   BEGIN
     DECLARE @RetNames nvarchar(1000)
 	SET @RetNames = ''
 	DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(70))
 	insert into @tmpTable 
	select sysroPassports_Categories.IDPassport, sysroPassports_Categories.LevelOfAuthority, sysroPassports.Name  FROM sysroPassports_Categories inner join  sysroPassports on sysroPassports_Categories.IDPassport =  sysroPassports.id  WHERE IDCategory IN(select IDCategory FROM sysroNotificationTypes where id=@idNotification) AND  sysroPassports.Description not like '%@@ROBOTICS@@%' AND  sysroPassports.IsSupervisor = 1

 	SET @RetNames = (SELECT  Name  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
 	
 	IF @RetNames <> ''
     	BEGIN
     		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
     	END
 		
 	RETURN @RetNames
    END
GO

CREATE FUNCTION [dbo].[GetDirectSupervisorByRequest]
   (	
     	@idEmployee int,
     	@TypeRequest int
   )
   RETURNS nvarchar(1000)
   AS
   BEGIN
     DECLARE @RetNames nvarchar(1000)
 	SET @RetNames = ''
 	DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(70))
 	insert into @tmpTable 
	select sysroPassports_Categories.IDPassport, sysroPassports_Categories.LevelOfAuthority, sysroPassports.Name  FROM sysroPassports_Categories inner join  sysroPassports on sysroPassports_Categories.IDPassport =  sysroPassports.id  WHERE IDCategory IN(select IDCategory FROM sysroRequestType where idType=@TypeRequest) AND  sysroPassports.Description not like '%@@ROBOTICS@@%' AND  sysroPassports.IsSupervisor = 1

 	SET @RetNames = (SELECT  Name  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
 	
 	IF @RetNames <> ''
     	BEGIN
     		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
     	END
 		
 	RETURN @RetNames
    END
GO

-- Obtenemos el nivel de aprobacion necesario para aprobar definitivamente una solicitud
CREATE FUNCTION [dbo].[GetApprovedLevelOfRequest]  
  (  
   @RequestType int,	  
   @IDCause int	  
  )  
  RETURNS int  
  AS  
  BEGIN  
   declare @levelOfAuthority int  
	, @pRequestType int = @RequestType,
	@pIDCause int = @IDCause
   ;

  if (@pRequestType = 7 or @pRequestType = 9 or @pRequestType = 13 or @pRequestType = 14 )
  	-- si es una solicitud de Prevision de ausencia o de exceso
	--  obtenemos el nivel de aprobacion de la justificacion
    select @levelOfAuthority = (select isnull(ApprovedAtLevel,1) FROM Causes where Id= @pIDCause);

  else
	-- si es cualquier otra solicitud
	--  el nivel para aprobar definitivamente siempre es 1
      select @levelOfAuthority = 1;

   
   IF @levelOfAuthority is null set @levelOfAuthority = 1  
   RETURN @levelOfAuthority  
END
GO

-- Obtenemos el nivel de aprobacion mínimo para empezar a gestionarla
CREATE FUNCTION [dbo].[GetMinLevelOfRequest]  
  (  
   @RequestType int,	  
   @IDCause int	  
  )  
  RETURNS int  
  AS  
  BEGIN  
   declare @levelOfAuthority int  
	, @pRequestType int = @RequestType,
	@pIDCause int = @IDCause
   ;

  if (@pRequestType = 7 or @pRequestType = 9 or @pRequestType = 13 or @pRequestType = 14 )
  	-- si es una solicitud de Prevision de ausencia o de exceso
	--  obtenemos el nivel de aprobacion de la justificacion
    select @levelOfAuthority = (select isnull(MinLevelOfAuthority,11) FROM Causes where Id= @pIDCause);

  else
	-- si es cualquier otra solicitud
	--  el nivel para empezar a gestionar siempre es 11
      select @levelOfAuthority = 11;

   
   IF @levelOfAuthority is null set @levelOfAuthority = 11  
   RETURN @levelOfAuthority  
END
GO

ALTER FUNCTION [dbo].[GetFeatureNextLevelPassportsIDsByEmployee]
(	
    @idEmployee int,
    @featureAlias nvarchar(100),
    @employeeFeatureId int,
	@NotificationType int,@Version int
)
RETURNS nvarchar(1000)
AS
BEGIN
DECLARE @RetNames nvarchar(1000),@pVersion int = @Version;
SET @RetNames = ''
DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))
	if (@pVersion = 3)
	-- si es seguridad v3
	begin
	insert into @tmpTable 
	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,@NotificationType,-2,3) AS LevelOfAuthority, sysroPassports.Name
     		FROM sysroPassports  
     		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
     				dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
  				dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'
	SET @RetNames = (SELECT  CONVERT(nvarchar(4000),PassportID ) + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
	IF @RetNames <> ''
		BEGIN
     		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
		END
	end
	if (@pVersion <> 3)
	-- si es seguridad v1 o v2
	begin
	insert into @tmpTable 
	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,0,0,1) AS LevelOfAuthority, sysroPassports.Name
    		FROM sysroPassports  
    		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
    			  dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
 			  dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'

	SET @RetNames = (SELECT  CONVERT(NVARCHAR(4000),PassportID ) + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))

	IF @RetNames <> ''
    	BEGIN
    		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
    	END
	end


RETURN @RetNames
END
GO

 ALTER FUNCTION [dbo].[GetFeatureNextLevelPassportsByEmployee]
   (	
     	@idEmployee int,
     	@featureAlias nvarchar(100),
     	@employeeFeatureId int
   )
   RETURNS nvarchar(1000)
   AS
   BEGIN
     DECLARE @RetNames nvarchar(1000)
 	SET @RetNames = ''
 	DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))
 	insert into @tmpTable 
 	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,0,0,1) AS LevelOfAuthority, sysroPassports.Name
     		FROM sysroPassports  
     		WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
     			  dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
  			  dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'
 	SET @RetNames = (SELECT  Name  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
 	
 	IF @RetNames <> ''
     	BEGIN
     		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
     	END
 		
 	RETURN @RetNames
    END
GO



ALTER FUNCTION [dbo].[GetSupervisedEmployeesByPassport]
     (
     	@idPassport int,
     	@featureAlias nvarchar(50),@Version int
     )
     RETURNS @result table (ID int PRIMARY KEY)
     AS
     	BEGIN
     		DECLARE @EmployeeID int
     		DECLARE @SupervisorLevel int
     		DECLARE @featureEmployeeID int
     		DECLARE @featurePermission int
     		DECLARE @parentPassport int
      		
    		DECLARE @GroupType nvarchar(50),@pVersion int = @Version;
     	
    		SELECT @GroupType = isnull(GroupType, '') FROM sysroPassports WHERE ID = @idPassport
      		
      		if @GroupType = 'U'
      		begin
      			SET @parentPassport = @idPassport
      		end
      		else
      		begin
      			SELECT @parentPassport = IDParentPassport FROM sysroPassports WHERE ID = @idPassport
      		end
     		 if (@pVersion = 3)
		   -- si es seguridad v3
		   begin
				SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport,45,-2,3))
				SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias and Type='U')
				SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
				
				IF @featurePermission > 3
					BEGIN
						INSERT INTO @result
							select Distinct IDEmployee from (
								select IDEmployee from sysrovwCurrentEmployeeGroups where IDGroup in (
									SELECT EmployeeGroupID from (select EmployeeGroupID, MAX(LevelOfAuthority) as MaxLevel, PassportID from [dbo].[sysroPermissionsOverGroups]
														where Permission > 3 and  EmployeeFeatureID  = @featureEmployeeID  group by EmployeeGroupID,PassportID) allPerm
									WHERE allPerm.PassportID = @parentPassport and allPerm.MaxLevel = @SupervisorLevel)) allEmployees
								WHERE IDEmployee not in (select EmployeeID from sysroPermissionsOverEmployeesExceptions where PassportID = @parentPassport and EmployeeFeatureID = @featureEmployeeID and Permission <= 3)
					END
			end
			
		if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin
		SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport,0,0,1))
    		SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias and Type='U')
    		SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
    		
    		IF @featurePermission > 3
    			BEGIN
    				INSERT INTO @result
   					select Distinct IDEmployee from (
   						select IDEmployee from sysrovwCurrentEmployeeGroups where IDGroup in (
   							SELECT EmployeeGroupID from (select EmployeeGroupID, MAX(LevelOfAuthority) as MaxLevel, PassportID from [dbo].[sysroPermissionsOverGroups]
    												where Permission > 3 and  EmployeeFeatureID  = @featureEmployeeID  group by EmployeeGroupID,PassportID) allPerm
   							WHERE allPerm.PassportID = @parentPassport and allPerm.MaxLevel = @SupervisorLevel)) allEmployees
   						WHERE IDEmployee not in (select EmployeeID from sysroPermissionsOverEmployeesExceptions where PassportID = @parentPassport and EmployeeFeatureID = @featureEmployeeID and Permission <= 3)
    			END
		
		end
			
     		RETURN
     	END
GO


ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
     (
   	@IDAction int,
     	@IDObject int,@Version int
     )
     AS
     /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
  	BEGIN
	DECLARE @pIDAction int = @IDAction,  
      @pIDObject int = @IDObject, @pVersion int = @Version; 

  		IF @pIDAction = -2
  		BEGIN
 			exec dbo.sysro_GenerateAllPermissionsOverGroups @pVersion
 			exec dbo.sysro_GenerateAllPermissionsOverFeatures
  			exec dbo.sysro_GenerateAllPermissionsOverEmployeesExceptions
 			exec dbo.GenerateAllRequestPassportPermission
  		END
 		IF @pIDAction = -1 -- Cambio de dia
  		BEGIN
 			exec dbo.GenerateChangeDayRequestPassportPermission
  		END
  		IF @pIDAction = 0 -- Creación passport
  		BEGIN
  			exec dbo.InsertPassportRequestsPermission @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
 			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
  		END
  		IF @pIDAction = 1 -- Modificación passport
  		BEGIN
  			exec dbo.AlterPassportRequestsPermission @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverGroups @pIDObject,@pVersion
 			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @pIDObject
 			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @pIDObject
  		END
  		IF @pIDAction = 2 -- Creación solicitud
  		BEGIN
  			exec dbo.InsertRequestPassportPermission @pIDObject
  		END
  		IF @pIDAction = 3 -- Creación grupo de empleados
  		BEGIN
  			exec dbo.InsertGroupPassportPermission @pIDObject,@pVersion
  		END
  	END	
go

ALTER FUNCTION [dbo].[GetFeatureNextLevelPassportsIDs]
   (	
     	@featureAlias nvarchar(100),@NotificationType int,@Version int   	
   )
   RETURNS nvarchar(1000)
   AS
   BEGIN
     	DECLARE @RetNames nvarchar(1000),@pVersion int = @Version;
     	SET @RetNames = ''
     	
 		DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))
 		
		if (@pVersion = 3)
		-- si es seguridad v3
		begin
		insert into @tmpTable 
 		SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,@NotificationType,-2,3) AS LevelOfAuthority, sysroPassports.Name
     			FROM sysroPassports  
     			WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null and
     			dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'
     	SET @RetNames = (SELECT  CONVERT(nvarchar(4000),PassportID )  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
 	
 		IF @RetNames <> ''
     		BEGIN
     			SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
     		END
		end
		
		
		if (@pVersion <> 3)
		-- si es seguridad v1 o v2
		begin

		insert into @tmpTable 
 		SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,0,0,1) AS LevelOfAuthority, sysroPassports.Name
     			FROM sysroPassports  
     			WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null and
     			dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3 AND Description not like '@@ROBOTICS@@%'
     	SET @RetNames = (SELECT  CONVERT(nvarchar(4000),PassportID )  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
 	
 		IF @RetNames <> ''
     		BEGIN
     			SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
     		END
		end
 		
 		RETURN @RetNames
   END
GO

IF not exists (select * from [dbo].sysroLiveAdvancedParameters where ParameterName like 'SecurityMode')
INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
     VALUES ('SecurityMode','1')
GO

UPDATE sysroParameters SET Data='492' WHERE ID='DBVersion'
GO




