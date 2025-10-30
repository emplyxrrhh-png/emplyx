alter view [dbo].[sysrovwCurrentVisistsStatus]  
     as  
SELECT v.idvisitor, 
       v.NAME VisitorName, 
       e.id, 
       e.NAME EmployeeName, 
       euf.value, 
       LastMoves.datepunch 
FROM   [dbo].[visit_visitor] vv 
       INNER JOIN [dbo].[visitor] v 
               ON vv.idvisitor = v.idvisitor 
       INNER JOIN [dbo].[visit] vi 
               ON vv.idvisit = vi.idvisit 
       INNER JOIN [dbo].[employees] e 
               ON vi.idemployee = e.id 
       INNER JOIN [dbo].[employeeuserfieldvalues] euf 
               ON e.id = euf.idemployee 
                  AND euf.date = (SELECT TOP 1 date 
                                  FROM   employeeuserfieldvalues 
                                  WHERE  date < Getdate() 
                                         AND fieldname LIKE 
                                             (SELECT fieldname 
                                              FROM   [dbo].[sysrouserfields] 
                                              WHERE 
                                             [description] LIKE 
                                             'Info.EmergencyPointInfo') 
											 and IDEmployee = e.ID
                                  ORDER  BY date DESC) 
       INNER JOIN (SELECT Row_number() 
                            OVER( 
                              partition BY idvisit, idvisitor 
                              ORDER BY idvisit, idvisitor, datepunch DESC) AS 
                                                                  "Row Number", 
                          * 
                   FROM   visit_visitor_punch 
                  ) AS 
                                                                  LastMoves 
               ON LastMoves.idvisit = vi.idvisit 
                  AND LastMoves.idvisitor = vv.idvisitor 
                  AND [row number] = 1 
WHERE  fieldname LIKE (SELECT fieldname 
                       FROM   [dbo].[sysrouserfields] 
                       WHERE  [description] LIKE 'Info.EmergencyPointInfo') 
       AND vi.status = 1 
       AND LastMoves.datepunch >= Dateadd(n, -2880, Getdate()) 
       AND LastMoves.action = 'IN' 
GO

-- Se añaden permisos para que Consultores, Técnicos y Soporte puedan dar de alta terminales. (v1)
delete [dbo].[sysroPassports_PermissionsOverFeatures]
where IDPassport in (select id 
					from [sysroPassports]
					where  grouptype='U'
					and ([dbo].[sysroPassports].[Description] like '%@@Soporte' 
					or [dbo].[sysroPassports].[Description] like '%@@Tecnicos'
					or [dbo].[sysroPassports].[Description] like '%@@Consultores'))
and IDFeature like '8%'
GO

insert into [dbo].[sysroPassports_PermissionsOverFeatures](IDPassport, IDFeature, Permission) 
select [dbo].[sysroPassports].ID,[dbo]. [sysroFeatures].[ID],9
from  [dbo].[sysroPassports], [dbo].[sysroFeatures]
where  grouptype='U'
and [dbo].[sysroPassports].[Description] like '%@@Consultores'
and [dbo].[sysroFeatures].ID like '8%'
GO

insert into [dbo].[sysroPassports_PermissionsOverFeatures](IDPassport, IDFeature, Permission) 
select [dbo].[sysroPassports].ID, [dbo].[sysroFeatures].[ID],9
from  [dbo].[sysroPassports], [dbo].[sysroFeatures]
where  grouptype='U'
and [dbo].[sysroPassports].[Description] like '%@@Tecnicos'
and [dbo].[sysroFeatures].ID like '8%'
GO

insert into [dbo].[sysroPassports_PermissionsOverFeatures](IDPassport, IDFeature, Permission) 
select [dbo].[sysroPassports].ID, [dbo].[sysroFeatures].[ID],9
from  [dbo].[sysroPassports], [dbo].[sysroFeatures]
where  grouptype='U'
and [dbo].[sysroPassports].[Description] like '%@@Soporte'
and [dbo].[sysroFeatures].ID like '8%'
GO


-- Se añaden permisos para que Consultores, Técnicos y Soporte puedan dar de alta terminales. (v2)
delete [dbo].[sysroGroupFeatures_PermissionsOverFeatures]
where IDGroupFeature in (select id 
					from sysroGroupFeatures
					where  ([dbo].sysroGroupFeatures.name like '%@@Soporte' 
					or [dbo].sysroGroupFeatures.name like '%@@Tecnicos'
					or [dbo].sysroGroupFeatures.name like '%@@Consultores'))
and IDFeature like '8%'
GO

