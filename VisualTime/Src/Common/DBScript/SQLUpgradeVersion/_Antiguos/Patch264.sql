--
-- Hacemos mas grande el contrato
--
ALTER TABLE [dbo].[EmployeeContracts] ALTER COLUMN IDcontract [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='264' WHERE ID='DBVersion'
GO
