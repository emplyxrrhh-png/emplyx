CREATE TABLE [dbo].[Communiques](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Subject] [nchar](30) NOT NULL,
	[IdCompany] [int] NULL,
	[IdCreatedBy] [int] NOT NULL,
	[CreatedOn] [smalldatetime] NOT NULL,
	[Mandatory] [bit] NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[AllowedResponses] [nchar](100) NULL,
	[AllowChangeResponse] [bit] NOT NULL,
	[ResponseLimitPercentage] [int] NULL,
	[ExpirationDate] [smalldatetime] NOT NULL,
	[Status] [smallint] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Communiques]  WITH CHECK ADD FOREIGN KEY([IdCompany])
REFERENCES [dbo].[Groups] ([Id])
GO

ALTER TABLE [dbo].[Communiques]  WITH CHECK ADD FOREIGN KEY([IdCreatedBy])
REFERENCES [dbo].[sysroPassports] ([Id])
GO

CREATE TABLE [dbo].[CommuniqueEmployees](
	[IdCommunique] [int] NOT NULL,
	[IdEmployee] [int] NOT NULL,
 CONSTRAINT [PK_CommuniqueEmployees] PRIMARY KEY CLUSTERED 
(
	[IdCommunique] ASC,
	[IdEmployee] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CommuniqueEmployees]  WITH CHECK ADD FOREIGN KEY([IdCommunique])
REFERENCES [dbo].[Communiques] ([Id])
GO

ALTER TABLE [dbo].[CommuniqueEmployees]  WITH CHECK ADD FOREIGN KEY([IdEmployee])
REFERENCES [dbo].[Employees] ([Id])
GO

CREATE TABLE [dbo].[CommuniqueGroups](
	[IdCommunique] [int] NOT NULL,
	[IdGroup] [int] NOT NULL,
CONSTRAINT [PK_CommuniqueGroups] PRIMARY KEY CLUSTERED 
(
	[IdCommunique] ASC,
	[IdGroup] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CommuniqueGroups] WITH CHECK ADD FOREIGN KEY([IdCommunique])
REFERENCES [dbo].[Communiques] ([Id])
GO

ALTER TABLE [dbo].[CommuniqueGroups]  WITH CHECK ADD FOREIGN KEY([IdGroup])
REFERENCES [dbo].[Groups] ([Id])
GO

CREATE TABLE [dbo].[CommuniqueEmployeeStatus](
	[IdCommunique] [int] NOT NULL,
	[IdEmployee] [int] NOT NULL,
	[ReadTimeStamp] [smalldatetime] NOT NULL,
	[Response] [nchar](20) NULL,
	[ResponseTimeStamp] [smalldatetime] NULL,
	 CONSTRAINT [PK_CommuniquesEmployeeStatus] PRIMARY KEY CLUSTERED 
(
	[IdCommunique] ASC,
	[IdEmployee] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CommuniqueEmployeeStatus]  WITH CHECK ADD FOREIGN KEY([IdCommunique])
REFERENCES [dbo].[Communiques] ([Id])
GO

ALTER TABLE [dbo].[CommuniqueEmployeeStatus]  WITH CHECK ADD FOREIGN KEY([IdEmployee])
REFERENCES [dbo].[Employees] ([Id])
GO

CREATE VIEW [dbo].[sysrovwCommuniquesEmployees]
AS
 	SELECT IdCommunique, IdEmployee FROM [dbo].[CommuniqueEmployees]
	LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueEmployees.IdCommunique
 	UNION 
 	SELECT IdCommunique, sysrovwCurrentEmployeeGroups.IdEmployee FROM [dbo].[CommuniqueGroups]
	LEFT JOIN [dbo].[Communiques] ON Communiques.Id = CommuniqueGroups.IdCommunique
 	INNER JOIN [dbo].[Groups] on CommuniqueGroups.idgroup = Groups.id
 	INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] on (sysrovwCurrentEmployeeGroups.Path = Groups.Path or sysrovwCurrentEmployeeGroups.Path like Groups.Path + '\%' )
	INNER JOIN [dbo].[sysroPassports] ON Communiques.IdCreatedBy = sysroPassports.Id
	INNER JOIN [dbo].[sysroPermissionsOverGroups] Perm ON Perm.EmployeeGroupID = CommuniqueGroups.IdGroup AND Perm.EmployeeFeatureID = 1 AND Perm.PassportID = sysroPassports.IDParentPassport AND Perm.Permission >= 3
GO

CREATE VIEW [dbo].[sysrovwCommuniquesStatistics]
AS
	SELECT c.Id as IdCommunique, 
		   c.Subject, 
		   svc.IdEmployee, 
		   CASE WHEN ReadTimeStamp IS NULL THEN 0 ELSE 1 END as ReadStatus, 
		   ces.ReadTimeStamp, 
		   CASE WHEN LEN(ISNULL(c.AllowedResponses,'')) > 0 THEN 1 ELSE 0 END as RequiresResponse,
		   CASE WHEN ResponseTimeStamp IS NULL THEN 0 ELSE 1 END as ResponseStatus, 
		   ces.Response, 
		   ces.ResponseTimeStamp 
	FROM [dbo].[sysrovwCommuniquesEmployees] svc
	LEFT JOIN [dbo].[Communiques] c ON svc.IdCommunique = c.Id
	LEFT JOIN [dbo].[CommuniqueEmployeeStatus] ces ON ces.IdCommunique = svc.IdCommunique AND svc.IdEmployee = ces.IdEmployee
GO

ALTER TABLE [dbo].[Documents] ADD [IdCommunique] INT NULL
GO

ALTER TABLE [dbo].[Communiques]
ALTER COLUMN [Subject] [nvarchar](30);

ALTER TABLE [dbo].[Communiques]
ALTER COLUMN [AllowedResponses] [nvarchar](100);

UPDATE [dbo].[Communiques] SET Subject = LTRIM(RTRIM(Subject)), AllowedResponses = LTRIM(RTRIM(AllowedResponses))
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='487' WHERE ID='DBVersion'
GO

