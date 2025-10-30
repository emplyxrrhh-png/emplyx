INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex])
	VALUES
		('Search','Portal\Security\SecurityChart\Management','tbSearch','Forms\Passports','U:Administration.Security=Read','showSearchDialog()','search.png',0,6)
GO
 
UPDATE dbo.sysroParameters SET Data='388' WHERE ID='DBVersion'
GO
