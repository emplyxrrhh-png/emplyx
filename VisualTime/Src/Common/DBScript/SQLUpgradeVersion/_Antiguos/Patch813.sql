ALTER TABLE [TMPDetailedCalendarEmployee]
ADD [IDConcept9] SMALLINT,
    [ConceptName9] NVARCHAR(3),
    [FormatConcept9] NVARCHAR(1),
    [ValueConcept9] NUMERIC(19,6),
    [IDConcept10] SMALLINT,
    [ConceptName10] NVARCHAR(3),
    [FormatConcept10] NVARCHAR(1),
    [ValueConcept10] NUMERIC(19,6),
    [IDConcept11] SMALLINT,
    [ConceptName11] NVARCHAR(3),
    [FormatConcept11] NVARCHAR(1),
    [ValueConcept11] NUMERIC(19,6),
    [IDConcept12] SMALLINT,
    [ConceptName12] NVARCHAR(3),
    [FormatConcept12] NVARCHAR(1),
    [ValueConcept12] NUMERIC(19,6),
    [IDConcept13] SMALLINT,
    [ConceptName13] NVARCHAR(3),
    [FormatConcept13] NVARCHAR(1),
    [ValueConcept13] NUMERIC(19,6),
    [IDConcept14] SMALLINT,
    [ConceptName14] NVARCHAR(3),
    [FormatConcept14] NVARCHAR(1),
    [ValueConcept14] NUMERIC(19,6),
    [IDConcept15] SMALLINT,
    [ConceptName15] NVARCHAR(3),
    [FormatConcept15] NVARCHAR(1),
    [ValueConcept15] NUMERIC(19,6);
GO

-- No borréis esta línea
CREATE OR ALTER VIEW [dbo].[sysrovwSecurity_PendingRequestsDependencies]  
AS  
SELECT  IdRequest, IdEmployee, IdPassport, IsRoboticsUser, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, NextLevelOfAuthorityRequired, AutomaticValidation, CASE WHEN (SupervisorLevelOfAuthority = NextLevelOfAuthorityRequired) THEN 1 ELSE 0 END  AS DirectDependence  
FROM   
(  
SELECT IdRequest, IdEmployee, IdPassport, IsRoboticsUser, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, AutomaticValidation,       
    MAX(CASE WHEN SupervisorLevelOfAuthority < RequestCurrentApprovalLevel AND Permission > 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdRequest Order By IdRequest ASC) AS NextLevelOfAuthorityRequired   
FROM sysrovwSecurity_PermissionOverRequests WHERE  Permission >= 3 AND RequestCurrentStatus IN (0,1)  AND RequestCurrentApprovalLevel > SupervisorLevelOfAuthority
) AUX   
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='813' WHERE ID='DBVersion'
GO
