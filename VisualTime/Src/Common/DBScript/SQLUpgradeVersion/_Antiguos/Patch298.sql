-- Cambio de tipo de campo de la columna fecha en entries
-- /****** Object:  Table [dbo].[ToDelEntries]    Script Date: 02/04/2013 13:26:01 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ToDelEntries]') AND type in (N'U'))
DROP TABLE [dbo].[ToDelEntries]
GO

CREATE TABLE [dbo].[ToDelEntries](
	[DateTime] [smalldatetime] NOT NULL,
	[IDCard] [numeric](28, 0) NOT NULL,
	[IDReader] [tinyint] NOT NULL,
	[Type] [char](1) NOT NULL,
	[IDCause] [smallint] NOT NULL,
	[InvalidRead] [bit] NOT NULL,
	[ID] [numeric](16, 0) NOT NULL,
	[Rdr] [tinyint] NULL,
	[Reliability] [int] NULL,
	[checked] [bit] NOT NULL,
 CONSTRAINT [PK_ToDelEntries] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO


-- /****** Object:  Copiamos el contenido de entries a toDelEntries   Script Date: 02/04/2013 13:26:01 ******/
INSERT INTO [dbo].[ToDelEntries] SELECT * FROM [dbo].[Entries]
GO


-- /****** Object:  Borramos todos los constraits y la tabla Entries   Script Date: 02/04/2013 13:26:01 ******/

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Fichajes_sysroTiposOperaciones]') AND parent_object_id = OBJECT_ID(N'[dbo].[Entries]'))
ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [FK_Fichajes_sysroTiposOperaciones]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Entries_IDCause]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [DF_Entries_IDCause]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Entries_InvalidRead]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [DF_Entries_InvalidRead]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Entries_Rdr]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [DF_Entries_Rdr]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Entries_Reliability]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [DF_Entries_Reliability]
END

GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Entries__checked__6D031153]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Entries] DROP CONSTRAINT [DF__Entries__checked__6D031153]
END

GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Entries]') AND type in (N'U'))
DROP TABLE [dbo].[Entries]
GO

