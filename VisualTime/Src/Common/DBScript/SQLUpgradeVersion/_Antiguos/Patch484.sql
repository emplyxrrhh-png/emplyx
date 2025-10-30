ALTER TABLE [dbo].[ReportPlannedExecutions] ADD  CONSTRAINT [DF_ReportPlannedExecutions_Destination]  DEFAULT ('{}') FOR [Destination]
GO

ALTER TABLE [dbo].[ReportLayouts]
ADD [IsEmergencyReport] BIT DEFAULT 0 NOT NULL
GO

UPDATE sysroLiveAdvancedParameters SET Value='9' where ParameterName='VTPortalApiVersion'
GO


-- Actualizar el password de los supervisores de sistema con el password en MD5
UPDATE sysroPassports_AuthenticationMethods Set Password = '6644d029a64f6cb79546c77330ee4e98' WHERE Credential like 'Consultor'
GO
UPDATE sysroPassports_AuthenticationMethods Set Password = 'c5c758ecc7a27e13802ebb9b0306dd55' WHERE Credential like 'Soporte'
GO
UPDATE sysroPassports_AuthenticationMethods Set Password = '5b9efffdb7babcce7fdc898581de87a3' WHERE Credential like 'Tecnico'
GO

-- Modos de validación para accesos
UPDATE dbo.sysroReaderTemplates SET ValidationMode = 'LocalServer,ServerLocal,Local' WHERE Type = 'mx9' and ScopeMode like '%ACC%'
GO

-- Actualiza versión de la base de datos

UPDATE sysroParameters SET Data='484' WHERE ID='DBVersion'
GO

