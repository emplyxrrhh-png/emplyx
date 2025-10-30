INSERT INTO [dbo].[Zones]
           ([ID]
           ,[Name]
           ,[Description]
           ,[IsWorkingZone]
           ,[X1]
           ,[X2]
           ,[Y1]
           ,[Y2]
           ,[Proportion]
           ,[IDZoneGroup]
           ,[IDParent]
           ,[Color]
           ,[ZoneImage]
           ,[IDCamera]
           ,[IDPlane]
           ,[TimeZone]
           ,[MapInfo]
           ,[Area]
           ,[Capacity]
           ,[CapacityVisible]
           ,[IsEmergencyZone]
           ,[ZoneNameAsLocation]
           ,[WorkCenter])
     VALUES
           (255
           ,'Sin especificar'
           ,'Zona sin especificar'
           ,1
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,1
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL
           ,NULL)

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='561' WHERE ID='DBVersion'
GO