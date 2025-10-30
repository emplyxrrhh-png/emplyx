DROP TABLE [TmpAnnualConceptsEmployee]
GO

CREATE TABLE [TmpAnnualConceptsEmployee](
	[IDPassport] [int] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[Ejercicio] [int] NOT NULL,
	[Periodo] [int] NOT NULL,
	[IdGroup] [int] NULL,
	[GroupName] [nvarchar](200) NULL,
	[EmployeeName] [nvarchar](50) NULL,
	[IDConcept1] [smallint] NULL,
	[ConceptColor1] [nvarchar](10) NULL,
	[ConceptName1] [nvarchar](3) NULL,
	[ConceptValue1] [numeric](19, 6) NULL,
	[ConceptValueTotal1] [numeric](19, 6) NULL,
	[IDConcept2] [smallint] NULL,
	[ConceptColor2] [nvarchar](10) NULL,
	[ConceptName2] [nvarchar](3) NULL,
	[ConceptValue2] [numeric](19, 6) NULL,
	[ConceptValueTotal2] [numeric](19, 6) NULL,
	[IDConcept3] [smallint] NULL,
	[ConceptColor3] [nvarchar](10) NULL,
	[ConceptName3] [nvarchar](3) NULL,
	[ConceptValue3] [numeric](19, 6) NULL,
	[ConceptValueTotal3] [numeric](19, 6) NULL,
	[IDConcept4] [smallint] NULL,
	[ConceptColor4] [nvarchar](10) NULL,
	[ConceptName4] [nvarchar](3) NULL,
	[ConceptValue4] [numeric](19, 6) NULL,
	[ConceptValueTotal4] [numeric](19, 6) NULL,
	[IDConcept5] [smallint] NULL,
	[ConceptColor5] [nvarchar](10) NULL,
	[ConceptName5] [nvarchar](3) NULL,
	[ConceptValue5] [numeric](19, 6) NULL,
	[ConceptValueTotal5] [numeric](19, 6) NULL,
	[IDConcept6] [smallint] NULL,
	[ConceptColor6] [nvarchar](10) NULL,
	[ConceptName6] [nvarchar](3) NULL,
	[ConceptValue6] [numeric](19, 6) NULL,
	[ConceptValueTotal6] [numeric](19, 6) NULL,
	[IDConcept7] [smallint] NULL,
	[ConceptColor7] [nvarchar](10) NULL,
	[ConceptName7] [nvarchar](3) NULL,
	[ConceptValue7] [numeric](19, 6) NULL,
	[ConceptValueTotal7] [numeric](19, 6) NULL,
	[IDConcept8] [smallint] NULL,
	[ConceptColor8] [nvarchar](10) NULL,
	[ConceptName8] [nvarchar](3) NULL,
	[ConceptValue8] [numeric](19, 6) NULL,
	[ConceptValueTotal8] [numeric](19, 6) NULL,
	[IDConcept9] [smallint] NULL,
	[ConceptColor9] [nvarchar](10) NULL,
	[ConceptName9] [nvarchar](3) NULL,
	[ConceptValue9] [numeric](19, 6) NULL,
	[ConceptValueTotal9] [numeric](19, 6) NULL,
	[IDConcept10] [smallint] NULL,
	[ConceptColor10] [nvarchar](10) NULL,
	[ConceptName10] [nvarchar](3) NULL,
	[ConceptValue10] [numeric](19, 6) NULL,
	[ConceptValueTotal10] [numeric](19, 6) NULL,
	[IDConcept11] [smallint] NULL,
	[ConceptColor11] [nvarchar](10) NULL,
	[ConceptName11] [nvarchar](3) NULL,
	[ConceptValue11] [numeric](19, 6) NULL,
	[ConceptValueTotal11] [numeric](19, 6) NULL,
	[IDConcept12] [smallint] NULL,
	[ConceptColor12] [nvarchar](10) NULL,
	[ConceptName12] [nvarchar](3) NULL,
	[ConceptValue12] [numeric](19, 6) NULL,
	[ConceptValueTotal12] [numeric](19, 6) NULL,
	[IDConcept13] [smallint] NULL,
	[ConceptColor13] [nvarchar](10) NULL,
	[ConceptName13] [nvarchar](3) NULL,
	[ConceptValue13] [numeric](19, 6) NULL,
	[ConceptValueTotal13] [numeric](19, 6) NULL,
	[IDConcept14] [smallint] NULL,
	[ConceptColor14] [nvarchar](10) NULL,
	[ConceptName14] [nvarchar](3) NULL,
	[ConceptValue14] [numeric](19, 6) NULL,
	[ConceptValueTotal14] [numeric](19, 6) NULL,
	[IDConcept15] [smallint] NULL,
	[ConceptColor15] [nvarchar](10) NULL,
	[ConceptName15] [nvarchar](3) NULL,
	[ConceptValue15] [numeric](19, 6) NULL,
	[ConceptValueTotal15] [numeric](19, 6) NULL,
	[IDConcept16] [smallint] NULL,
	[ConceptColor16] [nvarchar](10) NULL,
	[ConceptName16] [nvarchar](3) NULL,
	[ConceptValue16] [numeric](19, 6) NULL,
	[ConceptValueTotal16] [numeric](19, 6) NULL,
	[IDConcept17] [smallint] NULL,
	[ConceptColor17] [nvarchar](10) NULL,
	[ConceptName17] [nvarchar](3) NULL,
	[ConceptValue17] [numeric](19, 6) NULL,
	[ConceptValueTotal17] [numeric](19, 6) NULL,
	[IDConcept18] [smallint] NULL,
	[ConceptColor18] [nvarchar](10) NULL,
	[ConceptName18] [nvarchar](3) NULL,
	[ConceptValue18] [numeric](19, 6) NULL,
	[ConceptValueTotal18] [numeric](19, 6) NULL,
	[IDConcept19] [smallint] NULL,
	[ConceptColor19] [nvarchar](10) NULL,
	[ConceptName19] [nvarchar](3) NULL,
	[ConceptValue19] [numeric](19, 6) NULL,
	[ConceptValueTotal19] [numeric](19, 6) NULL,
	[IDConcept20] [smallint] NULL,
	[ConceptColor20] [nvarchar](10) NULL,
	[ConceptName20] [nvarchar](3) NULL,
	[ConceptValue20] [numeric](19, 6) NULL,
	[ConceptValueTotal20] [numeric](19, 6) NULL,
	[IDConcept21] [smallint] NULL,
	[ConceptColor21] [nvarchar](10) NULL,
	[ConceptName21] [nvarchar](3) NULL,
	[ConceptValue21] [numeric](19, 6) NULL,
	[ConceptValueTotal21] [numeric](19, 6) NULL,
	[IDConcept22] [smallint] NULL,
	[ConceptColor22] [nvarchar](10) NULL,
	[ConceptName22] [nvarchar](3) NULL,
	[ConceptValue22] [numeric](19, 6) NULL,
	[ConceptValueTotal22] [numeric](19, 6) NULL,
	[IDConcept23] [smallint] NULL,
	[ConceptColor23] [nvarchar](10) NULL,
	[ConceptName23] [nvarchar](3) NULL,
	[ConceptValue23] [numeric](19, 6) NULL,
	[ConceptValueTotal23] [numeric](19, 6) NULL,
	[IDConcept24] [smallint] NULL,
	[ConceptColor24] [nvarchar](10) NULL,
	[ConceptName24] [nvarchar](3) NULL,
	[ConceptValue24] [numeric](19, 6) NULL,
	[ConceptValueTotal24] [numeric](19, 6) NULL,
	[IDConcept25] [smallint] NULL,
	[ConceptColor25] [nvarchar](10) NULL,
	[ConceptName25] [nvarchar](3) NULL,
	[ConceptValue25] [numeric](19, 6) NULL,
	[ConceptValueTotal25] [numeric](19, 6) NULL,
	[IDConcept26] [smallint] NULL,
	[ConceptColor26] [nvarchar](10) NULL,
	[ConceptName26] [nvarchar](3) NULL,
	[ConceptValue26] [numeric](19, 6) NULL,
	[ConceptValueTotal26] [numeric](19, 6) NULL,
	[IDConcept27] [smallint] NULL,
	[ConceptColor27] [nvarchar](10) NULL,
	[ConceptName27] [nvarchar](3) NULL,
	[ConceptValue27] [numeric](19, 6) NULL,
	[ConceptValueTotal27] [numeric](19, 6) NULL,
	[IDConcept28] [smallint] NULL,
	[ConceptColor28] [nvarchar](10) NULL,
	[ConceptName28] [nvarchar](3) NULL,
	[ConceptValue28] [numeric](19, 6) NULL,
	[ConceptValueTotal28] [numeric](19, 6) NULL,
	[IDConcept29] [smallint] NULL,
	[ConceptColor29] [nvarchar](10) NULL,
	[ConceptName29] [nvarchar](3) NULL,
	[ConceptValue29] [numeric](19, 6) NULL,
	[ConceptValueTotal29] [numeric](19, 6) NULL,
	[IDConcept30] [smallint] NULL,
	[ConceptColor30] [nvarchar](10) NULL,
	[ConceptName30] [nvarchar](3) NULL,
	[ConceptValue30] [numeric](19, 6) NULL,
	[ConceptValueTotal30] [numeric](19, 6) NULL,
	[IDConcept31] [smallint] NULL,
	[ConceptColor31] [nvarchar](10) NULL,
	[ConceptName31] [nvarchar](3) NULL,
	[ConceptValue31] [numeric](19, 6) NULL,
	[ConceptValueTotal31] [numeric](19, 6) NULL,
	[AbsencesTemplate] [nvarchar](31) NULL,
 CONSTRAINT [PK_TmpAnnualConceptsEmployee] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC,
	[IDEmployee] ASC,
	[Ejercicio] ASC,
	[Periodo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE VIEW [dbo].[sysroDailyAccrualsByContract]
AS
SELECT     TOP (100) PERCENT IDEmployee, Date, IDConcept, Value, CarryOver, StartupValue,
                          (SELECT     TOP (1) IDContract
                            FROM          dbo.EmployeeContracts
                            WHERE      (dbo.DailyAccruals.IDEmployee = IDEmployee) AND (BeginDate <= dbo.DailyAccruals.Date) AND (EndDate >= dbo.DailyAccruals.Date)) AS NumContrato,
                          (SELECT     TOP (1) PERCENT CASE WHEN fixedpay = 0 THEN
                                                       (SELECT     TOP (1) (CAST(CAST(Value AS varchar(10)) AS decimal(10, 6)))
                                                         FROM          EmployeeUserFieldValues
                                                         WHERE      idEmployee = dailyaccruals.idemployee AND FieldName = RIGHT(Concepts.UsedField, LEN(Concepts.usedfield) - 4) AND 
                                                                                EmployeeUserFieldValues.DATE <= dailyaccruals.date
                                                         ORDER BY date DESC) WHEN FixedPay = 1 THEN payvalue END AS Expr1
                            FROM          dbo.Concepts
                            WHERE      (ViewInPays <> 0) AND (dbo.DailyAccruals.IDConcept = ID)) AS CosteHora
FROM         dbo.DailyAccruals

GO

CREATE VIEW [dbo].[sysroDailyScheduleByContract]
AS
SELECT     IDEmployee, Date, IDShift1, IDShift2, IDShift3, IDShift4, IDShiftUsed, Remarks, Status, JobStatus, LockedDay, StartShiftUsed, StartShift1, StartShift2, StartShift3, 
                      StartShift4, IDAssignment, IsCovered, OldIDAssignment, OldIDShift, IDEmployeeCovered, TaskStatus, IDShiftBase, StartShiftBase, IDAssignmentBase, IsHolidays,
                          (SELECT     TOP (1) IDContract
                            FROM          dbo.EmployeeContracts
                            WHERE      (dbo.DailySchedule.IDEmployee = IDEmployee) AND (BeginDate <= dbo.DailySchedule.Date) AND (EndDate >= dbo.DailySchedule.Date)) 
                      AS NumContrato
FROM         dbo.DailySchedule

GO

alter table TMPDailyPartialAccruals ADD idContract nvarchar(50)

GO

CREATE TABLE [dbo].[TMPHOLIDAYSCONTROLByContract](
	[IDEmployee] [int] NOT NULL,
	[IDConcept] [smallint] NOT NULL,
	[StartupValue] [numeric](16, 2) NOT NULL,
	[CarryOver] [numeric](16, 2) NOT NULL,
	[EnjoyedDays] [numeric](16, 2) NOT NULL,
	[CurrentBalance] [numeric](16, 2) NOT NULL,
	[DaysProvided] [numeric](16, 2) NOT NULL,
	[EndingBalance] [numeric](16, 2) NOT NULL,
	[idContract] [nvarchar](50) NOT NULL
) ON [PRIMARY]


GO

CREATE TABLE [dbo].[TMPCALENDAREMPLOYEEByContract](
	[IDEmployee] [numeric](18, 0) NOT NULL,
	[EMPLEADO] [nvarchar](50) NULL,
	[idContract] [nvarchar](50) NOT NULL,
	[MES] [int] NOT NULL,
	[NOMBRE] [nvarchar](50) NULL,
	[DIAS] [int] NULL,
	[horasdia1] [numeric](9, 6) NOT NULL,
	[horasdia2] [numeric](9, 6) NOT NULL,
	[horasdia3] [numeric](9, 6) NOT NULL,
	[horasdia4] [numeric](9, 6) NOT NULL,
	[horasdia5] [numeric](9, 6) NOT NULL,
	[horasdia6] [numeric](9, 6) NOT NULL,
	[horasdia7] [numeric](9, 6) NOT NULL,
	[horasdia8] [numeric](9, 6) NOT NULL,
	[horasdia9] [numeric](9, 6) NOT NULL,
	[horasdia10] [numeric](9, 6) NOT NULL,
	[horasdia11] [numeric](9, 6) NOT NULL,
	[horasdia12] [numeric](9, 6) NOT NULL,
	[horasdia13] [numeric](9, 6) NOT NULL,
	[horasdia14] [numeric](9, 6) NOT NULL,
	[horasdia15] [numeric](9, 6) NOT NULL,
	[horasdia16] [numeric](9, 6) NOT NULL,
	[horasdia17] [numeric](9, 6) NOT NULL,
	[horasdia18] [numeric](9, 6) NOT NULL,
	[horasdia19] [numeric](9, 6) NOT NULL,
	[horasdia20] [numeric](9, 6) NOT NULL,
	[horasdia21] [numeric](9, 6) NOT NULL,
	[horasdia22] [numeric](9, 6) NOT NULL,
	[horasdia23] [numeric](9, 6) NOT NULL,
	[horasdia24] [numeric](9, 6) NOT NULL,
	[horasdia25] [numeric](9, 6) NOT NULL,
	[horasdia26] [numeric](9, 6) NOT NULL,
	[horasdia27] [numeric](9, 6) NOT NULL,
	[horasdia28] [numeric](9, 6) NOT NULL,
	[horasdia29] [numeric](9, 6) NOT NULL,
	[horasdia30] [numeric](9, 6) NOT NULL,
	[horasdia31] [numeric](9, 6) NOT NULL,
	[TOTALHORAS] [nvarchar](50) NULL,
	[TOTALDIAS] [int] NULL,
	[TotalHorasAnuales] [numeric](18, 2) NULL,
	[TotalDiasAnuales] [numeric](18, 0) NULL,
 CONSTRAINT [PK_TMPCALENDAREMPLOYEEByContract] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[idContract] ASC,
	[MES] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract](
	[IDEmployee] [numeric](18, 0) NOT NULL,	
	[EMPLEADO] [nvarchar](50) NULL,
	[MES] [int] NOT NULL,
	[AÑO] [int] NOT NULL,
	[idContract] [nvarchar](50) NOT NULL,
	[NOMBRE] [nvarchar](50) NULL,
	[DIAS] [int] NULL,
	[horasdia1] [numeric](9, 6) NOT NULL,
	[horasdia2] [numeric](9, 6) NOT NULL,
	[horasdia3] [numeric](9, 6) NOT NULL,
	[horasdia4] [numeric](9, 6) NOT NULL,
	[horasdia5] [numeric](9, 6) NOT NULL,
	[horasdia6] [numeric](9, 6) NOT NULL,
	[horasdia7] [numeric](9, 6) NOT NULL,
	[horasdia8] [numeric](9, 6) NOT NULL,
	[horasdia9] [numeric](9, 6) NOT NULL,
	[horasdia10] [numeric](9, 6) NOT NULL,
	[horasdia11] [numeric](9, 6) NOT NULL,
	[horasdia12] [numeric](9, 6) NOT NULL,
	[horasdia13] [numeric](9, 6) NOT NULL,
	[horasdia14] [numeric](9, 6) NOT NULL,
	[horasdia15] [numeric](9, 6) NOT NULL,
	[horasdia16] [numeric](9, 6) NOT NULL,
	[horasdia17] [numeric](9, 6) NOT NULL,
	[horasdia18] [numeric](9, 6) NOT NULL,
	[horasdia19] [numeric](9, 6) NOT NULL,
	[horasdia20] [numeric](9, 6) NOT NULL,
	[horasdia21] [numeric](9, 6) NOT NULL,
	[horasdia22] [numeric](9, 6) NOT NULL,
	[horasdia23] [numeric](9, 6) NOT NULL,
	[horasdia24] [numeric](9, 6) NOT NULL,
	[horasdia25] [numeric](9, 6) NOT NULL,
	[horasdia26] [numeric](9, 6) NOT NULL,
	[horasdia27] [numeric](9, 6) NOT NULL,
	[horasdia28] [numeric](9, 6) NOT NULL,
	[horasdia29] [numeric](9, 6) NOT NULL,
	[horasdia30] [numeric](9, 6) NOT NULL,
	[horasdia31] [numeric](9, 6) NOT NULL,
	[TOTALHORAS] [numeric](9, 6) NOT NULL,
	[TOTALDIAS] [int] NULL,
	[TotalHorasAnuales] [numeric](18, 2) NULL,
	[TotalDiasAnuales] [numeric](18, 0) NULL,
 CONSTRAINT [PK_TMPANNUALINDIVIDUALCALENDARByContract] PRIMARY KEY NONCLUSTERED 
(
	[IDEmployee] ASC,
	[MES] ASC,
	[AÑO] ASC,
	[IDContract] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia17]  DEFAULT ((0)) FOR [horasdia17]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia18]  DEFAULT ((0)) FOR [horasdia18]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia19]  DEFAULT ((0)) FOR [horasdia19]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia20]  DEFAULT ((0)) FOR [horasdia20]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia21]  DEFAULT ((0)) FOR [horasdia21]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia22]  DEFAULT ((0)) FOR [horasdia22]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia23]  DEFAULT ((0)) FOR [horasdia23]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia24]  DEFAULT ((0)) FOR [horasdia24]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia25]  DEFAULT ((0)) FOR [horasdia25]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia26]  DEFAULT ((0)) FOR [horasdia26]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia27]  DEFAULT ((0)) FOR [horasdia27]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia28]  DEFAULT ((0)) FOR [horasdia28]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia29]  DEFAULT ((0)) FOR [horasdia29]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia30]  DEFAULT ((0)) FOR [horasdia30]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_horasdia31]  DEFAULT ((0)) FOR [horasdia31]
