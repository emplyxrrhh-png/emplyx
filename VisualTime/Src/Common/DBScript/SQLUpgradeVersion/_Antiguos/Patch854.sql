-- No borréis esta línea

ALTER PROCEDURE [dbo].[Report_lista_de_tareas]
@idPassport nvarchar(100) = '1',
@IDTask nvarchar(max) = '0'
AS
select isnull(Project,'') as Project,  Name, Description, barcode, ID as TaskID from Tasks
inner join sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK)
on pof.IDPassport = @idPassport And pof.IdFeature = 1 AND pof.Permission > 1
where id > 0 and id in (select * from split(@IDTask,','))
RETURN NULL
GO


UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='854' WHERE ID='DBVersion'

GO
