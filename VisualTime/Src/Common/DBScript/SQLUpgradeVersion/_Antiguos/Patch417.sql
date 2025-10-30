UPDATE dbo.sysroLiveAdvancedParameters SET Value = 3 WHERE ParameterName='VTPortalApiVersion'
GO
delete DBO.sysroFeatures where ID = 30 or IDParent = 30
go

UPDATE dbo.sysroParameters SET Data='417' WHERE ID='DBVersion'
GO
