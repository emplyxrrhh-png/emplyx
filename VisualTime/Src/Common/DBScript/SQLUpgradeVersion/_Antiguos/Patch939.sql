-- No borréis esta línea
INSERT INTO dbo.sysroLiveAdvancedParameters ([ParameterName],[Value]) VALUES ('BroadcasterTaskBatchSize','200')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='939' WHERE ID='DBVersion'
GO
