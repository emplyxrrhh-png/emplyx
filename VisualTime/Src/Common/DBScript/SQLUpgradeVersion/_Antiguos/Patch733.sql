INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities],[Edition])
     VALUES
('Portal\Security\LogBook',
'LogBook',
'/LogBook',
'LogBook.png',
NULL,
NULL,
'Feature\Complaints',
NULL,
2205,
'NWR',
'U:Employees.Complaints=Admin',
NULL)
GO


-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='733' WHERE ID='DBVersion'
GO
