-- No borréis esta línea
insert into sysroRuleType (id, Type) values (11,'MinimumConsecutiveDays')
GO

insert into sysroRequestRuleTypes (IDRequestType, IDRuleType) values (6,11)
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1042' WHERE ID='DBVersion'
GO
