ALTER TABLE dbo.Documents ADD BlobFileName [nvarchar](max) NULL
GO

ALTER TABLE dbo.Documents ADD CRC [nvarchar](32) NULL
GO

-- Estado recibo de documento
ALTER TABLE dbo.Documents ADD Received SMALLINT NOT NULL default(0)
GO
-- fecha de recibo de documento
ALTER TABLE dbo.Documents ADD ReceivedDate datetime default(null)
GO

CREATE TABLE [dbo].[ReportLayouts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[LayoutName] [varchar](500) NOT NULL,
	[LayoutXMLBinary] [varbinary](max) NULL,
	[IdPassport] [int] NOT NULL,
 CONSTRAINT [PK_ReportLayouts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 1) ON [PRIMARY],
 CONSTRAINT [Uniques] UNIQUE NONCLUSTERED 
(
	[LayoutName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReportFiles](
	[BlobLink] [nvarchar](500) NULL,
	[ExecutionTime] [smalldatetime] NOT NULL,
	[layoutID] [int] NOT NULL,
	[owner] [int] NOT NULL
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[ReportLayoutPermission](
	[ReportId] [int] NOT NULL,
	[PassportId] [int] NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ReportLayoutPermission]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayoutPermission_ReportLayouts] FOREIGN KEY([ReportId])
REFERENCES [dbo].[ReportLayouts] ([ID])
GO

ALTER TABLE [dbo].[ReportLayoutPermission] CHECK CONSTRAINT [FK_ReportLayoutPermission_ReportLayouts]
GO

ALTER TABLE [dbo].[ReportLayoutPermission]  WITH CHECK ADD  CONSTRAINT [FK_ReportLayoutPermission_sysroPassports] FOREIGN KEY([ReportId])
REFERENCES [dbo].[sysroPassports] ([ID])
GO

ALTER TABLE [dbo].[ReportLayoutPermission] CHECK CONSTRAINT [FK_ReportLayoutPermission_sysroPassports]
GO


ALTER VIEW [dbo].[sysrovwAbsencesEx]
   AS
   SELECT row_number() OVER(order by x.BeginDate) AS ID,  
				x.IDRelatedObject,
    			x.IDCause, 
    			x.IDEmployee, 
    			x.BeginDate, 
    			x.FinishDate, 
    			x.BeginTime, 
    			x.EndTime, 
    			x.Description ,
				CASE WHEN x.DocumentsDelivered > 0 THEN 0 ELSE 1 END AS DocumentsDelivered,
    			x.Status,
    			x.AbsenceType, 
			    emp.Name as EmployeeName, cause.Name as CauseName, grp.IDGroup, grp.FullGroupName,
                grp.Path, grp.CurrentEmployee, grp.BeginDate AS BeginDateMobility, grp.EndDate AS EndDateMobility
    FROM (
    (SELECT 	AbsenceID, NULL AS IDRelatedObject,
    			IDCause AS IDCause, 
    			IDEmployee AS IDEmployee, 
    			BeginDate AS BeginDate, 
    			ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) AS FinishDate, 
    			NULL AS BeginTime, 
    			NULL AS EndTime, 
    			CONVERT(NVARCHAR(4000),Description) AS Description ,
				(select COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = pa.AbsenceID and adf.forecasttype = 'days')  AS DocumentsDelivered, 
    			(SELECT CASE WHEN GETDATE() between BeginDate and DATEADD(DAY,1,(ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )))) THEN '1' WHEN ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
    			'ProgrammedAbsence' AS AbsenceType 
     FROM dbo.ProgrammedAbsences pa)
    union
    (SELECT   AbsenceID, ID AS IDRelatedObject,
    			IDCause AS IDCause, 
    			IDEmployee AS IDEmployee, 
    			Date AS BeginDate, 
    			ISNULL(FinishDate,Date) AS FinishDate, 
    			BeginTime AS BeginTime, 
    			EndTime AS EndTime, 
    			CONVERT(NVARCHAR(4000),Description) AS Description, 
				(select COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = pc.AbsenceID and adf.forecasttype = 'hours') AS DocumentsDelivered, 
    			(SELECT CASE WHEN GETDATE() between Date and DATEADD(DAY,1,(ISNULL(FinishDate, Date))) THEN '1' WHEN  DATEADD(DAY,1,ISNULL(FinishDate,Date)) < GETDATE()  THEN '2'  ELSE '0' END) AS Status,
    			'ProgrammedCause' AS AbsenceType 
     FROM dbo.ProgrammedCauses pc)
    union
    (SELECT   NULL as AbsenceID, ID AS IDRelatedObject,
    			IDCause AS IDCause, 
    			IDEmployee AS IDEmployee, 
    			Date1 AS BeginDate, 
    			ISNULL(Date2,Date1) AS FinishDate, 
    			FromTime AS BeginTime, 
    			ToTime AS EndTime, 
    			CONVERT(NVARCHAR(4000),Comments) AS Description, 
    			1 AS DocumentsDelivered,
    			(SELECT CASE WHEN Status = 0 THEN '-1' ELSE '-2' END) AS Status,
    			(SELECT CASE WHEN RequestType = 7 THEN 'RequestAbsence' ELSE 'RequestCause' END) AS AbsenceType 
     FROM dbo.Requests where RequestType in(7,9) and Status in(0,1))
     ) x
     inner join dbo.Employees as emp on x.IDEmployee = emp.ID
     inner join dbo.Causes as cause on x.IDCause = cause.ID
     inner join dbo.sysrovwCurrentEmployeeGroups as grp on x.IDEmployee = grp.IDEmployee
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='475' WHERE ID='DBVersion'
GO




