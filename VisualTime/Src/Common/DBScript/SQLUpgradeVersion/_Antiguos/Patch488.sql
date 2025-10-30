INSERT INTO [dbo].[sysroFeatures] (ID, IDParent, Alias, Name, Description, Type, PermissionTypes, AppHasPermissionsOverEmployees, AliasID, EmployeeFeatureID) VALUES (1700,1, 'Employees.Communiques','Comunicados','','U','RW',NULL,NULL,1)
GO

INSERT INTO [dbo].[sysroGUI] ([IDPath] ,[LanguageReference] ,[URL] ,[IconURL] ,[Type] ,[Parameters] ,[RequiredFeatures] ,[SecurityFlags] ,[Priority] ,[AllowedSecurity] ,[RequiredFunctionalities]) 
     VALUES ('Portal\GeneralManagement\Communiques' ,'Communiques' ,'/Communique' ,'Communique.png' ,NULL ,NULL ,NULL ,NULL ,108 ,'NWR' ,'U:Employees.Communiques=Read' )
GO

ALTER VIEW [dbo].[sysrovwCommuniquesStatistics]
AS
	SELECT c.Id as IdCommunique, 
		   c.Subject, 
		   svc.IdEmployee, 
		   emp.Name, 
		   CASE WHEN ReadTimeStamp IS NULL THEN 0 ELSE 1 END as ReadStatus, 
		   ces.ReadTimeStamp, 
		   CASE WHEN LEN(ISNULL(c.AllowedResponses,'')) > 0 THEN 1 ELSE 0 END as RequiresResponse,
		   CASE WHEN ResponseTimeStamp IS NULL THEN 0 ELSE 1 END as ResponseStatus, 
		   ces.Response, 
		   ces.ResponseTimeStamp 
	FROM [dbo].[sysrovwCommuniquesEmployees] svc
	INNER JOIN [dbo].[Employees] emp ON emp.ID = svc.IdEmployee
	LEFT JOIN [dbo].[Communiques] c ON svc.IdCommunique = c.Id
	LEFT JOIN [dbo].[CommuniqueEmployeeStatus] ces ON ces.IdCommunique = svc.IdCommunique AND svc.IdEmployee = ces.IdEmployee
GO

ALTER TABLE [dbo].[Communiques] ADD [SentOn] [smalldatetime] NULL
GO

UPDATE dbo.sysroLiveAdvancedParameters SET Value = 10 WHERE ParameterName='VTPortalApiVersion'
GO

ALTER TABLE [dbo].[CommuniqueEmployeeStatus]
ALTER COLUMN [Response] [nvarchar](20);

UPDATE [dbo].[CommuniqueEmployeeStatus] SET Response = LTRIM(RTRIM(Response))
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='488' WHERE ID='DBVersion'
GO

