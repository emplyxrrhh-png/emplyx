IF NOT EXISTS(SELECT * FROM sys.columns 
            WHERE Name = N'Export' AND Object_ID = Object_ID(N'Shifts'))
BEGIN
	ALTER TABLE Shifts ADD Export nvarchar(5) NULL
END
GO

IF EXISTS(SELECT * FROM sys.columns 
            WHERE Name = N'Export' AND Object_ID = Object_ID(N'Shifts'))
BEGIN

	DECLARE @ExportName varchar(3), @IdShift int, @nullCount int, @nullName varchar(3)
	set @nullCount = 1
	set @nullName = 'SNC'	

	DECLARE cShifts CURSOR FOR    
	select id,ShortName from Shifts

	OPEN cShifts

	FETCH cShifts INTO @IdShift, @ExportName
	WHILE (@@FETCH_STATUS = 0 )
	BEGIN
		
		DECLARE @counter INT, @counterExport int, @newExportName varchar(5), @newNullExportName varchar(5)
		--Miro cuantas veces se reptie el nombre corto
		IF (@ExportName IS NOT NULL)
		BEGIN
			select @counter = COUNT(ShortName) from Shifts
			WHERE ShortName = @ExportName
			group by ShortName
		
			--si es solo 1 actualizo directamente el export
			IF(@counter = 1)
			BEGIN
				UPDATE Shifts 
				SET Export = @ExportName
				WHERE ID = @IdShift
			END
			--si ya existe creo un secuencial para añadir al nombre corto
			ELSE
			BEGIN
				SELECT @counterExport = COUNT(Export) FROM Shifts
				WHERE Export LIKE '%' + @ExportName + '%' 			
				set @newExportName = @ExportName + CONVERT(varchar(2),@counterExport+1)
				SET @newExportName = right('DDDDD'+convert(varchar(5), @newExportName), 5) 
			
				UPDATE Shifts 
				SET Export = LTRIM(RTRIM(@newExportName))
				WHERE ID = @IdShift
			END		
		END
		--si el nombre corto es nulo, asigno un nombre estándar con un secuencial
		ELSE
		BEGIN
			set @newNullExportName = @nullName +  CONVERT(varchar(2),@nullCount) 
			UPDATE Shifts 
			SET Export = LTRIM(RTRIM(@newNullExportName))
			WHERE ID = @IdShift	
			set @nullCount = @nullCount + 1
		END
		

		FETCH cShifts INTO @IdShift, @ExportName		

	END
	CLOSE cShifts
	DEALLOCATE cShifts
END
GO

ALTER TABLE Shifts ALTER COLUMN Export nvarchar(5) NOT NULL
GO

ALTER TABLE Shifts
ADD CONSTRAINT DF_Shifts_ExportValue UNIQUE (Export); 
GO


UPDATE dbo.sysroParameters SET Data='392' WHERE ID='DBVersion'
GO
