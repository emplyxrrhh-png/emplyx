UPDATE [dbo].[EmployeeCenters]
   SET [BeginDate] = '20000101'
 WHERE BeginDate IS NULL
GO

ALTER TABLE [dbo].[EmployeeCenters]
ALTER COLUMN [BeginDate] SMALLDATETIME NOT NULL

GO

ALTER TABLE [dbo].[EmployeeCenters]
DROP CONSTRAINT [PK_EmployeeCenters]

GO

ALTER TABLE [dbo].[EmployeeCenters]
ADD CONSTRAINT [PK_EmployeeCenters] PRIMARY KEY([IDEmployee], [IDCenter], [BeginDate])

GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='538' WHERE ID='DBVersion'
GO