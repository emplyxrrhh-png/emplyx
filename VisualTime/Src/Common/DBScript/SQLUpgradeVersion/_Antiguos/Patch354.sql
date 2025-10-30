INSERT INTO dbo.sysroLiveAdvancedParameters([ParameterName],[Value]) VALUES ('MassPunchWizardAvailable','0')
GO

ALTER TABLE dbo.sysroDeletedPunchesSync ADD
	EmployeeID int NOT NULL CONSTRAINT DF_sysroDeletedPunchesSync_EmployeeID DEFAULT 0
GO

-- Comportamiento de comedor para terminales virtuales
INSERT INTO [dbo].[sysroEntryTypes]
           ([Type]
           ,[Description]
           ,[Context])
     VALUES
           ('D','Comedor','COMEDOR')
GO

INSERT INTO [dbo].[sysroReaderTemplates]
           ([Type]
           ,[IDReader]
           ,[ID]
           ,[ScopeMode]
           ,[UseDispKey]
           ,[InteractionMode]
           ,[InteractionAction]
           ,[ValidationMode]
           ,[EmployeesLimit]
           ,[OHP]
           ,[CustomButtons]
           ,[Output]
           ,[InvalidOutput]
           ,[Direction]
           ,[AllowAccessPeriods])
     VALUES
           ('Virtual',1,41,'DIN',1,'Blind','X','Local',0,0,0,0,0,'Remote',0)
GO


UPDATE dbo.sysroParameters SET Data='354' WHERE ID='DBVersion'
GO


