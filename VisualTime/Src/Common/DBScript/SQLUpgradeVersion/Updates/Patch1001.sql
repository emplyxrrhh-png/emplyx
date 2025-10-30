-- No borréis esta línea
IF NOT EXISTS (SELECT * FROM Terminals WHERE Type = 'LIVEPORTAL' AND ISNULL(Other,'') = 'APP')
BEGIN 
	DECLARE @MaxId int;  
	SET @MaxId = (SELECT MIN(t1.ID + 1) AS PrimerIDFaltante 
					FROM Terminals t1
					LEFT JOIN Terminals t2 ON t1.ID + 1 = t2.ID
					WHERE t2.ID IS NULL);  

	INSERT INTO [dbo].[Terminals]
			   ([ID]
			   ,[Description]
			   ,[Type]
			   ,[Behavior]
			   ,[Location]
			   ,[LastUpdate]
			   ,[LastStatus]
			   ,[SupportedModes]
			   ,[SupportedOutputs]
			   ,[SupportedSirens]
			   ,[SirensOutput]
			   ,[Functions]
			   ,[LastAction]
			   ,[Other]
			   ,[RigidMode]
			   ,[skin]
			   ,[AllowAntiPassBack]
			   ,[IsDifferentZoneTime]
			   ,[ZoneTime]
			   ,[AllowCustomButton]
			   ,[CustomLabel]
			   ,[CustomOutPut]
			   ,[CustomDuration]
			   ,[CustomField]
			   ,[CustomFieldValue]
			   ,[ConfData]
			   ,[AllowMoveReason]
			   ,[TimeZoneName]
			   ,[AutoDaylight]
			   ,[DisconnectionCounter]
			   ,[Model]
			   ,[LastStatusNotified]
			   ,[SerialNumber]
			   ,[RegistrationCode]
			   ,[PunchStamp]
			   ,[OperStamp]
			   ,[FirmVersion]
			   ,[SyncStartDate]
			   ,[ForceConfig]
			   ,[FirmwareUpgradeAvailable]
			   ,[SecurityOptions]
			   ,[UserCount]
			   ,[FaceCount]
			   ,[PalmCount]
			   ,[FingerCount]
			   ,[Wifi]
			   ,[Enabled])
		 SELECT
				@MaxId,
				CONCAT(Terminals.Description, ' APP'),  
				Terminals.Type,  
				Behavior,  
				Location,  
				LastUpdate,  
				LastStatus,  
				SupportedModes,  
				SupportedOutputs,  
				SupportedSirens,  
				SirensOutput,  
				Functions,  
				LastAction,  
				'APP',  
				RigidMode,  
				skin,  
				AllowAntiPassBack,  
				IsDifferentZoneTime,  
				ZoneTime,  
				AllowCustomButton,  
				CustomLabel,  
				CustomOutPut,  
				CustomDuration,  
				CustomField,  
				CustomFieldValue,  
				ConfData,  
				AllowMoveReason,  
				TimeZoneName,  
				AutoDaylight,  
				DisconnectionCounter,  
				Model,  
				LastStatusNotified,  
				SerialNumber,  
				RegistrationCode,  
				PunchStamp,  
				OperStamp,  
				FirmVersion,  
				SyncStartDate,  
				ForceConfig,  
				FirmwareUpgradeAvailable,  
				SecurityOptions,  
				UserCount,  
				FaceCount,  
				PalmCount,  
				FingerCount,  
				Wifi,  
				Enabled
		  FROM Terminals 
		  INNER JOIN TerminalReaders ON TerminalReaders.IDTerminal = Terminals.ID 
		  WHERE Terminals.Type = 'LivePortal' AND ISNULL(Other,'') <> 'APP' 
END 

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1001' WHERE ID='DBVersion'
GO
