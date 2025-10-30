/****** Object:  Table [dbo].[ProgrammedAbsences]    Script Date: 03/10/01 10:57:14 ******/
if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[ProgrammedAbsences]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
drop table [dbo].[ProgrammedAbsences]
GO

/****** Object:  Table [dbo].[ProgrammedAbsences]    Script Date: 03/10/01 10:57:14 ******/
CREATE TABLE [dbo].[ProgrammedAbsences] (
	[IDCause] [smallint] NOT NULL ,
	[IDEmployee] [int] NOT NULL ,
	[BeginDate] [smalldatetime] NOT NULL ,
	[FinishDate] [smalldatetime] NULL ,
	[MaxLastingDays] [smallint] NOT NULL ,
	[Description] [ntext] NULL 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='123' WHERE ID='DBVersion'
GO