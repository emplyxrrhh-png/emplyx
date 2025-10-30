update geniusviews set CustomFields = REPLACE(CustomFields,'"IncludeZeroBusinessCenter":true','"IncludeZeroBusinessCenter":false')
where dsfunction = 'Genius_CostCenters_Detail(@initialDate,@endDate,@idpassport,@employeeFilter,@userFieldsFilter,@costCenterFilter,@causesFilter,@includeZeros)' 
and CustomFields like '%"IncludeZeroCauses":true%'

GO

-- No borréis esta línea

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='691' WHERE ID='DBVersion'
GO
