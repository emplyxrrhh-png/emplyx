IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroGUI_Actions] WHERE IDPath = 'MassMarkConsents')
	insert into sysroGUI_Actions values ('MassMarkConsents', 'Portal\General\Employees\Employees', 'tbMassMarkConsents', 'Forms\Employees', 'U:Employees=Admin', 'ShowEmployeeMassMarkConsents()', 'btnTbMassMarkConsents', 1, 5)
GO

-- TABLA DE CONSENTIMIENTOS
CREATE TABLE [dbo].[sysroPassports_Consents](
       [ID] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
       [IDPassport] [int] NOT NULL,
       [Type] [tinyint] NOT NULL,
       [Message] [nvarchar](max) NOT NULL,
       [ApprovalDate] [smalldatetime]  NOT NULL default(getdate()),
       [IsValid] [bit]  NOT NULL DEFAULT(1),
       [IDPassportAction] [int] NULL default(0),
CONSTRAINT [PK_sysroPassports_Consents] PRIMARY KEY CLUSTERED 
(
       [ID] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE dbo.Groups ALTER COLUMN Name nvarchar(500)
GO

IF EXISTS (SELECT 1 FROM [dbo].[sysroFeatures] WHERE ID =23101)
BEGIN

	DELETE FROM [dbo].[sysroPassports_PermissionsOverFeatures] WHERE IDFeature = 23101
	DELETE FROM [dbo].[sysroGroupFeatures_PermissionsOverFeatures] where  IDFeature = 23101
	DELETE FROM [dbo].[sysroFeatures] where  ID = 23101
END
GO

UPDATE dbo.sysroParameters SET Data='439' WHERE ID='DBVersion'
GO