insert into [dbo].[sysroGroupFeatures_PermissionsOverFeatures](IDGroupFeature, IDFeature, Permision) 
select [dbo].sysroGroupFeatures.ID,[dbo]. [sysroFeatures].[ID],9
from  [dbo].sysroGroupFeatures, [dbo].[sysroFeatures]
where  [dbo].sysroGroupFeatures.name like '%@@Consultores'
and [dbo].[sysroFeatures].ID like '8%'
GO

insert into [dbo].[sysroGroupFeatures_PermissionsOverFeatures](IDGroupFeature, IDFeature, Permision) 
select [dbo].sysroGroupFeatures.ID, [dbo].[sysroFeatures].[ID],9
from  [dbo].sysroGroupFeatures, [dbo].[sysroFeatures]
where   [dbo].sysroGroupFeatures.name like '%@@Tecnicos'
and [dbo].[sysroFeatures].ID like '8%'
GO

insert into [dbo].[sysroGroupFeatures_PermissionsOverFeatures](IDGroupFeature, IDFeature, Permision) 
select [dbo].sysroGroupFeatures.ID, [dbo].[sysroFeatures].[ID],9
from  [dbo].sysroGroupFeatures, [dbo].[sysroFeatures]
where  [dbo].sysroGroupFeatures.name like '%@@Soporte'
and [dbo].[sysroFeatures].ID like '8%'
GO


-- Se añaden permisos para que Consultores, Técnicos y Soporte puedan ver las alertas.(v1)
delete [dbo].[sysroPassports_PermissionsOverFeatures] 
where IDPassport in (select id 
					from [sysroPassports]
					where  grouptype='U'
					and ([dbo].[sysroPassports].[Description] like '%@@Soporte' 
					or [dbo].[sysroPassports].[Description] like '%@@Tecnicos'))
and IDFeature = 7700
GO

insert into [dbo].[sysroPassports_PermissionsOverFeatures](IDPassport, IDFeature, Permission) 
select [dbo].[sysroPassports].ID, [dbo].[sysroFeatures].[ID], 3
from  [dbo].[sysroPassports], [dbo].[sysroFeatures]
where  grouptype='U'
and ([dbo].[sysroPassports].[Description] like '%@@Soporte' 
	or [dbo].[sysroPassports].[Description] like '%@@Tecnicos')
and [dbo].[sysroFeatures].ID = 7700
GO

-- Se añaden permisos para que Consultores, Técnicos y Soporte puedan ver las alertas. (v2)
delete [dbo].[sysroGroupFeatures_PermissionsOverFeatures] 
where IDGroupFeature in (select id 
					from sysroGroupFeatures
					where  ([dbo].sysroGroupFeatures.Name like '%@@Soporte' 
					or [dbo].sysroGroupFeatures.Name like '%@@Tecnicos'))
and IDFeature = 7700
GO

insert into [dbo].[sysroGroupFeatures_PermissionsOverFeatures](IDGroupFeature, IDFeature, Permision) 
select [dbo].sysroGroupFeatures.ID, [dbo].[sysroFeatures].[ID], 3
from  [dbo].sysroGroupFeatures, [dbo].[sysroFeatures]
where  ([dbo].sysroGroupFeatures.Name like '%@@Soporte' 
       or [dbo].sysroGroupFeatures.Name like '%@@Tecnicos')
and [dbo].[sysroFeatures].ID = 7700
GO

ALTER TABLE dbo.Causes ADD
	RequestAvailability nvarchar(MAX) NULL
GO

