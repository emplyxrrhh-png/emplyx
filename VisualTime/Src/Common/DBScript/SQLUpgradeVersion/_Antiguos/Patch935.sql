-- No borréis esta línea
IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroLiveAdvancedParameters] WHERE ParameterName = 'VisualTime.Link.Suprema.CheckPeriod')
	INSERT INTO dbo.sysroliveadvancedparameters(ParameterName,Value) Values ('VisualTime.Link.Suprema.CheckPeriod','00:20@5')
ELSE
	UPDATE [dbo].[sysroLiveAdvancedParameters] SET Value = '00:20@5' WHERE ParameterName = 'VisualTime.Link.Suprema.CheckPeriod'
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='935' WHERE ID='DBVersion'
GO
