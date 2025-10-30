
ALTER VIEW [dbo].[sysrovwCommuniquesEmployees]
 AS
      SELECT IdCommunique, IdEmployee FROM [dbo].[CommuniqueEmployees]
     LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueEmployees.IdCommunique
      UNION
      SELECT IdCommunique, sysrovwCurrentEmployeeGroups.IdEmployee FROM [dbo].[CommuniqueGroups]
     LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueGroups.IdCommunique
      INNER JOIN [dbo].[Groups] on CommuniqueGroups.idgroup = Groups.id
      INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] on (sysrovwCurrentEmployeeGroups.Path = Groups.Path or sysrovwCurrentEmployeeGroups.Path like Groups.Path + '\%' ) and sysrovwCurrentEmployeeGroups.CurrentEmployee = 1
     INNER JOIN [dbo].[sysroPassports] ON Communiques.IdCreatedBy = sysroPassports.Id
     INNER JOIN [dbo].[sysroPermissionsOverGroups] Perm ON Perm.EmployeeGroupID = CommuniqueGroups.IdGroup AND Perm.EmployeeFeatureID = 1 AND Perm.PassportID = sysroPassports.IDParentPassport AND Perm.Permission >= 3
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.ProgrammedAbsences.JustifyUntilExpectedHours')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.ProgrammedAbsences.JustifyUntilExpectedHours','1')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VTLive.DisableCRReports')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('VTLive.DisableCRReports','0')
GO


IF exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'CTaimaCenterId')
	DELETE FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'CTaimaCenterId'
GO

update dbo.sysroGUI set RequiredFeatures = '' where IDPath = 'Portal\GeneralManagement\AdvReport'
GO

UPDATE sysroParameters SET Data='499' WHERE ID='DBVersion'
GO

