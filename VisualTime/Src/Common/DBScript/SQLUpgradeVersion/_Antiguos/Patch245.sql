/* Creación de la tabla temporal TMPDailyPartialAccruals que es usada para el informe de acumulados dia a dia*/
CREATE TABLE [dbo].[TMPDailyPartialAccruals](
	[FullGroupName] [nvarchar](500) NOT NULL,
	[IDGroup] [int] NOT NULL,
	[GroupName] [nvarchar](200) NOT NULL,
	[Position] [int] NULL,
	[IDReportGroup] [int] NULL,
	[TotalValueGroup] [numeric](16, 2) NULL,
	[Path] [nvarchar](200) NULL,
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](100) NOT NULL,
	[TotalValueEmployee] [numeric](16, 2) NULL,
	[Day] [smalldatetime] NULL,
	[ShiftName] [nvarchar](200) NULL,
	[ExpectedWorkingHours] [numeric](16, 2) NULL,
	[Acum1] [numeric](16, 2) NULL,
	[Acum2] [numeric](16, 2) NULL,
	[Acum3] [numeric](16, 2) NULL,
	[Acum4] [numeric](16, 2) NULL,
	[Acum5] [numeric](16, 2) NULL,
	[Acum6] [numeric](16, 2) NULL,
	[Acum7] [numeric](16, 2) NULL,
	[Acum8] [numeric](16, 2) NULL,
	[Acum9] [numeric](16, 2) NULL,
	[Acum10] [numeric](16, 2) NULL,
	[Acum11] [numeric](16, 2) NULL,
	[Acum12] [numeric](16, 2) NULL,
	[Acum13] [numeric](16, 2) NULL
) ON [PRIMARY]
GO

/* Actualización de la tabla TMPTOP del informe de los que más */
DROP TABLE TMPTOP
GO

CREATE TABLE [dbo].[TMPTOP](
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](50) NOT NULL,
	[IDPrimaryConcept] [smallint] NOT NULL,
	[IDSecundaryConcept] [smallint] NULL,
	[ConceptPrimaryName] [nvarchar](50) NOT NULL,
	[ConceptSecundaryName] [nvarchar](50) NULL,
	[Date] [datetime] NOT NULL,
	[BeginTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
	[Value] [numeric](19, 6) NULL,
 CONSTRAINT [PK_TMPTOP] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDPrimaryConcept] ASC,
	[Date] ASC,
	[BeginTime] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

/* Creación de la tabla temporal para el nuevo informe de control de vacaciones */
CREATE TABLE [dbo].[TMPCONTROLHOLIDAYS](
	[FullGroupName] [nvarchar](500) NOT NULL,
	[GroupName] [nvarchar](500) NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](500) NOT NULL,
	[StartupValue] [numeric](16, 2) NOT NULL,
	[CarryOver] [numeric](16, 2) NOT NULL,
	[StartupBalance] [numeric](16, 2) NOT NULL,
	[EnjoyedDays] [numeric](16, 2) NOT NULL,
	[LineEnjoyedDays] [nvarchar](1000) NOT NULL,
	[CurrentBalance] [numeric](16, 2) NOT NULL,
	[DaysProvided] [numeric](16, 2) NOT NULL,
	[LineDaysProvided] [nvarchar](1000) NOT NULL,
	[EndingBalance] [numeric](16, 2) NOT NULL,
	[MUID] [nvarchar](16) NULL
) ON [PRIMARY]
GO


/* ***************************************************************************************************************************** */

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='245' WHERE ID='DBVersion'
GO
