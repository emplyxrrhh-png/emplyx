-- Actualizamos la función para que sea compatible con mas terminales

if exists (select * from sysobjects where id = object_id(N'[dbo].[Access_CheckPunchResult]'))
	drop FUNCTION [dbo].[Access_CheckPunchResult]
GO

 CREATE FUNCTION [dbo].[Access_CheckPunchResult] 
 (
 	@idmployee int,
 	@idterminal tinyint,
 	@idreader tinyint,
 	@punchdate datetime 
 )
 RETURNS nvarchar(4)
 AS
 BEGIN
 	DECLARE @result as nvarchar(4)
 	DECLARE @zone as int
 	DECLARE @acc as int
	DECLARE @acc2 as int
 	DECLARE @group as int
 	
 	set @acc = (Select count (*) from TerminalReaders where idterminal=@idterminal and id=@idreader and mode like '%ACC%')	
 	set @acc2 = (Select count (*) from Terminals where id=@idterminal and behavior like '%ACC%')	
 	set @zone = (Select isnull(idzone,0) from TerminalReaders where idterminal=@idterminal and id=@idreader)	
 	-- Si no tiene accessos devolvemos un NA 
 	if  (@acc=0 and @acc2=0) or @zone = 0
 		SET @result = 'NA'
 	ELSE
 		BEGIN
 			-- Miramos si pertenece a un grupo asignado a esa zona
 			set @group = (select count(*)
 							from AccessGroupsPermissions agp
 							inner join Employees e
 							on agp.IDAccessGroup=e.IDAccessGroup
 							where agp.IDZone=@zone
 							and e.id=@idmployee)		
 			IF @group=0
 				SET @result = 'AIC'
 			ELSE
 				BEGIN
 					DECLARE @periodo as int
 					DECLARE @periodoHolidays as int
 					-- Miramos si esta dentro del perioro de esa zona
 					SET @periodo = (select count(*)
 										from AccessGroupsPermissions agp
 										inner join AccessPeriodDaily apd
 										on agp.IDAccessPeriod=apd.IDAccessPeriod
 										where agp.IDZone=@zone
 										and dayofweek=datepart(w,@punchdate)
 										and convert(smalldatetime, CONVERT(VARCHAR(8),@punchdate,114) ,120)
 										between convert(smalldatetime, CONVERT(VARCHAR(8),begintime,114) ,120) 
 										and convert(smalldatetime, CONVERT(VARCHAR(8),endtime,114) ,120))
 					SET @periodoHolidays = (select count(*)
 										from AccessPeriodHolidays aph
 										inner join AccessGroupsPermissions agp
 										on agp.IDAccessPeriod=aph.IDAccessPeriod
 										where agp.IDZone=@zone
 										and [day]=datepart(d,getdate())
 										and [month]=datepart(m,getdate())
 										and convert(smalldatetime, CONVERT(VARCHAR(8),@punchdate,114) ,120)
 										between convert(smalldatetime, CONVERT(VARCHAR(8),begintime,114) ,120) 
 										and convert(smalldatetime, CONVERT(VARCHAR(8),endtime,114) ,120))
 					IF @periodo = 0 and @periodoHolidays=0
 						SET @result = 'AIT'
 					ELSE
 						BEGIN
 							--Miramos si tiene PRL
 							declare @ohp as bit
 							SET @ohp = (Select isnull(ohp,0) from TerminalReaders where idterminal=@idterminal and id=@idreader)	 
 							IF @ohp = 1
 								SET @result = 'PRL'
 							ELSE
 								SET @result = 'AV'
 						END
 				END
 		END
 RETURN @result
 END
GO


-- Creamos las tablas temporales para los informes de HRScheduling
CREATE TABLE [dbo].[TmpMonthlyScheduling](
	[IDGroup] [numeric](18, 0) NOT NULL,
	[IDAssignment] smallint NOT NULL,
	[Type] [smallint] NOT NULL,
	[Value1] [numeric](10, 2) NULL,
	[Value2] [numeric](16, 6) NULL,
	[Value3] [numeric](16, 6) NULL,
	[Value4] [numeric](16, 6) NULL,
	[Value5] [numeric](16, 6) NULL,
	[Value6] [numeric](16, 6) NULL,
	[Value7] [numeric](16, 6) NULL,
	[Value8] [numeric](16, 6) NULL,
	[Value9] [numeric](16, 6) NULL,
	[Value10] [numeric](16, 6) NULL,
	[Value11] [numeric](16, 6) NULL,
	[Value12] [numeric](16, 6) NULL,
	[Value13] [numeric](16, 6) NULL,
	[Value14] [numeric](16, 6) NULL,
	[Value15] [numeric](16, 6) NULL,
	[Value16] [numeric](16, 6) NULL,
	[Value17] [numeric](16, 6) NULL,
	[Value18] [numeric](16, 6) NULL,
	[Value19] [numeric](16, 6) NULL,
	[Value20] [numeric](16, 6) NULL,
	[Value21] [numeric](16, 6) NULL,
	[Value22] [numeric](16, 6) NULL,
	[Value23] [numeric](16, 6) NULL,
	[Value24] [numeric](16, 6) NULL,
	[Value25] [numeric](16, 6) NULL,
	[Value26] [numeric](16, 6) NULL,
	[Value27] [numeric](16, 6) NULL,
	[Value28] [numeric](16, 6) NULL,
	[Value29] [numeric](16, 6) NULL,
	[Value30] [numeric](16, 6) NULL,
	[Value31] [numeric](16, 6) NULL,
	[Total] [numeric](16, 6) NULL,
 CONSTRAINT [PK_TmpMonthlyScheduling] PRIMARY KEY NONCLUSTERED 
(
	[IDGroup] ASC,
	[IDAssignment] ASC,
	[Type] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO


CREATE TABLE [dbo].[TmpWeeklyScheduling](
	[IDGroup] [numeric](18, 0) NOT NULL,
	[IDAssignment] smallint NOT NULL,
	[Type] [smallint] NOT NULL,
	[Value1] [numeric](10, 2) NULL,
	[Value2] [numeric](16, 6) NULL,
	[Value3] [numeric](16, 6) NULL,
	[Value4] [numeric](16, 6) NULL,
	[Value5] [numeric](16, 6) NULL,
	[Value6] [numeric](16, 6) NULL,
	[Value7] [numeric](16, 6) NULL,
	[Total] [numeric](16, 6) NULL,
 CONSTRAINT [PK_TmpWeeklyScheduling] PRIMARY KEY NONCLUSTERED 
(
	[IDGroup] ASC,
	[IDAssignment] ASC,
	[Type] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[TMPSchedulingCost](
            [IDGroup] [int] NOT NULL,
            [Date] [smalldatetime] NOT NULL,
            [IDAssignment] [smallint] NOT NULL,
            [Cost1] [numeric](18, 2) NOT NULL CONSTRAINT [DF_TMPSchedulingCost_Cost1]  DEFAULT ((0)),
            [Cost2] [numeric](18, 2) NULL CONSTRAINT [DF_TMPSchedulingCost_Cost2]  DEFAULT ((0)),
 CONSTRAINT [PK_TMPSchedulingCost] PRIMARY KEY CLUSTERED 
(
            [IDGroup] ASC,
            [Date] ASC,
            [IDAssignment] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO







-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='265' WHERE ID='DBVersion'
GO
