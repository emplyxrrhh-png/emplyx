ALTER TABLE BotRules
ADD Definition VARCHAR(MAX);
GO
-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='872' WHERE ID='DBVersion'

GO
