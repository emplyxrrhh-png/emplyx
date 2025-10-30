CREATE FUNCTION [dbo].[GetValueFromEmployeeUserFieldValuesWithDefault]  
(  
@idEmployee int,  
@FieldName nvarchar(50),  
@Date smalldatetime,
@DefaultValue nvarchar(50)
)  
RETURNS nvarchar(100)  
AS  
BEGIN  
DECLARE @Result nvarchar(100)  
SELECT TOP 1 @Result = ISNULL(convert(nvarchar(100),[Value]),'') FROM EmployeeUserFieldValues WITH (NOLOCK)  
WHERE EmployeeUserFieldValues.IDEmployee = @idEmployee AND EmployeeUserFieldValues.FieldName = @FieldName AND  
EmployeeUserFieldValues.Date <= @Date ORDER BY Date DESC  
RETURN ISNULL(@Result,@DefaultValue)
END  
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='569' WHERE ID='DBVersion'
GO