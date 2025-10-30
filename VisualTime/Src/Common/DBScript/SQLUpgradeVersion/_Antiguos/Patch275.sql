-- Nuevo Terminal rxF
INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxF',1,1,'TA',1,'Blind','E,S,X','Local','1,0',0,0,'1,0','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxF',1,2,'ACC',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxF',1,3,'ACCTA',1,'Blind','E,S,X','Local','1,0',0,0,'1','0','Remote')
GO

UPDATE sysroReaderTemplates SET type='rxA' WHERE type = 'rxa'
GO

UPDATE sysroReaderTemplates SET type='rxB' WHERE type = 'rxb'
GO

UPDATE sysroReaderTemplates SET type='rxA100fp' WHERE type = 'rxa100fp'
GO

UPDATE sysroReaderTemplates SET type='SAIWALL' WHERE type = 'saiwall'
GO

UPDATE sysroReaderTemplates SET type='rxA100FP' WHERE type = 'rxA100fp'
GO

UPDATE sysroReaderTemplates SET type='rxA100P' WHERE type = 'rxA100p'
GO

UPDATE sysroReaderTemplates SET type='rxA200' WHERE type = 'rxa200'
GO

UPDATE Terminals SET type='LivePortal' WHERE type = 'LIVEPORTAL'
GO

UPDATE Terminals SET type='mx6' WHERE type = 'MX6'
GO

UPDATE Terminals SET type='mx7' WHERE type = 'MX7'
GO

UPDATE Terminals SET type='mxART' WHERE type = 'MXART'
GO

UPDATE Terminals SET type='rx' WHERE type = 'RX'
GO

UPDATE Terminals SET type='rxA' WHERE type = 'RXA'
GO

UPDATE Terminals SET type='rxA100FP' WHERE type = 'RXA100FP'
GO

UPDATE Terminals SET type='rxA100P' WHERE type = 'RXA100P'
GO

UPDATE Terminals SET type='rxA200' WHERE type = 'RXA200'
GO

UPDATE Terminals SET type='rxB' WHERE type = 'RXB'
GO

UPDATE Terminals SET type='rxF' WHERE type = 'RXF'
GO

UPDATE Terminals SET type='SAIWALL' WHERE type = 'SAIWALL'
GO

UPDATE sysroReaderTemplates SET OHP ='1,0' WHERE Type='mx7' and IDReader=1 and ScopeMode like '%TA%'
GO

  ALTER FUNCTION [dbo].[Access_CheckPunchResult] 
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
  										inner join Employees e
										on agp.IDAccessGroup=e.IDAccessGroup and e.id=@idmployee
										where agp.IDZone=@zone
										and dayofweek=datepart(w,@punchdate)
										and convert(smalldatetime, substring(CONVERT(VARCHAR(20),@punchdate,120),11,9) ,120)
										between convert(smalldatetime, substring(CONVERT(VARCHAR(20),begintime,120),11,9) ,120) 
										and convert(smalldatetime, substring(CONVERT(VARCHAR(20),endtime,120),11,9) ,120))
  					SET @periodoHolidays = (select count(*)
  										from AccessPeriodHolidays aph
  										inner join AccessGroupsPermissions agp
  										on agp.IDAccessPeriod=aph.IDAccessPeriod
  										inner join Employees e
										on agp.IDAccessGroup=e.IDAccessGroup and e.id=@idmployee
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
UPDATE sysroParameters SET Data='275' WHERE ID='DBVersion'
GO
