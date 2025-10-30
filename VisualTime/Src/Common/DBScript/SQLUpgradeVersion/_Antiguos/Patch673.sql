-- No borréis esta línea

CREATE VIEW [dbo].[sysrovwNonSupervisedDepartments]	
AS
SELECT g.id, g.name, g.path
FROM groups AS g
LEFT JOIN (
    SELECT DISTINCT g.path AS auxpath
    FROM sysroPassports_Groups AS spg
    INNER JOIN groups AS g ON g.ID = spg.IDGroup
    INNER JOIN sysroPassports AS sp ON sp.id = spg.IDPassport
          AND sp.Description NOT LIKE '@@ROBOTICS@@%'
) AS aux ON aux.auxpath = g.path OR g.path LIKE CONCAT(aux.auxpath, '%')
WHERE aux.auxpath IS NULL
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='673' WHERE ID='DBVersion'
GO
