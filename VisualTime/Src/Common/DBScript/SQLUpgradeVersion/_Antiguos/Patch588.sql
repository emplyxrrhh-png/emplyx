ALTER PROCEDURE [dbo].[WebLogin_Passports_Insert] 
      	(
      		@id int OUTPUT, @idParentPassport int, @groupType varchar(1), @name nvarchar(50), @description nvarchar(100), @email nvarchar(100), @idUser int, @idEmployee int, @idLanguage int,
      		@levelOfAuthority tinyint, @ConfData text, @AuthenticationMerge nvarchar(50), @StartDate smalldatetime, @ExpirationDate smalldatetime, @State smallint, @EnabledVTDesktop bit,
    		@EnabledVTPortal bit, @EnabledVTSupervisor bit, @EnabledVTPortalApp bit , @EnabledVTSupervisorApp bit , @NeedValidationCode bit , @TimeStampValidationCode smalldatetime ,
    		@ValidationCode nvarchar(100), @EnabledVTVisits bit, @EnabledVTVisitsApp bit, @PhotoRequiered bit, @LocationRequiered bit, @LicenseAccepted bit, @IsSupervisor bit
      	)
      AS
	  
	DECLARE @Customization nvarchar(max)
	DECLARE @SecurityMode nvarchar(max)

	select @Customization = value FROM sysroLiveAdvancedParameters WHERE ParameterName = 'Customization'
	select @SecurityMode = value FROM sysroLiveAdvancedParameters WHERE ParameterName = 'SecurityMode'

	if @Customization = 'CRUZROJA'
	begin
      	SET @PhotoRequiered = 0
		SET @LocationRequiered = 0
	END

	INSERT INTO sysroPassports (
      	IDParentPassport, GroupType, Name, Description, Email, IDUser, IDEmployee, IDLanguage, LevelOfAuthority, ConfData, AuthenticationMerge, StartDate, ExpirationDate, [State],
     	EnabledVTDesktop, EnabledVTPortal, EnabledVTSupervisor, EnabledVTPortalApp, EnabledVTSupervisorApp, NeedValidationCode, TimeStampValidationCode, ValidationCode, 
 		EnabledVTVisits, EnabledVTVisitsApp, PhotoRequiered, LocationRequiered, LicenseAccepted, IsSupervisor
    )
    VALUES (
      	@idParentPassport, @groupType, @name, @description, @email, @idUser, @idEmployee, @idLanguage, @levelOfAuthority, @ConfData, @AuthenticationMerge, @StartDate, @ExpirationDate, @State, 
 		@EnabledVTDesktop, @EnabledVTPortal, @EnabledVTSupervisor, @EnabledVTPortalApp, @EnabledVTSupervisorApp, @NeedValidationCode, @TimeStampValidationCode, @ValidationCode, 
 		@EnabledVTVisits, @EnabledVTVisitsApp, @PhotoRequiered, @LocationRequiered, @LicenseAccepted, @IsSupervisor
    )
	SELECT @id = @@IDENTITY



   	IF @groupType = 'U' 
     	BEGIN
   		-- 0 / Actualizar los permisos del grupo y sus hijos
     		insert into RequestPassportPermissionsPending Values(0, @id)
     	END

		IF @idEmployee > 0 and @SecurityMode <> '1'
     	BEGIN
			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20,6) 
			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20000,3)
			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20001,6)
			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20100,6)
			INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20110,6)
			
			IF @Customization = 'CRUZROJA'
				BEGIN
				    INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,20120,6)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21,6) 
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21000,3)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21100,6)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21200,6)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21110,6)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21120,6)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21140,6)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21150,6)
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,21160,6)

					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,26,6) 
					INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport,IdFeature, Permission) values (@id,26001,3)
				END
		END
  RETURN
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO
UPDATE sysroParameters SET Data='588' WHERE ID='DBVersion'
GO
