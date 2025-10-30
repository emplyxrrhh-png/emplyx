CREATE NONCLUSTERED INDEX IDX_RequestTypeStatus
ON [dbo].[Requests] ([RequestType],[Status])
INCLUDE ([IDEmployee],[Date1],[Date2],[IDCause],[Hours],[FromTime],[ToTime])
GO


CREATE INDEX [IX_Sessions_DataId] ON [dbo].[sysroPassports_Sessions] ([DataId])
GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='692' WHERE ID='DBVersion'
GO
