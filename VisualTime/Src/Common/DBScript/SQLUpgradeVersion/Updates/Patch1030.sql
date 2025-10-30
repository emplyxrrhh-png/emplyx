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
           ,[AllowAccessPeriods]
           ,[SupportedSirens]
           ,[RequiredFeatures]
           ,[Partners])
     VALUES
           ('Time Gate'
           ,1
           ,218
           ,'CO'
           ,1
           ,'Fast'
           ,'X'
           ,'Server'
           ,'1,0'
           ,'1,0'
           ,'0'
           ,'1,0'
           ,'0'
           ,NULL
           ,1
           ,1
           ,''
           ,NULL)
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1030' WHERE ID='DBVersion'
GO