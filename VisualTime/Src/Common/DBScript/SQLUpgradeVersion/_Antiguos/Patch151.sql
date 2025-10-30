--
-- Actualiza Employees
-- 
ALTER TABLE [dbo].[Employees] ADD [AliasType] [bit] NULL , [Alias] [nvarchar] (50) NULL 
--
-- Crea Messages
--
CREATE TABLE [dbo].[Messages]
    (
  [ID] [numeric] (16, 0) NOT NULL IDENTITY (1, 1) ,
  [IDEmployee] [int] NOT NULL ,
  [Text] [nvarchar] (50) NULL ,
  [RepeatEveryDays] [smallint] NOT NULL ,
  [TimetoShow] [smalldatetime] NULL ,
  [LastTimeShow] [smalldatetime] NULL ,
  [Terminate] [smallint] NULL ,
  [CreatedDate] [smalldatetime] NULL 
)
GO
ALTER TABLE [dbo].[Messages] ADD CONSTRAINT [DF_Messages_IDEmployee] DEFAULT (0) FOR [IDEmployee]
GO
ALTER TABLE [dbo].[Messages] ADD CONSTRAINT [DF_Messages_RepeatEveryDays] DEFAULT (0) FOR [RepeatEveryDays]
GO
ALTER TABLE [dbo].[Messages] ADD CONSTRAINT [FK_Messages_Employees] FOREIGN KEY (IDEmployee)  REFERENCES [dbo].[Employees] (ID) 
GO
ALTER TABLE [dbo].[Messages] ADD CONSTRAINT [DF_Messages_Terminate] DEFAULT (0) FOR [Terminate]
GO
ALTER TABLE [dbo].[Messages] WITH NOCHECK ADD CONSTRAINT [PK_Messages] PRIMARY KEY NONCLUSTERED (ID) 
GO
CREATE  INDEX [IX_Messages_IDEmployee] ON [dbo].[Messages]([IDEmployee]) ON [PRIMARY]
GO
--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='151' WHERE ID='DBVersion'
GO

