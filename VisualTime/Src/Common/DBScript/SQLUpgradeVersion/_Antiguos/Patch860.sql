-- No borréis esta línea
ALTER TABLE Concepts DROP COLUMN HolidaysEnjoymentType 
GO
ALTER TABLE Concepts DROP COLUMN HolidaysEnjoymentValue 
GO
ALTER TABLE Concepts DROP COLUMN HolidaysEnjoymentLabAgrees 
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='860' WHERE ID='DBVersion'

GO
