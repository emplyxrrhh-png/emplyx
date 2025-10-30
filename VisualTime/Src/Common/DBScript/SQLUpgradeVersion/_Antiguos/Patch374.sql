UPDATE [dbo].[sysroGUI_Actions] SET [RequieredFeatures]='Forms\Concepts' where  [IDGUIPath] = 'Portal\ShiftManagement\Accruals\Accruals'
GO

UPDATE [dbo].[sysroGUI] SET [RequiredFeatures]=NULL  WHERE [IDPath]='Portal\CostControl\BusinessCenters'
GO
UPDATE [dbo].[sysroGUI] SET [RequiredFeatures]=NULL  WHERE [IDPath]='Portal\CostControl'
GO
UPDATE [dbo].[sysroGUI_Actions] SET [RequieredFeatures]=NULL  WHERE [IDGUIPath]='Portal\CostControl\BusinessCenters\Management'
GO
UPDATE [dbo].[sysroGUI_Actions] SET [RequieredFeatures]=NULL  WHERE [IDGUIPath]='Portal\GeneralManagement\AccessZones\management'
GO
UPDATE [dbo].[sysroGUI_Actions] SET [RequieredFeatures]=NULL  WHERE [IDGUIPath]='Portal\GeneralManagement\AccessZones\Resume'
GO
UPDATE [dbo].[sysroGUI_Actions] SET [RequieredFeatures]='Forms\Concepts' where  [IDGUIPath] = 'Portal\ShiftManagement\Accruals\Accruals'
GO


-- Importación de centros de coste
INSERT INTO [dbo].[ImportGuides]([ID],[Name],[Template],[Mode],[Type],[FormatFilePath],[SourceFilePath],[Separator],[CopySource],[LastLog],[RequieredFunctionalities], [FeatureAliasID])
     VALUES (13,'Carga de Centros de Coste',0,0,0,NULL,'','',0,'','BusinessCenters.DataLink.Imports.BusinessCenter','Employees')
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(12420,12400,'BusinessCenters.DataLink.Imports','Importaciones','','U','RWA',NULL,NULL,NULL)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 12420, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 12400
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(12421,12420,'BusinessCenters.DataLink.Imports.BusinessCenter','Importación de centros de coste','','U','RWA',NULL,NULL,NULL)
GO

INSERT INTO [dbo].[sysroPassports_PermissionsOverFeatures] SELECT [IDPassport], 12421, [Permission]  FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE [IDFeature] = 12420
GO

EXEC dbo.sysro_GenerateAllPermissionsOverFeatures
GO

ALTER TABLE [dbo].[BusinessCenters] ALTER COLUMN [Name] [nvarchar](max) not NULL
GO

-- Factor de exportación en Causes
ALTER TABLE [dbo].[Causes] add [Export]  NVARCHAR(15)
GO

ALTER TABLE [dbo].[Causes] ADD  CONSTRAINT [DF_Causes_Export]  DEFAULT (('0')) FOR [Export]
GO

UPDATE [dbo].[Causes] SET [Export]='0' 
GO

-- Procedure para el informe Fichajes detallado por empleado
IF EXISTS (select * from dbo.sysobjects where id = object_id(N'[dbo].[ExplodeDates]') and OBJECTPROPERTY(id, N'IsTableFunction') = 1) drop function [dbo].[ExplodeDates]
GO

CREATE FUNCTION [dbo].[ExplodeDates](@startdate datetime, @enddate datetime)
returns table as
return (
with 
 N0 as (SELECT 1 as n UNION ALL SELECT 1)
,N1 as (SELECT 1 as n FROM N0 t1, N0 t2)
,N2 as (SELECT 1 as n FROM N1 t1, N1 t2)
,N3 as (SELECT 1 as n FROM N2 t1, N2 t2)
,N4 as (SELECT 1 as n FROM N3 t1, N3 t2)
,N5 as (SELECT 1 as n FROM N4 t1, N4 t2)
,N6 as (SELECT 1 as n FROM N5 t1, N5 t2)
,nums as (SELECT ROW_NUMBER() OVER (ORDER BY (SELECT 1)) as num FROM N6)
SELECT DATEADD(day,num-1,@startdate) as Date
FROM nums
WHERE num <= DATEDIFF(day,@startdate,@enddate) + 1
);
GO


UPDATE dbo.sysroParameters SET Data='374' WHERE ID='DBVersion'
GO