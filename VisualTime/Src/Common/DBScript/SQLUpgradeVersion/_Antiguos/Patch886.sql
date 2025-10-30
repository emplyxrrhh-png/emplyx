CREATE OR ALTER PROCEDURE [dbo].[Report_documentos_pendientes_entregar]
@idPassport nvarchar(100) = '1',
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301',
@IDEmployee nvarchar(max) = '114,242',
@IDUserField nvarchar(100) = '0'
AS
select fdf.*, dt.name as DocumentTemplate, c.name as Cause, d.CurrentlyValid, cast(fdf.duedate as date) as DueDateP, concat(CAST(fdf.begindate AS date), ' ', CAST(fdf.starthour AS TIME)) as startDateTime, concat(CAST(fdf.enddate AS date), ' ', CAST(fdf.endhour AS TIME)) as endDateTime, eg.EmployeeName, eg.FullGroupName, EmployeeUserFieldValues.Value as campoFicha from sysrovwForecastDocumentsFaults fdf
inner join (select * from split(@IDEmployee,',')) tmp on fdf.idemp = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
And poe.IDEmployee = fdf.idemp
and (CAST(fdf.begindate as date) between poe.BeginDate and poe.EndDate
or CAST(fdf.enddate as date) between poe.BeginDate and poe.EndDate)
and poe.IdFeature = 1 and poe.FeaturePermission > 1
inner join sysrovwAllEmployeeGroups eg on fdf.idemp = eg.IDEmployee
inner join DocumentTemplates dt ON dt.id = fdf.templateid 
inner join causes c ON c.id = fdf.idcause 
left join documents d on d.IdEmployee = fdf.idemp and (d.IdDaysAbsence = fdf.forecastId or d.IdHoursAbsence = fdf.forecastId) 
left join EmployeeUserFieldValues with (nolock) on EmployeeUserFieldValues.IDEmployee = fdf.idemp and EmployeeUserFieldValues.FieldName = (select FieldName from sysroUserFields Where ID = @IDUserField) and (EmployeeUserFieldValues.date >= getDate() or EmployeeUserFieldValues.date = '1900-01-01 00:00:00')
where (CAST(fdf.begindate as date) between CAST(@startDate as date) and CAST(@endDate as date) or CAST(fdf.enddate as date) between CAST(@startDate as date) and CAST(@endDate as date))
RETURN NULL
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='886' WHERE ID='DBVersion'
GO
