--Corrección en la creación de una función para sql 2005
CREATE FUNCTION [dbo].[GetAllPassportParentsRequestPermissions]
 	(
 		
 	)
 RETURNS @result table (IDPassport int, IDParentPassport int, RequestPermissionCount int)
 AS
 BEGIN
 	/* Returns all parents of specified passport */
 	
 	Declare @idPassport int
 	Declare @idRunningPassport int
 	Declare @permissionsCount int
 	
 	DECLARE db_cursor CURSOR FOR  
	SELECT ID
	FROM sysroPassports
	WHERE GroupType <> 'U' And IDUser is not null

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @idPassport  

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		   set @idRunningPassport = @idPassport
		   
		   SELECT @idRunningPassport = IDParentPassport FROM sysroPassports WHERE ID = @idRunningPassport
		   
		   WHILE NOT @idRunningPassport IS NULL
			BEGIN
				set @permissionsCount = (select COUNT(*) from sysroPassports_PermissionsOverFeatures where IDPassport = @idRunningPassport and IDFeature in (1560,2321,2322,2323,2510,2520,2540,2550,25800) )
				INSERT INTO @result VALUES (@idPassport,@idRunningPassport, @permissionsCount)
				
				SELECT @idRunningPassport = IDParentPassport
				FROM sysroPassports
				WHERE ID = @idRunningPassport
			END
			
				   
		   FETCH NEXT FROM db_cursor INTO @idPassport  
	END  

	CLOSE db_cursor  
	DEALLOCATE db_cursor 
 	
 	RETURN 
 END
GO
--Fin corrección

