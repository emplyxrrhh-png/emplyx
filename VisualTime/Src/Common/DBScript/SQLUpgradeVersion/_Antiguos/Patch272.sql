INSERT INTO sysroReaderTemplates VALUES ('saiwall',1,2,'ACC',1,'Blind','X','Local',0,0,0,1,0,'Remote')
GO

/****** Eliminamos incides ******/
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[sysroPassports]') AND name = N'IX_sysroPassports_IDUser')
DROP INDEX [IX_sysroPassports_IDUser] ON [dbo].[sysroPassports] WITH ( ONLINE = OFF )
GO

/****** Eliminamos claves ******/
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_sysroPassports_sysroUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[sysroPassports]'))
ALTER TABLE [dbo].[sysroPassports] DROP CONSTRAINT [FK_sysroPassports_sysroUsers]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[sysroUsers]') AND name = N'IX_sysroUsers_IDUser')
DROP INDEX [IX_sysroUsers_IDUser] ON [dbo].[sysroUsers] WITH ( ONLINE = OFF )
GO


/****** Cambiamos tipo de datos ******/
alter table dbo.sysroPassports alter column IDUser int
GO

alter table dbo.sysroUsers alter column IDUser int
GO

/****** Creamos incides ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_sysroUsers_IDUser] ON [dbo].[sysroUsers] 
([IDUser] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_sysroPassports_IDUser] ON [dbo].[sysroPassports] 
([IDUser] ASC) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

/****** Creamos claves ******/
ALTER TABLE [dbo].[sysroPassports]  WITH CHECK ADD  CONSTRAINT [FK_sysroPassports_sysroUsers] FOREIGN KEY([IDUser])
REFERENCES [dbo].[sysroUsers] ([IDUser])
GO

ALTER TABLE [dbo].[sysroPassports] CHECK CONSTRAINT [FK_sysroPassports_sysroUsers]
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='272' WHERE ID='DBVersion'
GO
