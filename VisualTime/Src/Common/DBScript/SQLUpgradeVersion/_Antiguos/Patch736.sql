IF ((SELECT COUNT(*) FROM sysroGUI WHERE IDPath = 'Portal\Security\LogBook') > 0 )
BEGIN
DELETE FROM sysroGUI WHERE IDPath = 'Portal\Security\LogBook'
END

GO

INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities],[Edition])
     VALUES
('Portal\Security\LogBook',
'LogBook',
'/LogBook',
'LogBook.png',
NULL,
NULL,
'Feature\Channels',
NULL,
2205,
'NWR',
'U:Employees.Complaints=Admin',
NULL)
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='736' WHERE ID='DBVersion'
GO
