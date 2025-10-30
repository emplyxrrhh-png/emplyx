/* Modificamos el stored de Visits_Visitor_DeleteFromDate debido a que en el primer update le faltaba una condición que hacia que mostrara errores de clave foránea al arrancar */
ALTER PROCEDURE [dbo].[Visits_Visitor_DeleteFromDate]
   	@Duration as int
AS
UPDATE VisitPlan SET VisitorID = NULL, VisitorAlias = 'Eliminado LOPD ' + Convert(Varchar,Getdate(),103) 
WHERE VisitorID IN (SELECT ID FROM Visitors WHERE LastVisitdate is not null and datediff(day,lastvisitdate,getdate())>@Duration OR LastVisitdate is null and datediff(day,Created,getdate()) >@Duration);
DELETE Visitors WHERE (LastVisitdate is not null and datediff(day,lastvisitdate,getdate()) >@Duration) OR (LastVisitdate is null and datediff(day,Created,getdate()) >@Duration);
DELETE VisitorUserFieldsValues FROM Visitors INNER JOIN VisitorUserFieldsValues ON Visitors.ID = VisitorUserFieldsValues.VisitorID WHERE (LastVisitdate is not null and datediff(day,lastvisitdate,getdate()) >@Duration) OR (LastVisitdate is null and datediff(day,Created,getdate()) >@Duration);
GO


/* ***************************************************************************************************************************** */
-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='249' WHERE ID='DBVersion'
GO