GO

ALTER TABLE [dbo].[TMPANNUALINDIVIDUALCALENDARByContract] ADD  CONSTRAINT [DF_TMPANNUALINDIVIDUALCALENDARByContract_TOTALHORAS]  DEFAULT ((0)) FOR [TOTALHORAS]
GO

CREATE TABLE [dbo].[sysroSecurityParameters](
	[IDPassport] [int] NOT NULL,
	[PasswordSecurityLevel] [smallint] NULL,
	[PreviousPasswordsStored] [int] NULL,
	[DaysPasswordExpired] [int] NULL,
	[AccessAttempsTemporaryBlock] [int] NULL,
	[AccessAttempsPermanentBlock] [int] NULL,
	[OnlyAllowedAdress] [text] NULL,
	[OnlySameVersionApp] [bit] NULL,
	[RequestValidationCode] [bit] NULL,
	[SaveAuthorizedPointDays] [int] NULL,
	[DifferentAccessPointExceeded] [int] NULL,
	[BlockAccessVTSupervisor] [bit] NULL,
	[BlockAccessVTPortal] [bit] NULL,
	[BlockAccessVTDesktop] [bit] NULL,
	[EnabledAccessSupportRobotics] [bit] NULL,
 CONSTRAINT [PK_sysroSecurityParameters] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_PasswordSecurityLevel]  DEFAULT ((0)) FOR [PasswordSecurityLevel]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_PreviousPasswordsStored]  DEFAULT ((0)) FOR [PreviousPasswordsStored]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_DaysPasswordExpired]  DEFAULT ((0)) FOR [DaysPasswordExpired]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_AccessAttempsTemporaryBlock]  DEFAULT ((0)) FOR [AccessAttempsTemporaryBlock]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_AccessAttempsPermanentBlock]  DEFAULT ((0)) FOR [AccessAttempsPermanentBlock]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_OnlySameVersionApp]  DEFAULT ((0)) FOR [OnlySameVersionApp]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_RequestValidationCode]  DEFAULT ((0)) FOR [RequestValidationCode]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_SaveAuthorizedPointDays]  DEFAULT ((0)) FOR [SaveAuthorizedPointDays]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_DifferentAccessPointExceeded]  DEFAULT ((0)) FOR [DifferentAccessPointExceeded]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_BlockAccessVTSupervisor]  DEFAULT ((0)) FOR [BlockAccessVTSupervisor]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_BlockAccessVTPortal]  DEFAULT ((0)) FOR [BlockAccessVTPortal]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_BlockAccessVTDesktop]  DEFAULT ((0)) FOR [BlockAccessVTDesktop]
