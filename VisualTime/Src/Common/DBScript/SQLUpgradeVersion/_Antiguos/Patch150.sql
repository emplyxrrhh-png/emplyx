--
-- Actualiza concepts 
--
ALTER TABLE  [dbo].[concepts] ADD [ViewInTerminals] [bit] NULL 
GO
ALTER TABLE [dbo].[Concepts] ADD CONSTRAINT [DF_Concepts_ViewInTerminals] DEFAULT (0) FOR [ViewInTerminals]
GO
--
-- Actualiza Employees 
--
ALTER TABLE [dbo].[Employees] ADD [AllowQueriesOnTerminal] [bit] NULL , [AllowQueryAccrualsOnTerminal] [bit] NULL , [AllowQueryShiftsOnTerminal] [bit] NULL 
GO
--
-- Actualiza JobIncidences
--
CREATE  INDEX [IX_JobIncidences_ReaderInputCode] ON [dbo].[JobIncidences]([ReaderInputCode]) WITH  FILLFACTOR = 80,  PAD_INDEX  ON [PRIMARY]
GO
--
-- Actualiza Teams
--
CREATE  INDEX [IX_Teams_IDCard] ON [dbo].[Teams]([IDCard]) WITH  FILLFACTOR = 80,  PAD_INDEX  ON [PRIMARY]
GO
--
-- Actualiza EmployeeJobMoves
--
CREATE  INDEX [IX_EmployeeJobMoves_IDEmployee] ON [dbo].[EmployeeJobMoves]([IDEmployee]) WITH  FILLFACTOR = 80 ON [PRIMARY]
GO
--
-- Actualiza EmployeeContracts
--
CREATE  INDEX [IX_EmployeecontractsBeginDateIDEmployee] ON [dbo].[EmployeeContracts]([BeginDate], [IDEmployee], [EndDate], [IDCard]) ON [PRIMARY]
GO

-- Actualiza SysroGui 
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='Menu\Tools\ChangePassword' 
GO
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='Menu\File\Close' 
GO
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='NavBar' 
GO
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='Menu' 
GO
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='Menu\File' 
GO
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='NavBar\FirstTab\Reports' 
GO
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='NavBar\Config\Options' 
GO
UPDATE sysroGUI SET AllowedSecurity ='NR' WHERE IDPath='NavBar\Config\Users' 
GO
--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='150' WHERE ID='DBVersion'
GO
