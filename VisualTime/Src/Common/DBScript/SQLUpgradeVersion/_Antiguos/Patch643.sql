ALTER TABLE GeniusViews
ADD ContextMenuOptions NVARCHAR(MAX)

GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='643' WHERE ID='DBVersion'
GO