GO

ALTER TABLE [dbo].[sysroSecurityParameters] ADD  CONSTRAINT [DF_sysroSecurityParameters_EnabledAccessSupportRobotics]  DEFAULT ((1)) FOR [EnabledAccessSupportRobotics]
GO

-- registro por defecto con la configuración general
INSERT INTO [dbo].[sysroSecurityParameters] (IDPassport,PasswordSecurityLevel, PreviousPasswordsStored, DaysPasswordExpired, AccessAttempsTemporaryBlock, AccessAttempsPermanentBlock, OnlyAllowedAdress, OnlySameVersionApp, RequestValidationCode, SaveAuthorizedPointDays, DifferentAccessPointExceeded , BlockAccessVTSupervisor, BlockAccessVTPortal, BlockAccessVTDesktop, EnabledAccessSupportRobotics) 
	VALUES (0,0,1,0, 5,10,'',0,0,60, 2,0,0,0,1) 
GO



-- nueva entrada en el menu principal
INSERT INTO sysroGUI
([IDPath],[LanguageReference],[URL],[IconURL],[RequiredFeatures],[Priority],[RequiredFunctionalities])
VALUES ('Portal\Security\Options', 'SecurityOptions', 'Security/SecurityOptions.aspx', 'Options.png', 'Forms\Passports', 220, 'U:Administration.SecurityOptions=Read')
GO

INSERT INTO sysroFeatures
([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (7630 ,7 ,'Administration.SecurityOptions' ,'opciones de seguridad' ,'' ,'U' ,'NWR' ,NULL)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, 6
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (7630) AND
	  sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
AND (SELECT Count(*) from sysroPassports_PermissionsOverFeatures Where IDFeature = 7 And IDPassport = sysroPassports.ID) > 0
GO



--  NUEVOS CAMPOS PARA INDICAR APLICACIONES PERMITIDAS DEL PASSPORT
ALTER TABLE [dbo].[sysroPassports] ADD  
	EnabledVTDesktop bit NULL DEFAULT(1),
	EnabledVTPortal bit NULL DEFAULT(1),
	EnabledVTSupervisor bit NULL DEFAULT(1),
	EnabledVTPortalApp bit NULL DEFAULT(1),
	EnabledVTSupervisorApp bit NULL DEFAULT(1)
GO

UPDATE [dbo].[sysroPassports] SET EnabledVTDesktop = 1 WHERE EnabledVTDesktop is null
GO

UPDATE [dbo].[sysroPassports] SET EnabledVTPortal = 1 WHERE EnabledVTPortal is null
GO

UPDATE [dbo].[sysroPassports] SET EnabledVTSupervisor = 1 WHERE EnabledVTSupervisor is null
GO

UPDATE [dbo].[sysroPassports] SET EnabledVTPortalApp = 1 WHERE EnabledVTPortalApp is null
GO

UPDATE [dbo].[sysroPassports] SET EnabledVTSupervisorApp = 1 WHERE EnabledVTSupervisorApp is null
GO


--  nuevos campos para controlar login de usuarios
ALTER TABLE [dbo].[sysroPassports] ADD  
	NeedValidationCode bit NULL DEFAULT(0),
	ValidationCode nvarchar(100) NULL DEFAULT(''),
	TimeStampValidationCode SMALLDATETIME NULL
GO

UPDATE [dbo].[sysroPassports] SET NeedValidationCode = 0 WHERE NeedValidationCode is null
GO

UPDATE [dbo].[sysroPassports] SET ValidationCode= '' WHERE ValidationCode is null
GO



-- Modificamos procedures de Passports
ALTER PROCEDURE [dbo].[WebLogin_Passports_Select] 
(
	@idPassport int
)
AS
   	SELECT ID,
   		IDParentPassport,
   		GroupType,
   		Name,
   		Description,
   		Email,
   		IDUser,
   		IDEmployee,
   		IDLanguage,
   		dbo.GetPassportLevelOfAuthority(@idPassport) AS LevelOfAuthority,
   		ConfData,
  		AuthenticationMerge,
  		StartDate,
  		ExpirationDate,
  		[State],
		EnabledVTDesktop,
		EnabledVTPortal,
		EnabledVTSupervisor,
		EnabledVTPortalApp ,
		EnabledVTSupervisorApp ,
		NeedValidationCode ,
		TimeStampValidationCode,
		ValidationCode
   	FROM sysroPassports
   	WHERE ID = @idPassport
   	
	RETURN
GO


  ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectByIDEmployee] 
  	(
  		@idEmployee int
  	)
  AS
  	SELECT ID,
  		IDParentPassport,
  		GroupType,
  		Name,
  		Description,
  		Email,
  		IDUser,
  		IDEmployee,
  		IDLanguage,
  		LevelOfAuthority,
 		ConfData,
 		AuthenticationMerge,
 		StartDate,
 		ExpirationDate,
 		[State],
		EnabledVTDesktop,
		EnabledVTPortal,
		EnabledVTSupervisor,
		EnabledVTPortalApp ,
		EnabledVTSupervisorApp ,
		NeedValidationCode ,
		TimeStampValidationCode,
		ValidationCode
 		
  	FROM sysroPassports
  	WHERE IDEmployee = @idEmployee
  	
  	RETURN
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectByIDUser]
  	(
  		@idUser int
  	)
  AS
  	SELECT ID,
  		IDParentPassport,
  		GroupType,
  		Name,
  		Description,
  		Email,
  		IDUser,
  		IDEmployee,
  		IDLanguage,
  		LevelOfAuthority,
 		ConfData,
 		AuthenticationMerge,
 		StartDate,
 		ExpirationDate,
 		[State],
		EnabledVTDesktop,
		EnabledVTPortal,
		EnabledVTSupervisor,
		EnabledVTPortalApp ,
		EnabledVTSupervisorApp ,
		NeedValidationCode ,
		TimeStampValidationCode ,
		ValidationCode
 		
  	FROM sysroPassports
  	WHERE IDUser = @idUser
  	
  	RETURN
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAll] 
  	
  AS
  	SELECT ID,
  		IDParentPassport,
  		GroupType,
  		Name,
  		Description,
  		Email,
  		IDUser,
  		IDEmployee,
  		IDLanguage,
  		LevelOfAuthority,
 		ConfData,
 		AuthenticationMerge,
 		StartDate,
 		ExpirationDate,
 		[State],
		EnabledVTDesktop,
		EnabledVTPortal,
		EnabledVTSupervisor,
		EnabledVTPortalApp ,
		EnabledVTSupervisorApp ,
		NeedValidationCode ,
		TimeStampValidationCode ,
		ValidationCode
  	FROM sysroPassports
  	
  	RETURN
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAllEmployee] 
  	
  AS
  	SELECT ID,
  		IDParentPassport,
  		GroupType,
  		Name,
  		Description,
  		Email,
  		IDUser,
  		IDEmployee,
  		IDLanguage,
  		LevelOfAuthority,
 		ConfData,
 		AuthenticationMerge,
 		StartDate,
 		ExpirationDate,
 		[State],
		EnabledVTDesktop,
		EnabledVTPortal,
		EnabledVTSupervisor,
		EnabledVTPortalApp ,
		EnabledVTSupervisorApp ,
		NeedValidationCode ,
		TimeStampValidationCode ,
		ValidationCode
  	FROM sysroPassports
  	WHERE IDEmployee IS NOT NULL
  	
  	RETURN
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_SelectAllUser] 
  	
  AS
  	SELECT ID,
  		IDParentPassport,
  		GroupType,
  		Name,
  		Description,
  		Email,
  		IDUser,
  		IDEmployee,
  		IDLanguage,
  		LevelOfAuthority,
 		ConfData,
 		AuthenticationMerge,
 		StartDate,
 		ExpirationDate,
 		[State],
		EnabledVTDesktop,
		EnabledVTPortal,
		EnabledVTSupervisor,
		EnabledVTPortalApp ,
		EnabledVTSupervisorApp ,
		NeedValidationCode ,
		TimeStampValidationCode ,
		ValidationCode
  	FROM sysroPassports
  	WHERE IDUser IS NOT NULL
  	
  	RETURN
