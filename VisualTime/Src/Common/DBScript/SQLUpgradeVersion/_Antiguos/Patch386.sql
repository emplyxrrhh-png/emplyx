CREATE TABLE [dbo].[sysroGroupFeatures](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	
 CONSTRAINT [PK_sysroGroupFeatures] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 

GO

CREATE TABLE [dbo].[sysroGroupFeatures_PermissionsOverFeatures](
	[IDGroupFeature] [int] NOT NULL,
	[IDFeature] [int] NOT NULL,
	[Permision] [tinyint] NOT NULL,
 CONSTRAINT [PK_sysroGroupFeatures_PermissionsOverFeatures] PRIMARY KEY CLUSTERED 
(
	[IDGroupFeature] ASC,
	[IDFeature] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[sysroSecurityNodes](
	[ID] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[IDParent] [int],
[Path] [nvarchar](max) NOT NULL,
CONSTRAINT [PK_sysroSecurityNodes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY] 
GO

CREATE TABLE [dbo].[sysroSecurityNode_Groups](
	[IDSecurityNode] [int] NOT NULL,
	[IDGroup] [int] NOT NULL,
 CONSTRAINT [PK_sysroSecurityNode_Groups] PRIMARY KEY CLUSTERED 
(
	[IDSecurityNode] ASC,
	[IDGroup] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO


CREATE TABLE [dbo].[sysroSecurityNode_Passports](
	[IDSecurityNode] [int] NOT NULL,
	[IDPassport] [int] NOT NULL,
	[IDGroupFeature] [int] NOT NULL,
	[LevelOfAuthority] [tinyint] NULL,
 CONSTRAINT [PK_sysroSecurityNode_Passports] PRIMARY KEY CLUSTERED 
(
	[IDSecurityNode] ASC,
	[IDPassport] ASC,
	[IDGroupFeature] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[sysroSecurityNode_Passports_PermissionsOverEmployeesExceptions](
	[IDSecurityNode] [int] NOT NULL,
	[IDPassport] [int] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[IDApplication] [int] NOT NULL,
	[Permision] [tinyint] NOT NULL,
 CONSTRAINT [PK_sysroSecurityNode_Passports_PermissionsOverEmployeesExceptions] PRIMARY KEY CLUSTERED 
(
	[IDSecurityNode] ASC,
	[IDPassport] ASC,
	[IDEmployee] ASC,
	[IDApplication] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

INSERT INTO dbo.sysroGroupFeatures (ID,Name) Values (0,'Administradores')
GO

INSERT INTO sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature, IDFeature, Permision)
SELECT 0, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes LIKE '%A%' THEN 9 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%W%' THEN 6 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%R%' THEN 3 ELSE 0 END END END
FROM sysroFeatures WHERE Type = 'U'
GO

INSERT INTO dbo.sysroGroupFeatures (ID,Name) Values (1,'@@ROBOTICS@@Consultores')
GO

INSERT INTO sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature, IDFeature, Permision)
SELECT 1, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes LIKE '%A%' THEN 9 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%W%' THEN 6 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%R%' THEN 3 ELSE 0 END END END
FROM sysroFeatures WHERE Type = 'U'
GO

INSERT INTO dbo.sysroGroupFeatures (ID,Name) Values (2,'@@ROBOTICS@@Tecnicos')
GO

INSERT INTO sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature, IDFeature, Permision)
SELECT 2, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes LIKE '%A%' THEN 9 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%W%' THEN 6 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%R%' THEN 3 ELSE 0 END END END
FROM sysroFeatures WHERE Type = 'U' AND Alias = 'Access' or Alias LIKE 'Access.%' OR Alias LIKE 'Employees.%' OR Alias = 'Employees' OR Alias LIKE 'Terminals.%'  OR Alias = 'Terminals'  
GO

INSERT INTO sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature, IDFeature, Permision)
SELECT 2, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes LIKE '%A%' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%W%' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%R%' THEN 3 ELSE 0 END END END
FROM sysroFeatures WHERE Type = 'U' AND  Alias <> 'Access' and Alias not LIKE 'Access.%' and Alias not LIKE 'Employees.%' and Alias <> 'Employees' and Alias not  LIKE 'Terminals.%'  and Alias <> 'Terminals'  
GO

INSERT INTO dbo.sysroGroupFeatures (ID,Name) Values (3,'@@ROBOTICS@@Soporte')
GO

INSERT INTO sysroGroupFeatures_PermissionsOverFeatures (IDGroupFeature, IDFeature, Permision)
SELECT 3, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes LIKE '%A%' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%W%' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes LIKE '%R%' THEN 3 ELSE 0 END END END
FROM sysroFeatures WHERE Type = 'U' 
GO

INSERT INTO sysroSecurityNodes (ID, Name, IDParent, Path ) VALUES (1,'Empresa', null, '1')
GO

INSERT INTO sysroSecurityNode_Passports (IDSecurityNode,IDPassport,IDGroupFeature,LevelOfAuthority) 
SELECT 1,id,0,LevelOfAuthority  FROM sysroPassports
WHERE IDParentPassport IN (SELECT ID FROM sysroPassports where ID = 3) and GroupType <> 'U'
GO

INSERT INTO sysroSecurityNode_Passports (IDSecurityNode,IDPassport,IDGroupFeature,LevelOfAuthority) 
SELECT 1,id,1,LevelOfAuthority  FROM sysroPassports
WHERE IDParentPassport IN (SELECT ID FROM sysroPassports where IDParentPassport  is null and Description like '%@@ROBOTICS@@Consultores%') 
GO

INSERT INTO sysroSecurityNode_Passports (IDSecurityNode,IDPassport,IDGroupFeature,LevelOfAuthority) 
SELECT 1,id,2,LevelOfAuthority  FROM sysroPassports
WHERE IDParentPassport IN (SELECT ID FROM sysroPassports where IDParentPassport  is null and Description like '%@@ROBOTICS@@Tecnicos%')
GO

INSERT INTO sysroSecurityNode_Passports (IDSecurityNode,IDPassport,IDGroupFeature,LevelOfAuthority) 
SELECT 1,id,3,LevelOfAuthority  FROM sysroPassports
WHERE IDParentPassport IN (SELECT ID FROM sysroPassports where IDParentPassport  is null and 
Description like '%@@ROBOTICS@@Soporte%')
GO

--INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
--     VALUES ('SecurityMode','2')
--GO

ALTER TABLE dbo.sysroPassports ADD
	IsSupervisor bit NULL
GO

UPDATE dbo.sysroPassports SET IsSupervisor=1 WHERE GroupType =''
GO

UPDATE dbo.sysroPassports SET IsSupervisor=0 WHERE GroupType = 'U'
GO

UPDATE dbo.sysroPassports SET IsSupervisor=0 WHERE GroupType = 'E'
GO


CREATE FUNCTION [dbo].[GetGroupParent]
(
	 @fullGroupPath nvarchar(max)
)
RETURNS integer
AS
BEGIN
	DECLARE @tmpPath nvarchar(max)
	DECLARE @position integer
	
	set @position = CHARINDEX('\',REVERSE(@fullGroupPath),0)
	
	if @position > 0 
	BEGIN
		set @tmpPath = SUBSTRING(@fullGroupPath,0, LEN(@fullGroupPath) - CHARINDEX('\',REVERSE(@fullGroupPath),0) + 1)
		set @position = CHARINDEX('\',REVERSE(@tmpPath),0)

		if @position > 0 
		BEGIN
			set @tmpPath = REPLACE(@tmpPath, SUBSTRING(@tmpPath,0, LEN(@tmpPath) - CHARINDEX('\',REVERSE(@tmpPath),0) + 2) ,'')
		END
	END
	ELSE
	BEGIN
		set @tmpPath = '0'
	END
	
	return convert(integer,@tmpPath)
END
GO

CREATE FUNCTION [dbo].[GetFullSecurityNodePathName] (@securityNodeID int)
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
  	select @path=Path from sysroSecurityNodes where ID=str(@securityNodeID)
  	--RETURN 'El path es: '+@path
  	-- Miramos la longitud total del Path
  	select @longitud = len(@path)
  	--RETURN 'La longitud es: '+str(@longitud)
  	-- Controlamos el primer valor del grupo
  	while (@contador=0)
  	begin
 		--Recogemos el grupo padre
 		select @index = CHARINDEX(@delimiter,@path)	
 		IF (@index != 0)
  			select @id=left(@path,@index-1)
 		ELSE
 			select @id=@path
  		select @contador=@contador+1
  		select @nombre=Name from sysroSecurityNodes where ID=@id
  		select @resultado=@nombre
  	end
 	set @index=1
  	-- Controlamos que hay alguna \ en el path
  	while (@index!=0)
  	begin
  		-- Buscamos la posicion de la \
  		select @index = CHARINDEX(@delimiter,@path,@i)
 		--RETURN 'Posición de la primera barra: '+str(@index)
  		-- Guardamos la localizacion de la primera \
  		select @anterior = @index
  		-- Si encuentra la \
  		if (@index!=0) 
  		begin
  			-- Colocamos el cursor en la siguiente posicion		
  			select @i=@index+1
  			-- Volvemos a buscar \ por si es la ultima
  			select @index = CHARINDEX(@delimiter,@path,@i)
  			--RETURN 'Posicion de la \: '+str(@index)
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
  					select @nombre=Name from sysroSecurityNodes where ID=@id
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
  					select @nombre=Name from sysroSecurityNodes where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
  				-- Si el id del grupo esta formado por tres digitos
  				if (@digitos=3)
  				begin
  					select @i=@i+2
  					select @id=right(left(@path,@i),3)
  					select @nombre=Name from sysroSecurityNodes where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
 				-- Si el id del grupo esta formado por cuatro digitos
 				if (@digitos=4)
  				begin
  					select @i=@i+3
  					select @id=right(left(@path,@i),4)
  					select @nombre=Name from sysroSecurityNodes where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
  			end
  			-- Sino no encuentra \
  			else
  			begin
  				select @digitos=@longitud-@anterior
 				--RETURN 'Digitos: '+str(@digitos)
  				-- Si el id del grupo esta formado por un digito
  				if (@digitos=1)
  				begin
 					-- Movemos el puntero para poder recortar el id del grupo
 					select @i=@i+2
 					select @id=right(left(@path,@i),1)
 					select @nombre=Name from sysroSecurityNodes where ID=@id
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
  				end
  				-- Si el id del grupo esta formado por dos digitos
  				if (@digitos=2)
  				begin
  					select @i=@i+3
  					select @id=right(left(@path,@i),2)
  					select @nombre=Name from sysroSecurityNodes where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
 					--RETURN 'resultado: '+@resultado
  				end
  				-- Si el id del grupo esta formado por tres digitos
  				if (@digitos=3)
  				begin
 					select @i=@i+2
 					select @id=right(left(@path,@i),3)
 					select @nombre=Name from sysroSecurityNodes where ID=@id
 					-- Concatenamos el valor al resultado final
 					select @resultado=@resultado+' \ '+@nombre
 					return @resultado
  				--return 'El nombre es:'+@resultado
  				end
 				-- Si el id del grupo esta formado por cuatro digitos
 				if (@digitos=4)
  				begin
  					select @i=@i+3
  					select @id=right(left(@path,@i),4)
  					select @nombre=Name from sysroSecurityNodes where ID=@id
  					-- Concatenamos el valor al resultado final
  					select @resultado=@resultado+' \ '+@nombre
  				end
  			end
  		end
  	end
  	return (@resultado)
 END
GO


ALTER TABLE dbo.sysroGroupFeatures ADD
	Description nvarchar(MAX) NULL
GO

UPDATE [dbo].[sysroGUI] SET [Parameters] = 'SingleSecurity'
     WHERE [IDPath] = 'Portal\Security\Passports'
GO
   
INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\Security\SecurityChart','Gui.SecurityChart','SecurityChart/SecurityChart.aspx','SecurityChart.png',NULL,'AdvancedSecurity','Forms\Passports',NULL,'102',NULL,'U:Administration.Security=Read')
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('Edit','Portal\Security\SecurityChart\Management','tbEditMode','Forms\Passports','U:Administration.Security=Write','EditSelectedNode()','edit.png',0,2)
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('Reload','Portal\Security\SecurityChart\Management','tbRefresh','Forms\Passports','U:Administration.Security=Read','ReloadSecurityChartView()','refresh.png',0,3)
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('ZoomOut','Portal\Security\SecurityChart\Management','tbZoomOut','Forms\Passports','U:Administration.Security=Read','changeZoomLevel(-1)','zoomout.png',0,4)
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('ZoomIn','Portal\Security\SecurityChart\Management','tbZoomIn','Forms\Passports','U:Administration.Security=Read','changeZoomLevel(1)','zoomin.png',0,5)
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\Security\SecurityFunctions','Gui.SecurityFunctions','SecurityChart/SecurityFunctions.aspx','SecurityFunctions.png',NULL,'AdvancedSecurity','Forms\Passports',NULL,'103',NULL,'U:Administration.Security=Read')
GO

update [dbo].[sysroGUI] set Priority = '105' where IDpath = 'Portal\Security\Audit'
GO

update [dbo].[sysroGUI] set Priority = '106' where IDpath = 'Portal\Security\License'
GO

update [dbo].[sysroGUI] set Priority = '107' where IDpath = 'Portal\Security\Aministration'
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('MaxMinimize','Portal\Security\SecurityChart\SecurityFunctions','tbMaximize','Forms\Passports',NULL,'MaxMinimize','btnMaximize2',0,1)
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('New','Portal\Security\SecurityChart\SecurityFunctions','tbAddNewFunction','Version\Live','U:Administration.Security=Read','newSecurityFunction()','btnTbAdd2',0,2)
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('Del','Portal\Security\SecurityChart\SecurityFunctions','tbDelFunction','Version\Live','U:Administration.Security=Read','ShowRemoveSecurityFunction(''#ID#'')','btnTbDel2',0,3)
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES
           ('Portal\Security\supervisors','Gui.Supervisors','SecurityChart/Supervisors.aspx','Supervisors.png',NULL,'AdvancedSecurity','Forms\Passports',NULL,'104',NULL,'U:Administration.Security=Read')
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('MaxMinimize','Portal\Security\SecurityChart\Supervisors','tbMaximize','Forms\Passports',NULL,'MaxMinimize','btnMaximize2',0,1)
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('New','Portal\Security\SecurityChart\Supervisors','tbAddNewPassport','Forms\Passports','U:Administration.Security=Admin','ShowNewPassportWizard(''#ID#'')','btnTbAdd2',0,2)
GO

INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('Delete','Portal\Security\SecurityChart\Supervisors','tbDelPassport','Forms\Passports','U:Administration.Security=Admin','ShowRemovePassport(''#ID#'')','btnTbDel2',0,3)
GO

	
UPDATE dbo.sysroParameters SET Data='386' WHERE ID='DBVersion'
GO
