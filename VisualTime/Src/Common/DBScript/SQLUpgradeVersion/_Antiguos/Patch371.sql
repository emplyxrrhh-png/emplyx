update [dbo].sysroReaderTemplates set ValidationMode = 'ServerLocal,Local,LocalServer' where Type = 'mxART'
GO


UPDATE dbo.sysroParameters SET Data='371' WHERE ID='DBVersion'
GO