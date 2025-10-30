ALTER FUNCTION [dbo].[GetDirectSupervisorByRequest]
(	
    @idEmployee int,
    @featureAlias nvarchar(100),
    @employeeFeatureId int,
	@RequestType int
)
RETURNS nvarchar(1000)
AS
BEGIN
DECLARE @RetNames nvarchar(1000);
SET @RetNames = ''
DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))
	-- si es seguridad v3
	begin
	insert into @tmpTable 
	SELECT sysroPassports.id AS PassportID, dbo.GetPassportLevelOfAuthority(sysroPassports.ID,@RequestType,0,3) AS LevelOfAuthority, sysroPassports.Name
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

RETURN @RetNames
END
GO

UPDATE sysroGUI SET RequiredFunctionalities = 'U:Reports=Read' WHERE IDPath = 'Portal\GeneralManagement\AdvReport'
GO

ALTER TABLE CommuniqueEmployeeStatus ALTER COLUMN Response NVARCHAR(50)
GO

UPDATE sysroParameters SET Data='500' WHERE ID='DBVersion'
GO

