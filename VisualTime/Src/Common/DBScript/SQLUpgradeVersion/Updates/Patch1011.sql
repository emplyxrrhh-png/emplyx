ALTER TABLE [dbo].[TMPReports_CriterioSaldos] ADD Concept4 numeric(19,6) NULL
GO

ALTER TABLE [dbo].[TMPReports_CriterioSaldos] ADD Concept5 numeric(19,6) NULL
GO

ALTER TABLE [dbo].[TMPReports_CriterioSaldos] ADD Concept6 numeric(19,6) NULL
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1011' WHERE ID='DBVersion'
GO
