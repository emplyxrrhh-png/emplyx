IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TMPEmergencyVisitsZones]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TMPEmergencyVisitsZones](
	[VisitPlanId] [int] NULL,
	[BeginTime] [datetime] NULL,
	[VisitorId] [nvarchar](40) NULL,
	[EmpVisitedId] [int] NULL,
	[MeettingPoint] [nvarchar](100) NULL,
	[IDReportTask] [numeric](16, 0) NULL DEFAULT ((0))
) ON [PRIMARY]
END


IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[sysrovwCurrentVisistsStatus]'))
DROP VIEW [dbo].[sysrovwCurrentVisistsStatus]
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[sysrovwCurrentVisistsStatus]'))
EXEC dbo.sp_executesql @statement = N' CREATE view [dbo].[sysrovwCurrentVisistsStatus]
   as
   select v.IDVisitor, v.Name VisitorName, e.ID,e.Name EmployeeName, euf.Value
   from [dbo].[Visit_Visitor] vv
   join [dbo].[Visitor] v on vv.IDVisitor = v.IDVisitor
   join [dbo].[Visit] vi on vv.IDVisit = vi.IDVisit
   join [dbo].[Employees] e on vi.IDEmployee = e.ID
   join [dbo].[EmployeeUserFieldValues] euf on e.ID = euf.IDEmployee
   where FieldName like (SELECT FieldName FROM [dbo].[sysroUserFields]
                         WHERE [Description] like ''Info.EmergencyPointInfo'') 
   and vi.Status = 1


' 
GO

UPDATE dbo.sysroParameters SET Data='383' WHERE ID='DBVersion'
GO
