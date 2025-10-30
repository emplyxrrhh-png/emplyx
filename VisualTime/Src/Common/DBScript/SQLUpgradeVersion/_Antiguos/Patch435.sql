--Sirenas para terminales rxCePUSH y rxCPUSH
UPDATE [dbo].[sysroReaderTemplates] set SupportedSirens = 1 where UPPER(type) in ('RXCP','RXCEP')
GO

UPDATE dbo.sysroParameters SET Data='435' WHERE ID='DBVersion'
GO
