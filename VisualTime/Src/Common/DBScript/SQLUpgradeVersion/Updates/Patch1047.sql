IF NOT EXISTS (
    SELECT 1
    FROM sysroLiveAdvancedParameters
    WHERE ParameterName = 'VTLive.Requests.RulesToCheck'
          AND ISNULL(Value, '') <> ''
)
BEGIN
    DECLARE @NextID INT;

    SELECT @NextID = ISNULL(MAX(IDRule), 0) + 1 FROM RequestsRules;

    INSERT INTO RequestsRules (IDRule, IDLabAgree, IDRequestType, IDRuleType, Name, Definition, Activated, RuleCriteria)
    SELECT 
        ROW_NUMBER() OVER (ORDER BY ID) + @NextID - 1 AS IDRule,
        ID AS IDLabAgree,
        8 AS IDRequestType,
        10 AS IDRuleType,
        'Cumplimiento planificación' AS Name,
        '<?xml version="1.0"?><roCollection version="2.0"><Item key="EmployeeValidation" type="11">True</Item><Item key="IDReasons" type="8">-1</Item><Item key="IDPlanificationRules" type="8">-1</Item><Item key="Action" type="2">0</Item></roCollection>' AS Definition,
        1 AS Activated,
        '<?xml version="1.0"?><roCollection version="2.0"><Item key="TotalConditions" type="2">0</Item></roCollection>' AS RuleCriteria
    FROM 
        LabAgree;
END;

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1047' WHERE ID='DBVersion'
GO
