update dbo.sysroNotificationTypes Set Scheduler = 305 where ID = 19
GO

update dbo.Notifications Set AllowMail = 1 where IDType IN(83,84)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='619' WHERE ID='DBVersion'
GO
