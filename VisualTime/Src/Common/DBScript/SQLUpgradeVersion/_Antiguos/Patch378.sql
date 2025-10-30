UPDATE dbo.employeegroups SET BeginDate = CONVERT(DATE, BeginDate), EndDate = CONVERT(DATE,EndDate)
GO

update dbo.BusinessCenters set Name = REPLACE(Name, ' \','\')
GO
update dbo.BusinessCenters set Name = REPLACE(Name, '\ ','\')
GO
update dbo.BusinessCenters set Name = REPLACE(Name, ' \','\')
GO
update dbo.BusinessCenters set Name = REPLACE(Name, '\ ','\')
GO
update dbo.BusinessCenters set Name = REPLACE(Name, '\',' \ ')
GO


UPDATE dbo.sysroParameters SET Data='378' WHERE ID='DBVersion'
GO
