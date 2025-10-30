-- Permitir creas masterASP con y sin rele 

UPDATE sysroReaderTemplates
	SET Output = '1,0'
WHERE type = 'masterASP' 
GO

--  Borrar del selector de creacion muchos terminales que no se crean desde VTLive
UPDATE sysroReaderTemplates
	SET Direction = null
where (Type like 'rx%' OR Type = 'mx9' OR Type = 'mxV') and
Direction is not null
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='800' WHERE ID='DBVersion'
GO
