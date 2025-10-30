UPDATE dbo.GeniusViews SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"},{"uniqueName":"UserField1"},{"uniqueName":"UserField2"},{"uniqueName":"UserField3"},{"uniqueName":"UserField4"},{"uniqueName":"UserField5"},{"uniqueName":"UserField6"},{"uniqueName":"UserField7"},{"uniqueName":"UserField8"},{"uniqueName":"UserField9"},{"uniqueName":"UserField10"}],"rows":[{"uniqueName":"FullGroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"punches"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"dynamic_concepts","aggregation":"count"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off"}},"creationDate":"2021-06-28T07:53:55.468Z"}'
WHERE NAME = 'conceptsColumns' AND IdPassport = -1

GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='535' WHERE ID='DBVersion'
GO