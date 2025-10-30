IF NOT EXISTS (SELECT 1 FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroVisualtimeID')
	INSERT INTO [dbo].[sysroUserFields] ([FieldName],[FieldType],[Used],[AccessLevel] ,[Pos],[Category],[ListValues],[Type],[Description],[AccessValidation],[History],[RequestPermissions],[RequestCriteria],[isSystem],[Alias],[DocumentTemplateId]) 
	VALUES('Identificador usuario',0,1,0,null,'VisualTime','',0,'',0,0,1,'<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>',1,'sysroVisualtimeID',null)
GO

ALTER TABLE [dbo].[sysrouserfields] ADD [ReadOnly] bit not null default (0)
GO

UPDATE [dbo].[sysroUserFields] set [ReadOnly] = 1 where [Alias] = 'sysroVisualtimeID'
GO

INSERT INTO [dbo].[EmployeeUserFieldValues] (IDEmployee,Fieldname,date,value) SELECT id,(SELECT FieldName FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroVisualtimeID'),convert(smalldatetime,'1900-01-01',120), convert(nvarchar(max),employees.id) from [dbo].[employees]
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='608' WHERE ID='DBVersion'
GO
