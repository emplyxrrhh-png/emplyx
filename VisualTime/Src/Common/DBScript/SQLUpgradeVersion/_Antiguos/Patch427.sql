-- Aparece Chino en lista de idiomas (no instalado)
INSERT INTO dbo.[sysroLanguages] ([ID],[LanguageKey],[Culture],[Parameters]) VALUES (9, 'POR', 'pt-PT', '<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">fr</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>')
GO

UPDATE dbo.[sysroLanguages] SET ID = 8 WHERE ID = 4
GO

UPDATE dbo.[sysroLanguages] SET ID = 4 WHERE ID = 9
GO
 
UPDATE dbo.sysroParameters SET Data='427' WHERE ID='DBVersion'
GO
