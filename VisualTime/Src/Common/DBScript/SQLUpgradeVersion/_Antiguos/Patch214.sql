-- Nueva tabla para guardar huellas de los terminales RX
CREATE TABLE [dbo].[EmployeeBiometricDataRX](

      [IDEmployee] [int] NOT NULL,

      [IDFinger] [smallint] NOT NULL,

      [Data] [image] NULL,

      [TimeStamp] [smalldatetime] NULL,

      [IDTerminal] [int] NULL,

 CONSTRAINT [PK_EmployeeBiometricDataRX] PRIMARY KEY CLUSTERED 

(

      [IDEmployee] ASC,

      [IDFinger] ASC

) ON [PRIMARY]

) ON [PRIMARY]

GO

CREATE TABLE dbo.Tmp_AuditLive
	(
	ID numeric(18, 0) NOT NULL IDENTITY (1, 1),
	PassportName nvarchar(50) NOT NULL,
	Date datetime NOT NULL,
	ActionID smallint NOT NULL,
	ElementID smallint NOT NULL,
	ElementName nvarchar(4000) NULL,
	MessageParameters text NULL,
	SessionID int NOT NULL,
	ClientLocation nvarchar(50) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT dbo.Tmp_AuditLive ON
GO
IF EXISTS(SELECT * FROM dbo.AuditLive)
	 EXEC('INSERT INTO dbo.Tmp_AuditLive (ID, PassportName, Date, ActionID, ElementID, ElementName, MessageParameters, SessionID, ClientLocation)
		SELECT ID, PassportName, Date, ActionID, ElementID, ElementName, CONVERT(text, MessageParameters), SessionID, ClientLocation FROM dbo.AuditLive WITH (HOLDLOCK TABLOCKX)')
GO
SET IDENTITY_INSERT dbo.Tmp_AuditLive OFF
GO
DROP TABLE dbo.AuditLive
GO
EXECUTE sp_rename N'dbo.Tmp_AuditLive', N'AuditLive', 'OBJECT' 
GO
ALTER TABLE dbo.AuditLive ADD CONSTRAINT
	PK_AuditLive PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO

UPDATE sysroGUI SET RequiredFeatures = 'WebForms\Calendar' WHERE IDPath = 'Portal\Attendance\Scheduler'	
GO
UPDATE sysroGUI SET RequiredFeatures = 'WebForms\Causes' WHERE IDPath = 'Portal\Attendance\Causes'	
GO
UPDATE sysroGUI SET RequiredFeatures = 'WebForms\Employees' WHERE IDPath = 'Portal\Attendance\Employees'	
GO
UPDATE sysroGUI SET RequiredFeatures = 'WebForms\Options' WHERE IDPath = 'Portal\Configuration\Options'	
GO
UPDATE sysroGUI SET RequiredFeatures = 'WebForms\ShiftsExpress' WHERE IDPath = 'Portal\Attendance\Shifts'	
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='214' WHERE ID='DBVersion'
GO
