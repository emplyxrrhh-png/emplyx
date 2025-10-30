insert into sysroLanguages(ID,LanguageKey,Culture,Parameters) 
	values(9,'SLK','sk-sK','<?xml version="1.0"?><roCollection version="2.0"><Item key="ExtLanguage" type="8">sp</Item><Item key="ExtDatePickerFormat" type="8">d/m/Y</Item><Item key="ExtDatePickerStartDay" type="8">1</Item></roCollection>')
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='871' WHERE ID='DBVersion'

GO
