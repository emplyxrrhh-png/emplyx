-- No borréis esta línea
CREATE NONCLUSTERED INDEX [IX_sysroPassports_StateIsSupervisor]
ON [dbo].[sysroPassports] ([State],[IsSupervisor])
INCLUDE ([StartDate],[ExpirationDate])
GO

ALTER VIEW [dbo].[sysrovwCommuniquesEmployees]  
AS  
SELECT IdCommunique, 
       IdEmployee, IdCreatedBy 
FROM [dbo].[CommuniqueEmployees] WITH (NOLOCK)  
LEFT JOIN [dbo].[Communiques]  WITH (NOLOCK) ON Communiques.Id = CommuniqueEmployees.IdCommunique  
UNION  
SELECT DISTINCT IdCommunique, ceg.IDEmployee, IdCreatedBy
FROM Communiques with (nolock) 
INNER JOIN [dbo].[CommuniqueGroups]  CG WITH (NOLOCK) ON Communiques.Id = CG.IdCommunique  
INNER JOIN [dbo].[Groups] WITH (NOLOCK) ON Groups.ID =  CG.IdGroup  
INNER JOIN [dbo].[sysrovwCurrentEmployeeGroups] CEG WITH (NOLOCK) ON (CEG.Path = Groups.Path OR CEG.Path LIKE Groups.Path + '\%') AND CEG.CurrentEmployee = 1  
INNER JOIN [dbo].[sysrovwSecurity_PermissionOverEmployees] POE WITH (NOLOCK) ON CEG.IDEmployee = POE.IdEmployee AND POE.IdPassport = communiques.IdCreatedBy AND convert(date, getdate()) BETWEEN POE.BeginDate AND POE.EndDate
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='833' WHERE ID='DBVersion'
GO
