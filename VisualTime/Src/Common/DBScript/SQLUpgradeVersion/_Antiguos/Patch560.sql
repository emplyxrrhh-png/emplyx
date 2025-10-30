CREATE TABLE [dbo].[ExportGuidesSchedules](
[ID] [smallint] NOT NULL,
[Name] [nvarchar](128) NOT NULL,
[IDGuide] [smallint] NOT NULL,
[IDTemplate] [smallint] NULL,
[EmployeeFilter] [nvarchar](max) NULL,
[Destination] [nvarchar](128) NULL,
[ExportFileName] [nvarchar](128) NULL,
[ExportFileNameTimeStampFormat] [nvarchar](20) NULL,
[ExportFileType] [smallint] NULL,
[WSParameters] [nvarchar](max) NULL,
[Separator] [nvarchar](1) NULL,
[Scheduler] [nvarchar](128) NULL,
[AutomaticDatePeriod] [nvarchar](max) NULL,
[NextExecution] [datetime] NULL,
[LastLog] [text] NULL,
[Active] [bit] NULL,
[Enabled] [bit] NULL,
[ApplyLockDate] [bit] NOT NULL,
DatasourceFile [nvarchar](50) NOT NULL
CONSTRAINT [PK_ExportGuidesSchedules] PRIMARY KEY NONCLUSTERED
(
[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) )
GO

INSERT INTO ExportGuidesSchedules (ID,Name,IDGuide,IDTemplate,EmployeeFilter,Destination,ExportFileName,ExportFileNameTimeStampFormat,ExportFileType,WSParameters,Separator,Scheduler,AutomaticDatePeriod,NextExecution,LastLog,Active,Enabled,ApplyLockDate,DataSourceFile) 
	select ROW_NUMBER() OVER(ORDER BY name ASC) AS ID, Name, ID,IDDefaultTemplate,EmployeeFilter,Destination,ExportFileName,ExportFileNameTimeStampFormat, ExportFileType, WSParameters,Separator,Scheduler,AutomaticDatePeriod,NextExecution,LastLog,Active,Enabled,ApplyLockDate,DataSourceFile from ExportGuides where Version = 2 and Mode = 2
GO

UPDATE sysroLiveAdvancedParameters SET Value = 19 WHERE ParameterName='VTPortalApiVersion'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='560' WHERE ID='DBVersion'
GO