INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,75,'EIP',1,'Interactive','X','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

UPDATE sysroParameters SET Data='350' WHERE ID='DBVersion'
GO


