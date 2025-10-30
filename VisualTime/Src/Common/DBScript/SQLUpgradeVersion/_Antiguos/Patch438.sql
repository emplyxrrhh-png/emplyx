EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

ALTER TABLE Groups ALTER COLUMN Name nvarchar(Max)
GO

UPDATE dbo.sysroParameters SET Data='438' WHERE ID='DBVersion'
GO

