INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rxa100fp',1,25,'TA','1','Blind','E,S,X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rxa100fp',1,26,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rxa100fp',1,27,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rxa100p',1,28,'TA','1','Blind','E,S,X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rxa100p',1,29,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('rxa100p',1,30,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO

/* ***************************************************************************************************************************** */

ALTER TABLE dbo.TerminalsTasksSX
	DROP CONSTRAINT DF_TerminalsTasksSX_TaskDate
GO
CREATE TABLE dbo.Tmp_TerminalsTasksSX
	(
	IDTerminal int NOT NULL,
	Task nvarchar(20) NOT NULL,
	IDEmployee int NULL,
	Finger smallint NULL,
	DeleteConfirm bit NULL,
	TaskDate datetime NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_TerminalsTasksSX ADD CONSTRAINT
	DF_TerminalsTasksSX_TaskDate DEFAULT (getdate()) FOR TaskDate
GO
IF EXISTS(SELECT * FROM dbo.TerminalsTasksSX)
	 EXEC('INSERT INTO dbo.Tmp_TerminalsTasksSX (IDTerminal, Task, IDEmployee, Finger, DeleteConfirm, TaskDate)
		SELECT IDTerminal, Task, IDEmployee, Finger, DeleteConfirm, CONVERT(datetime, TaskDate) FROM dbo.TerminalsTasksSX WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.TerminalsTasksSX
GO
EXECUTE sp_rename N'dbo.Tmp_TerminalsTasksSX', N'TerminalsTasksSX', 'OBJECT' 
GO

/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='227' WHERE ID='DBVersion'
GO
