-- No borréis esta línea
INSERT INTO [dbo].[sysroGUI]
           ([IDPath]
           ,[LanguageReference]
           ,[URL]
           ,[IconURL]
           ,[Type]
           ,[Parameters]
           ,[RequiredFeatures]
           ,[SecurityFlags]
           ,[Priority]
           ,[AllowedSecurity]
           ,[RequiredFunctionalities]
           ,[Edition])
     VALUES
           ('Portal\Configuration\RulesGroup',	'Gui.Rules',	'/RulesGroup',	'RulesGroup.png',	NULL,	'June6610',	NULL,	NULL,	2208,	NULL,	'U:Administration.Options=Read',	NULL)
GO

CREATE TABLE TMPDummyRulesGroups (
    DummyData NVARCHAR(MAX)
)
GO

INSERT INTO TMPDummyRulesGroups (DummyData)
VALUES (N'[
  {
    "Id": 1,
    "Name": "Grupo de Reglas para Departamento IT",
    "Description": "Reglas aplicables al personal del departamento de IT",
    "EmployeeContext": null,
    "Shifts": [
      {
        "Id": 101,
        "Name": "Turno Mañana"
      },
      {
        "Id": 102,
        "Name": "Turno Tarde"
      }
    ],
    "Rules": [
      {
        "Id": 1001,
        "Name": "Validación de justificante médico",
        "Description": "Regla para validar justificantes médicos",
        "Type": 2,
        "TypeDescription": "Justification",
        "GroupId": 1,
        "Conditions": [],
        "Actions": [],
        "XmlDefinition": "<?xml version=\"1.0\"?><roCollection version=\"2.0\"><Item key=\"Incidence\" type=\"2\">1031</Item><Item key=\"Zone\" type=\"2\">-1</Item><Item key=\"ConditionValueType\" type=\"2\">0</Item><Item key=\"FromTime\" type=\"7\">00:00:00</Item><Item key=\"ToTime\" type=\"7\">23:59:00</Item><Item key=\"FromValueUserField\" type=\"8\"></Item><Item key=\"ToValueUserField\" type=\"8\"></Item><Item key=\"BetweenValueUserField\" type=\"8\"></Item><Item key=\"Cause\" type=\"2\">1</Item><Item key=\"ActionValueType\" type=\"2\">0</Item><Item key=\"MaxTime\" type=\"7\">23:59:00</Item><Item key=\"MaxValueUserField\" type=\"8\"></Item></roCollection>",
        "Tags": ["Médico", "Justificante", "Automático"],
        "BeginDate": "2023-01-01T00:00:00",
        "EndDate": null
      },
      {
        "Id": 1002,
        "Name": "Justificación por formación",
        "Description": "Valida automáticamente las formaciones aprobadas",
        "Type": 2,
        "TypeDescription": "Justification",
        "GroupId": 1,
        "Conditions": [],
        "Actions": [],
        "XmlDefinition": "<?xml version=\"1.0\"?><roCollection version=\"2.0\"><Item key=\"Incidence\" type=\"2\">1031</Item><Item key=\"Zone\" type=\"2\">-1</Item><Item key=\"ConditionValueType\" type=\"2\">0</Item><Item key=\"FromTime\" type=\"7\">00:00:00</Item><Item key=\"ToTime\" type=\"7\">23:59:00</Item><Item key=\"FromValueUserField\" type=\"8\"></Item><Item key=\"ToValueUserField\" type=\"8\"></Item><Item key=\"BetweenValueUserField\" type=\"8\"></Item><Item key=\"Cause\" type=\"2\">1</Item><Item key=\"ActionValueType\" type=\"2\">0</Item><Item key=\"MaxTime\" type=\"7\">23:59:00</Item><Item key=\"MaxValueUserField\" type=\"8\"></Item></roCollection>",
        "Tags": ["Formación", "Justificante"],
        "BeginDate": "2023-01-01T00:00:00",
        "EndDate": "2025-12-31T00:00:00"
      }
    ],
    "BeginDate": "2023-01-01T00:00:00",
    "EndDate": null
  },
  {
    "Id": 2,
    "Name": "Grupo de Reglas para Producción",
    "Description": "Reglas aplicables al personal de producción",
    "EmployeeContext": null,
    "Shifts": [
      {
        "Id": 201,
        "Name": "Turno Producción A"
      },
      {
        "Id": 202,
        "Name": "Turno Producción B"
      },
      {
        "Id": 203,
        "Name": "Turno Noche"
      }
    ],
    "Rules": [
      {
        "Id": 2001,
        "Name": "Cálculo horas extra",
        "Description": "Regla para el cálculo de horas extra diarias",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 2,
        "Conditions": [],
        "Actions": [],
        "XmlDefinition": "<?xml version=\"1.0\"?><roCollection version=\"2.0\"><Item key=\"Name\" type=\"8\">Test</Item><Item key=\"Description\" type=\"8\"></Item><Item key=\"DayValidationRule\" type=\"2\">0</Item><Item key=\"PreviousShiftValidationRule\" type=\"8\">0</Item><Item key=\"RuleType\" type=\"8\">3</Item><Item key=\"TotalConditions\" type=\"2\">2</Item><Item key=\"ConditionCauses_1\" type=\"8\">93_0</Item><Item key=\"ConditionTimeZones_1\" type=\"8\">-1</Item><Item key=\"Compare_1\" type=\"2\">0</Item><Item key=\"Type_1\" type=\"2\">0</Item><Item key=\"FromValue_1\" type=\"8\">0:00</Item><Item key=\"ToValue_1\" type=\"8\">00:00</Item><Item key=\"UserField_1\" type=\"8\"></Item><Item key=\"CompareCauses_1\" type=\"8\"></Item><Item key=\"CompareTimeZones_1\" type=\"8\"></Item><Item key=\"ConditionCauses_2\" type=\"8\">61_0</Item><Item key=\"ConditionTimeZones_2\" type=\"8\">-1</Item><Item key=\"Compare_2\" type=\"2\">0</Item><Item key=\"Type_2\" type=\"2\">0</Item><Item key=\"FromValue_2\" type=\"8\">0:00</Item><Item key=\"ToValue_2\" type=\"8\">00:00</Item><Item key=\"UserField_2\" type=\"8\"></Item><Item key=\"CompareCauses_2\" type=\"8\"></Item><Item key=\"CompareTimeZones_2\" type=\"8\"></Item><Item key=\"TotalActions\" type=\"2\">1</Item><Item key=\"Action_1\" type=\"2\">0</Item><Item key=\"CarryOverAction_1\" type=\"2\">0</Item><Item key=\"CarryOverDirectValue_1\" type=\"8\">0:00</Item><Item key=\"CarryOverUserFieldValue_1\" type=\"8\"></Item><Item key=\"CarryOverConditionPart_1\" type=\"2\">0</Item><Item key=\"CarryOverConditionNumber_1\" type=\"2\">0</Item><Item key=\"CarryOverActionResult_1\" type=\"2\">0</Item><Item key=\"CarryOverDirectValueResult_1\" type=\"8\">00:00</Item><Item key=\"CarryOverUserFieldValueResult_1\" type=\"8\"></Item><Item key=\"CarryOverConditionPartResult_1\" type=\"2\">0</Item><Item key=\"CarryOverConditionNumberResult_1\" type=\"2\">0</Item><Item key=\"CarryOverIDCauseFrom_1\" type=\"2\">93</Item><Item key=\"CarryOverIDCauseTo_1\" type=\"2\">137</Item><Item key=\"PlusIDCause_1\" type=\"2\">0</Item><Item key=\"PlusAction_1\" type=\"2\">0</Item><Item key=\"PlusDirectValue_1\" type=\"8\">00:00</Item><Item key=\"PlusUserFieldValue_1\" type=\"8\"></Item><Item key=\"PlusConditionPart_1\" type=\"2\">0</Item><Item key=\"PlusConditionNumber_1\" type=\"2\">0</Item><Item key=\"PlusActionResult_1\" type=\"2\">0</Item><Item key=\"PlusDirectValueResult_1\" type=\"8\">00:00</Item><Item key=\"PlusUserFieldValueResult_1\" type=\"8\"></Item><Item key=\"PlusConditionPartResult_1\" type=\"2\">0</Item><Item key=\"PlusConditionNumberResult_1\" type=\"2\">0</Item><Item key=\"PlusActionSign_1\" type=\"2\">0</Item><Item key=\"CarryOverSingleCause_1\" type=\"8\">0</Item><Item key=\"ActionCauses_1\" type=\"8\"></Item></roCollection>",
        "Tags": ["Horas Extra", "Diario"],
        "BeginDate": "2023-01-01T00:00:00",
        "EndDate": null
      },
      {
        "Id": 2002,
        "Name": "Plus nocturnidad",
        "Description": "Aplica plus por trabajo en turno de noche",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 2,
        "Conditions": [],
        "Actions": [],
        "XmlDefinition": "<?xml version=\"1.0\"?><roCollection version=\"2.0\"><Item key=\"Name\" type=\"8\">Test</Item><Item key=\"Description\" type=\"8\"></Item><Item key=\"DayValidationRule\" type=\"2\">0</Item><Item key=\"PreviousShiftValidationRule\" type=\"8\">0</Item><Item key=\"RuleType\" type=\"8\">3</Item><Item key=\"TotalConditions\" type=\"2\">2</Item><Item key=\"ConditionCauses_1\" type=\"8\">93_0</Item><Item key=\"ConditionTimeZones_1\" type=\"8\">-1</Item><Item key=\"Compare_1\" type=\"2\">0</Item><Item key=\"Type_1\" type=\"2\">0</Item><Item key=\"FromValue_1\" type=\"8\">0:00</Item><Item key=\"ToValue_1\" type=\"8\">00:00</Item><Item key=\"UserField_1\" type=\"8\"></Item><Item key=\"CompareCauses_1\" type=\"8\"></Item><Item key=\"CompareTimeZones_1\" type=\"8\"></Item><Item key=\"ConditionCauses_2\" type=\"8\">61_0</Item><Item key=\"ConditionTimeZones_2\" type=\"8\">-1</Item><Item key=\"Compare_2\" type=\"2\">0</Item><Item key=\"Type_2\" type=\"2\">0</Item><Item key=\"FromValue_2\" type=\"8\">0:00</Item><Item key=\"ToValue_2\" type=\"8\">00:00</Item><Item key=\"UserField_2\" type=\"8\"></Item><Item key=\"CompareCauses_2\" type=\"8\"></Item><Item key=\"CompareTimeZones_2\" type=\"8\"></Item><Item key=\"TotalActions\" type=\"2\">1</Item><Item key=\"Action_1\" type=\"2\">0</Item><Item key=\"CarryOverAction_1\" type=\"2\">0</Item><Item key=\"CarryOverDirectValue_1\" type=\"8\">0:00</Item><Item key=\"CarryOverUserFieldValue_1\" type=\"8\"></Item><Item key=\"CarryOverConditionPart_1\" type=\"2\">0</Item><Item key=\"CarryOverConditionNumber_1\" type=\"2\">0</Item><Item key=\"CarryOverActionResult_1\" type=\"2\">0</Item><Item key=\"CarryOverDirectValueResult_1\" type=\"8\">00:00</Item><Item key=\"CarryOverUserFieldValueResult_1\" type=\"8\"></Item><Item key=\"CarryOverConditionPartResult_1\" type=\"2\">0</Item><Item key=\"CarryOverConditionNumberResult_1\" type=\"2\">0</Item><Item key=\"CarryOverIDCauseFrom_1\" type=\"2\">93</Item><Item key=\"CarryOverIDCauseTo_1\" type=\"2\">137</Item><Item key=\"PlusIDCause_1\" type=\"2\">0</Item><Item key=\"PlusAction_1\" type=\"2\">0</Item><Item key=\"PlusDirectValue_1\" type=\"8\">00:00</Item><Item key=\"PlusUserFieldValue_1\" type=\"8\"></Item><Item key=\"PlusConditionPart_1\" type=\"2\">0</Item><Item key=\"PlusConditionNumber_1\" type=\"2\">0</Item><Item key=\"PlusActionResult_1\" type=\"2\">0</Item><Item key=\"PlusDirectValueResult_1\" type=\"8\">00:00</Item><Item key=\"PlusUserFieldValueResult_1\" type=\"8\"></Item><Item key=\"PlusConditionPartResult_1\" type=\"2\">0</Item><Item key=\"PlusConditionNumberResult_1\" type=\"2\">0</Item><Item key=\"PlusActionSign_1\" type=\"2\">0</Item><Item key=\"CarryOverSingleCause_1\" type=\"8\">0</Item><Item key=\"ActionCauses_1\" type=\"8\"></Item></roCollection>",
        "Tags": ["Plus", "Nocturno"],
        "BeginDate": "2023-06-01T00:00:00",
        "EndDate": null
      },
      {
        "Id": 2003,
        "Name": "Justificación retraso por tráfico",
        "Description": "Regla para validar retrasos por incidencias de tráfico",
        "Type": 2,
        "TypeDescription": "Justification",
        "GroupId": 2,
        "Conditions": [],
        "Actions": [],
        "XmlDefinition": "<?xml version=\"1.0\"?><roCollection version=\"2.0\"><Item key=\"Incidence\" type=\"2\">1031</Item><Item key=\"Zone\" type=\"2\">-1</Item><Item key=\"ConditionValueType\" type=\"2\">0</Item><Item key=\"FromTime\" type=\"7\">00:00:00</Item><Item key=\"ToTime\" type=\"7\">23:59:00</Item><Item key=\"FromValueUserField\" type=\"8\"></Item><Item key=\"ToValueUserField\" type=\"8\"></Item><Item key=\"BetweenValueUserField\" type=\"8\"></Item><Item key=\"Cause\" type=\"2\">1</Item><Item key=\"ActionValueType\" type=\"2\">0</Item><Item key=\"MaxTime\" type=\"7\">23:59:00</Item><Item key=\"MaxValueUserField\" type=\"8\"></Item></roCollection>",
        "Tags": ["Retraso", "Tráfico"],
        "BeginDate": "2023-01-01T00:00:00",
        "EndDate": null
      }
    ],
    "BeginDate": "2023-01-01T00:00:00",
    "EndDate": null
  },
  {
    "Id": 3,
    "Name": "Grupo de Reglas para Administración",
    "Description": "Reglas aplicables al personal administrativo",
    "EmployeeContext": null,
    "Shifts": [
      {
        "Id": 301,
        "Name": "Jornada Continua"
      },
      {
        "Id": 302,
        "Name": "Jornada Partida"
      }
    ],
    "Rules": [
      {
        "Id": 3001,
        "Name": "Flexibilidad horaria",
        "Description": "Permite flexibilidad en la entrada de hasta 30 minutos",
        "Type": 3,
        "TypeDescription": "Daily",
        "GroupId": 3,
        "Conditions": [],
        "Actions": [],
        "XmlDefinition": "<?xml version=\"1.0\"?><roCollection version=\"2.0\"><Item key=\"Name\" type=\"8\">Test</Item><Item key=\"Description\" type=\"8\"></Item><Item key=\"DayValidationRule\" type=\"2\">0</Item><Item key=\"PreviousShiftValidationRule\" type=\"8\">0</Item><Item key=\"RuleType\" type=\"8\">3</Item><Item key=\"TotalConditions\" type=\"2\">2</Item><Item key=\"ConditionCauses_1\" type=\"8\">93_0</Item><Item key=\"ConditionTimeZones_1\" type=\"8\">-1</Item><Item key=\"Compare_1\" type=\"2\">0</Item><Item key=\"Type_1\" type=\"2\">0</Item><Item key=\"FromValue_1\" type=\"8\">0:00</Item><Item key=\"ToValue_1\" type=\"8\">00:00</Item><Item key=\"UserField_1\" type=\"8\"></Item><Item key=\"CompareCauses_1\" type=\"8\"></Item><Item key=\"CompareTimeZones_1\" type=\"8\"></Item><Item key=\"ConditionCauses_2\" type=\"8\">61_0</Item><Item key=\"ConditionTimeZones_2\" type=\"8\">-1</Item><Item key=\"Compare_2\" type=\"2\">0</Item><Item key=\"Type_2\" type=\"2\">0</Item><Item key=\"FromValue_2\" type=\"8\">0:00</Item><Item key=\"ToValue_2\" type=\"8\">00:00</Item><Item key=\"UserField_2\" type=\"8\"></Item><Item key=\"CompareCauses_2\" type=\"8\"></Item><Item key=\"CompareTimeZones_2\" type=\"8\"></Item><Item key=\"TotalActions\" type=\"2\">1</Item><Item key=\"Action_1\" type=\"2\">0</Item><Item key=\"CarryOverAction_1\" type=\"2\">0</Item><Item key=\"CarryOverDirectValue_1\" type=\"8\">0:00</Item><Item key=\"CarryOverUserFieldValue_1\" type=\"8\"></Item><Item key=\"CarryOverConditionPart_1\" type=\"2\">0</Item><Item key=\"CarryOverConditionNumber_1\" type=\"2\">0</Item><Item key=\"CarryOverActionResult_1\" type=\"2\">0</Item><Item key=\"CarryOverDirectValueResult_1\" type=\"8\">00:00</Item><Item key=\"CarryOverUserFieldValueResult_1\" type=\"8\"></Item><Item key=\"CarryOverConditionPartResult_1\" type=\"2\">0</Item><Item key=\"CarryOverConditionNumberResult_1\" type=\"2\">0</Item><Item key=\"CarryOverIDCauseFrom_1\" type=\"2\">93</Item><Item key=\"CarryOverIDCauseTo_1\" type=\"2\">137</Item><Item key=\"PlusIDCause_1\" type=\"2\">0</Item><Item key=\"PlusAction_1\" type=\"2\">0</Item><Item key=\"PlusDirectValue_1\" type=\"8\">00:00</Item><Item key=\"PlusUserFieldValue_1\" type=\"8\"></Item><Item key=\"PlusConditionPart_1\" type=\"2\">0</Item><Item key=\"PlusConditionNumber_1\" type=\"2\">0</Item><Item key=\"PlusActionResult_1\" type=\"2\">0</Item><Item key=\"PlusDirectValueResult_1\" type=\"8\">00:00</Item><Item key=\"PlusUserFieldValueResult_1\" type=\"8\"></Item><Item key=\"PlusConditionPartResult_1\" type=\"2\">0</Item><Item key=\"PlusConditionNumberResult_1\" type=\"2\">0</Item><Item key=\"PlusActionSign_1\" type=\"2\">0</Item><Item key=\"CarryOverSingleCause_1\" type=\"8\">0</Item><Item key=\"ActionCauses_1\" type=\"8\"></Item></roCollection>",
        "Tags": ["Flexibilidad", "Entrada"],
        "BeginDate": "2023-01-01T00:00:00",
        "EndDate": null
      },
      {
        "Id": 3002,
        "Name": "Justificación por asuntos personales",
        "Description": "Permite justificar ausencias por asuntos personales",
        "Type": 2,
        "TypeDescription": "Justification",
        "GroupId": 3,
        "Conditions": [],
        "Actions": [],
        "XmlDefinition": "<?xml version=\"1.0\"?><roCollection version=\"2.0\"><Item key=\"Incidence\" type=\"2\">1031</Item><Item key=\"Zone\" type=\"2\">-1</Item><Item key=\"ConditionValueType\" type=\"2\">0</Item><Item key=\"FromTime\" type=\"7\">00:00:00</Item><Item key=\"ToTime\" type=\"7\">23:59:00</Item><Item key=\"FromValueUserField\" type=\"8\"></Item><Item key=\"ToValueUserField\" type=\"8\"></Item><Item key=\"BetweenValueUserField\" type=\"8\"></Item><Item key=\"Cause\" type=\"2\">1</Item><Item key=\"ActionValueType\" type=\"2\">0</Item><Item key=\"MaxTime\" type=\"7\">23:59:00</Item><Item key=\"MaxValueUserField\" type=\"8\"></Item></roCollection>",
        "Tags": ["Personal", "Ausencia"],
        "BeginDate": "2023-01-01T00:00:00",
        "EndDate": null
      }
    ],
    "BeginDate": "2023-01-01T00:00:00",
    "EndDate": "2025-12-31T00:00:00"
  }
]')
GO

UPDATE sysroParameters SET Data='0' WHERE ID='DXVersion'
GO

UPDATE sysroParameters SET Data='1033' WHERE ID='DBVersion'
GO
