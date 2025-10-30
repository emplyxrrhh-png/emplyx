UPDATE GeniusViews SET CubeLayout = '{"slice":{"reportFilters":[{"uniqueName":"IDContract"},{"uniqueName":"CurrentEmployee"}],"rows":[{"uniqueName":"EmployeeName"},{"uniqueName":"Mes"},{"uniqueName":"Date_ToDateString"},{"uniqueName":"PlannedDayType"},{"uniqueName":"ExpectedWorkingHours(HH:MM)_TOHOURS"},{"uniqueName":"PunchDayType"},{"uniqueName":"EffectivePresenceTime(HH:MM)_TOHOURS"},{"uniqueName":"EffectiveTCTime(HH:MM)_TOHOURS"}],"columns":[{"uniqueName":"[Measures]"}],"measures":[{"uniqueName":"Compliance","formula":"IF((\"IdPunchDayType\" = 0 AND \"IdPlannedDayType\"  = 0) OR (\"IdPunchDayType\" = 1 AND \"IdPlannedDayType\"  = 1) OR (\"IdPlannedDayType\" = 2 AND \"IdPunchDayType\" = 0), 1, 0)"}],"expands":{"expandAll":true}},"options":{"grid":{"type":"classic","showTotals":"off","showGrandTotals":"off"}},"conditions":[{"formula":"#value = 0","measure":"Compliance","format":{"backgroundColor":"#FF0000","color":"#FF0000","fontFamily":"Arial","fontSize":"12px"}},{"formula":"#value = 1","measure":"Compliance","format":{"backgroundColor":"#FFFFFF","color":"#FFFFFF","fontFamily":"Arial","fontSize":"12px"}}],"creationDate":"2022-09-26T11:36:54.677Z"}'
WHERE NAME = 'telecommutingAgreementCompliance' AND IdPassport = 0

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='604' WHERE ID='DBVersion'
GO
