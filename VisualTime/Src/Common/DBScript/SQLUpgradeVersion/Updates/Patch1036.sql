-- No borréis esta línea
update WebLinks set url = 'https://helpcenter.ila.cegid.com/es/visualtime/' where url = 'https://www.cegid.com/ib/asistencia-al-cliente/software-gestion-tiempo-visualtime/'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1036' WHERE ID='DBVersion'
GO
