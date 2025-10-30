/* ***************************************************************************************************************************** */
/*  Patch old bugs */
ALTER TABLE [dbo].[EmployeeContracts]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeContracts_LabAgree] FOREIGN KEY([IDLabAgree])
REFERENCES [dbo].[LabAgree] ([ID])
GO
ALTER TABLE [dbo].[EmployeeContracts] CHECK CONSTRAINT [FK_EmployeeContracts_LabAgree]
GO
ALTER TABLE dbo.Activities ADD CONSTRAINT
	PK_Activities PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.ActivityCompanies ADD CONSTRAINT
	PK_ActivityCompanies PRIMARY KEY CLUSTERED 
	(
	IDActivity,
	IDGroup
	) ON [PRIMARY]
GO
ALTER TABLE dbo.ActivityCompanies ADD CONSTRAINT
	FK_ActivityCompanies_Groups FOREIGN KEY
	(
	IDGroup
	) REFERENCES dbo.Groups
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.ActivityCompanies ADD CONSTRAINT
	FK_ActivityCompanies_GroupsParent FOREIGN KEY
	(
	IDParent
	) REFERENCES dbo.Groups
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	DF_InvalidAccessMoves_DateTime DEFAULT (getdate()) FOR DateTime
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	PK_InvalidAccessMoves PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	FK_InvalidAccessMoves_InvalidAccessMoves FOREIGN KEY
	(
	ID
	) REFERENCES dbo.InvalidAccessMoves
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	FK_InvalidAccessMoves_Employees FOREIGN KEY
	(
	IDEmployee
	) REFERENCES dbo.Employees
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO
CREATE TABLE dbo.Captures
	(
	ID int NOT NULL,
	DateTime smalldatetime NOT NULL,
	Capture image NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Captures ADD CONSTRAINT
	PK_Captures PRIMARY KEY CLUSTERED 
	(
	ID
	) ON [PRIMARY]

GO

ALTER TABLE dbo.InvalidAccessMoves ADD
	IDCapture int NULL
GO
ALTER TABLE dbo.InvalidAccessMoves ADD CONSTRAINT
	FK_InvalidAccessMoves_Captures FOREIGN KEY
	(
	IDCapture
	) REFERENCES dbo.Captures
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO

ALTER TABLE dbo.AccessMoves ADD
	IDCapture int NULL
GO
ALTER TABLE dbo.AccessMoves ADD CONSTRAINT
	FK_AccessMoves_Captures FOREIGN KEY
	(
	IDCapture
	) REFERENCES dbo.Captures
	(
	ID
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
/* ***************************************************************************************************************************** */
/*  sysroGUI menu changes */

DELETE From sysroGUI Where IDPath Like 'Portal%'
GO

INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal',NULL,NULL,NULL,NULL,NULL,NULL,NULL,1,NULL,NULL)
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Configuration','Configuration',NULL,'Administracion.ico',NULL,NULL,NULL,NULL,1006,'NWR','U:Administration.Options=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl','GUI.ShiftControl',NULL,'Presencia.ico',NULL,NULL,NULL,NULL,1001,'NWR',NULL)
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Security\Audit','Audit','Audit/Audit.aspx','Audit.png',NULL,NULL,NULL,NULL,210,'NWR','U:Administration.Audit=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl\Causes','GUI.Causes','Causes/Causes.aspx','Causes.png',NULL,NULL,'Forms\Causes',NULL,660,'NWR','U:Causes.Definition=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl\Accruals','GUI.Accruals','Concepts/Concepts.aspx','Concepts.png',NULL,NULL,NULL,NULL,622,'NWR','U:Concepts.Definition=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Configuration\Options','ConfigurationOptions','Options/ConfigurationOptions.aspx','Options.png',NULL,NULL,'Forms\Options',NULL,110,'NWR','U:Administration.Options.General=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Configuration\AttendanceOptions','AttendanceOptions','Options/AttendanceOptions.aspx','AttendanceOptions.png',NULL,NULL,NULL,NULL,120,'NWR','U:Administration.Options.Attendance=Read OR U:Employees.UserFields.Definition=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl\Scheduler','GUI.Scheduler','Scheduler/Scheduler.aspx','Scheduler.png',NULL,NULL,'Forms\Calendar',NULL,610,'NWR','U:Calendar=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl\Shifts','GUI.Shifts','Shifts/ShiftsExpress.aspx','Shifts.png',NULL,NULL,'Forms\ShiftsExpress',NULL,625,'NWR','U:Shifts.Definition=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Security','Security',NULL,'Security.png',NULL,NULL,NULL,NULL,1005,NULL,NULL)
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Security\Passports','GUI.Passports','Security/Passports.aspx','Passports.png',NULL,NULL,'Forms\Passports',NULL,200,NULL,'U:Administration.Security=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Configuration\Terminal','Terminals','Terminals/Terminals.aspx','TerminalesRT.png',NULL,NULL,'Forms\Terminals',NULL,130,'NWR','U:Terminals=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl\ShiftsPro','GUI.Shifts','Shifts/Shifts.aspx','Shifts.png',NULL,NULL,'Forms\Shifts',NULL,625,'NWR','U:Shifts.Definition=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\General\DataLink','DataLink','DataLink/DataLink.aspx','DataLink.png',NULL,NULL,'Forms\DataLink',NULL,603,NULL,NULL)
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Access','Access',NULL,'Access.png',NULL,NULL,'Forms\Access',NULL,1003,NULL,'U:Access=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Access\AccessStatus','GUI.AccessStatus','Access/AccessStatus.aspx','AccessStatus.png',NULL,NULL,'Forms\Access',NULL,710,NULL,'U:Access.Zones=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Access\AccessGroups','GUI.AccessGroups','Access/AccessGroups.aspx','AccessGroups.png',NULL,NULL,'Forms\Access',NULL,720,NULL,'U:Access.Groups=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Access\AccessPeriods','GUI.AccessPeriods','Access/AccessPeriods.aspx','AccessPeriods.png',NULL,NULL,'Forms\Access',NULL,730,NULL,'U:Access.Periods=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\Access\AccessZones','GUI.AccessZones','Access/AccessZones.aspx','AccessZones.png',NULL,NULL,'Forms\Access',NULL,740,NULL,'U:Access.Zones=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\OHP','OHP',NULL,'OHP.png',NULL,NULL,'Feature\OHP',NULL,1004,NULL,NULL)
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\OHP\Activities','Activities','Activities/Activities.aspx','Activity.png',NULL,NULL,'Forms\Activities',NULL,670,NULL,'U:Activities.Definition=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\ShiftControl\LabAgree','LabAgree','LabAgree/LabAgree.aspx','LabAgree.png',NULL,NULL,'Feature\ConcertRules',NULL,621,NULL,'U:LabAgree=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\LabAgree\LabAgreeRules','LabAgreeRules','LabAgree/LabAgreeRules.aspx','LabAgreeRules.png',NULL,NULL,'Feature\ConcertRules',NULL,2020,NULL,'U:LabAgree.AccrualRules=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\LabAgree\LabAgree','LabAgree','LabAgree/LabAgree.aspx','LabAgree.png',NULL,NULL,'Feature\ConcertRules',NULL,2010,NULL,'U:LabAgree.Definition=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\LabAgree\StartupValues','StartupValues','LabAgree/StartupValues.aspx','StartupValues.png',NULL,NULL,'Feature\ConcertRules',NULL,2030,NULL,'U:LabAgree.StartupValues=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\General\Employees','GUI.General.Employees','Employees/Employees.aspx','Employees.png',NULL,NULL,'Forms\Employees',NULL,602,'NWR','U:Employees=Read')
GO
INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
     VALUES ('Portal\General','General',NULL,'Maintenances.ico',NULL,NULL,'Forms\Employees',NULL,601,NULL,'U:Employees=Read')
GO

/* ***************************************************************************************************************************** */
/* sysroReaderTemplates RXA200 */

ALTER TABLE [sysroReaderTemplates] ADD 
	[Direction] [nvarchar](50) NULL
GO

DELETE FROM sysroReaderTemplates Where Type = 'rxa200'
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput, Direction) 
VALUES('rxa200',1,22,'TA','1','Blind','E,S,X','Local','1,0','0','0','1,0','0','Remote')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput, Direction) 
VALUES('rxa200',1,23,'ACC','1','Blind','X','Local','0','0','0','1','0','Remote')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput, Direction) 
VALUES('rxa200',1,24,'ACCTA','1','Blind','X','Local','0','0','0','1','0','Remote')
GO

/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='225' WHERE ID='DBVersion'
GO
