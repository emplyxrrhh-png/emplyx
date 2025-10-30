UPDATE [dbo].[GeniusViews]
   SET CubeLayout = '{"slice":{"rows":[{"uniqueName":"GroupName"},{"uniqueName":"EmployeeName"},{"uniqueName":"UserField4"},{"uniqueName":"IDContract"},{"uniqueName":"UserField2"},{"uniqueName":"UserField1"},{"uniqueName":"UserField3"},{"uniqueName":"LabAgree"},{"uniqueName":"Age"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Enabled","aggregation":"count"}],"expands":{"expandAll":true},"flatOrder":["GroupName","EmployeeName","UserField4","IDContract","UserField2","UserField1","UserField3","Age"]},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"creationDate":"2021-12-16T15:19:03.964Z"}'					    
 WHERE Name = 'usersContract' and IdPassport = 0
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO


UPDATE sysroParameters SET Data='556' WHERE ID='DBVersion'
GO