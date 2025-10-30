CREATE OR ALTER PROCEDURE [dbo].[Report_lista_de_usuarios]
@idPassport nvarchar(100) = '1',
@IDEmployee nvarchar(max) = '114,242',
@userField1 nvarchar(100) = '0',
@userField2 nvarchar(100) = '0',
@userField3 nvarchar(100) = '0',
@userField4 nvarchar(100) = '0'
AS
SELECT e.ID,
(select top 1 Value from EmployeeUserFieldValues where IDEmployee = e.ID AND FieldName = (Select FieldName from sysroUserFields where ID = @userField1) and EmployeeUserFieldValues.date <= cast(getDate() as date) order by EmployeeUserFieldValues.date desc) As userField1,
(select top 1 Value from EmployeeUserFieldValues where IDEmployee = e.ID AND FieldName = (Select FieldName from sysroUserFields where ID = @userField2) and EmployeeUserFieldValues.date <= cast(getDate() as date) order by EmployeeUserFieldValues.date desc) As userField2,
(select top 1 Value from EmployeeUserFieldValues where IDEmployee = e.ID AND FieldName = (Select FieldName from sysroUserFields where ID = @userField3) and EmployeeUserFieldValues.date <= cast(getDate() as date) order by EmployeeUserFieldValues.date desc) As userField3,
(select top 1 Value from EmployeeUserFieldValues where IDEmployee = e.ID AND FieldName = (Select FieldName from sysroUserFields where ID = @userField4) and EmployeeUserFieldValues.date <= cast(getDate() as date) order by EmployeeUserFieldValues.date desc) As userField4,
eg.EmployeeName, eg.FullGroupName
FROM employees e
inner join (select * from split(@IDEmployee,',')) tmp on e.ID = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe On poe.IDPassport = @idPassport
and (GETDATE() between poe.BeginDate and poe.EndDate
or GETDATE() between poe.BeginDate and poe.EndDate)
and poe.IdFeature = 1 and poe.FeaturePermission > 1
inner join sysrovwAllEmployeeGroups eg on e.ID = eg.IDEmployee
GROUP BY e.ID, eg.EmployeeName, eg.FullGroupName
OPTION (USE HINT('ASSUME_JOIN_PREDICATE_DEPENDS_ON_FILTERS'))
RETURN NULL
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='894' WHERE ID='DBVersion'
GO