GO


  ALTER PROCEDURE [dbo].[WebLogin_Passports_Insert] 
  	(
  		@id int OUTPUT,
  		@idParentPassport int,
  		@groupType varchar(1),
  		@name nvarchar(50),
  		@description nvarchar(100),
  		@email nvarchar(100),
  		@idUser int,
  		@idEmployee int,
  		@idLanguage int,
  		@levelOfAuthority tinyint, 
  		@ConfData text,
 		@AuthenticationMerge nvarchar(50),
 		@StartDate smalldatetime,
 		@ExpirationDate smalldatetime,
 		@State smallint, 		
 		@EnabledVTDesktop bit,
		@EnabledVTPortal bit,
		@EnabledVTSupervisor bit,
		@EnabledVTPortalApp bit ,
		@EnabledVTSupervisorApp bit ,
		@NeedValidationCode bit ,
		@TimeStampValidationCode smalldatetime ,
		@ValidationCode nvarchar(100)
  	)
  AS
  	INSERT INTO sysroPassports (
  		IDParentPassport,
  		GroupType,
  		Name,
  		Description,
  		Email,
  		IDUser,
  		IDEmployee,
  		IDLanguage,
  		LevelOfAuthority, 
  		ConfData,
 		AuthenticationMerge,
 		StartDate,
 		ExpirationDate,
 		[State],
 		EnabledVTDesktop,
		EnabledVTPortal,
		EnabledVTSupervisor,
		EnabledVTPortalApp,
		EnabledVTSupervisorApp,
		NeedValidationCode,
		TimeStampValidationCode,
		ValidationCode
 		
  	)
  	VALUES (
  		@idParentPassport,
  		@groupType,
  		@name,
  		@description,
  		@email,
  		@idUser,
  		@idEmployee,
  		@idLanguage,
  		@levelOfAuthority,
  		@ConfData,
 		@AuthenticationMerge,
 		@StartDate,
 		@ExpirationDate,
 		@State,
 		@EnabledVTDesktop,
		@EnabledVTPortal,
		@EnabledVTSupervisor,
		@EnabledVTPortalApp,
		@EnabledVTSupervisorApp,
		@NeedValidationCode,
		@TimeStampValidationCode,
		@ValidationCode 
 		
  	)
  	
  	SELECT @id = @@IDENTITY
  	
  	RETURN
 GO
 
 ALTER PROCEDURE [dbo].[WebLogin_Passports_Update] 
 (@id int,
 @idParentPassport int,
 @groupType varchar(1),
 @name nvarchar(50),
 @description nvarchar(4000),
 @email nvarchar(100),
 @idUser int,
 @idEmployee int,
 @idLanguage int,
 @levelOfAuthority tinyint,
 @ConfData text,
 @AuthenticationMerge nvarchar(50),
 @StartDate smalldatetime,
 @ExpirationDate smalldatetime,
 @State smallint,
 @EnabledVTDesktop bit,
 @EnabledVTPortal bit,
 @EnabledVTSupervisor bit,
 @EnabledVTPortalApp bit,
 @EnabledVTSupervisorApp bit,
 @NeedValidationCode bit,
 @TimeStampValidationCode smalldatetime,
 @ValidationCode  nvarchar(100))
AS
 IF @groupType <> 'U' 
 	BEGIN
 		SET @levelOfAuthority = NULL
 	END
 UPDATE sysroPassports SET
 	IDParentPassport = @idParentPassport,
 	GroupType = @groupType,
 	Name = @name,
 	Description = @description,
 	Email = @email,
 	IDUser = @idUser,
 	IDEmployee = @idEmployee,
 	IDLanguage = @idLanguage,
 	LevelOfAuthority = @levelOfAuthority,
 	ConfData = @ConfData,
 	AuthenticationMerge = @AuthenticationMerge,
 	StartDate = @StartDate,
 	ExpirationDate = @ExpirationDate,
 	[State] = @State, 
	EnabledVTDesktop = @EnabledVTDesktop,
	EnabledVTPortal = @EnabledVTPortal,
	EnabledVTSupervisor = @EnabledVTSupervisor,
	EnabledVTPortalApp = @EnabledVTPortalApp,
	EnabledVTSupervisorApp = @EnabledVTSupervisorApp,
	NeedValidationCode = @NeedValidationCode,
	TimeStampValidationCode = @TimeStampValidationCode,
	ValidationCode  = @ValidationCode 
 	
 WHERE ID = @id
 RETURN
GO



---- Almacenar historico de contraseñas
CREATE TABLE [dbo].[sysroPassports_PasswordHistory](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[IDPassport] [int] NOT NULL,
	[Password] [nvarchar](1000) NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_sysroPassports_PasswordHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

----- Almacenar direcciones autorizadas por passport
CREATE TABLE [dbo].[sysroPassports_AuthorizedAdress](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[Adress] [nvarchar](50),
	[IDPassport] [int] NOT NULL,
	[TimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_sysroPassports_sysroPassports_AuthorizedAdress] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO


------ Almacenar ultimo cambio de contraseña accesos erroneos y bloqueo del passport a aplicaciones
ALTER TABLE [dbo].[sysroPassports_AuthenticationMethods] ADD  
	LastUpdatePassword [datetime] NULL, 
	BloquedAccessApp [bit] NULL DEFAULT(0), 
	InvalidAccessAttemps [int] NULL DEFAULT(0),
	LastDateInvalidAccessAttempted [datetime] NULL
GO

UPDATE [dbo].[sysroPassports_AuthenticationMethods] SET BloquedAccessApp = 0 WHERE BloquedAccessApp is null
GO

UPDATE [dbo].[sysroPassports_AuthenticationMethods] SET LastUpdatePassword= getdate() WHERE LastUpdatePassword is null
GO

UPDATE [dbo].[sysroPassports_AuthenticationMethods] SET InvalidAccessAttemps= 0 WHERE 	InvalidAccessAttemps is null
GO




  ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethod_Select] 
  	(
  		@method int,
 		@version nvarchar(50)		
  	)
  AS
  	SELECT IDPassport,
  		Method,
 		Version,
  		Credential,
  		Password,
  		StartDate,
  		ExpirationDate,
 		BiometricID,
 		BiometricData,
 		TimeStamp,
 		Enabled,
		LastUpdatePassword,
		BloquedAccessApp ,
		InvalidAccessAttemps, 
		LastDateInvalidAccessAttempted
  	FROM sysroPassports_AuthenticationMethods
  	WHERE Method = @method AND
 		  Version = @version AND		  
  		  (StartDate IS NULL OR StartDate <= GetDate()) AND
  		  (ExpirationDate IS NULL OR ExpirationDate > GetDate())
 	
  	
  	RETURN
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Insert] 
  	(
  		@idPassport int,
  		@method int,
 		@version nvarchar(50),
  		@credential nvarchar(255),
  		@password nvarchar(1000),
  		@startDate datetime,
  		@expirationDate datetime,
 		@biometricID int,		
 		@timestamp datetime,
 		@enabled bit,
		@LastUpdatePassword datetime,
		@BloquedAccessApp bit,
		@InvalidAccessAttemps int, 
		@LastDateInvalidAccessAttempted datetime 		
 		
  	)
  AS
  	INSERT INTO sysroPassports_AuthenticationMethods (
  		IDPassport,
  		Method,
 		Version,
  		Credential,
  		Password,
  		StartDate,
  		ExpirationDate,
 		BiometricID,
 		BiometricData,
 		TimeStamp,
 		Enabled,
		LastUpdatePassword ,
		BloquedAccessApp ,
		InvalidAccessAttemps , 
		LastDateInvalidAccessAttempted 
 		
  	)
  	VALUES (
  		@idPassport,
  		@method,
 		@version,
  		@credential,
  		@password,
  		@startDate,
  		@expirationDate,
 		@biometricID,
 		null,
 		@timestamp,
 		@enabled,
		@LastUpdatePassword,
		@BloquedAccessApp,
		@InvalidAccessAttemps, 
		@LastDateInvalidAccessAttempted
 		
  	)
  	
  	RETURN
  GO
 
  ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Select] 
  	(
  		@idPassport int,
  		@method int,
 		@version nvarchar(50)
  	)
  AS
  	SELECT IDPassport,
  		Method,
 		Version,
  		Credential,
  		Password,
  		StartDate,
  		ExpirationDate,
 		BiometricID,
 		BiometricData,
 		TimeStamp,
 		Enabled,
		LastUpdatePassword,
		BloquedAccessApp ,
		InvalidAccessAttemps, 
		LastDateInvalidAccessAttempted
 		
  	FROM sysroPassports_AuthenticationMethods
  	WHERE IDPassport = @idPassport AND
 		  (@method IS NULL OR Method = @method) AND
 		  (@version IS NULL OR Version = @version)
  	
  	RETURN
GO

  ALTER PROCEDURE [dbo].[WebLogin_Passports_AuthenticationMethods_Update]
  	(
  		@idPassport int,
  		@method int,
 		@version nvarchar(50),
  		@credential nvarchar(255),
  		@password nvarchar(1000),
  		@startDate datetime,
  		@expirationDate datetime,
 		@biometricID int,		
 		@timestamp datetime,
 		@enabled bit,
		@LastUpdatePassword datetime,
		@BloquedAccessApp bit,
		@InvalidAccessAttemps int, 
		@LastDateInvalidAccessAttempted datetime 		
 		
  	)
  AS
  	UPDATE sysroPassports_AuthenticationMethods SET
  		Credential = @credential,
  		Password = @password,
  		StartDate = @startDate,
  		ExpirationDate = @expirationDate,		
 		TimeStamp = @timestamp,
 		Enabled = @enabled,
		LastUpdatePassword = @LastUpdatePassword ,
		BloquedAccessApp = @BloquedAccessApp ,
		InvalidAccessAttemps = @InvalidAccessAttemps , 
		LastDateInvalidAccessAttempted = @LastDateInvalidAccessAttempted  		
 		
  	WHERE IDPassport = @idPassport AND
  		Method = @method AND
 		Version = @version AND
 		BiometricID = @biometricID
  	
  	RETURN
