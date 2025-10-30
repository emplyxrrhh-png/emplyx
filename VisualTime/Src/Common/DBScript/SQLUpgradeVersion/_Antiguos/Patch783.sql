-- No borréis esta línea
DROP VIEW IF EXISTS [dbo].[sysrovwSecurity_PendingRequestsDependencies]
GO
CREATE VIEW [dbo].[sysrovwSecurity_PendingRequestsDependencies]  
AS  
SELECT  IdRequest, IdEmployee, IdPassport, IsRoboticsUser, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, NextLevelOfAuthorityRequired, AutomaticValidation, CASE WHEN (SupervisorLevelOfAuthority = NextLevelOfAuthorityRequired) THEN 1 ELSE 0 END  AS DirectDependence  
FROM   
(  
SELECT IdRequest, IdEmployee, IdPassport, IsRoboticsUser, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, AutomaticValidation,       
    MAX(CASE WHEN SupervisorLevelOfAuthority < RequestCurrentApprovalLevel AND Permission >= 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdRequest Order By IdRequest ASC) AS NextLevelOfAuthorityRequired   
FROM sysrovwSecurity_PermissionOverRequests WHERE  Permission >= 3 AND RequestCurrentStatus IN (0,1)  AND RequestCurrentApprovalLevel > SupervisorLevelOfAuthority
) AUX   
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='783' WHERE ID='DBVersion'
GO
