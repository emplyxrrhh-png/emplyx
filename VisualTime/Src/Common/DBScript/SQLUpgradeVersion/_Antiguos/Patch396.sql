INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('EmployeeMultiMobility','Portal\General\Employees\Employees','tbEmployeeMultiMobility','Forms\Employees','U:Employees.GroupMobility=Write','ShowMultiMobilityEmployee(''e'')','btnMultiMobilityEmployee',1,3)
GO

INSERT INTO [dbo].[sysroGUI_Actions]
           ([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
     VALUES
           ('EmployeeMultiMobility','Portal\General\Employees\Groups','tbEmployeeMultiMobility','Forms\Employees','U:Employees.GroupMobility=Write','ShowMultiMobilityEmployee(''e'')','btnMultiMobilityEmployee',1,5)
GO

UPDATE dbo.sysroParameters SET Data='396' WHERE ID='DBVersion'
GO
