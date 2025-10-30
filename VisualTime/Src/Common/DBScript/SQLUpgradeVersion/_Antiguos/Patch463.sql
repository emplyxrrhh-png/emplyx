update sysroGUI_Actions set AppearsOnPopup=1 where id in (210)
GO

update dbo.ImportGuidesTemplates set Profile = 'tmplImportEmployeesBasic.xlsx' where ID = 1 and IDParentGuide = 20
GO

update dbo.ImportGuidesTemplates set Profile = 'tmpImportScheduleBasic.xlsx' where ID = 2 and IDParentGuide = 21
GO

update dbo.ExportGuidesTemplates set Profile = 'tmplExportEmployeesBasic.xlsx' where ID = 1 and IDParentGuide = 20001
GO

update dbo.ExportGuidesTemplates set Profile = 'tmplExportAccrualsBasic.xlsx' where ID = 2 and IDParentGuide = 20002
GO

update dbo.ExportGuidesTemplates set Profile = 'CalendarLinkCellLayout.xlsx' where ID = 3 and IDParentGuide = 20003
GO

update dbo.ExportGuidesTemplates set Profile = 'tmplExportPunchesBasic.xlsx' where ID = 4 and IDParentGuide = 20004
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='463' WHERE ID='DBVersion'
GO