CREATE TABLE [dbo].[TmpAnnualConceptsEmployee](
	[IDPassport] [int] NOT NULL,
	[IDEmployee] [int] NOT NULL,
	[Ejercicio][int] NOT NULL,
	[Periodo] [int] NOT NULL,
	[IdGroup] [int] NULL,
	[GroupName] [nvarchar](200) NULL,
	[EmployeeName] [nvarchar](50) NULL,
	[IDConcept1] [smallint] NULL, [ConceptColor1] [nvarchar](10) NULL, [ConceptName1] [nvarchar](3) NULL, [ConceptValue1] [numeric](19, 6) NULL, [ConceptValueTotal1] [numeric](19, 6) NULL, 
	[IDConcept2] [smallint] NULL, [ConceptColor2] [nvarchar](10) NULL, [ConceptName2] [nvarchar](3) NULL, [ConceptValue2] [numeric](19, 6) NULL, [ConceptValueTotal2] [numeric](19, 6) NULL, 
	[IDConcept3] [smallint] NULL, [ConceptColor3] [nvarchar](10) NULL, [ConceptName3] [nvarchar](3) NULL, [ConceptValue3] [numeric](19, 6) NULL, [ConceptValueTotal3] [numeric](19, 6) NULL, 
	[IDConcept4] [smallint] NULL, [ConceptColor4] [nvarchar](10) NULL, [ConceptName4] [nvarchar](3) NULL, [ConceptValue4] [numeric](19, 6) NULL, [ConceptValueTotal4] [numeric](19, 6) NULL, 
	[IDConcept5] [smallint] NULL, [ConceptColor5] [nvarchar](10) NULL, [ConceptName5] [nvarchar](3) NULL, [ConceptValue5] [numeric](19, 6) NULL, [ConceptValueTotal5] [numeric](19, 6) NULL, 
	[IDConcept6] [smallint] NULL, [ConceptColor6] [nvarchar](10) NULL, [ConceptName6] [nvarchar](3) NULL, [ConceptValue6] [numeric](19, 6) NULL, [ConceptValueTotal6] [numeric](19, 6) NULL, 
	[IDConcept7] [smallint] NULL, [ConceptColor7] [nvarchar](10) NULL, [ConceptName7] [nvarchar](3) NULL, [ConceptValue7] [numeric](19, 6) NULL, [ConceptValueTotal7] [numeric](19, 6) NULL, 
	[IDConcept8] [smallint] NULL, [ConceptColor8] [nvarchar](10) NULL, [ConceptName8] [nvarchar](3) NULL, [ConceptValue8] [numeric](19, 6) NULL, [ConceptValueTotal8] [numeric](19, 6) NULL, 
	[IDConcept9] [smallint] NULL, [ConceptColor9] [nvarchar](10) NULL, [ConceptName9] [nvarchar](3) NULL, [ConceptValue9] [numeric](19, 6) NULL, [ConceptValueTotal9] [numeric](19, 6) NULL, 
	[IDConcept10] [smallint] NULL, [ConceptColor10] [nvarchar](10) NULL, [ConceptName10] [nvarchar](3) NULL, [ConceptValue10] [numeric](19, 6) NULL, [ConceptValueTotal10] [numeric](19, 6) NULL, 
	[IDConcept11] [smallint] NULL, [ConceptColor11] [nvarchar](10) NULL, [ConceptName11] [nvarchar](3) NULL, [ConceptValue11] [numeric](19, 6) NULL, [ConceptValueTotal11] [numeric](19, 6) NULL, 
	[IDConcept12] [smallint] NULL, [ConceptColor12] [nvarchar](10) NULL, [ConceptName12] [nvarchar](3) NULL, [ConceptValue12] [numeric](19, 6) NULL, [ConceptValueTotal12] [numeric](19, 6) NULL, 
	[IDConcept13] [smallint] NULL, [ConceptColor13] [nvarchar](10) NULL, [ConceptName13] [nvarchar](3) NULL, [ConceptValue13] [numeric](19, 6) NULL, [ConceptValueTotal13] [numeric](19, 6) NULL, 
	[IDConcept14] [smallint] NULL, [ConceptColor14] [nvarchar](10) NULL, [ConceptName14] [nvarchar](3) NULL, [ConceptValue14] [numeric](19, 6) NULL, [ConceptValueTotal14] [numeric](19, 6) NULL, 
	[IDConcept15] [smallint] NULL, [ConceptColor15] [nvarchar](10) NULL, [ConceptName15] [nvarchar](3) NULL, [ConceptValue15] [numeric](19, 6) NULL, [ConceptValueTotal15] [numeric](19, 6) NULL, 
	[IDConcept16] [smallint] NULL, [ConceptColor16] [nvarchar](10) NULL, [ConceptName16] [nvarchar](3) NULL, [ConceptValue16] [numeric](19, 6) NULL, [ConceptValueTotal16] [numeric](19, 6) NULL, 
	[IDConcept17] [smallint] NULL, [ConceptColor17] [nvarchar](10) NULL, [ConceptName17] [nvarchar](3) NULL, [ConceptValue17] [numeric](19, 6) NULL, [ConceptValueTotal17] [numeric](19, 6) NULL, 
	[IDConcept18] [smallint] NULL, [ConceptColor18] [nvarchar](10) NULL, [ConceptName18] [nvarchar](3) NULL, [ConceptValue18] [numeric](19, 6) NULL, [ConceptValueTotal18] [numeric](19, 6) NULL, 
	[IDConcept19] [smallint] NULL, [ConceptColor19] [nvarchar](10) NULL, [ConceptName19] [nvarchar](3) NULL, [ConceptValue19] [numeric](19, 6) NULL, [ConceptValueTotal19] [numeric](19, 6) NULL, 
	[IDConcept20] [smallint] NULL, [ConceptColor20] [nvarchar](10) NULL, [ConceptName20] [nvarchar](3) NULL, [ConceptValue20] [numeric](19, 6) NULL, [ConceptValueTotal20] [numeric](19, 6) NULL, 
	[IDConcept21] [smallint] NULL, [ConceptColor21] [nvarchar](10) NULL, [ConceptName21] [nvarchar](3) NULL, [ConceptValue21] [numeric](19, 6) NULL, [ConceptValueTotal21] [numeric](19, 6) NULL, 
	[IDConcept22] [smallint] NULL, [ConceptColor22] [nvarchar](10) NULL, [ConceptName22] [nvarchar](3) NULL, [ConceptValue22] [numeric](19, 6) NULL, [ConceptValueTotal22] [numeric](19, 6) NULL, 
	[IDConcept23] [smallint] NULL, [ConceptColor23] [nvarchar](10) NULL, [ConceptName23] [nvarchar](3) NULL, [ConceptValue23] [numeric](19, 6) NULL, [ConceptValueTotal23] [numeric](19, 6) NULL, 
	[IDConcept24] [smallint] NULL, [ConceptColor24] [nvarchar](10) NULL, [ConceptName24] [nvarchar](3) NULL, [ConceptValue24] [numeric](19, 6) NULL, [ConceptValueTotal24] [numeric](19, 6) NULL, 
	[IDConcept25] [smallint] NULL, [ConceptColor25] [nvarchar](10) NULL, [ConceptName25] [nvarchar](3) NULL, [ConceptValue25] [numeric](19, 6) NULL, [ConceptValueTotal25] [numeric](19, 6) NULL, 
	[IDConcept26] [smallint] NULL, [ConceptColor26] [nvarchar](10) NULL, [ConceptName26] [nvarchar](3) NULL, [ConceptValue26] [numeric](19, 6) NULL, [ConceptValueTotal26] [numeric](19, 6) NULL, 
	[IDConcept27] [smallint] NULL, [ConceptColor27] [nvarchar](10) NULL, [ConceptName27] [nvarchar](3) NULL, [ConceptValue27] [numeric](19, 6) NULL, [ConceptValueTotal27] [numeric](19, 6) NULL, 
	[IDConcept28] [smallint] NULL, [ConceptColor28] [nvarchar](10) NULL, [ConceptName28] [nvarchar](3) NULL, [ConceptValue28] [numeric](19, 6) NULL, [ConceptValueTotal28] [numeric](19, 6) NULL, 
	[IDConcept29] [smallint] NULL, [ConceptColor29] [nvarchar](10) NULL, [ConceptName29] [nvarchar](3) NULL, [ConceptValue29] [numeric](19, 6) NULL, [ConceptValueTotal29] [numeric](19, 6) NULL, 
	[IDConcept30] [smallint] NULL, [ConceptColor30] [nvarchar](10) NULL, [ConceptName30] [nvarchar](3) NULL, [ConceptValue30] [numeric](19, 6) NULL, [ConceptValueTotal30] [numeric](19, 6) NULL, 
	[IDConcept31] [smallint] NULL, [ConceptColor31] [nvarchar](10) NULL, [ConceptName31] [nvarchar](3) NULL, [ConceptValue31] [numeric](19, 6) NULL, [ConceptValueTotal31] [numeric](19, 6) NULL 

CONSTRAINT [PK_TmpAnnualConceptsEmployee] PRIMARY KEY CLUSTERED 
(
	[IdPassport] ASC,
	[IdEmployee] ASC,
	[Ejercicio] ASC,
	[Periodo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO




-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='311' WHERE ID='DBVersion'
GO

