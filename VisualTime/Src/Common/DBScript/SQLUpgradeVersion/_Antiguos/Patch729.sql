ALTER TABLE GeniusViews
ADD IdSystemView int 

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='729' WHERE ID='DBVersion'
GO
