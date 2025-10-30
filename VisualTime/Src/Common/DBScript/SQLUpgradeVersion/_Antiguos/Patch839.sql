-- No borréis esta línea
CREATE NONCLUSTERED INDEX [IX_DocumentTemplates_Status]
ON [dbo].[Documents] ([IdDocumentTemplate],[IdEmployee],[Status])
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='839' WHERE ID='DBVersion'

GO
