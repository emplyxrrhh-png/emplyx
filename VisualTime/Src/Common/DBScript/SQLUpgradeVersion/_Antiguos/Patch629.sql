DECLARE @templateIndex AS INT 
SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)

insert into ExportGuidesTemplates values (@templateIndex+1,20003,'holidays','',NULL,NULL,NULL)


insert into ExportGuidesTemplates values (@templateIndex+2,20003,'presence','',NULL,NULL,NULL)

GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='629' WHERE ID='DBVersion'
GO
