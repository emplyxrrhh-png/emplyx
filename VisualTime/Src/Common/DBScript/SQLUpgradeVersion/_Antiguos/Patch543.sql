 ALTER FUNCTION [dbo].[GetValueFromEmployeeUserFieldValues]  
 (    
 @idEmployee int,  
 @FieldName nvarchar(50),  
 @Date smalldatetime  
 )  
 RETURNS nvarchar(100)  
 AS  
 BEGIN  
 DECLARE @Result nvarchar(100)  
 SELECT TOP 1 @Result = ISNULL(convert(nvarchar(100),[Value]),'') FROM EmployeeUserFieldValues WITH (NOLOCK)     
 WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND EmployeeUserFieldValues.FieldName = @FieldName AND   
 EmployeeUserFieldValues.Date <= @Date ORDER BY Date DESC  
 RETURN ISNULL(@Result,'')  
 END
 GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='543' WHERE ID='DBVersion'
GO