-- /****** Object:  Table [dbo].[Entries]    Script Date: 02/04/2013 13:26:01 ******/
CREATE TABLE [dbo].[Entries](
	[DateTime] [datetime] NOT NULL,
	[IDCard] [numeric](28, 0) NOT NULL,
	[IDReader] [tinyint] NOT NULL,
	[Type] [char](1) NOT NULL,
	[IDCause] [smallint] NOT NULL,
	[InvalidRead] [bit] NOT NULL,
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[Rdr] [tinyint] NULL,
	[Reliability] [int] NULL,
	[checked] [bit] NOT NULL,
 CONSTRAINT [PK_Entries] PRIMARY KEY NONCLUSTERED 
(
	[ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Entries]  WITH NOCHECK ADD  CONSTRAINT [FK_Fichajes_sysroTiposOperaciones] FOREIGN KEY([Type])
REFERENCES [dbo].[sysroEntryTypes] ([Type])
GO

ALTER TABLE [dbo].[Entries] CHECK CONSTRAINT [FK_Fichajes_sysroTiposOperaciones]
GO

ALTER TABLE [dbo].[Entries] ADD  CONSTRAINT [DF_Entries_IDCause]  DEFAULT (0) FOR [IDCause]
GO

ALTER TABLE [dbo].[Entries] ADD  CONSTRAINT [DF_Entries_InvalidRead]  DEFAULT (0) FOR [InvalidRead]
GO

ALTER TABLE [dbo].[Entries] ADD  CONSTRAINT [DF_Entries_Rdr]  DEFAULT (0) FOR [Rdr]
GO

ALTER TABLE [dbo].[Entries] ADD  CONSTRAINT [DF_Entries_Reliability]  DEFAULT ((100)) FOR [Reliability]
GO

ALTER TABLE [dbo].[Entries] ADD  DEFAULT ((0)) FOR [checked]
GO

/****** Object:  Copiamos el contenido de toDelEntries a Entries   Script Date: 02/04/2013 13:26:01 ******/
INSERT INTO [dbo].[Entries]([DateTime],[IDCard],[IDReader],[Type],[IDCause],[InvalidRead],[Rdr],[Reliability],[checked]) SELECT [DateTime],[IDCard],[IDReader],[Type],[IDCause],[InvalidRead],[Rdr],[Reliability],[checked]
  FROM [dbo].[ToDelEntries]
GO
-- Fin cambio de tipo de campo en la tabla entries


--===========================================
CREATE TABLE [dbo].[TMPDetailedCalendarEmployee](
	[IdGroup] [int] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[DatePlanification] [smalldatetime] NOT NULL,
	[GroupName] [nvarchar](200) NULL,
	[EmployeeName] [nvarchar](50) NULL,
	[PuncheIn1] [smalldatetime] NULL,
	[PuncheOut1] [smalldatetime] NULL,
	[PuncheIn2] [smalldatetime] NULL,
	[PuncheOut2] [smalldatetime] NULL,
	[PuncheIn3] [smalldatetime] NULL,
	[PuncheOut3] [smalldatetime] NULL,
	[PuncheIn4] [smalldatetime] NULL,
	[PuncheOut4] [smalldatetime] NULL,
	[PuncheIn5] [smalldatetime] NULL,
	[PuncheOut5] [smalldatetime] NULL,
	[IdShift] [smallint] NOT NULL,
	[IDConcept1] [smallint] NOT NULL,
	[ConceptName1] [nvarchar](3) NULL,
	[FormatConcept1] [nvarchar](1) NULL,
	[ValueConcept1] [numeric](19, 6) NOT NULL,
	[IDConcept2] [smallint] NOT NULL,
	[ConceptName2] [nvarchar](3) NULL,
	[FormatConcept2] [nvarchar](1) NULL,
	[ValueConcept2] [numeric](19, 6) NOT NULL,
	[IDConcept3] [smallint] NOT NULL,
	[ConceptName3] [nvarchar](3) NULL,
	[FormatConcept3] [nvarchar](1) NULL,
	[ValueConcept3] [numeric](19, 6) NOT NULL,
	[IDConcept4] [smallint] NOT NULL,
	[ConceptName4] [nvarchar](3) NULL,
	[FormatConcept4] [nvarchar](1) NULL,
	[ValueConcept4] [numeric](19, 6) NOT NULL,
	[IDConcept5] [smallint] NOT NULL,
	[ConceptName5] [nvarchar](3) NULL,
	[FormatConcept5] [nvarchar](1) NULL,
	[ValueConcept5] [numeric](19, 6) NOT NULL,
	[IDConcept6] [smallint] NOT NULL,
	[ConceptName6] [nvarchar](3) NULL,
	[FormatConcept6] [nvarchar](1) NULL,
	[ValueConcept6] [numeric](19, 6) NOT NULL,
	[IDConcept7] [smallint] NOT NULL,
	[ConceptName7] [nvarchar](3) NULL,
	[FormatConcept7] [nvarchar](1) NULL,
	[ValueConcept7] [numeric](19, 6) NOT NULL,
	[IDConcept8] [smallint] NOT NULL,
	[ConceptName8] [nvarchar](3) NULL,
	[FormatConcept8] [nvarchar](1) NULL,
	[ValueConcept8] [numeric](19, 6) NOT NULL,
	[Cause] [tinyint] NOT NULL,
 CONSTRAINT [PK_TMPDetailedCalendarEmployee] PRIMARY KEY CLUSTERED 
(
	[IdGroup] ASC,
	[IDEmployee] ASC,
	[DatePlanification] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDEmployee]  DEFAULT ((0)) FOR [IDEmployee]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IdShift]  DEFAULT ((0)) FOR [IdShift]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept1]  DEFAULT ((0)) FOR [IDConcept1]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept1]  DEFAULT ((0)) FOR [ValueConcept1]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept2]  DEFAULT ((0)) FOR [IDConcept2]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept2]  DEFAULT ((0)) FOR [ValueConcept2]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept3]  DEFAULT ((0)) FOR [IDConcept3]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept3]  DEFAULT ((0)) FOR [ValueConcept3]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept4]  DEFAULT ((0)) FOR [IDConcept4]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept4]  DEFAULT ((0)) FOR [ValueConcept4]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept5]  DEFAULT ((0)) FOR [IDConcept5]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept5]  DEFAULT ((0)) FOR [ValueConcept5]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept6]  DEFAULT ((0)) FOR [IDConcept6]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept6]  DEFAULT ((0)) FOR [ValueConcept6]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept7]  DEFAULT ((0)) FOR [IDConcept7]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept7]  DEFAULT ((0)) FOR [ValueConcept7]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_IDConcept8]  DEFAULT ((0)) FOR [IDConcept8]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_ValueConcept8]  DEFAULT ((0)) FOR [ValueConcept8]
