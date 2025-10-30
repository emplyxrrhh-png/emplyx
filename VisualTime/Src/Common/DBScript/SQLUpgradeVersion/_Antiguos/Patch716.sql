INSERT INTO [dbo].[Channels]
           ([Title]
           ,[IdCreatedBy]
           ,[Status]
           ,[CreatedOn]
           ,[IdModifiedBy]
           ,[ModifiedOn]
           ,[PublishedOn]
           ,[ReceiptAcknowledgment]
           ,[AllowAnonymous]
           ,[IsComplaintChannel]
           ,[Deleted]
           ,[IdDeletedBy]
           ,[DeletedOn])
     SELECT
           '{Complaints}'
           ,ID 
           ,0
           ,GETDATE()
           ,NULL
           ,NULL
           ,NULL
           ,1
           ,1
           ,1
           ,0
           ,NULL
           ,NULL
	FROM [dbo].[sysroPassports]	WHERE Name = 'System'
GO


-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='716' WHERE ID='DBVersion'
GO
