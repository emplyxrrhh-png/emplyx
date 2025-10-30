CREATE PROCEDURE [dbo].[Report_seguimiento_solicitudes]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@fechaStart datetime2 = '20210301',
	@fechaEnd datetime2 = '20210301',
	@IDUserField nvarchar(100) = '0'
AS
select emp.id, emp.name, ec.idcontract, srt.idtype as IDRequestType, srt.Type, req.RequestDate, req.Status reqstatus, req.id as idrequest, req.Date1, req.Date2, sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.GroupName, EmployeeUserFieldValues.Value as campoFicha 
from requests req with (nolock) 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on req.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = req.IDEmployee And cast(req.RequestDate as date) between poe.BeginDate And poe.EndDate 
inner join sysroRequestType srt with (nolock) on req.RequestType = srt.idtype
inner join employees emp with (nolock) on emp.id = req.IDEmployee
inner join EmployeeContracts ec with (nolock) on ec.IDEmployee = req.IDEmployee and req.RequestDate between ec.BeginDate and ec.EndDate
inner join sysroEmployeeGroups with (nolock) ON emp.ID=sysroEmployeeGroups.IDEmployee and req.RequestDate between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate 
inner join Groups with (nolock) ON sysroEmployeeGroups.IDGroup=Groups.ID 
left join EmployeeUserFieldValues with (nolock) on EmployeeUserFieldValues.IDEmployee = emp.ID and EmployeeUserFieldValues.FieldName = (select FieldName from sysroUserFields Where ID = @IDUserField) and (EmployeeUserFieldValues.date >= getDate() or EmployeeUserFieldValues.date = '1900-01-01 00:00:00')
where cast(RequestDate as date) between cast(@fechaStart as date) and cast(@fechaEnd as date) 
ORDER BY sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.GroupName, emp.Name
RETURN NULL
GO

CREATE FUNCTION [dbo].[GetDirectSupervisorsByRequestAndEmployee]
    (	
      	@idRequest int,
      	@idEmployee int
    )
    RETURNS nvarchar(1000)
    AS
    BEGIN
      DECLARE @RetNames nvarchar(1000)
  	SET @RetNames = ''
  	DECLARE @tmpTable Table(PassportID int,LevelOfAuthority int ,Name nvarchar(50))
  	insert into @tmpTable 
  	SELECT IdPassport, SupervisorLevelOfAuthority as LevelOfAuthority, Name  
	FROM   
	(  
	SELECT IdRequest, sysroPassports.IdEmployee, IdPassport, sysroPassports.Name, IsRoboticsUser, Permission, SupervisorLevelOfAuthority, ShowFromLevelOfAuthority, RequestCurrentStatus, RequestCurrentApprovalLevel, AutomaticValidation,       
		MAX(CASE WHEN SupervisorLevelOfAuthority < RequestCurrentApprovalLevel AND Permission > 3 THEN SupervisorLevelOfAuthority ELSE 0 END)  OVER (PARTITION BY IdRequest Order By IdRequest ASC) AS NextLevelOfAuthorityRequired   
	FROM sysrovwSecurity_PermissionOverRequests 
	INNER JOIN sysroPassports on sysrovwSecurity_PermissionOverRequests.IdPassport = sysroPassports.ID 
	WHERE  Permission >= 3 and sysrovwSecurity_PermissionOverRequests.IdEmployee = @idEmployee and IdRequest = @idRequest
	AND RequestCurrentApprovalLevel > SupervisorLevelOfAuthority AND AutomaticValidation = 0 AND IsRoboticsUser = 0
	) AUX WHERE (SupervisorLevelOfAuthority = NextLevelOfAuthorityRequired AND Permission > 3)
  	SET @RetNames = (SELECT  Name  + ','  AS [text()] from @tmpTable tt where  tt.LevelOfAuthority = (SELECT max(LevelOfAuthority) from @tmpTable) For XML PATH (''))
  	
  	IF @RetNames <> ''
      	BEGIN
      		SET @RetNames =  LEFT(@RetNames,LEN(@RetNames) - 1)
      	END
  		
  	RETURN @RetNames
     END
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='835' WHERE ID='DBVersion'
GO
