ALTER TABLE dbo.ImportGuides ADD
   RequieredFunctionalities nvarchar(200) NULL
GO

INSERT INTO [dbo].[ImportGuides]([ID],[Name],[Template],[Mode],[Type],[FormatFilePath],[SourceFilePath],[Separator],[CopySource],[LastLog],[RequieredFunctionalities])
     VALUES (11,'Importación de justificaciones',0,0,0,NULL,'','',0,'','Calendar.DirectCauses')
GO

INSERT INTO [dbo].[sysroLiveAdvancedParameters]([ParameterName],[Value]) VALUES('ImportPrimaryKeyUserField','NIF')
GO

-- Forzamos el lanzamiento de broadcaster si hay mx8 para que se generen los ficheros para USB
IF EXISTS (select top 1 * from Terminals where [Type] like 'mx8')
INSERT INTO dbo.sysroTasks (ProcessID,CreatorID,Context,DateCreated ,ID ,SystemTask ,ExternalTask)  VALUES
('PROC:\\BROADCASTER','SESSION:\\SYSTEM','<?xml version="1.0"?><roCollection version="2.0"><Item key="Status" type="8">PENDING</Item><Item key="Command" type="8">ON_DAY_CHANGE</Item></roCollection>', getdate(),'TASK:\\999999', 0, 0)
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='339' WHERE ID='DBVersion'
GO


