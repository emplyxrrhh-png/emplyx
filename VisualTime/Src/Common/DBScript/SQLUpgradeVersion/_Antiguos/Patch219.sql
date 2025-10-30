IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmployeeGroup]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetEmployeeGroup]

GO

 CREATE FUNCTION [dbo].[GetEmployeeGroup] 
 	(
 		@idEmployee int,
 		@date smalldatetime
 	)
 RETURNS int
 AS
 	BEGIN
 		
 		DECLARE @Result int
 	
		SELECT @date = CONVERT(smalldatetime, CONVERT(varchar, @date, 112))

 		SELECT @Result = IDGroup
 		FROM EmployeeGroups
 		WHERE IDEmployee = @idEmployee AND
 			@date >= BeginDate AND @date <= EndDate

		IF @Result IS NULL 
			BEGIN

				SELECT TOP 1 @Result = IDGroup
				FROM EmployeeGroups
				WHERE IDEmployee = @idEmployee AND 
					  BeginDate > @date
				ORDER BY BeginDate

			END
 			
 	RETURN @Result
 	END

GO

DROP TABLE TMPTOP
GO
CREATE TABLE [dbo].[TMPTOP](
	[IDEmployee] [int] NOT NULL,
	[EmployeeName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[IDPrimaryConcept] [smallint] NOT NULL,
	[IDSecundaryConcept] [smallint] NULL,
	[ConceptPrimaryName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[ConceptSecundaryName] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[Date] [datetime] NOT NULL,
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[Value] [numeric](19, 6) NULL,
 CONSTRAINT [PK_TMPTOP] PRIMARY KEY CLUSTERED 
(
	[IDEmployee] ASC,
	[IDPrimaryConcept] ASC,
	[Date] ASC
) ON [PRIMARY]
) ON [PRIMARY]
GO

DELETE FROM sysroGUI WHERE IDPath = 'Portal\Configuration\DataLink'
GO
INSERT INTO sysroGUI (IDPath,LanguageReference,URL,IconURL,Priority,AllowedSecurity,RequiredFunctionalities,RequiredFeatures) VALUES ('Portal\Configuration\DataLink','DataLink','DataLink/DataLink.aspx','DataLink.png',140,NULL,NULL,'Forms\DataLink')
GO

-- Actualizamos versión de BBDD
UPDATE sysroParameters SET Data='219' WHERE ID='DBVersion'
GO
