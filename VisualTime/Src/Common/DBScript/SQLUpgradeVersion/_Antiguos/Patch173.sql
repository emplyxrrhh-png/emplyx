--
-- Actualizamos la tabla employees para que tenga los campos para el Sistema Biometric (Huella)
-- 03/2004

ALTER TABLE [dbo].[Employees] ADD [BiometricData] [varBinary] (1266) NULL
GO
ALTER TABLE [dbo].[Employees] ADD [BiometricTimeStamp] [datetime] NULL
GO

ALTER TABLE [dbo].[Employees] ADD [AllowBiometric] [bit]  NOT NULL DEFAULT 1
GO
ALTER TABLE [dbo].[Employees] ADD [AllowCards] [bit] NOT NULL DEFAULT 1
GO
ALTER TABLE [dbo].[Employees] ADD [AllowWeb] [bit] NOT NULL DEFAULT 0 
GO

 
ALTER TABLE [dbo].[Employees] ADD [WebLogin] [nvarChar] (50) NULL
GO
ALTER TABLE [dbo].[Employees] ADD [WebPassword] [nvarChar] (50) NULL
GO
ALTER TABLE [dbo].[Employees] ADD [WebIntegratedSecurity] [bit] NOT NULL DEFAULT 0
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='173' WHERE ID='DBVersion'
GO
