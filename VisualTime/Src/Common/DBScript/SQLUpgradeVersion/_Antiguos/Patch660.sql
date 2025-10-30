
IF NOT EXISTS (SELECT 1 FROM [dbo].[ExportGuidesTemplates] WHERE Name = 'absencesTemplate')
	insert into ExportGuidesTemplates(ID,IDParentGuide,Name,Profile) values ((select MAX(id) + 1 from ExportGuidesTemplates),20007,'absencesTemplate','tmplExportAbsences.xlsx')
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='660' WHERE ID='DBVersion'
GO
