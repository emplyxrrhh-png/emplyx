--NUEVA TABLA PARA EL INFORME DE PLANIFICACIÓN ANUAL POR EMPLEADO
CREATE TABLE [dbo].[TMPANNUALINDIVIDUALCALENDAR](
	[IDEmployee] [numeric](18, 0) NOT NULL,
	[EMPLEADO] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[MES] [int] NOT NULL,
	[AÑO] [int] NOT NULL,
	[NOMBRE] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[DIAS] [int] NULL,
	[horasdia1] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia2] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia3] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia4] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia5] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia6] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia7] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia8] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia9] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia10] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia11] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia12] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia13] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia14] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia15] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia16] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia17] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia18] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia19] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia20] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia21] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia22] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia23] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia24] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia25] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia26] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia27] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia28] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia29] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia30] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[horasdia31] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TOTALHORAS] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[TOTALDIAS] [int] NULL,
	[TotalHorasAnuales] [numeric](18, 2) NULL,
	[TotalDiasAnuales] [numeric](18, 0) NULL,
 CONSTRAINT [PK_TMPANNUALINDIVIDUALCALENDAR] PRIMARY KEY NONCLUSTERED 
(
	[IDEmployee] ASC,
	[MES] ASC,
	[AÑO] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='261' WHERE ID='DBVersion'
GO