GO
  	

--PERMISOS DE APLICACIONES PERMITIDAS SOBRE EMPLEADOS
INSERT INTO [dbo].[sysroFeatures] ([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees])
VALUES (1580,1,'Employees.AllowedApplications','Aplicaciones permitidas','','U','RWA',NULL)
GO

INSERT INTO sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT sysroPassports.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE 
WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, sysroPassports		
WHERE sysroFeatures.ID IN (1580) AND sysroPassports.IDParentPassport IS NULL AND sysroPassports.GroupType = 'U'
GO

-- nuevas notificaciones para enviar mail de cuentas bloqueadas temporalmente o indefinidamente
INSERT INTO dbo.sysroNotificationTypes VALUES(36,'Advice for temporary blocked account',null, 1)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1900, 36, 'Aviso de cuenta bloqueada temporalmente','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO

INSERT INTO dbo.sysroNotificationTypes VALUES(37,'Advice for permanent blocked account',null, 1)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1901, 37, 'Aviso de cuenta bloqueada indefinidamente','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO

-- nueva notificacion para enviar mail de clave temporal
INSERT INTO dbo.sysroNotificationTypes VALUES(38,'Advice for validation code',null, 1)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1902, 38, 'Aviso de código de validación','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO

-- nueva notificacion para enviar mail de nueva contraseña
INSERT INTO dbo.sysroNotificationTypes VALUES(39,'Advice for new password',null, 1)
GO

INSERT INTO dbo.Notifications (ID,IDTYPE,NAME,CONDITION,DESTINATION,ACTIVATED, CREATORID, AllowPortal) VALUES(1903, 39, 'Aviso de nueva contraseña','<?xml version="1.0"?><roCollection version="2.0"><Item key="MailListUserfield" type="8"></Item><Item key="DatePeriodUserfield" type="8"></Item><Item key="TargetTypeConcept" type="8"></Item><Item key="TargetConcept" type="8"></Item></roCollection>','<?xml version="1.0"?><roCollection version="2.0"></roCollection>', 1, 'SYSTEM',0)
GO


-- modificar stored para que al eliminar passport elimine registros de las tablas relacionadas de seguridad
 ALTER PROCEDURE [dbo].[WebLogin_Passports_Delete] 
 	(
 		@id int
 	)
 AS
 	SET NOCOUNT ON
 	
 	DELETE FROM sysroSecurityParameters
 	WHERE IDPassport = @id
 	 	
 	DELETE FROM sysroPassports_PasswordHistory
 	WHERE IDPassport = @id
 	
	DELETE FROM sysroPassports_AuthorizedAdress
 	WHERE IDPassport = @id

 	DELETE FROM sysroPassports
 	WHERE ID = @id
 	
 	SET NOCOUNT OFF
 	
 	RETURN
GO


--MODIFICAR VISTA ANALITICA PRODUCTIV PARA AÑADIR IDCONTRACT
ALTER VIEW [dbo].[sysroTasksCube]
AS
SELECT     dbo.sysroEmployeeGroups.IDGroup, dbo.sysroEmployeeGroups.FullGroupName AS GroupName, dbo.Employees.ID AS IDEmployee, 
                      dbo.Employees.Name AS EmployeeName, DATEPART(year, dbo.DailyTaskAccruals.Date) AS Date_Year, DATEPART(month, dbo.DailyTaskAccruals.Date) 
                      AS Date_Month, DATEPART(day, dbo.DailyTaskAccruals.Date) AS Date_Day, dbo.DailyTaskAccruals.Date, dbo.BusinessCenters.ID AS IDCenter, 
                      dbo.BusinessCenters.Name AS CenterName, dbo.Tasks.ID AS IDTask, dbo.Tasks.Name AS TaskName, dbo.Tasks.InitialTime, ISNULL(dbo.Tasks.Field1, '') 
                      AS Field1_Task, ISNULL(dbo.Tasks.Field2, '') AS Field2_Task, ISNULL(dbo.Tasks.Field3, '') AS Field3_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field4), 0) 
                      AS Field4_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field5), 0) AS Field5_Task, ISNULL(CONVERT(numeric(25, 6), dbo.Tasks.Field6), 0) AS Field6_Task, 
                      ISNULL(dbo.DailyTaskAccruals.Field1, '') AS Field1_Total, ISNULL(dbo.DailyTaskAccruals.Field2, '') AS Field2_Total, ISNULL(dbo.DailyTaskAccruals.Field3, '') 
                      AS Field3_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field4), 0) AS Field4_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field5), 
                      0) AS Field5_Total, ISNULL(CONVERT(numeric(25, 6), dbo.DailyTaskAccruals.Field6), 0) AS Field6_Total, ISNULL(CONVERT(numeric(25, 6), 
                      dbo.DailyTaskAccruals.Value), 0) AS Value, ISNULL(dbo.Tasks.Project, '') AS Project, ISNULL(dbo.Tasks.Tag, '') AS Tag, dbo.Tasks.IDPassport, 
                      ISNULL(dbo.Tasks.TimeChangedRequirements, 0) AS TimeChangedRequirements, ISNULL(dbo.Tasks.ForecastErrorTime, 0) AS ForecastErrorTime, 
                      ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) AS NonProductiveTimeIncidence, ISNULL(dbo.Tasks.EmployeeTime, 0) AS EmployeeTime, 
                      ISNULL(dbo.Tasks.TeamTime, 0) AS TeamTime, ISNULL(dbo.Tasks.MaterialTime, 0) AS MaterialTime, ISNULL(dbo.Tasks.OtherTime, 0) AS OtherTime, 
                      ISNULL(dbo.Tasks.InitialTime, 0) + ISNULL(dbo.Tasks.TimeChangedRequirements, 0) + ISNULL(dbo.Tasks.ForecastErrorTime, 0) 
                      + ISNULL(dbo.Tasks.NonProductiveTimeIncidence, 0) + ISNULL(dbo.Tasks.EmployeeTime, 0) + ISNULL(dbo.Tasks.TeamTime, 0) + ISNULL(dbo.Tasks.MaterialTime, 0) 
                      + ISNULL(dbo.Tasks.OtherTime, 0) AS Duration, 
                      CASE WHEN Tasks.Status = 0 THEN 'Activa' ELSE CASE WHEN Tasks.Status = 1 THEN 'Finalizada' ELSE CASE WHEN Tasks.Status = 2 THEN 'Cancelada' ELSE 'Pendiente'
                       END END END AS Estado, dbo.EmployeeContracts.IDContract
FROM         dbo.Employees INNER JOIN
                      dbo.DailyTaskAccruals ON dbo.DailyTaskAccruals.IDEmployee = dbo.Employees.ID INNER JOIN
                      dbo.Tasks ON dbo.Tasks.ID = dbo.DailyTaskAccruals.IDTask INNER JOIN
                      dbo.sysroEmployeeGroups ON dbo.sysroEmployeeGroups.IDEmployee = dbo.DailyTaskAccruals.IDEmployee INNER JOIN
                      dbo.BusinessCenters ON dbo.Tasks.IDCenter = dbo.BusinessCenters.ID INNER JOIN
                      dbo.EmployeeContracts ON dbo.DailyTaskAccruals.IDEmployee = dbo.EmployeeContracts.IDEmployee AND 
                      dbo.DailyTaskAccruals.Date >= dbo.EmployeeContracts.BeginDate AND dbo.DailyTaskAccruals.Date <= dbo.EmployeeContracts.EndDate
WHERE     (dbo.DailyTaskAccruals.Date BETWEEN dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeeGroups.EndDate)
GO

-- Grupo Consultores
if NOT exists (select * from dbo.sysroPassports where Description = '@@ROBOTICS@@Consultores')
BEGIN
INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(NULL,'U','Robotics Consultores','@@ROBOTICS@@Consultores','',NULL,NULL,0,NULL)
DECLARE @IDGROUP int
SET @IDGROUP = @@IDENTITY

INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDGROUP AS varchar(10)),NULL,'',NULL)
DECLARE @IDUser int
SET @IDUser = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUser WHERE ID = @IDGROUP

-- Admin en todo menos usuarios y opciones  de seguridad de lectura
INSERT INTO dbo.sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT @IDGROUP AS IDPassport, dbo.sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM dbo.sysroFeatures  
WHERE Alias <> 'Administration.Security' AND Alias <> 'Administration.SecurityOptions' 

INSERT INTO dbo.sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT @IDGROUP AS IDPassport, dbo.sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 3 ELSE 3 END END
FROM dbo.sysroFeatures  
WHERE Alias = 'Administration.Security' or  Alias = 'Administration.SecurityOptions' 


INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT @IDGROUP, Groups.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, Groups
WHERE sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID) AND Alias <> 'Administration.Security' AND Alias <> 'Administration.SecurityOptions' 

INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT @IDGROUP, Groups.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 3 ELSE 3 END END
FROM sysroFeatures, Groups
WHERE sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID) AND (Alias = 'Administration.Security' OR Alias = 'Administration.SecurityOptions') 


-- Usuario Consultor
INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(@IDGROUP,'','Consultor','@@ROBOTICS@@Consultor','',NULL,NULL,0,NULL)
DECLARE @IDPASSPORT int
SET @IDPASSPORT = @@IDENTITY

INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDPASSPORT AS varchar(10)),NULL,'',NULL)
DECLARE @IDUserPassport int
SET @IDUserPassport = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUserPassport WHERE ID = @IDPASSPORT
INSERT INTO [sysroPassports_AuthenticationMethods] ([IDPassport],[Method],[Credential],[Password],[StartDate],[ExpirationDate])VALUES(@IDPASSPORT,1,'Consultor','kXU/5oA7vqfvgvzAS/Mtng==',NULL,NULL)


