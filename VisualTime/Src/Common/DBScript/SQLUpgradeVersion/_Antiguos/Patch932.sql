IF EXISTS (SELECT * FROM sysroParameters WHERE ID ='CCode' AND CONVERT(VARCHAR,Data) = 'gtisn3180')
BEGIN
	UPDATE TerminalReaders SET Type = 'SP' WHERE IDTerminal IN (SELECT ID FROM Terminals WHERE Type = 'Virtual')
	UPDATE Terminals SET Type = 'Suprema' WHERE Type = 'Virtual'
END 
GO

DELETE dbo.sysroLiveAdvancedParameters WHERE ParameterName = 'VisualTime.Link.Suprema.ExtraEventCodes'
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='932' WHERE ID='DBVersion'
GO
