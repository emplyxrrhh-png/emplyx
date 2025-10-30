-- Modificamos Stored de Visitas
/****** Object:  StoredProcedure [Visits_VisitPlan_Update]    Script Date: 10/26/2007 15:20:21 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[Visits_VisitPlan_Update]') AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [Visits_VisitPlan_Update]
GO

/****** Object:  StoredProcedure [Visits_VisitPlan_Update]    Script Date: 10/26/2007 15:19:44 ******/
 CREATE PROCEDURE [Visits_VisitPlan_Update]
 (
 	@VisitorAlias nvarchar(50),
 	@VisitorId int,
 	@EmpVisitedId int,
 	@Date smalldatetime,
 	@Comments text,
 	@Status int,
 	@Type int,
 	@ID int,
	@Ticket int
 )
 AS
 	SET NOCOUNT OFF;
 UPDATE VisitPlan SET VisitorAlias = @VisitorAlias, VisitorId = @VisitorId, EmpVisitedId = @EmpVisitedId, Date = @Date, Comments = @Comments, Status = @Status, Type = @Type, Ticket = @Ticket WHERE (ID = @ID) ;
 	SELECT ID, VisitorAlias, VisitorId, EmpVisitedId, Date, Comments, Status, PlannedById, Type FROM VisitPlan WHERE (ID = @ID)
GO

-- Creamos nuevo Stored para Visitas
CREATE             PROCEDURE [Visits_VisitPlan_GetNextTicket]
AS
SET NOCOUNT OFF;
Declare @Ticket int
SELECT @Ticket = count(*) + 1 FROM visitmoves
WHERE year([BeginTime]) =  year(getdate()) 
       AND month([BeginTime]) =  month(getdate()) 
       AND day([BeginTime]) =  day(getdate())
RETURN @Ticket
GO

UPDATE sysroParameters SET Data='204' WHERE ID='DBVersion'
GO

