alter table tmpdetailedCalendarEmployee alter column EmployeeName varchar(250)
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters] Values ('SSOType', 'ADFS')
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='485' WHERE ID='DBVersion'
GO

