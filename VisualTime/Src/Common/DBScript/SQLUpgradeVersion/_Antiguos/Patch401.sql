INSERT INTO sysrogui_actions([IDPath],[IDGUIPath],[LanguageTag],[RequieredFeatures],[RequieredFunctionalities],[AfterFunction],[CssClass],[Section],[ElementIndex]) 
	VALUES ('Remarks','Portal\ShiftControl\Calendar\Review','RemarksConfig','Forms\Calendar','U:Calendar.Highlight=Read','ShowRemarksConfig()','btnTbRemarksConfig2',0,5);
GO
 
 
UPDATE dbo.sysroParameters SET Data='401' WHERE ID='DBVersion'
GO
