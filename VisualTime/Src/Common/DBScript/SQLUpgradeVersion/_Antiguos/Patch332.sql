-- Creación de la tabla de tareas en background de live
CREATE TABLE [dbo].[sysroLiveTasks](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[IDPassport] [int] NOT NULL,
	[Action] [nvarchar](max) NOT NULL,
	[Parameters] [nvarchar](max) NOT NULL,
	[Status] [int] NOT NULL,
	[ErrorCode] [nvarchar](max) NULL,
	[TimeStamp] [datetime] NULL,
	[ExecutionDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[Progress] [int] NULL,
 CONSTRAINT [PK_sysroLiveTasks] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[sysroLiveTasks] ADD  CONSTRAINT [DF_sysroLiveTasks_Status]  DEFAULT ((0)) FOR [Status]
GO
-- Fin creación de la tabla de tareas en background de live


-- Procedimiento para crear los permisos al crear un grupo de empleados nuevo
CREATE PROCEDURE [dbo].[InsertGroupPassportPermission]
	(
		@idGroup int
	)
    AS
    BEGIN
  	
	DELETE FROM sysroPermissionsOverGroups WHERE EmployeeGroupID = @idGroup

	DECLARE @featureEmployeeID int
 	DECLARE EmployeeFeatureCursor CURSOR
 	FOR 
 		SELECT distinct EmployeeFeatureID from sysroFeatures where employeefeatureid is not null
 	OPEN EmployeeFeatureCursor
 	FETCH NEXT FROM EmployeeFeatureCursor INTO @featureEmployeeID
 	WHILE @@FETCH_STATUS = 0
 	BEGIN	
 		
 		INSERT INTO [dbo].[sysroPermissionsOverGroups] (PassportID, EmployeeGroupID, EmployeeFeatureID, LevelOfAuthority, Permission) 
 				select supPassports.ID, Groups.ID, @featureEmployeeID, dbo.GetPassportLevelOfAuthority(supPassports.ID),
 						dbo.sysro_GetPermissionOverGroup(supPassports.ID,Groups.ID,@featureEmployeeID,0)
 				from sysroPassports supPassports, Groups
 				where supPassports.GroupType = 'U' AND Groups.ID = @idGroup
 		
 		FETCH NEXT FROM EmployeeFeatureCursor 
 		INTO @featureEmployeeID
 	END 
 	CLOSE EmployeeFeatureCursor
 	DEALLOCATE EmployeeFeatureCursor
	    	
    END

GO


-- Funcion encargada de lanzar todas las opciones de recalculo de permisos
ALTER PROCEDURE [dbo].[ExecuteRequestPassportPermissionsAction]
    (
  	@IDAction int,
    	@IDObject int
    )
    AS
    /* Añadimos todos los pasaportes que tienen permisos para una solicitud a la tabla temporal */
 	BEGIN
 		IF @IDAction = -2
 		BEGIN
			exec dbo.sysro_GenerateAllPermissionsOverGroups
			exec dbo.sysro_GenerateAllPermissionsOverFeatures
 			exec dbo.sysro_GenerateAllPermissionsOverEmployeesExceptions
			exec dbo.GenerateAllRequestPassportPermission
 		END
		IF @IDAction = -1 -- Cambio de dia
 		BEGIN
			exec dbo.GenerateChangeDayRequestPassportPermission
 		END
 		IF @IDAction = 0 -- Creación passport
 		BEGIN
 			exec dbo.InsertPassportRequestsPermission @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverGroups @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @IDObject
 		END
 		IF @IDAction = 1 -- Modificación passport
 		BEGIN
 			exec dbo.AlterPassportRequestsPermission @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverGroups @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverFeatures @IDObject
			exec dbo.sysro_UpdatePassportPermissionsOverEmployeesExceptions @IDObject
 		END
 		IF @IDAction = 2 -- Creación solicitud
 		BEGIN
 			exec dbo.InsertRequestPassportPermission @IDObject
 		END
 		IF @IDAction = 3 -- Creación grupo de empleados
 		BEGIN
 			exec dbo.InsertGroupPassportPermission @IDObject
 		END

 	END

GO



 
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='332' WHERE ID='DBVersion'
GO


