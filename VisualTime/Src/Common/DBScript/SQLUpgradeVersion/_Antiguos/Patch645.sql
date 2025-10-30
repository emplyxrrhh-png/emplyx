ALTER TABLE [dbo].[sysroPassports_AuthenticationMethods] ADD LastAppActionDate SMALLDATETIME NULL
GO
ALTER TABLE [dbo].[sysroPassports_AuthenticationMethods] ADD BlockedAccessByInactivity BIT NULL 
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='645' WHERE ID='DBVersion'
GO
