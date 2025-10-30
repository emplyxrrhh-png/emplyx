IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDirectSupervisorByNotification]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
 DROP FUNCTION [dbo].[GetDirectSupervisorByNotification]
GO

UPDATE [dbo].[TerminalsSyncBiometricData] SET Version = 'RXFFNG' WHERE Version IS NULL
GO

update dbo.sysroGUI set [Parameters] = '' where IDPath = 'Portal\GeneralManagement\AdvReport'
GO

ALTER TABLE CommuniqueEmployeeStatus ALTER COLUMN Response NVARCHAR (50);
GO

alter VIEW [dbo].[sysrovwHoursAbsences]  
     AS  
   SELECT IDCause, IDEmployee, Date1 as [Date], Date2 as FinishDate, FromTime as BeginTime, ToTime as EndTime, Hours as Duration, ID as AbsenceId, 1 as IsRequest  
   FROM Requests   
   WHERE RequestType = 9 AND (Status = 0 OR Status = 1)  
     UNION  
   SELECT IdCause, IDEmployee, Date, FinishDate, BeginTime, EndTime, Duration, AbsenceID, 0 as IsRequest  
   FROM ProgrammedCauses 
GO
UPDATE sysroParameters SET Data='501' WHERE ID='DBVersion'
GO

