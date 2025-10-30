insert into [dbo].sysroLiveAdvancedParameters values ('Application.CustomLogLevel','-1')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='638' WHERE ID='DBVersion'
GO
