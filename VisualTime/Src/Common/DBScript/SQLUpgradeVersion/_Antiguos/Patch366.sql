
 CREATE FUNCTION [dbo].[GetNumberVisitsByUserField]
  (	
    	@Date smalldatetime,
		@UserField nvarchar(50),
		@UserFieldValue nvarchar(100),
		@maxTime int
  )
  RETURNS int
  AS
  BEGIN
    	DECLARE @Result int
		
		DECLARE @tmpVisitsTable table(VisitDate smalldatetime,Visitor nvarchar(50),VisitStatus nvarchar(10), MeetingPoint nvarchar(100), IDEmployee int, vWVisitorID int)

		insert into @tmpVisitsTable SELECT tmpVisits.* FROM
		(SELECT sysrovwVisitsInOutPunches.DateTime AS VisitDate, sysrovwVisitsInOutPunches.Visitor AS Visitor,
				sysrovwVisitsInOutPunches.Status As VisitStatus, GetAllEmployeeUserFieldValue_1.value AS MeetingPoint, 
				sysrovwVisitsInOutPunches.IDEmployee AS IDEmployee, Visitors.ID AS vWVisitorID
				FROM Employees INNER JOIN sysrovwVisitsInOutPunches ON Employees.ID = sysrovwVisitsInOutPunches.IDEmployee 
							INNER JOIN Visitors ON Visitors.ID = sysrovwVisitsInOutPunches.VisitorID 
							INNER JOIN dbo.GetAllEmployeeUserFieldValue(@UserField,@Date) AS GetAllEmployeeUserFieldValue_1 ON sysrovwVisitsInOutPunches.IDEmployee = GetAllEmployeeUserFieldValue_1.IDEmployee)tmpVisits
		WHERE (tmpVisits.VisitDate >= DATEADD(n, @maxTime, GETDATE())) 
				AND (tmpVisits.Visitor IS NOT NULL) 
				AND (tmpVisits.MeetingPoint = @UserFieldValue)
	
		SELECT @Result = count(*)
		FROM @tmpVisitsTable std WHERE VisitStatus = 'IN' 
		AND ( 
			(VisitDate >  (SELECT TOP (1) VisitDate FROM @tmpVisitsTable AS srviom 
									WHERE (std.IDEmployee = srviom.IDEmployee) AND 
											(std.vWVisitorID = srviom.vWVisitorID) AND (srviom.VisitStatus = 'OUT')  ORDER BY srviom.VisitDate DESC))
			OR
			(VisitStatus = (SELECT TOP (1) VisitStatus FROM @tmpVisitsTable AS srviom 
								WHERE (std.IDEmployee = srviom.IDEmployee) 
										And (std.vWVisitorID = srviom.vWVisitorID) ORDER BY srviom.VisitDate DESC))
		)

		return @Result
  END
GO


UPDATE dbo.sysroParameters SET Data='366' WHERE ID='DBVersion'
GO