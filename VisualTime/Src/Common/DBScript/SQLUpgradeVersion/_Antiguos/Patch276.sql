-- Error en campo Security Flags
update groups set SecurityFlags = '' where SecurityFlags is null
GO

-- Cambio de tipo de campos que contienen referencias a identificadores de usarios
ALTER TABLE DailyCauses ALTER COLUMN CauseUser int
GO
ALTER TABLE Moves ALTER COLUMN InIdReader int
GO
ALTER TABLE Moves ALTER COLUMN OutIdReader int
GO

-- Corrección de tipos de terminales en tabla Terminals
UPDATE sysroReaderTemplates SET type='rxa' WHERE type = 'rxA'
GO

UPDATE sysroReaderTemplates SET type='rxb' WHERE type = 'rxB'
GO

UPDATE sysroReaderTemplates SET type='rxa100fp' WHERE type = 'rxA100fp'
GO

UPDATE sysroReaderTemplates SET type='SaiwallSecure' WHERE type = 'SAIWALL'
GO

UPDATE sysroReaderTemplates SET type='rxA100fp' WHERE type = 'rxA100FP'
GO

UPDATE sysroReaderTemplates SET type='rxA100p' WHERE type = 'rxA100P'
GO

UPDATE sysroReaderTemplates SET type='rxa200' WHERE type = 'rxA200'
GO

UPDATE Terminals SET type='LIVEPORTAL' WHERE type = 'LivePortal'
GO

UPDATE Terminals SET type= 'MX6' WHERE type = 'mx6'
GO

UPDATE Terminals SET type='MX7' WHERE type = 'mx7'
GO

UPDATE Terminals SET type='MXART' WHERE type = 'mxART'
GO

UPDATE Terminals SET type='RX' WHERE type = 'rx'
GO

UPDATE Terminals SET type='RXA' WHERE type = 'rxA'
GO

UPDATE Terminals SET type='RXA100FP' WHERE type = 'rxA100FP'
GO

UPDATE Terminals SET type='RXA100P' WHERE type = 'rxA100P'
GO

UPDATE Terminals SET type='RXA200' WHERE type = 'rxA200'
GO

UPDATE Terminals SET type='RXB' WHERE type = 'rxB'
GO

UPDATE Terminals SET type='RXF' WHERE type = 'rxF'
GO

UPDATE sysroReaderTemplates SET type='SaiwallSecure' WHERE type = 'SAIWALL'
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='276' WHERE ID='DBVersion'
GO
