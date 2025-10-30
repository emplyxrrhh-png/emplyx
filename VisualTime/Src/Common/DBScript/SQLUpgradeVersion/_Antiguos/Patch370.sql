INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value]) VALUES('RoboticsFeedUrl','http://www.robotics.es/feed')
GO
INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value]) VALUES('RoboticsSupportUrl','http://www.robotics.es/RoboticsQS.exe')
GO
INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value]) VALUES('RoboticsUrl','http://www.robotics.es/software-control-de-presencia/')
GO

update [dbo].sysroReaderTemplates set ValidationMode = 'ServerLocal,Local,LocalServer' where Type = 'mxART'
GO

UPDATE dbo.sysroParameters SET Data='370' WHERE ID='DBVersion'
GO