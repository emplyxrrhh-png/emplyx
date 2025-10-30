/****** Object:  Table [dbo].[sysroPassports_CacheData]    Script Date: 08/07/2024 11:27:50 ******/
CREATE TABLE [dbo].[sysroPassports_CacheData](
	[IDPassport] [int] NOT NULL,
	[IDCacheData] [nvarchar](150) NOT NULL,
	[Value] [nvarchar](max) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
CONSTRAINT [PK_sysroPassports_CacheData] PRIMARY KEY CLUSTERED 
(
	[IDPassport] ASC,
	[IDCacheData] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='870' WHERE ID='DBVersion'

GO
