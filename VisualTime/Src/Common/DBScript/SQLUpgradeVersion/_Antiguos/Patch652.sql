-- No borréis esta línea
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE [ParameterName] = 'VTPortal.ShowPermissionsIcon')
insert into sysroLiveAdvancedParameters values ('VTPortal.ShowPermissionsIcon', 'True')
GO
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='652' WHERE ID='DBVersion'
GO
