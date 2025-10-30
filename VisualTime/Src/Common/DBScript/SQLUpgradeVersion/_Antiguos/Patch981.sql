CREATE OR ALTER PROCEDURE [dbo].[Report_logs_de_acceso]
@idPassport nvarchar(100) = '1',
@startDate datetime2 = '20210301',
@endDate datetime2 = '20210301'
AS
select * from sysroConnectionHistory 
where CAST(EventDateTime as date) between CAST(@startDate as date) and CAST(@endDate as date)
order by EventDateTime asc
RETURN NULL
GO

-- No borréis esta línea
UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='981' WHERE ID='DBVersion'
GO
