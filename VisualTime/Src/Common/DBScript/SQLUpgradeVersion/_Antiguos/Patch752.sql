update DocumentTemplates set Compulsory = 0 where LeaveDocType = 1

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='752' WHERE ID='DBVersion'
GO