-- Grupo Tecnico
INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(NULL,'U','Robotics Tecnicos','@@ROBOTICS@@Tecnicos','',NULL,NULL,0,NULL)
SET @IDGROUP = @@IDENTITY

INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDGROUP AS varchar(10)),NULL,'',NULL)
SET @IDUser = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUser WHERE ID = @IDGROUP

-- permisos de Administracion en Empleados/Accesos/Terminales y lectura en el resto
INSERT INTO dbo.sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT @IDGROUP AS IDPassport, dbo.sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM dbo.sysroFeatures  WHERE Alias = 'Access' or Alias LIKE 'Access.%' OR Alias LIKE 'Employees.%' OR Alias = 'Employees' OR Alias LIKE 'Terminals.%'  OR Alias = 'Terminals'  

-- El resto solo lectura
INSERT INTO dbo.sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT @IDGROUP AS IDPassport, dbo.sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 3 ELSE 3 END END
FROM dbo.sysroFeatures  WHERE Alias <> 'Access' and Alias not LIKE 'Access.%' and Alias not LIKE 'Employees.%' and Alias <> 'Employees' and Alias not  LIKE 'Terminals.%'  and Alias <> 'Terminals'  


INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT @IDGROUP, Groups.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 6 ELSE 9 END END
FROM sysroFeatures, Groups
WHERE sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID)  AND (Alias = 'Access' or Alias LIKE 'Access.%' OR Alias LIKE 'Employees.%' OR Alias = 'Employees' OR Alias LIKE 'Terminals.%'  OR Alias = 'Terminals'  )

INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT @IDGROUP, Groups.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 3 ELSE 3 END END
FROM sysroFeatures, Groups
WHERE sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID)  AND (Alias <> 'Access' and Alias not LIKE 'Access.%' and Alias not LIKE 'Employees.%' and Alias <> 'Employees' and Alias not  LIKE 'Terminals.%'  and Alias <> 'Terminals'   )


-- Usuario Tecnico
INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(@IDGROUP,'','Tecnico','@@ROBOTICS@@Tecnico','',NULL,NULL,0,NULL)
SET @IDPASSPORT = @@IDENTITY

INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDPASSPORT AS varchar(10)),NULL,'',NULL)
SET @IDUserPassport = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUserPassport WHERE ID = @IDPASSPORT
INSERT INTO [sysroPassports_AuthenticationMethods] ([IDPassport],[Method],[Credential],[Password],[StartDate],[ExpirationDate])VALUES(@IDPASSPORT,1,'Tecnico','FyKusWBDeEw=',NULL,NULL)



-- Grupo Soporte
INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(NULL,'U','Robotics Soporte','@@ROBOTICS@@Soporte','',NULL,NULL,0,NULL)
SET @IDGROUP = @@IDENTITY

INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDGROUP AS varchar(10)),NULL,'',NULL)
SET @IDUser = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUser WHERE ID = @IDGROUP

-- permisos de lectura en todo
INSERT INTO dbo.sysroPassports_PermissionsOverFeatures (IDPassport, IDFeature, Permission)
SELECT @IDGROUP AS IDPassport, dbo.sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 3 ELSE 3 END END
FROM dbo.sysroFeatures  

INSERT INTO sysroPassports_PermissionsOverGroups (IDPassport, IDGroup, IDApplication, Permission)
SELECT @IDGROUP, Groups.ID, sysroFeatures.ID, CASE WHEN sysroFeatures.PermissionTypes = 'R' THEN 3 ELSE CASE WHEN sysroFeatures.PermissionTypes = 'RW' THEN 3 ELSE 3 END END
FROM sysroFeatures, Groups
WHERE sysroFeatures.AppHasPermissionsOverEmployees = 1 AND Groups.Path = CONVERT(varchar, Groups.ID)  

-- Usuario Soporte
INSERT INTO [sysroPassports] ([IDParentPassport],[GroupType],[Name],[Description],[Email],[IDUser],[IDEmployee],[IDLanguage],[LevelOfAuthority])
VALUES(@IDGROUP,'','Soporte','@@ROBOTICS@@Soporte','',NULL,NULL,0,NULL)
SET @IDPASSPORT = @@IDENTITY

INSERT INTO [sysroUsers] ([Login],[Password],[IDSecurityGroup],[UserConfData])VALUES('´WebLogin´ : ' + CAST(@IDPASSPORT AS varchar(10)),NULL,'',NULL)
SET @IDUserPassport = @@IDENTITY

UPDATE sysroPassports SET IDUser = @IDUserPassport WHERE ID = @IDPASSPORT
INSERT INTO [sysroPassports_AuthenticationMethods] ([IDPassport],[Method],[Credential],[Password],[StartDate],[ExpirationDate])VALUES(@IDPASSPORT,1,'Soporte','qmz8cgX7Q8c=',NULL,NULL)
END
GO

-- campo nuevo de notificaciones
ALTER TABLE Notifications
ADD AllowMail [bit] NULL DEFAULT(0)
GO

UPDATE Notifications SET AllowMail=0 WHERE AllowMail Is null
GO

ALTER TABLE dbo.sysroNotificationTypes ADD Feature [nvarchar](MAX) default ''
GO
UPDATE dbo.sysroNotificationTypes Set Feature = '' WHERE Feature IS NULL
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees.Contract' WHERE ID in(1,2)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Punches' WHERE ID in(3,4,5,6,7)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Punches.Punches' WHERE ID in(8)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Access' WHERE ID in(9)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Terminals.Definition' WHERE ID in(11)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Scheduler' WHERE ID in(13,14)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar' WHERE ID in(15)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees.UserFields' WHERE ID in(16,17)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Punches.Punches' WHERE ID in(19,20,35)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.JustifyIncidences' WHERE ID in(21)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees.IdentifyMethods' WHERE ID in(22)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Tasks.Definition' WHERE ID in(23,24,25,26,29,30)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Requests.Vacations' WHERE ID in(27)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Calendar.Requests.ShiftChange' WHERE ID in(28)
GO
UPDATE dbo.sysroNotificationTypes Set Feature = 'Employees' WHERE ID in(32,33,34,18)
GO
ALTER TABLE dbo.sysroNotificationTypes ADD FeatureType [nvarchar](MAX) default ''
GO
UPDATE dbo.sysroNotificationTypes Set FeatureType = '' WHERE FeatureType IS NULL
GO
UPDATE dbo.sysroNotificationTypes Set FeatureType = 'E' WHERE ID in(1,2,3,4,5,6,7,8,13,14,15,16,17,19,20,21,22,27,28,34,18,32,33,35,9)
GO

DELETE FROM sysroGUI WHERE IDPath ='Portal\Configuration\AttendanceOptions'
GO

INSERT INTO sysroGUI ([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities])
   VALUES ('Portal\ShiftControl\AttendanceOptions','AttendanceOptions','Options/AttendanceOptions.aspx','AttendanceOptions.png',NULL,NULL,NULL,NULL,670,'NWR','U:Administration.Options.Attendance=Read OR U:Employees.UserFields.Definition=Read')
GO

 
CREATE FUNCTION [dbo].[GetFullFeaturePath] (@FeatureId int)
  	RETURNS varchar(300) 
 AS
 BEGIN
	declare @parentID int; 
  	declare @path varchar(200);
  	declare @resultado varchar(500);
  	set @resultado=''
  	-- Seleccionamos el Path del grupo que nos manda el usuario
  	select @path=Alias,@parentID=IDParent from sysroFeatures where ID=str(@FeatureId)
  	
  	select @resultado = @path
  	while (@parentID is not null)
  	begin
  		select @resultado = '\' + @resultado 
  		select @path=Alias,@parentID=IDParent from sysroFeatures where ID=str(@parentID)
  		
  		select @resultado = @path + @resultado
  	end
  	
  	return (@resultado)
 END
GO

CREATE FUNCTION [dbo].[GetMainFeaturePath] (@FeatureId int)
  	RETURNS varchar(300) 
 AS
 BEGIN
	declare @parentID int; 
  	declare @path varchar(200);
  	declare @resultado varchar(500);
  	set @resultado=''
  	-- Seleccionamos el Path del grupo que nos manda el usuario
  	select @path=Alias,@parentID=IDParent from sysroFeatures where ID=str(@FeatureId)
  	
  	select @resultado = @path 
  	while (@parentID is not null)
  	begin 
  		select @path=Alias,@parentID=IDParent from sysroFeatures where ID=str(@parentID)
  		
  		select @resultado = @path 
  	end
  	
  	return (@resultado)
 END
GO


CREATE FUNCTION [dbo].[GetFeatureNextLevelByEmployee]
 (	
   	@idEmployee int,
   	@employeeFeatureID int, 
   	@featureAlias nvarchar(100)
 )
 RETURNS int
 AS
 BEGIN
   	DECLARE @NextLevel int
   	SELECT @NextLevel = 
   	(SELECT MAX(dbo.GetPassportLevelOfAuthority(sysroPassports.ID))
   	FROM sysroPassports  
   	WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND 
   	      dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) > 3 AND 
		  dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3   	      
   	      )
   	   		   
   	RETURN @NextLevel
 END

