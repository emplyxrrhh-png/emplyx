IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VisualTime.Link.Suprema.CheckPeriod')
	insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.CheckPeriod','00:20@120')
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VisualTime.Link.Suprema.ExtraEventCodes')
	insert into dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.ExtraEventCodes','')
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='916' WHERE ID='DBVersion'
GO
