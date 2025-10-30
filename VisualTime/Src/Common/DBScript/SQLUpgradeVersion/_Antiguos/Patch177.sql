--
-- Crear tabla de grupos de horarios
--

CREATE TABLE [dbo].[ShiftGroups] (
	[ID] [smallint] NOT NULL ,
	[Name] [nvarchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ShiftGroups] WITH NOCHECK ADD 
	CONSTRAINT [PK_ShiftGroups] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] 
GO

ALTER TABLE SHIFTS ADD [IDGroup] [int] NOT Null default(0)
GO

INSERT INTO ShiftGroups VALUES (0,'(ninguno)')
GO

UPDATE SHIFTS SET IDGROUP=0 
GO


--
-- TABLA TEMPORAL PARA INFORME DE CALENDARIO X EMPLEADO
--

CREATE TABLE [dbo].[TMPCALENDAREMPLOYEE] (
	[IDEmployee] [numeric](18, 0) not NULL,
	[EMPLEADO] [nvarchar] (50) null,
	[MES] [int] NOT NULL ,
	[NOMBRE] [nvarchar] (50) NULL ,
	[DIAS] [int] NULL ,
	[horasdia1] [nvarchar] (50) NULL ,
	[horasdia2] [nvarchar] (50) NULL ,
	[horasdia3] [nvarchar] (50) NULL ,
	[horasdia4] [nvarchar] (50) NULL ,
	[horasdia5] [nvarchar] (50) NULL ,
	[horasdia6] [nvarchar] (50) NULL ,
	[horasdia7] [nvarchar] (50) NULL ,
	[horasdia8] [nvarchar] (50) NULL ,
	[horasdia9] [nvarchar] (50) NULL ,
	[horasdia10] [nvarchar] (50) NULL ,
	[horasdia11] [nvarchar] (50) NULL ,
	[horasdia12] [nvarchar] (50) NULL ,
	[horasdia13] [nvarchar] (50) NULL ,
	[horasdia14] [nvarchar] (50) NULL ,
	[horasdia15] [nvarchar] (50) NULL ,
	[horasdia16] [nvarchar] (50) NULL ,
	[horasdia17] [nvarchar] (50) NULL ,
	[horasdia18] [nvarchar] (50) NULL ,
	[horasdia19] [nvarchar] (50) NULL ,
	[horasdia20] [nvarchar] (50) NULL ,
	[horasdia21] [nvarchar] (50) NULL ,
	[horasdia22] [nvarchar] (50) NULL ,
	[horasdia23] [nvarchar] (50) NULL ,
	[horasdia24] [nvarchar] (50) NULL ,
	[horasdia25] [nvarchar] (50) NULL ,
	[horasdia26] [nvarchar] (50) NULL ,
	[horasdia27] [nvarchar] (50) NULL ,
	[horasdia28] [nvarchar] (50) NULL ,
	[horasdia29] [nvarchar] (50) NULL ,
	[horasdia30] [nvarchar] (50) NULL ,
	[horasdia31] [nvarchar] (50) NULL ,
	[TOTALHORAS] [nvarchar] (50) NULL ,
	[TOTALDIAS] [int] NULL ,
	[TotalHorasAnuales] [numeric](18, 2)  NULL,
	[TotalDiasAnuales] [numeric](18, 0)  NULL

) ON [PRIMARY]
GO


ALTER TABLE [dbo].[TMPCALENDAREMPLOYEE] WITH NOCHECK ADD 
	CONSTRAINT [PK_TMPCALENDAREMPLOYEE] PRIMARY KEY  NONCLUSTERED 
	(
		[IDEmployee],
		[MES]

	)  ON [PRIMARY] 
GO

--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='177' WHERE ID='DBVersion'
GO
