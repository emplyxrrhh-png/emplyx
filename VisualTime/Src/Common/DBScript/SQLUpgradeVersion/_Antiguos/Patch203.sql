-- Se crea una tabla para los grupos de informes de emergencia
CREATE TABLE dbo.EmergencyReportsGroups
	(
	ID tinyint NOT NULL,
	Name text not null,
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.EmergencyReportsGroups ADD CONSTRAINT
	PK_EmergencyReportsGroups PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO
-- Creamos el grupo general
INSERT INTO EmergencyReportsGroups vALUES(1,'Default')
GO
Alter table dbo.ReportsScheduler
add EmergencyGroup tinyint not null default(0)
GO
-- Actualizamos los informes planificados actualmente
UPDATE dbo.ReportsScheduler SET EmergencyGroup=1 where EmergencyReport = 1 
GO
UPDATE dbo.ReportsScheduler SET EmergencyGroup=0 where EmergencyGroup = NULL 
GO

UPDATE sysroParameters SET Data='203' WHERE ID='DBVersion'
GO

