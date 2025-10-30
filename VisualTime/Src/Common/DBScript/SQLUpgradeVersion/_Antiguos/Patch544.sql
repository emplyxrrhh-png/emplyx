UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Price"}],"columns":[{"uniqueName":"ConceptName"},{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Hours_ToHours","formula":"sum(\"Value\")"},{"uniqueName":"Importe","formula":"sum(\"Price\") * sum(\"Value\")","individual":true,"caption":"Importe"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"},"timePattern":"HH:mm"},"creationDate":"2022-03-15T12:45:24.218Z"}'
 WHERE Name = 'hoursReceiptByContract' and IdPassport = 0
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='544' WHERE ID='DBVersion'
GO