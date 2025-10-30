--Mantenimiento de perfiles 
DECLARE @idprofile int
DECLARE @profileName nvarchar(50)
DECLARE @profileDefinition nvarchar(max)
DECLARE @idReport int
DECLARE @newIdProfile int
DECLARE @oldProfilesValues table([ID] int, [Name] nvarchar(50), [Definition] nvarchar(max))

--Borramos perfiles no utilizados
DELETE FROM sysroReportProfile WHERE [ID] not in(SELECT [Profile] FROM ReportsScheduler)
--Cogemos los perfiles que debemos comprobar si se duplican
INSERT INTO @oldProfilesValues SELECT [ID],[Name],[Definition] FROM sysroReportProfile

DECLARE UserProfilesCursor CURSOR
FOR  SELECT ID,Name,Definition From @oldProfilesValues
OPEN UserProfilesCursor
FETCH NEXT FROM UserProfilesCursor
INTO @idprofile,@profileName,@profileDefinition
WHILE @@FETCH_STATUS = 0
BEGIN	

	DECLARE ReportsCursor CURSOR
	FOR  SELECT [ID] From ReportsScheduler WHERE [Profile] = @idprofile
	OPEN ReportsCursor
	FETCH NEXT FROM ReportsCursor
	INTO @idReport
	WHILE @@FETCH_STATUS = 0
	BEGIN	
   
		INSERT INTO sysroReportProfile([Name],[Definition]) VALUES(@profileName,@profileDefinition)

		SELECT @newIdProfile = Max([ID]) FROM sysroReportProfile

		UPDATE ReportsScheduler SET [Profile] = @newIdProfile WHERE [ID] = @idReport

		FETCH NEXT FROM ReportsCursor 
		INTO @idReport
	END 
	CLOSE ReportsCursor
	DEALLOCATE ReportsCursor

    FETCH NEXT FROM UserProfilesCursor 
    INTO @idprofile,@profileName,@profileDefinition
END 
CLOSE UserProfilesCursor
DEALLOCATE UserProfilesCursor


--Borramos perfiles temporales creados
DELETE FROM sysroReportProfile WHERE ID not in(SELECT [Profile] FROM ReportsScheduler)

--Introducimos todas las nuevas readertemplates
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,61,'TA',1,'Fast','E','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,62,'TA',1,'Fast','S','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,63,'TATSK',1,'Fast','E','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,64,'TATSK',1,'Fast','S','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,65,'TAEIP',1,'Fast','E','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,66,'TAEIP',1,'Fast','S','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,67,'TATSKEIP',1,'Fast','E','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO
INSERT INTO [dbo].[sysroReaderTemplates]([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction],[AllowAccessPeriods],[SupportedSirens],[RequiredFeatures])
VALUES('mx8',1,68,'TATSKEIP',1,'Fast','S','LocalServer,ServerLocal,Server,Local','1,0','1,0','0','1,0','0',NULL,1,'1','')
GO

UPDATE sysroParameters SET Data='345' WHERE ID='DBVersion'
GO


