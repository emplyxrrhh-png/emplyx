IF EXISTS (SELECT * FROM sysroLiveAdvancedParameters where ParameterName = 'Customization' AND Value = 'TAIF')
BEGIN

EXEC('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO1]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @conceptIDs Table(idConcept int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,'','');

Select CEG.EmployeeName, DA.Date, DA.idConcept, C.ShortName as ConceptShortName, C.idType as ConceptType, C.Name as ConceptName, DA.Value,  CEG.GroupName, CEG.FullGroupName, CarryOver, StartUpValue, FEUF.* 
	From DailyAccruals as DA 
		inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, @pendDate, 112)) as FEUF on FEUF.idEmployee = DA.IDEmployee 
		Inner Join sysrovwCurrentEmployeeGroups as CEG on CEG.idEmployee = DA.idEmployee 
		Left Join Concepts C on C.id=DA.idConcept 
		where DA.IDEmployee IN (SELECT * FROM @employeeIDs)	
		and DA.Date between @pinitialDate and @pendDate
	and DA.idConcept in (SELECT * FROM @conceptIDs)
	Order by DA.idEmployee, DA.Date
')

EXEC('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO3]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @conceptsFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @conceptIDs Table(idConcept int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pconceptsFilter nvarchar(max) = @conceptsFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @conceptIDs select * from dbo.SplitToInt(@pconceptsFilter,'','');

select CEG.EmployeeName,DAP.idConcept,DAP.Total, C.ShortName as ConceptShortName, C.idType as ConceptType, C.Name as ConceptName, 
		CEG.GroupName, CEG.FullGroupName, FEUF.*  
		from 
		(
		Select DA.IDEmployee, DA.idConcept, sum(DA.Value) as Total
			From DailyAccruals as DA 	
				Left Join Concepts C on C.id=DA.idConcept
			WHERE DA.idEmployee IN (SELECT * FROM @employeeIDs)	
				and DA.Date between @pinitialDate and @pendDate
			group by DA.idEmployee, DA.IDConcept
		) as DAP
			inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, @pendDate, 112)) as FEUF on FEUF.idEmployee = DAP.IDEmployee 
			Inner Join sysrovwCurrentEmployeeGroups as CEG on CEG.idEmployee = DAP.idEmployee 
			Left Join Concepts C on C.id=DAP.idConcept 
		WHERE DAP.idEmployee IN (SELECT * FROM @employeeIDs) and DAP.idConcept in (SELECT * FROM @conceptIDs)
')

EXEC('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO5]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @causesFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @causesIDs Table(idConcept int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pcausesFilter nvarchar(max) = @causesFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,'','');

Select CEG.EmployeeName, DS.Date, 
 C.ShortName as CauseShortName, C.Name as CauseName, DC.Value, 
 ISNULL(DS.IDShiftUsed, DS.IDShift1) AS IDShift, s.name as ShiftName, S.ShortName as ShiftShortName, 
 CEG.GroupName, CEG.FullGroupName, FEUF.*
	From DailySchedule as Ds
	inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, @pendDate, 112)) as FEUF on FEUF.idEmployee = DS.IDEmployee 
	Inner Join sysrovwCurrentEmployeeGroups as CEG on cEG.idEmployee = DS.idEmployee 
	Left join DailyCauses DC on DC.idEmployee=DS.idEmployee and DC.Date=DS.Date
	inner join Causes C on C.id=DC.idCause	
	inner join Shifts S on S.ID= ISNULL(DS.IDShiftUsed, DS.IDShift1)
Where DS.idEmployee IN (SELECT * FROM @employeeIDs)
		 and DS.Date between @pinitialDate and @pendDate
		  and DC.idCause in (SELECT * FROM @causesIDs)
order by dc.IDEmployee,DC.Date
')

