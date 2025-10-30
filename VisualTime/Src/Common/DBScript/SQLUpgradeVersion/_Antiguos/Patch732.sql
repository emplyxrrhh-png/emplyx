delete from sysroGUI where Parameters like '%Security%' and Parameters not like '%SecurityV3%' and URL like '%security%' 
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='732' WHERE ID='DBVersion'
GO
