-- Añadimos permisos de administración al grupo de usuarios consultores de robotics
if exists (select * from dbo.sysroPassports where Description = '@@ROBOTICS@@Consultores')
BEGIN

DECLARE @idGroup int,
		@securityId int,
		@securityOptionsID int

select @idGroup = ID from dbo.sysroPassports where Description = '@@ROBOTICS@@Consultores'
select @securityId = ID from dbo.sysroFeatures where Alias = 'Administration.Security'
select @securityOptionsID = ID from dbo.sysroFeatures where Alias = 'Administration.SecurityOptions'

update dbo.sysroPassports_PermissionsOverFeatures Set Permission = 9 where IDPassport = @idGroup and IDFeature = @securityId
update dbo.sysroPassports_PermissionsOverFeatures Set Permission = 6 where IDPassport = @idGroup and IDFeature = @securityOptionsID
END
GO

CREATE NONCLUSTERED INDEX [PX_PUNCHES_DETAIL]
ON [dbo].[Punches] ([IDEmployee],[ActualType],[ShiftDate])
INCLUDE ([ID],[DateTime])
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='319' WHERE ID='DBVersion'
GO