EXEC('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO6]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @causesFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @causesIDs Table(idConcept int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pcausesFilter nvarchar(max) = @causesFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,'','');

Select CEG.EmployeeName, DS.Date, 
 C.ShortName as CauseShortName, C.Name as CauseName, DC.Value, 
 [1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],
 ISNULL(DS.IDShiftUsed, DS.IDShift1) AS IDShift, s.name as ShiftName, S.ShortName as ShiftShortName, 
 CEG.GroupName, CEG.FullGroupName, FEUF.*
	From DailySchedule as Ds
	inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, @pendDate, 112)) as FEUF on FEUF.idEmployee = DS.IDEmployee 
	Inner Join sysrovwCurrentEmployeeGroups as CEG on CEG.idEmployee = DS.idEmployee 
	Left join DailyCauses DC on DC.idEmployee=DS.idEmployee and DC.Date=DS.Date
	inner join Causes C on C.id=DC.idCause	
	inner join Shifts S on S.ID= ISNULL(DS.IDShiftUsed, DS.IDShift1)
	Left join (SELECT IDEmployee,ShiftDate,[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12] FROM 
		(
			select idemployee,shiftdate,datetime,RowNumber 
			from(
			select id,idemployee,shiftdate,datetime, ROW_NUMBER() OVER(PARTITION BY idemployee,shiftdate ORDER BY DATETIME) AS RowNumber 
			FROM PUNCHES
			WHERE idEmployee IN (SELECT * FROM @employeeIDs)
				and Punches.ShiftDate between @pinitialDate and @pendDate
			) as tabla
		) as PT 
		PIVOT (
		max(datetime) FOR rownumber IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS PIVOTTABLE) as P on P.IDEmployee =DC.IDEmployee and P.ShiftDate =DS.Date 	

Where DS.idEmployee IN (SELECT * FROM @employeeIDs)
		 and DS.Date between @pinitialDate and @pendDate
		  and DC.idCause in (SELECT * FROM @causesIDs)
order by dc.IDEmployee,DC.Date
')

EXEC('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO7]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @causesFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @causesIDs Table(idConcept int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @pcausesFilter nvarchar(max) = @causesFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @causesIDs select * from dbo.SplitToInt(@pcausesFilter,'','');

Select CEG.EmployeeName, DS.Date, 
 C.ShortName as CauseShortName, C.Name as CauseName, DC.Value, 
 [1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12],
 ISNULL(DS.IDShiftUsed, DS.IDShift1) AS IDShift, s.name as ShiftName, S.ShortName as ShiftShortName, 
 CEG.GroupName, CEG.FullGroupName, FEUF.*
	From DailySchedule as Ds
	inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, @pendDate, 112)) as FEUF on FEUF.idEmployee = DS.IDEmployee 
	Inner Join sysrovwCurrentEmployeeGroups as CEG on CEG.idEmployee = DS.idEmployee 
	Left join DailyCauses DC on DC.idEmployee=DS.idEmployee and DC.Date=DS.Date
	inner join Causes C on C.id=DC.idCause	
	inner join Shifts S on S.ID= ISNULL(DS.IDShiftUsed, DS.IDShift1)
	Left join (SELECT IDEmployee,ShiftDate,[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12] FROM 
		(
			select idemployee,shiftdate,datetime,RowNumber 
			from(
			select id,idemployee,shiftdate,datetime, ROW_NUMBER() OVER(PARTITION BY idemployee,shiftdate ORDER BY DATETIME) AS RowNumber 
			FROM PUNCHES
			WHERE idEmployee IN (SELECT * FROM @employeeIDs)
				and Punches.ShiftDate between @pinitialDate and  @pendDate 	) as tabla
		) as PT 
		PIVOT (
		max(datetime) FOR rownumber IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS PIVOTTABLE) as P on P.IDEmployee =DC.IDEmployee and P.ShiftDate =DS.Date 	

Where DS.idEmployee IN (SELECT * FROM @employeeIDs)
		 and DS.Date between  @pinitialDate and @pendDate  
		  and DC.idCause in (SELECT * FROM @causesIDs)
order by dc.IDEmployee,DC.Date

')

exec('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO8]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate

select EF.idpassport, P.Name as EmployeeName, EF.idfeature, F.Name as FeatureName, EF.Permission,   
		PermissionName = CASE EF.Permission
			 WHEN 0 THEN ''Nada''
			 WHEN 3 THEN ''Lectura''
			 WHEN 6 THEN ''Escritura''
			 WHEN 9 THEN ''Administrador''
			 ELSE ''??''
		END
		, GroupName, FullGroupName, FEUF.*
	from GetEmployeeFeatures() EF  
		INNER JOIN SYSROPASSPORTS P	ON EF.idPassport=P.ID 
		INNER JOIN SYSROFEATURES F ON F.ID=EF.IDFEATURE
		inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, getdate(), 112)) as FEUF on FEUF.idEmployee = P.IDEmployee 	
		Inner Join sysrovwCurrentEmployeeGroups as CEG on CEG.idEmployee = P.idEmployee 
	Where P.idEmployee IN (SELECT * FROM @employeeIDs)
		and EF.Permission<>0		
	Order by P.Name, F.Name

')

