-- No borréis esta línea
--Activate Bots Menu
UPDATE sysroGUI
    SET RequiredFunctionalities = 'U:Bots.Definition=Read'
WHERE IDPath = 'Portal\Bots' 
GO 

--Create Bots DB Structure
CREATE TABLE [dbo].[Bots](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Type] [smallint] NOT NULL,
	[Status] [smallint] NULL,
	[LastExecutionDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO



CREATE TABLE [dbo].[BotRules](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IDBot] [int] NOT NULL,
	[Type] [smallint] NOT NULL,	
	[Name] [nvarchar](250) NULL,
	[IDTemplate] [int] NULL,
	[IsActive] [bit] not NULL default(1),
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='737' WHERE ID='DBVersion'
GO
