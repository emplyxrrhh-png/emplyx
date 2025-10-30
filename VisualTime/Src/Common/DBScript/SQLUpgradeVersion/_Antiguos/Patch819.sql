-- No borréis esta línea
DROP PROCEDURE IF EXISTS [dbo].[Report_registro_jornada_laboral]
GO
CREATE PROCEDURE [dbo].[Report_registro_jornada_laboral]
	@idPassport nvarchar(100) = '1',
	@IDEmployee nvarchar(max) = '0',
	@FechaInicio date = '20210301',
	@FechaFin date = '20210301'
AS
select distinct punches.IDEmployee, ShiftDate from punches 
inner join (select * from splitMAX(@IDEmployee,',')) tmp on punches.IDEmployee = tmp.Value
inner join sysrovwSecurity_PermissionOverEmployees poe WITH (NOLOCK) On poe.IDPassport =  @idPassport And poe.IDEmployee = punches.IDEmployee And cast(isnull(@FechaInicio,getdate()) as date) between poe.BeginDate And poe.EndDate 
where cast(ShiftDate as date) between cast(isnull(@FechaInicio,getdate()) as date) and cast(isnull(@FechaFin,getdate()) as date) 
and ActualType in (1,2) and punches.IDEmployee > 0 
RETURN NULL 
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='819' WHERE ID='DBVersion'
GO
