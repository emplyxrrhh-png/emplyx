insert into [dbo].sysroGUI_Actions(IDPath,IDGUIPath,LanguageTag,RequieredFeatures,RequieredFunctionalities,AfterFunction,CssClass,Section,ElementIndex,AppearsOnPopup)
values
		('NewBot','Portal\General\Bots','tbAddNewBot',NULL,NULL,'addNewBot()','btnAddNewBot2',0,1,0),
		('DeleteBot','Portal\General\Bots','tbDeleteBot',NULL,NULL,'deleteCurrentBot()','btnTbDel2',0,2,0)
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='728' WHERE ID='DBVersion'
GO
