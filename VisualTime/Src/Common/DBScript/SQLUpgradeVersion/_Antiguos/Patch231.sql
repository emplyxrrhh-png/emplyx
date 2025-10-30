
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetAllEmployeeAllUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[GetAllEmployeeAllUserFieldValue]
GO

 CREATE FUNCTION [dbo].[GetAllEmployeeAllUserFieldValue]
 (				
 	@Date smalldatetime
 )
 RETURNS @ValueTable table(idEmployee int, FieldName nvarchar(50), [value] varchar(4000), [Date] smalldatetime) 
 AS
 BEGIN
 	INSERT INTO @ValueTable
 	SELECT Employees.ID, sysroUserFields.FieldName,
 			(SELECT TOP 1 CONVERT(varchar(4000), [Value])
 			 FROM EmployeeUserFieldValues
 			 WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
 			  	   EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
 				   EmployeeUserFieldValues.Date <= @Date
 			 ORDER BY EmployeeUserFieldValues.Date DESC),
 			ISNULL((SELECT TOP 1 [Date]
 				    FROM EmployeeUserFieldValues
 			        WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
 			  	          EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
 				          EmployeeUserFieldValues.Date <= @Date
 			        ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
 	FROM Employees, sysroUserFields
 	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1
 	RETURN
 END
 
 GO
 
/* ***************************************************************************************************************************** */
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetAllEmployeeUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[GetAllEmployeeUserFieldValue]
GO

 CREATE FUNCTION [dbo].[GetAllEmployeeUserFieldValue]
 (			
 	@FieldName nvarchar(50),
 	@Date smalldatetime
 )
 RETURNS @ValueTable table(idEmployee int, [value] varchar(4000), [Date] smalldatetime) 
 AS
 BEGIN
 	INSERT INTO @ValueTable
 	SELECT Employees.ID, 
 			(SELECT TOP 1 CONVERT(varchar(4000), [Value])
 			 FROM EmployeeUserFieldValues
 			 WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
 			  	   EmployeeUserFieldValues.FieldName = @FieldName AND
 				   EmployeeUserFieldValues.Date <= @Date
 			 ORDER BY EmployeeUserFieldValues.Date DESC),
 			ISNULL((SELECT TOP 1 [Date]
 				    FROM EmployeeUserFieldValues
 			        WHERE EmployeeUserFieldValues.IDEmployee = Employees.ID AND 
 			  	          EmployeeUserFieldValues.FieldName = @FieldName AND
 				          EmployeeUserFieldValues.Date <= @Date
 			        ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
 	FROM Employees, sysroUserFields
 	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 AND
 		  sysroUserFields.FieldName = @FieldName
 	RETURN
 END
 GO
 
/* ***************************************************************************************************************************** */
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetEmployeeAllUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[GetEmployeeAllUserFieldValue]
GO

 CREATE FUNCTION [dbo].[GetEmployeeAllUserFieldValue]
 (		
 	@idEmployee int,	
 	@Date smalldatetime
 )
 RETURNS @ValueTable table(FieldName nvarchar(50), [value] varchar(4000), [Date] smalldatetime) 
 AS
 BEGIN
 	INSERT INTO @ValueTable
 	SELECT sysroUserFields.FieldName,
 		   (SELECT TOP 1 CONVERT(varchar(4000), [Value])
 			FROM EmployeeUserFieldValues				
 			WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
 				  EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
 				  EmployeeUserFieldValues.Date <= @Date
 			ORDER BY EmployeeUserFieldValues.Date DESC),
 		   ISNULL((SELECT TOP 1 [Date]
 				   FROM EmployeeUserFieldValues				
 				   WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
 				         EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
 				         EmployeeUserFieldValues.Date <= @Date
 			       ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
 	FROM sysroUserFields
 	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 
 	RETURN
 END
 GO
 
/* ***************************************************************************************************************************** */
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[GetEmployeeUserFieldValue]') AND xtype in (N'FN', N'IF', N'TF'))
DROP FUNCTION [dbo].[GetEmployeeUserFieldValue]
GO

 CREATE FUNCTION [dbo].[GetEmployeeUserFieldValue]
 (		
 	@idEmployee int,
 	@FieldName nvarchar(50),
 	@Date smalldatetime
 )
 RETURNS @ValueTable table([value] varchar(4000), [Date] smalldatetime) 
 AS
 BEGIN
 	INSERT INTO @ValueTable
 	SELECT (SELECT TOP 1 CONVERT(varchar(4000), [Value])
 			FROM EmployeeUserFieldValues				
 			WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
 				  EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
 				  EmployeeUserFieldValues.Date <= @Date
 			ORDER BY EmployeeUserFieldValues.Date DESC),
 		   ISNULL((SELECT TOP 1 [Date]
 				   FROM EmployeeUserFieldValues				
 				   WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND 
 				         EmployeeUserFieldValues.FieldName = sysroUserFields.FieldName AND
 				         EmployeeUserFieldValues.Date <= @Date
 			       ORDER BY EmployeeUserFieldValues.Date DESC), CONVERT(smalldatetime, '1900/01/01', 120))
 	FROM sysroUserFields
 	WHERE sysroUserFields.Type = 0 AND sysroUserFields.Used = 1 AND
 		  sysroUserFields.FieldName = @FieldName
 	RETURN
 END
GO

 /* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='231' WHERE ID='DBVersion'
GO
