-- Nuevos elementos para auditoría
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(35,'EmployeeUsrFieldData'	,NULL)
GO
INSERT INTO [dbo].[sysroAuditElements]([ID], [Name], [Description]) VALUES(36,'AccessGroup'	,NULL)
GO


--
-- Actualiza versión de la base de datos
--
UPDATE sysroParameters SET Data='193' WHERE ID='DBVersion'
GO

