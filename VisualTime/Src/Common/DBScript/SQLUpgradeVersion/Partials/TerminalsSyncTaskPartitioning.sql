--Particionado para TerminalsSyncTask en Live v6.5.6.0 (revisar si es necesario para versiones posteriores). 160 particiones por IDTerminal
BEGIN TRANSACTION

		IF OBJECT_ID('dbo.TerminalsSyncTasksPartitioned', 'U') IS NOT NULL
		BEGIN
			ALTER TABLE [dbo].[TerminalsSyncTasksPartitioned] DROP CONSTRAINT [DF_TerminalsSyncTaskPartitioned_DeleteOnConfirm];
			ALTER TABLE [dbo].[TerminalsSyncTasksPartitioned] DROP  CONSTRAINT [DF_TerminalsSyncTaskPartitioned_TaskDate];
			DROP TABLE [dbo].[TerminalsSyncTasksPartitioned];
		END

		IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'TerminalIDPartitionScheme')
		BEGIN
			DROP PARTITION SCHEME TerminalIDPartitionScheme;
		END

		IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'TerminalIDPartitionFunction')
		BEGIN
			DROP PARTITION FUNCTION TerminalIDPartitionFunction;
		END

		CREATE PARTITION FUNCTION TerminalIDPartitionFunction (INT)
		AS RANGE LEFT FOR VALUES (1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 
								 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 
								 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 
								 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 
								 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 
								 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 
								 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 
								 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160);

		CREATE PARTITION SCHEME TerminalIDPartitionScheme
		AS PARTITION TerminalIDPartitionFunction TO (
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], 
			[PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY], [PRIMARY]
		);

		CREATE TABLE [dbo].[TerminalsSyncTasksPartitioned](
			[IDTerminal] [int] NOT NULL,
			[Task] [nvarchar](30) NOT NULL,
			[IDEmployee] [int] NULL,
			[IDFinger] [smallint] NULL,
			[DeleteOnConfirm] [bit] NOT NULL,
			[TaskDate] [datetime] NOT NULL,
			[ID] [numeric](18, 0) IDENTITY(1,1) NOT NULL,
			[Parameter1] [int] NULL,
			[Parameter2] [int] NULL,
			[TaskData] [nvarchar](max) NULL,
			[TaskSent] [datetime] NULL,
			[TaskRetries] [smallint] NULL
		) ON TerminalIDPartitionScheme (IDTerminal);
		GO

		ALTER TABLE [dbo].[TerminalsSyncTasksPartitioned] ADD  CONSTRAINT [DF_TerminalsSyncTaskPartitioned_DeleteOnConfirm]  DEFAULT ((0)) FOR [DeleteOnConfirm]
		GO

		ALTER TABLE [dbo].[TerminalsSyncTasksPartitioned] ADD  CONSTRAINT [DF_TerminalsSyncTaskPartitioned_TaskDate]  DEFAULT (getdate()) FOR [TaskDate]
		GO

		CREATE CLUSTERED INDEX IX_TerminalsSyncTasksPartitioned_IdTerminal
		ON TerminalsSyncTasksPartitioned (IdTerminal)
		ON TerminalIDPartitionScheme (IdTerminal);

		INSERT INTO [dbo].[TerminalsSyncTasksPartitioned]
				   ([IDTerminal]
				   ,[Task]
				   ,[IDEmployee]
				   ,[IDFinger]
				   ,[DeleteOnConfirm]
				   ,[TaskDate]
				   ,[Parameter1]
				   ,[Parameter2]
				   ,[TaskData]
				   ,[TaskSent]
				   ,[TaskRetries])
		SELECT [IDTerminal]
			  ,[Task]
			  ,[IDEmployee]
			  ,[IDFinger]
			  ,[DeleteOnConfirm]
			  ,[TaskDate]
			  ,[Parameter1]
			  ,[Parameter2]
			  ,[TaskData]
			  ,[TaskSent]
			  ,[TaskRetries]
		  FROM [dbo].[TerminalsSyncTasks]
		GO

		EXEC sp_rename 'TerminalsSyncTasks', 'TerminalsSyncTasksBackup';
		EXEC sp_rename 'TerminalsSyncTasksPartitioned', 'TerminalsSyncTasks';

COMMIT


--Rollback: Particionado de base de datos
BEGIN TRANSACTION 
	EXEC sp_rename 'TerminalsSyncTasks', 'TerminalsSyncTasksPartitioned';
	EXEC sp_rename 'TerminalsSyncTasksBackup', 'TerminalsSyncTasks';

	TRUNCATE TABLE TerminalsSyncTasks
	GO
	INSERT INTO [dbo].[TerminalsSyncTasks]
			   ([IDTerminal]
			   ,[Task]
			   ,[IDEmployee]
			   ,[IDFinger]
			   ,[DeleteOnConfirm]
			   ,[TaskDate]
			   ,[Parameter1]
			   ,[Parameter2]
			   ,[TaskData]
			   ,[TaskSent]
			   ,[TaskRetries])
	SELECT [IDTerminal]
		  ,[Task]
		  ,[IDEmployee]
		  ,[IDFinger]
		  ,[DeleteOnConfirm]
		  ,[TaskDate]
		  ,[Parameter1]
		  ,[Parameter2]
		  ,[TaskData]
		  ,[TaskSent]
		  ,[TaskRetries]
	  FROM [dbo].[TerminalsSyncTasksPartitioned]
	GO

COMMIT