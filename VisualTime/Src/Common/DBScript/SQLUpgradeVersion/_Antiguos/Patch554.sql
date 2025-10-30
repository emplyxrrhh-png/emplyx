

UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"IncidenceName"}],"rows":[{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"ShiftName"},{"uniqueName":"ConceptName"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")","caption":"Hours_ToHours"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"creationDate":"2022-01-20T09:37:50.150Z"}'
 WHERE Name = 'scheduleAndConcepts' and IdPassport = -1
GO



UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='554' WHERE ID='DBVersion'
GO