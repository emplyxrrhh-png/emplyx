-- Nuevo campos en los horarios para indicar parametros avanzados
ALTER TABLE [dbo].[Shifts] ADD
	[AdvancedParameters] [text] COLLATE SQL_Latin1_General_CP1_CI_AS NULL
GO
insert into sysroAuditElements values (40,'ProgrammedCause',null)
GO
insert into sysroAuditElements values (41,'AnalyticsCube',null)
GO
insert into sysroAuditScreens values (25,'Analytics',null)
GO
insert into sysroAuditScreens values (26,'DataLink',null)
GO
insert into sysroAuditElements values (42,'ImportEmployees',null)
GO
insert into sysroAuditElements values (43,'GuideExtraction',null)
GO


-- Creamos el botón de Enlace de Datos para Producción
insert into Sysrogui values ('NavBar\Job\DataLink','DataLink','roFormGuidesExtraction.vbd','exports.ico',NULL,NULL,'Forms\Datalink','311111111111111111111111111111',1000,'NWR')
GO
--Añadimos las guías de sólo lectura para extracción de datos de producción (Acumulados y Fichajes)
insert into GuidesExtraction values (9000,'Movimientos de producción','ASCII','Esta plantilla nos permitirá exportar los movimientos de producción de VisualTime.','<?xml version="1.0"?>  <roCollection version="2.0"><Item key="LenDecimals" type="8">2</Item><Item key="Header" type="8">1</Item><Item key="FormatConceptValue" type="8">1</Item><Item key="NameConcept" type="8">1</Item><Item key="Symbol" type="8">,</Item><Item key="ConceptValue" type="8">1</Item><Item key="ConceptZero" type="8">1</Item><Item key="TypeConcepts" type="2">0</Item><Item key="Concepts" type="8"></Item></roCollection>',null)
GO
insert into GuidesExtraction values (9001,'Acumulados de producción','ASCII','Esta plantilla nos permitirá exportar los acumulados de producción de VisualTime.','<?xml version="1.0"?>  <roCollection version="2.0"><Item key="LenDecimals" type="8">2</Item><Item key="Header" type="8">1</Item><Item key="FormatConceptValue" type="8">1</Item><Item key="NameConcept" type="8">1</Item><Item key="Symbol" type="8">,</Item><Item key="ConceptValue" type="8">1</Item><Item key="ConceptZero" type="8">1</Item><Item key="TypeConcepts" type="2">0</Item><Item key="Concepts" type="8"></Item></roCollection>',null)
GO


-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='208' WHERE ID='DBVersion'
GO





