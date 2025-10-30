-- Error en la lista de autorizaciones asignadas a los departamentos
ALTER VIEW [dbo].[sysrovwAccessAuthorizations]
AS
select IDEmployee, IDAuthorization, '0' as GroupPath from EmployeeAccessAuthorization
UNION 
SELECT IDEmployee, IDAuthorization, gr.Path as GroupPath  FROM GroupsAccessAuthorization ga
join Groups gr on ga.idgroup = gr.id
join sysrovwCurrentEmployeeGroups ce2 on (ce2.Path = gr.Path or ce2.Path like gr.Path + '\%' )
GO

UPDATE dbo.sysroGUI SET RequiredFeatures = 'Forms\Options;!Feature\ONE' WHERE IDPath = 'Portal\Configuration\EmergencyReport'
GO

UPDATE dbo.sysroGUI SET RequiredFeatures = 'Forms\Options;!Feature\ONE' WHERE IDPath = 'Portal\Configuration\Routes'
GO

UPDATE dbo.sysroGUI SET RequiredFeatures = '!Feature\ONE' WHERE IDPath = 'Portal\GeneralManagement\AccessZones'
GO

UPDATE dbo.sysroGUI SET RequiredFeatures = '!Feature\ONE' WHERE IDPath = 'Portal\CostControl\BusinessCenters'
GO

UPDATE dbo.sysroParameters SET Data='428' WHERE ID='DBVersion'
GO
