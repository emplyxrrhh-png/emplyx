ALTER TABLE sysRopassports DROP column CubeLayout
GO

ALTER TABLE sysRopassports ADD DataUser NVARCHAR(MAX)
GO

ALTER TABLE Shifts ADD VisibilityPermissions Smallint
GO

ALTER TABLE Shifts ADD VisibilityCriteria NVARCHAR(MAX)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='304' WHERE ID='DBVersion'
GO
