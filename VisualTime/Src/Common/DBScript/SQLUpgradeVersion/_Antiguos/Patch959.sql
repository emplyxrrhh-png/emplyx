-- No borréis esta línea
CREATE TABLE [dbo].[WebLinks] (
    [ID] [int] NOT NULL IDENTITY(1,1),
	[Title] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
    [URL] [nvarchar](max) NOT NULL,
	[LinkCaption] [nvarchar](50) NOT NULL,
	[Order] [INT] NULL,
	[ShowOnLiveDashboard] [BIT] NULL,
	[ShowOnPortalMenu] [BIT] NULL,
	[ShowOnPortalDashboard] [BIT] NULL,
    CONSTRAINT [PK_WebLinks] PRIMARY KEY CLUSTERED ([ID] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
GO


INSERT INTO [dbo].[WebLinks]
           ([Title]
           ,[Description]
           ,[URL]
           ,[LinkCaption]
           ,[Order]
           ,[ShowOnLiveDashboard]
           ,[ShowOnPortalMenu]
           ,[ShowOnPortalDashboard])
     VALUES
           (N'Visualtime Academy'
           ,N'Aula virtual para que siempre estés al día. Conocerás todas las novedades que afectan a la gestión del tiempo de los usuarios, normativas, funcionalidades de Visualtime, y mucho más.'
           ,N'https://www.cegid.com/ib/es/lp/hcm-talent-time-cegid-visualtime-calendario-academy/'
           ,N'¡Apúntate!'
           , 1
           , 1
           , 0
           , 0)
GO


INSERT INTO [dbo].[WebLinks]
           ([Title]
           ,[Description]
           ,[URL]
           ,[LinkCaption]
           ,[Order]
           ,[ShowOnLiveDashboard]
           ,[ShowOnPortalMenu]
           ,[ShowOnPortalDashboard])
     VALUES
           (N'Soporte'
           ,N'A través de nuestro servicio de soporte exclusivo para clientes de Cegid Visualtime, puedes solicitar ayuda o asesoramiento personalizado.'
           ,N'https://www.cegid.com/ib/asistencia-al-cliente/software-gestion-tiempo-visualtime/'
           ,N'Contacto'
           , 2
           , 1
           , 0
           , 0)
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='959' WHERE ID='DBVersion'
GO