GO

ALTER TABLE [dbo].[TMPDetailedCalendarEmployee] ADD  CONSTRAINT [DF_TMPDetailedCalendarEmployee_Cause]  DEFAULT ((0)) FOR [Cause]
GO
--===========================================

--===========================================
ALTER VIEW [dbo].[sysroVisitPlan]
 AS
SELECT dbo.Visitors.Name AS VisitorName, dbo.Visitors.NIF AS VisitorNIF, dbo.Employees.Name AS EmployeeVisitedName, dbo.VisitPlan.ID, 
 		dbo.VisitPlan.VisitorAlias, dbo.VisitPlan.VisitorId, dbo.VisitPlan.EmpVisitedId, dbo.VisitPlan.Date, dbo.VisitPlan.Comments, dbo.VisitPlan.Status, 
 		dbo.VisitPlan.PlannedById, dbo.VisitPlan.PlannedDate, dbo.VisitPlan.Type, dbo.VisitPlan.Ticket, dbo.VisitPlan.PeriodicID, dbo.VisitPlan.PeriodicBegin, 
 		dbo.VisitPlan.PeriodicEnd, dbo.VisitPlan.IDLocation, dbo.VisitMoves.BeginTime, dbo.VisitMoves.EndTime, Visitors.Company
 	FROM dbo.Employees 
 		INNER JOIN dbo.VisitPlan ON dbo.Employees.ID = dbo.VisitPlan.EmpVisitedId 
 		LEFT OUTER JOIN dbo.VisitMoves ON dbo.VisitPlan.ID = dbo.VisitMoves.VisitPlanId 
 		LEFT OUTER JOIN dbo.Visitors ON dbo.VisitPlan.VisitorId = dbo.Visitors.ID
GO
--===========================================


--===========================================
DROP PROCEDURE  [dbo].[Visits_VisitPlan_GetAllByPeriodAndStatusExtended]
GO
CREATE PROCEDURE [dbo].[Visits_VisitPlan_GetAllByPeriodAndStatusExtended]
   	@BeginDate smalldatetime,
   	@EndDate smalldatetime,
   	@Status int,
   	@Location int,
   	@Type int,
	@VisitorName nvarchar(50), 
	@Company nvarchar(50)

   AS

	DECLARE @SQL nvarchar(1000)

  	IF @Status = 1 Or @Status=4 
  		IF @Location = -1
  			SET @SQL = 'SELECT * FROM sysroVisitPlan WHERE [Date] >= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @BeginDate, 120) + ''',120) AND [Date] <= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @EndDate, 120) + ''', 120) AND [Status] = ' + CONVERT(NVARCHAR, @Status, 120) + ' AND (IDLocation IS NULL OR IDLocation >= 0)'
  		ELSE
  			SET @SQL = 'SELECT * FROM sysroVisitPlan WHERE [Date] >= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @BeginDate, 120) + ''',120) AND [Date] <= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @EndDate, 120) + ''', 120) AND [Status] = ' + CONVERT(NVARCHAR, @Status, 120) + ' AND IDLocation = ' + @Location
  	ELSE IF @Status = 2 
  		IF @Location = -1
  			SET @SQL = 'SELECT * FROM sysroVisitPlan WHERE BeginTime >= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @BeginDate, 120) + ''',120) AND BeginTime<= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @EndDate, 120) + ''', 120) AND EndTime IS NULL AND [Status] = ' + CONVERT(NVARCHAR, @Status, 120) + ' AND (IDLocation IS NULL OR IDLocation >= 0)'
  		ELSE
  			SET @SQL = 'SELECT * FROM sysroVisitPlan WHERE BeginTime >= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @BeginDate, 120) + ''',120) AND BeginTime<= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @EndDate, 120) + ''', 120) AND EndTime IS NULL AND [Status] = ' + CONVERT(NVARCHAR, @Status, 120) + ' AND IDLocation = ' + @Location
  	ELSE
  		IF @Location = -1
  			SET @SQL = 'SELECT * FROM sysroVisitPlan WHERE BeginTime >= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @BeginDate, 120) + ''',120) AND EndTime <= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @EndDate, 120) + ''', 120) AND [Status] = ' + CONVERT(NVARCHAR, @Status, 120) + ' AND (IDLocation IS NULL OR IDLocation >= 0)'
  		ELSE
  			SET @SQL = 'SELECT * FROM sysroVisitPlan WHERE BeginTime >= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @BeginDate, 120) + ''',120) AND EndTime <= CONVERT(smalldatetime,''' + CONVERT(NVARCHAR, @EndDate, 120) + ''', 120) AND [Status] = ' + CONVERT(NVARCHAR, @Status, 120) + ' AND IDLocation = ' + @Location 

