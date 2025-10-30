--
-- Crea tabla de tipos de layers en horarios (util para visualizar en asistencias tecnicas)
-- 
if exists (select * from sysobjects where id = object_id(N'[dbo].[sysroShiftsLayersTypes]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[sysroShiftsLayersTypes]
GO
CREATE TABLE [dbo].[sysroShiftsLayersTypes]
    (
  [ID] [SMALLINT] NOT NULL,
  [Name] [nvarchar] (32) NOT NULL
)
GO
ALTER TABLE [dbo].[sysroShiftsLayersTypes] WITH NOCHECK ADD 
	CONSTRAINT [PK_ShiftsLayersTypes] PRIMARY KEY NONCLUSTERED ([ID])
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (0,'UNDEFINED')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1000,'roLTWorking')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1100,'roLTMandatory')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1200,'roLTBreak')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1300,'roLTPaidTime')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1400,'roLTUnitFilter')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1500,'roLTGroupFilter')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1600,'roLTWorkingMaxMinFilter')
GO
INSERT INTO [dbo].[sysroShiftsLayersTypes] (ID,Name) VALUES (1700,'roLTDailyTotalsFilter')
GO
ALTER TABLE [dbo].[sysroShiftsLayers] ADD 
	CONSTRAINT [FK_ShiftLayersTypes_ShiftsLayers] FOREIGN KEY 
	(
		[IDType]
	) REFERENCES [dbo].[sysroShiftsLayersTypes] (
		[ID]
	)
GO

CREATE  INDEX [IX_ShiftsLayersTypes] ON [dbo].[sysroShiftsLayersTypes](ID) ON [PRIMARY]
GO

--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='154' WHERE ID='DBVersion'
GO