UPDATE dbo.Causes set RequestAvailability = '-1'
GO


 -- Añadir campo horas teoricas absolutas
  ALTER VIEW [dbo].[sysroEmployeesShifts]
  AS
  SELECT        dbo.Employees.ID AS IDEmployee, dbo.Employees.Name, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, 
                           dbo.Shifts.ID AS IDShift, dbo.Shifts.Name AS ShiftName, (CASE WHEN isnull(dbo.DailySchedule.IsHolidays, 0) = 1 THEN 0 ELSE isnull(dbo.DailySchedule.ExpectedWorkingHours, 
                           dbo.Shifts.ExpectedWorkingHours) END) AS ExpectedWorkingHours, dbo.DailySchedule.Date AS CurrentDate, dbo.sysroEmployeeGroups.SecurityFlags, 
                           dbo.GetFullGroupPathName(dbo.sysroEmployeeGroups.IDGroup) AS FullGroupName, dbo.Shifts.IsFloating, dbo.Shifts.StartFloating, CASE WHEN dbo.DailySchedule.Date <= GETDATE() 
                           THEN StartShiftUsed ELSE StartShift1 END AS StartFloatingOnDay, dbo.DailySchedule.IDAssignment, case when dbo.DailySchedule.IDAssignment > 0 then (select Name FROM dbo.Assignments WHERE ID=dbo.DailySchedule.IDAssignment ) else '' END As AssignmentName,
                           (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition)) as IDProductiveUnit,
                           (SELECT Name from ProductiveUnits where id in (select IDProductiveUnit FROM DailyBudgets where ID IN(select IDDailyBudget FROM dbo.DailyBudget_Positions WHERE dbo.DailyBudget_Positions.ID = dbo.DailySchedule.IDDailyBudgetPosition))) as ProductiveUnitName,
 						  dbo.sysroRemarks.Text as Remark,
						  (CASE WHEN isnull(dbo.DailySchedule.IsHolidays, 0) = 1 THEN isnull(dbo.DailySchedule.ExpectedWorkingHours, (select isnull(ExpectedWorkingHours,0) from Shifts where id = dbo.DailySchedule.IDShiftBase)) ELSE isnull(dbo.DailySchedule.ExpectedWorkingHours, 
                           dbo.Shifts.ExpectedWorkingHours) END) AS ExpectedWorkingHoursAbs
  FROM            dbo.DailySchedule INNER JOIN
                           dbo.Employees ON dbo.DailySchedule.IDEmployee = dbo.Employees.ID INNER JOIN
                           dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee AND dbo.DailySchedule.Date >= dbo.EmployeeContracts.BeginDate AND 
                           dbo.DailySchedule.Date <= dbo.EmployeeContracts.EndDate INNER JOIN
                           dbo.sysroEmployeeGroups ON dbo.Employees.ID = dbo.sysroEmployeeGroups.IDEmployee AND dbo.DailySchedule.Date >= dbo.sysroEmployeeGroups.BeginDate AND 
                           dbo.DailySchedule.Date <= dbo.sysroEmployeeGroups.EndDate LEFT OUTER JOIN
                           dbo.Shifts ON dbo.Shifts.ID =
                               (SELECT        CASE WHEN Date <= GETDATE() THEN IDShiftUsed ELSE IDShift1 END AS Expr1
                                 FROM            dbo.DailySchedule AS DS
                                 WHERE        (Date = dbo.DailySchedule.Date) AND (IDEmployee = dbo.DailySchedule.IDEmployee))
 						  LEFT OUTER JOIN dbo.sysroRemarks ON dbo.sysroRemarks.ID = dbo.DailySchedule.remarks
GO

