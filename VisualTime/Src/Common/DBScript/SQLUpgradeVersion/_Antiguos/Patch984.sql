-- No borréis esta línea
IF EXISTS (SELECT * FROM sysroParameters WHERE ID ='CCode' AND CONVERT(VARCHAR,Data) = 'vsl3177')
BEGIN
    INSERT INTO [dbo].[sysroGUI]([IDPath],[LanguageReference],[URL],[IconURL],[Type],[Parameters],[RequiredFeatures],[SecurityFlags],[Priority],[AllowedSecurity],[RequiredFunctionalities],[Edition])
     VALUES
            ('Portal\Task\PartesTrabajo',
            'PartesTrabajo',
            'Tasks/PartesTrabajo.aspx',
            'PartesTrabajo.png',
            NULL,
            NULL,
            'Feature\Productiv',
            NULL,
            1804,
            NULL,
            'U:Tasks.Definition=Read',
            NULL)

END 
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='984' WHERE ID='DBVersion'
GO
