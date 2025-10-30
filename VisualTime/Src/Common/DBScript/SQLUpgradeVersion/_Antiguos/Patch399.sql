IF NOT EXISTS (SELECT * FROM [dbo].[sysroGUI_Actions] WHERE IDGUIPath = 'Portal\ShiftControl\Scheduler\calendar' AND IDPath = 'WeekPlan')
   BEGIN
       INSERT INTO [dbo].[sysroGUI_Actions]([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) VALUES ('WeekPlan','Portal\ShiftControl\Scheduler\calendar','WeekPlan','Forms\Calendar','U:Calendar.Scheduler=Write','ShowWeekScheduleWizard()','btnTbPlanS2',1,1)
   END
GO

DECLARE @LocationsToAppend AS nvarchar(max)
CREATE TABLE #Location (IDLocation int, Value varchar(200),IDImportGuide int);

--insert import guides
insert into  #Location(Value,IDImportGuide)
(
	SELECT '@'+substring(SourceFilePath,0,LEN(cast(SourceFilePath as varchar(max))) + 1 - CHARINDEX('\', REVERSE(cast(SourceFilePath as varchar(max)))))+'@Link@', ID
	FROM importguides WHERE Mode=2 AND CHARINDEX('\',SourceFilePath) > 0
)

--set pending IDlocation
update #Location set IDLocation=(IDImportGuide*100) where IDImportGuide is not null
update #Location set Value=CAST(IDLocation as varchar(20))+ Value where IDImportGuide is not null

--generate variable to update
SELECT @LocationsToAppend = COALESCE(@LocationsToAppend + '*', '') + Value 
FROM #Location 
where IDImportGuide is not null 
order by IDImportGuide asc;

--update locations on parameters
IF @LocationsToAppend is not null
BEGIN
	if exists (select * from sysroParameters where ID='LOCATIONS' AND (Data is not null AND CONVERT(NVARCHAR(4000),Data) <> ''))
	BEGIN
		UPDATE sysroParameters SET Data=REPLACE(CAST(Data as varchar(max)),'</Item></roCollection>','*'+@LocationsToAppend+'</Item></roCollection>') where ID='LOCATIONS'
	END
	
	if exists (select * from sysroParameters where ID='LOCATIONS' AND (Data is null OR CONVERT(NVARCHAR(4000),Data) = ''))
	BEGIN
		UPDATE sysroParameters SET Data='<?xml version="1.0"?><roCollection version="2.0"><Item key="Locations" type="8">'+@LocationsToAppend+'</Item></roCollection>' where ID='LOCATIONS'
	END

	--update importguides
	update importguides set Destination=l.IDLocation from importguides as g inner join #Location as l on g.ID=l.IDImportGuide
	update importguides set SourceFilePath=Right(cast(SourceFilePath as varchar(max)),CHARINDEX('\', REVERSE(cast(SourceFilePath as varchar(max))))-1) from importguides as g inner join #Location as l on g.ID=l.IDImportGuide where g.SourceFilePath is not null and CHARINDEX('\',g.SourceFilePath) > 0
	update importguides set FormatFilePath=Right(cast(FormatFilePath as varchar(max)),CHARINDEX('\', REVERSE(cast(FormatFilePath as varchar(max))))-1) from importguides as g inner join #Location as l on g.ID=l.IDImportGuide where g.FormatFilePath is not null and CHARINDEX('\',g.FormatFilePath) > 0
END

DROP TABLE #Location
GO

UPDATE dbo.sysroParameters SET Data='399' WHERE ID='DBVersion'
GO
