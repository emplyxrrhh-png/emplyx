-- No borréis esta línea
ALTER TABLE [dbo].[TMPReports_CriterioSaldos] ALTER COLUMN [date] smalldatetime
GO

ALTER TABLE [dbo].TMPReports_CriterioCausesIncidences ALTER COLUMN [date] smalldatetime
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1025' WHERE ID='DBVersion'
GO