GO

 CREATE FUNCTION [dbo].[GetFeatureNextLevelPassportsByEmployee]
 (	
   	@idEmployee int,
   	@featureAlias nvarchar(100),
   	@employeeFeatureId int
 )
 RETURNS nvarchar(1000)
 AS
 BEGIN
   	DECLARE @RetNames nvarchar(1000), 
   			@PassportName nvarchar(50),
   			@nextFeatureLevel int
   	SET @RetNames = ''
   	
   	select @nextFeatureLevel = (select dbo.GetFeatureNextLevelByEmployee(@idEmployee,@employeeFeatureId, @featureAlias))
   	DECLARE PassportsCursor CURSOR
 		FOR  SELECT sysroPassports.Name 
   			 FROM sysroPassports 
   			 INNER JOIN (
   			 SELECT srp.ID, dbo.GetPassportLevelOfAuthority(srp.ID) AS Level, 
   			 dbo.WebLogin_GetPermissionOverFeature(srp.ID, @featureAlias,'U',2) AS FeaturePermission,
   			 dbo.WebLogin_GetPermissionOverEmployee(srp.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) AS EmployeePermission 
   			 FROM sysroPassports srp
   				inner join sysroPassports pVis on pVis.ID = srp.ID and srp.GroupType <> 'U' AND srp.IDUser is not null and srp.Description not like '%@@ROBOTICS@@%'
   			 ) gpl
   			 on sysroPassports.ID=gpl.ID 
   			 WHERE gpl.Level = @nextFeatureLevel
 			 AND gpl.FeaturePermission > 3 AND gpl.EmployeePermission > 3
   			 ORDER BY sysroPassports.Name
   	OPEN PassportsCursor
   	FETCH NEXT FROM PassportsCursor
   	INTO @PassportName
   	WHILE @@FETCH_STATUS = 0
   	BEGIN	
   	
   		IF @RetNames = ''
   			BEGIN
   				SET @RetNames = @PassportName
   			END
   		ELSE
   			BEGIN
   				SET @RetNames = @RetNames + ' / '+ @PassportName
   			END
   		FETCH NEXT FROM PassportsCursor 
   		INTO @PassportName
   	END 
   	CLOSE PassportsCursor
   	DEALLOCATE PassportsCursor
   	RETURN @RetNames
 END
GO


update sysroFeatures set EmployeeFeatureID = 2 where dbo.GetMainFeaturePath(ID) = 'Calendar'
GO
update sysroFeatures set EmployeeFeatureID = 9 where dbo.GetMainFeaturePath(ID) = 'Access'
GO
update sysroFeatures set EmployeeFeatureID = 1 where dbo.GetMainFeaturePath(ID) = 'Employees'
GO
update sysroFeatures set EmployeeFeatureID = 11 where dbo.GetMainFeaturePath(ID) = 'LabAgree'
GO
update sysroFeatures set EmployeeFeatureID = 25 where dbo.GetMainFeaturePath(ID) = 'Tasks'
GO


CREATE FUNCTION [dbo].[GetRequestNextLevelPassportsIDs]
 (	
   	@idRequest int	
 )
 RETURNS nvarchar(1000)
 AS
 BEGIN
   	DECLARE @RetNames nvarchar(1000), 
   			@PassportID int
   	SET @RetNames = ''
   	DECLARE PassportsCursor CURSOR
 		FOR  SELECT sysroPassports.ID 
   			 FROM sysroPassports 
   			 INNER JOIN (SELECT id, dbo.GetPassportLevelOfAuthority(ID) AS Level FROM sysroPassports) gpl
   			 on sysroPassports.ID=gpl.ID 
   			 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null and sysroPassports.Description not like '%@@ROBOTICS@@%'
   			 AND gpl.level=dbo.GetRequestNextLevel(@idRequest)
 			 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 3
   			 ORDER BY sysroPassports.Name
   	OPEN PassportsCursor
   	FETCH NEXT FROM PassportsCursor
   	INTO @PassportID
   	WHILE @@FETCH_STATUS = 0
   	BEGIN	
   	
   		IF @RetNames = ''
   			BEGIN
   				SET @RetNames = CONVERT(nvarchar(10),@PassportID)
   			END
   		ELSE
   			BEGIN
   				SET @RetNames = @RetNames + ','+ CONVERT(nvarchar(10),@PassportID)
   			END
   		FETCH NEXT FROM PassportsCursor 
   		INTO @PassportID
   	END 
   	CLOSE PassportsCursor
   	DEALLOCATE PassportsCursor
   	RETURN @RetNames
 END
GO

ALTER FUNCTION [dbo].[GetRequestPassportPermission]
  (
  	@idPassport int,
   	@idRequest int	
  )
  RETURNS int
  AS
  BEGIN
   	DECLARE @FeatureAlias nvarchar(100),
   			@EmployeefeatureID int,
   			@idEmployee int,
  			@RequestType int
  			
   	SELECT @RequestType = Requests.RequestType,
  		   @idEmployee = Requests.IDEmployee
     FROM Requests
  	WHERE Requests.ID = @idRequest
  	
  	SELECT @featureAlias = Alias, 
  		   @EmployeefeatureID = EmployeeFeatureId 
  	FROM dbo.sysroFeatures 
  	WHERE sysroFeatures.AliasId = @RequestType
   	
   	
   	DECLARE @Permission int
   	SET @Permission = (select dbo.WebLogin_GetPermissionOverFeature(@idPassport,@FeatureAlias, 'U', 2)) 
   	
   	IF @Permission > 0 
   		BEGIN
   			SET @Permission = (select dbo.WebLogin_GetPermissionOverEmployee(@idPassport,@idEmployee,@EmployeefeatureID,2,1,getdate()))
   		END
   		   
   	RETURN @Permission
END
  
GO

