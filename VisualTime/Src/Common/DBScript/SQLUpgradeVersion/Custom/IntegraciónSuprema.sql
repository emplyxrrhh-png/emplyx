alter table dbo.terminals alter column punchstamp nvarchar(50)
GO

insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.CheckPeriod','00:20@20')
GO

insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.EmployeeField','LIBRO MATRICULA')
GO

insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.Password','Esprinet2021!')
GO

insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.URL','https://195.55.194.115/')
GO

insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.UserName','admin')
GO

insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.InitialLinkDate','2021-01-01T00:00:00.000Z')
GO
