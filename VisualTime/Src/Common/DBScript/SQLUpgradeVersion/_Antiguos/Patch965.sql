UPDATE sysroLiveAdvancedParameters
	SET Value = '{"causes":{"refresh":1,"interval":1440,"off":""},"zones":{"refresh":1,"interval":1440,"off":""},"status":{"refresh":1,"interval":1,"off":""},"punches":{"refresh":1,"interval":1,"off":""},"accruals":{"refresh":1,"interval":-1,"off":"07:50-08:10@13:50-14:10@16:50-19:10"},"notifications":{"refresh":1,"interval":30,"off":""},"dailyrecord":{"refresh":1,"interval":-1,"off":""},"communiques":{"refresh":1,"interval":60,"off":""},"channels":{"refresh":1,"interval":60,"off":""},"telecommute":{"refresh":1,"interval":-1,"off":""},"tcinfo":{"refresh":1,"interval":-1,"off":""},"surveys":{"refresh":1,"interval":60,"off":""},"weblinks":{"refresh":1,"interval":-1,"off":""}}'
WHERE ParameterName = 'VTPortal.RefreshConfiguration'
GO

EXEC sp_rename 'Weblinks.Order', 'Position', 'COLUMN';
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='965' WHERE ID='DBVersion'
GO
