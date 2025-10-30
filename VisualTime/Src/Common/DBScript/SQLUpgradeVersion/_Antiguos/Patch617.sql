delete from sysrogui where parameters like '%CalendarV1%'
go 
update sysroLiveAdvancedParameters set value='2' where ParameterName = 'CalendarMode'
go

ALTER Table sysroNotificationTypes add NextExecution datetime null
GO

ALTER Table sysroNotificationTypes add IsRunning bit not null default 0
GO

delete sysroGUI where IDPath like '%Analytics%'
GO
delete sysroGUI where IDPath like '%Portal\Configuration\Documents%'
GO
delete sysroGUI where URL like '%ReportScheduler%'
GO

UPDATE sysroNotificationTypes SET NextExecution = DATEADD(minute,Scheduler,getdate()) WHERE NextExecution is null
GO

delete from Notifications where IDType = 10 
GO
delete from sysroNotificationTypes where ID = 10 
GO
delete from Notifications where IDType = 12 
GO
delete from sysroNotificationTypes where ID = 12 
GO
delete from Notifications where IDType = 32 
GO
delete from sysroNotificationTypes where ID = 32 
GO
delete from Notifications where IDType = 34 
GO
delete from sysroNotificationTypes where ID = 34 
GO

UPDATE sysroNotificationTypes SET Scheduler = -30 where id = 54
GO

UPDATE sysroNotificationTypes SET Scheduler = -1 where id in(13,14,19,31,32,33,34,35,36,37,38,39,40,41,42,43)
GO
UPDATE sysroNotificationTypes SET Scheduler = -1 where id in(45,46,47,48,49,50,51,52,53,55,57,58,59,60,61,62,63,64)
GO
UPDATE sysroNotificationTypes SET Scheduler = -1 where id in(66,67,68,69,71,85,86)
GO

UPDATE sysroNotificationTypes SET Scheduler = 600 where id = 1
GO
UPDATE sysroNotificationTypes SET Scheduler = 610 where id = 2
GO
UPDATE sysroNotificationTypes SET Scheduler = 210 where id = 3
GO
UPDATE sysroNotificationTypes SET Scheduler = 220 where id = 4
GO
UPDATE sysroNotificationTypes SET Scheduler = 230 where id = 5
GO
UPDATE sysroNotificationTypes SET Scheduler = 10 where id = 8
GO
UPDATE sysroNotificationTypes SET Scheduler = 7 where id = 9
GO
UPDATE sysroNotificationTypes SET Scheduler = 2 where id = 11
GO
UPDATE sysroNotificationTypes SET Scheduler = 510 where id = 16
GO
UPDATE sysroNotificationTypes SET Scheduler = 500 where id = 17
GO
UPDATE sysroNotificationTypes SET Scheduler = 300 where id = 20
GO
UPDATE sysroNotificationTypes SET Scheduler = 310 where id = 21
GO
UPDATE sysroNotificationTypes SET Scheduler = 5 where id = 22
GO
UPDATE sysroNotificationTypes SET Scheduler = 320 where id = 23
GO
UPDATE sysroNotificationTypes SET Scheduler = 330 where id = 24
GO
UPDATE sysroNotificationTypes SET Scheduler = 340 where id = 25
GO
UPDATE sysroNotificationTypes SET Scheduler = 350 where id = 26
GO
UPDATE sysroNotificationTypes SET Scheduler = 115 where id = 27
GO
UPDATE sysroNotificationTypes SET Scheduler = 125 where id = 28
GO
UPDATE sysroNotificationTypes SET Scheduler = 400 where id = 29
GO
UPDATE sysroNotificationTypes SET Scheduler = 390 where id = 44
GO
UPDATE sysroNotificationTypes SET Scheduler = 380 where id = 65
GO
UPDATE sysroNotificationTypes SET Scheduler = 360 where id = 77
GO
UPDATE sysroNotificationTypes SET Scheduler = 370 where id = 78
GO
UPDATE sysroNotificationTypes SET Scheduler = 1440 where id = 82
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='617' WHERE ID='DBVersion'
GO
