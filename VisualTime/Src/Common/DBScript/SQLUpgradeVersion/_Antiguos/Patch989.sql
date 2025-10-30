UPDATE
    [dbo].[Punches]
SET
    Punches.SystemDetails = CONCAT('Type=',CONVERT(NVARCHAR,Punches.Type),',','ActualType=',CONVERT(NVARCHAR,Punches.ActualType)),
	Punches.Type = Punches.Type*10 + AUX.rownumber1 +50,
	Punches.ActualType = Punches.Type*10 + AUX.rownumber1 +50
FROM 
	[dbo].[Punches]
	INNER JOIN (
	SELECT ROW_NUMBER() OVER (PARTITION BY IDEmployee,DateTime,Type,ActualType,TypeData,IDTerminal order by id desc) AS rownumber1, * FROM Punches
	) AUX ON AUX.ID = Punches.ID
WHERE
   AUX.rownumber1 > 1
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = 'UC_RepeteatedPunches')
BEGIN
    ALTER TABLE [dbo].[Punches]
		ADD CONSTRAINT UC_RepeteatedPunches UNIQUE (IDEmployee,DateTime,Type,ActualType,TypeData,IDTerminal)
END
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='989' WHERE ID='DBVersion'
GO
