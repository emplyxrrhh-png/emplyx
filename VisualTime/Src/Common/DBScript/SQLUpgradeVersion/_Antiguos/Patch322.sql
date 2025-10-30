Update sysroGUI Set RequiredFeatures = 'Forms\DataLink', RequiredFunctionalities = 'U:Employees=Read' Where IDPath = 'Portal\General\DataLink'
GO
Update sysroGUI Set RequiredFeatures = 'Forms\Employees', RequiredFunctionalities = NULL Where IDPath = 'Portal\General'
GO

   
UPDATE sysrofeatures SET PermissionTypes = 'RWA' WHERE Alias = 'Terminals.StatusInfo'
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='322' WHERE ID='DBVersion'
GO