ALTER FUNCTION [dbo].[GetRequestLevelsBelow]
  	(
  	@idPassport int,
  	@idRequest int
  	)
  RETURNS int
  AS
  BEGIN
  	DECLARE @LevelsBelow int,
  			@LevelOfAuthority int,
     		@featureAlias nvarchar(100),
  			@EmployeefeatureID int,
  			@idEmployee int,
  			@RequestLevel int,
  			@RequestType int
  	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
  	
  	SELECT @RequestType = Requests.RequestType,
  		   @idEmployee = Requests.IDEmployee,
  		   @RequestLevel = ISNULL(Requests.StatusLevel, 11)
     FROM Requests
  	WHERE Requests.ID = @idRequest
  	
  	SELECT @featureAlias = Alias, 
  		   @EmployeefeatureID = EmployeeFeatureId 
  	FROM dbo.sysroFeatures 
  	WHERE sysroFeatures.AliasId = @RequestType
  	
  	 	
  	/* Obtiene el número de niveles que hay entre el nivel del validador (@idPassport) y el nivel actual de la solicitud (@RequestLevel) */
  	SELECT @LevelsBelow = 
  		(SELECT COUNT( DISTINCT dbo.GetPassportLevelOfAuthority(sysroPassports.ID))
  		 FROM sysroPassports INNER JOIN 
  		 (SELECT Id, dbo.GetPassportLevelOfAuthority(ID) AS value FROM sysroPassports WHERE ID in(select DISTINCT(IDPassport) FROM dbo.GetAllPassportParentsRequestPermissions() where RequestPermissionCount > 0 )) gpla
  		 ON gpla.ID = sysroPassports.id 
  		 WHERE sysroPassports.GroupType <> 'U' AND gpla.value > @LevelOfAuthority AND gpla.value <= @RequestLevel AND
		 sysroPassports.Description not like '%@@ROBOTICS@@%' AND
  		 (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 3 AND
  		 (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 3)
     	IF @RequestLevel = 11  SET @LevelsBelow = @LevelsBelow + 1
  	
  	RETURN @LevelsBelow
END
GO
  
ALTER FUNCTION [dbo].[GetRequestNextLevelPassports]
 (	
   	@idRequest int	
 )
 RETURNS nvarchar(1000)
 AS
 BEGIN
   	DECLARE @RetNames nvarchar(1000), 
   			@PassportName nvarchar(50)
   	SET @RetNames = ''
   	DECLARE PassportsCursor CURSOR
 		FOR  SELECT sysroPassports.Name 
   			 FROM sysroPassports 
   			 INNER JOIN (SELECT id, dbo.GetPassportLevelOfAuthority(ID) AS Level FROM sysroPassports) gpl
   			 on sysroPassports.ID=gpl.ID 
   			 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
   			 AND sysroPassports.Description not like '%@@ROBOTICS@@%'
   			 AND gpl.level=dbo.GetRequestNextLevel(@idRequest)
 			 AND dbo.GetRequestPassportPermission(sysroPassports.ID, @idRequest) > 3
   			 ORDER BY sysroPassports.Name
   	OPEN PassportsCursor
   	FETCH NEXT FROM PassportsCursor
   	INTO @PassportName
   	WHILE @@FETCH_STATUS = 0
   	BEGIN	
   	
   		IF @RetNames = ''
   			BEGIN
   				SET @RetNames = @PassportName
   			END
   		ELSE
   			BEGIN
   				SET @RetNames = @RetNames + ', '+ @PassportName
   			END
   		FETCH NEXT FROM PassportsCursor 
   		INTO @PassportName
   	END 
   	CLOSE PassportsCursor
   	DEALLOCATE PassportsCursor
   	RETURN @RetNames
 END
GO
  
ALTER FUNCTION [dbo].[GetRequestNextLevel]
 (	
   	@idRequest int	
 )
 RETURNS int
 AS
 BEGIN
   	DECLARE @NextLevel int
   	SELECT @NextLevel = 
   	(SELECT MAX(dbo.GetPassportLevelOfAuthority(Parents.ID))
   	FROM sysroPassports  INNER JOIN sysroPassports Parents
 		ON sysroPassports.IDParentPassport = Parents.ID
   	WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null 
   		  AND sysroPassports.Description not like '%@@ROBOTICS@@%' AND 
   	      dbo.GetPassportLevelOfAuthority(Parents.ID) < ISNULL(Requests.StatusLevel, 11) AND
   	      dbo.GetRequestPassportPermission(sysroPassports.ID, Requests.ID) > 3)
   	FROM Requests
   	WHERE Requests.ID = @idRequest 
   		   
   	RETURN @NextLevel
 END
GO
 
ALTER FUNCTION [dbo].[GetRequestMinStatusLevel]
 (
 	@idPassport int,
 	@featureAlias nvarchar(100),
 	@EmployeefeatureID int,
 	@idEmployee int
 )
 RETURNS int
 AS
 BEGIN
 	/* While looking only at permissions defined directly on passport,
   	returns the first permission found in the employees groups hierarchy */
   	DECLARE @MinStatusLevel int
   	DECLARE @LevelOfAuthority int
  	
  	SELECT @LevelOfAuthority = dbo.GetPassportLevelOfAuthority(@idPassport)
   	
  	SELECT @MinStatusLevel = 
  	(SELECT TOP 1 Parents.LevelOfAuthority
  	 FROM sysroPassports INNER JOIN sysroPassports Parents
  			ON sysroPassports.IDParentPassport = Parents.ID
  	 WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null AND
  	      sysroPassports.Description not like '%@@ROBOTICS@@%' AND 
  	 	  dbo.GetPassportLevelOfAuthority(Parents.ID) > @LevelOfAuthority AND
  		  (select dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID,@FeatureAlias, 'U', 2)) > 3 AND
  		  (select dbo.WebLogin_GetPermissionOverEmployee(sysroPassports.ID,@idEmployee,@EmployeefeatureID,2,1,getdate())) > 3 
  	 ORDER BY Parents.LevelOfAuthority ASC)
   	
 	RETURN @MinStatusLevel
 END
GO

CREATE FUNCTION [dbo].[GetSupervisedEmployeesByPassport]
 	(
 		@idPassport int,
 		@featureAlias nvarchar(50)
 	)
 RETURNS @result table (ID int PRIMARY KEY)
  AS
  BEGIN
 	DECLARE @EmployeeID int
  	DECLARE @SupervisorLevel int
  	DECLARE @featureEmployeeID int
  	DECLARE @featurePermission int
  	DECLARE @tmpPermissions table (PassportID int, EmployeeID int, PassportLevelOfAuthority int, EmployeePermission int)
  	DECLARE @tmpMaxFeatureWithPermissionByEmployee table(EmployeeID int, PassportLevelOfAuthority int)
  	
  	SELECT @SupervisorLevel = (SELECT dbo.GetPassportLevelOfAuthority(@idPassport))
  	SELECT @featureEmployeeID = (SELECT EmployeeFeatureID from sysroFeatures where Alias = @featureAlias)
  	SELECT @featurePermission = (dbo.WebLogin_GetPermissionOverFeature(@idPassport, @featureAlias,'U',2))
  	
  	IF @featurePermission > 3
  		BEGIN
  			INSERT INTO @tmpPermissions(PassportID, EmployeeID,PassportLevelOfAuthority,EmployeePermission) 
 				select supPassports.ID, empPassports.IDEmployee,dbo.GetPassportLevelOfAuthority(supPassports.ID),
 						dbo.WebLogin_GetPermissionOverEmployee(supPassports.ID,empPassports.IDEmployee,@featureEmployeeID,0,1,getdate())
 						from sysroPassports supPassports, sysroPassports empPassports
 						where supPassports.GroupType = 'U' and supPassports.Description NOT LIKE '@@ROBOTICS@@%' and
 							  empPassports.IDEmployee is not null
 			
 			INSERT INTO @tmpMaxFeatureWithPermissionByEmployee(EmployeeID,PassportLevelOfAuthority)
 				select EmployeeID, MAX(PassportLevelOfAuthority) from @tmpPermissions 
 					where EmployeePermission > 3 group by EmployeeID
 			
 			DECLARE PassportsCursor CURSOR
 				FOR
 					SELECT EmployeeID
					FROM  	
					(select sysroPassports.ID, tPerm.EmployeeID, tPerm.PassportLevelOfAuthority as SupervisorLevel 
						from sysroPassports 
							inner join (
								select perm.* from @tmpPermissions perm inner join @tmpMaxFeatureWithPermissionByEmployee empPerm
								on perm.EmployeeID = empPerm.EmployeeID and empPerm.PassportLevelOfAuthority = perm.PassportLevelOfAuthority
							) tPerm
						on sysroPassports.IDParentPassport = tPerm.PassportID and tPerm.EmployeePermission > 3 and tPerm.PassportLevelOfAuthority = @SupervisorLevel
						where  sysroPassports.ID = @idPassport 
					) pasPerm
 		 			
 			OPEN PassportsCursor
 			FETCH NEXT FROM PassportsCursor
 			INTO @EmployeeID
 			WHILE @@FETCH_STATUS = 0
 			BEGIN	
 				INSERT INTO @result VALUES (@EmployeeID)
 		   		
 				FETCH NEXT FROM PassportsCursor 
 				INTO @EmployeeID
 			END 
 			CLOSE PassportsCursor
 			DEALLOCATE PassportsCursor	
 		END
    		
  	RETURN
 END
GO

 CREATE FUNCTION [dbo].[GetFeatureNextLevel]
 (	
	@featureAlias nvarchar(100)
 )
 RETURNS int
 AS
 BEGIN
   	DECLARE @NextLevel int
   	SELECT @NextLevel = 
   	(SELECT MAX(dbo.GetPassportLevelOfAuthority(sysroPassports.ID))
   	FROM sysroPassports  
   	WHERE sysroPassports.GroupType <> 'U' AND sysroPassports.IDUser is not null and
   	dbo.WebLogin_GetPermissionOverFeature(sysroPassports.ID, @featureAlias,'U',2) > 3)
   	   		   
   	RETURN @NextLevel
 END
GO


CREATE FUNCTION [dbo].[GetFeatureNextLevelPassportsIDs]
 (	
   	@featureAlias nvarchar(100)   	
 )
 RETURNS nvarchar(1000)
 AS
 BEGIN
   	DECLARE @RetNames nvarchar(1000), 
   			@PassportID int,
   			@nextFeatureLevel int
   	SET @RetNames = ''
   	
   	select @nextFeatureLevel = (select dbo.GetFeatureNextLevel(@featureAlias))
   	DECLARE PassportsCursor CURSOR
 		FOR  SELECT sysroPassports.ID
   			 FROM sysroPassports 
   			 INNER JOIN (
   			 SELECT srp.ID, dbo.GetPassportLevelOfAuthority(srp.ID) AS Level, 
   			 dbo.WebLogin_GetPermissionOverFeature(srp.ID, @featureAlias,'U',2) AS FeaturePermission
   			 FROM sysroPassports srp
   				inner join sysroPassports pVis on pVis.ID = srp.ID and srp.GroupType <> 'U' AND srp.IDUser is not null and srp.Description not like '%@@ROBOTICS@@%'
   			 ) gpl
   			 on sysroPassports.ID=gpl.ID 
   			 WHERE gpl.Level = @nextFeatureLevel
 			 AND gpl.FeaturePermission > 3 
   			 ORDER BY sysroPassports.Name
   	OPEN PassportsCursor
   	FETCH NEXT FROM PassportsCursor
   	INTO @PassportID
   	WHILE @@FETCH_STATUS = 0
   	BEGIN	
   	
   		IF @RetNames = ''
   			BEGIN
   				SET @RetNames =  convert(nvarchar(10),@PassportID)
   			END
   		ELSE
   			BEGIN
   				SET @RetNames = @RetNames + ','+ convert(nvarchar(10),@PassportID)
   			END
   		FETCH NEXT FROM PassportsCursor 
   		INTO @PassportID
   	END 
   	CLOSE PassportsCursor
   	DEALLOCATE PassportsCursor
   	RETURN @RetNames
 END
GO
 
CREATE FUNCTION [dbo].[GetFeatureNextLevelPassportsIDsByEmployee]
 (	
   	@idEmployee int,
   	@featureAlias nvarchar(100),
   	@employeeFeatureId int
 )
 RETURNS nvarchar(1000)
 AS
 BEGIN
   	DECLARE @RetNames nvarchar(1000), 
   			@PassportID int,
   			@nextFeatureLevel int
   	SET @RetNames = ''
   	
   	select @nextFeatureLevel = (select dbo.GetFeatureNextLevelByEmployee(@idEmployee,@employeeFeatureId, @featureAlias))
   	DECLARE PassportsCursor CURSOR
 		FOR  SELECT sysroPassports.ID
   			 FROM sysroPassports 
   			 INNER JOIN (
   			 SELECT srp.ID, dbo.GetPassportLevelOfAuthority(srp.ID) AS Level, 
   			 dbo.WebLogin_GetPermissionOverFeature(srp.ID, @featureAlias,'U',2) AS FeaturePermission,
   			 dbo.WebLogin_GetPermissionOverEmployee(srp.ID,@idEmployee,@employeeFeatureID,2,1,getdate()) AS EmployeePermission 
   			 FROM sysroPassports srp
   				inner join sysroPassports pVis on pVis.ID = srp.ID and srp.GroupType <> 'U' AND srp.IDUser is not null and srp.Description not like '%@@ROBOTICS@@%'
   			 ) gpl
   			 on sysroPassports.ID=gpl.ID 
   			 WHERE gpl.Level = @nextFeatureLevel
 			 AND gpl.FeaturePermission > 3 AND gpl.EmployeePermission > 3
   			 ORDER BY sysroPassports.Name
   	OPEN PassportsCursor
   	FETCH NEXT FROM PassportsCursor
   	INTO @PassportID
   	WHILE @@FETCH_STATUS = 0
   	BEGIN	
   	
   		IF @RetNames = ''
   			BEGIN
   				SET @RetNames =  convert(nvarchar(10),@PassportID)
   			END
   		ELSE
   			BEGIN
   				SET @RetNames = @RetNames + ','+ convert(nvarchar(10),@PassportID)
   			END
   		FETCH NEXT FROM PassportsCursor 
   		INTO @PassportID
   	END 
   	CLOSE PassportsCursor
   	DEALLOCATE PassportsCursor
   	RETURN @RetNames
 END
GO

UPDATE [dbo].[sysroPassports_AuthenticationMethods] SET Enabled = 0 WHERE Credential = 'User'
GO

DECLARE @STRPARAMETERS  nvarchar(MAX)
SET @STRPARAMETERS =  (SELECT DATA FROM dbo.sysroParameters WHERE ID='OPTIONS')
SET @STRPARAMETERS = REPLACE(@STRPARAMETERS,'SessionTimeout" type="2">5','SessionTimeout" type="2">30')
UPDATE dbo.sysroParameters SET Data = @STRPARAMETERS WHERE ID='OPTIONS'
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='313' WHERE ID='DBVersion'
GO

