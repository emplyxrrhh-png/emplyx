DECLARE @templateIndex AS INT 
SET @templateIndex = (SELECT MAX(ID) FROM ExportGuidesTemplates)

insert into ExportGuidesTemplates values (@templateIndex+1,20003,'schedulecompany',N'CalendarLinkCellCompanyLayout.xlsx',NULL,NULL,NULL)
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1058' WHERE ID='DBVersion'
GO
