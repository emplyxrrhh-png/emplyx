INSERT INTO [dbo].[sysroRuleType] (ID, TYPE) VALUES(10, 'ScheduleRuleToValidate')
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(5,10)
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(6,10)
GO

INSERT INTO [dbo].[sysroRequestRuleTypes] ([IDRequestType], [IDRuleType]) VALUES(8,10)
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1043' WHERE ID='DBVersion'
GO
