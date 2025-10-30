UPDATE sysroLiveAdvancedParameters SET Value = '19' WHERE ParameterName='VTPortalApiVersion'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='641' WHERE ID='DBVersion'
GO