-- añadimos campo horas teoricas absolutas
 ALTER PROCEDURE  [dbo].[Analytics_Schedule]
     	@initialDate smalldatetime,
     	@endDate smalldatetime,
     	@idpassport int,
     	@employeeFilter nvarchar(max),
     	@userFieldsFilter nvarchar(max)
      AS
      DECLARE @employeeIDs Table(idEmployee int)
  	 DECLARE @pinitialDate smalldatetime = @initialDate,  
       @pendDate smalldatetime = @endDate,  
       @pidpassport int = @idpassport,  
       @pemployeeFilter nvarchar(max) = @employeeFilter,  
       @puserFieldsFilter nvarchar(max) = @userFieldsFilter  
      insert into @employeeIDs exec dbo.ObtainEmployeeIDsFromFilter @pemployeeFilter,@pinitialDate,@pendDate;
      SELECT     CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
  		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10) AS KeyView, 
  		dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.IDGroup, dbo.EmployeeContracts.IDContract, dbo.sysroEmployeeGroups.GroupName, 
  		dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name AS EmployeeName, dbo.sysroEmployeesShifts.ShiftName, 
  		dbo.sysroEmployeesShifts.CurrentDate AS Date, SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHours) AS Hours,SUM(dbo.sysroEmployeesShifts.ExpectedWorkingHoursAbs) as HoursAbs, COUNT(*) AS Count, 
  		MONTH(dbo.sysroEmployeesShifts.CurrentDate) AS Mes, YEAR(dbo.sysroEmployeesShifts.CurrentDate) AS Año, (DATEPART(dw, 
  		dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1 AS DayOfWeek, DATEPART(wk, dbo.sysroEmployeesShifts.CurrentDate) AS WeekOfYear, 
  		DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate) AS DayOfYear, dbo.sysroEmployeeGroups.Path, dbo.sysroEmployeeGroups.CurrentEmployee, 
  		dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
  		dbo.EmployeeContracts.BeginDate AS BeginContract, dbo.EmployeeContracts.endDate AS EndContract, @pinitialDate AS BeginPeriod, @pendDate AS EndPeriod,
  		dbo.sysroEmployeesShifts.IDAssignment,
  		CONVERT(NVARCHAR(4000),dbo.sysroEmployeesShifts.Remark) as Remark,
  		dbo.sysroEmployeesShifts.AssignmentName,
  		dbo.sysroEmployeesShifts.IDProductiveUnit,
  		dbo.sysroEmployeesShifts.ProductiveUnitName, 
		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,1,'\') As Nivel1,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,2,'\') As Nivel2,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,3,'\') As Nivel3,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,4,'\') As Nivel4,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,5,'\') As Nivel5,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,6,'\') As Nivel6,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,7,'\') As Nivel7,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,8,'\') As Nivel8,
  		dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,9,'\') As Nivel9,dbo.UFN_SEPARATES_COLUMNS(dbo.sysroEmployeeGroups.FullGroupName,10,'\') As Nivel10,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,1,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField1,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,2,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField2,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,3,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField3,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,4,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField4,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,5,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField5,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,6,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField6,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,7,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField7,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,8,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField8,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,9,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField9,
  		dbo.GetEmployeeUserFieldValueMin(sysroEmployeesShifts.idEmployee,dbo.UFN_SEPARATES_COLUMNS(@puserFieldsFilter,10,','),dbo.sysroEmployeesShifts.CurrentDate) As UserField10
      FROM         dbo.sysroEmployeesShifts with (nolock) 
  		INNER JOIN dbo.sysroEmployeeGroups with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.sysroEmployeeGroups.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.sysroEmployeeGroups.endDate 
  		INNER JOIN dbo.EmployeeContracts with (nolock) ON dbo.sysroEmployeesShifts.IDEmployee = dbo.EmployeeContracts.IDEmployee AND dbo.sysroEmployeesShifts.CurrentDate >= dbo.EmployeeContracts.BeginDate AND dbo.sysroEmployeesShifts.CurrentDate <= dbo.EmployeeContracts.endDate 
  		LEFT OUTER JOIN dbo.Employees with (nolock) ON dbo.sysroEmployeeGroups.IDEmployee = dbo.Employees.ID
  	WHERE  dbo.WebLogin_GetPermissionOverEmployee(@pidpassport,dbo.sysroEmployeesShifts.IDEmployee,2,0,0,dbo.sysroEmployeesShifts.CurrentDate) > 1
  		AND dbo.sysroEmployeesShifts.IDEmployee in( select idEmployee from @employeeIDs) and dbo.sysroEmployeesShifts.CurrentDate between @pinitialDate and @pendDate
  	GROUP BY dbo.sysroEmployeesShifts.IDEmployee, dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.FullGroupName, dbo.Employees.Name, 
  		dbo.sysroEmployeesShifts.ShiftName, dbo.sysroEmployeesShifts.CurrentDate, MONTH(dbo.sysroEmployeesShifts.CurrentDate), 
  		YEAR(dbo.sysroEmployeesShifts.CurrentDate), (DATEPART(dw, dbo.sysroEmployeesShifts.CurrentDate) + @@DATEFIRST - 1 - 1) % 7 + 1, DATEPART(wk, 
  		dbo.sysroEmployeesShifts.CurrentDate), DATEPART(dy, dbo.sysroEmployeesShifts.CurrentDate), dbo.sysroEmployeeGroups.IDGroup, 
  		dbo.EmployeeContracts.IDContract, CAST(dbo.sysroEmployeesShifts.IDEmployee AS varchar) + '-' + CAST(dbo.sysroEmployeeGroups.IDGroup AS varchar) 
  		+ '-' + dbo.EmployeeContracts.IDContract + '-' + LEFT(CONVERT(VARCHAR, dbo.sysroEmployeesShifts.CurrentDate, 120), 10), dbo.sysroEmployeeGroups.Path, 
  		dbo.sysroEmployeeGroups.CurrentEmployee, dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.endDate,
  		dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.endDate,dbo.sysroEmployeesShifts.IDAssignment,dbo.sysroEmployeesShifts.AssignmentName,dbo.sysroEmployeesShifts.IDProductiveUnit,dbo.sysroEmployeesShifts.ProductiveUnitName, CONVERT(NVARCHAR(4000),dbo.sysroEmployeesShifts.Remark)
GO

-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='459' WHERE ID='DBVersion'
GO
