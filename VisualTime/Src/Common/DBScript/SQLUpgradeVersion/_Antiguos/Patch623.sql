CREATE VIEW [dbo].[sysrovwGetRequestPassportPermissionv3] AS
	SELECT Requests.id as IDRequest 
	, gpove.IDPassport, vwgpof.Permision
	FROM Requests  WITH (NOLOCK)
	INNER JOIN sysrovwCurrentEmployeeGroups with (nolock) ON Requests.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee 
	INNER JOIN dbo.sysroFeatures WITH (NOLOCK) on sysroFeatures.AliasId = Requests.RequestType
	INNER JOIN sysrovwGetPermissionOverEmployeev3 gpove  WITH (NOLOCK) on gpove.EmployeeID = Requests.IDEmployee 
	INNER JOIN (select id as passportID , idparentpassport, IDGroupFeature, (select BusinessGroupList from sysroPassports z WITH (NOLOCK) where z.id = sysropassports.IDParentPassport) as BusinessGroupList   from sysropassports WITH (NOLOCK) WHERE IsSupervisor=1 and grouptype <> 'U' AND iduser IS NOT NULL AND description NOT LIKE '%@@ROBOTICS@@%' ) as passports on gpove.idpassport = passports.passportID
	INNER JOIN sysrovwGetPermissionOverFeaturev3 vwgpof  WITH (NOLOCK)  on sysroFeatures.Alias = vwgpof.Alias  and vwgpof.IDGroupFeature =  passports.IDGroupFeature
	INNER JOIN (select id as CauseID, isnull(BusinessGroup,'') as BusinessGroup from Causes WITH (NOLOCK)) as causes  on ISNULL(Requests.idcause,0) = causes.CauseID
	INNER JOIN (SELECT Shifts.id as ShiftID, ISNULL(ShiftGroups.BusinessGroup, '') AS BusinessGroup FROM Shifts  WITH (NOLOCK) LEFT OUTER JOIN ShiftGroups  WITH (NOLOCK) ON Shifts.IDGroup = ShiftGroups.ID) as Shifts  on ISNULL(Requests.IDShift,0) = Shifts.ShiftID
	AND sysrovwCurrentEmployeeGroups.IDGroup = gpove.IDGroup  AND convert(date,getdate()) BETWEEN  gpove.BeginDate AND gpove.EndDate
	WHERE
	1 = case when Requests.RequestType = 12 then (SELECT count(idcenter) from sysroPassports_Centers c WITH (NOLOCK) where c.idcenter = Requests.IDCenter AND c.IDPassport = passports.idparentpassport) else 1 end
	AND
	1 = case when len(BusinessGroupList) > 0 AND len(CAUSES.BusinessGroup) > 0 and IDCause > 0 and charindex(CAUSES.BusinessGroup, BusinessGroupList) = 0 then 0 else 1 end
	AND
	1 = case when len(BusinessGroupList) > 0 AND len(Shifts.BusinessGroup) > 0 and IDShift > 0 and charindex(Shifts.BusinessGroup, BusinessGroupList) = 0 then 0 else 1 end
GO



CREATE VIEW [dbo].[sysrovwGetMaxPassportLevelOfAuthorityv3] AS
		select IDRequest, Level from (
select * , ROW_NUMBER() over(partition by IDRequest order by Level desc )  as rowlevel from (
	   select * from (
	   SELECT sysroPassports.ID , IDRequest, Isnull(Requests.StatusLevel, 12)  as  statuslevel,
	   isnull(case when Requests.RequestType = 7 or Requests.RequestType = 9 or Requests.RequestType = 13 or Requests.RequestType = 14   then
			(select LevelOfAuthority FROM sysroPassports_Categories WITH (NOLOCK) WHERE IDPassport = sysroPassports.ID AND IDCategory IN(select IDCategory FROM Causes WITH (NOLOCK) where Id= Requests.IDCause))
		else 
			(select LevelOfAuthority FROM sysroPassports_Categories WITH (NOLOCK) WHERE IDPassport = sysroPassports.ID AND IDCategory IN(select IDCategory FROM sysroRequestType WITH (NOLOCK) where IdType= Requests.RequestType))
		end,15) as level
	   FROM sysroPermissionsOverRequests  WITH (NOLOCK)
	    inner join sysroPassports WITH (NOLOCK) on sysroPassports.IDParentPassport = sysroPermissionsOverRequests.IDParentPassport
		inner join Requests WITH (NOLOCK) on Requests.id = sysroPermissionsOverRequests.IDRequest
		where Requests.status IN ( 0, 1 )  AND Requests.automaticvalidation = 0       AND sysropassports.issupervisor = 1       
		) tmp
		where level  < statuslevel 

			) as tmptable )   tmpglobal
			where rowlevel=1
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='623' WHERE ID='DBVersion'
GO
