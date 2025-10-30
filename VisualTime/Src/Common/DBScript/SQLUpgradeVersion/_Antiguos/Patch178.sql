CREATE TABLE [EmployeeBiometricData] (
	[IDEmployee] [int] NOT NULL ,
	[FWVersion] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[BiometricData] [varbinary] (1266) NULL ,
	[BiometricTimeStamp] [datetime] NULL ,
	[BiometricIDTerminal] [smallint] NULL ,
	CONSTRAINT [PK_EmployeeBiometricData] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[FWVersion]
	)  ON [PRIMARY] 
) ON [PRIMARY]
GO


INSERT INTO EmployeeBiometricData SELECT id,'1',biometricdata,BiometricTimeStamp,BiometricIDTerminal from employees where biometricdata is not null
GO




--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='178' WHERE ID='DBVersion'
GO
