UPDATE [dbo].[TerminalReaders] set TYPE ='RX' WHERE TYPE = 'VR' AND IDTERMINAL in (SELECT ID FROM [dbo].[TERMINALS] 
	WHERE [dbo].[TerminalReaders].IDTerminal =[dbo].[Terminals].id AND [dbo].[Terminals].TYPE in ('rxC','rxCe','rxF'))
GO

UPDATE sysroParameters SET Data='348' WHERE ID='DBVersion'
GO


