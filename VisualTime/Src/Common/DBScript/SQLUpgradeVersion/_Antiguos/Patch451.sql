DELETE FROM [dbo].[sysroReaderTemplates] WHERE [Type] = 'wx1'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroReaderTemplates] WHERE [Type] = 'mxV' and ID=1)
	INSERT INTO  [dbo].[sysroReaderTemplates]
           ([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
	VALUES ('mxV',1,1,'TA',1,'Fast','X','LocalServer,ServerLocal,Server,Local','1,0','1,0',0,'1,0',0,'Remote',1,0,'')	
GO

UPDATE [dbo].[Terminals] SET [Type]= 'mxV' WHERE [Type] = 'wx1'
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='451' WHERE ID='DBVersion'
GO

