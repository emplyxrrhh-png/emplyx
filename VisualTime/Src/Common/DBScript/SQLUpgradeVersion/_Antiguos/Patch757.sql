IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'MinimumCalendarDataWarning')
	insert into dbo.sysroLiveAdvancedParameters (ParameterName,Value) values ('MinimumCalendarDataWarning',10000)
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='757' WHERE ID='DBVersion'
GO
