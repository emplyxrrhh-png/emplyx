INSERT INTO [dbo].[sysroGUI]
           ([IDPath]
           ,[LanguageReference]
           ,[URL]
           ,[IconURL]
           ,[Type]
           ,[Parameters]
           ,[RequiredFeatures]
           ,[SecurityFlags]
           ,[Priority]
           ,[AllowedSecurity]
           ,[RequiredFunctionalities]
           ,[Edition])
     VALUES
           ('Portal\ShiftManagement\Shiftsv2',	'GUI.Shifts',	'Shifts/Shiftsv2.aspx',	'Shifts.png',	NULL,	'June6610',	'Forms\Shifts',	NULL,	2308,	'NWR',	'U:Shifts.Definition=Read',	NULL)

           GO

ALTER TABLE SHIFTS ADD  VisibilityCollectives NVARCHAR(MAX) NULL

GO

UPDATE SYSROGUI SET PARAMETERS = 'June6610', RequiredFeatures = NULL WHERE IDPath = 'Portal\Company\Collectives'

GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1021' WHERE ID='DBVersion'
GO
