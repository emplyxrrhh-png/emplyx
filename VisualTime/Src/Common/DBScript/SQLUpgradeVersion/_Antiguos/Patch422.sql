
ALTER TABLE dbo.sysroGroupFeatures ADD
	BusinessGroupList nvarchar(MAX) not NULL default ''
GO

CREATE TABLE [dbo].[sysroSecurityGroupFeature_Centers](
	[IDGroupFeature] [int] NOT NULL,
	[IDCenter] [smallint] NOT NULL,
 CONSTRAINT [PK_[sysroSecurityGroupFeature_Centers] PRIMARY KEY CLUSTERED 
(
	[IDGroupFeature] ASC,
	[IDCenter] ASC
) ON [PRIMARY]
) ON [PRIMARY]

GO

UPDATE dbo.sysroParameters SET Data='422' WHERE ID='DBVersion'
GO
