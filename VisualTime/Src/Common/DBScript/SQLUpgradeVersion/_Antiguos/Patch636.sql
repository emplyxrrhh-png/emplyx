--Update reports DX

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='636' WHERE ID='DBVersion'
GO
