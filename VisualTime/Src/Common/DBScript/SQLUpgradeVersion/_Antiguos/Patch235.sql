-- Optimizacion de indices

CREATE CLUSTERED INDEX [_dta_index_DailySchedule_c_6_503672842__K9_K2] ON [dbo].[DailySchedule] 
(
	[Status] ASC,
	[Date] ASC
)ON [PRIMARY]
GO

CREATE STATISTICS [_dta_stat_503672842_2_9] ON [dbo].[DailySchedule]([Date], [Status])
GO

CREATE CLUSTERED INDEX [_dta_index_DailyAccruals_c_6_98815414__K1_K2_K6_K5] ON [dbo].[DailyAccruals] 
(
	[IDEmployee] ASC,
	[Date] ASC,
	[StartupValue] ASC,
	[CarryOver] ASC
) ON [PRIMARY]
GO

CREATE STATISTICS [_dta_stat_98815414_3_2] ON [dbo].[DailyAccruals]([IDConcept], [Date])
GO

CREATE STATISTICS [_dta_stat_98815414_3_1] ON [dbo].[DailyAccruals]([IDConcept], [IDEmployee])
GO

CREATE STATISTICS [_dta_stat_98815414_2_1_5_6] ON [dbo].[DailyAccruals]([Date], [IDEmployee], [CarryOver], [StartupValue])
GO

CREATE CLUSTERED INDEX [_dta_index_DailyCauses_c_6_2054298378__K1_K9_K2] ON [dbo].[DailyCauses] 
(
	[IDEmployee] ASC,
	[AccrualsRules] ASC,
	[Date] ASC
) ON [PRIMARY]
GO

CREATE STATISTICS [_dta_stat_2054298378_1_2_9_4_6] ON [dbo].[DailyCauses]([IDEmployee], [Date], [AccrualsRules], [IDCause], [Manual])
GO

CREATE CLUSTERED INDEX [_dta_index_Moves_c_6_18815129__K1_K8] ON [dbo].[Moves] 
(
	[IDEmployee] ASC,
	[ShiftDate] ASC
) ON [PRIMARY]


CREATE STATISTICS [_dta_stat_18815129_3_8] ON [dbo].[Moves]([OutDateTime], [ShiftDate])
GO

CREATE STATISTICS [_dta_stat_18815129_1_9] ON [dbo].[Moves]([IDEmployee], [ID])
GO

CREATE STATISTICS [_dta_stat_18815129_1_8_2] ON [dbo].[Moves]([IDEmployee], [ShiftDate], [InDateTime])
GO

CREATE STATISTICS [_dta_stat_18815129_1_8_3] ON [dbo].[Moves]([IDEmployee], [ShiftDate], [OutDateTime])
GO

CREATE STATISTICS [_dta_stat_18815129_1_8_9] ON [dbo].[Moves]([IDEmployee], [ShiftDate], [ID])
GO

CREATE STATISTICS [_dta_stat_18815129_2_3_1_8] ON [dbo].[Moves]([InDateTime], [OutDateTime], [IDEmployee], [ShiftDate])
GO

CREATE CLUSTERED INDEX [_dta_index_ProgrammedAbsences_c_6_999674609__K2_K3] ON [dbo].[ProgrammedAbsences] 
(
	[IDEmployee] ASC,
	[BeginDate] ASC
) ON [PRIMARY]
GO


CREATE STATISTICS [_dta_stat_999674609_1_2_3] ON [dbo].[ProgrammedAbsences]([IDCause], [IDEmployee], [BeginDate])
GO

CREATE CLUSTERED INDEX [_dta_index_sysroShiftsCausesRules_c_6_132195521__K1] ON [dbo].[sysroShiftsCausesRules] 
(
	[IDShift] ASC
) ON [PRIMARY]
GO

CREATE STATISTICS [_dta_stat_132195521_4_2] ON [dbo].[sysroShiftsCausesRules]([RuleType], [ID])
GO

CREATE STATISTICS [_dta_stat_132195521_1_4_2] ON [dbo].[sysroShiftsCausesRules]([IDShift], [RuleType], [ID])
GO

CREATE CLUSTERED INDEX [_dta_index_sysroShiftsLayers_c_6_1019150676__K1] ON [dbo].[sysroShiftsLayers] 
(
	[IDShift] ASC
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [_dta_index_sysroShiftsLayers_6_1019150676__K1_K3_2_5] ON [dbo].[sysroShiftsLayers] 
(
	[IDShift] ASC,
	[IDType] ASC
)
  ON [PRIMARY]
GO

CREATE STATISTICS [_dta_stat_1019150676_3_1] ON [dbo].[sysroShiftsLayers]([IDType], [IDShift])
GO


CREATE STATISTICS [_dta_stat_503672842_9_2_8] ON [dbo].[DailySchedule]([Status], [Date], [Remarks])
GO

CREATE STATISTICS [_dta_stat_503672842_7_1_2] ON [dbo].[DailySchedule]([IDShiftUsed], [IDEmployee], [Date])
GO

CREATE STATISTICS [_dta_stat_503672842_9_2_7] ON [dbo].[DailySchedule]([Status], [Date], [IDShiftUsed])
GO

CREATE STATISTICS [_dta_stat_503672842_9_1_2_8] ON [dbo].[DailySchedule]([Status], [IDEmployee], [Date], [Remarks])
GO

CREATE STATISTICS [_dta_stat_503672842_9_1_2_7] ON [dbo].[DailySchedule]([Status], [IDEmployee], [Date], [IDShiftUsed])
GO

CREATE NONCLUSTERED INDEX [_dta_index_EmployeeGroups_6_485576768__K1_K2_K3_K4] ON [dbo].[EmployeeGroups] 
(
	[IDEmployee] ASC,
	[IDGroup] ASC,
	[BeginDate] ASC,
	[EndDate] ASC
)  ON [PRIMARY]
GO

CREATE STATISTICS [_dta_stat_485576768_3_1_2_4] ON [dbo].[EmployeeGroups]([BeginDate], [IDEmployee], [IDGroup], [EndDate])
GO

CREATE STATISTICS [_dta_stat_1798297466_2_56_57_58_59_60] ON [dbo].[Employees]([Name], [AttControlled], [AccControlled], [JobControlled], [ExtControlled], [RiskControlled])
GO

CREATE STATISTICS [_dta_stat_1798297466_2_1_56_57_58_59_60] ON [dbo].[Employees]([Name], [ID], [AttControlled], [AccControlled], [JobControlled], [ExtControlled], [RiskControlled])
GO

-- Nuevo permiso Groups en sysroFeatures
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
		 VALUES (1290 ,1 ,'Employees.Groups' ,'Grupos' ,'' ,'U' ,'RWA' ,NULL)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, 
(SELECT Permission 	from sysroPassports_PermissionsOverFeatures Where IDFeature = 1 And IDPassport = sysroPassports.ID) 
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (1290) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
AND (SELECT Count(*) from sysroPassports_PermissionsOverFeatures Where IDFeature = 1 And IDPassport = sysroPassports.ID) > 0
GO

-- Vista para el informe de estructura organizativa
CREATE VIEW sysroTypeGroups
AS
	SELECT ID, Name FROM Groups WHERE CHARINDEX('\',Path) = 0
GO

-- Hacemos que por defecto la pestaña de informes de Accesos sea la de Crystal
update sysrogui set URL = 'roFormReportsCR.vbd' where idpath = 'NavBar\Access\Reports'
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='235' WHERE ID='DBVersion'
GO
