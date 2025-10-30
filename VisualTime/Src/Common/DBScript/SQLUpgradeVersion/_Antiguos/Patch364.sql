INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
     VALUES ('Virtual',1,42,'TA',1,'Blind','E','Local',0,0,0,0,0,'Remote',0,NULL,'')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
     VALUES ('Virtual',1,43,'TA',1,'Blind','S','Local',0,0,0,0,0,'Remote',0,NULL,'')
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
	 VALUES ('Customization','')
 GO
 
 INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value])
	 VALUES ('CustomizationLogEnabled','0')
 GO

UPDATE dbo.sysroParameters SET Data='364' WHERE ID='DBVersion'
GO