ALTER TABLE dbo.sysroReportGroups ADD
	BusinessGroup nvarchar(50) NULL
GO

ALTER TABLE dbo.Requests ADD
	IDCenter int NULL
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(2324,2320,'Calendar.Punches.Requests.CostCenterForgotten','Fichajes olvidados','','U','RWA',NULL,12,2)
GO

INSERT INTO [dbo].[sysroFeatures]([ID],[IDParent],[Alias],[Name],[Description],[Type],[PermissionTypes],[AppHasPermissionsOverEmployees],[AliasID],[EmployeeFeatureID])
     VALUES(20140,20100,'Punches.Requests.CostCenterForgotten','Fichajes Olvidados de centro de coste','','E','RW',NULL,NULL,NULL)
GO

--Función para conversión de hexa a decimal
create function [dbo].[fn_HexToIntnt](@str varchar(16)) returns bigint as begin

	-- Convertir en mayusculas
	select @str=upper(@str)

	declare @i int, @len int, @char char(1), @output bigint

	--Obtenemos longitud
	select @len=len(@str)
			,@i=@len
			,@output=case when @len>0 then 0 end

	while (@i>0)
		begin
			select @char=substring(@str,@i,1), @output=@output+(ASCII(@char)-(case when @char between 'A' and 'F' then 55 else
																			  case when @char between '0' and '9' then 48 end end))*power(16.,@len-@i),@i=@i-1
	end
	return @output
end

GO
 
UPDATE dbo.sysroParameters SET Data='359' WHERE ID='DBVersion'
GO