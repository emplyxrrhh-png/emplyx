-- Creamos comportamientos para terminal mxART
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxART',1,35,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxART',1,36,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxART',2,37,'ACC','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxART',2,38,'ACCTA','1','Blind','X','Local','0','0','0','1,0','0')
GO
INSERT INTO sysroReaderTemplates(Type, IDReader, ID, ScopeMode, UseDispKey, InteractionMode, InteractionAction, ValidationMode, EmployeesLimit, OHP, CustomButtons, Output, InvalidOutput) 
VALUES('mxART',2,39,'',NULL,NULL,NULL,NULL,'0',NULL,NULL,NULL,NULL)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='259' WHERE ID='DBVersion'
GO
