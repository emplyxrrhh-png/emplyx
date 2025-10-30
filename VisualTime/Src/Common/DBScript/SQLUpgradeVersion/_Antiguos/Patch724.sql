
INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities],[Edition])
     VALUES
('Portal\Bots',
'Bots',
'/Bots',
'Bots.png',
NULL,
NULL,
'Feature\Bots',
NULL,
1505,
NULL,
'U:Bots.Definition=Read',
NULL)
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='724' WHERE ID='DBVersion'
GO
