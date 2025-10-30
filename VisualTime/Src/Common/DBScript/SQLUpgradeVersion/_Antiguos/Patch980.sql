-- No borréis esta línea
CREATE TABLE [dbo].[sysroConnectionHistory](
	[ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](500) NOT NULL,
	[Application] [smallint] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[EventType] [smallint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='980' WHERE ID='DBVersion'
GO
