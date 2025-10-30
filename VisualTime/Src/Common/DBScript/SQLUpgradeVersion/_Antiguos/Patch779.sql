delete from sysroLanguages where LanguageKey = 'CHN'
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='779' WHERE ID='DBVersion'
GO
