IF not exists (select * from [dbo].[sysroLiveAdvancedParameters] where ParameterName = 'vst_locationField')
	insert into [dbo].[sysroLiveAdvancedParameters] values ('vst_locationField', 'Ubicación')
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TMPVisitsLocation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[TMPVisitsLocation](
	[IDVisitor] [nvarchar](40) NULL,
	[VisitorName] [nvarchar](200) NULL,
	[IDEmployee] [int] NULL,
	[EmployeeName] [nvarchar](200) NULL,
	[IDVisit] [nvarchar](40) NULL,
	[Location] [nvarchar](40) NULL,
	[DateTime] [datetime] NULL,
	[IDReportTask] [numeric](16, 0) NULL
) ON [PRIMARY]
END
GO

if not exists (select * from [sysroLiveAdvancedParameters] where ParameterName = 'SeeWithOutTask')
	insert into [sysroLiveAdvancedParameters] values ('SeeWithOutTask','0')
GO

CREATE FUNCTION [dbo].[GetEmployeeGroupWithChilds] 
  	(
  		@idEmployeeGroup int
  	)
  RETURNS @ValueTable table(ID int)
  AS
  	BEGIN
  		DECLARE @Path nvarchar(100)
  		SELECT @Path = Path
  		FROM Groups
  		WHERE ID = @idEmployeeGroup
  	
  		INSERT INTO @ValueTable
  			SELECT ID
  			FROM Groups
  			WHERE Path LIKE @Path + '\%' or Path = @Path 
  			ORDER BY ID
  		
  	RETURN
END 
GO
	
UPDATE dbo.sysroParameters SET Data='385' WHERE ID='DBVersion'
GO
