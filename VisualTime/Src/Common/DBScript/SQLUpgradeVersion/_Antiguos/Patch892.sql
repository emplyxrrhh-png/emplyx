CREATE PROCEDURE [dbo].[Report_cards_on_use]
	@idPassport nvarchar(100) = '1'
AS
SELECT "EmployeeContracts"."BeginDate", "EmployeeContracts"."EndDate", "Employees"."Name", "sysroPassports_AuthenticationMethods"."Credential", "sysroPassports_AuthenticationMethods"."Method", "sysroPassports_AuthenticationMethods"."Version", "sysroPassports_AuthenticationMethods"."BiometricID", "EmployeeContracts"."IDEmployee", "sysroPassports_AuthenticationMethods"."Enabled", "Employees"."ID"
 FROM   ("sysroPassports_AuthenticationMethods" "sysroPassports_AuthenticationMethods" 
 INNER JOIN "sysroPassports" "sysroPassports" ON "sysroPassports_AuthenticationMethods"."IDPassport"="sysroPassports"."ID") 
 INNER JOIN ("EmployeeContracts" "EmployeeContracts" 
 INNER JOIN "Employees" "Employees" ON "EmployeeContracts"."IDEmployee"="Employees"."ID") ON "sysroPassports"."IDEmployee"="Employees"."ID"
 inner join sysrovwSecurity_PermissionOverEmployeeAndFeature poe on poe.IdEmployee = Employees.ID and GETDATE() between poe.BeginDate and poe.EndDate and poe.IdFeature = 1400 and poe.FeaturePermission > 1 and poe.IdPassport = @idPassport
 WHERE  "EmployeeContracts"."EndDate">=CONVERT(DATETIME, CAST(GETDATE() AS DATE), 120) 
 AND "EmployeeContracts"."BeginDate"<=CONVERT(DATETIME, CAST(GETdATE() AS DATE), 120) 
 AND "sysroPassports_AuthenticationMethods"."Credential"<>N'' 
 AND "sysroPassports_AuthenticationMethods"."Method"=3 AND "sysroPassports_AuthenticationMethods"."Version"=N'' 
 AND "sysroPassports_AuthenticationMethods"."BiometricID"=0 AND "sysroPassports_AuthenticationMethods"."Enabled"=1
RETURN NULL 
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='892' WHERE ID='DBVersion'
GO
