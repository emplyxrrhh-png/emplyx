--Deshabilitar PRL en terminales rx. Hay un BUG en pantalla que hace que no se puedan guardar estos terminales, y el comportamiento PRL no se usa
update sysroReaderTemplates set OHP = 0 where type in ('rxC','mxC','rxCe','rxF','rxCP') and OHP = 1
GO

--Comportamientos disponibles rxCePush
INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCeP',1,102,'TA',1,'Blind','E,S,X','Local','1,0',0,0,'1,0','0',NULL)
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCeP',1,103,'ACC',1,'Blind','E,S,X','Local','1,0',0,0,'1','0',NULL)
GO

INSERT INTO sysroReaderTemplates([Type],[IDReader],[ID],[ScopeMode],[UseDispKey],[InteractionMode],[InteractionAction],[ValidationMode],[EmployeesLimit],[OHP],[CustomButtons],[Output],[InvalidOutput],[Direction])
 VALUES ('rxCeP',1,104,'ACCTA',1,'Blind','E,S,X','Local','1,0',0,0,'1','0',NULL)
GO


UPDATE dbo.sysroParameters SET Data='393' WHERE ID='DBVersion'
GO
