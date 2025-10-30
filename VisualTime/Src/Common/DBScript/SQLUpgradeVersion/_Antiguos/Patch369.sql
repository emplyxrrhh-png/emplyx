update [dbo].[sysroGUI] set RequiredFunctionalities ='U:LabAgree.Definition=Read OR U:LabAgree.AccrualRules=Read OR U:LabAgree.StartupValues=Read' where IDPath = 'Portal\LabAgree'
GO

UPDATE dbo.sysroParameters SET Data='369' WHERE ID='DBVersion'
GO