-- No borréis esta línea
DROP TABLE Collectives
GO

CREATE TABLE [dbo].[Collectives](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](255) NULL,
 CONSTRAINT [PK_Collectives] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) WITH (ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 
GO

CREATE TABLE [dbo].[CollectivesDefinitions](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [IDCollective] [int] NOT NULL,
    [Definition] [nvarchar](max) NOT NULL,
    [Filter] [nvarchar](max) NOT NULL,
    [BeginDate] [smalldatetime] NOT NULL,
    [EndDate] [smalldatetime] NULL,
    CONSTRAINT [PK_CollectivesDefinitions] PRIMARY KEY CLUSTERED 
    (
        [ID] 
    ) WITH (ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CollectivesDefinitions]
ADD CONSTRAINT FK_CollectivesDefinitions_Collectives
FOREIGN KEY ([IDCollective]) REFERENCES [dbo].[Collectives]([ID])
GO

CREATE INDEX IX_CollectivesDefinitions_IDCollective_BeginDate
ON [dbo].[CollectivesDefinitions] ([IDCollective], [BeginDate])
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1005' WHERE ID='DBVersion'
GO
