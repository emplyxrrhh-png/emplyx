------
--- CREACION DE NUEVOS CAMOPOS DE PRODUCCION
----

ALTER TABLE DAILYJOBACCRUALS ADD 
Performance NUMERIC(10,2) DEFAULT(0),
PieceTime NUMERIC(18,6) DEFAULT(0),
TotalPieces NUMERIC(18,6) DEFAULT(0)
GO

-- ACTUALIZAMOS LOS DATOS
UPDATE DAILYJOBACCRUALS SET Performance=0,PieceTime=0,TotalPieces=0
GO

CREATE PROCEDURE [dbo].[ActualizaRendimientos] AS
BEGIN
	DECLARE @Piece1CountasPiece bit;
	DECLARE @Piece2CountasPiece bit;
	DECLARE @Piece3CountasPiece bit;
	DECLARE @Contador int;
	DECLARE @Contador2 int;
	DECLARE @IDJob numeric (20);
	DECLARE @Path nvarchar(200);
	DECLARE @ID nvarchar(200);
	DECLARE @ID1 nvarchar(200);
	DECLARE @ID2 nvarchar(200);
	DECLARE @Pathaux nvarchar(200);
	DECLARE @PieceTimeJob numeric(18,6);
	DECLARE @PieceTime numeric(18,6);
	DECLARE @PreparationTime numeric(18,6);
	DECLARE @UnitPieces numeric(18,6);
	DECLARE @Amount numeric(18,6);
	DECLARE @i int;
	DECLARE @x int;
	DECLARE @z int;
	DECLARE @TimeReg numeric(18,6);
	DECLARE @PiecesReg numeric(18,6);
	DECLARE @Pieces1 numeric(18,6);
	DECLARE @Pieces2 numeric(18,6);
	DECLARE @Pieces3 numeric(18,6);
	DECLARE @PERFORMANCE numeric(18,6);
	DECLARE @IDOrder NUMERIC(18);
	DECLARE @AmountAux NUMERIC(18,6);
	DECLARE @itsok bit;
	DECLARE @ideMPLOYEE numeric(18);
	DECLARE @IDTeam numeric(10);
	DECLARE @IDMachine numeric(10);
	DECLARE @dATE smalldatetime;


	--- OBTENEMOS LAS CARACTERISTICAS DE LOS TIPOS DE PIEZAS
	SELECT @Piece1CountasPiece=IsConsideredValid From SysroPiecetypes WHERE 
        ID =1;
	SELECT @Piece2CountasPiece=IsConsideredValid From SysroPiecetypes WHERE 
        ID =2;
	SELECT @Piece3CountasPiece=IsConsideredValid From SysroPiecetypes WHERE 
        ID =3;


	--- OBTENEMOS LOS ACUMULADOS DE PRODUCCIÓN
	DECLARE crs CURSOR FOR

        SELECT  DailyJobAccruals.IDJob, Jobs.Path, Value, Pieces1, Pieces2, Pieces3, DailyJobAccruals.ideMPLOYEE, DailyJobAccruals.IDTeam,DailyJobAccruals.IDMachine, DailyJobAccruals.dATE  FROM DailyJobAccruals 
	INNER JOIN DailySchedule ON 
	DailyJobAccruals.IDEmployee=DailySchedule.IDEmployee AND 
        DailyJobAccruals.Date = DailySchedule.Date 
        INNER JOIN Jobs ON DailyJobAccruals.IDJob= Jobs.ID 
	WHERE IDIncidence= 0 
	ORDER BY DailyJobAccruals.IDJob asc
	--

	OPEN crs;
	FETCH NEXT FROM crs INTO @idJob, @path, @TimeReg, @pieces1,@pieces2,@pieces3,@ideMPLOYEE,@IDTeam,@IDMachine,@dATE ;

	WHILE @@FETCH_STATUS = 0 BEGIN
		--- OBTENEMOS LOS DATOS GENERALES DE LA FASE
		SELECT @PieceTimeJob=PieceTime,@PreparationTime=PreparationTime, @UnitPieces=UnitPieces FROM Jobs 
		WHERE ID = @idJob;

		---- OBTENEMOS LAS ORDENES A LA QUE PERTENECE LA FASE
		SET @Pathaux = @path;
		SET @Contador = 0;
		WHILE (charindex('\',@Pathaux) > 0)
		BEGIN
			   SET @Pathaux = substring(@Pathaux, charindex('\',@Pathaux ) + Len('\'), 999);
		   SET  @Contador =  @Contador + 1;
		END;
		IF rtrim(ltrim(@Pathaux)) <> '' BEGIN SET  @Contador =  @Contador + 1; END


		        SET @ID = '';
		SET @Pathaux= @Path;
		SET @i = 0;
		        WHILE @i <=@Contador - 2 BEGIN
		        IF @i = 0 
                        BEGIN
			    -- @ID = String2Item(Path, @i, "\")
				SET @x = @i;
				IF @x < 0 Or @x > @Contador - 1 
				BEGIN
    				        SET @ID = '';
				END
				ELSE
				BEGIN
					WHILE (@x > 0)
					BEGIN
					    SET @Pathaux = substring(@Pathaux, charindex('\',@Pathaux) + Len('\'), 999);
					    SET @x = @x - 1;
					END;
					IF charindex('\',@Pathaux) > 0 
					BEGIN 
						SET @Pathaux = substring(@Pathaux, 1, charindex('\',@Pathaux) - 1)
					END
					SET @ID = rtrim(lTrim(@Pathaux))
				END
                        END          
		        ELSE
                        BEGIN
				SET @Pathaux= @Path;
				SET @x = @i;

				IF @x < 0 Or @x > @Contador - 1 
				BEGIN
    				        SET @ID = @id;
				END
				ELSE
				BEGIN
					WHILE (@x > 0)
					BEGIN
					    SET @Pathaux = substring(@Pathaux, charindex('\',@Pathaux) + Len('\'), 999);
					    SET @x = @x - 1;
					END;
					IF charindex('\',@Pathaux) > 0 
					BEGIN 
						SET @Pathaux = substring(@Pathaux, 1, charindex('\',@Pathaux) - 1)
					END
					SET @ID = @id + ltrim(rtrim(@Pathaux));
				END
                        END
			SET @i = @i + 1;
	        END;

		SET @ID1 = '';
		SET @Pathaux = @Path;
		SET @x = 0;
		IF @x < 0 Or @x > @Contador - 1 
		BEGIN
		        SET @ID1 = '';
		END
		ELSE
		BEGIN
			WHILE (@x > 0)
			BEGIN
			    SET @Pathaux = substring(@Pathaux, charindex('\',@Pathaux) + Len('\'), 999);
			    SET @x = @x - 1;
			END
			IF charindex('\',@Pathaux) > 0 
			BEGIN 
				SET @Pathaux = substring(@Pathaux, 1, charindex('\',@Pathaux) - 1)
			END
			SET @ID1 = rtrim(lTrim(@Pathaux))
		END


		-- OBTENEMOS LAS UNIDADES A REALIZAR DE LA FASE
		DECLARE crs2 CURSOR FOR

		SELECT ID,Amount FROM Orders WHERE LEN(id) <=  Len(@ID)  AND ID Like  @ID1 + '%' AND LEN(id) >=  Len(@ID1)

		OPEN crs2;
		SET @Amount = 0;
		FETCH NEXT FROM crs2 INTO @IDOrder, @AmountAUX;
		WHILE @@FETCH_STATUS = 0 BEGIN
			SET @Pathaux = @IDOrder;
			SET @Contador2 = 0;
			WHILE (charindex('\',@Pathaux) > 0)
			BEGIN
 			   SET @Pathaux = substring(@Pathaux, charindex('\',@Pathaux ) + Len('\'), 999);
			   SET  @Contador2 =  @Contador2 + 1;
			END;
			IF rtrim(ltrim(@Pathaux)) <> '' BEGIN SET  @Contador2 =  @Contador2 + 1; END
   		        SET @itsok = 1;
			SET @x=0;
			WHILE @x <= (@contador2 -1)
			BEGIN
				SET @ID2 = '';
				SET @Pathaux = @IDOrder;
				SET @z = @x;
				IF @z < 0 Or @z > @contador2 - 1 
				BEGIN
					SET @ID2 = '';
				END
				ELSE
				BEGIN
					WHILE (@z > 0)
					BEGIN
					    SET @Pathaux = substring(@Pathaux, charindex('\',@Pathaux) + Len('\'), 999);
					    SET @z = @z - 1;
					END;
					IF charindex('\',@Pathaux) > 0 
					BEGIN 
						SET @Pathaux = substring(@Pathaux, 1, charindex('\',@Pathaux) - 1)
					END
					SET @ID2 = rtrim(lTrim(@Pathaux))
				END

				IF NOT (@id1 =@id2)
				BEGIN
					SET @itsok=0;
				END
				SET @x= @x + 1;
			            END;

			    IF @itsok = 1 
			    BEGIN
			    	SET @Amount = @Amount +  @Amountaux;
			    END
			FETCH NEXT FROM crs2 INTO @IDOrder, @Amountaux;
		END;
		CLOSE crs2;	
		DEALLOCATE crs2;

		-- OBTENEMOS EL TOTAL DE PIEZAS REALIZADAS
		SET @piecesreg= 0;
		IF @Piece1CountasPiece = 1 
		BEGIN
			SET @piecesreg= @piecesreg + @Pieces1;
		END

		IF @Piece2CountasPiece = 1
		BEGIN
			SET @piecesreg= @piecesreg + @Pieces2;
		END

		IF @Piece3CountasPiece = 1
		BEGIN
			SET @piecesreg= @piecesreg + @Pieces3;
		END

		-- CALCULAMOS EL RENDIMIENTO
		IF @UnitPieces <> 0 and @Amount <> 0
		BEGIN
			SET @performance = ((@PieceTimeJob * @UnitPieces * @Amount) + @PreparationTime) / (@UnitPieces * @Amount);
        	        SET @PieceTime = @Performance;
		        IF @PiecesReg <> 0 And @TimeReg <> 0
			BEGIN
				SET @Performance = (@Performance / (@TimeReg / @PiecesReg)) * 100;
                        END
		        ELSE
			BEGIN
				SET @Performance = 0;
		        END
		END
		ELSE
		BEGIN
			SET @Performance = 0;
		        SET @PieceTime = 0;

		END
		
		PRINT convert(nvarchar(200),@idJob) + ' ' + convert(nvarchar(200),@Performance);
		PRINT convert(nvarchar(200),@idJob) + 'Unidades ' + convert(nvarchar(200),@piecesreg);
		PRINT convert(nvarchar(200),@idJob) + 'TiempoPieza' + convert(nvarchar(200),@PieceTime);
		PRINT convert(nvarchar(200),@idJob) + 'Tiemporegistro' + convert(nvarchar(200),@TimeReg);

		--- ACTUALIZAMOS EL RENDIMIENTO DEL REGISTRO
		UPDATE DailyJobAccruals Set Performance=CONVERT(NUMERIC(10,2),@Performance),PieceTime=CONVERT(NUMERIC(18,6),@PieceTime),TotalPieces=CONVERT(NUMERIC(18,6),@piecesreg)
		WHERE DailyJobAccruals.IDEMPLOYEE=@IDEMPLOYEE
		AND DailyJobAccruals.DATE=@DATE
		AND DailyJobAccruals.IDMACHINE=@IDMACHINE
		AND DailyJobAccruals.IDTEAM = @IDTEAM
		AND DailyJobAccruals.IDINCIDENCE=0
		AND DailyJobAccruals.IDJOB=@IDJOB


	-- VAMOS AL SIGUIENTE REGISTRO
	FETCH NEXT FROM crs INTO @idJob, @path, @TimeReg, @pieces1,@pieces2,@pieces3,@ideMPLOYEE,@IDTeam,@IDMachine,@dATE ;

	END;
	CLOSE crs;
	DEALLOCATE crs;
END
GO

-- Lo ejecutamos
EXEC ActualizaRendimientos
GO

-- Lo eliminamos
DROP PROCEDURE [dbo].[ActualizaRendimientos]
GO


-- actualziamos los datos de la fase para los recuross
alter table Jobs
add FirstResource tinyint default(1),
SecondResource tinyint default(0),
AllowPunchResource bit default(0),
BeginEndAction tinyint default(1)
GO

UPDATE JOBS SET FirstResource=1, SecondResource=0, AllowPunchResource=0,BeginEndAction=1
GO

alter table JobTemplates
add FirstResource tinyint default(1),
SecondResource tinyint default(0),
AllowPunchResource bit default(0),
BeginEndAction tinyint default(1)
GO

UPDATE JobTemplates SET FirstResource=1, SecondResource=0, AllowPunchResource=0,BeginEndAction=1
GO


-- cREAMOS LA TABLA DE MOVIMIENTOS DE LAS MAQUINAS
CREATE TABLE [MachineJobMoves] (
	[IDMachine] [int] NOT NULL ,
	[InDateTime] [smalldatetime] NULL ,
	[InIDEmployee] [int] NULL CONSTRAINT [DF_MachineJobMoves_InIDEmployee] DEFAULT (0) ,
	[OutDateTime] [smalldatetime] NULL ,
	[OutIDEmployee] [int] NULL CONSTRAINT [DF_MachineJobMoves_OutIDEmployee] DEFAULT (0) ,
	[InIDReader] [tinyint] NULL ,
	[OutIDReader] [tinyint] NULL ,
	[IDJob] [numeric](18, 0) NULL ,
	[Processed] [bit] NOT NULL CONSTRAINT [DF_MachineJobMoves_Processed] DEFAULT (0),
	[IDIncidence] [tinyint] NOT NULL CONSTRAINT [DF_MachineJobMoves_IDIncidence] DEFAULT (0),
	[ID] [numeric](16, 0) IDENTITY (1, 1) NOT NULL ,
	[Pieces1] [numeric](9, 2) NOT NULL CONSTRAINT [DF_MachineJobMoves_Pieces1] DEFAULT (0),
	[Pieces2] [numeric](9, 2) NOT NULL CONSTRAINT [DF_MachineJobMoves_Pieces2] DEFAULT (0),
	[Pieces3] [numeric](9, 2) NOT NULL CONSTRAINT [DF_MachineJobMoves_Pieces3] DEFAULT (0),
	CONSTRAINT [PK_MachineJobMoves] PRIMARY KEY  NONCLUSTERED 
	(
		[ID]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_MachineJobMoves_JobIncidences] FOREIGN KEY 
	(
		[IDIncidence]
	) REFERENCES [JobIncidences] (
		[ID]
	),
	CONSTRAINT [FK_MachineJobMoves_Jobs] FOREIGN KEY 
	(
		[IDJob]
	) REFERENCES [Jobs] (
		[ID]
	)
) ON [PRIMARY]
GO


-- CREAMOS TABLA DE ESTADO DE CALCULO DE MAQUINAS
CREATE TABLE [DailyMachineSchedule] (
	[IDMachine] [tinyint] NOT NULL ,
	[Date] [smalldatetime] NOT NULL ,
	[JobStatus] [tinyint] NOT NULL CONSTRAINT [DF_DailyMachineSchedule_JobStatus] DEFAULT (0),
	CONSTRAINT [PK_DailyMachineSchedule] PRIMARY KEY  NONCLUSTERED 
	(
		[IDMachine],
		[Date]
	)  ON [PRIMARY] ,
	CONSTRAINT [FK_DailyMachineSchedule_Machine] FOREIGN KEY 
	(
		[IDMachine]
	) REFERENCES [Machines] (
		[ID]
	)
) ON [PRIMARY]
GO


-- MODIFICAMOS LA TABLA DE ACUMULADOS DIARIOS DE MAQUINAS
alter table DailyMachineAccruals add
        [IDIncidence] [tinyint]  NULL ,
	[Pieces1] [numeric](9, 2) NULL CONSTRAINT [DF_DailyMachineAccruals_Pieces1] DEFAULT (0),
	[Pieces2] [numeric](9, 2) NULL CONSTRAINT [DF_DailyMachineAccruals_Pieces2] DEFAULT (0),
	[Pieces3] [numeric](9, 2)  NULL CONSTRAINT [DF_DailyMachineAccruals_Pieces3] DEFAULT (0),
	[Performance] [numeric](10, 2) NULL CONSTRAINT [DF__DailyMachineAccruals__Perfo__7F21C18E] DEFAULT (0),
	[PieceTime] [numeric](18, 6) NULL CONSTRAINT [DF__DailyMachineAccruals__Piece__0015E5C7] DEFAULT (0),
	[TotalPieces] [numeric](18, 6) NULL CONSTRAINT [DF__DailyMachineAccruals__Total__010A0A00] DEFAULT (0)
GO

alter table DailyMachineAccruals DROP CONSTRAINT PK_DailyMachineAccruals
go
alter table DailyMachineAccruals alter column IDIncidence [tinyint] not null
GO
alter table DailyMachineAccruals add CONSTRAINT [PK_DailyMachineAccruals] PRIMARY KEY  NONCLUSTERED 
	(
		[IDMachine],
		[IDJob],
		[IDIncidence],
		[Date]
	)  ON [PRIMARY] 
GO
ALTER TABLE EMPLOYEES ADD MachinesController [bit] default(0)
GO
UPDATE EMPLOYEES SET MachinesController=0 WHERE MachinesController IS NULL
GO
ALTER TABLE MACHINES ADD IDTerminal [tinyint] default(0)
GO
UPDATE MACHINES SET IDTerminal=0 WHERE IDTerminal IS NULL
GO

ALTER TABLE [dbo].[JobIncidences] ADD [Image] [Image] NULL
GO


-- Añade pantalla de Estado de Planta
INSERT INTO sysroGUI(IDPath,LanguageReference,URL,IconURL,SecurityFlags,Priority,AllowedSecurity,RequiredFeatures) 
	VALUES ('NavBar\Job\JobsLive','JobsLive','roFormJobsLive.vbd','JobsLive.ico','3111111111',699,'NRW','Forms\JobIncidences')
GO


--
-- Actualiza versión de la base de datos
--

UPDATE sysroParameters SET Data='190' WHERE ID='DBVersion'
GO
