update sysroFeatures set Name ='Fichajes olvidados de centro de coste' where Alias='Calendar.Punches.Requests.CostCenterForgotten'
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroRequestRuleTypes] WHERE IDRequestType = 6 and IDRuleType=2)
	INSERT INTO sysroRequestRuleTypes (IDRequestType, IDRuleType) values (6,2)
GO


-- Actualiza versión de la base de datos
UPDATE sysroParameters SET Data='458' WHERE ID='DBVersion'
GO