EXEC('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO9]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @requestTypesFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @requestsIDs Table(idRequest int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @prequestsFilter nvarchar(max) = @requestTypesFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate
insert into @requestsIDs select * from dbo.SplitToInt(@prequestsFilter,'','');

SELECT Name as Supervisor, tmpTbl.EmployeeName, tmpTbl.RequestDate, tmptbl.Comments, tmpTbl.TipoSolicitud, tmpTbl.Estado, tmpTbl.GroupName, tmpTbl.FullGroupName, FEUF.* 
from sysroPassports SRP
inner join
	(SELECT Requests.*, CEG.EmployeeName, CEG.GroupName, CEG.FullGroupName,
		TipoSolicitud=case 
			when RequestType=1 then ''Cambio Campos Ficha''
			when RequestType=2 then ''Fichaje Olvidado''
			when RequestType=3 then ''Fichaje Justificado''
			when RequestType=4 then ''ExternalWorkResumePart''
			when RequestType=5 then ''Cambio Horario''
			when RequestType=6 then ''Vacaciones o Permisos''
			when RequestType=7 then ''Previsión días de ausencia''
			when RequestType=8 then ''Intercambio Horarios''
			when RequestType=9 then ''Previsión horas de ausencia''
			when RequestType=10 then ''Fichaje olvidado de tareas''
			when RequestType=11 then ''Cancelar Vacaciones''
			when RequestType=12 then ''Fichaje olvidado de CC''
		END,
		Estado=case
			When Requests.Status=0 then ''Pendiente''
			When Requests.Status=1 then ''En Proceso''
			When Requests.Status=2 then ''Aceptada''
			When Requests.Status=3 then ''Denegada''
	END,
		(SELECT TOP 1 RequestsApprovals.DateTime FROM RequestsApprovals WHERE RequestsApprovals.IDRequest = Requests.ID ORDER BY DateTime DESC) AS LastRequestApprovalDate, 
		(SELECT TOP 1 RequestsApprovals.IDPassport FROM RequestsApprovals WHERE RequestsApprovals.IDRequest = Requests.ID ORDER BY DateTime DESC) AS LastRequestApprovalIDPassport, 
		(SELECT TOP 1 sysroPassports.Name FROM RequestsApprovals INNER JOIN sysroPassports ON RequestsApprovals.IDPassport = sysroPassports.ID WHERE RequestsApprovals.IDRequest = Requests.ID ORDER BY DateTime DESC) AS LastRequestApprovalPassportName
	FROM Requests 
		INNER JOIN sysrovwCurrentEmployeeGroups CEG ON Requests.IDEmployee = CEG.IDEmployee 
	WHERE Requests.idEmployee in (SELECT * FROM @employeeIDs) and Requests.RequestDate between @pinitialDate AND @pendDate
	) tmpTbl on (SELECT COUNT(*) FROM sysrovwSecurity_PermissionOverRequests WHERE IdPassport = SRP.ID AND IdRequest = tmpTbl.ID AND Permission > 3 AND SupervisorLevelOfAuthority = 1) > 0
	inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, @pendDate, 112)) as FEUF on FEUF.idEmployee = tmpTbl.IDEmployee 
where srp.GroupType <> ''U''
ORDER BY Name, RequestDate ASC

')

EXEC('ALTER PROCEDURE [dbo].[Genius_Custom_IVECO10]
@initialDate smalldatetime, @endDate smalldatetime, @idpassport int, @employeeFilter nvarchar(max), @userFieldsFilter nvarchar(max), @includeZeros tinyint AS
DECLARE @employeeIDs Table(idEmployee int)
DECLARE @pinitialDate smalldatetime = @initialDate, @pendDate smalldatetime = @endDate, @pidpassport int = @idpassport, @pemployeeFilter nvarchar(max) = @employeeFilter, @puserFieldsFilter nvarchar(max) = @userFieldsFilter, @pincludeZeros tinyint = @includeZeros
DECLARE @intdatefirst int
SELECT @intdatefirst = CONVERT(xml, sysroParameters.Data).value(''(/roCollection/Item[@key=("WeekPeriod")]/text())[1]'', ''nvarchar(max)'') FROM sysroParameters WHERE ID = ''OPTIONS''
SET DateFirst @intdatefirst;

insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@initialDate,@pendDate

Select CEG.EmployeeName, P.shiftdate as Date,   
 [1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12], 
 CEG.GroupName, CEG.FullGroupName, FEUF.*	
	FROM sysrovwCurrentEmployeeGroups as CEG 
	inner join Fiat_ExplodeEmployeeUserFields(CONVERT(VARCHAR, @pendDate, 112)) as FEUF on FEUF.idEmployee = CEG.IDEmployee 	
	Inner join (SELECT IDEmployee,ShiftDate,[1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12] FROM 
		(
			select idemployee,shiftdate,datetime,RowNumber 
			from(
			select id,idemployee,shiftdate,datetime, ROW_NUMBER() OVER(PARTITION BY idemployee,shiftdate ORDER BY DATETIME) AS RowNumber 
			FROM PUNCHES
			WHERE idEmployee in (select * from @employeeIDs) 
				and Punches.ShiftDate between @pinitialDate and @pendDate
			) as tabla
		) as PT 
		PIVOT (
		max(datetime) FOR rownumber IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
		) AS PIVOTTABLE) as P on P.IDEmployee =CEG.IDEmployee
Where CEG.idEmployee in (select * from @employeeIDs) and P.shiftdate between @pinitialDate and @pendDate
order by CEG.IDEmployee, P.shiftdate

')

UPDATE GeniusViews SET CubeLayout = 'empty' WHERE NAME = 'Saldos diarios entre fechas' AND DSFunction LIKE 'Genius_Custom_IVECO1(%'

END
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='891' WHERE ID='DBVersion'
GO
