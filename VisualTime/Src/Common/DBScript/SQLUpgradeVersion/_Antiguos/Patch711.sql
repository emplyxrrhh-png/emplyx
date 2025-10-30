-- No borréis esta línea
UPDATE sysroGUI
	SET IDPath = 'Portal\Communications\Communiques',
		[Priority] = 1501 
WHERE IDPath = 'Portal\Communiques'

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities],[Edition])
     VALUES
('Portal\Communications',
'Communications',
NULL,
'Communications.png',
NULL,
NULL,
NULL,
NULL,
1500,
'NWR',
NULL,
NULL)
GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities],[Edition])
     VALUES
('Portal\Communications\Channels',
'Channels',
'/Channels',
'Channels.png',
NULL,
NULL,
'Feature\Channels',
NULL,
1502,
'NWR',
'U:Employees.Channels=Read OR U:Employees.Complaints=Read',
NULL)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='711' WHERE ID='DBVersion'
GO
