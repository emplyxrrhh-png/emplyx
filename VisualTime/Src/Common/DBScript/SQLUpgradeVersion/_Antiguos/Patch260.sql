--- Creamos tablas para los nuevos mx7+
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='260' WHERE ID='DBVersion'
GO