IF ISNULL(@Type,0) <> 0
	SET @SQL = @SQL + ' AND ([Type] = ' + CONVERT(NVARCHAR, @Type, 120) + ')'

IF ISNULL(@Company,'') <> '' 
	SET @SQL = @SQL + ' AND ([Company] LIKE ''%' + @Company + '%'')'


IF ISNULL(@VisitorName,'') <> '' 
	SET @SQL = @SQL + ' AND ([VisitorName] LIKE ''%' + @VisitorName + '%'' OR [VisitorAlias] LIKE ''%' + @VisitorName + '%'')'

SET @SQL = @SQL + ' ORDER BY convert(nvarchar(10), [Date], 120) DESC, Type, VisitorName, VisitorAlias'
EXECUTE(@sql)
GO
--======================================

--======================================
DROP PROCEDURE [dbo].[Visits_VisitPlan_GetAllByPeriodAndStatus]
GO
CREATE PROCEDURE [dbo].[Visits_VisitPlan_GetAllByPeriodAndStatus]
   	@BeginDate smalldatetime,
   	@EndDate smalldatetime,
   	@Status int,
   	@Location int
   AS
  	IF @Status = 1 Or @Status=4 
  		IF @Location = -1
  			SELECT * FROM sysroVisitPlan WHERE [Date] >= @BeginDate AND [Date] <= @EndDate AND [Status] = @Status AND 
			(IDLocation IS NULL OR IDLocation >= 0) ORDER BY convert(nvarchar(10), [Date], 120) DESC, [Type], VisitorName, VisitorAlias
  		ELSE
  			SELECT * FROM sysroVisitPlan WHERE [Date] >= @BeginDate AND [Date] <= @EndDate AND [Status] = @Status AND 
			IDLocation = @Location ORDER BY convert(nvarchar(10), [Date], 120) DESC, [Type], VisitorName, VisitorAlias
  	ELSE IF @Status = 2 
  		IF @Location = -1
  			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND BeginTime<=@EndDate AND EndTime IS NULL AND [Status] = @Status AND 
			(IDLocation IS NULL OR IDLocation >= 0) ORDER BY convert(nvarchar(10), [Date], 120) DESC, [Type], VisitorName, VisitorAlias
  		ELSE
  			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND BeginTime<=@EndDate AND EndTime IS NULL AND [Status] = @Status AND 
			IDLocation = @Location ORDER BY convert(nvarchar(10), [Date], 120) DESC, [Type], VisitorName, VisitorAlias
  	ELSE
  		IF @Location = -1
  			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND EndTime <= @EndDate AND [Status] = @Status AND 
			(IDLocation IS NULL OR IDLocation >= 0) ORDER BY convert(nvarchar(10), [Date], 120) DESC, [Type], VisitorName, VisitorAlias
  		ELSE
  			SELECT * FROM sysroVisitPlan WHERE BeginTime >= @BeginDate AND EndTime <= @EndDate AND [Status] = @Status AND 
			IDLocation = @Location ORDER BY convert(nvarchar(10), [Date], 120) DESC, [Type], VisitorName, VisitorAlias
GO
--======================================
  			

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='298' WHERE ID='DBVersion'
GO

