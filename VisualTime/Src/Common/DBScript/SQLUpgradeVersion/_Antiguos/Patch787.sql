-- No borréis esta línea
ALTER VIEW [dbo].[sysrovwSecurity_PermissionOverEmployees]  
AS  
   SELECT POG.IdPassport,  
    POG.IsRoboticsUser,  
    EG.IdEmployee,  
    EG.BeginDate,  
    EG.EndDate  
   FROM   Groups WITH (nolock)  
    INNER JOIN (SELECT idgroup, idpassport, path, CASE CHARINDEX('@@ROBOTICS@@',SP.Description) WHEN 0 THEN 0 ELSE 1 END AS IsRoboticsUser  
       FROM   sysropassports_groups SPG WITH (nolock)  
       INNER JOIN groups ON groups.id = SPG.idgroup  
       INNER JOIN sysroPassports SP ON SP.Id = SPG.IDPassport AND SP.IsSupervisor = 1  
      ) POG ON groups.path = POG.path OR groups.path LIKE POG.path + '\%'  
    INNER JOIN dbo.sysrovwgetemployeegroup AS EG WITH (nolock) ON groups.id = EG.idgroup  
    LEFT OUTER JOIN sysroPassports_Employees SPE WITH (nolock) ON SPE.IDPassport = POG.idpassport AND SPE.IDEmployee = EG.idemployee  
   WHERE SPE.Permission IS NULL
   UNION  
   SELECT SPE.IdPassport,  
    0 AS IsRoboticsUser,  
    SPE.IdEmployee,  
    CONVERT(smalldatetime,'1900-01-01',120) AS BeginDate,  
    CONVERT(smalldatetime,'2079-01-01',120) AS EndDate  
   FROM   sysroPassports_Employees SPE  
   WHERE  SPE.Permission = 1   
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='787' WHERE ID='DBVersion'
GO
